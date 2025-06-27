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
namespace BFPlus.Extensions.EnemyAI
{
    public class CeliaAI : AI
    {
        enum Attacks
        {
            ShieldThrow,
            MultiPunch,
            HealingItem,
            AttackingItem,
            BuffingItem,
            Revive,
            Relay
        }

        BattleControl battle = null;
        const int SHIELD_DAMAGE = 2;
        const int PUNCH_DAMAGE = 2;
        public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
        {
            battle = MainManager.battle;

            int leviIndex = battle.EnemyInField(new int[] { (int)NewEnemies.Levi });
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
                MainManager.Items.BurlyCandy,
                MainManager.Items.CoffeeCandy,
                MainManager.Items.ProteinShake,
                MainManager.Items.HustleSeed,
                MainManager.Items.FrenchFries
            };

            MainManager.Items[] attackingItems = new MainManager.Items[]
            {
                MainManager.Items.SleepBomb,
                MainManager.Items.BurlyBomb,
                MainManager.Items.LonglegSummoner
            };

            if (battle.reservedata.Count != 0 && battle.enemydata[actionid].data[0] == 0 && battle.enemydata[actionid].data[3] > 0)
            {
                battle.enemydata[actionid].data[0] = 1;
                battle.enemydata[actionid].data[3]--;
                yield return Instance.UseItem(entity, UnityEngine.Random.Range(0, battle.reservedata.Count), battle, MainManager.Items.MagicDrops);
                yield break;
            }

            List<Attacks> chances = new List<Attacks>() { Attacks.ShieldThrow, Attacks.MultiPunch, Attacks.ShieldThrow };

            if (battle.enemydata[actionid].data[1] <= 0 && Instance.GetLowHPEnemy() > -1 && battle.enemydata[actionid].data[3] > 0)
            {
                chances.Add(Attacks.HealingItem);
            }

            if (battle.enemydata[actionid].data[1] <= 0 && hpPercent > 0.3f && battle.enemydata[actionid].data[3] > 0)
            {
                chances.Add(Attacks.BuffingItem);
            }

            if (battle.enemydata[actionid].data[1] <= 0 && hpPercent <= 0.65f && battle.enemydata[actionid].data[3] > 0)
            {
                chances.AddRange(new Attacks[] { Attacks.AttackingItem, Attacks.AttackingItem });
            }

            if (battle.enemydata[actionid].data[1] > 0)
            {
                battle.enemydata[actionid].data[1]--;
            }

            if (leviIndex > -1 && battle.enemydata[leviIndex].data != null && battle.enemydata[actionid].data[2] != 1)
            {
                if (Instance.CanRelay(entity, battle))
                {
                    chances.Add(Attacks.Relay);
                }
            }

            switch (chances[UnityEngine.Random.Range(0, chances.Count)])
            {
                case Attacks.ShieldThrow:
                    yield return DoShieldThrow(entity, actionid);
                    break;
                case Attacks.MultiPunch:
                    yield return DoMultiPunch(entity, actionid);
                    break;

                case Attacks.HealingItem:
                    int lowHp = Instance.GetLowHPEnemy();
                    int randomEnemy = UnityEngine.Random.Range(0, battle.enemydata.Length);

                    yield return Instance.UseItem(entity, lowHp == -1 ? randomEnemy : lowHp, battle, healingItems[UnityEngine.Random.Range(0, healingItems.Length)]);
                    battle.enemydata[actionid].data[1] = 1;
                    battle.enemydata[actionid].data[3]--;
                    break;

                case Attacks.AttackingItem:
                    yield return Instance.UseItem(entity, actionid, battle, attackingItems[UnityEngine.Random.Range(0, attackingItems.Length)]);
                    battle.enemydata[actionid].data[1] = 1;
                    battle.enemydata[actionid].data[3]--;
                    break;

                case Attacks.BuffingItem:
                    yield return Instance.UseItem(entity, actionid, battle, buffingItems[UnityEngine.Random.Range(0, buffingItems.Length)]);
                    battle.enemydata[actionid].data[1] = 1;
                    battle.enemydata[actionid].data[3]--;
                    break;

                case Attacks.Relay:
                    battle.enemydata[leviIndex].data[2] = 1;
                    yield return battle.EnemyRelay(entity, BattleControl_Ext.Instance.FindRelayable(entity, battle));
                    yield break;
            }
            battle.enemydata[actionid].data[2] = 0;
        }


        IEnumerator DoShieldThrow(EntityControl entity, int actionid)
        {
            battle.nonphyscal = true;
            entity.MoveTowards(new Vector3(2.5f, 0f, -0.55f), 2f);
            while (entity.forcemove)
            {
                yield return null;
            }
            entity.flip = false;
            entity.animstate = 105;
            entity.spin = new Vector3(0f, 30f);
            MainManager.PlaySound("Woosh", -1, 1.2f, 1f, true);
            yield return EventControl.halfsec;
            MainManager.StopSound("Woosh");
            entity.spin = Vector3.zero;
            MainManager.PlaySound("Toss8");
            AudioSource sound = MainManager.PlaySound("Toss2", 3, 0.8f, 1f, true);

            Transform shield = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Objects/Shield")).transform;
            shield.GetComponent<SpriteRenderer>().materials = new Material[] { MainManager.spritematlit };
            shield.transform.parent = entity.transform;

            entity.animstate = 102;
            ParticleSystem.MainModule p = shield.GetComponentInChildren<ParticleSystem>().main;
            Vector3 startPos = entity.transform.position + new Vector3(0f, 1.25f, 0f);
            Vector3 targetPos = startPos + new Vector3(-15f, 0f);
            int hitTarget = 0;
            float a = 0f;
            float b = 40f;
            bool block = false;
            bool checkBlock = false;
            while (a <= b)
            {
                shield.position = Vector3.Lerp(startPos, targetPos, a / b);
                shield.localEulerAngles += Vector3.forward * 10f * MainManager.framestep;
                p.startRotationMultiplier = shield.localEulerAngles.z;

                if (hitTarget < battle.partypointer.Length && shield.position.x <= MainManager.instance.playerdata[battle.partypointer[hitTarget]].battleentity.transform.position.x)
                {
                    if (MainManager.instance.playerdata[battle.partypointer[hitTarget]].hp > 0)
                    {
                        if (!checkBlock)
                        {
                            block = battle.commandsuccess;
                            checkBlock = true;
                        }

                        battle.DoDamage(actionid, battle.partypointer[hitTarget], SHIELD_DAMAGE, null, block);
                    }
                    hitTarget++;
                }
                if(sound != null)
                {
                    sound.volume = Mathf.Lerp(1f, 0f, a / b) * MainManager.soundvolume;
                }
                yield return null;
                a += MainManager.framestep;
            }
            yield return EventControl.halfsec;

            int[] targets = battle.partypointer.Reverse().ToArray();
            checkBlock = false;
            hitTarget = 0;
            a = 0f;
            b = 40f;
            do
            {
                shield.position = Vector3.Lerp(targetPos, startPos, a / b) + Vector3.up * 1.5f;
                shield.localEulerAngles += Vector3.forward * (-10f * MainManager.framestep);
                p.startRotationMultiplier = shield.localEulerAngles.z;
                if (hitTarget < targets.Length && shield.position.x >= MainManager.instance.playerdata[targets[hitTarget]].battleentity.transform.position.x)
                {
                    if (MainManager.instance.playerdata[targets[hitTarget]].hp > 0)
                    {
                        if (!checkBlock)
                        {
                            block = battle.commandsuccess;
                            checkBlock = true;
                        }
                        battle.DoDamage(actionid, targets[hitTarget], SHIELD_DAMAGE, null, block);
                    }
                    hitTarget++;
                }
                if(sound != null)
                    sound.volume = Mathf.Lerp(0f, 1f, a / b) * MainManager.soundvolume;
                yield return null;
                a += MainManager.framestep;
            } while (a < b);

            MainManager.StopSound(3);
            MainManager.PlaySound("Ding2");
            UnityEngine.Object.Destroy(shield.gameObject);
            entity.animstate = 104;
            yield return entity.SlowSpinStop(Vector3.up * -50f,100f);
            entity.spin = Vector3.zero;
        }

        IEnumerator DoMultiPunch(EntityControl entity, int actionid)
        {
            battle.GetSingleTarget();
            battle.CameraFocusTarget();

            entity.MoveTowards(battle.playertargetentity.transform.position + new Vector3(1.5f, 0f, -0.1f), 2f, 101, 101);
            while (entity.forcemove)
            {
                yield return null;
            }

            yield return EventControl.tenthsec;
            int punchs = 2;
            for(int i = 0; i < punchs; i++)
            {
                entity.animstate = 102;
                battle.DoDamage(actionid, battle.playertargetID, PUNCH_DAMAGE, null, battle.commandsuccess);
                yield return EventControl.tenthsec;
                entity.animstate = 103;
                yield return EventControl.thirdsec;
                yield return EventControl.tenthsec;
            }
            BattleControl.SetDefaultCamera();
        }
    }
}
