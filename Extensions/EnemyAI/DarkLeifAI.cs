using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace BFPlus.Extensions.EnemyAI
{
    public class DarkLeifAI : AI
    {
        enum Attacks
        {
            FrigidCoffin,
            Icefall,
            IceRain,
            BubbleShieldAll,
            FrostRelay,
            Relay,
            MiracleShake,
            FrozenDrill,
            FrostBowling,
            QueenDinner,
            Empower,
            Fortify,
            ChargeUp,
            Enfeeble,
            Break
        }
        public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
        {
            List<Attacks> chances = new List<Attacks>
            {
                Attacks.FrigidCoffin,Attacks.FrigidCoffin,
                Attacks.Icefall,Attacks.Icefall,Attacks.Icefall,Attacks.Icefall,
                Attacks.IceRain,Attacks.IceRain,Attacks.IceRain,
                Attacks.Enfeeble,Attacks.Break
            };

            var battle = MainManager.battle;

            int viIndex = battle.EnemyInField((int)NewEnemies.DarkVi);
            int kabbuIndex = battle.EnemyInField((int)NewEnemies.DarkKabbu);

            float hpPercent = battle.HPPercent(battle.enemydata[actionid]);

            //frost relay
            if (viIndex > -1 && hpPercent <= 0.7f && BattleControl_Ext.CanBeRelayed(battle.enemydata[viIndex]))
            {
                chances.AddRange(new Attacks[] { Attacks.FrostRelay, Attacks.FrostRelay, Attacks.FrostRelay, Attacks.FrostRelay, Attacks.FrostRelay });
            }

            //queens dinner
            if (DarkTeamSnakemouth.HasItem((int)MainManager.Items.KingDinner))
            {
                for (int i = 0; i != battle.enemydata.Length; i++)
                {
                    float hp = battle.HPPercent(battle.enemydata[i]);
                    if (hp <= 0.6f)
                    {
                        chances.AddRange(new Attacks[] { Attacks.QueenDinner, Attacks.QueenDinner, Attacks.QueenDinner, Attacks.QueenDinner });
                    }
                }
            }

            //frozen Drill
            if (kabbuIndex > -1 && hpPercent <= 0.7f && BattleControl_Ext.CanBeRelayed(battle.enemydata[kabbuIndex]))
            {
                chances.AddRange(new Attacks[] { Attacks.FrozenDrill, Attacks.FrozenDrill, Attacks.FrozenDrill, Attacks.FrozenDrill, Attacks.FrozenDrill });
            }

            //frost bowling
            if (viIndex > -1 && kabbuIndex > -1 && hpPercent <= 0.7f && BattleControl_Ext.CanBeRelayed(battle.enemydata[kabbuIndex]) && BattleControl_Ext.CanBeRelayed(battle.enemydata[viIndex]))
            {
                chances.AddRange(new Attacks[] { Attacks.FrostBowling, Attacks.FrostBowling, Attacks.FrostBowling });
            }

            //relay 
            if (BattleControl_Ext.Instance.CanRelay(entity, battle) && !DarkTeamSnakemouth.relayedThisTurn[2])
            {
                chances.AddRange(new Attacks[] { Attacks.Relay, Attacks.Relay });

                for (int i = 0; i != battle.enemydata.Length; i++)
                {
                    if (battle.enemydata[i].charge > 2 || MainManager.HasCondition(MainManager.BattleCondition.AttackUp, battle.enemydata[i]) > -1)
                    {
                        chances.AddRange(new Attacks[] { Attacks.Relay, Attacks.Relay });
                    }
                }
            }

            if (MainManager.HasCondition(MainManager.BattleCondition.AttackUp, battle.enemydata[actionid]) == -1)
            {
                chances.AddRange(new Attacks[] { Attacks.Empower, Attacks.Empower });
            }

            if (MainManager.HasCondition(MainManager.BattleCondition.Shield, battle.enemydata[actionid]) == -1)
            {
                chances.AddRange(new Attacks[] { Attacks.BubbleShieldAll, Attacks.BubbleShieldAll });
            }

            if (MainManager.HasCondition(MainManager.BattleCondition.DefenseUp, battle.enemydata[actionid]) == -1)
            {
                chances.AddRange(new Attacks[] { Attacks.Fortify, Attacks.Fortify });
            }

            if (battle.enemydata[actionid].charge < 3)
            {
                chances.AddRange(new Attacks[] { Attacks.ChargeUp, Attacks.ChargeUp });
            }

            Attacks action = chances[UnityEngine.Random.Range(0, chances.Count)];
            if (DarkTeamSnakemouth.CanUseMiracleShake())
                action = Attacks.MiracleShake;

            EntityControl[] entities;
            MainManager.BattleData[] battleDatas;
            switch (action)
            {
                case Attacks.FrigidCoffin:
                    yield return DoFrigidCoffin(entity, actionid, battle);
                    break;
                case Attacks.Icefall:
                    yield return DoIcefall(entity, actionid, battle);
                    break;
                case Attacks.IceRain:
                    yield return DoIceRain(entity, actionid, battle);
                    break;
                case Attacks.BubbleShieldAll:
                    yield return DoBubbleShieldAll(entity, battle);
                    break;
                case Attacks.FrostRelay:
                    entities = new EntityControl[] { battle.enemydata[viIndex].battleentity, battle.enemydata[actionid].battleentity };
                    yield return DarkTeamSnakemouth.DoFrostRelay(battle, entities, actionid);
                    break;
                case Attacks.Relay:
                    yield return battle.EnemyRelay(entity, BattleControl_Ext.Instance.FindRelayable(entity, battle));
                    DarkTeamSnakemouth.relayedThisTurn[2] = true;
                    break;
                case Attacks.MiracleShake:
                    yield return DarkTeamSnakemouth.UseItem((int)MainManager.Items.MiracleShake,entity, actionid);
                    break;
                case Attacks.FrozenDrill:
                    entities = new EntityControl[] { battle.enemydata[kabbuIndex].battleentity, battle.enemydata[actionid].battleentity };
                    yield return DarkTeamSnakemouth.DoFrozenDrill(battle.enemydata[actionid].battleentity, battle, entities, actionid);
                    break;
                case Attacks.FrostBowling:
                    battleDatas = new MainManager.BattleData[] { battle.enemydata[viIndex], battle.enemydata[kabbuIndex], battle.enemydata[actionid] };
                    yield return DarkTeamSnakemouth.DoFrostBowling(actionid, battle, battleDatas);
                    break;
                case Attacks.QueenDinner:
                    yield return DarkTeamSnakemouth.UseItem((int)MainManager.Items.KingDinner, entity, actionid);
                    break;
                case Attacks.Empower:
                    yield return DoLeifBuffAll(entity, battle, LeifSpell.Empower);
                    break;
                case Attacks.Fortify:
                    yield return DoLeifBuffAll(entity, battle, LeifSpell.Fortify);
                    break;
                case Attacks.ChargeUp:
                    yield return DoLeifBuffAll(entity, battle, LeifSpell.ChargeUp);
                    break;
                case Attacks.Enfeeble:
                    yield return DoLeifDebuff(entity, battle, LeifSpell.Enfeeble);
                    break;
                case Attacks.Break:
                    yield return DoLeifDebuff(entity, battle, LeifSpell.Break);
                    break;
            }
        }

        public IEnumerator DoLeifBuffAll(EntityControl entity, BattleControl battle, LeifSpell spell)
        {
            battle.dontusecharge = true;
            MainManager.battle.StartCoroutine(battle.ItemSpinAnim(entity.transform.position + Vector3.up, MainManager.itemsprites[1, (int)Medal.Blightfury], true));

            var targets = new List<int>();
            for (int i = 0; i != battle.enemydata.Length; i++)
            {
                if (battle.enemydata[i].hp > 0)
                {
                    targets.Add(i);
                }
            }
            MainManager.PlaySound("Magic");
            GameObject magicParticles = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/MagicUp"), entity.transform.position + new Vector3(0f, 0.5f), Quaternion.identity) as GameObject;
            entity.animstate = 4;
            entity.spin = new Vector3(0f, -20f, 0f);

            for (int i = 0; i < targets.Count; i++)
            {
                battle.enemydata[targets[i]].battleentity.spin = new Vector3(0f, -15f, 0f);
            }
            yield return new WaitForSeconds(0.75f);
            magicParticles.transform.position = new Vector3(0f, 999f);
            UnityEngine.Object.Destroy(magicParticles, 5f);
            for (int i = 0; i < targets.Count; i++)
            {
                switch (spell)
                {
                    case LeifSpell.ChargeUp:
                        MainManager.PlaySound("StatUp", -1, 1.25f, 1f);
                        battle.StartCoroutine(battle.StatEffect(battle.enemydata[targets[i]].battleentity, 4));

                        int maxCharge = battle.enemydata[targets[i]].animid == (int)NewEnemies.DarkVi ? 5 : 3;

                        if (battle.enemydata[targets[i]].charge < maxCharge)
                        {
                            battle.enemydata[targets[i]].charge++;

                            if(maxCharge == 5 && battle.enemydata[targets[i]].charge > 3)
                            {
                                battle.StartCoroutine(battle.ItemSpinAnim(entity.transform.position + Vector3.up, MainManager.itemsprites[1, (int)Medal.Powerbank], true));
                            }
                        }
                        break;
                    case LeifSpell.Empower:
                        MainManager.PlaySound("StatUp");
                        MainManager.SetCondition(MainManager.BattleCondition.AttackUp, ref battle.enemydata[targets[i]], 4);
                        battle.StartCoroutine(battle.StatEffect(battle.enemydata[targets[i]].battleentity, 0));
                        break;
                    case LeifSpell.Fortify:
                        MainManager.PlaySound("StatUp", -1, 0.9f, 1f);
                        MainManager.SetCondition(MainManager.BattleCondition.DefenseUp, ref battle.enemydata[targets[i]], 4);
                        battle.StartCoroutine(battle.StatEffect(battle.enemydata[targets[i]].battleentity, 1));
                        break;
                }
                battle.enemydata[targets[i]].battleentity.spin = Vector3.zero;
                DoBlightfurry(entity, ref battle.enemydata[targets[i]], true);
            }
        }

        public IEnumerator DoLeifDebuff(EntityControl entity, BattleControl battle, LeifSpell spell)
        {
            battle.dontusecharge = true;
            entity.animstate = 111;
            MainManager.PlaySound("FastWoosh");
            MainManager.battle.StartCoroutine(battle.ItemSpinAnim(entity.transform.position + Vector3.up, MainManager.itemsprites[1, (int)Medal.Blightfury], true));

            yield return EventControl.halfsec;
            var aliveplayers = new List<int>();
            for (int i = 0; i != MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.instance.playerdata[i].hp > 0)
                    aliveplayers.Add(i);
            }
            SpriteBounce[] spriteBounces = new SpriteBounce[aliveplayers.Count];
            GameObject[] particles = new GameObject[spriteBounces.Length];
            Vector3[] rotaterLocalScales = new Vector3[aliveplayers.Count];
            for (int i = 0; i < aliveplayers.Count; i++)
            {
                switch (spell)
                {
                    case LeifSpell.Break:
                        MainManager.SetCondition(MainManager.BattleCondition.DefenseDown, ref MainManager.instance.playerdata[aliveplayers[i]], 4);
                        battle.StartCoroutine(battle.StatEffect(MainManager.instance.playerdata[aliveplayers[i]].battleentity, 3));
                        break;
                    case LeifSpell.Enfeeble:
                        MainManager.SetCondition(MainManager.BattleCondition.AttackDown, ref MainManager.instance.playerdata[aliveplayers[i]], 4);
                        battle.StartCoroutine(battle.StatEffect(MainManager.instance.playerdata[aliveplayers[i]].battleentity, 2));
                        break;
                }
                rotaterLocalScales[i] = MainManager.instance.playerdata[aliveplayers[i]].battleentity.rotater.localScale;
                spriteBounces[i] = MainManager.instance.playerdata[aliveplayers[i]].battleentity.rotater.gameObject.AddComponent<SpriteBounce>();
                spriteBounces[i].speed = 20f;
                MainManager.instance.playerdata[aliveplayers[i]].battleentity.animstate = 30;
                DoBlightfurry(entity, ref MainManager.instance.playerdata[aliveplayers[i]], true);
            }
            MainManager.PlaySound("StatDown");

            for (int i = 0; i < spriteBounces.Length; i++)
            {
                particles[i] = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/MagicUp"), spriteBounces[i].transform.GetChild(0).position + new Vector3(0f, 0.5f), Quaternion.identity) as GameObject;
            }
            yield return new WaitForSeconds(0.85f);
            for (int i = 0; i < spriteBounces.Length; i++)
            {
                UnityEngine.Object.Destroy(spriteBounces[i]);
                yield return null;
                MainManager.instance.playerdata[aliveplayers[i]].battleentity.rotater.localScale = rotaterLocalScales[i];
                yield return EventControl.tenthsec;
                particles[i].transform.position = new Vector3(0f, 999f);
                UnityEngine.Object.Destroy(particles[i], 5f);
            }
            yield return EventControl.halfsec;
        }

        public IEnumerator DoFrigidCoffin(EntityControl entity, int actionid, BattleControl battle)
        {
            battle.nonphyscal = true;
            battle.GetSingleTarget();

            var playerTargetEntityRef = battle.playertargetentity;
            var playerTargetIDRef = battle.playertargetID;

            MainManager.instance.camtargetpos = Vector3.Lerp(entity.transform.position, playerTargetEntityRef.transform.position, 0.5f);
            MainManager.instance.camspeed = 0.01f;
            MainManager.instance.camoffset = new Vector3(0f, 2.65f, -7f);
            MainManager.PlaySound("IceMothThrow");
            entity.animstate = 108;
            GameObject mothBattleSphere = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/AnimSpecific/mothbattlesphere"), entity.transform.position + new Vector3(-0.75f, 1.55f), Quaternion.Euler(90f, 0f, 0f)) as GameObject;
            MeshRenderer singleSphere = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Objects/SingleSphere"), Vector3.zero, Quaternion.identity, mothBattleSphere.transform) as GameObject).GetComponent<MeshRenderer>();
            float a = 0f;
            float b = 30f;
            do
            {
                mothBattleSphere.transform.position = MainManager.BeizierCurve3(entity.transform.position + new Vector3(1.1f, 1.85f, -0.1f), entity.transform.position + new Vector3(-2f, -0.5f), 5f, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b + 1f);
            singleSphere.transform.parent = null;

            UnityEngine.Object.Destroy(mothBattleSphere.gameObject);
            Vector3 spherePos = new Vector3(singleSphere.transform.position.x, 0f, entity.transform.position.z);
            a = 0f;
            b = 80f;
            MainManager.PlaySound("WatcherIce");
            do
            {
                singleSphere.transform.position = MainManager.SmoothLerp(spherePos, playerTargetEntityRef.transform.position, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b + 1f);
            UnityEngine.Object.Destroy(singleSphere.gameObject);

            singleSphere.transform.localPosition = Vector3.zero;
            singleSphere.material.color = new Color(1f, 1f, 1f, 0.5f);
 
            BattleControl.SetDefaultCamera();
            MainManager.instance.camspeed = 0.3f;
            UnityEngine.Object.Destroy(UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/mothicenormal"), playerTargetEntityRef.transform.position + Vector3.up / 2f, Quaternion.Euler(-90f, 0f, 0f)) as GameObject, 2f);
            GameObject icePillar = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Objects/icepillar"), playerTargetEntityRef.transform.position, Quaternion.Euler(-90f, 0f, 0f)) as GameObject;
            DialogueAnim icePillarAnim = icePillar.AddComponent<DialogueAnim>();
            icePillarAnim.targetscale = new Vector3(0.5f, 0.5f, 1f);
            icePillar.transform.localScale = Vector3.zero;
            entity.animstate = 111;
            MainManager.PlaySound("IceMothHit");
            int bs = playerTargetEntityRef.basestate;
            icePillarAnim.targetscale = Vector3.one * (MainManager.instance.playerdata[playerTargetIDRef].size / 2f);
            MainManager.instance.playerdata[playerTargetIDRef].battleentity.overrideanim = true;
            MainManager.instance.playerdata[playerTargetIDRef].battleentity.animstate = 11;
            playerTargetEntityRef.digging = false;
            yield return EventControl.halfsec;
            entity.animstate = 105;
            icePillar.transform.parent = playerTargetEntityRef.sprite.transform;

            float startTime = 0f;
            float endTime = 15f;
            Vector3 b46 = new Vector3(playerTargetEntityRef.transform.position.x, 1.5f * Mathf.Clamp01(1f - MainManager.instance.playerdata[playerTargetIDRef].weight / 100f), playerTargetEntityRef.transform.position.z);
            Vector3 h3 = playerTargetEntityRef.transform.position;
            playerTargetEntityRef.LockRigid(value: true);
            do
            {
                playerTargetEntityRef.spin = Vector3.up * 10f * (startTime / endTime) * (1f - MainManager.instance.playerdata[playerTargetIDRef].weight / 100f);
                playerTargetEntityRef.transform.position = Vector3.Lerp(h3, b46, startTime / endTime);
                startTime += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (startTime < endTime);
            entity.animstate = 102;
            yield return EventControl.sec;
            entity.animstate = 116;
            yield return null;

            battle.DoDamage(battle.enemydata[actionid], ref MainManager.instance.playerdata[playerTargetIDRef], DarkTeamSnakemouth.darkLeifBaseDamage + 2, BattleControl.AttackProperty.Magic, null, battle.commandsuccess);
            if (!battle.commandsuccess)
            {
                MainManager.SetCondition(MainManager.BattleCondition.Freeze, ref MainManager.instance.playerdata[playerTargetIDRef], MainManager.ConditionTurns(entity.animid, 2));
                MainManager.instance.playerdata[playerTargetIDRef].cantmove = 1;
                playerTargetEntityRef.Freeze();
                DoBlightfurry(entity, ref MainManager.instance.playerdata[playerTargetIDRef]);
            }
            UnityEngine.Object.Destroy(icePillar.gameObject);
            MainManager.ShakeScreen(Vector3.one * 0.1f, 0.1f);
            MainManager.PlayParticle("IceShatter", "IceBreak", playerTargetEntityRef.sprite.transform.position + new Vector3(0f, 1.25f));
            yield return EventControl.halfsec;
            playerTargetEntityRef.spin = Vector3.zero;
            startTime = 0f;
            endTime = 10f;
            do
            {
                playerTargetEntityRef.transform.position = Vector3.Lerp(b46, h3, startTime / endTime);
                startTime += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (startTime < endTime);
            playerTargetEntityRef.StopAllCoroutines();
            entity.animstate = 118;

            yield return EventControl.halfsec;
            playerTargetEntityRef.LockRigid(value: false);
            playerTargetEntityRef.overrideanim = false;
            playerTargetEntityRef.basestate = bs;
            playerTargetEntityRef.onground = false;
            if (icePillar != null)
            {
                icePillar.GetComponent<DialogueAnim>().shrink = true;
                icePillar.GetComponent<DialogueAnim>().shrinkspeed = 0.05f;
            }
        }

        public IEnumerator DoIcefall(EntityControl entity, int actionid, BattleControl battle)
        {
            battle.nonphyscal = true;
            entity.animstate = 105;

            GameObject icecle = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Objects/icecle")) as GameObject;
            icecle.transform.position = entity.transform.position + new Vector3(-2f, 4.5f);
            icecle.transform.localScale = Vector3.zero;
            UnityEngine.Object.Destroy(icecle.GetComponent<BoxCollider>());

            float a = 0f;
            MainManager.PlaySound("Spin", 8, 0f, 1f, true);
            while (a < 60f)
            {
                icecle.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.25f, a / 60f);
                icecle.transform.eulerAngles += Vector3.up * 10f * (a / 60f);
                MainManager.sounds[8].pitch = a / 60f;
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            SpriteRenderer cursor = MainManager.NewUIObject("t", null, Vector3.zero, Vector3.one, MainManager.guisprites[41]).GetComponent<SpriteRenderer>();
            cursor.gameObject.layer = 15;

            a = 0f;
            float b = 80f;
            entity.animstate = 106;
            MainManager.PlaySound("Crosshair", 9, 0.9f, 0.35f, true);
            do
            {
                icecle.transform.eulerAngles += Vector3.up * 10f;
                cursor.transform.eulerAngles += new Vector3(0f, 0f, 5f * MainManager.TieFramerate(1f));
                cursor.transform.position = MainManager.SmoothLerp(new Vector3(0f, 3f), new Vector3(-4.5f, 1f), a / b) + new Vector3(0f, Mathf.Sin(Time.time * 2f) * (3f * (1f - a / b)));
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b + 1f);

            MainManager.StopSound(8);
            MainManager.StopSound(9);
            MainManager.PlaySound("IceMothThrow");
            entity.animstate = 107;
            UnityEngine.Object.Destroy(cursor.gameObject);

            a = 0f;
            b = 25f;
            icecle.transform.eulerAngles = new Vector3(0f, 0f, -45f);
            Vector3 startPos = icecle.transform.position;
            do
            {
                icecle.transform.position = Vector3.Lerp(startPos, new Vector3(-4.5f, 1f), a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b + 1f);

            MainManager.PlaySound("IceMothHit");
            MainManager.PlayParticle("mothicenormal", icecle.transform.position).transform.localScale = Vector3.one * 2f;
            UnityEngine.Object.Destroy(icecle.gameObject);
            battle.PartyDamage(actionid, DarkTeamSnakemouth.darkLeifBaseDamage, new BattleControl.AttackProperty?(BattleControl.AttackProperty.Freeze), battle.commandsuccess);

            if (!battle.commandsuccess)
            {
                for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                {
                    DoBlightfurry(entity, ref MainManager.instance.playerdata[i]);
                }
            }
            
            yield return EventControl.halfsec;
        }

        public IEnumerator DoBubbleShieldAll(EntityControl entity, BattleControl battle)
        {
            battle.dontusecharge = true;
            MainManager.instance.camtargetpos = entity.transform.position + new Vector3(2f, 0f);
            MainManager.instance.camspeed = 0.01f;
            MainManager.instance.camoffset = new Vector3(0f, 2.65f, -7f);
            entity.animstate = 102;
            yield return new WaitForSeconds(0.75f);
            entity.animstate = 119;
            yield return EventControl.quartersec;
            BattleControl.SetDefaultCamera();
            MainManager.PlaySound("Shield");
            for (int i = 0; i < battle.enemydata.Length; i++)
            {
                if (battle.enemydata[i].hp > 0)
                {
                    battle.enemydata[i].battleentity.CreateShield();
                    MainManager.SetCondition(MainManager.BattleCondition.Shield, ref battle.enemydata[i], 2);
                    DoBlightfurry(entity, ref battle.enemydata[i]);
                }
            }
            yield return new WaitForSeconds(0.75f);
        }

        public IEnumerator DoIceRain(EntityControl entity, int actionid, BattleControl battle)
        {
            battle.nonphyscal = true;
            entity.animstate = 105;
            yield return EventControl.halfsec;
            Vector3 startpos = MainManager.instance.playerdata[battle.partypointer[1]].battleentity.transform.position;
            int icecleAmount = 4;
            GameObject[] icecles = new GameObject[icecleAmount];
            List<Transform> crosshairs = new List<Transform>();
            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.instance.playerdata[i].hp > 0)
                {
                    crosshairs.Add(battle.TempCrosshair(MainManager.instance.playerdata[i], false));
                }
            }
            for (int i = 0; i < icecleAmount; i++)
            {
                if (MainManager.GetAlivePlayerAmmount() == 0)
                    break;
                MainManager.PlaySound("Crosshair", 9, 0.9f, 0.35f, loop: true);
                entity.animstate = 105;
                yield return EventControl.sec;
                MainManager.StopSound(9);
                icecles[i] = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Objects/icecle"), new Vector3(startpos.x + 0.5f, 15f, 0f), Quaternion.identity) as GameObject;
                battle.StartCoroutine(DoIceRainDamage(startpos.x + 0.5f, icecles[i].transform, battle, actionid, DarkTeamSnakemouth.darkLeifBaseDamage+1, entity));
                entity.animstate = 100;
                yield return EventControl.halfsec;
            }
            for (int i = 0; i < crosshairs.Count; i++)
            {
                UnityEngine.Object.Destroy(crosshairs[i].gameObject);
            }
            entity.animstate = 102;
            while (!MainManager.ArrayIsEmpty(icecles))
            {
                yield return null;
            }
            yield return EventControl.halfsec;
        }

        public IEnumerator DoIceRainDamage(float targetPos, Transform icecle, BattleControl battle, int actionid, int damage, EntityControl entity)
        {
            float startTime = 0f;
            float endTime = 45f;
            float startpos = targetPos;

            Vector3 tpos = new Vector3(startpos, 0f);
            Vector3 spos = icecle.transform.position;
            int lockedat = -1;
            Vector3 epos = Vector3.zero;
            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                epos = battle.CenterPos(MainManager.instance.playerdata[i], true);
                if (MainManager.GetDistance(epos.x, startpos) < 1f)
                {
                    lockedat = i;
                    break;
                }
            }
            do
            {
                float framestep = MainManager.TieFramerate(1f);
                icecle.transform.position = Vector3.Lerp(spos, tpos, startTime / endTime);
                icecle.transform.eulerAngles += new Vector3(0f, framestep * 20f);
                if (lockedat > -1 && icecle.transform.position.y < epos.y)
                {
                    tpos = icecle.transform.position;
                    break;
                }
                startTime += framestep;
                yield return null;
            }
            while (startTime < endTime + 1f);
            MainManager.PlayParticle("mothicenormal", tpos + Vector3.up).transform.localScale = Vector3.one * 2f;

            for (int j = 0; j < MainManager.instance.playerdata.Length; j++)
            {
                if (MainManager.instance.playerdata[j].hp > 0 && MainManager.instance.playerdata[j].position != BattleControl.BattlePosition.OutOfReach && battle.IsInRadius(tpos, MainManager.instance.playerdata[j], 2f))
                {
                    battle.DoDamage(battle.enemydata[actionid], ref MainManager.instance.playerdata[j], damage, BattleControl.AttackProperty.Freeze, null, battle.commandsuccess);
                    if (!battle.commandsuccess)
                    {
                        DoBlightfurry(entity, ref MainManager.instance.playerdata[j]);
                    }
                }
            }
            icecle.transform.position = new Vector3(0f, -999f);
            yield return EventControl.halfsec;
            UnityEngine.Object.Destroy(icecle.gameObject);
        }

        public void DoBlightfurry(EntityControl entity, ref MainManager.BattleData target, bool nospin=false)
        {
            BattleControl_Ext.Instance.DoPoison(ref target);

            if(!nospin)
                MainManager.battle.StartCoroutine(MainManager.battle.ItemSpinAnim(entity.transform.position + Vector3.up, MainManager.itemsprites[1, (int)Medal.Blightfury], true));
        }
    }
}
