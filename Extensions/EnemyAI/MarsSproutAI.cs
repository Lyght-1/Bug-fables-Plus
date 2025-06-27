using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using static BFPlus.Extensions.BattleControl_Ext;

namespace BFPlus.Extensions.EnemyAI
{
    public class MarsSproutAI : AI
    {
        enum Attacks
        {
            Bite,
            UndergroundBite,
            SeedRain,
            HealMars,
            MultiSeeds
        }

        BattleControl battle = null;
        int BITE_DAMAGE = 5;
        int SEED_DAMAGE = 3;
        int UNDERGROUND_STRIKE_DAMAGE = 6;
        Vector3 offset = new Vector3(2f, 1.5f, -0.1f);
        public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
        {
            battle = MainManager.battle;

            if (entity.GetComponent<MarsBud>() == null)
            {
                var mb = entity.gameObject.AddComponent<MarsBud>();
                mb.entity = entity;
            }

            if (battle.enemydata[actionid].hitaction)
            {
                entity.animstate = (int)MainManager.Animations.Angry;
                entity.emoticoncooldown = 30f;
                entity.emoticonid = 2;
                MainManager.PlaySound("Wam");
                while (entity.emoticoncooldown > 0f)
                {
                    yield return null;
                }
                yield return DoBite(entity, actionid);
            }
            else
            {
                int marsIndex = battle.EnemyInField((int)NewEnemies.Mars);
                float hpPercentMars = battle.HPPercent(battle.enemydata[marsIndex]);

                List<Attacks> chances = new List<Attacks>() { Attacks.Bite, Attacks.Bite, Attacks.UndergroundBite, Attacks.SeedRain, Attacks.MultiSeeds };

                if (hpPercentMars <= 0.75f)
                {
                    chances.AddRange(new Attacks[]{ Attacks.HealMars, Attacks.HealMars});
                }

                switch (chances[UnityEngine.Random.Range(0, chances.Count)])
                {
                    case Attacks.Bite:
                        yield return DoBite(entity, actionid);
                        break;
                    case Attacks.UndergroundBite:
                        yield return DoUndergroundStrike(entity, actionid);
                        break;
                    case Attacks.SeedRain:
                        yield return DoSeedRain(entity, actionid);
                        break;
                    case Attacks.HealMars:
                        yield return HealMars(entity, actionid, marsIndex);
                        break;
                    case Attacks.MultiSeeds:
                        if (UnityEngine.Random.Range(0, 2) == 0)
                        {
                            yield return DoSeedAttack(entity, actionid, false);
                        }
                        else
                        {
                            yield return DoSeedAttack(entity, actionid, true);
                        }
                        break;
                }
            }

            yield return null;
        }

        //100 = starting to Charge Up
        //101 = chargeUp
        //102 = bites
        IEnumerator DoBite(EntityControl entity, int actionid)
        {
            Transform arm = entity.extras[0].transform.parent.parent.parent.parent;
            Vector3 baseBonePos = arm.position;
            MainManager.PlaySound("Chew", -1, 0.80f, 1f);

            battle.GetSingleTarget();
            var playertargetentityRef = battle.playertargetentity;

            battle.MiddleCamera(entity.transform, playertargetentityRef.transform);
            entity.animstate = 100;
            yield return EventControl.halfsec;
            yield return EventControl.quartersec;

            var mb = entity.GetComponent<MarsBud>();
            mb.target = playertargetentityRef.transform.position + new Vector3(2f, 1.5f, -0.1f);

            entity.animstate = 102;

            battle.DoDamage(actionid, battle.playertargetID, BITE_DAMAGE, BattleControl.AttackProperty.Poison, battle.commandsuccess);
            battle.CameraFocus(playertargetentityRef.transform.position);

            MainManager.instance.camoffset += Vector3.right;

            MainManager.PlaySound("Bite2", -1, 0.9f, 1f);
            yield return EventControl.halfsec;

            entity.animstate = 0;
            yield return EventControl.tenthsec;

            BattleControl.SetDefaultCamera();
            arm.position = baseBonePos;
            yield return null;
        }


        //107 = start charge up
        //108 = charge up
        //109 = shoots
        IEnumerator DoSeedAttack(EntityControl entity, int actionid, bool big)
        {
            battle.nonphyscal = true;
            int hits = big ? 1 : 3;

            SpriteRenderer[] seeds = new SpriteRenderer[hits];
            int damage = big ? SEED_DAMAGE *2 : SEED_DAMAGE;

            BattleControl.AttackProperty[] properties =
            {
                BattleControl.AttackProperty.Poison,
                BattleControl.AttackProperty.DefDownOnBlock,
                BattleControl.AttackProperty.AtkDownOnBlock,
                BattleControl.AttackProperty.Ink,
                BattleControl.AttackProperty.Sticky
            };

            for (int i = 0; i < hits; i++)
            {
                MainManager.PlaySound("Blosh", 1.25f, 1);
                battle.GetSingleTarget();
                int playerTargetID = battle.playertargetID;

                //charges up
                entity.animstate = 107;
                yield return new WaitForSeconds(big ? 1.5f : 0.6f);

                //shoots
                entity.animstate = 109;
                GameObject head = entity.extras[1];
                seeds[i] = MainManager.NewSpriteObject(head.transform.position, null, MainManager.itemsprites[0, 23]);
                seeds[i].material.color = new Color(0.63f, 0.129f, 0.129f);
                seeds[i].transform.localScale = big ? Vector3.one * 1.5f : Vector3.one;

                BattleControl.AttackProperty property = properties[UnityEngine.Random.Range(0, properties.Length)];
                battle.StartCoroutine(battle.Projectile(damage, property, battle.enemydata[actionid], playerTargetID, seeds[i].transform, 25f, 0, "keepcolor", null, "WoodHit", "", new Vector3(0,0,20),false));
                MainManager.PlaySound("PingShot");
                yield return EventControl.quartersec;
            }
            yield return new WaitUntil(()=> MainManager.ArrayIsEmpty(seeds));

        }

        //103 = going underground
        //104 = one straight line
        //105 = eat anim
        //106 = eat STILL

        IEnumerator DoUndergroundStrike(EntityControl entity, int actionid)
        {
            Vector3 baseModelPos = entity.model.localPosition;
            Vector3 baseEntityPos = entity.transform.position;

            entity.animstate = 103;
            yield return EventControl.tenthsec;

            entity.digging = true;
            MainManager.PlaySound("Digging", 9, 1.1f, 0.75f, true);
            while (entity.digtime < 29f)
            {
                yield return null;
            }

            entity.model.localPosition = entity.model.localPosition + Vector3.down * 4.95f;
            battle.GetSingleTarget();
            var playertargetentityRef = battle.playertargetentity;

            Vector3 targetPos = playertargetentityRef.transform.position;
            yield return LerpPosition(100f, entity.transform.position, targetPos + new Vector3(0f, 0f, -0.25f), entity.transform);

            MainManager.StopSound(9);
            yield return EventControl.quartersec;

            entity.animstate = 105;
            entity.digging = false;
            MainManager.PlaySound("DigPop2");
            while (entity.digtime < 29f)
            {
                yield return null;
            }

            yield return EventControl.quartersec;
            MainManager.PlaySound("Bite2", -1, 0.9f, 1f);
            int playerTargetID = battle.playertargetID;
            battle.DoDamage(actionid, playerTargetID, UNDERGROUND_STRIKE_DAMAGE, BattleControl.AttackProperty.Poison, battle.commandsuccess);
            
            yield return EventControl.halfsec;

            MainManager.PlaySound("Digging", 9, 1.1f, 0.75f, true);
            entity.digging = true;
            while (entity.digtime < 29f)
            {
                yield return null;
            }

            //goes back to base pos
            yield return LerpPosition(20f, entity.transform.position, baseEntityPos, entity.transform);
            entity.transform.position = baseEntityPos;
            entity.model.localPosition = baseModelPos;

            MainManager.StopSound(9);
            entity.digging = false;
            while (entity.digtime < 29f)
            {
                yield return null;
            }
            entity.animstate = 0;
            yield return null;
        }


        //Healsmoke
        //flip first bud
        //anim 113
        IEnumerator HealMars(EntityControl entity, int actionid, int marsIndex)
        {
            battle.dontusecharge = true;
            bool flip = entity.flip;

            entity.FaceTowards(battle.enemydata[marsIndex].battleentity.transform.position);
            entity.animstate = 113;
            yield return EventControl.tenthsec;

            entity.StartCoroutine(entity.ShakeSprite(new Vector3(0.1f, 0f), 20f));
            yield return EventControl.quartersec;

            MainManager.PlaySound("HealBreath");
            MainManager.PlayParticle("HealSmoke", null, entity.extras[1].transform.position + new Vector3(entity.flip ? 0.9f : (-0.9f), 0f, -0.1f), new Vector3(0f, (float)(entity.flip ? 90 : (-90)), 90f), 5f);
            yield return new WaitForSeconds(2);
            battle.Heal(ref battle.enemydata[marsIndex], 10, false);
            yield return null;

            entity.flip = flip;
            entity.animstate = 0;
        }

        IEnumerator DoSeedRain(EntityControl entity, int actionid)
        {
            battle.nonphyscal = true;
            BattleControl.AttackProperty[] properties =
            {
                BattleControl.AttackProperty.Poison,
                BattleControl.AttackProperty.DefDownOnBlock,
                BattleControl.AttackProperty.AtkDownOnBlock,
                BattleControl.AttackProperty.Ink,
                BattleControl.AttackProperty.Sticky
            };

            //114 start charge up
            //115 charge
            //116 release
            entity.animstate = 103;
            yield return EventControl.quartersec;


            int seedAmount = 4;
            SpriteRenderer[] seeds = new SpriteRenderer[seedAmount];

            entity.animstate = 114;
            yield return EventControl.halfsec;
            for (int i = 0; i < seedAmount; i++)
            {
                //charges up
                entity.animstate = 115;
                yield return EventControl.thirdsec;
                //shoots
                entity.animstate = 116;

                GameObject head = entity.extras[1];

                seeds[i] = MainManager.NewSpriteObject(head.transform.position, null, MainManager.itemsprites[0, 23]);
                seeds[i].material.color = new Color(0.63f, 0.129f, 0.129f);

                battle.StartCoroutine(LerpPosition(20, seeds[i].transform.position, seeds[i].transform.position + Vector3.up * 10, seeds[i].transform));
                MainManager.PlaySound("PingShot");
                yield return EventControl.quartersec;
            }

            entity.animstate = 0;
            yield return EventControl.halfsec;

            for (int i = 0; i < seedAmount; i++)
            {
                yield return EventControl.quartersec;
                battle.GetSingleTarget();
                yield return EventControl.tenthsec;

                BattleControl.AttackProperty property = properties[UnityEngine.Random.Range(0, properties.Length)];
                battle.StartCoroutine(battle.Projectile(SEED_DAMAGE, property, battle.enemydata[actionid], battle.playertargetID, seeds[i].transform, 40f, 0, "keepcolor", null, "WoodHit", "Fall2", new Vector3(0, 0, 20), false));
                yield return EventControl.quartersec;
                yield return EventControl.tenthsec;
            }
            yield return new WaitUntil(() => MainManager.ArrayIsEmpty(seeds));
        }

        class MarsBud : MonoBehaviour
        {
            public Vector3 target = Vector3.zero;
            public EntityControl entity;
            Transform bone;

            void Start()
            {
                bone = entity.extras[0].transform.parent.parent.parent.parent;
            }

            void LateUpdate()
            {
                if(entity.animstate == 102)
                {
                    bone.position = target;
                }
            }
        }
    }
}
