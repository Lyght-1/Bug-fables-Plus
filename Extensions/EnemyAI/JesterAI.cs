using BFPlus.Patches;
using HarmonyLib;
using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static FlappyBee;
using static MainManager;

namespace BFPlus.Extensions.EnemyAI
{
//    Giant's Jester

//The jester would also have a passive ability where it HEALS from being on fire instead of taking damage

//Gets +1 action per turn att 65% HP.
//Gets Def Up at 35 % HP

//Attacks:

//"Fiery Wind-Up Punch"
//The Jester will lean back and wind up its left hand for a punch across the screen[a good reference for the anim would be Sonic's forward smash in Smash Bros Ultimate]. Right before releasing the punch, it would make some sort of sound que and change its facial expression, to let you know when the punch is coming, and it would be amazing if it could hold the wind-up for a random amount of time between 2-5 seconds . The attack would deal 4 damage to a single target (6 on hard mode) and it would light the target on fire for 1 turn if blocked, and for 3 turns if unblocked (in hard mode it increases to 2 turns if blocked and 4 turns if unblocked).

//"Fireball Juggler"
//The jester would juggle 3 fireballs(4 in hard mode) for a brief moment before lobbing the balls at a random party memeber.These fireballs would deal 3 damage(4 on hard mode) and would light the target on fire for 2 turns if unblocked(3 turns in Hard Mode).
//It would have a 50 % chance to drop the last fireball, healing itself the 4 HP(6 HP on Hard Mode), but it will be lit on fire for 2 / 3 turns

//"Mega-Fire-Cherry Surprise"
//The Jester would go into its box before re - emerging with a giant cherry bomb on fire(will draw sprite for this in a bit.) held over its head.
//The jester would lift the bomb upwards and off - screen as a coundown ranging from 4 - 8 seconds start counting down.
//The second the counter hits 0, the jester insantly slams down the bomb in the middle of the party, dealing 5 damage to the party(7 in hard mode), inflicting burn for 2 turns if unblocked, as well as another unavoidable random negative status effect to each party memer(aside from Freeze, Sleep or Numb) for 2 turns.

//"Jester Says"
//The jester would dive into its box and re - emerge with a sign displaying either the A, B, or X button.The party would have a bried window of time to press the correct button, making the Jester go back into its box and re - emerge with another button.
//This would repeat 3 times on normal mode and 4 times on hard mode.failing at any of these will make the jester re - emerge and snap it's fingers on its right hand, and everyone would be set on fire for 3 turns. Succeeding would instead make the jester heal everyone for 3 HP.

//"Bug Catcher"
//The Jester would hop over to the bug at the front of the party and grab it with both hands and slowly lift them upwards.The caught bug would have to quickly mash to not get caught, similar to a Mantidfly attack.If the Jester catches the bug, it will hold it hostage until its next turn.

//"Bug Bullseye"
//if the Jester has captured a bug, it will always do this on its next turn.It will use the caught bug as a projectile.It will first light the caught bug on fire for 2 turns(3 turns in hard mode) with a finger snap, and then throw the bug at another party member, dealing 6 damage to both party members(8 in hard mode).If not blocked, the targeted bug would also catch fire for as many turns.

//"Jumping Jack"
//The jester would retreat into its box, becoming sturdy and gaining 3 charge.After a turn, the jester would preform a big jump with its entire box, landing on the team and setting them on fire for 1 turn if blocked, and 3 turns if unblocked(2 / 4 turns in hard mode) whilst dealing 5 ? damage to the party(7 in hard mode)

    public class JesterAI : AI
    { 
        enum Attacks
        {
            Punch,
            Juggling,
            CherryBomb,
            JesterSays,
            BugCatcher,
            ChargeBox
        }

        BattleControl battle = null;
        const int PUNCH_DAMAGE = 4;
        const int FIREBALL_DAMAGE = 3;
        const int BULLSEYE_DAMAGE = 6;
        const int FIREBALL_DRAINTP = 2;
        const int CHERRY_BOMB_DAMAGE = 4;
        const int JUMPING_JACK_DAMAGE = 5;
        const int JESTER_SAYS_AMOUNT = 4;
        public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
        {
            battle = MainManager.battle;
            battle.SetData(actionid, 12);
            float hpPercent = battle.HPPercent(battle.enemydata[actionid]);

            if(battle.enemydata[actionid].data[0] != 1)
            {
                int count = 0;
                for (int i = 7; i < 12; i++)
                {
                    if (battle.enemydata[actionid].data[i] == 0 && hpPercent <= (1 - 0.2f * count))
                    {
                        yield return TellJoke(count, entity, i, actionid);
                        break;
                    }
                    count++;
                }
            }

            if (hpPercent <= 0.65f && battle.enemydata[actionid].data[4] == 0)
            {
                if(battle.enemydata[actionid].data[0] == 0)
                    entity.animstate = 129;
                MainManager.PlaySound("Charge7", -1, 1.2f, 1f);
                battle.StartCoroutine(entity.ShakeSprite(0.1f, 30f));
                yield return EventControl.halfsec;
                battle.enemydata[actionid].moves = 2;
                battle.enemydata[actionid].cantmove--;
                battle.StartCoroutine(battle.StatEffect(entity, 5));
                MainManager.PlaySound("StatUp");
                battle.enemydata[actionid].data[4] = 1;
                yield return EventControl.halfsec;
            }

            if (hpPercent <= 0.35f && battle.enemydata[actionid].data[5] == 0)
            {
                if (battle.enemydata[actionid].data[0] == 0)
                    entity.animstate = 129;
                entity.StartCoroutine(entity.ShakeSprite(0.1f, 45f));
                MainManager.PlaySound("Charge7");
                yield return EventControl.sec;

                battle.StartCoroutine(battle.StatEffect(battle.enemydata[actionid].battleentity, 1));
                MainManager.SetCondition(MainManager.BattleCondition.DefenseUp, ref battle.enemydata[actionid], 9999999);
                MainManager.PlaySound("StatUp");
                yield return EventControl.halfsec;
                battle.enemydata[actionid].data[5] = 1;
            }

            if (battle.enemydata[actionid].ate != null)
            {
                if (battle.enemydata[actionid].data[6] <= 0)
                {
                    yield return DoBugBullseye(entity, actionid, false);
                    yield break;
                }
                battle.enemydata[actionid].data[6]--;
            }
            else
            {
                battle.enemydata[actionid].data[6] = 0;
            }

            if (battle.enemydata[actionid].data[0] == 1)
            {
                yield return DoJumpingJack(entity, actionid);
                yield break;
            }

            Dictionary<Attacks, int> attacks = new Dictionary<Attacks, int>()
            {
                { Attacks.Punch, 20},
                { Attacks.Juggling, 17},
                { Attacks.JesterSays, 18},
                { Attacks.CherryBomb, 19},
                { Attacks.ChargeBox, 16},
                { Attacks.BugCatcher, 10},
            };
            attacks[Attacks.BugCatcher] += battle.enemydata[actionid].data[3] * 5;

            if (battle.enemydata[actionid].ate != null)
            {
                attacks.Remove(Attacks.BugCatcher);
                attacks.Remove(Attacks.ChargeBox);
            }

            if (MainManager.GetAlivePlayerAmmount() == 1)
                attacks.Remove(Attacks.BugCatcher);

            //Prevents Jester Says from being spammed
            if (battle.enemydata[actionid].data[1] == 1)
            {
                attacks.Remove(Attacks.JesterSays);
                battle.enemydata[actionid].data[1] = 0;
            }

            //Prevents ChargeBox from being spammed
            if (battle.enemydata[actionid].data[2] == 1)
            {
                attacks.Remove(Attacks.ChargeBox);
                battle.enemydata[actionid].data[2] = 0;
            }

            Attacks attack = MainManager_Ext.GetWeightedResult(attacks);

            if(attack != Attacks.BugCatcher && battle.enemydata[actionid].ate == null)
            {
                battle.enemydata[actionid].data[3]++;
            }
            else
            {
                battle.enemydata[actionid].data[3] = 0;
            }

            switch (attack)
            {
                case Attacks.Punch:
                    yield return DoWindUpPunch(entity, actionid);
                    break;
                case Attacks.Juggling:
                    yield return DoFireballJuggling(entity, actionid);
                    break;
                case Attacks.CherryBomb:
                    yield return DoCherryBomb(entity,actionid);
                    break;
                case Attacks.ChargeBox:
                    yield return DoBoxCharge(entity, actionid);
                    break;
                case Attacks.BugCatcher:
                    yield return DoBugCatcher(entity, actionid);
                    break;
                case Attacks.JesterSays:
                    yield return DoJesterSays(entity, actionid);
                    break;
            }
        }

        IEnumerator TellJoke(int jokeId, EntityControl entity, int flag, int actionid)
        {
            MainManager.SetCamera(new Vector3(entity.transform.position.x -2, entity.sprite.transform.localPosition.y + 3f, 1f));
            yield return EventControl.halfsec;
            string[] jokes = MainManager_Ext.assetBundle.LoadAsset<TextAsset>("JesterJokes").ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            MainManager.DialogueText(jokes[jokeId],
                entity.transform, null);

            while (MainManager.instance.message)
            {
                yield return null;
            }
            battle.enemydata[actionid].data[flag] = 1;
            BattleControl.SetDefaultCamera();
            yield return EventControl.quartersec;
        }

        IEnumerator DoJesterSays(EntityControl entity, int actionid)
        {
            battle.nonphyscal = true;
            battle.dontusecharge = true;
            battle.enemydata[actionid].data[1] = 1;
            bool hardmode = battle.HardMode();
            int amount = hardmode ? JESTER_SAYS_AMOUNT+1:JESTER_SAYS_AMOUNT;

            entity.animstate = 117;
            yield return EventControl.halfsec;
            entity.animstate = 123;
            MainManager.PlaySound("BagRustle");
            Transform[] mainParts = GetMainParts(entity);

            SpriteRenderer card = MainManager.NewSpriteObject(mainParts[0].transform.position, battle.battlemap.transform, MainManager_Ext.assetBundle.LoadAsset<Sprite>("JesterCard"));
            int buttonId = UnityEngine.Random.Range(4, 7);
            ButtonSprite button = new GameObject("Button").AddComponent<ButtonSprite>().SetUp(buttonId, -1, "", new Vector3(0,0,-0.1f), Vector3.one * 1.3f, -1, card.transform, Color.white);
            button.tridimentional = true;

            SpriteRenderer barHolder = MainManager.NewUIObject("barHolder", card.transform, new Vector3(-0.9f, 2f, -0.1f), new Vector3(2f, 1f, 1f), MainManager.guisprites[64], 0).GetComponent<SpriteRenderer>();
            SpriteRenderer bar = MainManager.NewUIObject("bar", barHolder.transform, new Vector3(0,0,-0.1f), Vector3.one, MainManager.guisprites[58], 0).GetComponent<SpriteRenderer>();
            bar.color = Color.yellow;
            float a = 0;
            float b = 20f;
            Vector3 startPos = card.transform.position;
            Vector3 targetPos = card.transform.position + new Vector3(-2.2f, 3.5f, -0.3f);
            do
            {
                card.transform.position = Vector3.Lerp(startPos, targetPos, a / b);
                card.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one*0.8f, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);
                
            card.transform.parent = mainParts[0].transform;

            bool failed = false;

            yield return EventControl.tenthsec;
            for (int i = 0; i < amount; i++)
            {
                a = 0;
                b = 35f - (i *2);
                bool success = false;
                do
                {
                    bar.transform.localScale = new Vector3(Mathf.Lerp(1,0,a/b), 1f, 1f);
                    if (MainManager.GetKey(buttonId, false))
                    {
                        success = true;
                        MainManager.PlaySound("ACReady");
                        button.basesprite.color = Color.green;
                        break;
                    }
                    else if(MainManager.GetKey(-4, false))
                    {
                        failed = true;
                        button.basesprite.color = Color.red;
                        MainManager.PlayBuzzer();
                        break;
                    }
                    a += MainManager.TieFramerate(1f);
                    yield return null;
                } while (a < b+1);

                if (!success)
                {
                    failed = true;
                    button.basesprite.color = Color.red;
                    MainManager.PlayBuzzer();
                }

                Vector3 baseRotation = card.transform.localEulerAngles;
                Vector3 baseScale = card.transform.localScale;
                Vector3 basePos = card.transform.localPosition;

                card.transform.parent = battle.battlemap.transform;
                GameObject tempCard = card.gameObject;

                if (i < amount - 1 && !failed)
                {
                    tempCard = UnityEngine.Object.Instantiate(card.gameObject);
                    UnityEngine.Object.Destroy(tempCard.GetComponentInChildren<ButtonSprite>());
                    UnityEngine.Object.Destroy(tempCard.transform.Find("barHolder").gameObject);
                    tempCard.transform.position = card.transform.position + new Vector3(0, 0, -0.3f);
                }
                entity.animstate = failed ? 126 : 125;
                MainManager.PlaySound("Toss6", 0.8f, 1);
                yield return EventControl.tenthsec;

                battle.StartCoroutine(MainManager.ArcMovement(tempCard.gameObject, tempCard.transform.position, tempCard.transform.position + Vector3.right * 15,new Vector3(0,0,20), 10, 30f,true));

                if (!failed && i < amount-1)
                {
                    bar.transform.localScale = Vector3.one;
                    buttonId = UnityEngine.Random.Range(4, 7);
                    UnityEngine.Object.Destroy(card.transform.GetComponentInChildren<ButtonSprite>().gameObject);
                    button = new GameObject("Button").AddComponent<ButtonSprite>().SetUp(buttonId, -1, "", new Vector3(0, 0, -0.1f), Vector3.one * 1.3f, -1, card.transform, Color.white);
                    button.tridimentional = true;
                    yield return EventControl.tenthsec;
                    card.transform.parent = mainParts[0].transform;
                    card.transform.localEulerAngles = baseRotation;
                    card.transform.localScale = baseScale;
                    card.transform.localPosition = basePos;
                    yield return EventControl.tenthsec;
                    entity.animstate = 124;
                }
                else
                {
                    break;
                }
            }

            entity.animstate = 127;
            yield return EventControl.halfsec;
            MainManager.PlaySound("Block2", 1.1f, 1);
            MainManager.PlayParticle("Gleam", "Gleam", mainParts[1].transform.position + new Vector3(-1.1f, 1f, -0.3f), new Vector3(-90f, 0f), 0.5f);

            if (failed)
            {
                MainManager.SetCondition(MainManager.BattleCondition.Fire, ref battle.enemydata[actionid], 3);
                MainManager.PlayParticle("Fire", entity.model.GetChild(0).GetChild(6).position);
            }
            else
                battle.Heal(ref battle.enemydata[actionid], 3);

            for (int i=0; i< MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.instance.playerdata[i].hp > 0 && MainManager.instance.playerdata[i].eatenby == null)
                {
                    if (failed)
                    {
                        MainManager.PlayParticle("Fire", MainManager.instance.playerdata[i].battleentity.transform.position);
                        battle.TryCondition(ref MainManager.instance.playerdata[i], MainManager.BattleCondition.Fire, 4);
                    }
                    else
                    {
                        battle.Heal(ref MainManager.instance.playerdata[i], 3);
                    }
                }
            }
            yield return EventControl.sec;
        }

        IEnumerator DoBoxCharge(EntityControl entity, int actionid)
        {
            battle.enemydata[actionid].data[2] = 1;
            battle.dontusecharge = true;
            entity.animstate = 101;
            entity.overrideanim = true;
            Entity_Ext.GetEntity_Ext(entity).overrideDamageAnim = true;
            yield return EventControl.halfsec;

            entity.model.GetChild(1).GetChild(2).gameObject.SetActive(false);
            battle.enemydata[actionid].cursoroffset = new Vector3(0.2f, 2.5f);
            entity.StartCoroutine(entity.ShakeSprite(0.2f, 60f));
            MainManager.PlaySound("Charge7");
            yield return EventControl.sec;

            battle.StartCoroutine(battle.StatEffect(battle.enemydata[actionid].battleentity, 4));
            battle.StartCoroutine(battle.StatEffect(battle.enemydata[actionid].battleentity, 1));
            battle.enemydata[actionid].charge = 3;
            MainManager.SetCondition(MainManager.BattleCondition.Sturdy, ref battle.enemydata[actionid], 2);
            MainManager.PlaySound("StatUp");

            entity.basestate = 100;
            BattleControl_Ext.Instance.startState = entity.basestate;
            battle.enemydata[actionid].data[0] = 1;
            yield return null;
        }

        IEnumerator DoJumpingJack(EntityControl entity, int actionid)
        {
            Entity_Ext.GetEntity_Ext(entity).overrideDamageAnim = false;
            entity.animstate = 100;
            bool hardmode = battle.HardMode();
            battle.enemydata[actionid].data[0] = 0;
            Vector3 basePos = entity.model.position;

            entity.LockRigid(true);
            entity.overridefly = true;
            entity.overrridejump = true;

            entity.StartCoroutine(entity.ShakeSprite(0.1f, 60f));
            MainManager.PlaySound("AhoneynationBodySlamCharge",1.2f,1);
            Vector3 startScale = entity.model.transform.localScale;
            Vector3 targetScale = new Vector3(startScale.x, 0.5f,startScale.z);

            yield return MainManager.GradualScale(entity.model.transform, targetScale, 60f, false);
            battle.StartCoroutine(MainManager.GradualScale(entity.model.transform, startScale, 10f, false));
            Vector3 baseRotation = entity.model.localEulerAngles;

            MainManager.PlaySound("Boing0", 0.8f, 1);
            MainManager.PlaySound("BeetleDash",-1, 0.8f, 0.7f,true);
            yield return MainManager.ArcMovement(entity.model.gameObject, entity.model.position, battle.partymiddle + Vector3.up, new Vector3(-30,0,30), 15, 60f, false);
            MainManager.StopSound("BeetleDash");
            entity.model.localEulerAngles = baseRotation;

            battle.StartCoroutine(MainManager.GradualScale(entity.model.transform, targetScale, 10f, false));

            MainManager.PlaySound("BigHit");
            MainManager.PlayParticle("impactsmoke", battle.partymiddle);
            
            int damage = hardmode ? JUMPING_JACK_DAMAGE + 1 : JUMPING_JACK_DAMAGE;
            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.instance.playerdata[i].hp > 0 && MainManager.instance.playerdata[i].eatenby == null)
                {
                    battle.DoDamage(actionid, i, damage, null, battle.commandsuccess);

                    if (MainManager.instance.playerdata[i].hp > 0)
                    {
                        int fireTurns = hardmode ? 4 : 3;
                        if (battle.commandsuccess)
                        {
                            fireTurns = hardmode ? 2 : 1;
                        }
                        battle.TryCondition(ref MainManager.instance.playerdata[i], MainManager.BattleCondition.Fire, fireTurns);
                    }

                }
            }
            
            yield return EventControl.quartersec;

            MainManager.PlaySound("Boing0", 0.8f, 1);
            MainManager.PlaySound("BeetleDash", -1, 0.8f, 0.7f, true);
            battle.StartCoroutine(MainManager.GradualScale(entity.model.transform, startScale, 10f, false));
            yield return MainManager.ArcMovement(entity.model.gameObject, entity.model.position, basePos, new Vector3(-30,0,30), 15, 30f, false);
            MainManager.StopSound("BeetleDash");
            entity.model.localEulerAngles = baseRotation;
            entity.model.localScale = startScale;

            yield return EventControl.tenthsec;
            battle.enemydata[actionid].cursoroffset = new Vector3(0.2f, 5.6f);
            entity.animstate = 102;
            yield return EventControl.halfsec;
            entity.model.GetChild(1).GetChild(2).gameObject.SetActive(true);

            MainManager.RemoveCondition(MainManager.BattleCondition.Sturdy, battle.enemydata[actionid]);
            entity.basestate = 0;
            BattleControl_Ext.Instance.startState = 0;
            entity.LockRigid(false);
            entity.overridefly = false;
            entity.overrridejump = false;
            entity.overrideanim = false;
        }

        IEnumerator DoWindUpPunch(EntityControl entity, int actionid) 
        {
            bool hardmode = battle.HardMode();
            battle.GetSingleTarget();
            var playertargetentityRef = battle.playertargetentity;

            battle.MiddleCamera(entity.transform, playertargetentityRef.transform);

            entity.animstate = 103;
            yield return EventControl.halfsec;
            Transform[] mainParts = GetMainParts(entity);
            GameObject firePart = MainManager.PlayParticle("Flame", mainParts[0].transform.position);
            firePart.transform.parent = mainParts[0];

            MainManager.PlaySound("Spin2", 0.8f, 1);
            int waitTime = UnityEngine.Random.Range(1, hardmode ? 4 : 3);
            yield return new WaitForSeconds(waitTime);

            MainManager.PlayParticle("Gleam", "Gleam", mainParts[2].transform.position + new Vector3(-1f, -0.2f, -0.2f), new Vector3(-90f, 0f), 0.5f);
            yield return EventControl.quartersec;
            MainManager.StopSound("Spin2");
            yield return EventControl.tenthsec;

            MainManager.PlaySound("Coiled", 1f, 1);
            entity.animstate = 105;

            yield return null;

            JesterSprings leftSpring = mainParts[0].GetComponentInParent<JesterSprings>();
            leftSpring.SetPartPos(leftSpring.mainPart.position, playertargetentityRef.transform.position + new Vector3(1f, 1f, -0.2f), 10f);
            yield return EventControl.tenthsec;

            battle.DoDamage(actionid, battle.playertargetID, hardmode ? PUNCH_DAMAGE+1 : PUNCH_DAMAGE, null, battle.commandsuccess);
            int fireTurns = hardmode ? 4 : 3;
            if (battle.commandsuccess)
            {
                fireTurns = hardmode ? 2 : 1;
            }
            if(MainManager.instance.playerdata[battle.playertargetID].hp > 0)
            {
                battle.TryCondition(ref MainManager.instance.playerdata[battle.playertargetID], MainManager.BattleCondition.Fire, fireTurns);
            }
            battle.CameraFocus(playertargetentityRef.transform.position);

            MainManager.instance.camoffset += Vector3.right;

            yield return EventControl.halfsec;

            UnityEngine.Object.Destroy(firePart);
            leftSpring.ResetTarget();

            entity.animstate = 0;
            yield return EventControl.tenthsec;

            BattleControl.SetDefaultCamera();
            yield return null;
        }

        IEnumerator DoFireballJuggling(EntityControl entity, int actionid)
        {
            battle.nonphyscal = true;
            bool hardmode = battle.HardMode();
            entity.animstate = 106;

            Transform[] mainParts = GetMainParts(entity);

            MainManager.PlaySound("WaspKingMFireball1");
            yield return new WaitForSeconds(0.15f);

            int fireballAmount = hardmode ? 4 : 3;
            Transform[] fireballs = new Transform[fireballAmount];

            Vector3 fireballPos = mainParts[1].position + new Vector3(-0.5f,0);
            Vector3 offset = new Vector3(0, 0.5f, -0.2f);
            int[] order = { 0, 2, 1};

            if(hardmode)
                order = new int[]{ 0,3,1,2};

            for (int i = 0; i < order.Length; i++)
            {
                int index = order[i];
                fireballPos = index > 1 ? mainParts[0].position : mainParts[1].position + new Vector3(-0.5f, 0);
 
                battle.StartCoroutine(CreateFireballs(fireballs, index, fireballPos + offset));
            }
            yield return EventControl.halfsec;
            entity.animstate = 107;

            Vector3 targetPos;
            Coroutine[] coroutines = new Coroutine[fireballs.Length];
            for (int i = 0; i < order.Length; i++)
            {
                int index = order[i];

                targetPos = index > 1 ? mainParts[1].position : mainParts[0].position;

                yield return new WaitForSeconds(0.03f);
                coroutines[index] = battle.StartCoroutine(StartJuggling(fireballs[index],targetPos + offset, 6+i*2, coroutines, index));
                yield return new WaitForSeconds(0.05f);
            }


            for(int i = 0; i < order.Length; i++)
            {
                int index = order[i];
                yield return new WaitUntil(() => coroutines[index] == null);
                if (MainManager.GetAlivePlayerAmmount() == 0)
                    break;

                MainManager.PlaySound("WaspKingFireball1", 1.2f, 1);

                bool healBall = i == fireballs.Length - 1 && UnityEngine.Random.Range(0, 2) == 1;

                if (!healBall)
                {
                    battle.GetSingleTarget();
                    battle.StartCoroutine(battle.Projectile(FIREBALL_DAMAGE, BattleControl.AttackProperty.Fire, battle.enemydata[actionid], battle.playertargetID, fireballs[index], hardmode ? 25 : 30, 10, "SepPart@2@4", "Fire", "WaspKingMFireball2", null, Vector3.zero, false));
                    yield return EventControl.quartersec;
                    while (fireballs[index] != null)
                    {
                        yield return null;
                    }

                    Vector3 startPos = battle.playertargetentity.transform.position + Vector3.up * 2f;
                    Vector3 endPos = battle.playertargetentity.transform.position + Vector3.up * 4f;

                    int tpdrain = hardmode ? FIREBALL_DRAINTP + 1 : FIREBALL_DRAINTP;

                    if (battle.commandsuccess)
                    {
                        tpdrain--;
                        if (battle.GetSuperBlock(0))
                            tpdrain -= 1;
                    }
                    BattleControl_Ext.Instance.RemoveTP(-tpdrain, startPos, endPos);
                }
                else
                {
                    
                    Vector3 startPos = fireballs[index].position;
                    targetPos = entity.model.GetChild(0).GetChild(6).position;

                    yield return MainManager.ArcMovement(fireballs[index].gameObject, fireballs[index].position, targetPos, new Vector3(0, 0, 20), 10, 30f, true);
                    entity.animstate = 108;
                    battle.Heal(ref battle.enemydata[actionid], hardmode ? 6 : 4);
                    MainManager.SetCondition(MainManager.BattleCondition.Fire, ref battle.enemydata[actionid], hardmode ? 3 : 2);
                    MainManager.PlayParticle("Fire", targetPos);
                    MainManager.PlaySound("Boing1");
                    yield return EventControl.sec;
                }
            }


            for(int i=0; i < fireballs.Length; i++)
            {
                if (fireballs[i] != null)
                    UnityEngine.Object.Destroy(fireballs[i].gameObject);
            }
        }

        IEnumerator CreateFireballs(Transform[] fireballs, int index, Vector3 position)
        {
            fireballs[index] = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/Fireball"), position, Quaternion.identity, battle.battlemap.transform) as GameObject).transform;
            float a = 0f;
            float b = 20f;
            do
            {
                fireballs[index].localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 0.75f, a / b);
                a += MainManager.framestep;
                yield return null;
            }
            while (a < b + 1f);
        }

        IEnumerator StartJuggling(Transform fireball, Vector3 targetPos, int juggleCount, Coroutine[] coroutines, int index)
        {
            Vector3 tempPos;
            Vector3 startPos = fireball.position;
            for(int i=0; i < juggleCount;i++)
            {
                MainManager.PlaySound("WaspKingMFireball2", 1.2f, 0.5f);
                yield return MainManager.ArcMovement(fireball.gameObject, startPos,targetPos, new Vector3(0,0,20), 7, 25f,false);
                tempPos = startPos;
                startPos = targetPos;
                targetPos = tempPos;
            }
            coroutines[index] = null; 
        }

        IEnumerator DoBugCatcher(EntityControl entity, int actionid)
        {
            battle.enemydata[actionid].data[6] = 1;
            battle.dontusecharge = true;
            Vector3 basePos = entity.transform.position;
            battle.GetSingleTarget();

            MainManager.SetCamera(battle.playertargetentity.transform, battle.playertargetentity.transform.position + new Vector3(0,4,0), 0.02f, new Vector3(0f, 4f, -7f));
            entity.MoveTowards(battle.playertargetentity.transform.position + new Vector3(4f, 0f, 0.1f), 1f);
            while (entity.forcemove)
            {
                yield return null;
            }
            yield return EventControl.tenthsec;

            entity.animstate = 109;
            yield return EventControl.sec;
            yield return EventControl.halfsec;
            Transform[] mainParts = GetMainParts(entity);

            battle.playertargetentity.overrideanim = true;
            battle.playertargetentity.overrridejump = true;
            battle.playertargetentity.overrideonlyflip = true;
            battle.playertargetentity.LockRigid(true);

            entity.animstate = 111;
            MainManager.PlaySound("Coiled", 1f, 1);
            yield return null;

            JesterSprings leftSpring = mainParts[0].GetComponentInParent<JesterSprings>();
            leftSpring.SetPartPos(leftSpring.mainPart.position, battle.playertargetentity.transform.position + new Vector3(1f, 1f, -0.2f), 10f);
            
            JesterSprings rightSpring = mainParts[1].GetComponentInParent<JesterSprings>();
            rightSpring.SetPartPos(rightSpring.mainPart.position, battle.playertargetentity.transform.position + new Vector3(1f, 1f, 0.2f), 10f);
            battle.playertargetentity.animstate = (int)MainManager.Animations.Hurt;
            yield return EventControl.quartersec;

            entity.animstate = 112;
            yield return null;
            leftSpring.ResetTarget();
            rightSpring.ResetTarget();

            battle.playertargetentity.animstate = (int)MainManager.Animations.Hurt;
            battle.playertargetentity.transform.parent = mainParts[0];
            battle.playertargetentity.transform.localPosition = new Vector3(0.2f, 0.1f, 0.1f);
            yield return EventControl.tenthsec;

            MainManager.SetCamera(entity.transform, entity.transform.position + Vector3.up * 6, 0.02f, new Vector3(0f, 2.3f, -7f));
            yield return EventControl.sec;
            Vector3 playerPos = battle.playertargetentity.transform.position;

            battle.playertargetentity.transform.parent = battle.battlemap.transform;
            battle.playertargetentity.transform.localEulerAngles = Vector3.zero;
            battle.playertargetentity.transform.position = playerPos;

            entity.animstate = 113;

            yield return null;
            MainManager.PlaySound("Fall");
            yield return BattleControl_Ext.LerpPosition(20, playerPos, new Vector3(playerPos.x, 0, playerPos.z), battle.playertargetentity.transform);

            battle.Eat(actionid, battle.playertargetID, 2);
            battle.TryCondition(ref MainManager.instance.playerdata[battle.playertargetID], MainManager.BattleCondition.Fire, 3);

            yield return EventControl.halfsec;
            BattleControl.SetDefaultCamera();
            battle.playertargetentity.transform.position = basePos;
        }

        public static IEnumerator DoBugBullseye(EntityControl entity, int actionid, bool jesterDead)
        {
            BattleControl battle = MainManager.battle;
            battle.nonphyscal = true;
            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.instance.playerdata[i].battleentity.transform == battle.enemydata[actionid].ate.transform)
                {
                    bool hardmode = battle.HardMode();
                    Transform[] mainParts = GetMainParts(entity);

                    entity.animstate = 114;
                    yield return EventControl.tenthsec;

                    EntityControl playerEntity = MainManager.instance.playerdata[i].battleentity;

                    Vector3 playerPos = battle.partypos[Array.FindIndex(battle.partypointer, x => x == i)];
                    playerEntity.sprite.gameObject.SetActive(true);
                    playerEntity.shadow.gameObject.SetActive(true);
                    playerEntity.sprite.transform.localEulerAngles = Vector3.zero;
                    playerEntity.LockRigid(true);
                    playerEntity.overrridejump = true;
                    playerEntity.overrideanim = true;
                    playerEntity.transform.parent = mainParts[0];
                    playerEntity.transform.localPosition = new Vector3(0f, 0.1f, 0.1f);
                    yield return EventControl.sec;

                    entity.animstate = 116;
                    yield return null;

                    Vector3 targetPos = playerPos;
                    playerEntity.spin = new Vector3(0, 20);
                    int playerTargetId = -1;
                    if (!jesterDead)
                    {
                        playerTargetId = battle.GetRandomAvaliablePlayer();
                        targetPos = MainManager.instance.playerdata[playerTargetId].battleentity.transform.position + Vector3.up;
                    }

                    Vector3 oldPos = playerEntity.transform.position;
                    playerEntity.transform.parent = battle.battlemap.transform;
                    playerEntity.transform.localEulerAngles = Vector3.zero;
                    playerEntity.transform.localPosition = Vector3.zero;
                    playerEntity.transform.position = oldPos;

                    MainManager.PlaySound("Coiled", 0.9f, 1);
                    yield return BattleControl_Ext.LerpPosition(30, playerEntity.transform.position, targetPos, playerEntity.transform);
                    MainManager.RemoveCondition(MainManager.BattleCondition.Eaten, MainManager.instance.playerdata[i]);

                    if (!jesterDead)
                    {
                        int damage = hardmode ? BULLSEYE_DAMAGE + 1 : BULLSEYE_DAMAGE;

                        int[] targets = { playerTargetId, i };

                        for(int j=0; j<targets.Length; j++)
                        {
                            if(MainManager.instance.playerdata[targets[j]].hp > 0)
                            {
                                battle.DoDamage(actionid, targets[j], damage, null, battle.commandsuccess);

                                if (MainManager.instance.playerdata[targets[j]].hp > 0 && j == 0)
                                {
                                    battle.TryCondition(ref MainManager.instance.playerdata[targets[j]], MainManager.BattleCondition.Fire, hardmode ? 3 : 2);
                                }
                            }
                        }

                        Vector3 startPos = playerEntity.transform.position;
                        float a = 0;
                        float b = 20f;
                        do
                        {
                            playerEntity.transform.position = MainManager.BeizierCurve3(startPos, playerPos, 3f, a / b);
                            a += MainManager.TieFramerate(1f);
                            yield return null;
                        } while (a <b);
                    }

                    if (MainManager.instance.playerdata[i].hp > 0)
                    {
                        MainManager.instance.playerdata[i].cantmove = 1;
                    }

                    playerEntity.spin = Vector3.zero;
                    playerEntity.overrridejump = false;
                    playerEntity.overrideonlyflip = false;
                    playerEntity.LockRigid(false);
                    playerEntity.transform.position = playerPos;
                    playerEntity.animstate = (int)MainManager.Animations.KO;
                    //BattleControl.SetDefaultCamera();

                    yield return EventControl.halfsec;
                    playerEntity.overrideanim = false;
                    MainManager.RemoveCondition(MainManager.BattleCondition.Eaten, MainManager.instance.playerdata[i]);
                    MainManager.instance.playerdata[i].eatenby = null;

                    if (battle.enemydata.Length != 0 && actionid < battle.enemydata.Length)
                    {
                        battle.enemydata[actionid].ate = null;
                    }
                    yield break;
                }
            }
            yield return null;
        }

        IEnumerator DoCherryBomb(EntityControl entity, int actionid)
        {
            battle.nonphyscal = true;
            bool hardmode = battle.HardMode();
            entity.animstate = 117;
            yield return EventControl.halfsec;
            entity.animstate = 122;
            MainManager.PlaySound("BagRustle");
            Transform[] mainParts = GetMainParts(entity);

            int time = UnityEngine.Random.Range(5, hardmode ? 9 :8);
            SpriteRenderer cherryBomb = MainManager.NewSpriteObject(mainParts[0].transform.position, battle.battlemap.transform, MainManager_Ext.assetBundle.LoadAsset<Sprite>("JesterCherryBomb"));
            cherryBomb.transform.localScale = Vector3.one;
            cherryBomb.transform.localPosition += new Vector3(0.3f, 0.5f);

            battle.StartCoroutine(BattleControl_Ext.LerpPosition(20f, cherryBomb.transform.position, cherryBomb.transform.position + new Vector3(-0.2f, 5, -0.3f), cherryBomb.transform));

            DynamicFont timer = DynamicFont.SetUp(time.ToString(), true, true, 1f, 2, 0, Vector2.one * 2.25f, cherryBomb.transform, new Vector3(-0.3f, -1.4f, -0.1f), Color.red);
            timer.tridimentional = true;
            timer.dropshadow = true;
            MainManager.PlaySound("ItemHold");
            yield return EventControl.halfsec;

            cherryBomb.transform.parent = battle.battlemap.transform;
            battle.StartCoroutine(StartBombTimer(timer, time));

            AudioSource fuseSound = cherryBomb.gameObject.AddComponent<AudioSource>();
            if (MainManager.SoundVolume())
            {
                fuseSound.clip = MainManager_Ext.assetBundle.LoadAsset<AudioClip>("Fuse2");
                fuseSound.pitch = 0.8f;
                fuseSound.volume = MainManager.soundvolume * 0.8f;
                fuseSound.loop = true;
                fuseSound.Play();
                fuseSound.spatialBlend = 0;
            }

            yield return EventControl.sec;
            yield return EventControl.sec;
            entity.animstate = 119;
            MainManager.PlaySound("Coiled", 1f, 1);
            Vector3 cherryPos = cherryBomb.transform.position;
            cherryBomb.transform.parent = mainParts[0];
            cherryBomb.transform.position = cherryPos;
            yield return EventControl.tenthsec;
            yield return EventControl.tenthsec;
            AudioSource[] sounds = { timer.gameObject.GetComponent<AudioSource>(), fuseSound };

            cherryPos = cherryBomb.transform.position;
            cherryBomb.transform.parent = battle.battlemap.transform;
            cherryBomb.transform.position = cherryPos;

            float a = 0;
            float b = 40f;
            Vector3 startPos = cherryBomb.transform.position;
            do
            {
                cherryBomb.transform.position = MainManager.BeizierCurve3(startPos, startPos + Vector3.up*20, 10, a / b);
                for(int i = 0; i < sounds.Length; i++)
                {
                    if (sounds[i] != null)
                    {
                        sounds[i].volume = Mathf.Lerp(1, 0.1f, a / b) * MainManager.soundvolume;
                    }
                }

                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);

            cherryBomb.transform.position = new Vector3(battle.partymiddle.x, cherryBomb.transform.position.y, battle.partymiddle.z);
            startPos = cherryBomb.transform.position;


            a = 0;
            b = 60f * (time - 3);
            do
            {
                cherryBomb.transform.position = Vector3.Lerp(startPos, new Vector3(startPos.x, 10, startPos.z), a / b);

                for(int i=0; i<sounds.Length; i++)
                {
                    if (sounds[i] != null)
                    {
                        sounds[i].volume = Mathf.Lerp(0.1f, 1, a / b) * MainManager.soundvolume;
                    }
                }

                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);
            yield return BattleControl_Ext.LerpPosition(10, cherryBomb.transform.position, battle.partymiddle, cherryBomb.transform);

            fuseSound.Stop();
            entity.animstate = (int)MainManager.Animations.Happy;
            MainManager.PlayParticle("explosion", battle.partymiddle);
            MainManager.PlaySound("Explosion2");

            UnityEngine.Object.Destroy(cherryBomb.gameObject);

            MainManager.BattleCondition[] conditions = { MainManager.BattleCondition.Taunted, MainManager.BattleCondition.AttackDown, MainManager.BattleCondition.DefenseDown, MainManager.BattleCondition.Inked, MainManager.BattleCondition.Poison, MainManager.BattleCondition.Sticky };
            
            for(int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.instance.playerdata[i].hp > 0 && MainManager.instance.playerdata[i].eatenby == null)
                {
                    battle.DoDamage(actionid, i, hardmode ? CHERRY_BOMB_DAMAGE + 1 : CHERRY_BOMB_DAMAGE, BattleControl.AttackProperty.Fire, battle.commandsuccess);
                    BattleCondition battleCondition = conditions[UnityEngine.Random.Range(0, conditions.Length)];
                    EntityControl playerEntity = MainManager.instance.playerdata[i].battleentity;
                    int turns = 3;
                    switch (battleCondition)
                    {
                        case MainManager.BattleCondition.Taunted:
                            MainManager.SetCondition(battleCondition, ref MainManager.instance.playerdata[i], 2);
                            break;

                        case BattleCondition.AttackDown:
                        case BattleCondition.DefenseDown:
                            battle.StatusEffect(MainManager.instance.playerdata[i], battleCondition, turns, true, false);
                            break;

                        case BattleCondition.Poison:
                            BattleControl_Ext.Instance.ApplyStatus(battleCondition, ref MainManager.instance.playerdata[i], turns, "Poison", 1, 1, "PoisonEffect", playerEntity.transform.position, Vector3.one);
                            break;

                        case BattleCondition.Inked:
                            BattleControl_Ext.Instance.ApplyStatus(battleCondition, ref MainManager.instance.playerdata[i], turns, "WaterSplash2", 1, 1, "InkGet", playerEntity.transform.position, Vector3.one);
                            break;

                        case BattleCondition.Sticky:
                            BattleControl_Ext.Instance.ApplyStatus(battleCondition, ref MainManager.instance.playerdata[i], turns, "AhoneynationSpit", 1, 1, "StickyGet", playerEntity.transform.position, Vector3.one);
                            break;
                    }
                }
            }
            yield return EventControl.sec;
        }

        IEnumerator StartBombTimer(DynamicFont timer, int baseTime)
        {
            AudioSource beepSound = null;
            if (MainManager.SoundVolume())
            {
                beepSound = timer.gameObject.AddComponent<AudioSource>();
                beepSound.clip = Resources.Load<AudioClip>("Audio/Sounds/" + "ACBeep");
                beepSound.pitch = 1.2f;
                beepSound.volume = MainManager.soundvolume * 0.8f;
                beepSound.spatialBlend = 0;
            }

            do
            {
                yield return EventControl.sec;
                baseTime--;
                timer.text = baseTime.ToString();
                if(baseTime != 0 && beepSound != null)
                {
                    beepSound.Play();
                }
            } while (baseTime > 0);
        }

        static Transform[] GetMainParts(EntityControl entity)
        {
            Transform spring5 = entity.model.GetChild(0).GetChild(5);
            return new Transform[] { spring5.GetChild(0).GetChild(11), spring5.GetChild(1).GetChild(11), entity.model.GetChild(0).GetChild(6) };
        }
    }

    public class JesterSprings : MonoBehaviour
    {
        public Vector3 target = Vector3.zero;
        public Vector3 startPos = Vector3.zero;
        public float time = 0;
        public float duration = 60f;
        public Transform[] links;

        public Vector3 start;

        public Vector3 end;

        public Vector3 middle;

        public bool localpos;

        public EntityControl entity;

        public Transform mainPart;

        private void Start()
        {
            mainPart = links[links.Length - 1].transform.parent;
        }

        private void LateUpdate()
        {
            if (target != Vector3.zero)
            {
                if(Vector3.Distance(mainPart.position, target) < 0.01f)
                {
                    time = 0;
                    mainPart.position = target;
                }
                else
                {
                    mainPart.position = Vector3.Lerp(startPos, target, time / duration);
                    time += MainManager.TieFramerate(1f);
                }
            }

            start = links[0].transform.position;
            end = links[links.Length - 1].transform.position;
            localpos = false;

            for (int i = 0; i < links.Length; i++)
            {
                float t = (float)i / (float)(links.Length - 1);
                if (middle.magnitude > 0.1f)
                {
                    Vector3 vector = middle;
                    Vector3 mid = Vector3.Lerp((!localpos) ? start : (base.transform.position + start), (!localpos) ? end : (base.transform.position + end), 0.5f) + vector;
                    links[i].position = BeizierCurve3((!localpos) ? start : (base.transform.position + start), (!localpos) ? end : (base.transform.position + end), mid, t);
                }
                else
                {
                    links[i].position = Vector3.Lerp((!localpos) ? start : (base.transform.position + start), (!localpos) ? end : (base.transform.position + end), t);
                }
            }
        }

        public static Vector3 BeizierCurve3(Vector3 start, Vector3 end, Vector3 mid, float t)
        {
            t = Mathf.Clamp01(t);
            return (1f - t) * (1f - t) * start + 2f * t * (1f - t) * mid + t * t * end;
        }

        public void SetPartPos(Vector3 startPos, Vector3 targetPos, float duration)
        {
            this.startPos = startPos;
            target = targetPos;
            this.duration = duration;
        }

        public void ResetTarget()
        {
            target = Vector3.zero;
        }
    }
}
