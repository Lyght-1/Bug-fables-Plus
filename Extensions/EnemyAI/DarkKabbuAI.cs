using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace BFPlus.Extensions.EnemyAI
{
    public class DarkKabbuAI : AI
    {
        const int DIG_DAMAGE = 5;
        const int HEAVY_STRIKE_DAMAGE = 5;
        const int PEBBLE_DAMAGE = 3;
        const int BOULDER_DAMAGE = 5;
        const int SMALL_BOULDER_DAMAGE = 2;

        enum Attacks
        {
            Taunt,
            HeavyStrike,
            PebbleToss,
            Understrike,
            BoulderToss,
            Flydrop,
            DashThrough,
            Relay,
            MiracleShake,
            FrozenDrill,
            FrostBowling,
            QueenDinner
        }
        BattleControl battle = null;
        public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
        {
            List<Attacks> chances = new List<Attacks> 
            { 
                Attacks.Taunt, Attacks.Taunt, Attacks.Taunt, 
                Attacks.HeavyStrike, Attacks.HeavyStrike, Attacks.HeavyStrike,
                Attacks.PebbleToss, Attacks.PebbleToss, Attacks.PebbleToss,
                Attacks.Understrike, Attacks.Understrike,
                Attacks.BoulderToss, Attacks.BoulderToss, Attacks.BoulderToss,
                Attacks.DashThrough,  Attacks.DashThrough,  Attacks.DashThrough,  Attacks.DashThrough
            };

            battle = MainManager.battle;
            DarkTeamSnakemouth.SetupFight(actionid);

            int viIndex = battle.EnemyInField((int)NewEnemies.DarkVi);
            int leifIndex = battle.EnemyInField((int)NewEnemies.DarkLeif);

            float hpPercent = battle.HPPercent(battle.enemydata[actionid]);

            //fly drop
            if (viIndex > -1 && hpPercent <= 0.7f && BattleControl_Ext.CanBeRelayed(battle.enemydata[viIndex]))
            {
                chances.AddRange(new Attacks[] { Attacks.Flydrop, Attacks.Flydrop, Attacks.Flydrop, Attacks.Flydrop, Attacks.Flydrop });
            }

            //queens dinner
            if (DarkTeamSnakemouth.HasItem((int)MainManager.Items.KingDinner))
            {
                for (int i = 0; i != battle.enemydata.Length; i++)
                {
                    float hp = battle.HPPercent(battle.enemydata[i]);
                    if (hp <= 0.6f)
                    {
                        chances.AddRange(new Attacks[] { Attacks.QueenDinner, Attacks.QueenDinner });
                    }
                }
            }

            //frozen Drill
            if (leifIndex > -1 && hpPercent <= 0.7f && BattleControl_Ext.CanBeRelayed(battle.enemydata[leifIndex]))
            {
                chances.AddRange(new Attacks[] { Attacks.FrozenDrill, Attacks.FrozenDrill, Attacks.FrozenDrill, Attacks.FrozenDrill, Attacks.FrozenDrill });
            }

            //frost bowling
            if (viIndex > -1 && leifIndex > -1 && hpPercent <= 0.7f && BattleControl_Ext.CanBeRelayed(battle.enemydata[leifIndex]) && BattleControl_Ext.CanBeRelayed(battle.enemydata[viIndex]))
            {
                chances.AddRange(new Attacks[] { Attacks.FrostBowling, Attacks.FrostBowling, Attacks.FrostBowling});
            }

            //relay 
            if (BattleControl_Ext.Instance.CanRelay(entity, battle) && !DarkTeamSnakemouth.relayedThisTurn[1])
            {
                chances.AddRange(new Attacks[] { Attacks.Relay, Attacks.Relay, Attacks.Relay });

                for (int i = 0; i != battle.enemydata.Length; i++)
                {
                    if (battle.enemydata[i].charge > 2 || MainManager.HasCondition(MainManager.BattleCondition.AttackUp, battle.enemydata[i]) > -1)
                    {
                        chances.AddRange(new Attacks[] { Attacks.Relay, Attacks.Relay, Attacks.Relay, Attacks.Relay, Attacks.Relay, Attacks.Relay });
                    }
                }
            }
        
            Attacks action = chances[UnityEngine.Random.Range(0, chances.Count)];

            if (DarkTeamSnakemouth.CanUseMiracleShake())
                action = Attacks.MiracleShake;


            if(action != Attacks.MiracleShake && battle.enemydata.Length == 1 && battle.enemydata[actionid].charge < 3)
            {
                battle.StartCoroutine(battle.ItemSpinAnim(entity.transform.position + Vector3.up, MainManager.itemsprites[1, (int)Medal.Vengeance], true));
                yield return EventControl.quartersec;
                battle.enemydata[actionid].charge = 3;
                battle.StartCoroutine(battle.StatEffect(entity, 4));
                entity.animstate = (int)MainManager.Animations.Angry;
                MainManager.PlaySound("Wam");
                MainManager.PlaySound("StatUp", -1, 1.25f, 1f);
                yield return EventControl.halfsec;
            }

            EntityControl[] entities;
            MainManager.BattleData[] battleDatas;
            switch (action)
            {
                case Attacks.Taunt:
                    yield return battle.EnemyTaunt(entity, true, 1);
                    break;
                case Attacks.HeavyStrike:
                    yield return battle.EnemyHeavyStrike(entity, HEAVY_STRIKE_DAMAGE);
                    break;
                case Attacks.PebbleToss:
                    yield return battle.EnemyPebbleToss(entity, PEBBLE_DAMAGE, true);
                    break;
                case Attacks.Understrike:
                    yield return battle.EnemyKabbuDig(entity, DIG_DAMAGE);
                    break;
                case Attacks.BoulderToss:
                    yield return DoBoulderToss(entity, actionid, battle);
                    break;
                case Attacks.Flydrop:
                    entities = new EntityControl[] { battle.enemydata[viIndex].battleentity, battle.enemydata[actionid].battleentity };
                    yield return DarkTeamSnakemouth.DoFlyDrop(battle, entities, actionid);
                    break;
                case Attacks.DashThrough:
                    yield return battle.StartCoroutine(battle.BeetleDash(actionid, 5, 116, 117, Vector3.zero, 25));
                    break;
                case Attacks.Relay:
                    yield return battle.EnemyRelay(entity, BattleControl_Ext.Instance.FindRelayable(entity, battle));
                    DarkTeamSnakemouth.relayedThisTurn[1] = true;
                    break;
                case Attacks.MiracleShake:
                    yield return DarkTeamSnakemouth.UseItem((int)MainManager.Items.MiracleShake, entity, actionid);
                    break;
                case Attacks.FrozenDrill:
                    entities = new EntityControl[] { battle.enemydata[actionid].battleentity, battle.enemydata[leifIndex].battleentity };
                    yield return DarkTeamSnakemouth.DoFrozenDrill(battle.enemydata[leifIndex].battleentity, battle, entities, actionid);
                    break;
                case Attacks.FrostBowling:
                    battleDatas = new MainManager.BattleData[] { battle.enemydata[viIndex], battle.enemydata[actionid], battle.enemydata[leifIndex] };
                    yield return DarkTeamSnakemouth.DoFrostBowling(actionid, battle, battleDatas);
                    break;
                case Attacks.QueenDinner:
                    yield return DarkTeamSnakemouth.UseItem((int)MainManager.Items.KingDinner, entity,actionid);
                    break;
            }
        }

        IEnumerator DoBoulderToss(EntityControl entity, int actionid, BattleControl instance)
        {
            battle.nonphyscal = true;
            battle.GetSingleTarget();

            var playerTargetEntityRef = battle.playertargetentity;
            var playerTargetIDRef = battle.playertargetID;

            MainManager.SetCamera(null, new Vector3(-1.25f, 1.6f, -1f), 0.055f);
            GameObject boulder = MainManager.CreateRock(new Vector3(0f, -50f), Vector3.one * 0.5f, new Vector3(0f, 0f, 35f));
            entity.MoveTowards(new Vector3(1.5f, 0f, -1f), 2f);
            while (entity.forcemove)
            {
                yield return null;
            }
            MainManager.PlaySound("Dig");

            yield return battle.KabbuDig(entity); ;
            yield return EventControl.halfsec;

            MainManager.ShakeScreen(0.2f, 0.5f);
            entity.overrideanim = false;
            entity.overrideflip = false;
            entity.overrideheight = false;

            entity.animstate = 103;
            float startTime = 0f;
            float endTime = 15f;

            float startTime2 = 0f;
            float endTime2 = 100f;
            MainManager.PlayParticle("DirtExplode2", entity.transform.position);
            MainManager.PlaySound("PebbleAtkDigUp");

            Vector3 startBoulderPos = entity.transform.position + new Vector3(0f, 12f);
            do
            {
                boulder.transform.position = MainManager.SmoothLerp(entity.transform.position, startBoulderPos, startTime / endTime);
                startTime += MainManager.framestep;
                startTime2 += MainManager.framestep;
                yield return null;
            }
            while (startTime < endTime + 1f);
            startTime = 0f;
            endTime = endTime2 - startTime2 - 15f;
            startBoulderPos = entity.transform.position + new Vector3(0f, 42.5f);
            do
            {
                boulder.transform.position = Vector3.Lerp(startBoulderPos, entity.transform.position + Vector3.up, startTime / endTime);
                startTime += MainManager.framestep;
                yield return null;
            }
            while (startTime < endTime + 1f);

            Vector3? camtargetpos = MainManager.instance.camtargetpos;
            MainManager.instance.camtargetpos = ((!camtargetpos.HasValue) ? null : new Vector3?(camtargetpos.GetValueOrDefault() + new Vector3(2f, 0f)));
            entity.animstate = 104;
            startTime = 0f;
            endTime = 40f;
            startTime2 = playerTargetEntityRef.height;
            startBoulderPos = playerTargetEntityRef.transform.position + new Vector3(0f, startTime2 + 1f);
            Vector3 start = entity.transform.position + new Vector3(0f, 2f);
            MainManager.PlaySound("PebbleAtkLaunch");
            do
            {
                boulder.transform.position = MainManager.BeizierCurve3(start, startBoulderPos, startTime2 + 5f, startTime / endTime);
                startTime += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (startTime < endTime + 1f);

            MainManager.PlaySound("PebbleAtkHit1");
            battle.DoDamage(instance.enemydata[actionid], ref MainManager.instance.playerdata[playerTargetIDRef], BOULDER_DAMAGE, null, null, instance.commandsuccess);
            
            bool wait = false;
            instance.StartCoroutine(MainManager.LateSound("PebbleAtkHit2", 0.5f));
            for (int i = 0; i < 2; i++)
            {
                GameObject miniBoulder = UnityEngine.Object.Instantiate(boulder);
                miniBoulder.transform.localScale = Vector3.one * 0.25f;
                miniBoulder.transform.position = playerTargetEntityRef.transform.position + Vector3.up;
                start = entity.transform.position + new Vector3((i != 0) ? 2.5f : 2.5f, 0f);
                if (MainManager.instance.playerdata.Length > 1)
                {
                    int j = playerTargetIDRef + ((i != 0) ? 1 : (-1));
                    if (j >= 0 && j < MainManager.instance.playerdata.Length && MainManager.instance.playerdata[j].hp > 0)
                    {
                        wait = true;
                        instance.StartCoroutine(BattleControl_Ext.Instance.DoLateDamage(actionid, j, SMALL_BOULDER_DAMAGE, null, 0.5f, instance));
                        start = MainManager.instance.playerdata[j].battleentity.transform.position + new Vector3(0f, MainManager.instance.playerdata[j].battleentity.height);
                    }
                }
                instance.StartCoroutine(MainManager.ArcMovement(miniBoulder, miniBoulder.transform.position, start, Vector3.zero, 4f, 30f, destroyonend: true));
            }

            battle.RockBreak(boulder);
            if (wait)
            {
                yield return EventControl.halfsec;
            }
            yield return EventControl.halfsec;
        }
    }
}
