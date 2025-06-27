using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BattleControl;
using UnityEngine;
using static BFPlus.Extensions.BattleControl_Ext;
using UnityEngine.Experimental.UIElements;

namespace BFPlus.Extensions.EnemyAI
{
    public class LeviAI : AI
    {

        enum Attacks
        {
            AirSlash,
            Slash,
            HealingItem,
            AttackingItem,
            Revive,
            Relay,
            BuffingItem,
        }

        BattleControl battle = null;
        const int SLASH_DAMAGE = 3;
        const int AIR_SLASH_DAMAGE = 4;
        public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
        {
            battle = MainManager.battle;

            int celiaIndex = battle.EnemyInField((int)NewEnemies.Celia);
            float hpPercent = battle.HPPercent(battle.enemydata[actionid]);

            if (battle.enemydata[actionid].data == null)
            {
                battle.enemydata[actionid].data = new int[4];
                battle.enemydata[actionid].data[3] = 5;
            }

            MainManager.Items[] healingItems = new MainManager.Items[]
            {
                MainManager.Items.Omelet,
                MainManager.Items.YamBread,
                MainManager.Items.MushroomCandy,
                MainManager.Items.MiteBurg,
                MainManager.Items.BerryJuice
            };

            MainManager.Items[] buffingItems = new MainManager.Items[]
            {
                MainManager.Items.SpicyCandy,
                MainManager.Items.CoffeeCandy,
                MainManager.Items.ProteinShake,
                MainManager.Items.HustleSeed,
                MainManager.Items.FrenchFries
            };

            MainManager.Items[] attackingItems = new MainManager.Items[]
            {
                (MainManager.Items)NewItem.SeedlingWhistle,
                MainManager.Items.PoisonBomb,
                MainManager.Items.SpicyBomb,
            };

            if (battle.reservedata.Count != 0 && battle.enemydata[actionid].data[0] == 0)
            {
                battle.enemydata[actionid].data[0] = 1;
                battle.enemydata[actionid].data[3]--;
                yield return Instance.UseItem(entity, UnityEngine.Random.Range(0,battle.reservedata.Count), battle, MainManager.Items.MagicDrops);
                yield break;
            }

            List<Attacks> chances = new List<Attacks>() { Attacks.Slash, Attacks.AirSlash, Attacks.AirSlash, Attacks.Slash };

            if (battle.enemydata[actionid].data[1] <= 0 && Instance.GetLowHPEnemy() > -1)
            {
                chances.Add(Attacks.HealingItem);
            }

            if (battle.enemydata[actionid].data[1] <= 0  && hpPercent >0.3f&& battle.enemydata[actionid].data[3] > 0)
            {
                chances.Add(Attacks.BuffingItem);
            }

            if (battle.enemydata[actionid].data[1] <= 0 && hpPercent <= 0.65f)
            {
                chances.AddRange(new Attacks[] { Attacks.AttackingItem, Attacks.AttackingItem });
            }

            if (battle.enemydata[actionid].data[1] > 0)
            {
                battle.enemydata[actionid].data[1]--;
            }


            if(celiaIndex > -1 && battle.enemydata[celiaIndex].data != null && battle.enemydata[actionid].data[2] != 1)
            {
                if (Instance.CanRelay(entity, battle))
                {
                    chances.Add(Attacks.Relay);
                }
            }

            switch (chances[UnityEngine.Random.Range(0, chances.Count)])
            {
                case Attacks.AirSlash:
                    yield return DoAirSlash(entity, actionid);
                    break;
                case Attacks.Slash:
                    yield return DoSlash(entity, actionid);
                    break;

                case Attacks.HealingItem:
                    int lowHp = Instance.GetLowHPEnemy();
                    int randomEnemy = UnityEngine.Random.Range(0, battle.enemydata.Length);

                    yield return Instance.UseItem(entity, lowHp == -1 ? randomEnemy : lowHp, battle, healingItems[UnityEngine.Random.Range(0,healingItems.Length)]);
                    battle.enemydata[actionid].data[1] = 1;
                    battle.enemydata[actionid].data[3]--;
                    break;

                case Attacks.BuffingItem:
                    yield return Instance.UseItem(entity, actionid, battle, buffingItems[UnityEngine.Random.Range(0, buffingItems.Length)]);
                    battle.enemydata[actionid].data[1] = 1;
                    battle.enemydata[actionid].data[3]--;
                    break;

                case Attacks.AttackingItem:
                    yield return Instance.UseItem(entity, actionid, battle, attackingItems[UnityEngine.Random.Range(0, attackingItems.Length)]);
                    battle.enemydata[actionid].data[1] = 1;
                    battle.enemydata[actionid].data[3]--;
                    break;

                case Attacks.Relay:
                    battle.enemydata[celiaIndex].data[2] = 1;
                    yield return battle.EnemyRelay(entity, BattleControl_Ext.Instance.FindRelayable(entity, battle));
                    yield break;
            }
            battle.enemydata[actionid].data[2] = 0;
        }


        IEnumerator DoSlash(EntityControl entity, int actionid)
        {
            battle.GetSingleTarget();
            int playerTargetID = battle.playertargetID;
            EntityControl playertargetentityRef = battle.playertargetentity;

            battle.CameraFocusTarget();

            entity.MoveTowards(playertargetentityRef.transform.position + new Vector3(1.5f, 0f, -0.1f), 2f, 1,0);
            while (entity.forcemove)
            {
                yield return null;
            }
            entity.animstate = 100;
            yield return EventControl.thirdsec;
            battle.DoDamage(actionid, playerTargetID, SLASH_DAMAGE, AttackProperty.DefDownOnBlock, battle.commandsuccess);

            yield return EventControl.quartersec;
            BattleControl.SetDefaultCamera();
        }

        IEnumerator DoAirSlash(EntityControl entity, int actionid)
        {
            battle.nonphyscal = true;
            battle.GetSingleTarget();

            int playerTargetID = battle.playertargetID;
            EntityControl playertargetentityRef = battle.playertargetentity;
            entity.animstate = 100;
            MainManager.PlaySound("FastWoosh", 1.2f, 1);
            yield return null;
            entity.overrideanimspeed = true;
            entity.anim.speed = 0f;
            yield return EventControl.halfsec;


            entity.anim.speed = 1f;
            entity.overrideanimspeed = false;
            yield return EventControl.thirdsec;
            SpriteRenderer slash = MainManager.NewSpriteObject(entity.transform.position + new Vector3(-1f, 1.1f, -0.1f), null, MainManager.instance.projectilepsrites[7]);
            slash.transform.localScale = Vector3.one * 1.5f;

            MainManager.PlaySound("WaspKingAxToss", 1.2f, 1f);
            battle.StartCoroutine(battle.Projectile(AIR_SLASH_DAMAGE, null, battle.enemydata[actionid], playerTargetID, slash.transform, 20, 0, null, null, null, null, Vector3.zero, false));

            yield return EventControl.quartersec;
            while (slash != null)
            {
                yield return null;
            }
        }
    }
}
