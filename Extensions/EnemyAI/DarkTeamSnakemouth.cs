using HarmonyLib;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static BattleControl;

namespace BFPlus.Extensions.EnemyAI
{
    public static class DarkTeamSnakemouth
    {
        public static int darkViBaseDamage = 4;
        public static int darkKabbuBaseDamage = 3;
        public static int darkLeifBaseDamage = 3;
        static List<int> inventory = new List<int>();
        public static bool[] relayedThisTurn = new bool[3];
        public static int GetDarkViDamage(int id, BattleControl instance)
        {
            return darkViBaseDamage + (MainManager.HasCondition(MainManager.BattleCondition.Poison, instance.enemydata[id]) > -1 ? 2 : 0);
        }

        public static void RefreshRelay()
        {
            for (int i = 0; i < relayedThisTurn.Length; i++)
                relayedThisTurn[i] = false;
        }

        public static void SetupFight(int actionid)
        {
            if (MainManager.battle.enemydata[actionid].data == null || MainManager.battle.enemydata[actionid].data[0] == 0)
            {
                inventory = new List<int>()
                {
                    (int)MainManager.Items.KingDinner, (int)MainManager.Items.KingDinner, (int)MainManager.Items.KingDinner,
                    (int)MainManager.Items.MiracleShake,(int)MainManager.Items.MiracleShake,(int)MainManager.Items.MiracleShake,
                    (int)MainManager.Items.SquashSoda, (int)MainManager.Items.SquashSoda, (int)MainManager.Items.SquashSoda,(int)MainManager.Items.SquashSoda
                };

                for (int i = 0; i < MainManager.battle.enemydata.Length; i++)
                {
                    MainManager.battle.SetData(i, 1);
                    MainManager.battle.enemydata[i].data[0] = 1;
                }
            }
        }

        public static IEnumerator UseItem(int itemid, EntityControl entity, int actionid)
        {
            inventory.Remove(itemid);
            yield return BattleControl_Ext.Instance.UseItem(entity, actionid, MainManager.battle, (MainManager.Items)itemid);
        }

        public static bool HasItem(int itemid) => inventory.Contains(itemid);

        public static bool CanUseMiracleShake()
        {
            return MainManager.battle.reservedata.Count != 0 && inventory.Contains((int)MainManager.Items.MiracleShake);
        }


        public static IEnumerator DoFrostRelay(BattleControl battle, EntityControl[] entities, int actionid)
        {
            battle.nonphyscal = true;
            battle.GetSingleTarget();

            var playerTargetIDRef = battle.playertargetID;

            MainManager.instance.camoffset += Vector3.right * 1.5f;
            var startEntitiesPos = new Vector3[2]
            {
                entities[0].transform.position,
                entities[1].transform.position
            };

            int[] atk = new int[2]
            {
                Mathf.CeilToInt(GetDarkViDamage(0,battle) / 1.15f),
                Mathf.CeilToInt(darkLeifBaseDamage / 1.15f)
            };

            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].overrideflip = true;
            }

            entities[0].MoveTowards(new Vector3(2.2f, 0f, -0.9f), 2f, 1, 13);
            entities[1].MoveTowards(new Vector3(3.65f, 0f, -1.55f), 2f, 1, 13);
            while (!MainManager.EntitiesAreNotMoving(entities))
            {
                yield return null;
            }

            entities[0].animstate = 0;
            yield return EventControl.halfsec;
            GameObject beemerang = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Objects/BeerangBattle")) as GameObject;

            float a = 0f;
            float b = 30f;
            Vector3 targetPos = battle.CenterPos(MainManager.instance.playerdata[playerTargetIDRef], true);
            Vector3 startPos = entities[0].transform.position + Vector3.up;

            int times = 6;
            int it = times;
            while (true)
            {
                if (times < 3)
                {
                    for (int i = 0; i < atk.Length; i++)
                    {
                        atk[i] = Mathf.Clamp(atk[i] - 1, 1, 99);
                    }
                    yield return null;
                }
                beemerang.transform.position = new Vector3(0f, -999f);
                entities[0].animstate = 103;
                entities[1].animstate = 111;
                MainManager.PlayParticle("mothicenormal", battle.CenterPos(MainManager.instance.playerdata[playerTargetIDRef], true) - Vector3.up);
                MainManager.PlaySound("IceMothHit");
                battle.DoDamage(battle.enemydata[1], ref MainManager.instance.playerdata[playerTargetIDRef], Mathf.Clamp(atk[1] - (it - times), 0, 99), BattleControl.AttackProperty.Freeze,null, battle.commandsuccess);

                battle.enemydata[2].charge = 0;
                yield return new WaitForSeconds(Mathf.Clamp01(0.5f - battle.combo * 0.025f));
                entities[1].animstate = 102;
                b = Mathf.Clamp(60f - (float)Mathf.Clamp(3 - times, 0, 3) * 13f, 10f, 120f);

                yield return EventControl.tenthsec;
                yield return EventControl.halfsec;
                MainManager.PlaySound("Toss");
                yield return EventControl.tenthsec;
                b /= 1.1f;
                a = 0f;
                MainManager.PlaySound("Toss2", 9, 1f + 0.1f * battle.combo, 1f, loop: true);
                entities[0].animstate = 105;
                do
                {
                    beemerang.transform.position = Vector3.Lerp(startPos, targetPos, a / (b / 1.5f));
                    beemerang.transform.Rotate(0f, 0f, MainManager.framestep * 10f + 2f * battle.combo);
                    a += MainManager.framestep;
                    yield return null;
                }
                while (a < b / 1.5f + 1f);

                battle.DoDamage(battle.enemydata[0], ref MainManager.instance.playerdata[playerTargetIDRef], Mathf.Clamp(atk[0] - (it - times), 0, 99), null, null, battle.commandsuccess);
                battle.enemydata[0].charge = 0;
                yield return EventControl.tenthsec;
                a = 0f;
                do
                {
                    beemerang.transform.position = MainManager.BeizierCurve3(targetPos, startPos, 8f, a / b);
                    beemerang.transform.Rotate(0f, 0f, MainManager.framestep * -10f - 2f * battle.combo);
                    a += MainManager.framestep;
                    yield return null;
                } while (a < b + 1f);
                MainManager.StopSound(9);
                yield return EventControl.tenthsec;
                if (times > 1)
                {
                    MainManager.PlayParticle("mothicenormal", battle.CenterPos(MainManager.instance.playerdata[playerTargetIDRef], true) - Vector3.up).transform.localScale = Vector3.one;
                }
                else
                {
                    entities[0].animstate = 13;
                }
                if (MainManager.instance.playerdata[playerTargetIDRef].hp <= 0)
                {
                    break;
                }
                yield return null;
                times--;
                if (times <= 0)
                {
                    break;
                }
            }

            UnityEngine.Object.Destroy(beemerang.gameObject);
            yield return EventControl.halfsec;
            BattleControl.SetDefaultCamera();
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].overrideflip = true;
                entities[i].MoveTowards(startEntitiesPos[i], 2f);
            }
            while (!MainManager.EntitiesAreNotMoving(entities))
            {
                yield return null;
            }
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].overrideflip = false;
                entities[i].flip = false;
                entities[i].transform.position = startEntitiesPos[i];
                if (i != actionid)
                    battle.enemydata[entities[i].battleid].cantmove++;
            }
            battle.UpdateAnim();
        }
        public static IEnumerator DoFrostBowling(int actionid, BattleControl battle, MainManager.BattleData[] entities)
        {
            battle.nonphyscal = true;
            battle.GetSingleTarget();

            var playerTargetEntityRef = battle.playertargetentity;
            var playerTargetIDRef = battle.playertargetID;
            var targetBattleEntity = MainManager.GetPlayerData(playerTargetIDRef);

            Vector3[] startPositionEntities = MainManager.GetEntitiesPos(entities.Select(e => e.battleentity).ToArray());
            entities[0].battleentity.MoveTowards(new Vector3(4.5f, 0f), 2f);
            entities[1].battleentity.MoveTowards(new Vector3(8.2f, 0f), 2f);
            entities[2].battleentity.MoveTowards(new Vector3(6.45f, 0f, -0.85f), 2f);
            MainManager.SetCamera(new Vector3(3.9f, 0.25f, 1.75f));

            while (!MainManager.EntitiesAreNotMoving(entities.Select(e => e.battleentity).ToArray()))
            {
                yield return null;
            }

            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].battleentity.overrideanim = true;
                entities[i].battleentity.overrridejump = true;
                entities[i].battleentity.overridefly = true;
                entities[i].battleentity.LockRigid(value: true);
                entities[i].battleentity.flip = false;
            }
            entities[2].battleentity.animstate = 105;
            entities[0].battleentity.animstate = 11;

            GameObject iceball = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Objects/IceSphere")) as GameObject;
            GameObject viSprite = iceball.transform.GetChild(0).gameObject;
            GameObject iceParticles = MainManager.PlayParticle("IceRadius", entities[0].battleentity.transform.position + Vector3.up, -1f);

            iceParticles.transform.localScale = Vector3.one * 7f;
            iceParticles.transform.parent = entities[0].battleentity.sprite.transform;
            iceball.transform.parent = entities[0].battleentity.sprite.transform;

            MainManager.PlaySound("OverworldIce");
            MainManager.PlayParticle("mothicenormal", null, entities[0].battleentity.transform.position + Vector3.up, Vector3.zero, 3f, 3500);

            iceball.transform.localPosition = Vector3.up;
            viSprite.transform.parent = null;
            viSprite.transform.position = new Vector3(0f, 999f);
            iceball.transform.localScale = Vector3.zero;
            entities[0].battleentity.bobrange = 1f;

            float startTime = 0f;
            float endTime = 40f;
            ParticleSystem particleSyst = iceParticles.GetComponent<ParticleSystem>();
            ParticleSystem.EmissionModule e3 = particleSyst.emission;
            Vector3 startPos = new Vector3(0f, 2.5f, -11f);

            MainManager.PlaySound("Spin", 9, 0.5f, 1f, loop: true);
            var fill = 0.1f;
            do
            {
                fill += MainManager.TieFramerate(0.1f);
                startTime = Mathf.Lerp(startTime, Mathf.Clamp(fill, 0.25f, 1f), MainManager.TieFramerate(0.1f));
                entities[0].battleentity.spin = new Vector3(0f, 20f * startTime);
                iceball.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 200f, Mathf.Clamp(startTime, 0.5f, 1f));
                entities[0].battleentity.height = Mathf.Lerp(entities[0].battleentity.height, 2.5f, MainManager.TieFramerate(0.0075f));
                e3.rateOverTime = new ParticleSystem.MinMaxCurve(Mathf.Lerp(1f, 20f, startTime));
                MainManager.sounds[9].pitch = Mathf.Lerp(0.5f, 0.75f, fill / 1f);
                MainManager.instance.camoffset = Vector3.Lerp(MainManager.defaultcamoffset, startPos, startTime);
                yield return null;
            } while (fill < 3f);

            MainManager.StopSound(9);

            float a = 1;
            iceball.transform.localScale = Vector3.one * 200f * Mathf.Clamp(a, 0.5f, 1f);
            MainManager.instance.camoffset = new Vector3(0f, 3.2f, -9.85f);
            MainManager.DestroyTemp(iceParticles, 5f);

            iceball.transform.parent = null;
            viSprite.transform.parent = iceball.transform;
            viSprite.transform.localPosition = Vector3.zero;
            entities[0].battleentity.transform.position = new Vector3(0f, -999f);
            MainManager.PlaySound("Freeze");
            MainManager.PlayParticle("mothicenormal", null, viSprite.transform.position, Vector3.one, 3f, 3500).transform.localScale = Vector3.one * 2f * a;

            yield return EventControl.halfsec;

            startTime = 0f;
            entities[1].battleentity.LockRigid(value: false);
            entities[1].battleentity.overrideflip = true;
            entities[1].battleentity.MoveTowards(new Vector3(9.5f, 0f), 2f, 1, 116);
            startPos = iceball.transform.position;

            bool hit = false;
            MainManager.PlaySound("Charge7", -1, 0.7f, 1f);
            do
            {
                if (!entities[1].battleentity.forcemove)
                {
                    battle.StartCoroutine(entities[1].battleentity.ShakeSprite(0.1f, 1f));
                    entities[1].battleentity.animstate = 116;
                    yield return null;
                }
                if (!hit && startTime / endTime > 0.4f)
                {
                    hit = true;
                }
                iceball.transform.position = MainManager.BeizierCurve3(startPos, startPos + Vector3.up * 2f, 8f, startTime / endTime);
                startTime += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (startTime < endTime + 1f);

            entities[1].battleentity.sprite.transform.localPosition = Vector3.zero;
            entities[1].battleentity.LockRigid(value: true);
            entities[2].battleentity.animstate = 102;
            startTime = 0f;
            endTime = 15;

            MainManager.PlaySound("PingShot", -1, 0.9f, 1f);
            Vector3 start = iceball.transform.position;
            Vector3 target = new Vector3(iceball.transform.position.x, 1.75f * a + 0.25f * (1f - a));
            startPos = entities[1].battleentity.transform.position;
            do
            {
                entities[1].battleentity.transform.position = Vector3.Lerp(startPos, new Vector3(target.x - 1f, 0f), startTime / endTime);
                iceball.transform.position = Vector3.Lerp(start, target, startTime / endTime);
                startTime += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (startTime < endTime + 1f);

            MainManager.ShakeScreen(0.2f * a, 0.5f, dontreset: true);
            SpinAround spinAround = iceball.AddComponent<SpinAround>();
            BattleControl.SetDefaultCamera();
            startTime = 0f;
            MainManager.HitPart(entities[1].battleentity.transform.position + Vector3.right);

            spinAround.itself = new Vector3(-10f, -3f, -8f) * a;
            MainManager.PlaySound("BigHit");
            endTime = 80f;
            startPos = iceball.transform.position;
            start = new Vector3(-10f, iceball.transform.position.y);
            hit = false;

            battle.StartCoroutine(MainManager.ArcMovement(entities[1].battleentity.gameObject, entities[1].battleentity.transform.position + new Vector3(1.5f, 0f), 3f, 40f));
            MainManager.PlaySound("Spin5", 9, 1.5f, 0.9f, loop: true);
            bool[] entityHit = new bool[MainManager.instance.playerdata.Length];
            int partydam = Mathf.FloorToInt(GetDarkViDamage(0, battle) + darkKabbuBaseDamage + darkLeifBaseDamage);

            yield return null;
            do
            {
                if (iceball.transform.position.x < MainManager.instance.playerdata[battle.partypointer[2]].battleentity.transform.position.x)
                    break;
                for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                {
                    if (!entityHit[i] && iceball.transform.position.x < MainManager.instance.playerdata[i].battleentity.transform.position.x)
                    {
                        entityHit[i] = true;
                        if (MainManager.instance.playerdata[i].hp > 0)
                        {
                            battle.DoDamage(null, ref MainManager.instance.playerdata[i], Mathf.Clamp(Mathf.CeilToInt((float)partydam * a), 2, 99), BattleControl.AttackProperty.Freeze,null, battle.commandsuccess);
                        }
                    }
                }
                iceball.transform.position = Vector3.Lerp(startPos, start, startTime / endTime);
                startTime += MainManager.framestep;
                yield return null;
            }
            while (startTime < endTime + 1f);

            entities[1].battleentity.animstate = 13;
            entities[2].battleentity.animstate = 13;
            MainManager.StopSound(9);
            MainManager.PlaySound("Explosion5");
            MainManager.PlayParticle("IceShatter", iceball.transform.position).transform.localScale = Vector3.one * 3f;
            entities[0].battleentity.transform.position = iceball.transform.position;

            UnityEngine.Object.Destroy(iceball);
            MainManager.ShakeScreen(0.3f, 1.25f, dontreset: true);
            MainManager.PlayParticle("mothicenormal", entities[0].battleentity.transform.position + Vector3.up).transform.localScale = Vector3.one * 4f * a;
            battle.PartyDamage(actionid, Mathf.FloorToInt(Mathf.Clamp((float)partydam / 2f * a, 1f, partydam)), BattleControl.AttackProperty.Freeze, battle.commandsuccess);
            
            startTime = 0f;
            endTime = 60f;
            entities[0].battleentity.spin = new Vector3(0f, 30f);
            entities[0].battleentity.height = 0f;
            startPos = entities[0].battleentity.transform.position;
            do
            {
                entities[0].battleentity.transform.position = MainManager.BeizierCurve3(startPos, startPositionEntities[0], 7f, startTime / endTime);
                startTime += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (startTime < endTime + 1f);
            entities[0].battleentity.animstate = 18;
            entities[0].battleentity.spin = Vector3.zero;
            MainManager.DeathSmoke(entities[0].battleentity.transform.position);
            MainManager.PlaySound("Death3");
            yield return EventControl.halfsec;
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].battleentity.overrideflip = false;
                entities[i].battleentity.LockRigid(value: false);
                entities[i].battleentity.overridefly = false;
                entities[i].battleentity.overrridejump = false;
                entities[i].battleentity.overrideanim = false;
                entities[i].battleentity.MoveTowards(startPositionEntities[i], 2f);
            }
            while (!MainManager.EntitiesAreNotMoving(entities.Select(e => e.battleentity).ToArray()))
            {
                yield return null;
            }
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].battleentity.flip = false;
                if (i != actionid)
                    battle.enemydata[entities[i].entity.battleid].cantmove++;
            }
            battle.UpdateAnim();
            yield return null;
        }

        public static IEnumerator DoFrozenDrill(EntityControl entity, BattleControl battle, EntityControl[] entities, int actionid)
        {
            battle.nonphyscal = true;
            battle.GetSingleTarget();


            var playerTargetEntityRef = battle.playertargetentity;
            var playerTargetIDRef = battle.playertargetID;

            Vector3[] startEntitiesPos = new Vector3[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                startEntitiesPos[i] = entities[i].transform.position;
                entities[i].flip = true;
                entities[i].overrideflip = true;
            }

            entities[0].MoveTowards(new Vector3(2.95f, 0f, -1.45f), 2f);
            entities[1].MoveTowards(new Vector3(4.25f, 0f, -2f), 2f);

            while (entities[0].forcemove || entities[1].forcemove)
            {
                yield return null;
            }

            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].overrideflip = false;
                entities[i].flip = false;
            }

            MainManager.instance.camtargetpos = Vector3.Lerp(entity.transform.position, playerTargetEntityRef.transform.position, 0.5f);
            MainManager.instance.camspeed = 0.01f;
            MainManager.instance.camoffset = new Vector3(0f, 2.65f, -7f);

            entities[1].animstate = 108;
            GameObject iceDrill = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/AnimSpecific/mothbattlesphere"), entities[1].transform.position + new Vector3(-0.75f, 1.55f), Quaternion.Euler(90f, 0f, 0f)) as GameObject;
            MeshRenderer singleSpehre = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Objects/SingleSphere"), Vector3.zero, Quaternion.identity, iceDrill.transform) as GameObject).GetComponent<MeshRenderer>();
            singleSpehre.transform.localPosition = Vector3.zero;
            singleSpehre.material.color = new Color(1f, 1f, 1f, 0.5f);

            battle.StartCoroutine(MainManager.ArcMovement(iceDrill, iceDrill.transform.position + new Vector3(2f, -5f), 3f, 40f));
            UnityEngine.Object.Destroy(iceDrill, 2f);
            MainManager.PlaySound("Dig");
   
            yield return battle.KabbuDig(entities[0]);

            yield return EventControl.halfsec;
            entities[0].LockRigid(value: true);
            battle.StartCoroutine(MainManager.MoveTowards(entities[0].transform, playerTargetEntityRef.transform.position + new Vector3(0f, 0f, -0.1f), 75f, smooth: true, local: false));
            yield return EventControl.sec;

            MainManager.PlaySound("DigPop");
            MainManager.PlayParticle("DirtExplode", playerTargetEntityRef.transform.position);
            MainManager.ShakeScreen(0.1f, 0.75f, dontreset: true);
            MainManager.PlaySound("WaspDrill", -1, 1f, 1f, loop: true);

            iceDrill = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Objects/IceDrill"), playerTargetEntityRef.transform.position + new Vector3(0f, -5f), Quaternion.Euler(-90f, 0f, 0f)) as GameObject);
            battle.StartCoroutine(MainManager.MoveTowards(iceDrill.transform, playerTargetEntityRef.transform.position, 10f, smooth: false, local: true));
            entities[1].animstate = 101;
            iceDrill.transform.localScale = new Vector3(1f, 1f, 0.75f);

            MainManager.SetCamera(playerTargetEntityRef.transform.position + Vector3.up * 2f);
            entities[1].animstate = 108;
            iceDrill.AddComponent<SpinAround>().itself = new Vector3(0f, 0f, -1.5f * 3);
            entities[0].digging = false;
            entities[0].transform.position += new Vector3(0f, -3f);

            if (MainManager.instance.playerdata[playerTargetIDRef].weight < 100f)
            {
                playerTargetEntityRef.LockRigid(value: true);
                playerTargetEntityRef.spin = new Vector3(0f, 2.5f * 3);
            }
            int damage = darkLeifBaseDamage + darkKabbuBaseDamage - 4;

            playerTargetEntityRef.overrideanim = true;
            playerTargetEntityRef.overrridejump = true;
            MainManager.instance.playerdata[playerTargetIDRef].battleentity.animstate = 11;
            Vector3 targetPos = new Vector3(playerTargetEntityRef.transform.position.x, 1.5f, playerTargetEntityRef.transform.position.z);
            for (int i = 0; i < 3; i++)
            {
                if (MainManager.instance.playerdata[playerTargetIDRef].weight < 100f)
                {
                    battle.StartCoroutine(MainManager.ArcMovement(playerTargetEntityRef.gameObject, targetPos, 4f, 40f));
                    playerTargetEntityRef.onground = false;
                }
                MainManager.PlaySound("ShieldHit", -1, 1f, 0.65f);
                battle.DoDamage(null, ref MainManager.instance.playerdata[playerTargetIDRef], damage, BattleControl.AttackProperty.Pierce,null, battle.commandsuccess);
                entities[1].animstate = 104;
                yield return EventControl.halfsec;
                yield return EventControl.tenthsec;
            }
            yield return EventControl.halfsec;

            entities[0].sprite.transform.localPosition = Vector3.zero;
            playerTargetEntityRef.spin = Vector3.zero;

            MainManager.StopSound("WaspDrill");
            MainManager.PlaySound("BigHit");
            MainManager.PlaySound("OverworldIce", -1, 0.9f, 1f);
            MainManager.PlayParticle("DirtExplode", playerTargetEntityRef.transform.position);
            MainManager.PlayParticle("mothicenormal", iceDrill.transform.position + new Vector3(0f, 2f)).transform.localScale = Vector3.one * 2.5f;
            MainManager.PlayParticle("IceShatter", iceDrill.transform.position + new Vector3(0f, 2f)).transform.localScale = Vector3.one * 2.5f;

            UnityEngine.Object.Destroy(iceDrill.gameObject);
            MainManager.ShakeScreen(0.2f, 0.85f, dontreset: true);
            entities[0].spin = new Vector3(0f, 30f);
            entities[0].overrideflip = false;
            entities[0].animstate = 119;

            if (MainManager.instance.playerdata[playerTargetIDRef].weight < 100f)
            {
                battle.StartCoroutine(MainManager.ArcMovement(playerTargetEntityRef.gameObject, new Vector3(playerTargetEntityRef.transform.position.x, 0f, playerTargetEntityRef.transform.position.z), 7.5f, 60f));
                playerTargetEntityRef.onground = false;
            }

            battle.DoDamage(null, ref MainManager.instance.playerdata[playerTargetIDRef], damage + 2, BattleControl.AttackProperty.Pierce,null, battle.commandsuccess);
            
            battle.StartCoroutine(MainManager.ArcMovement(entities[0].gameObject, startEntitiesPos[0] + Vector3.up, 10f, 60f));
            yield return EventControl.sec;

            entities[0].FlipAngle(setangle: true);
            BattleControl.SetDefaultCamera();
            MainManager.instance.camspeed = 0.015f;
            entities[0].sprite.transform.localPosition = Vector3.zero;
            while (battle.enemybounce != null && battle.enemybounce[0] != null)
            {
                yield return null;
            }

            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].MoveTowards(startEntitiesPos[i], 2f, 1, 13);
                entities[i].overrideflip = true;
                entities[i].overrideanim = false;
                entities[i].spin = Vector3.zero;
                entities[i].LockRigid(value: false);
            }

            while (entities[0].forcemove || entities[1].forcemove)
            {
                yield return null;
            }

            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].transform.position = startEntitiesPos[i];
                entities[i].overrideflip = false;
                entities[i].flip = false;
                if (i != actionid)
                    battle.enemydata[entities[i].battleid].cantmove++;
            }

            playerTargetEntityRef.LockRigid(value: false);
            playerTargetEntityRef.overrideanim = false;
        }
        public static IEnumerator DoFlyDrop(BattleControl battle, EntityControl[] entities, int actionid)
        {
            battle.GetSingleTarget();
            var playerTargetEntityRef = battle.playertargetentity;
            var playerTargetIDRef = battle.playertargetID;

            var targetBattleEntity = MainManager.GetPlayerData(playerTargetIDRef);

            Vector3[] entityStartPos = new Vector3[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].LockRigid(value: true);
                entities[i].overrideanim = true;
                entities[i].overrridejump = true;
                entityStartPos[i] = entities[i].transform.position;
            }

            float a = 0f;
            float b = 30f;
            MainManager.PlaySound("Jump");
            do
            {
                entities[0].animstate = 2;
                entities[0].transform.position = MainManager.BeizierCurve3(entityStartPos[0], entities[1].transform.position + Vector3.up, 4f, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b + 1f);

            MainManager.SetCamera(null, playerTargetEntityRef.transform.position, 0.025f, new Vector3(0f, 4.5f + playerTargetEntityRef.height, -9.5f));
            MainManager.PlaySound("BeeFly", 9, 1f, 1f, loop: true);
            entities[0].transform.position = entities[1].transform.position + new Vector3(0f, 0f, -0.1f);
            entities[0].animstate = 102;
            entities[1].animstate = 102;
            entities[1].transform.parent = entities[0].transform;

            a = 0f;
            b = 170f;
            Vector3 startPos = entities[0].transform.position;

            Vector3 targetPos = battle.CenterPos(targetBattleEntity, true) + new Vector3(0f, 0f, -0.1f);
            do
            {
                entities[0].transform.position = Vector3.Lerp(entities[0].transform.position, new Vector3(Mathf.Lerp(startPos.x, targetPos.x, a / b), Mathf.Lerp(0.5f, 5f, 1f / 1f) + Mathf.Lerp(0f, targetPos.y + 5f, a / b)), MainManager.TieFramerate(0.1f));
                MainManager.sounds[9].pitch = Mathf.Lerp(1f, 1.2f, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b + 1f);

            MainManager.StopSound(9);
            MainManager.PlaySound("Fall");
            entities[1].transform.parent = battle.battlemap.transform;
            entities[1].animstate = 119;
            entities[1].sprite.flipY = true;
            entities[1].spin = new Vector3(0f, 25f * 1);
            entities[0].animstate = 114;
            startPos = entities[0].transform.position;
            a = 0f;
            b = Mathf.Clamp(50f * 1, 25f, 50f);
            ButtonSprite but2 = null;

            do
            {
                entities[0].transform.position = MainManager.BeizierCurve3(startPos, new Vector3(20f, startPos.y + 10f), -3f, a / b);
                entities[1].transform.position = Vector3.Lerp(startPos, targetPos, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b + 1f);
            entities[0].transform.position = new Vector3(0f, -99f);
            if (but2 != null)
            {
                UnityEngine.Object.Destroy(but2.gameObject);
            }

            int damage = GetDarkViDamage(actionid, battle) + darkKabbuBaseDamage + 4;

            MainManager.PlaySound("AtkSuccess");
            MainManager.ShakeScreen(0.2f, 0.5f, dontreset: true);
            MainManager.PlaySound("HugeHit");

            battle.DoDamage(null, ref MainManager.instance.playerdata[playerTargetIDRef], damage, BattleControl.AttackProperty.Pierce, null,battle.commandsuccess);
            do
            {
                entities[1].transform.position += new Vector3(0f, 0f - MainManager.TieFramerate(0.25f));
                yield return null;
            }
            while (entities[1].transform.position.y > 0f);

            MainManager.PlayParticle("DirtExplode", entities[1].transform.position);
            MainManager.PlaySound("Dig", -1, 1.2f, 1f);

            entities[1].digging = true;
            entities[1].sprite.flipY = false;
            entities[1].spin = Vector3.zero;
            yield return EventControl.halfsec;
            BattleControl.SetDefaultCamera();

            a = 0f;
            b = 45f;
            startPos = new Vector3(targetPos.x, 0f, targetPos.z);
            targetPos = new Vector3(-15f, 10f);
            do
            {
                entities[0].transform.position = Vector3.Lerp(targetPos, entityStartPos[0] + Vector3.down, a / b);
                entities[1].transform.position = Vector3.Lerp(startPos, entityStartPos[1], a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b + 1f);

            entities[0].animstate = 13;
            entities[0].SetAnim(string.Empty, force: true);
            entities[0].transform.position = entityStartPos[0];

            if (entities[1].digging)
            {
                MainManager.PlaySound("DigPop");
                MainManager.PlayParticle("DirtExplode", entities[1].transform.position);
            }
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].digging = false;
                entities[i].overrideanim = false;
                entities[i].overrridejump = false;
                entities[i].LockRigid(value: false);
                if (i != actionid)
                    battle.enemydata[entities[i].battleid].cantmove++;
            }
            yield return EventControl.halfsec;
        }

        public static IEnumerator DoAntiTaunt()
        {
            MainManager.StopSound(9);
            for (int i = 0; i != MainManager.instance.playerdata.Length; i++)
            {
                if (!MainManager.battle.IsStoppedLite(MainManager.instance.playerdata[i]))
                    MainManager.instance.playerdata[i].battleentity.animstate = 9;
            }
            yield return EventControl.halfsec;
            MainManager.instance.StartCoroutine(MainManager.SetText("Did you seriously try to taunt strat us? We'll show you how it's done!", dialogue: true, Vector3.zero, MainManager.battle.enemydata[0].battleentity.transform, null));
            while (MainManager.instance.message)
            {
                yield return null;
            }
            yield return EventControl.halfsec;
            yield return new DarkLeifAI().DoBubbleShieldAll(MainManager.battle.enemydata[2].battleentity, MainManager.battle);

            yield return MainManager.battle.EnemyTaunt(MainManager.battle.enemydata[1].battleentity, true, 1);
        }

        public static bool CantTauntDTS()
        {
            var battle = MainManager.battle;
            int kabbuIndex = battle.EnemyInField((int)NewEnemies.DarkKabbu);
            int leifIndex = battle.EnemyInField((int)NewEnemies.DarkLeif);
            int viIndex = battle.EnemyInField((int)NewEnemies.DarkVi);

            return kabbuIndex != -1 && BattleControl_Ext.CanBeRelayed(battle.enemydata[kabbuIndex]) && viIndex != -1 && BattleControl_Ext.CanBeRelayed(battle.enemydata[viIndex]) && leifIndex != -1 && BattleControl_Ext.CanBeRelayed(battle.enemydata[leifIndex]);
        }
    }
}
