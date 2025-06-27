using BFPlus.Patches.DoActionPatches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.U2D;
using static BattleControl;
using static MainManager;
namespace BFPlus.Extensions.EnemyAI
{
    //Needle Charge.
    //He'd charge at a single target and ram into them with his nail like it's a spear (Think Gus from TTYD).
    //This attack would deal 3 / 4 damage and inflict permanent attack or defense down.This would also grant
    //Patton an ATK/DEF potion depending on what status was inflicted. If Patton uses this move on a target who has already had both
    //their ATK and DEF lowered, he's extract an MP potion from the target and steal a turn from them.

    //After extracting the potion and returning to his starting position, Patton would use the ATK/DEF/MP potion right away,
    //before this action is considered to be over. Using an ATK/DEF potion would grant the target the infinite ATK/DEF up status,
    //and the MP potion would grant +1 Hustle.Patton would also prioritize buffing his minions over himself,
    //and he wouldn't give someone a buff of a type that they already have.

    //Heart Extract.
    //He'd walk up to a singe target and then jab his needle into them. This would be a 2-hit attack that first just deals 1/2 damage,
    //but the second hit would deal 2/2 damage and permanently lower the bug's Max HP by 2 for the rest of the battle.Patton would also gain 2 small HP potions from this.
    //(See Delilah's succ attack as a reference for the timing)

    //Potion Jumble. 
    //He'd walk up in front of Team Snek and start mixing potions. He'd pull out 2 potions and start mixing them (he has the anim).
    //orange mist would start flying outward from Patton in both directions(See Bloatshroom attack), and unless you mash quickly enough Team Snek will get
    //their HP jumbled up. (They'll swap max HP temporarily, would return normal only after the fight.)
    //Regardless of if you mash or not, Patton's entire team recovers 5 HP, and this attack would deal 2/3 damage to all of
    //Team Snek that can be lowered by 1 by mashing.

    //Sleep Bomb
    //Throws a Sleep Bomb, but he also has the Bad Dreams effect

    //Point Swap
    //would only use this if his (or a minion's) current HP is below 50 % and is lower than the Party's current TP (unlimited uses)

    //HP Potion usage

    //HP potion:
    //At the start of his turn he'll check if he should use a HP potion. If he has at least 2 HP potions and if an himself or an ally
    //is below 50% HP, he'll have a 50% chance to use that potion on them.If either of his minions are defeated, he'll always use an
    //HP potion on the dead ally, and he can even use 2 a the same time if he has 2 potions, provided both minions are dead.

    // The HP potion would restore 7 HP to the target whilst curing any bad status effect. If the minion is dead, Patton would instead
    // throw the HP potion at the spot where the minion stood, and "revive it" with 7 HP (it would actually spawn in a new one with 5 HP,
    // with the same buffs as the previous minion)


    //Data Structure 
    //data[0] : hp potion amount
    //data[1] : abomiberry atk Up
    //data[2] : abomiberry defUp
    //data[3] : spider atk Up
    //data[4] : spider defUp

    public class PattonAI : AI
    {
        enum Attacks
        {
            NeedleCharge,
            HeartExtract,
            PotionJumble,
            PointSwap,
            SleepBomb
        }
        BattleControl battle = null;
        int NEEDLE_CHARGE_DMG = 4;
        int EXTRACT_FIRST_HIT_DMG = 3;
        int EXTRACT_SECOND_HIT_DMG = 3;
        int POTION_JUMBLE_DMG = 3;
        int HEALING_POTION_AMOUNT = 7;
        public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
        {
            battle = MainManager.battle;
            if (battle.enemydata[actionid].data == null)
            {
                battle.SetData(actionid, 7);
                battle.enemydata[actionid].data[6] = 2;
            }

            if(battle.enemydata.Length <3 && battle.enemydata[actionid].data[0] > 0)
            {
                int[] summonIds = { (int)NewEnemies.Abomiberry, (int)NewEnemies.LonglegsSpider };
                var possibleRevive = summonIds.Where(id => !battle.enemydata.Any(enemy => enemy.animid == id)).ToList();

                if(possibleRevive.Count != 0)
                    yield return ReviveEnemy(entity, actionid, possibleRevive);
            }
            else if(battle.enemydata[actionid].data[0] >= 2)
            {
                yield return CheckHealing(entity,actionid);
            }

            Dictionary<Attacks, int> attacks = new Dictionary<Attacks, int>()
            {
                { Attacks.NeedleCharge, 35},
                { Attacks.PotionJumble, 15},
                { Attacks.HeartExtract, 35},
                { Attacks.SleepBomb, 15}
            };

            List<int> possiblePointSwap = new List<int>();
            for(int i = 0; i < battle.enemydata.Length; i++)
            {
                if(battle.HPPercent(battle.enemydata[i]) < 0.5f && battle.enemydata[i].hp < MainManager.instance.tp)
                {
                    possiblePointSwap.Add(i);
                }
            }

            if(possiblePointSwap.Count != 0 && battle.enemydata[actionid].data[6] >0)
            {
                attacks.Add(Attacks.PointSwap, 20);
            }

            if (battle.enemydata[actionid].data[0] <= 0)
                attacks[Attacks.HeartExtract] += 15;

            Attacks attack = MainManager_Ext.GetWeightedResult(attacks);

            if (battle.enemydata.Length == 1 && battle.enemydata[actionid].data[0] <= 0)
                attack = Attacks.HeartExtract;

            switch (attack)
            {
                case Attacks.NeedleCharge:
                    yield return DoNeedleCharge(entity, actionid);
                    break;

                case Attacks.HeartExtract:
                    yield return DoHeartExtract(entity, actionid);
                    break;

                case Attacks.PotionJumble:
                    yield return DoPotionJumble(entity, actionid);
                    break;

                case Attacks.SleepBomb:
                    yield return BattleControl_Ext.Instance.UseItem(entity, actionid, battle, MainManager.Items.SleepBomb);
                    break;

                case Attacks.PointSwap:
                    int targetId = actionid;

                    for(int i = 0; i < possiblePointSwap.Count; i++)
                    {
                        if (battle.enemydata[possiblePointSwap[i]].hp < battle.enemydata[targetId].hp)
                            targetId = possiblePointSwap[i];
                    }
                    battle.enemydata[actionid].data[6]--;
                    yield return BattleControl_Ext.Instance.UseItem(entity, targetId, battle, (MainManager.Items)NewItem.PointSwap);
                    break;
            }
        }

        IEnumerator CheckHealing(EntityControl entity, int actionid)
        {
            if (battle.enemydata[actionid].data[5] == 1)
            {
                battle.enemydata[actionid].data[5] = 0;
                yield break;
            }

            var possibleHealingEnemies = battle.enemydata.Select((e,index)=>new {Enemy=e,Index=index}).Where(e => battle.HPPercent(e.Enemy) < 0.5f).Select(e => e.Index).ToArray();
            if (possibleHealingEnemies.Length > 0)
            {
                yield return ShowPotion(entity, actionid, MainManager.itemsprites[0, (int)MainManager.Items.HPPotion]);
                int targetid = possibleHealingEnemies[UnityEngine.Random.Range(0, possibleHealingEnemies.Length)];
                battle.Heal(ref battle.enemydata[targetid], HEALING_POTION_AMOUNT,false);
                battle.ClearStatus(ref battle.enemydata[targetid]);
                MainManager.PlaySound("Heal3");
                MainManager.PlayParticle("MagicUp", battle.enemydata[targetid].battleentity.transform.position);
                yield return EventControl.halfsec;
                battle.enemydata[actionid].data[0]--;
                battle.enemydata[actionid].data[5] = 1;
            }
        }

        IEnumerator ReviveEnemy(EntityControl entity, int actionid, List<int> possibleRevive)
        {
            int hpPotsAmount = battle.enemydata[actionid].data[0];

            for (int i = 0; i < hpPotsAmount; i++)
            {
                battle.enemydata[actionid].data[0]--;

                int reviveId = UnityEngine.Random.Range(0, possibleRevive.Count);
                MainManager.PlaySound("ItemHold");
                entity.animstate = (int)MainManager.Animations.ItemGet;
                SpriteRenderer potion = MainManager.NewSpriteObject(entity.transform.position + battle.enemydata[actionid].itemoffset, battle.battlemap.transform, MainManager.itemsprites[0, (int)MainManager.Items.HPPotion]);
                yield return EventControl.halfsec;
                entity.animstate = (int)MainManager.Animations.TossItem;
                MainManager.PlaySound("Toss12",1.1f,1);

                Vector3 battlePos = new Vector3(1, 0, 0);

                if (possibleRevive[reviveId] == (int)NewEnemies.LonglegsSpider)
                {
                    battlePos = new Vector3(3.5f, 0, 0.15f);
                }

                yield return MainManager.ArcMovement(potion.gameObject, potion.transform.position, battlePos, new Vector3(0, 0, 20), 5, 20, true);
                entity.animstate = 0;

                yield return battle.SummonEnemy(BattleControl.SummonType.FromGroundKeepScale, possibleRevive[reviveId], battlePos, true);
                possibleRevive.RemoveAt(reviveId);
                yield return EventControl.halfsec;
                int dataId = 1;
                battle.enemydata[battle.lastaddedid].exp = 0;

                if (battle.enemydata[battle.lastaddedid].animid == (int)NewEnemies.LonglegsSpider)
                    dataId += 2;

                BattleCondition[] conditions = { BattleCondition.AttackUp, BattleCondition.DefenseUp };

                for (int j = 0; j < conditions.Length; j++)
                {
                    if (battle.enemydata[actionid].data[dataId+j] == 1)
                    {
                        MainManager.SetCondition(conditions[j], ref battle.enemydata[battle.lastaddedid], 9999999);
                    }
                }

                if (possibleRevive.Count == 0)
                    break;
            }
        }

        IEnumerator DoPotionJumble(EntityControl entity, int actionid)
        {
            battle.nonphyscal = true;
            entity.MoveTowards(Vector3.zero);
            yield return new WaitUntil(() => !entity.forcemove);

            entity.animstate = 102;
            MainManager.PlaySound("Toss13", -1, 0.8f, 1f);
            yield return EventControl.sec;
            yield return EventControl.halfsec;

            GameObject sporeParticles = MainManager.PlayParticle("Spores", entity.transform.position + Vector3.up, -1f);
            var main = sporeParticles.GetComponent<ParticleSystem>().main;
            main.startColor = new Color(1f, 0.75f, 0.8f);
            MainManager.PlaySound("Mist");
            BattleControl.SetDefaultCamera();

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

            bool commandSuccess = battle.barfill >= 1f;

            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.instance.playerdata[i].hp > 0)
                {
                    battle.DoDamage(actionid, i, POTION_JUMBLE_DMG, null, commandSuccess);
                    MainManager.instance.playerdata[i].battleentity.spin = Vector3.zero;
                }
            }

            for(int i = 0; i < battle.enemydata.Length; i++)
            {
                battle.Heal(ref battle.enemydata[i], 3);
            }

            battle.DestroyHelpBox();
            //UnityEngine.Object.Destroy(spriteBounce);

            int[] indices = Enumerable.Range(0, MainManager.instance.playerdata.Length).ToArray();
            int[] result;
            do
            {
                result = indices
                    .OrderBy(_ => UnityEngine.Random.value)
                    .ToArray();
            }
            while (Enumerable.Range(0, MainManager.instance.playerdata.Length).Any(i => result[i] == i));

            var shuffledHp = MainManager.instance.playerdata
            .Select(o => o.maxhp)
            .ToArray();

            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                MainManager.instance.playerdata[i].maxhp = shuffledHp[result[i]];
                MainManager.instance.playerdata[i].hp = Mathf.Clamp(MainManager.instance.playerdata[i].hp, 0, MainManager.instance.playerdata[i].maxhp);
            }

            MainManager.DestroyTemp(sporeParticles, 10f);
            yield return EventControl.halfsec;
        }

        IEnumerator DoHeartExtract(EntityControl entity, int actionid)
        {
            Vector3 basePos = entity.transform.position;
            battle.GetSingleTarget();
            Vector3 playerPos = battle.playertargetentity.transform.position;

            battle.CameraFocusTarget();
            entity.MoveTowards(playerPos + Vector3.right*2f);
            yield return new WaitUntil(() => !entity.forcemove);

            entity.animstate = 104;
            yield return EventControl.halfsec;
            MainManager.PlaySound("Toss13", -1, 0.8f, 1f);
            entity.StartCoroutine(entity.ShakeSprite(0.2f, 60));
            yield return EventControl.sec;

            entity.MoveTowards(playerPos +new Vector3(1.3f,0,0.05f),2,106,107);
            yield return new WaitUntil(() => !entity.forcemove);
            battle.DoDamage(actionid, battle.playertargetID, EXTRACT_FIRST_HIT_DMG, null, battle.commandsuccess);
            SpriteBounce bounce = entity.sprite.gameObject.AddComponent<SpriteBounce>();
            entity.overrideflip = true;
            bounce.MessageBounce(1f);
            bounce.speed = 15f;
            for (int i = 0; i < 3; i++)
            {
                MainManager.PlaySound("Kiss",0.9f, 1);
                yield return EventControl.halfsec;
            }

            battle.DoDamage(actionid, battle.playertargetID, EXTRACT_SECOND_HIT_DMG, null, battle.commandsuccess);
            MainManager.PlaySound(MainManager_Ext.assetBundle.LoadAsset<AudioClip>("HealthSucked"));

            Rigidbody[] hearts = new Rigidbody[2];
            for (int i = 0; i < hearts.Length; i++)
            {
                hearts[i] = new GameObject().AddComponent<Rigidbody>();
                hearts[i].gameObject.AddComponent<SpriteRenderer>().sprite = MainManager.guisprites[24];
                hearts[i].transform.parent = battle.battlemap.transform;
                hearts[i].transform.position = battle.playertargetentity.transform.position;
                hearts[i].transform.localScale = Vector3.one * 0.5f;
                hearts[i].velocity = MainManager.RandomItemBounce(4f, 15f);
                yield return null;
            }

            int maxHp = MainManager.instance.playerdata[battle.playertargetID].maxhp;
            MainManager.instance.playerdata[battle.playertargetID].maxhp = Mathf.Clamp(maxHp-2,1,999);
            UnityEngine.Object.Destroy(bounce);
            BattleControl.SetDefaultCamera();

            entity.animstate = 109;
            yield return EventControl.sec;
            battle.enemydata[actionid].data[0] += 2;

            for (int i = 0; i < hearts.Length; i++)
                UnityEngine.Object.Destroy(hearts[i].gameObject);
        }
        IEnumerator DoNeedleCharge(EntityControl entity, int actionid)
        {
            Vector3 basePos = entity.transform.position;
            battle.GetSingleTarget();

            entity.animstate = 100;
            MainManager.PlaySound("BagRustle");
            yield return EventControl.halfsec;
            entity.animstate = 101;
            yield return EventControl.tenthsec;
            MainManager.PlaySound("Toss13", -1, 0.8f, 1f);

            yield return EventControl.sec;
            entity.animstate = 103;

            ParticleSystem walkDust = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/WalkDust")) as GameObject).GetComponent<ParticleSystem>();
            walkDust.transform.parent = entity.sprite.transform;
            walkDust.transform.localPosition = new Vector3(0f, 0.2f, -0.1f);

            MainManager.PlaySound("BeetleDash", 0.8f, 1);
            entity.StartCoroutine(entity.ShakeSprite(0.2f, 60));
            yield return EventControl.sec;
            entity.LockRigid(true);
            MainManager.PlaySound("Woosh4");
            entity.trail = true;
            yield return BattleControl_Ext.LerpPosition(20, entity.transform.position, battle.playertargetentity.transform.position, entity.transform);

            battle.DoDamage(actionid, battle.playertargetID, NEEDLE_CHARGE_DMG, null, battle.commandsuccess);

            MainManager.StopSound("BeetleDash");
            UnityEngine.Object.Destroy(walkDust.gameObject);

            BattleCondition[] properties = new BattleCondition[] { BattleCondition.AttackDown, BattleCondition.DefenseDown };
            List<BattleCondition> conditions = new List<BattleCondition>(properties);

            int itemSprite = (int)MainManager.Items.MPPotion;
            for (int i = 0; i < properties.Length; i++)
            {
                if (MainManager.HasCondition(properties[i], MainManager.instance.playerdata[battle.playertargetID]) != -1)
                {
                    conditions.Remove(properties[i]);
                }
            }
            BattleCondition? status = null;
            if (conditions.Count > 0)
            {
                status = conditions[UnityEngine.Random.Range(0, conditions.Count)];
                MainManager.battle.StatusEffect(MainManager.instance.playerdata[battle.playertargetID], status.Value, 9999999, true, false);
                itemSprite = status == BattleCondition.AttackDown ? (int)MainManager.Items.ATKPotion : (int)MainManager.Items.DEFPotion;
            }

            yield return MainManager.ArcMovement(entity.gameObject, entity.transform.position, entity.transform.position + Vector3.right * 3, Vector3.zero, 5, 20, false);
            entity.LockRigid(false);
            yield return null;

            entity.trail = false;
            entity.MoveTowards(basePos, 2);
            yield return new WaitUntil(() => !entity.forcemove);
            entity.flip = false;
            yield return ShowPotion(entity, actionid, MainManager.itemsprites[0, itemSprite]);

            if (itemSprite == (int)MainManager.Items.MPPotion)
            {
                Dictionary<int, int> weights = new Dictionary<int, int>();

                for (int i = 0; i < battle.enemydata.Length; i++)
                {
                    weights.Add(i, i == actionid ? 2 : 4);
                }

                int enemySelected = MainManager_Ext.GetWeightedResult(weights);
                MainManager.PlaySound("Heal3");
                battle.StartCoroutine(battle.StatEffect(battle.enemydata[enemySelected].battleentity, 5));
                battle.enemydata[enemySelected].moreturnnextturn++;
                MainManager.instance.playerdata[battle.playertargetID].cantmove++;
            }
            else
            {
                BattleCondition condition = status.Value == BattleCondition.AttackDown ? BattleCondition.AttackUp : BattleCondition.DefenseUp;

                int[] possibleBuffs = battle.enemydata
                    .Select((e, i) => (e, i))
                    .Where(x => MainManager.HasCondition(condition, x.e) == -1)
                    .Select(x => x.i)
                    .ToArray();

                if (possibleBuffs.Length != 0)
                {
                    Dictionary<int, int> weights = new Dictionary<int, int>();
                    for (int i = 0; i < possibleBuffs.Length; i++)
                    {
                        weights.Add(possibleBuffs[i], possibleBuffs[i] == actionid ? 2 : 4);
                    }

                    int enemySelected = MainManager_Ext.GetWeightedResult(weights);
                    MainManager.battle.StatusEffect(battle.enemydata[enemySelected], condition, 9999999, true, false);

                    if (battle.enemydata[enemySelected].animid != (int)NewEnemies.Patton)
                    {
                        int dataId = condition == BattleCondition.AttackUp ? 1 : 2;
                        if(battle.enemydata[enemySelected].animid == (int)NewEnemies.LonglegsSpider)
                        {
                            dataId += 2;
                        }
                        battle.enemydata[actionid].data[dataId] = 1;
                    }
                }

            }
        }


        IEnumerator ShowPotion(EntityControl entity, int actionid, Sprite itemSprite)
        {
            MainManager.PlaySound("ItemHold");
            entity.animstate = (int)MainManager.Animations.ItemGet;
            SpriteRenderer potion = MainManager.NewSpriteObject(entity.transform.position + battle.enemydata[actionid].itemoffset, battle.battlemap.transform, itemSprite);
            yield return EventControl.sec;
            UnityEngine.Object.Destroy(potion.gameObject);
        }
    }
}
