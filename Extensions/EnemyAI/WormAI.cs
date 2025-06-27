using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BFPlus.Extensions.EnemyAI
{
    public class WormAI : AI
    {
        bool isSwarm = false;

        public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
        {
            var instance = MainManager.battle;

            isSwarm = entity.animid == (int)NewAnimID.WormSwarm;
            Vector3 startPos = entity.transform.position;

            MainManager.PlaySound("Dig");
            entity.animstate = 100;

            List<EntityControl> worms = new List<EntityControl>() { entity };
            if (isSwarm)
            {
                worms.AddRange(entity.subentity);
                foreach (var worm in worms)
                    worm.digging = true;
            }
            else
            {
                instance.GetSingleTarget();
            }

            yield return DoDig(entity);
            float a = 0f;
            float b = 100f;
            Vector3 pos = entity.transform.position;
            Vector3 target = isSwarm ? instance.partymiddle + new Vector3(-0.25f, 0f, 0f) : instance.playertargetentity.transform.position + new Vector3(0f, 0f, -0.25f);
            MainManager.PlaySound("Digging", 9, 1.1f, 0.75f, true);
            do
            {
                entity.transform.position = Vector3.Lerp(pos, target, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b);
            yield return EventControl.quartersec;

            MainManager.StopSound(9);

            yield return WormAttacks(entity, worms, actionid, instance);

            a = 0f;
            b = 50f;
            pos = entity.transform.position;
            do
            {
                entity.transform.position = Vector3.Lerp(pos, startPos, a / b) + new Vector3(0f, 0f, Mathf.Sin(Time.time * 10f));
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b);
            entity.transform.position = startPos;
            foreach (var worm in worms)
                worm.digging = false;
        }

        IEnumerator WormAttacks(EntityControl entity, List<EntityControl> worms, int actionid, BattleControl instance)
        {
            bool hardmode = instance.HardMode();
            int damage = isSwarm ? 2 : 3;
            int hits = isSwarm ? 1 : 3;

            int playerTargetIDRef = isSwarm ? 0 : instance.playertargetID;


            int hitCount = 0;
            int maxSwarmHit = 7;
            for (int i = 0; i < hits; i++)
            {
                foreach (var worm in worms)
                {
                    worm.digging = false;
                    worm.animstate = 100;
                    MainManager.PlaySound("DigPop2");

                    if (isSwarm)
                        instance.PartyDamage(actionid, damage, null, instance.commandsuccess);
                    else
                        instance.DoDamage(actionid, playerTargetIDRef, damage, null, instance.commandsuccess);

                    yield return EventControl.quartersec;
                    worm.digging = true;
                    if (!isSwarm)
                        yield return DoDig(entity);

                    if (MainManager.GetAlivePlayerAmmount() == 0)
                        yield break;

                    float waitTime = isSwarm ? 0.45f : 0.25f;
                    waitTime -= hardmode ? 0.1f : 0f;
                    yield return new WaitForSeconds(waitTime);
                    hitCount++;

                    if (isSwarm)
                    {
                        if (hitCount >= maxSwarmHit)
                        {
                            break;
                        }
                    }
                }
            }
        }

        IEnumerator DoDig(EntityControl entity)
        {
            entity.digging = true;
            while (entity.digtime < 29f)
            {
                yield return null;
            }
        }
    }
}
