using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Experimental.UIElements;
using UnityEngine;
using static BattleControl;
using static MainManager;
using static BFPlus.Extensions.BattleControl_Ext;
using static UnityEngine.ParticleSystem;
using System.Security.Cryptography;
using static FlappyBee;
using System.Runtime.InteropServices;

namespace BFPlus.Extensions.EnemyAI
{
//Attacks(Grounded) - focus on multi-target attacks, and buffing/debuffing
//Empowering Breath(can only be used once per turn);
//Gives another enemy an attack and defense buff for 3 turns.Same as Venus' Bud healing breath, but will use blue and red mist instead of pink/salmon.
//Weakening Breath
//goes into the ground and Re-appears in front of Team Snakemouth.It will then proceed to start blowing out a white mist towards towards the entire party.
//The party must mash the A button to fill the bar to block the attack. (see Spuder's poison breath for reference). This attack will deal 3/4 damage to everyone. 
//If not blocked, the entire party's attack AND defense will be lowered for 3 turns. If blocked the attack and denfense will only be lowered for 2 turns.

//Pollen Lob;
//Will blow 4/5 pieces of pollen upward in a rythmic pattern, movig off-screen.Then they'll come back towards down towards random members of Team Snakemouth.
//There will be two types of pollen for this attack selected at random, each with a side Effect other than just dealing 2 damage un-blocked;
//A White pollen that will make the target unable to use items for 1 turn if not blocked, 
//and a yellow pollen that will sap 2 TP from the party (1 TP if blocked).

//if grounded at the end of the turn, it will have a passive healing Effect, just like other grounded plant enemies. It will heal 2/3 HP.

//It also shares Venus' Bud's mechanic of becoming airborn when struck with any attack.
//When becoming airborne will scatter green particles around it, giving all other enemies +1 charge.
//When airborn it can be knocked out of the air like any flying enemy.

//Attacks (Airborn) - focus on single target attacks, more straightforward attacks
//Airborn Pollen Spit
//Will spit a barrage of 4/5 pollen at a single target. These consist of the same two types of pollen for Pollen Lob and type is selected at random.

//Dive Bomb
//Will hover a bit higher in the air, then diving bombing random target to deal 6/8 damage, while playing its "screaming" animation. 
//This should ideally have an after-image effect too. (inspired by that funny bugged Abomniberry Dive bomb ⁠bug-reports⁠).
    public class MarsBudAI : AI
    {
        enum Attacks
        {
            EmpowerBreath,
            WeakeningBreath,
            PollenLob,
            PollenSpit,
            DiveBomb
        }
        const int PASSIVE_HEAL = 2;
        const int WEAKENING_BREATH_DAMAGE = 4;
        int POLLEN_LOB_DAMAGE = 3;
        const int POLLEN_LOB_AMOUNT = 5;
        const int POLLEN_LOB_TPDRAIN = -2;
        const int POLLEN_SPIT_AMOUNT = 4;
        const int DIVE_BOMB_DAMAGE = 6;
        BattleControl battle = null;
        public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
        {
            battle = MainManager.battle;
            if (battle.enemydata[actionid].data == null || battle.enemydata[actionid].data.Length == 0)
            {
                battle.enemydata[actionid].data = new int[] { -1, -1 };
            }

            if (battle.enemydata[actionid].hitaction)
            {
                yield return FlyUp(entity, actionid);
                yield break;
            }

            battle.enemydata[actionid].data[1]--;

            List<Attacks> attacks = new List<Attacks>();
            int empowerBreathTarget = -1;

            if (battle.enemydata[actionid].position == BattleControl.BattlePosition.Flying)
            {
                if (battle.enemydata[actionid].data[0] <= 0 || (battle.enemydata[actionid].data[0] == 1 && battle.enemydata.Length == 1))
                {
                    yield return FlyDown(entity, actionid);
                }
                else
                {
                    battle.enemydata[actionid].data[0]--;
                    attacks.AddRange(new Attacks[] { Attacks.PollenSpit, Attacks.DiveBomb });
                }
            }

            if(battle.enemydata[actionid].position == BattleControl.BattlePosition.Ground)
            {
                for(int i = 0; i < battle.enemydata.Length; i++)
                {
                    if(i != actionid && battle.enemydata[i].position == BattleControl.BattlePosition.Ground)
                    {
                        empowerBreathTarget = i;
                        break;
                    }
                }

                if(empowerBreathTarget != -1 && battle.enemydata[actionid].data[1] <= 0)
                {
                    attacks.Add(Attacks.EmpowerBreath);
                }

                attacks.AddRange(new Attacks[] { Attacks.WeakeningBreath, Attacks.PollenLob });
            }


            switch (attacks[UnityEngine.Random.Range(0, attacks.Count)])
            {
                case Attacks.EmpowerBreath:
                    yield return DoEmpowerBreath(entity, empowerBreathTarget);
                    battle.enemydata[actionid].data[1] = 2;
                    break;
                case Attacks.WeakeningBreath:
                    yield return DoWeakeningBreath(entity, actionid);
                    break;
                case Attacks.PollenLob:
                    yield return DoPollenLob(entity,actionid);
                    break;
                case Attacks.DiveBomb:
                    yield return DoDiveBomb(entity, actionid);
                    break;
                case Attacks.PollenSpit:
                    yield return DoPollenSpit(entity, actionid);
                    break;
            }
   
            //Passively heals if on the ground
            if (battle.enemydata[actionid].position == BattleControl.BattlePosition.Ground)
            {
                BattleControl.SetDefaultCamera();
                entity.flip = false;
                entity.animstate = 0;
                yield return EventControl.quartersec;
                battle.Heal(ref battle.enemydata[actionid], PASSIVE_HEAL, false);
                yield return EventControl.quartersec;
            }
        }

        IEnumerator DoDiveBomb(EntityControl entity, int actionid)
        {
            battle.GetSingleTarget();
            MainManager.instance.camoffset += new Vector3(-1.5f, 0f);
            MainManager.instance.camtarget = entity.transform;
            Vector3 basePos = entity.transform.position;
            float baseHeight = entity.height;

            float a = 0f;
            float b = 40f;
            float lastHeight = entity.height;
            MainManager.PlaySound("PingUp");
            entity.animstate = 100;
            do
            {
                entity.height = Mathf.Lerp(lastHeight, lastHeight + 2f, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < 40f);

            entity.animstate = 101;
            a = 0f;
            lastHeight = entity.height;

            entity.trail = true;
            entity.overrideflip = true;
            entity.sprite.transform.localEulerAngles = new Vector3(0f, 0f, 20f);

            EntityControl playerTargetEntity = battle.playertargetentity;
            Vector3 targetPos = playerTargetEntity.transform.position + new Vector3(1f, 0f, -0.1f);

            MainManager.instance.camtarget = playerTargetEntity.transform;
            MainManager.instance.camoffset += new Vector3(2f, 0f);

            MainManager.PlaySound("PingShot");
            MainManager.PlaySound("FastWoosh");

            b = 20f;
            Vector3 startPos = entity.transform.position;
            do
            {
                entity.height = Mathf.Lerp(lastHeight, 1f, a / b);
                entity.transform.position = Vector3.Lerp(startPos, targetPos, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b);

            entity.trail = false;
            battle.DoDamage(actionid, battle.playertargetID, DIVE_BOMB_DAMAGE, null, battle.commandsuccess);
            yield return new WaitForSeconds(0.2f);
            lastHeight = entity.height;
            entity.animstate = 0;

            a = 0f;
            entity.overrideflip = false;
            entity.sprite.transform.localEulerAngles = Vector3.zero;
            startPos = entity.transform.position;
            do
            {
                entity.height = Mathf.Lerp(lastHeight, baseHeight, a / b);
                entity.transform.position = Vector3.Lerp(startPos, basePos, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b);
        }

        IEnumerator DoPollenSpit(EntityControl entity, int actionid)
        {
            battle.nonphyscal = true;
            bool hardmode = battle.HardMode();
            if (hardmode)
            {
                POLLEN_LOB_DAMAGE--;
            }

            battle.GetSingleTarget();
            EntityControl playerTargetEntity = battle.playertargetentity;
            int playerTargetId = battle.playertargetID;

            MainManager.instance.camtarget = null;
            MainManager.instance.camtargetpos = new Vector3?(Vector3.Lerp(playerTargetEntity.transform.position, entity.transform.position, 0.5f));
            MainManager.instance.camoffset = new Vector3(0f, 2.75f, -7f);
            MainManager.instance.camspeed = 0.05f;

            entity.animstate = 100;
            yield return EventControl.halfsec;
            entity.animstate = 103;
            yield return EventControl.tenthsec;

            SpriteRenderer[] pollens = new SpriteRenderer[POLLEN_SPIT_AMOUNT];
            List<Coroutine> routines = new List<Coroutine>();
            for (int i = 0; i < pollens.Length; i++)
            {
                MainManager.PlaySound("Spit");
                bool isYellow = UnityEngine.Random.Range(0, 2) == 1;
                pollens[i] = MainManager.NewSpriteObject(entity.transform.position + new Vector3(-0.9f, 1f + entity.height, -0.1f), null, MainManager.instance.projectilepsrites[5]);
                pollens[i].gameObject.AddComponent<ShadowLite>().SetUp(0.4f, 0.5f);
                pollens[i].gameObject.AddComponent<SpinAround>().itself = new Vector3(0f, 0f, 10f);

                if (isYellow)
                {
                    pollens[i].material.color = Color.yellow;
                }
                routines.Add(battle.StartCoroutine(PollenFall(actionid, pollens[i], playerTargetId, playerTargetEntity, isYellow, 35f,routines,i)));
                yield return new WaitForSeconds(hardmode ? 0.225f : 0.3f);
            }
            entity.animstate = 0;

            yield return new WaitUntil(() => MainManager.ArrayIsEmpty(routines.ToArray()));
            for (int i = 0; i < pollens.Length; i++)
            {
                if (pollens[i] != null)
                {
                    UnityEngine.Object.Destroy(pollens[i].gameObject);
                }
            }
        }

        IEnumerator DoPollenLob(EntityControl entity, int actionid)
        {
            battle.nonphyscal=true;
            int baseAmount = POLLEN_LOB_AMOUNT;

            if (battle.HardMode())
            {
                POLLEN_LOB_DAMAGE--;
                baseAmount += 2;
            }
            MainManager.instance.camtarget = entity.transform;
            MainManager.instance.camoffset = new Vector3(-1.5f, 3.25f, -8.5f);
            entity.animstate = 100;
            yield return new WaitForSeconds(0.75f);

            SpriteRenderer[] pollens = new SpriteRenderer[baseAmount];
            int[] pollenType = new int[baseAmount];
            for(int i = 0; i < pollens.Length; i++)
            {
                entity.animstate = 105;
                yield return new WaitForSeconds(0.1f);
                entity.animstate = 106;
                yield return new WaitForSeconds(0.1f);
                MainManager.PlaySound("Spit");

                pollenType[i] = UnityEngine.Random.Range(0, 2);

                pollens[i] = MainManager.NewSpriteObject(entity.transform.position + new Vector3(0f, 2.25f, -0.1f), null, MainManager.instance.projectilepsrites[5]);
                if(pollenType[i] == 1)
                {
                    pollens[i].material.color = Color.yellow;
                }
                pollens[i].transform.parent = battle.battlemap.transform;
                pollens[i].gameObject.AddComponent<SpinAround>().itself = new Vector3(0f, 0f, -10f);
                battle.StartCoroutine(SpitPollenUp(entity, pollens[i]));
                pollens[i].transform.position = new Vector3(0f, 10f, 0f);
                yield return EventControl.tenthsec;
            }

            entity.animstate = 0;
            SetDefaultCamera();

            int playerID;
            List<Coroutine> routines = new List<Coroutine>();
            for (int i = 0; i < pollens.Length; i++)
            {
                yield return EventControl.tenthsec;
                yield return EventControl.tenthsec;

                if (MainManager.GetAlivePlayerAmmount() == 0)
                {
                    break;
                }
                playerID = battle.GetRandomAvaliablePlayer();
                EntityControl targetEntity = MainManager.instance.playerdata[playerID].battleentity;

                MainManager.PlaySound("Fall", 0.9f, 0.7f);
                routines.Add(battle.StartCoroutine(PollenFall(actionid, pollens[i], playerID, targetEntity, pollenType[i] == 1, 40f, routines, i)));
                yield return EventControl.tenthsec;
            }

            yield return new WaitUntil(() => MainManager.ArrayIsEmpty(routines.ToArray()));
            for (int i = 0; i < pollens.Length; i++)
            {
                if (pollens[i] != null) 
                {
                    UnityEngine.Object.Destroy(pollens[i].gameObject);
                }
            }
        }

        IEnumerator PollenFall(int actionid, SpriteRenderer pollen, int targetId, EntityControl targetEntity, bool isYellow, float speed, List<Coroutine> routines=null, int routineID=-1)
        {
            Vector3 startPos = pollen.transform.position;
            float a = 0;
            do
            {
                pollen.transform.position = MainManager.SmoothLerp(startPos, targetEntity.transform.position + Vector3.left * 0.1f + Vector3.up, a / speed);
                a += MainManager.framestep;
                yield return null;
            }
            while (a < speed);

            AttackProperty? property = null;

            if (!isYellow)
                property = BattleControl.AttackProperty.Sticky;

            if (MainManager.instance.playerdata[targetId].hp > 0)
            {
                battle.DoDamage(actionid, targetId, POLLEN_LOB_DAMAGE, property, battle.commandsuccess);

                if (isYellow)
                {
                    startPos = targetEntity.transform.position + Vector3.up * 2f;
                    Vector3 endPos = targetEntity.transform.position + Vector3.up * 4f;
                    BattleControl_Ext.Instance.RemoveTP(!battle.commandsuccess ? POLLEN_LOB_TPDRAIN : POLLEN_LOB_TPDRAIN + 1, startPos, endPos);
                }
            }
            pollen.gameObject.SetActive(false);

            if (routines != null)
                routines[routineID] = null;
        }

        IEnumerator SpitPollenUp(EntityControl entity, SpriteRenderer pollen)
        {
            float a = 0f;
            Vector3 startPos = pollen.transform.position;
            do
            {
                pollen.transform.position = Vector3.Lerp(startPos, new Vector3(entity.transform.position.x - 1f, 10f, 0f), a / 50f);
                pollen.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, a / 30f);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < 50f);
            yield return null;
            pollen.transform.position = new Vector3(0f, 10f, 0f);
        }

        IEnumerator DoEmpowerBreath(EntityControl entity, int target)
        {
            battle.dontusecharge = true;
            MainManager.instance.camtarget = null;
            MainManager.instance.camtargetpos = new Vector3?(Vector3.Lerp(battle.enemydata[target].battleentity.transform.position, entity.transform.position, 0.5f));
            MainManager.instance.camoffset = new Vector3(0f, 2.5f, -6.15f);
            MainManager.instance.camspeed = 0.05f;
            entity.FaceTowards(battle.enemydata[target].battleentity.transform.position);
            entity.animstate = 100;
            yield return EventControl.halfsec;
            entity.animstate = 103;
            yield return EventControl.tenthsec;
            MainManager.PlaySound("HealBreath");
            GameObject smokeParticle = MainManager.PlayParticle("HealSmoke", null, entity.transform.position + new Vector3(entity.flip ? 0.9f : (-0.9f), 1f, -0.1f), new Vector3(0f, (float)(entity.flip ? 90 : (-90)), 90f), 5f);
            ParticleSystem ps = smokeParticle.GetComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = new MinMaxGradient(Color.blue, Color.red);

            yield return EventControl.sec;
            MainManager.PlaySound("StatUp");
            MainManager.SetCondition(MainManager.BattleCondition.AttackUp, ref battle.enemydata[target], 3);
            MainManager.SetCondition(MainManager.BattleCondition.DefenseUp, ref battle.enemydata[target], 3);
            battle.StartCoroutine(battle.StatEffect(battle.enemydata[target].battleentity, 1));
            battle.StartCoroutine(battle.StatEffect(battle.enemydata[target].battleentity, 0));
            yield return new WaitForSeconds(1.15f);
        }

        IEnumerator DoWeakeningBreath(EntityControl entity, int actionid)
        {
            battle.nonphyscal = true;
            Vector3 startPos = entity.transform.position;

            yield return MoveBud(entity, new Vector3(0, 0, 0));
            yield return EventControl.tenthsec;

            MainManager.PlaySound("Blosh");
            entity.animstate = 100;
            MainManager.instance.camoffset = new Vector3(0f, 2.25f, -6.5f);
            MainManager.instance.camtarget = entity.sprite.transform;
            MainManager.instance.camspeed = 0.03f;
            yield return EventControl.halfsec;

            entity.animstate = 102;
            entity.StartCoroutine(entity.ShakeSprite(0.1f, 60f));
            yield return EventControl.sec;

            entity.animstate = 103;
            BattleControl.SetDefaultCamera();
            MainManager.PlaySound("Mist");
            GameObject smokeParticle = MainManager.PlayParticle("poisonsmoke", null, entity.transform.position + new Vector3(-1f, 1.3f), new Vector3(0f, -90f, 90f), 7.5f);
            ParticleSystem ps = smokeParticle.GetComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = new MinMaxGradient(Color.white, new Color(1,1,1,0.5f));

            battle.CreateHelpBox(4);

            float commandTime = 165f;
            float[] data = new float[] { 4f, 8f, 0f, 1f };

            //barfill decreased time
            data[2] = battle.HardMode() ? 1.35f : 1f;

            battle.StartCoroutine(battle.DoCommand(commandTime, ActionCommands.TappingKey, data));

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

            MainManager.PlaySound("StatDown");
            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.instance.playerdata[i].hp > 0)
                {
                    bool commandSuccess = battle.barfill >= 1f;
                    battle.DoDamage(actionid, i, WEAKENING_BREATH_DAMAGE, null, commandSuccess);
                    MainManager.instance.playerdata[i].battleentity.spin = Vector3.zero;
                    int debuffTurns = 2;
                    if (!commandSuccess)
                    {
                        debuffTurns++;
                    }

                    MainManager.SetCondition(MainManager.BattleCondition.AttackDown, ref MainManager.instance.playerdata[i], debuffTurns);
                    MainManager.SetCondition(MainManager.BattleCondition.DefenseDown, ref MainManager.instance.playerdata[i], debuffTurns);
                    battle.StartCoroutine(battle.StatEffect(MainManager.instance.playerdata[i].battleentity, 2));
                    battle.StartCoroutine(battle.StatEffect(MainManager.instance.playerdata[i].battleentity, 3));
                }
            }
            entity.animstate = 0;
            battle.DestroyHelpBox();
            yield return MoveBud(entity, startPos);
        }

        IEnumerator ChangeScaleBud(EntityControl entity, Vector3 targetScale, float endTime)
        {
            Vector3 startScale = entity.startscale;
            entity.spin = new Vector3(0, 20);
            float a = 0f;
            do
            {
                entity.startscale = Vector3.Lerp(startScale, targetScale, a / endTime);
                entity.sprite.transform.localScale = Vector3.Lerp(startScale, targetScale, a / endTime);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < endTime);
            entity.spin = Vector3.zero;
        }

        IEnumerator MoveBud(EntityControl entity, Vector3 targetPos)
        {
            Vector3 startScale = entity.startscale;
            entity.shadowsize = 0;
            MainManager.PlaySound("ChargeDown");
            yield return ChangeScaleBud(entity, Vector3.zero, 30f);
            yield return EventControl.halfsec;

            entity.transform.position = targetPos;
            MainManager.PlaySound("Charge");
            MainManager.DeathSmoke(entity.transform.position);
            yield return ChangeScaleBud(entity, startScale, 30f);
            entity.shadowsize = 1;
            yield return EventControl.tenthsec;
        }

        IEnumerator FlyUp(EntityControl entity, int actionid)
        {
            battle.dontusecharge = true;
            battle.enemydata[actionid].data[0] = 3;
            entity.animstate = 101;
            entity.emoticoncooldown = 30f;
            entity.emoticonid = 2;
            MainManager.PlaySound("Wam");
            while (entity.emoticoncooldown > 0f)
            {
                yield return null;
            }
            entity.spin = new Vector3(0f, -20f);
            MainManager.PlaySound("Rumble3");
            MainManager.ShakeScreen(0.7f);
            yield return new WaitForSeconds(0.7f);
            MainManager.PlayParticle("DirtExplode", entity.transform.position);
            GameObject flowerImpact = MainManager.PlayParticle("FlowerImpact", entity.transform.position);
            ParticleSystem ps = flowerImpact.GetComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = new MinMaxGradient(Color.green);

            MainManager.PlaySound("Charge7");
            for (int i=0;i < battle.enemydata.Length; i++)
            {
                battle.StartCoroutine(battle.StatEffect(battle.enemydata[i].battleentity, 4));
                battle.enemydata[i].charge += 1;
                battle.enemydata[i].charge = Mathf.Clamp(battle.enemydata[i].charge, 1, 3);
            }

            entity.spin = new Vector3(0f, -30f);
            float a = 0f;
            float b = 40f;
            entity.spinextra[0] = new Vector3(0f, -20f);
            MainManager.PlaySound("Charge6");
            do
            {
                entity.oldstate = -1;
                entity.height = MainManager.BeizierCurve(entity.transform.position, entity.transform.position + Vector3.up * 3f, 5f, a / b).y;
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b);
            entity.bobrange = entity.startbf;
            entity.bobspeed = entity.startbs;
            entity.spin = Vector3.zero;
            yield return EventControl.halfsec;
            battle.enemydata[actionid].position = BattleControl.BattlePosition.Flying;
        }

        IEnumerator FlyDown(EntityControl entity, int actionid)
        {
            battle.enemydata[actionid].data[0] = -1;
            MainManager.PlaySound("ChargeDown");
            entity.animstate = 101;
            entity.spin = new Vector3(0f, -20f);

            float a = 0f;
            float b = 30f;
            float startHeight = entity.height;
            do
            {
                entity.height = Mathf.Lerp(startHeight, 0f, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b);

            entity.height = 0f;
            entity.animstate = 0;
            entity.oldfly = !entity.oldfly;
            entity.spin = Vector3.zero;
            entity.FlipAngle(true);

            battle.enemydata[actionid].position = BattleControl.BattlePosition.Ground;
            yield return EventControl.quartersec;
        }
    }
}
