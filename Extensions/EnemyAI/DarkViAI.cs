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
    public class DarkViAI : AI
    {
        enum Attacks
        {
            BasicAttack,
            TornadoToss,
            HurricaneToss,
            NeedleToss,
            FlyDrop,
            SharingStash,
            HardCharge,
            MiracleShake,
            FrostRelay,
            FrostBowling,
            QueenDinner
        }
        public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
        {
            List<Attacks> chances = new List<Attacks>
            {
                Attacks.BasicAttack,Attacks.BasicAttack,
                Attacks.TornadoToss,Attacks.TornadoToss,
                Attacks.HurricaneToss,Attacks.HurricaneToss,Attacks.HurricaneToss,
                Attacks.NeedleToss,Attacks.NeedleToss,Attacks.NeedleToss,
            };

            var battle = MainManager.battle;
            DarkTeamSnakemouth.SetupFight(actionid);

            int kabbuIndex = battle.EnemyInField((int)NewEnemies.DarkKabbu);
            int leifIndex = battle.EnemyInField((int)NewEnemies.DarkLeif);
            float hpPercent = battle.HPPercent(battle.enemydata[actionid]);

            if (DarkTeamSnakemouth.HasItem((int)MainManager.Items.SquashSoda))
            {
                if (kabbuIndex > -1 && battle.enemydata[kabbuIndex].cantmove <= 0 && MainManager.HasCondition(MainManager.BattleCondition.AttackUp, battle.enemydata[actionid]) == -1 && BattleControl_Ext.CanBeRelayed(battle.enemydata[kabbuIndex]))
                {
                    yield return DarkTeamSnakemouth.UseItem((int)MainManager.Items.SquashSoda, battle.enemydata[kabbuIndex].battleentity, actionid);
                    battle.enemydata[kabbuIndex].cantmove++;
                    MainManager.SetCondition(MainManager.BattleCondition.Poison, ref battle.enemydata[actionid], 4);
                    MainManager.PlayParticle("PoisonEffect", "Poison", battle.enemydata[actionid].battleentity.transform.position);
                    yield return battle.ItemSpinAnim(entity.transform.position + Vector3.up,MainManager.itemsprites[1, 6], true);
                    battle.dontusecharge = false;
                }
            }

            //fly drop
            if (kabbuIndex > -1 && hpPercent <= 0.7f && BattleControl_Ext.CanBeRelayed(battle.enemydata[kabbuIndex]))
            {
                chances.AddRange(new Attacks[] { Attacks.FlyDrop, Attacks.FlyDrop, Attacks.FlyDrop });
            }

            bool hasQueenDinner = DarkTeamSnakemouth.HasItem((int)MainManager.Items.KingDinner);
            //sharing stash
            for (int i = 0; i != battle.enemydata.Length; i++)
            {
                float hp = battle.HPPercent(battle.enemydata[i]);
                if (hp <= 0.6f && MainManager.HasCondition(MainManager.BattleCondition.AttackUp, battle.enemydata[actionid]) == -1)
                {
                    chances.AddRange(new Attacks[] { Attacks.SharingStash, Attacks.SharingStash });
                }

                if (hasQueenDinner)
                {
                    if (hp <= 0.6f && MainManager.HasCondition(MainManager.BattleCondition.AttackUp, battle.enemydata[actionid]) == -1)
                    {
                        chances.AddRange(new Attacks[] { Attacks.QueenDinner, Attacks.QueenDinner });
                    }
                }
            }

            //hardcharge
            if (battle.enemydata[actionid].hp >= 20 && battle.enemydata[actionid].charge == 0)
            {
                chances.AddRange(new Attacks[] { Attacks.HardCharge, Attacks.HardCharge });
            }

            //frost relay
            if (leifIndex > -1 && hpPercent <= 0.7f && BattleControl_Ext.CanBeRelayed(battle.enemydata[leifIndex]))
            {
                chances.AddRange(new Attacks[] { Attacks.FrostRelay, Attacks.FrostRelay, Attacks.FrostRelay, Attacks.FrostRelay });
            }

            //frost bowling
            if (kabbuIndex > -1 && leifIndex > -1 && hpPercent <= 0.7f && BattleControl_Ext.CanBeRelayed(battle.enemydata[kabbuIndex]) && BattleControl_Ext.CanBeRelayed(battle.enemydata[leifIndex]))
            {
                chances.AddRange(new Attacks[] { Attacks.FrostBowling, Attacks.FrostBowling, Attacks.FrostBowling });
            }

            Attacks action = chances[UnityEngine.Random.Range(0, chances.Count)];
            if (DarkTeamSnakemouth.CanUseMiracleShake())
                action = Attacks.MiracleShake;

            EntityControl[] entities;
            MainManager.BattleData[] battleDatas;
            int darkViDmg = DarkTeamSnakemouth.GetDarkViDamage(actionid, battle);
            switch (action)
            {
                case Attacks.BasicAttack:
                    yield return battle.EnemyViRegular(entity, darkViDmg);
                    break;
                case Attacks.TornadoToss:
                    yield return battle.EnemyTornadoToss(entity, darkViDmg);
                    break;
                case Attacks.HurricaneToss:
                    yield return DoHurricaneToss(entity, actionid, battle);
                    break;
                case Attacks.NeedleToss:
                    yield return DoNeedleToss(entity, actionid, battle);
                    break;
                case Attacks.FlyDrop:
                    entities = new EntityControl[] { battle.enemydata[actionid].battleentity, battle.enemydata[kabbuIndex].battleentity };
                    yield return DarkTeamSnakemouth.DoFlyDrop(battle, entities, actionid);
                    break;
                case Attacks.SharingStash:
                    yield return battle.EnemySharingStash(entity);
                    break;
                case Attacks.HardCharge:
                    battle.StartCoroutine(battle.ItemSpinAnim(entity.transform.position + Vector3.up, MainManager.itemsprites[1, (int)Medal.Powerbank], true));
                    yield return BattleControl_Ext.Instance.DoHardCharge(entity, actionid, battle,5,9);
                    break;
                case Attacks.MiracleShake:
                    yield return DarkTeamSnakemouth.UseItem((int)MainManager.Items.MiracleShake, entity, actionid);
                    break;
                case Attacks.FrostRelay:
                    entities = new EntityControl[] { battle.enemydata[actionid].battleentity, battle.enemydata[leifIndex].battleentity };
                    yield return DarkTeamSnakemouth.DoFrostRelay(battle, entities, actionid);
                    break;
                case Attacks.FrostBowling:
                    battleDatas = new MainManager.BattleData[] { battle.enemydata[actionid], battle.enemydata[kabbuIndex], battle.enemydata[leifIndex] };
                    yield return DarkTeamSnakemouth.DoFrostBowling(actionid, battle, battleDatas);
                    break;
                case Attacks.QueenDinner:
                    yield return DarkTeamSnakemouth.UseItem((int)MainManager.Items.KingDinner, entity, actionid);
                    break;
            }
        }

        public IEnumerator DoHurricaneToss(EntityControl entity, int actionid, BattleControl battle)
        {
            battle.nonphyscal = true;
            battle.GetSingleTarget();

            var playerTargetEntityRef = battle.playertargetentity;
            var playerTargetIDRef = battle.playertargetID;

            battle.successfulchain = 4;
            battle.combo = 0;

            int baseState = playerTargetEntityRef.basestate;
            MainManager.instance.camtargetpos = new Vector3(-1.75f, 0f, -0.5f);
            MainManager.instance.camoffset = new Vector3(-0.5f, 2f, -5f);
            MainManager.instance.camspeed = 0.01f;
            entity.animstate = 100;
            yield return new WaitForSeconds(0.3f);
            yield return null;

            ParticleSystem huricaneParticles = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/Hurricane")) as GameObject).GetComponent<ParticleSystem>();
            huricaneParticles.transform.parent = entity.sprite.transform;
            huricaneParticles.transform.localPosition = new Vector3(0f, 0.2f, -0.1f);
            huricaneParticles.transform.localEulerAngles = new Vector3(-90f, 0f);
            ParticleSystem.EmissionModule emission = huricaneParticles.emission;
            ParticleSystem.MainModule mainParticle = huricaneParticles.main;

            yield return null;
            MainManager.PlaySound("Woosh", 9, 0.5f, 0.6f, loop: true);
            yield return null;
            float a = 0;
            while (a < 75f)
            {
                a += MainManager.TieFramerate(1f);
                entity.spin = new Vector3(0f, 10 * (battle.successfulchain + 1), 0f);
                yield return null;
            }
            MainManager.StopSound(9, 0.1f);
            MainManager.PlaySound("Toss");
            MainManager.PlaySound("Woosh", 8, 0.9f, 0.5f, loop: true);

            entity.spin = Vector3.zero;
            entity.animstate = 101;
            entity.LockRigid(value: false);
            GameObject beemerang = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Objects/BeerangBattle") as GameObject);
            beemerang.transform.position = entity.transform.position;
            huricaneParticles.transform.parent = beemerang.transform;

            Vector3 targetPos = playerTargetEntityRef.transform.position + Vector3.up / 2f;
            Vector3 mid = Vector3.Lerp(targetPos, entity.transform.position, 0.5f) + new Vector3(0f, 0f, -2.5f);

            a = 0f;
            float b = 20f;
            BattleControl.SetDefaultCamera();
            do
            {
                beemerang.transform.position = MainManager.BeizierCurve3(entity.transform.position, targetPos, mid, a / b);
                beemerang.transform.Rotate(0f, 0f, MainManager.TieFramerate(20f));
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b);

            int hits = 4;

            huricaneParticles.transform.parent = playerTargetEntityRef.transform;
            battle.enemybounce = new Coroutine[1] { battle.StartCoroutine(battle.BounceEnemy(MainManager.GetPlayerData(playerTargetIDRef), 0, hits, Mathf.Clamp((float)hits * 0.65f, 0f, 3f)))};
            emission.rateOverTime = Mathf.Clamp(50f * 1, 20f, 50f);

            SpinAround spinAround = beemerang.gameObject.AddComponent<SpinAround>();
            spinAround.itself = new Vector3(0f, 0f, 35f);

            GameObject hurr = MainManager.PlayParticle("HurricaneBig", playerTargetEntityRef.transform.position, -1f);
            int firstdamage = DarkTeamSnakemouth.GetDarkViDamage(actionid, battle);
            battle.combo = 0;

            for (int i = 0; i < hits; i++)
            {
                if (i == 0)
                {
                    battle.DoDamage(battle.enemydata[actionid], ref MainManager.instance.playerdata[playerTargetIDRef], firstdamage - 1, BattleControl.AttackProperty.Flip,null, battle.commandsuccess);
                    firstdamage = battle.lastdamage;
                }
                else if (firstdamage > 0)
                {
                    int value3 = firstdamage - i;
                    battle.DoDamage(null, ref MainManager.instance.playerdata[playerTargetIDRef], Mathf.Clamp(value3, 1, 99), BattleControl.AttackProperty.NoExceptions,null, battle.commandsuccess);
                    battle.ShowComboMessage(playerTargetEntityRef, battle.commandsuccess);
                }
                else
                {
                    battle.DoDamage(null, ref MainManager.instance.playerdata[playerTargetIDRef], 0, BattleControl.AttackProperty.NoExceptions,null, battle.commandsuccess);
                }
                if (hits > 1)
                {
                    yield return new WaitForSeconds(0.75f / hits);
                }
            }
            MainManager.DestroyTemp(hurr, 3f);
            UnityEngine.Object.Destroy(spinAround);

            huricaneParticles.transform.position = new Vector3(0f, 999f);
            huricaneParticles.transform.parent = null;
            UnityEngine.Object.Destroy(huricaneParticles.gameObject, 3f);

            mid = Vector3.Lerp(targetPos, entity.transform.position, 0.5f) + new Vector3(0f, 0f, 2.5f);
            a = 0f;
            b = 20f;
            do
            {
                beemerang.transform.Rotate(0f, 0f, 20f);
                beemerang.transform.position = MainManager.BeizierCurve3(targetPos, entity.transform.position + Vector3.up, mid, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b);

            MainManager.StopSound(8);
            UnityEngine.Object.Destroy(beemerang.gameObject);
            entity.animstate = 13;

            while (battle.enemybounce[0] != null)
            {
                yield return null;
            }
            yield return EventControl.quartersec;
            playerTargetEntityRef.basestate = baseState;
        }

        public IEnumerator DoNeedleToss(EntityControl entity, int actionid, BattleControl battle)
        {
            battle.nonphyscal = true;
            
            entity.animstate = 115;
            yield return new WaitForSeconds(0.65f);
            entity.overridefly = true;
            entity.animstate = 117;
            float a = 0f;
            float b = 20f;

            do
            {
                entity.transform.position = Vector3.Lerp(entity.transform.position, new Vector3(3.5f, 0f, -0.65f), a / b);
                entity.height = Mathf.Lerp(0f, 2.75f, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b + 1f);

            int needleAmount = 3;
            int attack = DarkTeamSnakemouth.GetDarkViDamage(actionid, battle);
            for (int j = 0; j < needleAmount; j++)
            {
                if (MainManager.GetAlivePlayerAmmount() == 0)
                {
                    break;
                }
                entity.animstate = 118;
                entity.bobspeed = 0.1f;
                entity.bobrange = 3f;
                MainManager.PlaySound("Crosshair", 9, 0.9f, 0.35f, loop: true);

                battle.GetSingleTarget();
                var playerTarget = MainManager.GetPlayerData(battle.playertargetID);

                Transform crosshair = battle.TempCrosshair(playerTarget, false);
                yield return EventControl.sec;
                MainManager.sounds[9].loop = false;
                entity.overrideanim = true;
                entity.bobrange = 0f;
                entity.bobspeed = 0f;
                entity.animstate = 119;
                MainManager.PlaySound("Toss");
                SpriteRenderer dart = MainManager.NewSpriteObject(entity.sprite.transform.position + new Vector3(1f, 0.5f, -0.1f), null, MainManager.instance.projectilepsrites[2]);
                dart.gameObject.AddComponent<ShadowLite>().SetUp(0.3f, 0.5f);
                Vector3 tae = MainManager.GetDirection(new Vector3(playerTarget.battleentity.transform.position.x, playerTarget.battleentity.transform.position.y - 0.25f), new Vector3(entity.transform.position.x + 1f, entity.transform.position.y + 2.5f));
                Vector3 ds = dart.transform.position;
                Vector3 dtg = ds + tae * 15f + new Vector3(0f, -2f);
                dart.transform.eulerAngles = new Vector3(0f, 0f, -90f - (dtg.y - 3f) * 5f);
                dart.transform.localScale = Vector3.one * 1.15f;

                var properties = new BattleControl.AttackProperty[] { BattleControl.AttackProperty.Numb, BattleControl.AttackProperty.Poison, BattleControl.AttackProperty.Sleep, BattleControl.AttackProperty.Fire, BattleControl.AttackProperty.Freeze };
                battle.StartCoroutine(battle.Projectile
                (
                    attack,
                    properties[UnityEngine.Random.Range(0, properties.Length)],
                    battle.enemydata[actionid],
                    battle.playertargetID,
                    dart.transform,
                    20,
                    0,
                    null,
                    null,
                    null,
                    null,
                    Vector3.zero, 
                    false
                ));
                UnityEngine.Object.Destroy(crosshair.gameObject);
                while (dart != null)
                {
                    yield return null;
                }          
                yield return EventControl.halfsec;
            }
            a = 0f;
            b = 15f;
            float h2 = entity.height;
            do
            {
                entity.height = Mathf.Lerp(h2, 0f, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b + 1f);
            entity.height = 0f;
            entity.overrideanim = false;
            entity.overridefly = false;
        }
    }
}
