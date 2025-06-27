using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BFPlus.Extensions.EnemyAI
{
    public class BelosslowAI : AI
    {
        enum Attacks
        {
            Shell,
            HeavyToss,
            SuperSlam
        }

        int SHELL_DAMAGE = 8;
        int SEISMIC_SLAM_DAMAGE = 8;
        int SEISMIC_ROCK_DAMAGE = 2;
        int THROW_AMOUNT = 2;
        int THROW_DAMAGE = 5;
        int SUPER_SLAM_DAMAGE = 11;
        BattleControl battle = null;
        public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
        {
            //ABILITIES
            //Has extreme defense and attack power, but can only move on every other turn.
            //Starts moving every turn at 1 / 3 HP, at the cost of gaining ATK DWN for the rest of the fight.
            //Rocks and projectiles inflict DEF DOWN, while all other attacks inflict ATK DOWN.

            //ATTACKS
            //Super Slam -High damage to one bug. Causes rocks to fall over the bugs it didn't hit on the next turn. (Damage + Delayed attacks)
            //Heavy Toss -Pulls out and tosses several objects at a single bug. Each object can be either a small boulder or an enemy. Thrown enemies can join the fight if there's space for them, but they won't be at full health.		
            //Charge - Gains + 1 Charge and Defense. (Only used once, after reaching 2 / 3 HP)
            //Seismic Jump -Jumps and slams down on all bugs for AoE damage. Afterwards, a ton of rocks fall down on random bugs for a LOT of extra hits. (Only used after Charge)
            battle = MainManager.battle;
            if (battle.enemydata[actionid].data == null)
            {
                battle.enemydata[actionid].data = new int[4];
            }

            float hpPercent = battle.HPPercent(battle.enemydata[actionid]);

            if (hpPercent <= 0.45f && battle.enemydata[actionid].data[2] != 1)
            {
                //atk down
                entity.animstate = 102;
                entity.StartCoroutine(entity.ShakeSprite(0.1f, 45f));
                battle.StartCoroutine(battle.StatEffect(battle.enemydata[actionid].battleentity, 0));
                MainManager.SetCondition(MainManager.BattleCondition.AttackDown, ref battle.enemydata[actionid], 9999999);
                battle.enemydata[actionid].data[2] = 1;
                MainManager.PlaySound("StatUp");
                battle.enemydata[actionid].data[0] = 0;
                yield return EventControl.sec;
            }

            if (hpPercent <= 0.66f && battle.enemydata[actionid].data[1] == 0)
            {
                //charge + def up;
                battle.dontusecharge=true;

                entity.animstate = 102;
                entity.StartCoroutine(entity.ShakeSprite(0.1f, 45f));
                MainManager.PlaySound("Charge7");
                yield return EventControl.sec;

                battle.StartCoroutine(battle.StatEffect(battle.enemydata[actionid].battleentity, 4));
                battle.StartCoroutine(battle.StatEffect(battle.enemydata[actionid].battleentity, 1));
                battle.enemydata[actionid].charge = 1;
                MainManager.SetCondition(MainManager.BattleCondition.DefenseUp, ref battle.enemydata[actionid], 9999999);
                MainManager.PlaySound("StatUp");
                yield return EventControl.halfsec;
                battle.enemydata[actionid].data[1] = 1;
                battle.enemydata[actionid].data[0] = 0;
                yield break;
            }
 

            if (battle.enemydata[actionid].data[0] == 0)
            {
                if(battle.enemydata[actionid].data[1] == 1)
                {
                    battle.enemydata[actionid].data[1] = 2;
                    yield return DoSeismicSlam(entity, actionid);
                }
                else
                {
                    Dictionary<Attacks, int> attacks = new Dictionary<Attacks, int>()
                    {
                        { Attacks.Shell, 30},
                        { Attacks.HeavyToss, 40},
                        { Attacks.SuperSlam, 30},
                    };
                    Attacks attack = MainManager_Ext.GetWeightedResult(attacks);

                    switch (attack)
                    {
                        case Attacks.Shell:
                            yield return DoShellAttack(entity, actionid);
                            break;
                        case Attacks.HeavyToss:
                            yield return DoHeavyToss(entity, actionid);
                            break;
                        case Attacks.SuperSlam:
                            yield return DoSuperSlam(entity, actionid);
                            break;
                    }
                }

                if (battle.enemydata[actionid].data[2] != 1)
                    battle.enemydata[actionid].data[0] = 1;
            }
            else
            {
                MainManager.PlaySound("Lost",0.8f,1);
                entity.Emoticon(MainManager.Emoticons.QuestionMark, 45);
                battle.enemydata[actionid].data[0] = 0;
                yield return EventControl.halfsec;
            }
            
            yield return null;

        }


        IEnumerator DoHeavyToss(EntityControl entity, int actionid)
        {
            bool hardmode = battle.HardMode();
            battle.nonphyscal = true;

            List<EntityControl> enemySpawns = new List<EntityControl>();

            bool summoned = false;
            for (int i = 0; i < THROW_AMOUNT; i++)
            {
                if (MainManager.GetAlivePlayerAmmount() > 0)
                {
                    //rock = 0, enemy = 1
                    int throwType = UnityEngine.Random.Range(0, 2);
                    //dig up rock

                    GameObject rock;

                    if (throwType == 0)
                    {
                        rock = MainManager.CreateRock(new Vector3(0f, -999f), Vector3.one * 0.65f, Vector3.zero);
                    }
                    else
                    {
                        MainManager.AnimIDs[] possibleEnemies = new MainManager.AnimIDs[] { MainManager.AnimIDs.Plumpling, MainManager.AnimIDs.Sandworm, MainManager.AnimIDs.Mantidfly };
                        //randomize enemy type
                        int thrownEnemy = (int)possibleEnemies[UnityEngine.Random.Range(0, possibleEnemies.Length)] - 1;
                        EntityControl enemy = EntityControl.CreateNewEntity("enemy", thrownEnemy, new Vector3(0f, -999f));
                        enemy.gameObject.layer = 9;
                        enemy.battle = true;
                        yield return null;
                        enemy.LockRigid(true);
                        enemy.animstate = (int)MainManager.Animations.Hurt;
                        rock = enemy.gameObject;
                    }
                    entity.animstate = 100;
                    yield return EventControl.halfsec;
                    MainManager.PlaySound("RockPluck");
                    MainManager.ShakeScreen(0.25f, 0.2f);

                    Vector3 startPos = entity.transform.position + new Vector3(-2f, -3f, -0.1f);
                    Vector3 endPos = entity.transform.position + battle.enemydata[actionid].itemoffset;
                    MainManager.PlayParticle("DirtExplode2", startPos + new Vector3(0f, 3f));

                    entity.animstate = 101;
                    float a = 0f;
                    float b = 10f;
                    do
                    {
                        rock.transform.position = Vector3.Lerp(startPos, endPos, a / b);
                        a += MainManager.TieFramerate(1f);
                        yield return null;
                    }
                    while (a < b + 1f);
                    entity.animstate = 4;

                    yield return EventControl.quartersec;
                    battle.GetSingleTarget();

                    EntityControl playerTargetEntityRef = battle.playertargetentity;

                    MainManager.battle.MiddleCamera(entity.transform, playerTargetEntityRef.transform);

                    MainManager.PlaySound("Charge19");
                    battle.StartCoroutine(entity.ShakeSprite(0.2f, 30f));
                    yield return EventControl.halfsec;
                    entity.animstate = 28;
                    MainManager.PlaySound("Toss12");
                    a = 0f;
                    b = (!hardmode) ? 42.5f : 35f;
                    startPos = rock.transform.position;
                    endPos = playerTargetEntityRef.transform.position + new Vector3(0f, 2f, -0.1f);
                    do
                    {
                        rock.transform.position = MainManager.BeizierCurve3(startPos, endPos, 5f, a / b);
                        a += MainManager.TieFramerate(1f);
                        yield return null;
                    }
                    while (a < b + 1f);
                    MainManager.ShakeScreen(0.2f, 0.65f, true);

                    MainManager.battle.DoDamage(actionid, MainManager.battle.playertargetID, THROW_DAMAGE, BattleControl.AttackProperty.DefDownOnBlock, battle.commandsuccess);

                    entity.animstate = 0;

                    if (throwType == 0)
                    {
                        MainManager.PlaySound("RockBreak");
                        MainManager.CrackRock(rock.transform, true);
                    }
                    else
                    {
                        //enemy bounce behind team snek,
                        //enemy spawn in battle as a enemy if theres enough space
                        bool enoughSpace = battle.enemydata.Length < 3 && !summoned;
                        Vector3? vector = null;
                        bool newEnemy = false;
                        if (enoughSpace)
                        {
                            Vector3[] points = new Vector3[]
                            {
                                new Vector3(0.5f, 0f, 0f),
                                new Vector3(6.55f, 0f, 0f)
                            };

                            vector = battle.GetFreeSpace(points, 1f, true);
                            if (vector != null)
                            {
                                newEnemy = true;
                                summoned = true;
                                enemySpawns.Add(rock.GetComponent<EntityControl>());
                            }
                        }

                        if (vector == null)
                            vector = new Vector3(-15f, 5f, 0f);
                        yield return MainManager.ArcMovement(rock, endPos, vector.Value + new Vector3(0f, 0f, -0.2f), Vector3.zero, Mathf.Clamp(Vector3.Distance(endPos, vector.Value), 2f, 4.5f), 35f, !newEnemy);
                        if (newEnemy)
                            enemySpawns[enemySpawns.Count - 1].animstate = 0;
                    }
                }
            }

            if (enemySpawns.Count > 0)
            {
                yield return new WaitForSeconds(0.75f);
                for (int i = 0; i < enemySpawns.Count; i++)
                {
                    EntityControl enemy = enemySpawns[i];
                    int enemyId = 0;

                    //can prolly just parse the string to other enemy enum idk
                    switch ((MainManager.AnimIDs)enemy.animid + 1)
                    {
                        case MainManager.AnimIDs.Plumpling:
                            enemyId = (int)MainManager.Enemies.Plumpling;
                            break;
                        case MainManager.AnimIDs.Sandworm:
                            enemyId = (int)MainManager.Enemies.Sandworm;
                            break;
                        case MainManager.AnimIDs.Mantidfly:
                            enemyId = (int)MainManager.Enemies.Mantidfly;
                            break;
                    }
                    battle.AddNewEnemy(enemyId, new Vector3(0f, 999f));
                    yield return null;

                    battle.enemydata[battle.lastaddedid].battleentity.transform.position = enemy.transform.position;
                    battle.enemydata[battle.lastaddedid].cantmove = 1;
                    UnityEngine.Object.Destroy(enemy.gameObject);
                }
            }

            yield return null;
        }

        IEnumerator DoSuperSlam(EntityControl entity, int actionid)
        {
            battle.GetSingleTarget();
            battle.CameraFocusTarget();

            var playerTargetEntityRef = battle.playertargetentity;

            Vector3 startPosition = entity.transform.position;
            int baseState = entity.animstate;
            bool flip = entity.flip;

            entity.MoveTowards(playerTargetEntityRef.transform.position + new Vector3(2.25f, 0f, -0.1f), 2f, 1, 102);
            while (entity.forcemove)
            {
                yield return null;
            }
            MainManager.PlaySound("AhoneynationBodySlamCharge", -1, 0.5f, 1f);
            MainManager.SetCamera(entity.transform, null, 0.075f, MainManager.defaultcamoffset + new Vector3(-1f, 0f, 3.5f));
            yield return EventControl.sec;
            yield return EventControl.halfsec;

            MainManager.PlaySound("Thud3");
            entity.animstate = 28;

            int originalTarget = battle.playertargetID;
            battle.DoDamage(actionid, originalTarget, SUPER_SLAM_DAMAGE,BattleControl.AttackProperty.Pierce, battle.commandsuccess);

            MainManager.PlayParticle("DirtExplode", playerTargetEntityRef.transform.position + new Vector3(0f, -1f));
            battle.playertargetID = -1;

            MainManager.PlaySound("Rumble", 9, 1f, 1f, true);
            yield return new WaitForSeconds(0.15f);
            BattleControl.SetDefaultCamera();
            MainManager.ShakeScreen(0.1f, -1f);
            battle.StartCoroutine(MoveBack(entity, startPosition, baseState, flip));
            yield return EventControl.halfsec;
            //instantiate falling dirt particles on both teammates that didnt get hit., setup del proj on them
            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.instance.playerdata[i].trueid != originalTarget && MainManager.instance.playerdata[i].hp > 0)
                {
                    battle.StartCoroutine(CreateRockProj(MainManager.instance.playerdata[i].battleentity, actionid, i));
                }
            }
            yield return EventControl.sec;
            yield return EventControl.halfsec;

            while (entity.forcemove)
            {
                yield return null;
            }
            MainManager.StopSound("Rumble");
            MainManager.screenshake = Vector3.zero;
        }

        IEnumerator DoShellAttack(EntityControl entity, int actionid)
        {
            Vector3 startPosition = entity.transform.position;
            entity.animstate = 103;
            yield return EventControl.tenthsec;
            MainManager.PlaySound("Grow2", 9, 1.2f, 0.4f);
            yield return new WaitForSeconds(0.733f);
            entity.animstate = 104;
            var hurricanePart = MainManager.PlayParticle("HurricaneBig", entity.transform.position);
            hurricanePart.transform.parent = entity.spritetransform;
            hurricanePart.transform.localScale = new Vector3(2, 1, 1);
            var shape = hurricanePart.GetComponent<ParticleSystem>().shape;
            shape.angle = 15;
            shape.radius = 0.05f;

            var emission = hurricanePart.GetComponent<ParticleSystem>().emission;
            emission.rateOverTime = 10;
            MainManager.PlaySound("Spin", 9, 0.8f, 1f, true);
            float a = 0f;
            float b = 30f;
            do
            {
                entity.spin = Vector3.Lerp(Vector3.zero, Vector3.up * -30f, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b);
            yield return EventControl.halfsec;
            MainManager.PlaySound("Woosh3", -1, 0.7f, 1f);

            a = 0f;
            b = 30f;
            bool hit = false;
            do
            {
                entity.transform.position = Vector3.Lerp(startPosition, new Vector3(-15f, 0f, 0f), a / b);
                if (!hit && entity.transform.position.x < -4f)
                {
                    for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                    {
                        if (MainManager.instance.playerdata[i].hp > 0)
                        {
                            battle.DoDamage(actionid, i, SHELL_DAMAGE, null, battle.commandsuccess);
                            if (!battle.commandsuccess)
                            {
                                battle.StartCoroutine(MainManager.instance.playerdata[i].battleentity.SlowSpinStop(new Vector3(0f, 30f), 60f));
                                battle.StartCoroutine(MainManager.instance.playerdata[i].battleentity.OverrideJumpTemp());
                                MainManager.instance.playerdata[i].battleentity.Jump();
                            }
                        }
                    }
                    hit = true;
                    MainManager.StopSound(9);
                }
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b);

            entity.transform.position = new Vector3(15f, 0f);
            a = 0f;
            b = 20f;
            do
            {
                entity.transform.position = Vector3.Lerp(new Vector3(15f, 0f), startPosition, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b);
            entity.spin = Vector3.zero;
            MainManager.StopSound(9);
            UnityEngine.Object.Destroy(hurricanePart);
            entity.animstate = 105;
            yield return EventControl.tenthsec;
            MainManager.PlaySound("Grow1", 9, 1.2f, 0.5f);
            yield return EventControl.sec;
            yield return null;
        }

        IEnumerator MoveBack(EntityControl entity, Vector3 position, int baseState, bool flip)
        {
            entity.MoveTowards(position, 2f, entity.walkstate, baseState);
            while (entity.forcemove)
            {
                yield return null;
            }
            entity.transform.position = position;
            entity.flip = flip;
        }

        IEnumerator CreateRockProj(EntityControl targetEntity, int actionid, int targetID)
        {
            int PROJ_DAMAGE = 6;

            var rock = MainManager.CreateRock(new Vector3(targetEntity.transform.position.x, 20f, targetEntity.transform.position.z), new Vector3(0.3f, 0.3f, 0.3f), Vector3.zero);
            rock.AddComponent<SpinAround>().itself = new Vector3(3, 3, 3);

            yield return null;

            UnityEngine.Object.Instantiate(MainManager_Ext.assetBundle.LoadAsset("DirtFalling"), rock.transform.position + new Vector3(0, -10f), Quaternion.identity);
            targetEntity.Emoticon(MainManager.Emoticons.QuestionMark, 100);
            yield return EventControl.sec;
            battle.AddDelayedProjectile(rock, targetID, PROJ_DAMAGE, 1, 0, BattleControl.AttackProperty.DefDownOnBlock, 55f, battle.enemydata[actionid], null, null, "Fall2");
        }


        IEnumerator DoSeismicSlam(EntityControl entity, int actionid)
        {
            Vector3 startPosition = entity.transform.position;
            int baseState = entity.animstate;
            bool flip = entity.flip;
            entity.overrridejump = true;
            entity.LockRigid(true);

            MainManager.PlaySound("AhoneynationBodySlamCharge", -1, 0.5f, 1f);
            MainManager.SetCamera(entity.transform, null, 0.075f, MainManager.defaultcamoffset + new Vector3(0f, 0f, 1.5f));
            entity.animstate = 102;
            yield return EventControl.sec;
            yield return EventControl.halfsec;

            MainManager.SetCamera(null, new Vector3?(new Vector3(-4f, 1f)), 0.055f);
            yield return EventControl.tenthsec;
            entity.spin = new Vector3(0f, 20f);

            entity.animstate = 104;
            Vector3 startPos = entity.transform.position;
            MainManager.PlaySound("AhoneynationBodySlamJump", -1, 0.5f, 1f);

            float a = 0f;
            float b = 50f;
            do
            {
                entity.transform.position = MainManager.BeizierCurve3(startPos, battle.partymiddle, 15f, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b + 1f);

            entity.transform.position = battle.partymiddle;
            entity.spin = Vector3.zero;
            MainManager.PlayParticle("impactsmoke", entity.transform.position);
            MainManager.ShakeScreen(0.3f, 0.5f);
            battle.PartyDamage(actionid, SEISMIC_SLAM_DAMAGE, null, battle.commandsuccess);
            MainManager.PlaySound("Thud3");

            entity.animstate = 105;
            yield return EventControl.tenthsec;
            entity.LockRigid(false);
            entity.overrridejump = false;

            battle.StartCoroutine(MoveBack(entity, startPosition, baseState, flip));
            yield return EventControl.thirdsec;

            MainManager.PlaySound("Rumble", 9, 1f, 1f, true);
            yield return new WaitForSeconds(0.15f);
            BattleControl.SetDefaultCamera();
            MainManager.ShakeScreen(0.1f, -1f);

            battle.nonphyscal = true;
            int rockAmount = 8;
            for (int i = 0; i < rockAmount; i++)
            {

                if (MainManager.GetAlivePlayerAmmount() == 0)
                    break;
                int playerID = battle.GetRandomAvaliablePlayer();
                EntityControl targetEntity = MainManager.instance.playerdata[playerID].battleentity;

                var rock = MainManager.CreateRock(new Vector3(targetEntity.transform.position.x, 20f, targetEntity.transform.position.z), new Vector3(0.2f, 0.2f, 0.2f), Vector3.zero);
                rock.AddComponent<SpinAround>().itself = new Vector3(3, 3, 3);

                MainManager.PlaySound("Fall",0.9f,0.7f);

                Vector3 startrockPos = rock.transform.position;
                a = 0;
                b = 40f;
                do
                {
                    rock.transform.position = MainManager.SmoothLerp(startrockPos, targetEntity.transform.position + Vector3.left * 0.1f + Vector3.up, a / b);
                    a += MainManager.framestep;
                    yield return null;
                }
                while (a < b);

                battle.DoDamage(actionid, playerID, SEISMIC_ROCK_DAMAGE, null, battle.commandsuccess);
                UnityEngine.Object.Destroy(rock);
            }
            yield return EventControl.halfsec;
            while (entity.forcemove)
            {
                yield return null;
            }
            MainManager.StopSound("Rumble");
            MainManager.screenshake = Vector3.zero;
            yield return null;
        }
    }
}
