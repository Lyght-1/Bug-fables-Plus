using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static BFPlus.Extensions.BattleControl_Ext;
using static UnityEngine.ParticleSystem;

namespace BFPlus.Extensions.EnemyAI
{


    //    He'd only have like 60-70 HP and 0 defense one action per round, a tougher miniboss.
    //When dropping below 50% of max HP, he'll get permanent attack-up .
    //The battle would start out with a Splotch Spider in front of the Shaman, and an empty enemy slot behind them.
    //    Actions/Moves

    //Attack 1 - Staff Spin.
    //They'd walk up to one party member and start to spin their staff in the target's face dealing 3x2 (3x3 in Hard Mode) damage and inflict inked for 1 turn for every time the attack is unblocked. (See Ultimax's pathetic swipes for anim/timing)

    //Attack 2 -  (Coat) Tail Swipe
    //walks up to a single party member and does a spin while displaying their "ChargeUp" anim, makingtheir mantle of leaves smack a single party member, dealing 5/6 damage and inking the target for 2 turns if not blocked. (See Zommoth's Tail Attack for anim)

    //Attack 3 - Sleep Powder
    //They'll blow white mist from their pouch to put the entire party to sleep for 2 turns and deal 3/4 damage. Mash A to block like Spuder's Poison Breath.

    //Item Attack - Ink Bomb
    //Throw Bry's new Ink Bomb to damage Team Snakemouth and ink EVERYONE for 3 turns.

    //Support Move 1 - Rain Dance
    //Would summon a rain cloud above all enemies
    //It would heal the Shaman for 8 HP and all other entities for 4 HP and clear all negative status effects.This move will be prioritized if the enemy party is Inked.
    //    Support Move 2 - Call for backup
    //    There are 2 other enemy slots in the battle that can be filled by one of 4 enemies.If the Shaman calls for backup, 1 of 4 enemies will come in, and the front enemy slot will always get filled in before the back slot. Due to only having one action per turn, calling for Backup would not waste an action. He'd have a chance  to call for backup before his main action every turn (basically like Termite Knight's Taunt). If he's alone he'd have a 70 % chance to call for backup.If he has one ally he'd have a 40 % chance to call for backup. If both slots are filled he'd be unable to call for backup.
    //    For every turn he doesn't call for backup his chance to call for backup next turn goes up by 10% to prevent getting too lucky for several turns in a row.
    //Splotch Spider(30% chance)
    //Leafbug Ninja(25% chance)
    //Leafbug Archer(25% chance)
    //Leafbug Clubber(20% chance)
    //(the Leafbugs summoned for the boss would have 50% more hp than usual)

    //Passive Ablility -  Leafbug Unity...WITH CHARGE GUARD!
    //If a Leafbug is attacked, they'll gain +1 charge, but instead of  it boosting their attack, it'll boost their defense like with Charge Guard!

    //Resistances
    //Poison - 85
    //Numb - 70
    //Sleep - 55
    //Freeze - 70 

    public class LeafbugShamanAI : AI
    {
        enum Attacks
        {
            RainDance,
            StaffSpin,
            TailSwipe,
            SleepPowder,
            InkBomb
        }

        BattleControl battle = null;
        int STAFF_SPIN_DAMAGE = 2;
        int STAFF_SPIN_AMOUNT = 3;
        int TAIL_DAMAGE = 5;
        int POWDER_DAMAGE = 3;
        int RAIN_DANCE_HEAL = 4;
        public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
        {
            battle = MainManager.battle;
            battle.dontusecharge = true;

            if (battle.enemydata[actionid].data == null)
            {
                battle.enemydata[actionid].data = new int[2];
            }
            float hpPercent = battle.HPPercent(battle.enemydata[actionid]);

            if (battle.enemydata[actionid].hitaction)
            {
                yield return battle.EnemyAngryCharge(battle.enemydata[actionid].battleentity, 5);
                battle.enemydata[actionid].def = battle.enemydata[actionid].basedef + battle.enemydata[actionid].charge;
                yield break;
            }

            if (hpPercent <= 0.5f && battle.enemydata[actionid].data[1] == 0)
            {
                entity.animstate = (int)MainManager.Animations.Angry;
                entity.StartCoroutine(entity.ShakeSprite(0.1f, 45f));
                battle.StartCoroutine(battle.StatEffect(battle.enemydata[actionid].battleentity, 0));
                MainManager.SetCondition(MainManager.BattleCondition.AttackUp, ref battle.enemydata[actionid], 9999999);
                MainManager.PlaySound("StatUp");
                battle.enemydata[actionid].data[1] = 1;
                yield return EventControl.sec;
            }

            if (battle.enemydata.Length < 3)
            {
                int odds = battle.enemydata.Length == 1 ? 70 : 40;
                if(UnityEngine.Random.Range(0,100) < odds + battle.enemydata[actionid].data[0]*10)
                {
                    battle.enemydata[actionid].data[0] = 0;
                    yield return CallBackup(entity, actionid);
                    yield return EventControl.halfsec;
                }
                else
                {
                    battle.enemydata[actionid].data[0]++;
                }
            }

            List<Attacks> attacks = new List<Attacks>() { Attacks.TailSwipe, Attacks.InkBomb, Attacks.StaffSpin, Attacks.StaffSpin, Attacks.TailSwipe, Attacks.SleepPowder };

            if(MainManager.HasCondition(MainManager.BattleCondition.Inked, battle.enemydata[actionid])> -1)
            {
                attacks.AddRange(new Attacks[] { Attacks.RainDance, Attacks.RainDance });
            }

            if(hpPercent<=0.9f)
                attacks.Add(Attacks.RainDance);

            switch (attacks[UnityEngine.Random.Range(0, attacks.Count)])
            {
                case Attacks.RainDance:
                    yield return DoRainDance(entity, actionid);
                    break;
                case Attacks.TailSwipe:
                    yield return DoTailSwipe(entity, actionid);
                    break;
                case Attacks.InkBomb:
                    yield return Instance.UseItem(entity, actionid, battle, (MainManager.Items)NewItem.InkBomb);
                    break;
                case Attacks.StaffSpin:
                    yield return DoStaffSpin(entity, actionid);
                    break;
                case Attacks.SleepPowder:
                    yield return DoSleepPowder(entity, actionid);
                    break;
            }
        }

        IEnumerator CallBackup(EntityControl entity, int actionid)
        {
            entity.animstate = 107; //pulls out horn
            yield return EventControl.quartersec;
            entity.animstate = 109;
            MainManager.PlaySound("Horn", -1, 1f, 0.7f);
            yield return EventControl.sec;
            yield return EventControl.quartersec;
            entity.animstate = 108; //pulls horn back

            Dictionary<int, int> enemyTypes = new Dictionary<int, int>()
            {
                { (int)MainManager.Enemies.LeafbugArcher, 25},
                { (int)MainManager.Enemies.LeafbugClubber, 20},
                { (int)MainManager.Enemies.LeafbugNinja, 25},
                { (int)NewEnemies.SplotchSpider, 30}
            };

            int enemySpawn = MainManager_Ext.GetWeightedResult(enemyTypes); 
            Vector3 targetPos = battle.GetFreeSpace(true).Value;

            battle.checkingdead = battle.StartCoroutine(battle.SummonEnemy(BattleControl.SummonType.Offscreen, enemySpawn, targetPos, true));
            yield return new WaitForSeconds(0.85f);
            entity.animstate = 0;
            while (battle.checkingdead != null)
            {
                yield return null;
            }
        }

        IEnumerator DoRainDance(EntityControl entity, int actionid)
        {
            bool baseFlip = entity.flip;
            //MainManager.PlaySound("PeacockSpiderDoubleTurnDance",0.8f,1);
            MainManager.PlaySound("PeacockSpiderNPCBuff", 1.2f, 1);
            entity.animstate = 102;
            Coroutine flipAround = entity.StartCoroutine(EventControl_Ext.FlipAround(entity));

            yield return EventControl.quartersec;

            GameObject rainCloud = Instance.CreateRainCloud(entity.transform.position + Vector3.up * 7, entity,60f);
            ParticleSystem rain = rainCloud.GetComponentInChildren<ParticleSystem>();

            yield return EventControl.sec;
            yield return EventControl.quartersec;
            rain.Play();
            MainManager.PlaySound("Water0", 1.2f, 0.5f);
            yield return EventControl.halfsec;

            MainManager.PlaySound("Heal");
            MainManager.PlaySound("Heal3");
            for (int i = 0; i < battle.enemydata.Length; i++)
            {
                int healAmount = RAIN_DANCE_HEAL;

                if (i == actionid)
                    healAmount += RAIN_DANCE_HEAL;
                battle.Heal(ref battle.enemydata[i], healAmount, false);
                Instance.CureNegativeStatus(ref battle.enemydata[i]);
                MainManager.PlayParticle("MagicUp", battle.enemydata[i].battleentity.transform.position);
            }
            yield return EventControl.sec;
            MainManager.StopSound("Water0");

            yield return EventControl.halfsec;
            entity.flip = baseFlip;
            entity.animstate = 0;
            entity.StopCoroutine(flipAround);
            yield return Instance.DestroyRainCloud(rainCloud, entity);
        }

        IEnumerator DoTailSwipe(EntityControl entity, int actionid)
        {
            entity.animstate = (int)MainManager.Animations.Angry;
            yield return EventControl.halfsec;

            battle.GetSingleTarget();
            EntityControl targetEntity = battle.playertargetentity;

            battle.CameraFocusTarget();
            entity.MoveTowards(targetEntity.transform.position + new Vector3(2f, 0f, 0.1f), 1f);
            while (entity.forcemove)
            {
                yield return null;
            }

            entity.animstate = (int)MainManager.Animations.Angry;

            yield return EventControl.tenthsec;
            MainManager.PlaySound("LeafFlipBack",0.8f,1);
            battle.StartCoroutine(entity.ShakeSprite(0.1f, 30f));

            float a = 0f;
            float b = 30f;
            do
            {
                entity.spritetransform.localEulerAngles = new Vector3(0f, Mathf.LerpAngle(0f, -30f, a / b));
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b + 1f);

            MainManager.PlaySound("ZammothSwipe",1.2f,1);
            entity.spritetransform.localEulerAngles = Vector3.zero;
            entity.animstate = 0;
            entity.overrideflip = true;

            battle.StartCoroutine(MainManager.Spin(entity.sprite.transform, new Vector3(0f, 360f), 30f, true));
            yield return EventControl.quartersec;
            battle.DoDamage(actionid, battle.playertargetID, TAIL_DAMAGE, BattleControl.AttackProperty.Ink, battle.commandsuccess);

            if (!battle.commandsuccess)
            {
                battle.enemybounce = new Coroutine[] { battle.StartCoroutine(battle.BounceEnemy(MainManager.instance.playerdata[battle.playertargetID], 0, 1, 1.25f)) };
            }
            yield return EventControl.halfsec;
            entity.overrideflip = false;

            if (battle.enemybounce != null)
            {
                while (battle.enemybounce[0] != null)
                {
                    yield return null;
                }
            }
        }

        IEnumerator DoStaffSpin(EntityControl entity, int actionid)
        {
            battle.GetSingleTarget();
            EntityControl targetEntity = battle.playertargetentity;

            battle.CameraFocusTarget();
            entity.MoveTowards(targetEntity.transform.position + new Vector3(2f, 0f, 0.1f), 1f);
            while (entity.forcemove)
            {
                yield return null;
            }

            entity.animstate = 100;
            
            yield return EventControl.tenthsec;
            MainManager.PlaySound("Spin2");
            entity.animstate = 101;
            for(int i = 0; i < STAFF_SPIN_AMOUNT; i++)
            {
                battle.DoDamage(actionid, battle.playertargetID, STAFF_SPIN_DAMAGE, BattleControl.AttackProperty.Ink, battle.commandsuccess);
                yield return new WaitForSeconds(0.65f);
            }
            MainManager.StopSound("Spin2");
            entity.animstate = 100;
            BattleControl.SetDefaultCamera();
            yield return EventControl.quartersec;
        }

        IEnumerator DoSleepPowder(EntityControl entity, int actionid)
        {
            battle.nonphyscal = true;

            entity.MoveTowards(new Vector3(0, 0, -0.5f));
            while (entity.forcemove)
                yield return null;

            MainManager.PlaySound("BagRustle");
            entity.animstate = 103;
            MainManager.instance.camoffset = new Vector3(0f, 2.25f, -6.5f);
            MainManager.instance.camtarget = entity.sprite.transform;
            MainManager.instance.camspeed = 0.03f;
            yield return EventControl.halfsec;

            entity.animstate = 104;
            yield return EventControl.quartersec;

            BattleControl.SetDefaultCamera();
            MainManager.PlaySound("Mist");
            GameObject smokeParticle = MainManager.PlayParticle("poisonsmoke", null, entity.transform.position + new Vector3(-1f, 1.3f), new Vector3(0f, -90f, 90f), 7.5f);
            
            ParticleSystem ps = smokeParticle.GetComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = new MinMaxGradient(Color.blue, new Color(1, 1, 1, 0.5f));

            battle.CreateHelpBox(4);
   
            float commandTime = 165f;
            float[] data = new float[] { 4f, 8f, 0f, 1f };

            //barfill decreased time
            data[2] = battle.HardMode() ? 1.35f : 1f;

            battle.StartCoroutine(battle.DoCommand(commandTime,BattleControl.ActionCommands.TappingKey, data));

            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.instance.playerdata[i].hp > 0)
                {
                    MainManager.instance.playerdata[i].battleentity.spin = new Vector3(0f, 15f);
                }
            }

            while (battle.doingaction)
            {
                for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                {
                    if (MainManager.instance.playerdata[i].hp > 0)
                    {
                        MainManager.instance.playerdata[i].battleentity.animstate = battle.blockcooldown > 0f ? 11 : 24;
                    }
                }
                yield return null;
            }

            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.instance.playerdata[i].hp > 0)
                {
                    bool commandSuccess = battle.barfill >= 1f;
                    battle.DoDamage(actionid, i, POWDER_DAMAGE, BattleControl.AttackProperty.Sleep, commandSuccess);
                    MainManager.instance.playerdata[i].battleentity.spin = Vector3.zero;
                }
            }
            entity.animstate = 0;
            battle.DestroyHelpBox();
        }
    }
}
