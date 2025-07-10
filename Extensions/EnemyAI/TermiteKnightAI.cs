using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static BattleControl;
using static HarmonyLib.Code;
using static MainManager;
using static UnityEngine.ParticleSystem;
using static BFPlus.Extensions.BattleControl_Ext;

namespace BFPlus.Extensions.EnemyAI
{
//    Attacks :
//Shield Boomerang(All Party)
//TK will spin around before chucking his shield at Team Snakemouth, hitting the entire party twice, once on the way out, once on the way back, and then catch his shield.This attack will deal 2/3 damage per hit.

//    Shield Surf (All Party)
//TK will walk up to Team Snakemouth and bounce on each of them once dealing 4/5 damage per bounce.This is very similar to the Dynamo Spore's Bounce attack.

//Grab n' Slam (Single target)
//TK will walk up to a single target and grab the target with both arms dealing 2/2 damage with the grab. After grabbing onto the targeted bug, TK will leap into the air before slamming down into the ground with his foe held beneath him to deal 8/10 damage

//Taunt (Single target)
//    Just a single target enemy Taunt.Like Zazp's and Tanjerin's. (Does not consume a turn)


//The Termite Knight will also have acess to variety of potions.If he has more than 1 action, he will always spend his first first action to use a normal attack, and his second action to use a potion. if he has a third action he can eiter do Attack, Attack, Potion or Attack, Potion, Potion.He can never use an attack after having used a potion on that same turn (ideally).

//Each of these potions (aside from Super HP) have 3 uses:
//Super HP Potion: A full 99 HP heal(scripted event/1 time use.)
//Super TP Potion:  Instantly gain +3 charge.
//    MP Potion:  Gains an extra turn both immediately AND for next turn
//ATK potion: +1 ATK for 3 turns
//    DEF Potion: +1 DEF for 3 turns
//    Mite Knight Potion: A potion that he throws at a single member of Team Snakemouth.It deals 4/5 damage, but also heals the targeted bug for 1 HP (in reference to it actually being a healing potion for 1 heart in the arcade game)

//In the middle of the battle after hitting 10 HP(damage cap) he'll use his Super HP potion to heal back up to full HP, AND He'll use a Mite Knight Key to give himself permanent hustle status for the second half of the fight.

//TK will also have the "stance" mechanic from Asthotheles or the Numbnails, where he can sometimes raise his shield to gain +2 defense.Hitting him with a flipping attack will break his stance.
    public class TermiteKnightAI : AI
    {
        enum Attacks
        {
            ShieldBoomerang,
            ShieldSurf,
            Taunt,
            Potion,
            GrabSlam
        }

        enum PotionType
        {
            SuperHP,
            SuperTP,
            MP,
            ATK,
            DEF,
            MitePotion,
            MiteKey
        }

        BattleControl battle = null;
        const int SHIELD_DAMAGE = 5;
        const int SHIELD_SURF_DAMAGE = 4;
        const int GRAB_DAMAGE = 2;
        const int SLAM_DAMAGE = 8;
        const int MITE_POTION_DAMAGE = 5;
        public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
        {
            battle = MainManager.battle;

            if (battle.enemydata[actionid].data == null)
            {
                battle.enemydata[actionid].data = new int[5];
            }
    
            if (battle.enemydata[actionid].hp <= 10 && battle.enemydata[actionid].data[1] == 0)
            {
                MainManager.SetCamera(new Vector3(entity.transform.position.x, entity.sprite.transform.localPosition.y + 0.25f, 2.5f));
                yield return EventControl.halfsec;
                MainManager.DialogueText("Hoo-wee! You chumps got some fight in you after all. But now playtime is over!|next|Try to keep up now Snakemouth! |shaky||size,1.5||line|It's time to KICK THINGS UP A NOTCH!|shaky|",
                    entity.transform, null);

                while (MainManager.instance.message)
                {
                    yield return null;
                }

                BattleControl.SetDefaultCamera();
                yield return EventControl.quartersec;
                yield return UsePotion(entity, actionid, PotionType.SuperHP);
                yield return UsePotion(entity, actionid, PotionType.MiteKey);
                battle.enemydata[actionid].weakness.Remove(AttackProperty.SurviveWith10);
                battle.enemydata[actionid].data[1] = 1;
            }

            if (battle.enemydata[actionid].data[0] < 0 && UnityEngine.Random.Range(0, 100) < 50)
            {
                yield return DoTaunt(entity, actionid);
                yield return EventControl.quartersec;
            }
            else
            {
                battle.enemydata[actionid].data[0]--;
            }

            for (int i = 2; i < battle.enemydata[actionid].data.Length; i++)
            {
                battle.enemydata[actionid].data[i]--;
            }
           
            List<Attacks> attacks = new List<Attacks>() { Attacks.ShieldSurf, Attacks.GrabSlam, Attacks.ShieldBoomerang, Attacks.ShieldBoomerang };

            switch (attacks[UnityEngine.Random.Range(0, attacks.Count)])
            {
                case Attacks.ShieldBoomerang:
                    yield return DoShieldBoomerang(entity, actionid);
                    break;
                case Attacks.ShieldSurf:
                    yield return DoShieldSurf(entity, actionid);
                    break;
                case Attacks.GrabSlam:
                    yield return DoGrabSlam(entity, actionid);
                    break;
            }

            battle.enemydata[actionid].charge = 0;
            battle.playertargetID = -1;
            yield return EventControl.quartersec;

            if(MainManager.GetAlivePlayerAmmount() >0)
                yield return UsePotion(entity, actionid, GetRandomPotion(actionid));
        }

        PotionType GetRandomPotion(int actionid)
        {
            PotionType[] possiblePotions = new PotionType[] { PotionType.SuperTP, PotionType.MP, PotionType.ATK, PotionType.DEF, PotionType.MitePotion };
            List<PotionType> usablePotions = new List<PotionType>();
            for (int i = 0; i < possiblePotions.Length; i++)
            {
                switch (possiblePotions[i])
                {
                    case PotionType.SuperTP:
                        if (battle.enemydata[actionid].charge == 0 && battle.enemydata[actionid].data[2] <= 0)
                        {
                            usablePotions.Add(possiblePotions[i]);
                        }
                        break;
                    case PotionType.DEF:
                        if (MainManager.HasCondition(MainManager.BattleCondition.DefenseUp, battle.enemydata[actionid]) == -1 && battle.enemydata[actionid].data[3] <= 0)
                        {
                            usablePotions.Add(possiblePotions[i]);
                        }
                        break;
                    case PotionType.ATK:
                        if (MainManager.HasCondition(MainManager.BattleCondition.AttackUp, battle.enemydata[actionid]) == -1 && battle.enemydata[actionid].data[4] <= 0)
                        {
                            usablePotions.Add(possiblePotions[i]);
                        }
                        break;
                    case PotionType.MP:
                        BattleCondition[] badCondition = new BattleCondition[] { BattleCondition.Poison, BattleCondition.Fire, BattleCondition.AttackDown, BattleCondition.Taunted };

                        if (MainManager.HasCondition(MainManager.BattleCondition.Sturdy, battle.enemydata[actionid]) == -1)
                        {
                            usablePotions.Add(possiblePotions[i]);
                            foreach (var condition in badCondition)
                            {
                                if (MainManager.HasCondition(condition, battle.enemydata[actionid]) > -1)
                                {
                                    usablePotions.Add(possiblePotions[i]);
                                    break;
                                }
                            }
                        }
                        break;
                    case PotionType.MitePotion:
                        usablePotions.Add(possiblePotions[i]);
                        break;
                }
            }

            return usablePotions[UnityEngine.Random.Range(0, usablePotions.Count)];
        }

        IEnumerator UsePotion(EntityControl entity, int actionid, PotionType type)
        {
            int baseState = entity.animstate;
            battle.dontusecharge = true;
            battle.nonphyscal = true;

            entity.animstate = (int)MainManager.Animations.ItemGet;

            MainManager.PlaySound("MKPotion");
            SpriteRenderer itemSprite = new GameObject().AddComponent<SpriteRenderer>();
            itemSprite.transform.position = entity.transform.position + new Vector3(-1.3f, 3.86f, -0.1f);

            Sprite potionSprite = null;

            Sprite[] dungeonSprites = Resources.LoadAll<Sprite>("Sprites/Misc/dungeongame");
            switch (type)
            {
                case PotionType.SuperHP:
                    potionSprite = MainManager.itemsprites[0, (int)MainManager.Items.SuperHPPotion];
                    break;
                case PotionType.SuperTP:
                    potionSprite = MainManager.itemsprites[0, (int)MainManager.Items.SuperTPPotion];
                    break;
                case PotionType.MP:
                    potionSprite = MainManager.itemsprites[0, (int)MainManager.Items.MPPotion];
                    break;
                case PotionType.ATK:
                    potionSprite = MainManager.itemsprites[0, (int)MainManager.Items.ATKPotion];
                    break;
                case PotionType.DEF:
                    potionSprite = MainManager.itemsprites[0, (int)MainManager.Items.DEFPotion];
                    break;
                case PotionType.MitePotion:
                    potionSprite = dungeonSprites[1];
                    itemSprite.transform.localScale = Vector3.one * 2;
                    break;
                case PotionType.MiteKey:
                    potionSprite = dungeonSprites[0];
                    itemSprite.transform.localScale = Vector3.one * 2;
                    break;
            }

            itemSprite.sprite = potionSprite;
            itemSprite.material.renderQueue = 50000;
            itemSprite.gameObject.layer = 14;

            if (type == PotionType.MitePotion)
                itemSprite.color = Color.green;

            if (type == PotionType.MiteKey)
                itemSprite.color = Color.yellow;

            yield return EventControl.sec;
            if(type != PotionType.MitePotion)
            {
                UnityEngine.Object.Destroy(itemSprite.gameObject);
            }

            switch (type)
            {
                case PotionType.SuperHP:
                    battle.Heal(ref battle.enemydata[actionid], battle.enemydata[actionid].maxhp, false);
                    break;
                case PotionType.SuperTP:
                    battle.StartCoroutine(battle.StatEffect(entity, 4));
                    MainManager.PlaySound("StatUp");
                    battle.enemydata[actionid].charge = 2;
                    battle.enemydata[actionid].data[2] = 4;
                    break;
                case PotionType.MP:
                    MainManager.PlaySound("Heal3");
                    battle.ClearStatus(ref battle.enemydata[actionid]);
                    MainManager.SetCondition(MainManager.BattleCondition.Sturdy, ref battle.enemydata[actionid], 2);
                    MainManager.PlayParticle("MagicUp", entity.transform.position);
                    battle.enemydata[actionid].delayedcondition = null;

                    if (battle.enemydata[actionid].frostbitep != null)
                    {
                        UnityEngine.Object.Destroy(battle.enemydata[actionid].frostbitep.gameObject);
                    }
                    break;
                case PotionType.ATK:
                    MainManager.PlaySound("StatUp");
                    MainManager.SetCondition(MainManager.BattleCondition.AttackUp, ref battle.enemydata[actionid], 4);
                    battle.StartCoroutine(battle.StatEffect(battle.enemydata[actionid].battleentity, 0));
                    battle.enemydata[actionid].data[4] = 3;
                    break;
                case PotionType.DEF:
                    MainManager.PlaySound("StatUp");
                    MainManager.SetCondition(MainManager.BattleCondition.DefenseUp, ref battle.enemydata[actionid], 4);
                    battle.StartCoroutine(battle.StatEffect(battle.enemydata[actionid].battleentity, 1));
                    battle.enemydata[actionid].data[3] = 3;
                    break;
                case PotionType.MitePotion:
                    battle.dontusecharge=true;
                    itemSprite.gameObject.SetActive(false);
                    battle.GetSingleTarget();
                    entity.animstate = (int)MainManager.Animations.TossItem;
                    itemSprite.transform.position = entity.transform.position + new Vector3(-1f, 1.6f, -0.1f);
                    yield return EventControl.tenthsec;
                    itemSprite.gameObject.SetActive(true);
                    MainManager.PlaySound("MKOpen");

                    float speed = 40f;
                    if (battle.HardMode())
                        speed = 30f;

                    battle.StartCoroutine(battle.Projectile(MITE_POTION_DAMAGE, null, battle.enemydata[actionid], battle.playertargetID, itemSprite.transform, speed, 0, "keepcolor", null, "MKPotion", null, new Vector3(0, 0, 20), false));
                    yield return EventControl.quartersec;
                    entity.animstate = baseState;
                    yield return new WaitUntil(() => itemSprite == null);
                    battle.Heal(ref MainManager.instance.playerdata[battle.playertargetID], 1, false);
                    break;
                case PotionType.MiteKey:
                    battle.enemydata[actionid].moves = 2;
                    battle.enemydata[actionid].cantmove--;
                    battle.StartCoroutine(battle.StatEffect(entity, 5));
                    MainManager.PlaySound("MKKey");
                    MainManager.PlaySound("StatUp");
                    break;
            }

            entity.animstate = (int)MainManager.Animations.Idle;
            entity.talking = true;
            yield return EventControl.sec;
            entity.talking = false;
        }

        IEnumerator DoTaunt(EntityControl entity, int actionid)
        {
            battle.GetSingleTarget();
            battle.dontusecharge = true;

            entity.animstate = (int)MainManager.Animations.Block;
            entity.talking = true;
            MainManager.PlaySound("Taunt2", 9, 0.85f, 1f, true);
            yield return new WaitForSeconds(0.85f);
            MainManager.SetCondition(MainManager.BattleCondition.Taunted, ref MainManager.instance.playerdata[battle.playertargetID], 2);
            EntityControl targetEntity = battle.playertargetentity;

            int baseState = targetEntity.animstate;
            targetEntity.overrideanim = true;
            MainManager.PlaySound("Taunt");
            switch (battle.playertargetID)
            {
                case 0:
                    targetEntity.animstate = 10;
                    break;
                case 1:
                    targetEntity.animstate = 5;
                    break;
                case 2:
                    targetEntity.animstate = 102;
                    break;
            }
            GameObject t = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/AngryParticle")) as GameObject;
            t.transform.parent = targetEntity.sprite.transform;
            t.transform.localPosition = Vector3.up;
            yield return new WaitForSeconds(0.85f);
            MainManager.StopSound(9);
            entity.animstate = (int)MainManager.Animations.Idle;

            yield return new WaitForSeconds(0.85f);
            UnityEngine.Object.Destroy(t.gameObject);
            entity.talking = false;
            targetEntity.overrideanim = false;
            targetEntity.animstate = baseState;
            battle.enemydata[actionid].data[0] = 2;
            battle.playertargetID = -1;
        }

        IEnumerator DoGrabSlam(EntityControl entity, int actionid)
        {
            Vector3 basePos = entity.transform.position;
            battle.GetSingleTarget();
            EntityControl targetEntity = battle.playertargetentity;
            Vector3 startLocalPos = targetEntity.transform.localPosition;

            entity.overrridejump = true;
            entity.overrideonlyflip = true;
            targetEntity.LockRigid(true);
            MainManager.SetCamera(targetEntity.transform, null, 0.02f, new Vector3(0f, 2.3f, -6f));

            MainManager.PlaySound("MKWalk", -1, 1f, 1f, true);
            entity.MoveTowards(targetEntity.transform.position + new Vector3(1.2f, 0f, 0.1f), 1.5f);
            while (entity.forcemove)
            {
                yield return null;
            }
            MainManager.StopSound("MKWalk");

            MainManager.PlaySound("Charge3", 0.85f, 1);
            entity.animstate = 103;
            entity.StartCoroutine(entity.ShakeSprite(0.1f, 60f));
            yield return EventControl.sec;
            MainManager.StopSound("Charge3");
            entity.animstate = 104;
            MainManager.PlaySound("MKKey");
            yield return EventControl.tenthsec;
            battle.DoDamage(actionid, battle.playertargetID, GRAB_DAMAGE, null, battle.commandsuccess);

            yield return EventControl.quartersec;
            targetEntity.animstate = (int)MainManager.Animations.Hurt;

            entity.LockRigid(true);
            targetEntity.transform.parent = entity.transform;
            Vector3 targetEntityBasePos = targetEntity.transform.position;

            entity.trail = true;
            entity.animstate = (int)MainManager.Animations.Hurt;
            MainManager.PlaySound("Jump", -1, 0.85f, 1f);
            MainManager.SetCamera(entity.transform,entity.transform.position + Vector3.up * 3, 0.02f, new Vector3(0f, 2.3f, -6f));

            targetEntity.transform.localPosition = new Vector3(-0.5f, 1f, -0.1f);
            yield return LerpPosition(15, entity.transform.position, entity.transform.position + Vector3.up*5, entity.transform);
            targetEntity.transform.localPosition = new Vector3(0.6f, -0.5f, -0.1f);
            targetEntity.transform.localEulerAngles = new Vector3(0, 0, 90);
            entity.animstate = 107;

            float a = 0;
            float b = 15;
            do
            {
                entity.transform.localEulerAngles = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, 0, 360), a / b);
                a += MainManager.framestep;
                yield return null;
            } while (a < b);
            entity.transform.localEulerAngles = Vector3.zero;

            Vector3 startPos = entity.transform.position;
            Vector3 targetPos = new Vector3(startPos.x, 0, startPos.z);
            MainManager.SetCamera(null, targetPos, 0.02f, new Vector3(0f, 2.3f, -6f));
            yield return LerpPosition(15, startPos, targetPos, entity.transform);

            battle.DoDamage(actionid, battle.playertargetID, SLAM_DAMAGE, null, battle.commandsuccess);
            yield return EventControl.tenthsec;

            battle.enemydata[actionid].charge = 0;
            MainManager.PlaySound("MKDeath");
            MainManager.PlayParticle("impactsmoke", targetEntity.transform.position + Vector3.left);
            SetDefaultCamera();
            MainManager.ShakeScreen(0.2f, 0.5f);
            //yield return EventControl.quartersec;
            targetEntity.transform.parent = battle.battlemap.transform;
            targetEntity.transform.position = targetEntityBasePos;
            targetEntity.transform.localPosition = startLocalPos;
            targetEntity.transform.localEulerAngles = Vector3.zero;
            targetEntity.LockRigid(false);

            a = 0;
            b = 25;
            startPos = targetEntity.transform.position;
            entity.animstate = 106;
            yield return null;
            yield return null;

            //this is needed cause base sprite pivot isnt the center, if we dont do that the rotation will look weird.
            entity.anim.enabled = false;
            Sprite originalSprite = entity.sprite.sprite;
            Rect spriteRect = originalSprite.rect;
            Sprite newSprite = Sprite.Create(
                originalSprite.texture,
                spriteRect,
                new Vector2(0.5f, 0.5f),
                originalSprite.pixelsPerUnit,
                0,
                SpriteMeshType.Tight,
                originalSprite.border
            );
            entity.sprite.sprite = newSprite;

            MainManager.PlaySound("Jump", -1, 0.85f, 1f);
            do
            {
                entity.spritetransform.Rotate(new Vector3(0, 0, MainManager.TieFramerate(-20f)));
                entity.transform.position = MainManager.BeizierCurve3(startPos, basePos, 15f, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b + 1f);
            entity.anim.enabled = true;

            entity.trail = false;
            entity.spritetransform.eulerAngles = Vector3.zero;
            entity.overrridejump = false;
            entity.overrideonlyflip = false;
            yield return null;

            MainManager.PlaySound("MKHit", -1, 1f, 1f);
            entity.animstate = (int)MainManager.Animations.KO;
            yield return null;
            a = 0;
            b = 30;
            do
            {
                entity.spritetransform.localEulerAngles = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, 360, 0), a / b);
                a += MainManager.framestep;
                yield return null;
            } while (a < b);
            entity.spritetransform.eulerAngles = Vector3.zero;
        }

        IEnumerator DoShieldSurf(EntityControl entity, int actionid)
        {
            int baseState = entity.animstate;
            Vector3 startPosition = entity.transform.position;

            MainManager.PlaySound("MKWalk", -1, 1f, 1f, true);
            MainManager.SetCamera(entity.transform, null, 0.02f);
            entity.spin = new Vector3(0, 30, 0);
            entity.animstate = 108;
            yield return EventControl.tenthsec;

            var hurricanePart = MainManager.PlayParticle("HurricaneBig", entity.transform.position).GetComponent<ParticleSystem>();
            hurricanePart.transform.parent = entity.spritetransform;
            hurricanePart.transform.localScale = new Vector3(1.5f, 1, 1);
            var shape = hurricanePart.shape;
            shape.angle = 15;
            shape.radius = 0.05f;

            var emission = hurricanePart.emission;
            emission.rateOverTime = 10;
            yield return EventControl.sec;
            yield return EventControl.quartersec;

            hurricanePart.Stop();
            SetDefaultCamera();
            MainManager.StopSound("MKWalk");
            float a;
            float b;
            for (int i = 0; i < battle.partypointer.Length; i++)
            {
                int index = battle.partypointer[i];
                if (MainManager.instance.playerdata[index].hp > 0)
                {
                    EntityControl targetEntity = MainManager.instance.playerdata[index].battleentity;
                    entity.LockRigid(true);
                    entity.overrridejump = true;

                    a = 0f;
                    b = 35;
                    Vector3 pp = entity.transform.position;
                    MainManager.PlaySound("Jump", -1, 0.85f, 1f);
                    MainManager.PlaySound("Turn", -1, 0.7f, 1f);
                    entity.animstate = 108;
                    do
                    {
                        entity.transform.position = MainManager.BeizierCurve3(pp, targetEntity.transform.position + new Vector3(0f, 1f, -0.1f), 5.5f, a / b);
                        a += MainManager.TieFramerate(1f);
                        yield return null;
                    }
                    while (a < b);
                    if (!battle.commandsuccess)
                    {
                        MainManager.ShakeScreen(new Vector3(0.075f, 0.3f), 0.75f, true);
                    }
                    entity.spin = Vector3.zero;
                    battle.DoDamage(actionid, index, SHIELD_SURF_DAMAGE, null, battle.commandsuccess);

                    entity.flip = !entity.flip;
                    MainManager.PlayParticle("impactsmoke", targetEntity.transform.position);
                    MainManager.PlaySound("MKHit2");
                }
            }
            entity.flip = false;
            entity.spin = new Vector3(0, 20, 0);
            a = 0f;
            b = 35f;
            Vector3 lastTargetPos = entity.transform.position;
            do
            {
                entity.transform.position = MainManager.BeizierCurve3(lastTargetPos, startPosition, 5.5f, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b + 1f);
            entity.spin = Vector3.zero;
            entity.transform.position = startPosition;
            entity.animstate = baseState;
            UnityEngine.Object.Destroy(hurricanePart);
            yield return null;
        }

        IEnumerator DoShieldBoomerang(EntityControl entity, int actionid)
        {
            int baseState = entity.animstate;
            battle.nonphyscal = true;
            Vector3 basePos = entity.transform.position;
            entity.MoveTowards(new Vector3(2.5f, 0f, -0.55f), 2f);
            while (entity.forcemove)
            {
                yield return null;
            }

            battle.GetSingleTarget();
            EntityControl playerTargetEntity = battle.playertargetentity;

            entity.flip = false;
            entity.animstate = 110;
            entity.spin = new Vector3(0f, 30f);

            MainManager.PlaySound("Woosh", -1, 0.9f, 1f, true);
            yield return EventControl.halfsec;

            Transform shield = CreateShield();
            ParticleSystem.MainModule p = shield.GetComponentInChildren<ParticleSystem>().main;
            MainManager.StopSound("Woosh");

            AudioSource sound = MainManager.PlaySound("Toss2", 3, 0.8f, 1f, true);
            Vector3 startPos = entity.transform.position + new Vector3(-1f, 1.6f, -0.1f);
            Vector3 targetPos = playerTargetEntity.transform.position + Vector3.up;
            yield return ThrowShield(shield, startPos, targetPos, actionid, 20f,p,entity,sound);

            entity.LockRigid(true);
            entity.overrideanim = true;
            entity.overrridejump = true;

            float jumpHeight = 3.5f;
            targetPos = new Vector3(1, jumpHeight, 0);
            Vector3 startPosShield = shield.position;
            startPos = entity.transform.position;
            entity.trail = true;
            MainManager.PlaySound("Jump",0.85f,1);
            Vector3 shieldOffset = new Vector3(-1.5f, 0f);
            float a = 0;
            float b = 20;
            while (a <= b)
            {
                shield.position = Vector3.Lerp(startPosShield, targetPos + shieldOffset, a / b);
                shield.localEulerAngles += Vector3.forward * 20f * MainManager.framestep;
                p.startRotationMultiplier = shield.localEulerAngles.z;
                entity.transform.position = MainManager.BeizierCurve3(startPos, targetPos, jumpHeight, a / b);

                if (sound != null)
                {
                    sound.volume = Mathf.Lerp(1f, 0f, a / b) * MainManager.soundvolume;
                }
                yield return null;
                a += MainManager.framestep;
            }
            entity.trail = false;
            entity.animstate = 110;
            MainManager.PlaySound("Ding2");
            UnityEngine.Object.Destroy(shield.gameObject);
            if (MainManager.GetAlivePlayerAmmount() > 0)
            {
                battle.GetSingleTarget();
                playerTargetEntity = battle.playertargetentity;

                entity.spritetransform.localEulerAngles = new Vector3(0, 0, 30);
                MainManager.PlaySound("Woosh", -1, 0.9f, 1f, true);
                a = 0;
                b = 20;
                while (a <= b)
                {
                    entity.spritetransform.localEulerAngles = Vector3.Lerp(new Vector3(0, 0, 30), new Vector3(0, 360, 30), a / b);
                    yield return null;
                    a += MainManager.framestep;
                }
                entity.spritetransform.localEulerAngles = new Vector3(0, 0, 30);

                shield = CreateShield();
                p = shield.GetComponentInChildren<ParticleSystem>().main;
                MainManager.StopSound("Woosh");

                sound = MainManager.PlaySound("Toss2", 3, 0.8f, 1f, true);
                startPos = entity.transform.position + new Vector3(-1f, 1.6f, -0.1f);
                targetPos = playerTargetEntity.transform.position + Vector3.up;

                yield return ThrowShield(shield, startPos, targetPos, actionid, 20f, p, entity, sound);
                startPosShield = shield.transform.position;
                a = 0;
                while (a <= b)
                {
                    shield.position = Vector3.Lerp(startPosShield, entity.transform.position + shieldOffset, a / b);
                    shield.localEulerAngles += Vector3.forward * 20f * MainManager.framestep;
                    p.startRotationMultiplier = shield.localEulerAngles.z;
                    if (sound != null)
                    {
                        sound.volume = Mathf.Lerp(1f, 0f, a / b) * MainManager.soundvolume;
                    }
                    yield return null;
                    a += MainManager.framestep;
                }
                entity.spritetransform.localEulerAngles = new Vector3(0, 0, 0);
                entity.animstate = 110;
                MainManager.PlaySound("Ding2");
                UnityEngine.Object.Destroy(shield.gameObject);
            }

            entity.spin = new Vector3(0, 20);
            MainManager.PlaySound("Jump", 0.85f, 1);
            entity.trail = true;
            a = 0;
            b = 20;
            startPos = entity.transform.position;
            while (a <= b)
            {
                entity.transform.position = MainManager.BeizierCurve3(startPos, basePos, jumpHeight, a / b);
                yield return null;
                a += MainManager.framestep;
            }
            entity.spin = Vector3.zero;
            entity.LockRigid(false);
            entity.overrideanim = false;
            entity.overrridejump = false;
            entity.trail = false;
            entity.animstate = baseState;
        }


        Transform CreateShield()
        {
            Transform shield = UnityEngine.Object.Instantiate(MainManager_Ext.assetBundle.LoadAsset<GameObject>("TermiteKnightShield")).transform;
            SpriteRenderer sr = shield.GetComponent<SpriteRenderer>();
            sr.materials = new Material[] { MainManager.spritematlit };
            shield.parent = battle.transform;
            return shield;
        }

        IEnumerator ThrowShield(Transform shield, Vector3 startPos, Vector3 targetPos, int actionid, float endTime, MainModule p, EntityControl entity, AudioSource sound)
        {
            entity.spin = Vector3.zero;
            MainManager.PlaySound("MKKey",0.9f,1);
            entity.animstate = 100;
            float a = 0f;
            while (a <= endTime)
            {
                shield.position = Vector3.Lerp(startPos, targetPos, a / endTime);
                shield.localEulerAngles += Vector3.forward * 20f * MainManager.framestep;
                p.startRotationMultiplier = shield.localEulerAngles.z;

                if (sound != null)
                {
                    sound.volume = Mathf.Lerp(1f, 0f, a / endTime) * MainManager.soundvolume;
                }
                yield return null;
                a += MainManager.framestep;
            }
            battle.DoDamage(actionid, battle.playertargetID, SHIELD_DAMAGE, null, battle.commandsuccess);
        }
    }
}
