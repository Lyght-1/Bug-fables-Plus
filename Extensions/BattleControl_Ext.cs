using BFPlus.Extensions.EnemyAI;
using BFPlus.Extensions.Stylish;
using BFPlus.Patches.EventControlTranspilers;
using InputIOManager;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using static BattleControl;
using static MainManager;

namespace BFPlus.Extensions
{
    public enum StylishReward
    {
        None,
        HPRegen,
        TPRegen,
        Berries,
        Buff,
        Debuff
    }

    public enum DelProjType
    {
        StickyBomb,
        InkTrap
    }

    public enum NewEventDialogue
    {
        MarsDeath = 20,
        JesterSpitout,
        PattonDeath,
        StylishTutorial
    }
    public class BattleControl_Ext : MonoBehaviour
    {
        public bool InVengeance = false;
        public List<int> leifSkillIds = new List<int>() { -1, 4, 21, 25, 27, 26, 31, 17, 7, 8, 22, 14, 23, 15, 30, 28, 29, 12, 13, (int)NewSkill.VitiationLite, (int)NewSkill.Vitiation, (int)NewSkill.CordycepsLeech };
        public List<int> leifBuffSkillIds = new List<int>() { 17, 7, 47, 8, 22, 14, 23, 15, 30, 28, 29, 12, 13, 54, 55 };
        public EntityControl entityAttacking;
        public static int actionID;
        public bool destroyedList = false;
        public int damageDeepCleanse = 0;
        public int tpRegenCleanse = 0;
        public const int DAMAGE_DEEPCLEANSE = 3;
        public const int TP_REGEN_CLEANSE = 3;
        public bool firstHitMulti = false;
        public int holoSkillID = -1;
        public List<int> attackedThisTurn = new List<int>();
        public bool revengarangIsActive = false;
        public int revengarangDMG = 0;
        public bool perfectKill = false;
        public int perfectKillAmount = 0;
        static int perfectKillTotal = 0;
        public int loomLegProgress = 0;
        int oldAnimID = -1;
        public int startState = -1;
        int rockyRampUpDmg = 0;
        bool usedPebbleToss = false;
        public bool twinedFateUsed = false;
        public bool spuderStickyBubble = false;
        public bool spinelingFlipped = false;
        public bool inEndOfTurnDamage = false;
        List<StrikeBlaster> strikeBlasters = new List<StrikeBlaster>();
        Coroutine strikeBlasterManager = null;
        public int trustFallTurn = -1;
        public int trustFallDamage = 0;
        public static bool enemyUsedItem = false;
        public bool inStylish = false;
        bool failedStylish = false;
        public static float startStylishAmount = 0;
        public static float stylishBarAmount = 0;
        public static StylishReward startStylishReward = StylishReward.None;
        SpriteRenderer stylishBarHolder = null;
        SpriteRenderer stylishBar = null;
        public static StylishReward stylishReward = StylishReward.None;
        SpriteRenderer rewardIcon = null;

        public bool inAiAttack = false;
        const int BASE_CORYCEPSLEECH_DMG = 8;
        public int gourmetItemUse = -1;
        List<(DelayedProjectileData data, DelProjType type)> delProjsPlayer = new List<(DelayedProjectileData, DelProjType)>();
        List<(int data, DelayedProjExtra extra)> delProjsExtras = new List<(int, DelayedProjExtra)>();
        public bool targetIsPlayer;
        static BattleControl_Ext instance = null;
        List<Entity_Ext> entity_Exts = new List<Entity_Ext>();
        public DelayedProjExtra currentDelayedProj = null;
        public int mothFlowerHits = 0;
        public bool inStylishTutorial = false;
        public static BattleControl_Ext Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.battle.gameObject.AddComponent<BattleControl_Ext>();
                }
                return instance;
            }
        }

        void Start()
        {
            CreateStylishBar();
        }

        void Update()
        {

            /*if (Input.GetKey(KeyCode.S))
            {
                battle.enemy = true;
            }*/

            if (VengeanceCondition)
            {
                for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                {
                    if (CheckVengeance(i) && MainManager.instance.playerdata[i].charge < MainManager_Ext.CheckMaxCharge(i))
                    {
                        InVengeance = true;
                        StartCoroutine(DoVengeance(i));
                    }
                }
            }

            if (!VengeanceCondition)
                InVengeance = false;
        }
        public void ResetStuff()
        {
            perfectKillTotal = 0;
            BattleControl_Ext.enemyUsedItem = false;
            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                var entity = MainManager.instance.playerdata[i].battleentity;
                if (entity != null && entity.bubbleshield != null && MainManager.BadgeIsEquipped((int)Medal.ViolentVitiation))
                {
                    Destroy(entity.bubbleshield.gameObject);
                    entity.bubbleshield = null;
                }
            }

            var entitiesExt = FindObjectsOfType<Entity_Ext>();
            for (int i = 0; i != entitiesExt.Length; i++)
            {
                Destroy(entitiesExt[i]);
            }

            if (stylishBarHolder != null)
                Destroy(stylishBarHolder.gameObject);

            Destroy(this);
        }

        public IEnumerator GetSkillList(int actionID)
        {
            yield return new WaitUntil(() => destroyedList);
            destroyedList = false;
            int playerId = 0;
            if (actionID == 50)
                playerId = 1;
            else if (actionID == 51)
                playerId = 2;
            else if (actionID == 52)
                playerId = 3;
            holoSkillID = playerId - 1;
            MainManager.battle.currentaction = BattleControl.Pick.SkillList;
            MainManager.RefreshSkills();
            MainManager.SetUpList(-playerId, true, false);
            MainManager.listammount = 5;
            MainManager.ShowItemList(-playerId, MainManager.defaultlistpos, true, false);
        }
        public delegate int DoDamageDelegateNoBlock(ref MainManager.BattleData target, int amount, BattleControl.AttackProperty? property);

        IEnumerator DoLifeLust(BattleControl __instance, MainManager.BattleData player, Entity_Ext entityExt)
        {
            battle.GetAvaliableTargets(false, false, -1, true);
            var targets = battle.avaliabletargets;

            targets = targets.Where(e => e.position != BattleControl.BattlePosition.Underground || e.position != BattleControl.BattlePosition.OutOfReach).ToArray();
            if (targets.Length > 0)
            {
                int healedThisTurn = entityExt.healedThisTurn;
                entityExt.healedThisTurn = 0;
                int rest = healedThisTurn % targets.Length;
                int result = healedThisTurn / targets.Length;
                var enemyMiddle = targets.Sum(a => a.battleentity.transform.position.x) / (float)targets.Length;
                var middlePoint = new Vector3(enemyMiddle, 5);

                CreateBeam(player.battleentity.transform.position, middlePoint, player.battleentity.transform);
                __instance.StartCoroutine(FadeImage(middlePoint, 90f));
                yield return null;


                for (int i = 0; i != targets.Length; i++)
                {
                    int damageAmount = result;

                    if (rest > 0)
                    {
                        damageAmount++;
                        rest--;
                    }

                    if (damageAmount > 0)
                    {
                        var enemy = targets[i];
                        CreateBeam(enemy.battleentity.sprite.transform.position, middlePoint, enemy.battleentity.transform);
                        battle.DoDamage(null, ref __instance.enemydata[targets[i].battleentity.battleid], damageAmount, BattleControl.AttackProperty.Pierce, new DamageOverride[] { DamageOverride.NoFall }, false);
                    }
                }
                yield return EventControl.halfsec;
            }
        }

        void CreateBeam(Vector3 startPos, Vector3 endPos, Transform parent)
        {
            var go = Instantiate(Resources.Load("Prefabs/Particles/Heal")) as GameObject;
            var beam = go.GetComponent<ParticleSystem>();

            var main = beam.main;
            main.startSize = 0.5f;

            var col = beam.colorOverLifetime;
            col.enabled = true;

            Gradient grad = new Gradient();
            grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.red, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(0.5f, 0.0f), new GradientAlphaKey(1f, 1.0f) });

            col.color = grad;
            main.startRotation = 0;
            go.transform.position = (endPos + startPos) / 2;
            var distance = Vector3.Distance(endPos, startPos);

            ParticleSystem.ShapeModule sm = beam.shape;
            sm.rotation = new Vector3(0, 90f);
            beam.transform.LookAt(endPos);
            sm.shapeType = ParticleSystemShapeType.SingleSidedEdge;
            sm.radiusMode = ParticleSystemShapeMultiModeValue.BurstSpread;
            sm.radius = distance;

            ParticleSystem.EmissionModule em = beam.emission;
            int numParticles = (int)(distance * 100);
            ParticleSystem.Burst b = new ParticleSystem.Burst(0, numParticles);
            em.SetBurst(0, b);

            Destroy(go, 3f);
        }

        IEnumerator FadeImage(Vector3 position, float frameTime)
        {
            GameObject heart = new GameObject("heart");
            var spriteR = heart.AddComponent<SpriteRenderer>();
            spriteR.sprite = MainManager.itemsprites[1, (int)Medal.LifeLust];
            heart.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            spriteR.sortingOrder = 1;
            heart.transform.position = position;

            float a = 0f;
            Color ic = spriteR.material.color;
            do
            {
                var scale = Mathf.Lerp(spriteR.transform.localScale.x, 3f, a / frameTime);
                spriteR.transform.localScale = new Vector3(scale, scale, scale);
                spriteR.material.color = new Color(ic.r, ic.g, ic.b, Mathf.Lerp(ic.a, 0f, a / frameTime));
                a += MainManager.framestep;
                yield return null;
            }
            while (a < frameTime);

            Destroy(heart);
        }

        IEnumerator TeamEffortCheck()
        {
            var idRequired = new List<int>();

            foreach (var player in MainManager.instance.playerdata)
                idRequired.Add(player.battleentity.battleid);

            if (!idRequired.Except(attackedThisTurn).Any())
            {
                if (MainManager.BadgeIsEquipped((int)Medal.TeamEffort))
                {
                    battle.HealTP(2);
                    yield return EventControl.halfsec;
                }

                if (MainManager.BadgeIsEquipped((int)Medal.TeamCheer))
                {
                    for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                    {
                        if (MainManager.instance.playerdata[i].hp > 0)
                            battle.Heal(ref MainManager.instance.playerdata[i], 1, false);
                    }
                    yield return EventControl.halfsec;
                }
            }
            attackedThisTurn.Clear();
        }

        public void DoYawnCheck(MainManager.BattleData entity, MainManager.BattleCondition condition)
        {
            int yawnBug = BattleControl_Ext.GetEquippedMedalBug(Medal.Yawn);
            if (yawnBug != -1)
            {
                if (entity.battleentity.playerentity && entity.trueid == MainManager.instance.playerdata[yawnBug].trueid)
                {
                    return;
                }

                if (condition == MainManager.BattleCondition.Sleep && MainManager.HasCondition(MainManager.BattleCondition.Sturdy, MainManager.instance.playerdata[yawnBug]) == -1)
                    MainManager.SetCondition(MainManager.BattleCondition.Sleep, ref MainManager.instance.playerdata[yawnBug], 2);
            }
        }

        static bool DestinyDreamSetHP()
        {
            int destinyDreamBug = GetDestinyDreamBug();
            if (destinyDreamBug > -1)
            {
                MainManager.instance.playerdata[destinyDreamBug].hp -= Mathf.Abs(MainManager.instance.flagvar[0]);
                return true;
            }
            return false;
        }

        static bool DestinyDreamChangeHud()
        {
            int destinyDreamBug = GetDestinyDreamBug();
            if (destinyDreamBug > -1)
            {
                MainManager.hudsprites[destinyDreamBug].color = Color.red;
                return true;
            }
            return false;
        }

        IEnumerator DoVengeance(int index)
        {
            MainManager.instance.playerdata[index].charge = MainManager_Ext.CheckMaxCharge(index);
            battle.StartCoroutine(battle.StatEffect(MainManager.instance.playerdata[index].battleentity, 4));
            yield return EventControl.halfsec;
            MainManager.PlaySound("Wam");
            MainManager.PlaySound("StatUp", -1, 1.25f, 1f);
        }

        public static bool CheckVengeanceCharge() => Instance.InVengeance && !MainManager.battle.enemy && MainManager.battle.currentaction != BattleControl.Pick.ItemList;

        public static bool CheckVengeance(int index)
        {
            return VengeanceCondition && MainManager.instance.playerdata[index].hp > 0 && MainManager.HasCondition(MainManager.BattleCondition.Eaten, MainManager.instance.playerdata[index]) == -1 && MainManager.BadgeIsEquipped((int)Medal.Vengeance, MainManager.instance.playerdata[index].trueid);
        }

        public static bool DestinyDreamCheck()
        {
            return GetDestinyDreamBug() != -1;
        }

        public static int GetDestinyDreamBug()
        {
            return GetEquippedMedalBug(Medal.DestinyDream, (i) => MainManager.HasCondition(MainManager.BattleCondition.Sleep, MainManager.instance.playerdata[i]) > -1 && MainManager.instance.playerdata[i].hp > 0);
        }

        public static int GetEquippedMedalBug(Medal medal, Func<int, bool> condition)
        {
            for (int i = 0; i != MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.BadgeIsEquipped((int)medal, MainManager.instance.playerdata[i].trueid) && condition(i))
                {
                    return i;
                }
            }
            return -1;
        }

        public static int GetEquippedMedalBug(Medal medal)
        {
            for (int i = 0; i != MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.BadgeIsEquipped((int)medal, MainManager.instance.playerdata[i].trueid))
                {
                    return i;
                }
            }
            return -1;
        }

        public static void CreateDestinySkillSprite(SpriteRenderer parent)
        {
            int destinyDreamBug = GetDestinyDreamBug();
            if (destinyDreamBug != -1)
            {
                int guispritesID = MainManager.instance.playerdata[destinyDreamBug].trueid + 5;
                MainManager.NewUIObject("destinyDream", parent.transform, new Vector3(2.55f, 0f), new Vector3(0.45f, 0.5f, 1f) * 0.35f, MainManager.guisprites[guispritesID], 11).GetComponent<SpriteRenderer>();
            }
        }
        public void PotentialEnergyCheck(ref MainManager.BattleData player)
        {
            if (MainManager.HasCondition(MainManager.BattleCondition.Numb, player) > -1 && MainManager.BadgeIsEquipped((int)Medal.PotentialEnergy, player.trueid) && player.moreturnnextturn < 15)
            {
                player.moreturnnextturn += 1;
                MainManager.PlaySound("Heal3");
                battle.StartCoroutine(battle.StatEffect(player.battleentity, 5));
            }
        }

        public void CheckFlavorCharger(MainManager.ItemUsage type, int? characterid)
        {
            MainManager.ItemUsage[] usages = new MainManager.ItemUsage[]{
                MainManager.ItemUsage.GradualHP, MainManager.ItemUsage.GradualTP, MainManager.ItemUsage.HPorDamage,
                MainManager.ItemUsage.HPRecover, MainManager.ItemUsage.Revive, MainManager.ItemUsage.HPRecoverFull,
                MainManager.ItemUsage.HPto1,MainManager.ItemUsage.TPRecover,MainManager.ItemUsage.TPRecoverFull,
                MainManager.ItemUsage.ChargeUp,MainManager.ItemUsage.TPto1, (MainManager.ItemUsage)NewItemUse.AddInk,
                (MainManager.ItemUsage)NewItemUse.AddSticky,(MainManager.ItemUsage)NewItemUse.RandomBuff,
                (MainManager.ItemUsage)NewItemUse.RandomDebuff, (MainManager.ItemUsage)NewItemUse.AddSturdy,
                (MainManager.ItemUsage)NewItemUse.AddAtkDown, (MainManager.ItemUsage)NewItemUse.AddDefDown,
                (MainManager.ItemUsage)NewItemUse.AddTaunt, (MainManager.ItemUsage)NewItemUse.AddFire,
                MainManager.ItemUsage.AddPoison, MainManager.ItemUsage.AddFreeze, MainManager.ItemUsage.AddSleep,
                MainManager.ItemUsage.AddNumb, MainManager.ItemUsage.TurnNextTurn,
            };

            if (usages.Contains(type) && MainManager.BadgeIsEquipped((int)Medal.FlavorCharger, characterid.Value))
            {
                DoFlavorCharger(characterid);
            }
            else
            {
                usages = new MainManager.ItemUsage[]
                {
                    MainManager.ItemUsage.GradualHPParty, MainManager.ItemUsage.HPRecoverAll, MainManager.ItemUsage.HPto1All,
                    MainManager.ItemUsage.ReviveAll, (MainManager.ItemUsage)NewItemUse.AddInkParty, (MainManager.ItemUsage)NewItemUse.AddStickyParty,
                    (MainManager.ItemUsage)NewItemUse.RandomBuffParty,(MainManager.ItemUsage)NewItemUse.RandomDebuffParty
                };

                if (usages.Contains(type))
                {
                    for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                    {
                        if (MainManager.BadgeIsEquipped((int)Medal.FlavorCharger, i))
                        {
                            DoFlavorCharger(i);
                        }
                    }
                }
            }
        }

        public void DoFlavorCharger(int? characterid)
        {
            MainManager.PlaySound("StatUp", -1, 1.25f, 1f);
            battle.StartCoroutine(battle.StatEffect(MainManager.instance.playerdata[characterid.Value].battleentity, 4));
            MainManager.instance.playerdata[characterid.Value].charge = Mathf.Clamp(MainManager.instance.playerdata[characterid.Value].charge + 1, 0, MainManager_Ext.CheckMaxCharge(characterid.Value));
        }


        public static bool VengeanceCondition => MainManager.GetAlivePlayerAmmount() == 1 && MainManager.instance.playerdata.Length != 1;

        public void DoPoison(ref MainManager.BattleData etarget)
        {
            int poisonTurn = MainManager.HasCondition(MainManager.BattleCondition.Poison, etarget);
            battle.TryCondition(ref etarget, BattleCondition.Poison, 2);
            if (poisonTurn != MainManager.HasCondition(MainManager.BattleCondition.Poison, etarget))
            {
                MainManager.PlayParticle("poisoneffect", etarget.battleentity.transform.position + new Vector3(0, etarget.battleentity.height));
            }
        }

        static int CheckPerkfectItemDrop(int baseChance, EntityControl entity)
        {
            //int baseChance = !MainManager.BadgeIsEquipped(18) ? !MainManager.BadgeIsEquipped(11) && !MainManager.instance.flags[614] ? -3 : -1 : -7;
            int itemChance = Mathf.Clamp(MainManager.BadgeIsEquipped((int)Medal.Perkfectionist) ? baseChance + perfectKillTotal : baseChance, baseChance, 0);

            int[] seedlings = new int[]
            {
               (int)MainManager.Enemies.Seedling,
               (int)MainManager.Enemies.FlyingSeedling,
               (int)MainManager.Enemies.Acornling,
               (int)MainManager.Enemies.Cactus,
               (int)MainManager.Enemies.Flowering,
               (int)MainManager.Enemies.Underling,
               (int)MainManager.Enemies.GoldenSeedling,
               (int)NewEnemies.Caveling,
               (int)NewEnemies.FlyingCaveling,
               (int)MainManager.Enemies.Plumpling,
            };
            bool gotWhistle = false;
            foreach (int enemyDefeated in MainManager.instance.lastdefeated)
            {
                if (!gotWhistle && seedlings.Contains(enemyDefeated) && UnityEngine.Random.Range(0, 100) == 0)
                {
                    gotWhistle = true;
                    MainManager_Ext.CreateItemEntity((int)NewItem.SeedlingWhistle, entity, entity.spritetransform.position, 0);
                }
            }

            var npcExt = NPCControl_Ext.GetNPCControl_Ext(entity.npcdata);

            List<int> items = new List<int>();
            for (int i = 0; i < npcExt.items.Length; i++)
            {
                if (npcExt.items[i] != -1 && !npcExt.usedItem[i])
                    items.Add(npcExt.items[i]);
            }

            foreach (var item in items)
            {
                if (UnityEngine.Random.Range(0, 100) < 50 + 10 * perfectKillTotal)
                {
                    int itemID = items[UnityEngine.Random.Range(0, items.Count)];
                    MainManager_Ext.CreateItemEntity(itemID, entity, entity.spritetransform.position, 0);
                    break;
                }
            }
            perfectKillTotal = 0;

            return UnityEngine.Random.Range(itemChance, entity.npcdata.vectordata.Length);
        }

        static void CheckHoloSkill()
        {
            EntityControl entity = MainManager.instance.playerdata[MainManager.battle.currentturn].battleentity;
            if (Instance.holoSkillID != -1 && entity.CompareTag("Player"))
            {
                MainManager.battle.StartCoroutine(Instance.UseHoloSkill(entity, MainManager.battle.selecteditem));
            }
            else
            {
                MainManager.battle.StartCoroutine(battle.DoAction(entity, battle.selecteditem));
            }
        }

        IEnumerator UseHoloSkill(EntityControl entity, int actionid)
        {
            if (MainManager.battle.cancelupdate)
            {
                yield return null;
                yield break;
            }
            MainManager.battle.overridechallengeblock = false;
            MainManager.battle.CancelInvoke("UpdateAnim");
            battle.DestroyHelpBox();
            MainManager.battle.action = true;
            battle.UpdateText();
            battle.UpdateAnim();

            entity.animstate = 4;
            MainManager.PlaySound("ItemHold");
            SpriteRenderer itemSprite = new GameObject().AddComponent<SpriteRenderer>();
            itemSprite.transform.position = entity.transform.position + new Vector3(0f, 2.5f, -0.1f);
            itemSprite.sprite = MainManager.itemsprites[1, (int)Medal.HoloSkill];
            itemSprite.material.renderQueue = 50000;
            itemSprite.gameObject.layer = 14;
            yield return EventControl.halfsec;
            Destroy(itemSprite.gameObject);
            MainManager.PlaySound("Scanner1");
            entity.spin = new Vector3(0, 30, 0);
            yield return EventControl.halfsec;
            oldAnimID = entity.animid;
            entity.animid = holoSkillID;
            entity.hologram = true;
            entity.UpdateSpriteMat();

            yield return EventControl.quartersec;
            entity.spin = Vector3.zero;
            MainManager.battle.StartCoroutine(battle.DoAction(entity, actionid));
        }

        void SleepScheduleCheck()
        {
            for (int i = 0; i != MainManager.instance.playerdata.Length; i++)
            {
                var entityExt = Entity_Ext.GetEntity_Ext(MainManager.instance.playerdata[i].battleentity);
                if (entityExt.sleepScheduleTurns == 1 && entityExt.sleepScheduled)
                {
                    entityExt.sleepScheduled = false;
                    MainManager.PlaySound("Sleep");
                    MainManager.SetCondition(MainManager.BattleCondition.Sleep, ref MainManager.instance.playerdata[i], 3);
                }
                else
                {
                    entityExt.sleepScheduleTurns--;
                }

            }
        }

        IEnumerator DoSleepSchedule(EntityControl entity)
        {
            entity.animstate = 4;
            MainManager.PlaySound("ItemHold");
            SpriteRenderer itemSprite = new GameObject().AddComponent<SpriteRenderer>();
            itemSprite.transform.position = entity.transform.position + new Vector3(0f, 2.5f, -0.1f);
            itemSprite.sprite = MainManager.itemsprites[1, (int)Medal.SleepSchedule];
            itemSprite.material.renderQueue = 50000;
            itemSprite.gameObject.layer = 14;
            yield return EventControl.sec;
            Destroy(itemSprite.gameObject);
            MainManager.PlaySound("Sleep");
            MainManager.DeathSmoke(entity.transform.position);
            var entityExt = Entity_Ext.GetEntity_Ext(entity);
            entityExt.sleepScheduled = true;
            entityExt.sleepScheduleTurns = 1;
        }

        IEnumerator DoWildfire()
        {
            var targets = new List<MainManager.BattleData>();

            for (int i = 0; i != MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.instance.playerdata[i].hp > 0 && MainManager.HasCondition(MainManager.BattleCondition.Sturdy, MainManager.instance.playerdata[i]) == -1)
                {
                    targets.Add(MainManager.instance.playerdata[i]);
                }
            }
            targets.AddRange(MainManager.battle.enemydata);

            var randomTarget = targets[UnityEngine.Random.Range(0, targets.Count)];
            MainManager.PlaySound("Flame");
            MainManager.PlayParticle("Fire", randomTarget.battleentity.transform.position + new Vector3(0, randomTarget.battleentity.height), 1f);
            MainManager.SetCondition(MainManager.BattleCondition.Fire, ref randomTarget, 3);
            yield return EventControl.halfsec;
        }

        public static int DoHeatingUp(MainManager.BattleData target)
        {
            var damage = Mathf.Clamp(Mathf.CeilToInt(target.maxhp / 7.5f) - 1, 2, 3);
            if (MainManager.BadgeIsEquipped((int)Medal.HeatingUp))
            {
                var entityExt = Entity_Ext.GetEntity_Ext(target.battleentity);
                damage += entityExt.fireDamage;
                entityExt.fireDamage++;
            }

            if (MainManager.BadgeIsEquipped((int)Medal.FierySpirit))
            {
                MainManager.PlaySound("Heal2");
                MainManager.instance.tp = Mathf.Clamp(MainManager.instance.tp + 1, 0, MainManager.instance.maxtp);
                battle.ShowDamageCounter(2, 1, target.battleentity.transform.position + target.cursoroffset + Vector3.up, target.battleentity.transform.position + target.cursoroffset + Vector3.up * 2);
            }

            for (int i = 0; i < battle.enemydata.Length; i++)
            {
                if (battle.enemydata[i].animid == (int)NewEnemies.FirePopper)
                    battle.Heal(ref battle.enemydata[i], 2);
            }

            if (!target.battleentity.isplayer)
            {
                switch (target.animid)
                {
                    case (int)NewEnemies.FirePopper:
                        damage = 0;
                        break;

                    case (int)NewEnemies.FireAnt:
                        MainManager.PlaySound("Heal3");
                        battle.StartCoroutine(battle.StatEffect(battle.enemydata[target.battleentity.battleid].battleentity, 5));
                        battle.enemydata[target.battleentity.battleid].cantmove--; ;
                        break;
                }
            }


            int fieryHeartBug = GetEquippedMedalBug(Medal.FieryHeart);
            if (fieryHeartBug > -1 && MainManager.instance.playerdata[fieryHeartBug].hp > 0)
            {
                MainManager.battle.StartCoroutine(battle.ItemSpinAnim(MainManager.instance.playerdata[fieryHeartBug].battleentity.transform.position + Vector3.up, MainManager.itemsprites[1, (int)Medal.FieryHeart], true));
                battle.Heal(ref MainManager.instance.playerdata[fieryHeartBug], 1, false);
            }

            return damage;
        }

        IEnumerator DoPerkfectionist()
        {
            if (Instance.perfectKill)
            {
                Instance.perfectKill = false;
                for (int i = 0; i != MainManager.instance.playerdata.Length; i++)
                {
                    if (MainManager.instance.playerdata[i].hp > 0)
                        battle.Heal(ref MainManager.instance.playerdata[i], 1 * Instance.perfectKillAmount, false);
                }
                yield return EventControl.quartersec;
                MainManager.PlaySound("Heal2");
                var tpRegen = 2 * Instance.perfectKillAmount;
                MainManager.instance.tp = Mathf.Clamp(MainManager.instance.tp + tpRegen, 0, MainManager.instance.maxtp);
                battle.ShowDamageCounter(2, tpRegen, battle.partymiddle + Vector3.up, battle.partymiddle + Vector3.up * 2);
                yield return EventControl.quartersec;
                perfectKillTotal += Instance.perfectKillAmount;
            }
            Instance.perfectKillAmount = 0;
            Instance.perfectKill = false;
        }

        static IEnumerator CheckPerkfectionist()
        {
            if (stylishBarAmount >= 1f)
            {
                yield return MainManager.battle.StartCoroutine(Instance.DoStylishReward());
            }

            if (MainManager.BadgeIsEquipped((int)Medal.Perkfectionist))
            {
                yield return MainManager.battle.StartCoroutine(Instance.DoPerkfectionist());
            }

            if (MainManager.BadgeIsEquipped((int)Medal.StrikeBlaster))
            {
                yield return new WaitUntil(() => Instance.strikeBlasterManager == null);
                Instance.strikeBlasters.Clear();
            }
        }

        static void DoInkBlotEnemy()
        {
            if (MainManager.BadgeIsEquipped((int)Medal.Inkblot))
            {
                if (battle.enemydata.Length > 1)
                {
                    for (int i = 0; i < battle.enemydata.Length; i++)
                    {
                        if (battle.enemydata[i].hp <= 0)
                        {
                            for (int j = 0; j < battle.enemydata.Length; j++)
                            {
                                if (i != j && battle.enemydata[j].hp > 0 && battle.enemydata[j].position != BattlePosition.Underground)
                                {
                                    EntityControl targetEntity = battle.enemydata[j].battleentity;
                                    bool isClose = MainManager.GetSqrDistance(targetEntity.transform.position + targetEntity.freezeoffset + Vector3.up * targetEntity.height, battle.enemydata[i].battleentity.transform.position) <= 15.5f;

                                    if (isClose)
                                    {
                                        Vector3 particlePos = battle.enemydata[j].battleentity.transform.position + Vector3.up + battle.enemydata[j].battleentity.height * Vector3.up;
                                        Instance.ApplyStatus(BattleCondition.Inked, ref battle.enemydata[j], 2, "WaterSplash2", 0.8f, 1, "InkGet", particlePos, Vector3.one);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        static void DoInkBlotPlayer(int playerid)
        {
            if (MainManager.instance.playerdata[playerid].hp <= 0 && MainManager.BadgeIsEquipped((int)Medal.Inkblot))
            {
                for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                {
                    if (i != playerid && MainManager.instance.playerdata[i].hp > 0 && MainManager.HasCondition(MainManager.BattleCondition.Sturdy, MainManager.instance.playerdata[i]) == -1)
                    {
                        EntityControl targetEntity = MainManager.instance.playerdata[i].battleentity;
                        float distance = MainManager.GetSqrDistance(targetEntity.transform.position, MainManager.instance.playerdata[playerid].battleentity.transform.position);
                        bool isClose = distance <= 6f;

                        if (isClose)
                        {
                            Vector3 particlePos = MainManager.instance.playerdata[i].battleentity.transform.position + Vector3.up;
                            Instance.ApplyStatus(BattleCondition.Inked, ref MainManager.instance.playerdata[i], 2, "WaterSplash2", 0.8f, 1, "InkGet", particlePos, Vector3.one);
                        }
                    }
                }
            }
        }

        int CheckInkBlotAdjacent(int arrayLength, int index, ref bool condition, bool after)
        {
            int nextTarget = index - 1;
            condition = nextTarget >= 0;
            if (after)
            {
                nextTarget = index + 1;
                condition = nextTarget < arrayLength;
            }
            return nextTarget;
        }

        static bool CheckCryostatis(MainManager.BattleData target, int indexCondition)
        {
            MainManager.BattleCondition[] conditions = new MainManager.BattleCondition[] { MainManager.BattleCondition.Topple, BattleCondition.Eaten, BattleCondition.EventStop, BattleCondition.Flipped };

            if (!conditions.Contains((MainManager.BattleCondition)target.condition[indexCondition][0]))
            {
                for (int i = 0; i != MainManager.instance.playerdata.Length; i++)
                {
                    if (MainManager.HasCondition(MainManager.BattleCondition.Freeze, MainManager.instance.playerdata[i]) > -1 && MainManager.BadgeIsEquipped((int)Medal.Cryostatis, MainManager.instance.playerdata[i].trueid) && (!target.battleentity.playerentity || target.trueid != MainManager.instance.playerdata[i].trueid))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void CheckSweetDreams(MainManager.BattleData target)
        {
            var battleEntity = target.battleentity;

            if (battleEntity.CompareTag("Player") && MainManager.BadgeIsEquipped((int)Medal.SweetDreams, target.trueid))
            {
                var entityExt = Entity_Ext.GetEntity_Ext(target.battleentity);

                if (target.isasleep)
                {
                    entityExt.asleepTurns += 1;
                }
                else
                {
                    if (entityExt.asleepTurns != 0)
                    {
                        MainManager.instance.tp = Mathf.Clamp(MainManager.instance.tp + entityExt.asleepTurns * 2, 0, MainManager.instance.maxtp);
                        battle.ShowDamageCounter(2, entityExt.asleepTurns * 2, battleEntity.transform.position + target.cursoroffset, Vector3.up);
                        entityExt.asleepTurns = 0;
                        MainManager.PlaySound("Heal2");
                    }
                }
            }
        }

        static IEnumerator EndOfTurnCheck()
        {
            var battle = MainManager.battle;
            battle.action = true;
            BattleControl_Ext.Instance.entityAttacking = null;
            if (!battle.firststrike)
            {
                yield return new WaitUntil(() => battle.mainturn != null);
                yield return Instance.DoDelProjPlayer();

                battle.UpdateText();
                MainManager_Ext.adrenalineChecked = false;
                Instance.usedPebbleToss = false;
                Instance.twinedFateUsed = false;
                DarkTeamSnakemouth.RefreshRelay();
                Instance.SleepScheduleCheck();

                if (MainManager.BadgeIsEquipped((int)Medal.TeamEffort) || MainManager.BadgeIsEquipped((int)Medal.TeamCheer))
                    yield return battle.StartCoroutine(Instance.TeamEffortCheck());

                if (MainManager.BadgeIsEquipped((int)Medal.Wildfire) && MainManager.battle.enemydata.Length > 0)
                    yield return battle.StartCoroutine(Instance.DoWildfire());

                int turns = battle.turns;

                if (turns == Instance.trustFallTurn + 1 && Instance.trustFallTurn != -1)
                {
                    battle.HealTP(Instance.trustFallDamage);
                    Instance.trustFallTurn = -1;
                    Instance.trustFallDamage = 0;
                    yield return EventControl.halfsec;
                }
                Instance.inEndOfTurnDamage = true;

                int[] moves = new int[battle.enemydata.Length];
                for (int i = 0; i < battle.enemydata.Length; i++)
                {
                    moves[i] = battle.enemydata[i].cantmove;
                }

                if (MainManager.BadgeIsEquipped((int)Medal.Hailstorm) && Instance.CheckHailstorm())
                    yield return battle.StartCoroutine(Instance.DoHailStorm(false));

                for (int i = 0; i != MainManager.instance.playerdata.Length; i++)
                {
                    var entityExt = Entity_Ext.GetEntity_Ext(MainManager.instance.playerdata[i].battleentity);
                    entityExt.inkWellActive = false;
                    BattleControl_Ext.Instance.CheckHDWGHConditionAmount(MainManager.instance.playerdata[i], entityExt);
                    if (entityExt.smearchargeActive)
                    {
                        entityExt.smearchargeActive = false;
                        Instance.DoSmearcharge(ref MainManager.instance.playerdata[i]);
                        yield return EventControl.halfsec;
                    }

                    if (entityExt.healedThisTurn > 0 && MainManager.BadgeIsEquipped((int)Medal.LifeLust, MainManager.instance.playerdata[i].trueid) && MainManager.instance.playerdata[i].hp > 0)
                        yield return battle.StartCoroutine(Instance.DoLifeLust(battle, MainManager.instance.playerdata[i], entityExt));

                    if (MainManager.BadgeIsEquipped((int)Medal.Nightmare, MainManager.instance.playerdata[i].trueid) && MainManager.HasCondition(MainManager.BattleCondition.Sleep, MainManager.instance.playerdata[i]) > -1)
                        yield return battle.StartCoroutine(Instance.DoNightmare(battle, MainManager.instance.playerdata[i]));
                }

                for (int i = 0; i < battle.enemydata.Length; i++)
                {
                    battle.enemydata[i].cantmove = moves[i];
                }

                Instance.inEndOfTurnDamage = false;
                if (battle.AliveEnemies() > 0)
                {
                    yield return Instance.CheckReviveEnemies();
                }

                yield return battle.StartCoroutine(battle.CheckDead());

                for (int i = 0; i < battle.enemydata.Length; i++)
                {
                    if (battle.enemydata[i].animid == (int)NewEnemies.IronSuit)
                    {
                        yield return IronSuitAI.ChangeForm(battle.enemydata[i].battleentity, battle.enemydata[i].battleentity.GetComponent<IronSuit>(), i);
                    }
                }
            }
        }

        IEnumerator CheckReviveEnemies()
        {
            bool revived = false;
            for (int i = 0; i < battle.reservedata.Count; i++)
            {
                var data = battle.reservedata[i];
                data.turnssincedeath++;
                battle.reservedata[i] = data;
                if (battle.reservedata[i].animid == (int)NewEnemies.FirePopper && battle.reservedata[i].turnssincedeath >= 2)
                {
                    EntityControl firePopper = battle.reservedata[i].battleentity;
                    MainManager.PlaySound("Charge7", 0.9f, 1);
                    firePopper.StartCoroutine(firePopper.ShakeSprite(0.2f, 60f));
                    yield return EventControl.sec;
                    firePopper.overrideanim = true;
                    firePopper.animstate = 110;
                    MainManager.PlaySound("Boing1", 1f, 1);
                    battle.ReviveEnemy(i, 0.5f, false, true);
                    revived = true;
                    yield return EventControl.halfsec;
                    break;
                }
            }

            if (revived && battle.reservedata.Count > 0)
                yield return CheckReviveEnemies();
        }

        public void DoInkWellCheck(int damageDone, ref BattleData target)
        {
            if (!battle.enemy && battle.chompyattack == null && Instance.entityAttacking != null && !inAiAttack && !Instance.targetIsPlayer && battle.currentturn != -1 && MainManager.BadgeIsEquipped((int)Medal.Inkwell, MainManager.instance.playerdata[battle.currentturn].trueid) && MainManager.HasCondition(BattleCondition.Inked, target) > -1 && MainManager.instance.playerdata[battle.currentturn].hp > 0 && !Instance.inEndOfTurnDamage)
            {
                var entityExt = Entity_Ext.GetEntity_Ext(Instance.entityAttacking);

                if (!entityExt.inkWellActive)
                {
                    MainManager.battle.StartCoroutine(MainManager.battle.ItemSpinAnim(Instance.entityAttacking.transform.position + Vector3.up, MainManager.itemsprites[1, (int)Medal.Inkwell], true));
                    battle.Heal(ref MainManager.instance.playerdata[battle.currentturn], damageDone);
                    entityExt.inkWellActive = true;
                }
            }
        }

        public void DoWebsheetCheck(BattleData? attacker, ref BattleData target)
        {
            if (attacker != null && Instance.targetIsPlayer && MainManager.BadgeIsEquipped((int)Medal.WebSheet, target.trueid) && !battle.nonphyscal)
            {
                bool didAnim = false;
                if (MainManager.HasCondition(MainManager.BattleCondition.Sticky, attacker.Value) == -1)
                {
                    EntityControl enemyEntity = attacker.Value.battleentity;
                    BattleControl_Ext.Instance.ApplyStatus(BattleCondition.Sticky, ref battle.enemydata[enemyEntity.battleid], 2, "AhoneynationSpit", 1, 1, "StickyGet", enemyEntity.transform.position, Vector3.one);
                    MainManager.battle.StartCoroutine(MainManager.battle.ItemSpinAnim(target.battleentity.transform.position + Vector3.up, MainManager.itemsprites[1, (int)Medal.WebSheet], true));
                    didAnim = true;
                }

                var caller = battle.caller;
                if (caller != null && MainManager.instance.items[0].Count < MainManager.instance.maxitems)
                {
                    var entityExt = Entity_Ext.GetEntity_Ext(attacker.Value.battleentity);
                    int enemyID = attacker.Value.battleentity.battleid;

                    if (MainManager.BadgeIsEquipped((int)Medal.WebSheet, target.trueid) && MainManager.HasCondition(MainManager.BattleCondition.Sticky, target) > -1 && entityExt.itemId != -1)
                    {
                        MainManager.instance.items[0].Add(entityExt.itemId);
                        GameObject item = Instantiate(entityExt.item.gameObject);
                        item.transform.parent = target.battleentity.transform;
                        item.transform.localPosition = new Vector3(0, 1, -0.1f);
                        item.GetComponent<SpriteRenderer>().enabled = true;

                        if (!didAnim)
                            battle.StartCoroutine(battle.ItemSpinAnim(target.battleentity.transform.position + Vector3.up, MainManager.itemsprites[1, (int)Medal.WebSheet], true));
                        entityExt.itemId = -1;
                        NPCControl_Ext.GetNPCControl_Ext(caller).items[enemyID] = -1;

                        Destroy(entityExt.item.gameObject);
                        Destroy(item, 1);
                    }
                }

            }

        }

        IEnumerator DoNightmare(BattleControl battle, MainManager.BattleData asleepPlayer)
        {
            battle.GetAvaliableTargets(false, false, -1, true);

            var targets = battle.avaliabletargets;
            var randomTarget = targets[UnityEngine.Random.Range(0, targets.Length)];

            bool playerTarget = false;
            if (UnityEngine.Random.Range(0, 10) < 3)
            {
                randomTarget = asleepPlayer;
                playerTarget = true;
            }

            Color baseSkyboxColor = RenderSettings.skybox.GetColor("_Tint");
            Color baseAmbientColor = RenderSettings.ambientLight;
            Color baseFogColor = RenderSettings.fogColor;
            float a = 0f;
            float b = 60f;
            do
            {
                RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, Color.black, a / b);
                RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, Color.black, a / b);
                RenderSettings.skybox.SetColor("_Tint", Color.Lerp(baseSkyboxColor, Color.black, a / b));
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b + 1f);

            MainManager.PlaySound("OmegaEye", 1.5f);
            var eye = (Instantiate(Resources.Load("Prefabs/Objects/Eye")) as GameObject).transform;
            eye.transform.position = randomTarget.battleentity.transform.position + new Vector3(0, 15f);
            eye.transform.rotation = Quaternion.Euler(0, 180, 180);

            var light = eye.GetChild(2);
            Renderer[] componentsInChildren = light.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].material.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, componentsInChildren[i].material.color.a);
            }

            yield return EventControl.sec;
            Destroy(eye.gameObject);

            var hand = (Instantiate(Resources.Load("Prefabs/Objects/DeadHand")) as GameObject).GetComponent<Animator>();
            hand.Play("1");
            Vector3 targetPos = randomTarget.battleentity.transform.position + new Vector3(0f, 5.2f, -0.1f) + new Vector3(0, randomTarget.battleentity.height);
            hand.transform.position = new Vector3(targetPos.x, targetPos.y + 5, targetPos.z);

            MainManager.PlaySound("OmegaMove");
            var startPos = hand.transform.position;
            a = 0f;
            b = 60f;
            do
            {
                hand.transform.position = MainManager.SmoothLerp(startPos, targetPos, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b + 1f);

            hand.Play("0");

            if (playerTarget)
                battle.DoDamage(null, ref MainManager.instance.playerdata[asleepPlayer.battleentity.battleid], 3, BattleControl.AttackProperty.Pierce, new DamageOverride[] { DamageOverride.NoFall }, false);
            else
                battle.DoDamage(null, ref battle.enemydata[randomTarget.battleentity.battleid], 3, BattleControl.AttackProperty.Pierce, new DamageOverride[] { DamageOverride.NoFall }, false);
            yield return EventControl.halfsec;

            hand.Play("1");
            startPos = hand.transform.position;
            targetPos = hand.transform.position + new Vector3(0, 20);
            a = 0f;
            b = 60f;
            do
            {
                hand.transform.position = MainManager.SmoothLerp(startPos, targetPos, a / b);
                RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, baseAmbientColor, a / b);
                RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, baseFogColor, a / b);
                RenderSettings.skybox.SetColor("_Tint", Color.Lerp(Color.black, baseSkyboxColor, a / b));
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b + 1f);

            Destroy(hand.gameObject);
            yield return null;
        }

        public static IEnumerator DoPhoenix(int playerId)
        {
            MainManager.battle.action = true;

            if (CheckPhoenix(playerId))
            {
                yield return Instance.FirePillarPhoenix(playerId);
                battle.RevivePlayer(playerId, 2, true);
                MainManager.instance.playerdata[playerId].battleentity.animstate = (int)MainManager.Animations.WeakBattleIdle;
                yield return EventControl.halfsec;
                battle.ClearStatus(ref MainManager.instance.playerdata[playerId]);
                MainManager.instance.playerdata[playerId].cantmove = 1;
            }
        }

        static bool CheckPhoenix(int playerId)
        {
            return MainManager.HasCondition(MainManager.BattleCondition.Fire, MainManager.instance.playerdata[playerId]) > -1 && MainManager.BadgeIsEquipped((int)Medal.Phoenix, MainManager.instance.playerdata[playerId].trueid) && MainManager.instance.playerdata[playerId].hp <= 0;

        }

        IEnumerator FirePillarPhoenix(int playerid)
        {
            var sound = MainManager_Ext.assetBundle.LoadAsset<AudioClip>("phoenixres");
            MainManager.PlaySound(sound);
            EntityControl battleEntity = MainManager.instance.playerdata[playerid].battleentity;
            var oldShieldpos = battleEntity.overrideshieldpos;
            battleEntity.animstate = (int)MainManager.Animations.KO;
            battleEntity.spin = Vector3.zero;
            battleEntity.CreateShield();
            battleEntity.shieldenabled = true;
            battleEntity.bubbleshield.targetscale = new Vector3(3f, 2.5f, 2f);
            battleEntity.overrideshieldpos = new Vector3?(new Vector3(0f, 0.5f));
            var mats = battleEntity.bubbleshield.GetComponent<Renderer>().materials;
            mats[0].SetColor("_EmissionColor", new Color(0.99f, 0.615f, 0.01f, 0.5f));
            mats[1].SetColor("_OutlineColor", Color.red);

            DialogueAnim pillar = (Instantiate(Resources.Load("Prefabs/Objects/FirePillar 1"), battleEntity.transform.position, Quaternion.identity) as GameObject).AddComponent<DialogueAnim>();
            pillar.transform.parent = MainManager.battle.battlemap.transform;
            pillar.transform.localScale = new Vector3(0f, 1f, 0f);
            pillar.targetscale = new Vector3(1f, 1f, 1f);
            pillar.shrink = false;
            pillar.shrinkspeed = 0.015f;
            battleEntity.bubbleshield.shrinkspeed = 0.015f;
            yield return new WaitForSeconds(3f);
            pillar.shrinkspeed = 0.2f;
            MainManager.ShakeScreen(0.25f, 0.75f);
            battleEntity.bubbleshield.shrink = true;
            battleEntity.bubbleshield.shrinkspeed = 0.2f;
            battleEntity.bubbleshield.targetscale = Vector3.zero;
            battleEntity.animstate = (int)MainManager.Animations.Hurt;
            yield return new WaitForSeconds(1.5f);
            pillar.targetscale = new Vector3(0f, 1f, 0f);
            ParticleSystem[] particles = pillar.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Stop();
            }
            pillar.shrinkspeed = 0.1f;
            Destroy(pillar.gameObject, 2f);
            Destroy(battleEntity.bubbleshield.gameObject);
            battleEntity.shieldenabled = false;
            battleEntity.bubbleshield = null;
            battleEntity.CreateShield();
            battleEntity.overrideshieldpos = oldShieldpos;
        }

        public delegate int DoDamageDelegate(MainManager.BattleData? attacker, ref MainManager.BattleData target, int damage, BattleControl.AttackProperty? property, bool block);
        public delegate int DoDamageOverridesDelegate(MainManager.BattleData? attacker, ref MainManager.BattleData target, int damage, BattleControl.AttackProperty? property, int[] overrides, bool block);

        public delegate void HealDelegate(ref MainManager.BattleData entity, int? amount, bool nosound);

        public int FindRelayable(EntityControl entity, BattleControl instance)
        {
            var enemies = new List<int>();
            for (int i = 0; i != instance.enemydata.Length; i++)
            {
                if (instance.enemydata[i].battleentity != entity && CanBeRelayed(instance.enemydata[i]))
                {
                    enemies.Add(i);
                    if (MainManager.HasCondition(MainManager.BattleCondition.AttackUp, instance.enemydata[i]) != -1 || instance.enemydata[i].charge > 0)
                    {
                        return i;
                    }
                }
            }
            return enemies[UnityEngine.Random.Range(0, enemies.Count)];
        }

        public bool CanRelay(EntityControl entity, BattleControl instance)
        {
            for (int i = 0; i != instance.enemydata.Length; i++)
            {
                if (instance.enemydata[i].battleentity != entity && CanBeRelayed(instance.enemydata[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CanBeRelayed(MainManager.BattleData enemy)
        {
            return enemy.hp > 0 && !enemy.lockrelayreceive
                && MainManager.HasCondition(MainManager.BattleCondition.Numb, enemy) == -1
                && MainManager.HasCondition(MainManager.BattleCondition.Sleep, enemy) == -1
                && MainManager.HasCondition(MainManager.BattleCondition.Freeze, enemy) == -1
                && MainManager.HasCondition(MainManager.BattleCondition.Taunted, enemy) == -1
                && MainManager.HasCondition(MainManager.BattleCondition.Sturdy, enemy) == -1;
        }

        public IEnumerator DoHardCharge(EntityControl entity, int animid, BattleControl instance, int maxCharge = 3, int hpReduction = 5)
        {
            battle.dontusecharge = true;
            MainManager.PlaySound("Charge7");
            entity.animstate = 24;
            StartCoroutine(entity.ShakeSprite(0.1f, 30f));
            yield return EventControl.halfsec;
            for (int i = 0; i < 3; i++)
            {
                battle.StartCoroutine(battle.StatEffect(entity, 4));
                MainManager.PlaySound("StatUp", -1, 0.9f + (float)(i + 1) * 0.1f, 1f);
                yield return EventControl.tenthsec;
                yield return EventControl.tenthsec;
            }

            if (entity.CompareTag("Player"))
            {
                MainManager.instance.playerdata[animid].charge = MainManager_Ext.CheckMaxCharge(animid);
            }
            else
            {
                instance.enemydata[animid].charge = maxCharge;

                instance.enemydata[animid].hp -= hpReduction;
            }
        }

        public IEnumerator UseItem(EntityControl entity, int targetID, BattleControl instance, MainManager.Items item, bool enemyOwnItem = false)
        {
            var entityExt = Entity_Ext.GetEntity_Ext(entity);

            if (enemyOwnItem)
            {
                enemyUsedItem = true;
                Destroy(entityExt.item);
                entityExt.itemId = -1;
            }

            if (battle.caller != null)
            {
                NPCControl_Ext.GetNPCControl_Ext(battle.caller).usedItem[actionID] = true;
            }

            battle.dontusecharge = true;
            entity.animstate = 4;
            MainManager.PlaySound("ItemHold");
            SpriteRenderer itemSprite = new GameObject().AddComponent<SpriteRenderer>();
            Vector3 offset = battle.enemydata[actionID].itemoffset == Vector3.zero ? new Vector3(0, 2.5f, -0.1f) : battle.enemydata[actionID].itemoffset;

            itemSprite.transform.position = entity.transform.position + offset + Vector3.up * entity.height;
            itemSprite.sprite = MainManager.itemsprites[0, (int)item];
            itemSprite.material.renderQueue = 50000;
            itemSprite.gameObject.layer = 14;
            yield return EventControl.sec;
            MainManager.ItemUse itemUse = MainManager.GetItemUse((int)item, 0);
            Destroy(itemSprite.gameObject);

            for (int i = 0; i < itemUse.usetype.Length; i++)
            {
                int reviveID = -1;
                if (itemUse.usetype[i] == MainManager.ItemUsage.Revive && battle.reservedata.Count > 0)
                {
                    reviveID = UnityEngine.Random.Range(0, battle.reservedata.Count);
                }
                yield return DoItemEffect(itemUse.usetype[i], itemUse.values[i], targetID, reviveID, item);
            }
        }

        void ReviveEnemy(int id, int value)
        {
            if (battle.reservedata.Count != 0)
            {
                MainManager.BattleData target = battle.reservedata[id];
                battle.ReviveEnemy(id, value, true, true);
                MainManager.HealParticle(target.battleentity.transform, Vector3.one, Vector3.up);
            }
        }


        public delegate void AddBuffDelegate(ref MainManager.BattleData entity, MainManager.BattleCondition condition, int turns);
        public delegate void DoDamageDelegateNoAttackerProperty(ref MainManager.BattleData target, int damage, BattleControl.AttackProperty? property);

        void RemoveEnemyCondition(int enemyID, MainManager.BattleCondition condition)
        {
            MainManager.PlaySound("Heal3");
            MainManager.PlayParticle("MagicUp", null, MainManager.battle.enemydata[enemyID].battleentity.transform.position);
            MainManager.RemoveCondition(condition, MainManager.battle.enemydata[enemyID]);
        }

        public void AddEnemyBuff(int enemyID, MainManager.BattleCondition condition, int value, string sound, int statEffect)
        {
            if (sound != null)
            {
                MainManager.PlaySound(sound);
            }

            battle.AddBuff(ref battle.enemydata[enemyID], condition, value);

            if (statEffect != -1)
            {
                MainManager.PlaySound("StatUp");
                battle.StartCoroutine(battle.StatEffect(battle.enemydata[enemyID].battleentity, statEffect));
            }
        }

        public void CureNegativeStatus(ref MainManager.BattleData target)
        {
            int conditionAmount = target.condition.Count;

            MainManager.RemoveCondition(MainManager.BattleCondition.Poison, target);
            MainManager.RemoveCondition(MainManager.BattleCondition.Sleep, target);
            MainManager.RemoveCondition(MainManager.BattleCondition.Freeze, target);
            MainManager.RemoveCondition(MainManager.BattleCondition.Numb, target);
            MainManager.RemoveCondition(MainManager.BattleCondition.Fire, target);
            MainManager.RemoveCondition(MainManager.BattleCondition.Inked, target);
            MainManager.RemoveCondition(MainManager.BattleCondition.Sticky, target);
            int amountCured = conditionAmount - target.condition.Count;
            if (target.battleentity.playerentity && target.hp > 0 && amountCured > 0)
            {
                Instance.DoPurifyingPulseCheck(ref target, amountCured);
                Instance.DoRevitalizingRippleCheck(ref target, amountCured);
            }

            if (target.battleentity.firepart != null)
            {
                Destroy(target.battleentity.firepart.gameObject);
            }
            target.battleentity.BreakIce();
            target.isasleep = false;
            target.isnumb = false;
        }



        public IEnumerator DoItemEffect(MainManager.ItemUsage type, int value, int? enemyID, int reviveID, MainManager.Items item)
        {

            bool wait = false;

            switch (type)
            {
                case MainManager.ItemUsage.Revive:
                    if (battle.reservedata.Count > 0 && reviveID != -1)
                    {
                        ReviveEnemy(reviveID, value);
                    }
                    else
                    {
                        battle.Heal(ref battle.enemydata[enemyID.Value], value, false);
                    }
                    wait = true;
                    break;


                case MainManager.ItemUsage.HPRecover:
                    battle.Heal(ref battle.enemydata[enemyID.Value], value, false);
                    wait = true;
                    break;

                case MainManager.ItemUsage.TPRecover:
                    MainManager.PlaySound("Heal2");
                    battle.ShowDamageCounter(2,
                            value,
                            battle.enemydata[enemyID.Value].battleentity.transform.position + battle.enemydata[enemyID.Value].cursoroffset,
                            Vector3.up);
                    wait = true;
                    break;

                case MainManager.ItemUsage.HPRecoverAll:
                    for (int i = 0; i < battle.enemydata.Length; i++)
                    {
                        battle.Heal(ref battle.enemydata[i], value, false);
                    }
                    wait = true;
                    break;

                case MainManager.ItemUsage.ReviveAll:
                    for (int i = 0; i < battle.enemydata.Length; i++)
                    {
                        battle.Heal(ref battle.enemydata[i], value, false);
                    }

                    for (int i = 0; i < battle.reservedata.Count; i++)
                    {
                        ReviveEnemy(i, value);
                    }
                    wait = true;
                    break;

                case ItemUsage.Battle:
                    yield return ManageBattleItems(battle.enemydata[actionID].battleentity, battle, item, enemyID.Value);
                    break;

                case (MainManager.ItemUsage)NewItemUse.AddAtkDown:
                    battle.StatusEffect(battle.enemydata[enemyID.Value], BattleCondition.AttackDown, value, true, false);
                    wait = true;
                    break;

                case (MainManager.ItemUsage)NewItemUse.AddDefDown:
                    battle.StatusEffect(battle.enemydata[enemyID.Value], BattleCondition.DefenseDown, value, true, false);
                    wait = true;
                    break;

                case (MainManager.ItemUsage)NewItemUse.AddTaunt:
                    Entity_Ext ext = Entity_Ext.GetEntity_Ext(battle.enemydata[enemyID.Value].battleentity);
                    ext.tauntedBy = battle.GetRandomAvaliablePlayer();
                    AddEnemyBuff(enemyID.Value, MainManager.BattleCondition.Taunted, value, "Taunt", -1);
                    wait = true;
                    break;

                case MainManager.ItemUsage.Sturdy:
                case (MainManager.ItemUsage)NewItemUse.AddSturdy:
                    AddEnemyBuff(enemyID.Value, MainManager.BattleCondition.Sturdy, value, "MagicUp", -1);
                    wait = true;
                    break;

                case (MainManager.ItemUsage)NewItemUse.AddFire:
                    AddEnemyBuff(enemyID.Value, MainManager.BattleCondition.Fire, value, "Flame", -1);
                    wait = true;
                    break;

                case MainManager.ItemUsage.DefUpStat:
                    AddEnemyBuff(enemyID.Value, MainManager.BattleCondition.DefenseUp, value, null, 1);
                    wait = true;
                    break;

                case MainManager.ItemUsage.AtkUpStat:
                    AddEnemyBuff(enemyID.Value, MainManager.BattleCondition.AttackUp, value, null, 0);
                    wait = true;
                    break;

                case (MainManager.ItemUsage)NewItemUse.RandomBuff:
                    if (battle.enemydata[enemyID.Value].hp > 0 && battle.enemydata[enemyID.Value].position != BattlePosition.Underground)
                    {
                        MainManager_Ext.Instance.DoRandomMysteryBuff(enemyID.Value, value, false);
                    }
                    wait = true;
                    break;

                case (MainManager.ItemUsage)NewItemUse.RandomBuffParty:
                    for (int i = 0; i < battle.enemydata.Length; i++)
                    {
                        if (battle.enemydata[i].hp > 0 && battle.enemydata[i].position != BattlePosition.Underground)
                        {
                            MainManager_Ext.Instance.DoRandomMysteryBuff(i, value, false);
                        }
                    }
                    wait = true;
                    break;

                case (MainManager.ItemUsage)NewItemUse.RandomDebuff:
                    if (battle.enemydata[enemyID.Value].hp > 0 && battle.enemydata[enemyID.Value].position != BattlePosition.Underground)
                    {
                        MainManager_Ext.Instance.DoRandomMysteryDebuff(enemyID.Value, value, false);
                    }
                    wait = true;
                    break;


                case (MainManager.ItemUsage)NewItemUse.RandomDebuffParty:
                    for (int i = 0; i < battle.enemydata.Length; i++)
                    {
                        bool once = type == (MainManager.ItemUsage)NewItemUse.RandomDebuff;

                        if (once)
                            i = enemyID.Value;

                        if (battle.enemydata[i].hp > 0 && battle.enemydata[i].position != BattlePosition.Underground)
                        {
                            MainManager_Ext.Instance.DoRandomMysteryDebuff(i, value, false);
                        }
                        if (once)
                            break;
                    }
                    wait = true;
                    break;

                case MainManager.ItemUsage.CurePoison:
                    RemoveEnemyCondition(enemyID.Value, MainManager.BattleCondition.Poison);
                    wait = true;
                    break;

                case MainManager.ItemUsage.CureFreeze:
                    RemoveEnemyCondition(enemyID.Value, MainManager.BattleCondition.Freeze);
                    battle.enemydata[enemyID.Value].battleentity.BreakIce();
                    wait = true;
                    break;

                case MainManager.ItemUsage.CureNumb:
                    RemoveEnemyCondition(enemyID.Value, MainManager.BattleCondition.Numb);
                    battle.enemydata[enemyID.Value].isnumb = false;
                    wait = true;
                    break;

                case MainManager.ItemUsage.CureSleep:
                    RemoveEnemyCondition(enemyID.Value, MainManager.BattleCondition.Sleep);
                    battle.enemydata[enemyID.Value].isasleep = false;
                    wait = true;
                    break;

                case MainManager.ItemUsage.CureParty:
                    MainManager.PlaySound("Heal3");
                    for (int i = 0; i < battle.enemydata.Length; i++)
                    {
                        if (battle.enemydata[i].hp > 0)
                        {
                            MainManager.PlayParticle("MagicUp", null, battle.enemydata[i].battleentity.transform.position);
                            CureNegativeStatus(ref battle.enemydata[i]);
                        }
                    }
                    wait = true;
                    break;

                case MainManager.ItemUsage.AddPoison:
                    if (MainManager.HasCondition(MainManager.BattleCondition.Sturdy, battle.enemydata[enemyID.Value]) > -1 || (UnityEngine.Random.Range(0, 100) < battle.enemydata[enemyID.Value].poisonres))
                    {
                        break;
                    }
                    AddEnemyBuff(enemyID.Value, MainManager.BattleCondition.Poison, value, "Poison", -1);
                    break;

                case MainManager.ItemUsage.AddSleep:
                    if (MainManager.HasCondition(MainManager.BattleCondition.Sturdy, battle.enemydata[enemyID.Value]) > -1 || (UnityEngine.Random.Range(0, 100) < battle.enemydata[enemyID.Value].sleepres))
                    {
                        break;
                    }
                    AddEnemyBuff(enemyID.Value, MainManager.BattleCondition.Sleep, value, "Sleep", -1);
                    break;

                case MainManager.ItemUsage.AddNumb:

                    if (MainManager.HasCondition(MainManager.BattleCondition.Sturdy, battle.enemydata[enemyID.Value]) > -1 || (UnityEngine.Random.Range(0, 100) < battle.enemydata[enemyID.Value].numbres))
                    {
                        break;
                    }
                    AddEnemyBuff(enemyID.Value, MainManager.BattleCondition.Numb, value, "Shock", -1);
                    break;

                case MainManager.ItemUsage.AddFreeze:
                    if (MainManager.HasCondition(MainManager.BattleCondition.Sturdy, battle.enemydata[enemyID.Value]) > -1 || (UnityEngine.Random.Range(0, 100) < battle.enemydata[enemyID.Value].freezeres))
                    {
                        break;
                    }
                    AddEnemyBuff(enemyID.Value, MainManager.BattleCondition.Freeze, value, "Freeze", -1);
                    MainManager.PlayParticle("mothicenormal", null, battle.enemydata[enemyID.Value].battleentity.transform.position + Vector3.up + battle.enemydata[enemyID.Value].battleentity.height * Vector3.up).transform.localScale = Vector3.one * 1.5f;
                    if (MainManager.HasCondition(MainManager.BattleCondition.Freeze, battle.enemydata[enemyID.Value]) > -1 && (battle.enemydata[enemyID.Value].battleentity.icecube == null || !battle.enemydata[enemyID.Value].battleentity.icecube.activeInHierarchy))
                    {
                        battle.enemydata[enemyID.Value].battleentity.Freeze();
                    }
                    break;

                case MainManager.ItemUsage.HPto1:
                    MainManager.PlaySound("Damage0");
                    battle.enemydata[enemyID.Value].hp = 1;
                    wait = true;
                    break;

                case MainManager.ItemUsage.GradualHP:
                    AddEnemyBuff(enemyID.Value, MainManager.BattleCondition.GradualHP, value, "Heal3", -1);
                    MainManager.PlayParticle("MagicUp", null, battle.enemydata[enemyID.Value].battleentity.transform.position);
                    break;

                case MainManager.ItemUsage.GradualTP:
                    MainManager.PlaySound("Heal3");
                    MainManager.PlayParticle("MagicUp", null, battle.enemydata[enemyID.Value].battleentity.transform.position);
                    wait = true;
                    break;

                case MainManager.ItemUsage.GradualHPParty:
                    MainManager.PlaySound("Heal3");
                    for (int i = 0; i < battle.enemydata.Length; i++)
                    {
                        if (battle.enemydata[i].hp > 0)
                        {
                            MainManager.PlayParticle("MagicUp", null, battle.enemydata[i].battleentity.transform.position);
                            AddEnemyBuff(i, MainManager.BattleCondition.GradualHP, value, null, -1);
                        }
                    }
                    wait = true;
                    break;

                case MainManager.ItemUsage.ChargeUp:
                    MainManager.PlaySound("StatUp");
                    battle.enemydata[enemyID.Value].charge = Mathf.Clamp(battle.enemydata[enemyID.Value].charge + value, 0, 3);
                    battle.StartCoroutine(battle.StatEffect(battle.enemydata[enemyID.Value].battleentity, 4));
                    wait = true;
                    break;

                case MainManager.ItemUsage.AtkDownAfter:
                    battle.enemydata[enemyID.Value].atkdownonloseatkup = true;
                    break;

                case MainManager.ItemUsage.CureFire:
                    MainManager.PlaySound("Heal3");
                    RemoveEnemyCondition(enemyID.Value, MainManager.BattleCondition.Fire);
                    if (battle.enemydata[enemyID.Value].battleentity.firepart != null)
                    {
                        Destroy(battle.enemydata[enemyID.Value].battleentity.firepart.gameObject);
                    }
                    wait = true;
                    break;

                case MainManager.ItemUsage.CureAll:
                    MainManager.PlaySound("Heal3");
                    CureNegativeStatus(ref battle.enemydata[enemyID.Value]);
                    break;

                case MainManager.ItemUsage.TurnNextTurn:
                    MainManager.PlaySound("Heal3");
                    battle.enemydata[enemyID.Value].moreturnnextturn += 1;
                    battle.StartCoroutine(battle.StatEffect(battle.enemydata[enemyID.Value].battleentity, 5));
                    break;

                case MainManager.ItemUsage.HPorDamage:
                    if (UnityEngine.Random.Range(0, 100) > 33)
                    {
                        battle.Heal(ref battle.enemydata[enemyID.Value], value, false);
                    }
                    else
                    {
                        battle.DoDamage(ref battle.enemydata[enemyID.Value], value, BattleControl.AttackProperty.NoExceptions);
                    }
                    battle.enemydata[enemyID.Value].hp = Mathf.Clamp(battle.enemydata[enemyID.Value].hp, 1, battle.enemydata[enemyID.Value].maxhp);
                    break;

                case MainManager.ItemUsage.CurePoisonAll:
                    MainManager.PlaySound("Heal3");
                    for (int i = 0; i < battle.enemydata.Length; i++)
                    {
                        if (battle.enemydata[i].hp > 0)
                        {
                            RemoveEnemyCondition(i, MainManager.BattleCondition.Poison);
                        }
                    }
                    break;
                case (ItemUsage)NewItemUse.AddInk:
                    AddEnemyBuff(enemyID.Value, MainManager.BattleCondition.Inked, value, "WaterSplash2", -1);
                    break;
                case (ItemUsage)NewItemUse.AddSticky:
                    AddEnemyBuff(enemyID.Value, MainManager.BattleCondition.Sticky, value, "WaterSplash2", -1);
                    break;
                case (ItemUsage)NewItemUse.AddInkParty:
                    MainManager.PlaySound("WaterSplash2");
                    for (int i = 0; i < battle.enemydata.Length; i++)
                    {
                        if (battle.enemydata[i].hp > 0)
                        {
                            AddEnemyBuff(i, MainManager.BattleCondition.Inked, value, null, -1);
                        }
                    }
                    break;
                case (ItemUsage)NewItemUse.AddStickyParty:
                    MainManager.PlaySound("WaterSplash2");
                    for (int i = 0; i < battle.enemydata.Length; i++)
                    {
                        if (battle.enemydata[i].hp > 0)
                        {
                            AddEnemyBuff(i, MainManager.BattleCondition.Sticky, value, null, -1);
                        }
                    }
                    break;

                case (ItemUsage)NewItemUse.ChargeMax:
                    MainManager.PlaySound("StatUp");
                    battle.enemydata[enemyID.Value].charge = 3;
                    battle.StartCoroutine(battle.StatEffect(battle.enemydata[enemyID.Value].battleentity, 4));
                    wait = true;
                    break;
            }

            if (wait)
            {
                yield return EventControl.thirdsec;
            }
        }

        public int GetLowHPEnemy()
        {
            for (int j = 0; j < battle.enemydata.Length; j++)
            {
                float hpPercent = battle.HPPercent(battle.enemydata[j]);
                if (hpPercent <= 0.7f)
                {
                    return j;
                }
            }
            return -1;
        }

        static IEnumerator CheckUseItem()
        {
            var entityExt = Entity_Ext.GetEntity_Ext(MainManager.battle.enemydata[actionID].battleentity);
            int target = -1;
            int itemUseOdds = 35;
            if (entityExt.itemId != -1)
            {
                MainManager.ItemUse itemUse = MainManager.GetItemUse(entityExt.itemId, 0);
                for (int i = 0; i < itemUse.usetype.Length; i++)
                {
                    switch (itemUse.usetype[i])
                    {
                        case ItemUsage.Revive:
                        case ItemUsage.ReviveAll:
                            if (battle.reservedata.Count > 0)
                            {
                                yield return BattleControl_Ext.Instance.UseItem(battle.enemydata[actionID].battleentity, -1, battle, (MainManager.Items)entityExt.itemId, true);
                                yield break;
                            }
                            else
                            {
                                target = BattleControl_Ext.Instance.GetLowHPEnemy();
                                if (target != -1)
                                {
                                    yield return BattleControl_Ext.Instance.UseItem(battle.enemydata[actionID].battleentity, target, battle, (MainManager.Items)entityExt.itemId, true);
                                    yield break;
                                }
                            }
                            break;

                        case ItemUsage.HPRecover:
                        case ItemUsage.HPRecoverAll:
                        case ItemUsage.HPRecoverFull:
                        case ItemUsage.HPorDamage:
                            target = BattleControl_Ext.Instance.GetLowHPEnemy();
                            if (target != -1)
                            {
                                yield return BattleControl_Ext.Instance.UseItem(battle.enemydata[actionID].battleentity, target, battle, (MainManager.Items)entityExt.itemId, true);
                                yield break;
                            }
                            break;

                        case ItemUsage.AtkUpStat:
                        case ItemUsage.DefUpStat:
                        case ItemUsage.GradualHP:
                        case ItemUsage.GradualHPParty:
                        case ItemUsage.TurnNextTurn:
                        case ItemUsage.ChargeUp:
                            target = UnityEngine.Random.Range(0, battle.enemydata.Length);
                            yield return BattleControl_Ext.Instance.UseItem(battle.enemydata[actionID].battleentity, target, battle, (MainManager.Items)entityExt.itemId, true);
                            yield break;

                        case ItemUsage.Battle:
                            itemUseOdds = 70;
                            break;
                    }
                }
                if (UnityEngine.Random.Range(0, 100) < itemUseOdds)
                {
                    target = UnityEngine.Random.Range(0, battle.enemydata.Length);
                    yield return BattleControl_Ext.Instance.UseItem(battle.enemydata[actionID].battleentity, target, battle, (MainManager.Items)entityExt.itemId, true);
                    yield break;
                }
            }
        }

        IEnumerator ManageBattleItems(EntityControl entity, BattleControl instance, MainManager.Items itemID, int targetId = -1)
        {
            battle.nonphyscal = true;
            int target = -1;

            int cherryDamage = 0;
            if (itemID == Items.CherryBomb)
            {
                cherryDamage = 3;
                var bombs = new MainManager.Items[] { Items.PoisonBomb, Items.SleepBomb, Items.FrostBomb, Items.BurlyBomb, Items.NumbBomb, Items.SpicyBomb, (Items)NewItem.InkBomb, (Items)NewItem.MysteryBomb };
                itemID = bombs[UnityEngine.Random.Range(0, bombs.Length)];
            }

            switch (itemID)
            {
                case Items.LonglegSummoner:
                    battle.GetSingleTarget();
                    yield return battle.LongLeg(entity, MainManager.instance.playerdata[battle.playertargetID]);
                    break;

                case (Items)NewItem.PointSwap:
                    yield return DoPointSwap(entity, true, targetId);
                    break;

                case (Items)NewItem.FlameBomb:
                    yield return DoFlameBomb(entity, true);
                    break;

                case (Items)NewItem.SeedlingWhistle:
                    yield return DoSeedlingStampede(true);
                    break;

                case (Items)NewItem.InkTrap:
                    battle.GetSingleTarget();
                    yield return DoInkTrap(true, entity, battle.enemydata[actionID]);
                    break;

                case (Items)NewItem.StickyBomb:
                    yield return DoStickyBomb(true, entity, battle.enemydata[actionID]);
                    break;
                case (Items)NewItem.WebWad:
                case Items.HardSeed:
                case Items.Ice:
                case Items.FlameRock:
                case Items.NumbDart:
                case Items.PoisonDart:
                case (Items)NewItem.SucculentSeed:
                case (Items)NewItem.SquashSeed:
                case (Items)NewItem.BeeBattery:
                case (Items)NewItem.MysterySeed:
                    battle.GetSingleTarget();
                    target = battle.playertargetID;

                    int damage = 2;
                    BattleControl.AttackProperty? property = null;
                    bool spin = true;
                    bool curve = true;
                    switch (itemID)
                    {
                        case (Items)NewItem.BeeBattery:
                            damage = 1;
                            property = BattleControl.AttackProperty.Numb;
                            break;
                        case Items.Ice:
                            property = BattleControl.AttackProperty.Freeze;
                            break;
                        case Items.FlameRock:
                            property = BattleControl.AttackProperty.Fire;
                            break;
                        case Items.NumbDart:
                            property = BattleControl.AttackProperty.Sleep;
                            spin = false;
                            curve = false;
                            break;
                        case Items.PoisonDart:
                            property = BattleControl.AttackProperty.Poison;
                            curve = false;
                            spin = false;
                            break;

                        case (Items)NewItem.WebWad:
                            damage = 1;
                            MainManager.SetCondition(BattleCondition.Sticky, ref battle.enemydata[actionID], 2);
                            MainManager.PlayParticle("StickyGet", entity.transform.position + Vector3.up);
                            MainManager.PlaySound("AhoneynationSpit", -1, 0.8f, 1f);
                            break;

                        case (Items)NewItem.SquashSeed:
                        case (Items)NewItem.SucculentSeed:
                        case (Items)NewItem.MysterySeed:
                            damage = 4;
                            break;
                    }
                    yield return ThrowItem(entity, battle, itemID, BattleControl.AttackArea.SingleEnemy, curve, spin);
                    int damageDealt = battle.DoDamage(null, ref MainManager.instance.playerdata[target], damage, property, null, battle.commandsuccess);

                    if (damageDealt > 0)
                    {
                        if (itemID == (Items)NewItem.SucculentSeed)
                            battle.Heal(ref battle.enemydata[actionID], damageDealt);

                        if (itemID == (Items)NewItem.SquashSeed)
                        {
                            MainManager.PlaySound("Heal2");
                            battle.ShowDamageCounter(2,
                                    damageDealt,
                                    battle.enemydata[actionID].battleentity.transform.position + battle.enemydata[actionID].cursoroffset,
                                    Vector3.up);
                        }
                    }

                    if (itemID == (Items)NewItem.MysterySeed)
                    {
                        DoMysteryEffect(ref MainManager.instance.playerdata[target], 3);

                    }

                    if (itemID == (Items)NewItem.WebWad)
                    {
                        if (MainManager.instance.playerdata[target].hp > 0)
                        {
                            MainManager.SetCondition(BattleCondition.Sticky, ref MainManager.instance.playerdata[target], 4);
                            MainManager.PlayParticle("StickyGet", MainManager.instance.playerdata[target].battleentity.transform.position + Vector3.up);
                        }
                        MainManager.PlaySound("AhoneynationSpit", -1, 0.8f, 1f);
                    }

                    yield return EventControl.quartersec;
                    break;

                case (Items)NewItem.MysteryBomb:
                    yield return CheckBomb(itemID, entity, 6 + cherryDamage, null, "explosion", 0.2f, Vector3.one);
                    for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                    {
                        if (MainManager.instance.playerdata[i].hp > 0)
                        {
                            DoMysteryEffect(ref MainManager.instance.playerdata[i], 3);
                        }
                    }
                    break;
                case Items.PoisonBomb:
                    yield return CheckBomb(itemID, entity, 4 + cherryDamage, AttackProperty.Poison, "PoisonEffect", 0.2f, Vector3.one * 2f);
                    break;
                case Items.SleepBomb:
                    yield return CheckBomb(itemID, entity, 4 + cherryDamage, AttackProperty.Sleep, null, 0.1f, Vector3.one * 3f);
                    break;
                case Items.FrostBomb:
                    yield return CheckBomb(itemID, entity, 4 + cherryDamage, AttackProperty.Freeze, "mothicenormal", 0.1f, Vector3.one * 3f);
                    break;
                case Items.BurlyBomb:
                    yield return CheckBomb(itemID, entity, 3 + cherryDamage, AttackProperty.DefDownOnBlock, "explosion", 0.2f, Vector3.one);
                    break;
                case Items.NumbBomb:
                    yield return CheckBomb(itemID, entity, 4 + cherryDamage, AttackProperty.Numb, "ElecFast", 0.1f, Vector3.one * 2f);
                    break;
                case Items.SpicyBomb:
                    yield return CheckBomb(itemID, entity, 5 + cherryDamage, null, "explosion", 0.2f, Vector3.one);
                    break;
                case (Items)NewItem.InkBomb:
                    yield return CheckBomb(itemID, entity, 4 + cherryDamage, AttackProperty.Ink, "InkGet", 0.2f, Vector3.one * 2);
                    for (int i = 0; i < battle.enemydata.Length; i++)
                    {
                        if (battle.enemydata[i].hp > 0 && battle.enemydata[i].position != BattlePosition.Underground)
                        {
                            MainManager.SetCondition(BattleCondition.Inked, ref battle.enemydata[i], 4);
                        }
                    }
                    var ps = MainManager.PlayParticle("impactsmoke", "BubbleBurst", Vector3.one).GetComponent<ParticleSystem>();
                    var main = ps.main;
                    main.startColor = new Color(0.22f, 0f, 0.5f, 1f);

                    break;
                case Items.Abombhoney:
                    yield return ThrowItem(entity, battle, itemID, BattleControl.AttackArea.All, true, true);
                    yield return DoAbomb(new Vector3(0f, 0f, -0.5f), battle);
                    break;
                case Items.ClearBomb:
                    yield return ThrowItem(entity, battle, itemID, BattleControl.AttackArea.All, true, true);
                    yield return battle.ClearBombEffect();
                    break;


                case Items.GenerousSeed:
                case Items.VitalitySeed:

                    MainManager.BattleCondition condition = BattleCondition.AttackUp;
                    int statEffect = 0;

                    if (itemID == Items.GenerousSeed)
                    {
                        statEffect = 1;
                        condition = BattleCondition.DefenseUp;
                    }

                    int targetID = UnityEngine.Random.Range(0, battle.enemydata.Length);
                    AddEnemyBuff(targetID, condition, 2, null, statEffect);
                    yield return EventControl.thirdsec;
                    break;
            }
        }

        void DoMysteryEffect(ref MainManager.BattleData target, int turns)
        {
            if (target.hp > 0)
            {
                BattleCondition status = UnityEngine.Random.Range(0, 2) == 0 ? BattleCondition.AttackDown : BattleCondition.AttackUp;
                MainManager.SetCondition(status, ref target, turns);
                if (status == BattleCondition.AttackDown)
                {
                    StartCoroutine(battle.StatEffect(target.battleentity, 2));
                    MainManager.PlaySound("StatDown");
                }
                else
                {
                    StartCoroutine(battle.StatEffect(target.battleentity, 0));
                    MainManager.PlaySound("StatUp");
                }
            }
        }

        IEnumerator CheckBomb(MainManager.Items bomb, EntityControl entity, int damage, AttackProperty? property, string particleName, float shakeAmount, Vector3 particleScale)
        {
            yield return ThrowItem(entity, battle, damage >= 6 ? Items.CherryBomb : bomb, BattleControl.AttackArea.AllEnemies, true, true);

            MainManager.ShakeScreen(shakeAmount, 0.75f, true);
            if (particleName != null)
            {
                MainManager.PlayParticle(particleName, battle.partymiddle).transform.localScale = particleScale;
            }
            if (bomb == Items.SleepBomb)
            {
                MainManager.DeathSmoke(battle.partymiddle, Vector3.one * 3f);
            }

            if (bomb == Items.NumbBomb)
            {
                MainManager.PlayParticle("impactsmoke", battle.partymiddle);
            }
            battle.PartyDamage(actionID, damage, property, battle.commandsuccess);
        }

        IEnumerator ThrowItem(EntityControl entity, BattleControl battle, MainManager.Items item, BattleControl.AttackArea area, bool curve, bool spin, float speed = 40f)
        {
            yield return EventControl.tenthsec;
            entity.animstate = 28;
            MainManager.PlaySound("Toss");
            yield return EventControl.tenthsec;

            Vector3 itemPos;
            bool usedByEnemy = battle.enemy;

            if (usedByEnemy)
            {
                itemPos = entity.transform.position + battle.enemydata[actionID].itemoffset + Vector3.up * entity.height;
            }
            else
            {
                itemPos = entity.transform.position + MainManager.instance.playerdata[battle.currentturn].cursoroffset - Vector3.up;
            }

            SpriteRenderer itemSprite = MainManager.NewSpriteObject(itemPos, null, MainManager.itemsprites[0, (int)item]);

            if (item == MainManager.Items.NumbDart || item == MainManager.Items.PoisonDart)
            {
                itemSprite.transform.localEulerAngles = new Vector3(0f, 0f, -15f);
            }

            Vector3 startPos = itemSprite.transform.position;
            Vector3 endPos = Vector3.zero;


            if (area == BattleControl.AttackArea.AllEnemies)
            {
                endPos = usedByEnemy ? battle.partymiddle : Vector3.right * 2;
            }
            else if (area == BattleControl.AttackArea.SingleEnemy)
            {
                if (usedByEnemy)
                {
                    var targetEntity = MainManager.instance.playerdata[battle.playertargetID].battleentity;
                    endPos = targetEntity.transform.position + Vector3.up;
                }
                else
                {
                    int target = battle.target;
                    var targetEntity = battle.enemydata[target].battleentity;
                    endPos = targetEntity.transform.position + battle.enemydata[target].cursoroffset + new Vector3(0f, targetEntity.height - 1f);
                }
            }

            if (item == Items.ClearBomb || (NewItem)item == NewItem.InkBomb)
            {
                endPos = Vector3.zero;
            }

            if (item == Items.Abombhoney)
            {
                endPos = new Vector3(0f, 0f, -0.5f);
            }

            float a = 0f;
            float b = speed;
            do
            {
                if (curve)
                {
                    itemSprite.transform.position = MainManager.BeizierCurve3(startPos, endPos, 5, a / b);
                }
                else
                {
                    itemSprite.transform.position = Vector3.Lerp(startPos, endPos, a / b);
                }

                if (spin)
                {
                    itemSprite.transform.eulerAngles += new Vector3(0f, 0f, -MainManager.framestep * 20f);
                }
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);
            Destroy(itemSprite.gameObject);
        }

        IEnumerator DoAbomb(Vector3 position, BattleControl battle)
        {
            MainManager.PlaySound("Splat1");
            MainManager.PlaySound("Fuse");
            GameObject part = Instantiate(Resources.Load("Prefabs/Objects/Abombnation"), position, Quaternion.identity) as GameObject;

            yield return new WaitForSeconds(2f);
            MainManager.PlayParticle("explosion", part.transform.position);
            MainManager.PlaySound("Explosion3");
            MainManager.PlaySound("Splat2");
            Destroy(part.gameObject);

            MainManager.ShakeScreen(0.25f, 0.75f);
            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.instance.playerdata[i].hp > 0 && MainManager.instance.playerdata[i].eatenby == null)
                {
                    if (MainManager.HasCondition(MainManager.BattleCondition.Shield, MainManager.instance.playerdata[i]) > -1)
                    {
                        MainManager.RemoveCondition(MainManager.BattleCondition.Shield, MainManager.instance.playerdata[i]);
                        MainManager.instance.playerdata[i].battleentity.shieldenabled = false;
                    }
                    else
                    {
                        if (!MainManager.instance.playerdata[i].plating)
                        {
                            battle.DoDamage(null, ref MainManager.instance.playerdata[i], 10, BattleControl.AttackProperty.NoExceptions, null, false);
                        }
                        else
                        {
                            MainManager.instance.playerdata[i].plating = false;
                        }
                    }
                }
            }
            for (int i = 0; i < battle.enemydata.Length; i++)
            {
                if (MainManager.HasCondition(MainManager.BattleCondition.Shield, battle.enemydata[i]) <= -1)
                {
                    if (battle.enemydata[i].animid == (int)MainManager.Enemies.Abomihoney || battle.enemydata[i].animid == (int)MainManager.Enemies.Ahoneynation || battle.enemydata[i].animid == (int)NewEnemies.Abomiberry)
                    {
                        battle.Heal(ref battle.enemydata[i], 10, false);
                    }
                    else
                    {
                        battle.DoDamage(null, ref battle.enemydata[i], 10, BattleControl.AttackProperty.NoExceptions, null, false);
                    }
                }
                else
                {
                    MainManager.RemoveCondition(MainManager.BattleCondition.Shield, battle.enemydata[i]);
                    battle.enemydata[i].battleentity.shieldenabled = false;
                }
            }

            SpriteRenderer[] splatters = new SpriteRenderer[3];
            Sprite splatterSprite = Resources.Load<Sprite>("Sprites/Objects/splatter");
            Vector3[] splattersSize = new Vector3[]
            {
                new Vector3(1.75f, 2f, 1f),
                Vector3.one * 2f,
                new Vector3(1.25f, 1.5f, 1f)
            };
            Vector3[] splattersPos = new Vector3[]
            {
                new Vector3(-4.73f, 1.22f, 10f),
                new Vector3(0f, 0f, 10f),
                new Vector3(5.65f, 0.15f, 10f)
            };
            for (int i = 0; i < splatters.Length; i++)
            {
                splatters[i] = MainManager.NewUIObject("splat" + i, MainManager.GUICamera.transform, splattersPos[i], splattersSize[i], splatterSprite, -5 - i).GetComponent<SpriteRenderer>();
                splatters[i].flipX = i == 1;
                DialogueAnim dialogueAnim = splatters[i].gameObject.AddComponent<DialogueAnim>();
                dialogueAnim.targetscale = splatters[i].transform.localScale;
                dialogueAnim.transform.localScale = Vector3.zero;
                dialogueAnim.shrinkspeed = 0.15f;
                splatters[i].color = new Color(1f, 0.75f, 0f);
                yield return new WaitForSeconds(0.025f);
            }

            float a = 0f;
            float b = 100f;
            do
            {
                if (a >= 50f)
                {
                    for (int i = 0; i < splatters.Length; i++)
                    {
                        splatters[i].color = new Color(splatters[i].color.r, splatters[i].color.g, splatters[i].color.b, Mathf.Lerp(1f, 0f, (a - 50f) / 50f));
                    }
                }
                a += MainManager.framestep;
                yield return null;
            } while (a < b);

            for (int i = 0; i < splatters.Length; i++)
            {
                Destroy(splatters[i].gameObject);
            }
            FixEnemyDiedOnItemUse();
        }

        void CheckEnemyItems()
        {
            var usableItems = new List<MainManager.Items>();

            usableItems.AddRange((Items[])Enum.GetValues(typeof(MainManager.Items)));
            usableItems.AddRange((Items[])Enum.GetValues(typeof(NewItem)));
            usableItems.Remove(Items.MoneySmall);
            usableItems.Remove(Items.MoneyMedium);
            usableItems.Remove(Items.None);

            for (int i = 0; i < usableItems.Count; i++)
            {
                switch (MainManager.GetItemUse((int)usableItems[i]).usetype[0])
                {
                    case ItemUsage.TPUP:
                    case ItemUsage.AttackUp:
                    case ItemUsage.HPUP:
                    case ItemUsage.HPUPAll:
                    case ItemUsage.MPUP:
                    case ItemUsage.DefenseUp:
                    case ItemUsage.None:
                        usableItems.Remove(usableItems[i]);
                        i--;
                        break;
                }
            }

            if (MainManager.battle.caller != null)
            {
                var npcExt = NPCControl_Ext.GetNPCControl_Ext(MainManager.battle.caller);
                for (int i = 0; i < MainManager.battle.enemydata.Length; i++)
                {
                    int enemyID = MainManager.battle.enemydata[i].animid;
                    if (EnemyItemData.enemyData[enemyID] != null && !MainManager.instance.inevent)
                    {
                        var entityExt = Entity_Ext.GetEntity_Ext(MainManager.battle.enemydata[i].battleentity);
                        if (npcExt.items[i] != -1)
                        {
                            entityExt.CreateItem(npcExt.items[i]);
                            continue;
                        }

                        if (!npcExt.rolledItem)
                        {
                            if (UnityEngine.Random.Range(0, 100) < 20 || MainManager.instance.flags[(int)NewCode.SCAVENGE])
                            {
                                int itemID;
                                if (MainManager.instance.flags[(int)NewCode.SCAVENGE])
                                {
                                    itemID = (int)usableItems[UnityEngine.Random.Range(0, usableItems.Count)];
                                }
                                else
                                {
                                    MainManager.Items[] items = EnemyItemData.enemyData[enemyID];
                                    itemID = (int)items[UnityEngine.Random.Range(0, items.Length)];
                                }
                                entityExt.CreateItem(itemID);
                                npcExt.items[i] = itemID;
                            }
                        }
                    }
                }
                npcExt.rolledItem = true;
            }
        }

        static IEnumerator DoPebbleToss()
        {
            if (!IsUsingItem())
            {
                var entity = MainManager.instance.playerdata[battle.currentturn].battleentity;

                battle.GetAvaliableTargets(false, false, actionID, true);
                var targetEntity = battle.avaliabletargets[battle.target];

                battle.dontusecharge = true;
                yield return EventControl.tenthsec;
                entity.animstate = 28;
                MainManager.PlaySound("Toss");
                yield return EventControl.tenthsec;

                SpriteRenderer pebble = new GameObject().AddComponent<SpriteRenderer>();
                Vector3 startPos = entity.transform.position + MainManager.instance.playerdata[battle.currentturn].cursoroffset - Vector3.up;
                Vector3 endPos = targetEntity.battleentity.transform.position + targetEntity.cursoroffset + new Vector3(0f, targetEntity.battleentity.height - 1f);

                List<GameObject> icecles = new List<GameObject>();

                bool bounce = false;

                bool tanjy = false;
                if (MainManager.BadgeIsEquipped((int)Medal.TanjyToss))
                {
                    tanjy = true;
                }

                if (tanjy || (MainManager.instance.flags[275] && UnityEngine.Random.Range(0, 128) == 0))
                {
                    pebble.sprite = Resources.LoadAll<Sprite>("Sprites/Entities/moth0")[(!MainManager.instance.flags[555]) ? 93 : ((!MainManager.instance.flags[682] || UnityEngine.Random.Range(0, 4) > 1) ? 92 : 91)];
                    pebble.transform.localScale = Vector3.one * 0.9f;
                    pebble.flipX = true;
                    bounce = true;
                }
                else
                {
                    pebble.sprite = MainManager.itemsprites[1, 13];
                }

                float startTime = 0f;
                float height = 5f + targetEntity.battleentity.height;
                float endTime = 40;

                bool failedStylish = false;
                bool succeedStylish = false;
                int stylishHits = 0;
                while (startTime < endTime)
                {
                    pebble.transform.eulerAngles += new Vector3(0f, 0f, -MainManager.framestep * 20f);
                    pebble.transform.position = MainManager.BeizierCurve3(startPos, endPos, height, startTime / endTime);

                    if (!failedStylish && !succeedStylish)
                    {
                        if (StylishUtils.CheckStylish(ref failedStylish, entity, startTime, 25f))
                        {
                            succeedStylish = true;
                            battle.StartCoroutine(KabbuStylish.DoPebbleTossStylish(entity, stylishHits));
                            stylishHits++;
                        }
                    }

                    startTime += MainManager.TieFramerate(1f);
                    yield return null;
                }

                entity.Emoticon(MainManager.Emoticons.None);

                int damage = tanjy ? 2 : 1;
                damage += Instance.rockyRampUpDmg;
                int enemyID = MainManager.battle.GetEnemyID(targetEntity.battleentity.transform);
                battle.DoDamage(ref battle.enemydata[enemyID], damage, new BattleControl.AttackProperty?(BattleControl.AttackProperty.NoExceptions));

                Instance.CheckAvalanche(battle, enemyID, icecles);
                Instance.CheckGrumbleGravel(enemyID, battle);

                if (MainManager.BadgeIsEquipped((int)Medal.SkippingStone))
                {
                    Instance.CreateRipple(pebble);
                    for (int i = enemyID + 1; i < battle.enemydata.Length; i++)
                    {
                        targetEntity = battle.enemydata[i];
                        failedStylish = false;
                        succeedStylish = false;

                        startTime = 0f;
                        endTime = 30f;
                        startPos = pebble.transform.position;
                        endPos = targetEntity.battleentity.transform.position + targetEntity.cursoroffset + new Vector3(0f, targetEntity.battleentity.height - 1f);
                        while (startTime < endTime)
                        {
                            pebble.transform.eulerAngles += new Vector3(0f, 0f, -MainManager.framestep * 20f);
                            pebble.transform.position = MainManager.BeizierCurve3(startPos, endPos, height, startTime / endTime);

                            if (!failedStylish && !succeedStylish)
                            {
                                if (StylishUtils.CheckStylish(ref failedStylish, entity, startTime, 15f))
                                {
                                    succeedStylish = true;
                                    battle.StartCoroutine(KabbuStylish.DoPebbleTossStylish(entity, stylishHits));
                                    stylishHits++;
                                }
                            }

                            startTime += MainManager.TieFramerate(1f);
                            yield return null;
                        }
                        entity.Emoticon(MainManager.Emoticons.None);
                        if (targetEntity.position == BattleControl.BattlePosition.Underground)
                            break;
                        Instance.CreateRipple(pebble);
                        battle.DoDamage(ref battle.enemydata[i], damage, new BattleControl.AttackProperty?(BattleControl.AttackProperty.NoExceptions));
                        Instance.CheckGrumbleGravel(i, battle);
                        Instance.CheckAvalanche(battle, i, icecles);
                    }
                }

                if (!bounce)
                {
                    Destroy(pebble.gameObject);
                }
                else
                {
                    MainManager.instance.StartCoroutine(MainManager.ArcMovement(pebble.gameObject, new Vector3(15f, 0f), 5f, 60f));
                    pebble.gameObject.AddComponent<SpinAround>().itself = new Vector3(0f, 0f, 20f);
                    Destroy(pebble.gameObject, 2f);
                }

                if (MainManager.BadgeIsEquipped((int)Medal.RockyRampUp) && !Instance.usedPebbleToss)
                {
                    Instance.rockyRampUpDmg++;
                    Instance.usedPebbleToss = true;
                }

                while (!MainManager.ArrayIsEmpty(icecles.ToArray()))
                {
                    yield return null;
                }
                yield return EventControl.halfsec;
            }
        }

        void CheckAvalanche(BattleControl battleControl, int enemyId, List<GameObject> icecles)
        {
            int avalancheDMG = 2;
            if (MainManager.BadgeIsEquipped((int)Medal.Avalanche) && UnityEngine.Random.Range(0, 2) == 0)
            {
                var endPosition = battleControl.enemydata[enemyId].battleentity.transform.position + Vector3.up * battleControl.enemydata[enemyId].battleentity.height;
                icecles.Add(Instantiate(Resources.Load("Prefabs/Objects/icecle"), new Vector3(endPosition.x, 15f, endPosition.z), Quaternion.identity) as GameObject);
                battleControl.StartCoroutine(Instance.DoAvalanche(icecles[icecles.Count - 1], icecles[icecles.Count - 1].transform.position, endPosition, battleControl, enemyId, avalancheDMG));
            }
        }

        void CreateRipple(SpriteRenderer pebble)
        {
            var ripplePos = new Vector3(pebble.transform.position.x, pebble.transform.position.y + 0.5f, pebble.transform.position.z);
            Instantiate(MainManager_Ext.assetBundle.LoadAsset("Ripples"), ripplePos, Quaternion.identity);
        }

        void CheckGrumbleGravel(int enemyID, BattleControl instance)
        {
            if (MainManager.BadgeIsEquipped((int)Medal.GrumbleGravel))
            {
                MainManager.SetCondition(MainManager.BattleCondition.Taunted, ref instance.enemydata[enemyID], 1);
                var entityExt = Entity_Ext.GetEntity_Ext(instance.enemydata[enemyID].battleentity);
                entityExt.tauntedBy = instance.currentturn;
            }
        }

        IEnumerator DoAvalanche(GameObject icecle, Vector3 startPos, Vector3 endPosition, BattleControl instance, int enemyID, int damage)
        {
            if (MainManager.BadgeIsEquipped((int)Medal.Avalanche))
            {
                float startTime = 0f;
                float endTime = 45f;
                do
                {
                    float framestep = MainManager.TieFramerate(1f);
                    icecle.transform.position = Vector3.Lerp(startPos, endPosition, startTime / endTime);
                    icecle.transform.eulerAngles += new Vector3(0f, framestep * 20f);
                    startTime += framestep;
                    yield return null;
                }
                while (startTime < endTime + 1f);

                MainManager.PlayParticle("mothicenormal", endPosition + Vector3.up).transform.localScale = Vector3.one * 2f;
                battle.DoDamage(ref instance.enemydata[enemyID], damage, BattleControl.AttackProperty.Freeze);
                icecle.transform.position = new Vector3(0f, -999f);
                yield return EventControl.halfsec;
                Destroy(icecle.gameObject);
            }
        }

        public delegate int DoDamageDelegateNoAttacker(ref MainManager.BattleData target, int damage, bool block);

        static IEnumerator CheckCustomEnemyAI()
        {
            var instance = MainManager.battle;
            if (instance.enemy && actionID >= 0 && AI.HasCustomAI((NewEnemies)instance.enemydata[actionID].animid))
            {
                yield return AI.GetAI((NewEnemies)instance.enemydata[actionID].animid).DoBattleAI(Instance.entityAttacking, actionID);
            }
        }

        public IEnumerator DoLateDamage(int enemyID, int playerID, int damage, BattleControl.AttackProperty? property, float delay, BattleControl instance)
        {
            yield return new WaitForSeconds(delay);
            battle.DoDamage(instance.enemydata[enemyID], ref MainManager.instance.playerdata[playerID], damage, property, instance.commandsuccess);
        }

        static int GetNewEnemySwap(int animid)
        {
            switch ((NewEnemies)animid)
            {
                case NewEnemies.Caveling:
                    return (int)MainManager.Enemies.Seedling;

                case NewEnemies.Frostfly:
                    return (int)MainManager.Enemies.Midge;

                case NewEnemies.PirahnaChomp:
                    return (int)MainManager.Enemies.FlyTrap;

                case NewEnemies.Moeruki:
                    return (int)MainManager.Enemies.ShockWorm;

                case NewEnemies.Abomiberry:
                    return (int)MainManager.Enemies.Abomihoney;

                case NewEnemies.SplotchSpider:
                    return (int)MainManager.Enemies.JumpingSpider;
                case NewEnemies.Spineling:
                    return (int)MainManager.Enemies.Cactus;
            }

            return MainManager.battle.enemydata[actionID].animid;
        }

        static bool IsCustomEnemy()
        {
            return MainManager.battle.enemydata[actionID].animid > 112;
        }

        static int CheckSeedlingDamage()
        {
            switch (MainManager.battle.enemydata[actionID].animid)
            {
                case (int)MainManager.Enemies.Mothfly:
                case (int)MainManager.Enemies.Flowering:
                    return 1;

                case (int)NewEnemies.Caveling:
                    return 3;

                default:
                    return 2;
            }
        }

        static bool IsFrostfly()
        {
            return MainManager.battle.enemydata[actionID].animid == (int)NewEnemies.Frostfly;
        }

        static int CheckEnemyWeevilRef()
        {
            var newEnemies = new int[] { (int)MainManager.Enemies.Seedling, (int)MainManager.Enemies.AngryPlant, (int)MainManager.Enemies.FlyTrap, (int)NewEnemies.Caveling };
            return battle.EnemyInField(actionID, newEnemies);
        }

        static void CheckWeevilEatBuff(int enemyEaten)
        {
            if (MainManager.battle.enemydata[enemyEaten].animid == (int)NewEnemies.Caveling)
            {
                MainManager.PlaySound("StatUp");
                battle.dontusecharge = true;
                MainManager.battle.enemydata[actionID].charge = 3;
                MainManager.battle.StartCoroutine(battle.StatEffect(MainManager.battle.enemydata[actionID].battleentity, 4));
            }
        }


        static bool IsUsingItem()
        {
            return MainManager.battle.currentaction == BattleControl.Pick.ItemList;
        }

        IEnumerator DoVitiation(EntityControl entity)
        {
            battle.dontusecharge = true;
            MainManager.instance.camtargetpos = new Vector3?(entity.transform.position + new Vector3(2f, 0f));
            MainManager.instance.camspeed = 0.01f;
            MainManager.instance.camoffset = new Vector3(0f, 2.65f, -7f);
            entity.animstate = 102;
            yield return new WaitForSeconds(0.75f);
            entity.animstate = 119;
            yield return EventControl.quartersec;
            BattleControl.SetDefaultCamera();

            int[] targets;
            if (actionID == (int)NewSkill.Vitiation)
            {
                targets = MainManager.OrganizeArrayInt(battle.partypointer, MainManager.GradualFill(MainManager.instance.playerdata.Length));
            }
            else
            {
                (targets = new int[1])[0] = battle.target;
            }

            MainManager.PlaySound("Shield");
            for (int i = 0; i < targets.Length; i++)
            {
                if (MainManager.instance.playerdata[targets[i]].hp > 0 && MainManager.instance.playerdata[targets[i]].eatenby == null)
                {
                    var entityExt = Entity_Ext.GetEntity_Ext(MainManager.instance.playerdata[targets[i]].battleentity);
                    entityExt.vitiation = true;
                    entityExt.vitiationDmg = 0;
                    MainManager.instance.playerdata[targets[i]].battleentity.shieldenabled = true;
                    var mats = MainManager.instance.playerdata[targets[i]].battleentity.bubbleshield.GetComponent<Renderer>().materials;
                    mats[0].SetColor("_EmissionColor", new Color(0.99f, 0.615f, 0.01f, 0.5f));
                    mats[1].SetColor("_OutlineColor", Color.red);
                }
            }

            float startTime = 0f;
            float endTime = 45f;

            bool failedStylish = false;
            bool succeedStylish = false;
            while (startTime < endTime)
            {
                if (!failedStylish && !succeedStylish)
                {
                    if (StylishUtils.CheckStylish(ref failedStylish, entity, startTime, 25f))
                    {
                        succeedStylish = true;
                        MainManager.battle.StartCoroutine(DoStylish(0));
                    }
                }

                startTime += MainManager.TieFramerate(1f);
                yield return null;
            }

            entity.Emoticon(MainManager.Emoticons.None);
            yield return WaitStylish(0f);

            if (actionID == (int)NewSkill.Vitiation)
            {
                MainManager.BattleData[] playerdata2 = MainManager.instance.playerdata;
                int num14 = battle.currentturn;
                playerdata2[num14].cantmove = playerdata2[num14].cantmove + 1;
            }
            yield break;
        }

        IEnumerator DoStealSkill(EntityControl entity)
        {
            entity.animstate = 103;
            yield return new WaitForSeconds(0.17f);
            entity.animstate = 104;

            int target = battle.target;
            MainManager.BattleData targetData = MainManager.battle.avaliabletargets[target];
            yield return DoCursorCommand(targetData, null, 150f, new Vector2(4f, 4f), 1.15f);

            if (battle.commandsuccess)
            {
                entity.animstate = 105;
            }
            else
            {
                entity.animstate = 106;
            }
            yield return new WaitForSeconds(0.05f);
            MainManager.PlaySound("Woosh", 8, 1.1f, 1f, true);
            GameObject beemerang = Instantiate<GameObject>(Resources.Load("Prefabs/Objects/BeerangBattle") as GameObject);
            beemerang.transform.position = entity.transform.position + Vector3.up;
            Vector3 startPos = beemerang.transform.position + new Vector3(0.5f, 0.5f);
            Vector3 targetPos = targetData.battleentity.sprite.transform.position + Vector3.up * 0.75f;
            Vector3 midPos = Vector3.Lerp(startPos, targetPos, 0.5f) + (battle.commandsuccess ? new Vector3(0f, 0f, -5f) : new Vector3(0f, 7f, 0f));

            int damage = battle.GetPlayerAttack(entity.animid, battle.commandsuccess);
            var entityExt = Entity_Ext.GetEntity_Ext(targetData.battleentity);
            SpriteRenderer enemyItem = null;
            if (battle.commandsuccess && entityExt.itemId != -1)
            {
                enemyItem = Instantiate(entityExt.item);
                Destroy(entityExt.item.gameObject);
            }

            float a = 0f;
            float b = 40f;
            bool hit = false;
            do
            {
                a += MainManager.framestep;
                beemerang.transform.position = MainManager.BeizierCurve3(startPos, targetPos, midPos, Mathf.Clamp01(battle.commandsuccess ? (hit ? ((b - a) / 20f) : (a / 20f)) : (a / b)));
                beemerang.transform.localEulerAngles = (battle.commandsuccess ? new Vector3(80f, 0f, beemerang.transform.localEulerAngles.z - MainManager.framestep * 20f) : new Vector3(0f, 0f, beemerang.transform.localEulerAngles.z - MainManager.framestep * 20f));
                if (battle.commandsuccess && !hit && a >= 20f)
                {
                    if (MainManager.battle.enemydata[targetData.battleentity.battleid].position != BattlePosition.Underground)
                        battle.DoDamage(MainManager.instance.playerdata[0], ref MainManager.battle.enemydata[targetData.battleentity.battleid], damage, null, null, false);

                    if (battle.commandsuccess && enemyItem != null)
                    {
                        enemyItem.enabled = true;
                    }

                    midPos = new Vector3(0f, 0f, 5f);
                    hit = true;
                }

                if (hit && enemyItem != null)
                {
                    enemyItem.transform.position = beemerang.transform.position + Vector3.up * 0.1f;
                }

                yield return null;
            }
            while (a < b);
            StartStylishTimer(3, 12, 0, false);

            if (battle.commandsuccess && enemyItem != null)
            {
                enemyItem.transform.parent = MainManager.battle.battlemap.transform;
                enemyItem.transform.position = entity.transform.position + MainManager.instance.playerdata[0].cursoroffset - Vector3.forward * 0.1f;
                entity.animstate = (int)MainManager.Animations.ItemGet;
                MainManager.PlaySound("ItemGet0");
            }

            Destroy(beemerang);
            MainManager.StopSound(8, 0.1f);
            if (!battle.commandsuccess)
            {
                battle.DoDamage(MainManager.instance.playerdata[0], ref MainManager.battle.enemydata[targetData.battleentity.battleid], damage, null, null, false);
            }

            if (enemyItem != null)
            {
                yield return EventControl.halfsec;
                Destroy(enemyItem.gameObject);

                if (MainManager.instance.items[0].Count < MainManager.instance.maxitems)
                {
                    MainManager.instance.items[0].Add(entityExt.itemId);
                }
                else
                {
                    MainManager.PlaySound("Fail");
                    entity.animstate = (int)MainManager.Animations.Angry;
                    yield return EventControl.quartersec;
                }

                entityExt.itemId = -1;
                NPCControl_Ext.GetNPCControl_Ext(battle.caller).items[target] = -1;
            }

            yield return EventControl.quartersec;
            yield return WaitStylish(0);

            entity.animstate = 13;
            yield return EventControl.tenthsec;
        }

        IEnumerator DoCursorCommand(MainManager.BattleData target, Vector3? cursorPos, float frameTime, Vector2 boundsOffset, float moveSpeed)
        {
            SpriteRenderer crosshair = MainManager.NewUIObject("cursor", null, Vector3.one, Vector3.one, MainManager.guisprites[41]).GetComponent<SpriteRenderer>();

            SpriteRenderer targetSprite = battle.TempCrosshair(target, false).GetComponent<SpriteRenderer>();
            targetSprite.transform.localScale = Vector3.one * 1.5f;

            if (cursorPos != null)
            {
                targetSprite.transform.position = cursorPos.Value;
            }

            Vector2 minBounds = (Vector2)targetSprite.transform.position - boundsOffset;
            Vector2 maxBounds = (Vector2)targetSprite.transform.position + boundsOffset;
            Vector2 cursorVelocity = Vector2.zero;
            float friction = 0.97f;
            float range = 1f;

            Vector2 crosshairPos = crosshair.transform.position;

            float a = 0;
            MainManager.PlaySound("Crosshair", 9, 0.9f, 0.35f, true);
            do
            {
                float inputX = 0;
                float inputY = 0;
                Vector2 lastaxis = new Vector3(InputIO.JoyStick(0), InputIO.JoyStick(1));
                if (MainManager.GetKey(2, true))
                {
                    inputX = -1;
                    if (lastaxis.x != 0f)
                    {
                        inputX = lastaxis.x;
                    }
                }
                else if (MainManager.GetKey(3, true))
                {
                    inputX = 1;
                    if (lastaxis.x != 0f)
                    {
                        inputX = lastaxis.x;
                    }
                }
                if (MainManager.GetKey(0, true))
                {
                    inputY = 1;
                    if (lastaxis.y != 0f)
                    {
                        inputY = -lastaxis.y;
                    }
                }
                else if (MainManager.GetKey(1, true))
                {
                    inputY = -1;
                    if (lastaxis.y != 0f)
                    {
                        inputY = -lastaxis.y;
                    }
                }

                if (Mathf.Abs(inputX) > 0.01f || Mathf.Abs(inputY) > 0.01f)
                {
                    cursorVelocity += new Vector2(inputX, inputY) * moveSpeed * Time.smoothDeltaTime;
                }

                cursorVelocity *= friction;
                crosshairPos += cursorVelocity;

                crosshairPos.x = Mathf.Clamp(crosshairPos.x, minBounds.x, maxBounds.x);
                crosshairPos.y = Mathf.Clamp(crosshairPos.y, -2, maxBounds.y);
                crosshair.transform.position = crosshairPos;

                if (Vector2.Distance(crosshair.transform.position, targetSprite.transform.position) <= range)
                {
                    crosshair.color = Color.green;
                    targetSprite.color = Color.green;
                    battle.commandsuccess = true;
                }
                else
                {
                    crosshair.color = Color.white;
                    targetSprite.color = Color.white;
                    battle.commandsuccess = false;
                }
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < frameTime);
            MainManager.StopSound(9);
            Destroy(crosshair.gameObject);
            Destroy(targetSprite.gameObject);
        }

        IEnumerator DoLecture(EntityControl entity)
        {
            yield return EventControl.tenthsec;
            AudioClip bleep = Resources.Load<AudioClip>("Audio/Sounds/Dialogue/Dialogue" + entity.dialoguebleepid);
            entity.talking = true;
            entity.animstate = (int)MainManager.Animations.Idle;

            int stepsAmount = 3;
            int baseInputs = 3;
            int baseOdds = 40;

            int[] kabbusAnims = new int[] { (int)MainManager.Animations.Idle, (int)MainManager.Animations.Angry, (int)MainManager.Animations.BattleIdle };
            int[] partyAnims = new int[] { (int)MainManager.Animations.Surprized, (int)MainManager.Animations.WeakPickAction, (int)MainManager.Animations.Sleep };

            for (int i = 0; i < stepsAmount; i++)
            {
                if (i != 0)
                {
                    yield return EventControl.sec;
                }
                entity.animstate = kabbusAnims[UnityEngine.Random.Range(0, kabbusAnims.Length)];
                MainManager.battle.StartCoroutine(battle.DoCommand(180f, ActionCommands.SequentialKeys, new float[] { baseInputs + i }));

                yield return null;
                int x = 0;
                while (MainManager.battle.doingaction)
                {
                    yield return null;
                    MainManager.PlayBleep(bleep, entity.bleeppitch, 1, x);
                    x++;
                    yield return null;
                }

                if (MainManager.battle.commandsuccess)
                {
                    baseOdds += 20;
                    entity.animstate = (int)MainManager.Animations.Happy;

                    for (int j = 0; j < MainManager.instance.playerdata.Length; j++)
                    {
                        if (MainManager.instance.playerdata[j].battleentity != entity && MainManager.instance.playerdata[j].hp > 0 && !battle.IsStopped(MainManager.instance.playerdata[j]))
                        {
                            if (partyAnims[i] == (int)MainManager.Animations.Surprized && j == 2)
                            {
                                MainManager.instance.playerdata[j].battleentity.animstate = (int)MainManager.Animations.Idle;
                            }
                            else
                            {
                                MainManager.instance.playerdata[j].battleentity.animstate = partyAnims[i];
                            }
                        }
                    }
                    StartStylishTimer(6, 15, i, false);
                }
                else
                {
                    break;
                }
            }

            entity.talking = false;
            Sprite[] notesprite = Resources.LoadAll<Sprite>("Sprites/Particles/music");

            float frequency = 1f;
            float amplitude = 1.5f;
            Vector3 targetPos = new Vector3(7, 3f, 0.1f);
            Vector3 startPos = new Vector3(0, 3f, 0.1f);
            int musicAmount = 12;
            SpriteRenderer[] musics = new SpriteRenderer[musicAmount];

            MainManager.PlaySound("Charge4", 1.2f, 1);
            yield return EventControl.tenthsec;

            for (int i = 0; i < musics.Length; i++)
            {
                float t = (float)i / (musicAmount - 1);
                Vector3 basePosition = Vector3.Lerp(startPos, targetPos, t);
                float waveOffset = Mathf.Sin(t * frequency * Mathf.PI * 2) * amplitude;
                basePosition.y += waveOffset;
                musics[i] = new GameObject().AddComponent<SpriteRenderer>();
                musics[i].sprite = notesprite[UnityEngine.Random.Range(0, notesprite.Length)];
                musics[i].color = MainManager.RainbowColor(UnityEngine.Random.Range(0, 10));

                musics[i].color = new Color(musics[i].color.r, musics[i].color.g, musics[i].color.b, 0);
                musics[i].transform.position = basePosition;
                MainManager.battle.StartCoroutine(MainManager_Ext.LerpSpriteColor(musics[i], 30f, new Color(musics[i].color.r, musics[i].color.g, musics[i].color.b, 1)));
                MainManager.battle.StartCoroutine(LerpPosition(120, musics[i].transform.position, musics[i].transform.position + Vector3.up, musics[i].transform));
                yield return new WaitForSeconds(0.05f);
            }

            yield return EventControl.halfsec;

            for (int i = 0; i < musics.Length; i++)
            {
                MainManager.battle.StartCoroutine(MainManager_Ext.LerpSpriteColor(musics[i], 30f, new Color(musics[i].color.r, musics[i].color.g, musics[i].color.b, 0)));
                Destroy(musics[i].gameObject, 2f);
            }
            yield return EventControl.tenthsec;

            for (int i = 0; i < MainManager.battle.enemydata.Length; i++)
            {
                if (UnityEngine.Random.Range(0, 100) < baseOdds && (MainManager.battle.enemydata[i].position != BattlePosition.Underground || MainManager.battle.enemydata[i].position != BattlePosition.OutOfReach))
                {
                    battle.TryCondition(ref MainManager.battle.enemydata[i], BattleCondition.Sleep, 2);
                }
            }
            yield return EventControl.quartersec;
        }

        IEnumerator DoCordycepsLeech(EntityControl entity)
        {
            Vector3 basePos = entity.transform.position;
            int target = battle.GetEnemies(false, false, false)[0];

            EntityControl targetEntity = MainManager.battle.enemydata[target].battleentity;
            MainManager.SetCamera(targetEntity.transform.position, MainManager.instance.camangleoffset, new Vector3(0f, 2.5f, -6f), 0.02f);

            float size = battle.GetEnemySize(target);

            float baseCamOffset = MainManager.instance.camoffset.y;
            entity.MoveTowards(targetEntity.transform.position + Vector3.left * Mathf.Clamp(size, 1.3f, float.PositiveInfinity) + Vector3.back * 0.25f, 2f, 1, 0);
            while (entity.forcemove)
            {
                yield return null;
            }

            MainManager.instance.camspeed = 0.01f;
            MainManager.instance.camoffset = new Vector3(0f, 2.5f, -5f);
            entity.animstate = (int)MainManager.Animations.WeakBattleIdle;

            float[] data = new float[] { -1f, 6.5f, 0.15f, 1f, 0f, 0f, 0f, 0f, 0f, 4f };
            MainManager.PlaySound("Fungi", -1, 1f, 1f, true);
            MainManager.battle.StartCoroutine(battle.DoCommand(180f, ActionCommands.TappingKey, data));
            yield return null;

            Vector3 startp = entity.spritetransform.localPosition;
            Vector3 intensity = new Vector3(0.1f, 0);
            entity.StartCoroutine(entity.ShakeSprite(0.1f, 240f));
            yield return new WaitUntil(() => !MainManager.battle.doingaction);

            int damage = Mathf.Clamp(Mathf.CeilToInt(BASE_CORYCEPSLEECH_DMG * battle.barfill), 4, BASE_CORYCEPSLEECH_DMG);
            yield return EventControl.sec;

            data = new float[] { (float)UnityEngine.Random.Range(4, 7) };
            MainManager.battle.StartCoroutine(battle.DoCommand(60f, ActionCommands.PressKeyTimer, data));

            yield return null;
            yield return new WaitUntil(() => !MainManager.battle.doingaction);
            yield return EventControl.halfsec;
            MainManager.StopSound("Fungi");
            MainManager.instance.camspeed = 0.1f;
            MainManager.instance.camoffset = new Vector3(0f, baseCamOffset, -7f);
            MainManager.PlaySound("Buzz1");

            yield return EventControl.tenthsec;

            if (battle.commandsuccess)
            {
                MainManager.ShakeScreen(0.2f, 0.5f, true);
                MainManager.PlaySound("HugeHit");
            }
            else
            {
                damage = damage / 2;
            }

            //cordyceps anim
            entity.animstate = 124;

            yield return EventControl.tenthsec;
            int damageDone = battle.DoDamage(MainManager.instance.playerdata[MainManager.battle.currentturn], ref MainManager.battle.enemydata[target], damage, null, null, false);
            yield return null;
            entity.animstate = 124;
            battle.Heal(ref MainManager.instance.playerdata[MainManager.battle.currentturn], Mathf.Clamp(Mathf.CeilToInt(damageDone * 0.75f), 0, 99), false);
            yield return EventControl.halfsec;
            StartStylishTimer(3, 15);
            entity.animstate = (int)MainManager.Animations.WeakBattleIdle;
            yield return EventControl.halfsec;
            yield return WaitStylish(0);
        }

        IEnumerator DoPlayerThrowable(EntityControl entity)
        {
            battle.nonphyscal = true;
            battle.dontusecharge = true;
            int target = battle.target;

            int selectedItem = battle.selecteditem;
            if (selectedItem == (int)NewItem.WebWad)
            {
                MainManager.SetCondition(BattleCondition.Sticky, ref MainManager.instance.playerdata[battle.currentturn], 2);
                MainManager.PlayParticle("StickyGet", entity.transform.position + Vector3.up);
                MainManager.PlaySound("AhoneynationSpit", -1, 0.8f, 1f);
            }

            yield return ThrowItem(entity, battle, (MainManager.Items)selectedItem, battle.itemarea, true, true);
            int damage;
            switch (selectedItem)
            {
                case (int)NewItem.MysteryBomb:
                    for (int i = 0; i < battle.enemydata.Length; i++)
                    {
                        if (battle.enemydata[i].hp > 0 && battle.enemydata[i].position != BattlePosition.Underground)
                        {
                            battle.DoDamage(null, ref battle.enemydata[i], 6, null, null, false);
                            DoMysteryEffect(ref battle.enemydata[i], 2);
                        }
                    }
                    MainManager.PlayParticle("explosion", Vector3.right * 2);
                    MainManager.PlaySound("Explosion");
                    MainManager.ShakeScreen(Vector3.one * 0.1f, 0.15f);
                    break;

                case (int)NewItem.InkBomb:
                    Vector3 position = Vector3.zero;
                    MainManager.PlayParticle("InkGet", position).transform.localScale = Vector3.one * 1.5f;
                    GameObject partcle = MainManager.PlayParticle("impactsmoke", "BubbleBurst", position);
                    partcle.transform.localScale = Vector3.one * 2f;
                    var main = partcle.GetComponent<ParticleSystem>().main;
                    main.startColor = new Color(1f, 0, 1f);

                    damage = 5;
                    if (MainManager.BadgeIsEquipped((int)MainManager.BadgeTypes.BombPlus))
                        damage += 2;

                    for (int i = 0; i < battle.enemydata.Length; i++)
                    {
                        if (battle.enemydata[i].hp > 0 && battle.enemydata[i].position != BattlePosition.Underground)
                        {
                            battle.DoDamage(null, ref battle.enemydata[i], damage, AttackProperty.Pierce, null, false);
                            MainManager.SetCondition(BattleCondition.Inked, ref battle.enemydata[i], 4);
                        }
                    }
                    MainManager.PlaySound("WaterSplash2", -1, 0.8f, 1f);
                    for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                    {
                        if (MainManager.instance.playerdata[i].hp > 0)
                        {
                            MainManager.SetCondition(BattleCondition.Inked, ref MainManager.instance.playerdata[i], 4);
                        }
                    }
                    MainManager.PlaySound("Explosion");
                    MainManager.ShakeScreen(Vector3.one * 0.1f, 0.15f);
                    break;
                case (int)NewItem.WebWad:
                    battle.DoDamage(null, ref battle.enemydata[target], 1, null, null, false);

                    if (battle.enemydata[target].hp > 0)
                    {
                        MainManager.SetCondition(BattleCondition.Sticky, ref battle.enemydata[target], 4);
                        MainManager.PlayParticle("StickyGet", battle.enemydata[target].battleentity.transform.position + Vector3.up);
                    }
                    MainManager.PlaySound("AhoneynationSpit", -1, 0.8f, 1f);
                    break;

                case (int)NewItem.SquashSeed:
                case (int)NewItem.SucculentSeed:
                    damage = battle.DoDamage(null, ref battle.enemydata[target], 4, null, null, false);
                    if (damage > 0)
                    {
                        if (selectedItem == (int)NewItem.SucculentSeed)
                            battle.Heal(ref MainManager.instance.playerdata[battle.currentturn], damage);

                        if (selectedItem == (int)NewItem.SquashSeed)
                            battle.HealTP(damage);
                    }
                    break;

                case (int)NewItem.BeeBattery:
                    battle.DoDamage(null, ref battle.enemydata[target], 1, AttackProperty.Numb, null, false);
                    break;

                case (int)NewItem.MysterySeed:
                    battle.DoDamage(null, ref battle.enemydata[target], 4, null, null, false);
                    DoMysteryEffect(ref battle.enemydata[target], 3);
                    break;
            }
            yield return EventControl.halfsec;
        }

        IEnumerator DoInkTrap(bool usedByEnemy, EntityControl entity, MainManager.BattleData summonedBy)
        {
            battle.dontusecharge = true;
            Vector3 startPos = entity.transform.position;

            GameObject inkTrap = Instantiate(Resources.Load("Prefabs/Objects/BombCart"), entity.transform.position + new Vector3(0f, -2f), Quaternion.identity, battle.battlemap.transform) as GameObject;
            SpriteRenderer bombSprite = inkTrap.transform.GetChild(0).GetComponent<SpriteRenderer>();
            bombSprite.sprite = MainManager.itemsprites[0, (int)NewItem.InkBomb];

            if (!usedByEnemy)
            {
                bombSprite.flipX = true;
                inkTrap.GetComponent<SpriteRenderer>().flipX = true;
            }
            int randomMult = UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
            Vector3 randompos = startPos + new Vector3(1, 0, -2) * randomMult;
            entity.MoveTowards(randompos, 2f);
            MainManager.SetCamera(randompos, 0.035f);
            while (entity.forcemove)
            {
                yield return null;
            }
            yield return EventControl.halfsec;

            if (!usedByEnemy)
            {
                switch (summonedBy.trueid)
                {
                    case 0:
                        entity.animstate = (int)MainManager.Animations.Happy;
                        break;
                    case 1:
                        entity.animstate = 105;
                        break;
                    case 2:
                        entity.animstate = 102;
                        break;
                }
            }
            MainManager.PlaySound("Dig2", -1, 1.1f, 1f);

            Vector3 start = new Vector3(0.5f, 0.85f, -0.1f);

            yield return LerpPosition(30f, start + entity.transform.position, new Vector3(-0.25f, -0.25f, -0.1f) + entity.transform.position, inkTrap.transform);

            GameObject part = MainManager.PlayParticle("Digging", inkTrap.transform.position, -1f);
            yield return EventControl.halfsec;
            float a = 0f;
            float b = 10f;
            do
            {
                inkTrap.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b + 1f);
            part.transform.parent = inkTrap.transform;
            part.transform.localScale = Vector3.one;
            int inkTrapDamage = 5;
            if (!usedByEnemy)
            {
                AddDelProjsPlayer(inkTrap, DelProjType.InkTrap, battle.target, inkTrapDamage, 1, 0, AttackProperty.Ink, 35f, summonedBy, "WaterSplash2", "InkGet", "Digging@Down");
                var (data, type) = delProjsPlayer[delProjsPlayer.Count - 1];
                data.args = "move,0,-0.5,0@noshadow@partoff,0,0.5,0@partoff,0,1,0";
                delProjsPlayer[delProjsPlayer.Count - 1] = (data, type);
            }
            else
            {
                battle.AddDelayedProjectile(inkTrap, battle.playertargetID, inkTrapDamage, 1, 0, AttackProperty.Ink, 35f, summonedBy, "WaterSplash2", "InkGet", "Digging@Down");
                battle.delprojs[battle.delprojs.Length - 1].args = "move,0,-0.5,0@noshadow@partoff,0,0.5,0@partoff,0,1,0";
            }
            yield return EventControl.halfsec;
        }

        IEnumerator DoStickyBomb(bool usedByEnemy, EntityControl entity, MainManager.BattleData summonedBy)
        {
            battle.dontusecharge = true;
            yield return EventControl.tenthsec;
            entity.animstate = 28;
            MainManager.PlaySound("Toss");
            yield return EventControl.tenthsec;

            Vector3 itemPos;

            if (usedByEnemy)
            {
                itemPos = entity.transform.position + new Vector3(-0.5f, 2f, -0.1f) + Vector3.up * entity.height;
                MainManager.SetCondition(MainManager.BattleCondition.Sticky, ref battle.enemydata[actionID], 2);
            }
            else
            {
                itemPos = entity.transform.position + MainManager.instance.playerdata[battle.currentturn].cursoroffset - Vector3.up;
                MainManager.SetCondition(MainManager.BattleCondition.Sticky, ref MainManager.instance.playerdata[battle.currentturn], 2);
            }
            MainManager.PlayParticle("StickyGet", entity.transform.position + Vector3.up);
            MainManager.PlaySound("AhoneynationSpit", -1, 0.8f, 1f);
            SpriteRenderer stickyBomb = MainManager.NewSpriteObject(itemPos, null, MainManager.itemsprites[0, (int)NewItem.StickyBomb]);

            Vector3 startPos = stickyBomb.transform.position;
            Vector3 targetpos;
            if (usedByEnemy)
            {
                targetpos = battle.partymiddle;
            }
            else
            {
                int target = battle.target;
                var targetEntity = battle.enemydata[target].battleentity;
                targetpos = targetEntity.transform.position + battle.enemydata[target].cursoroffset + new Vector3(0f, targetEntity.height - 1f);
            }
            Vector3 endPos = new Vector3(targetpos.x, 0.4f, targetpos.z - 0.1f);

            float a = 0f;
            float b = 35f;
            do
            {
                stickyBomb.transform.position = MainManager.BeizierCurve3(startPos, endPos, 5, a / b);
                stickyBomb.transform.eulerAngles += new Vector3(0f, 0f, -MainManager.framestep * 20f);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);

            stickyBomb.transform.position = endPos;
            MainManager.PlayParticle("StickyGet", endPos);
            MainManager.PlaySound("AhoneynationSpit", -1, 0.8f, 1f);

            int stickyBombDamage = 6;
            if (!usedByEnemy && MainManager.BadgeIsEquipped((int)MainManager.BadgeTypes.BombPlus))
                stickyBombDamage += 2;

            int areaDamage = 4;
            if (!usedByEnemy)
            {
                AddDelProjsPlayer(stickyBomb.gameObject, DelProjType.StickyBomb, battle.target, stickyBombDamage, 1, areaDamage, AttackProperty.Sticky, 35f, summonedBy, "AhoneynationSpit", "explosion", "Explosion");
                var (data, type) = delProjsPlayer[delProjsPlayer.Count - 1];
                data.args = "move,0,-0.5,0@noshadow@partoff,0,0.5,0@partoff,0,1,0";
                delProjsPlayer[delProjsPlayer.Count - 1] = (data, type);
            }
            else
            {
                battle.AddDelayedProjectile(stickyBomb.gameObject, 0, stickyBombDamage, 1, areaDamage + 2, AttackProperty.Sticky, 35f, summonedBy, "AhoneynationSpit", "explosion", "Explosion");
                battle.delprojs[battle.delprojs.Length - 1].args = "move,0,-0.5,0@noshadow@partoff,0,0.5,0@partoff,0,1,0";
            }
        }

        IEnumerator DoFlameBomb(EntityControl entity, bool usedByEnemy)
        {
            battle.dontusecharge = true;
            yield return EventControl.tenthsec;
            entity.animstate = 28;
            MainManager.PlaySound("Toss");
            yield return EventControl.tenthsec;

            Vector3 itemPos;

            if (usedByEnemy)
            {
                itemPos = entity.transform.position + new Vector3(-0.5f, 2f, -0.1f) + Vector3.up * entity.height;
            }
            else
            {
                itemPos = entity.transform.position + MainManager.instance.playerdata[battle.currentturn].cursoroffset - Vector3.up;
            }
            SpriteRenderer flameBomb = MainManager.NewSpriteObject(itemPos, null, MainManager.itemsprites[0, (int)NewItem.FlameBomb]);
            GameObject flamePart = MainManager.PlayParticle("Flame", flameBomb.transform.position);
            flamePart.transform.parent = flameBomb.transform;

            Vector3 startPos = flameBomb.transform.position;
            Vector3 endPos = new Vector3(0, 0.4f, 0 - 0.1f);

            float a = 0f;
            float b = 35f;
            do
            {
                flameBomb.transform.position = MainManager.BeizierCurve3(startPos, endPos, 5, a / b);
                flameBomb.transform.eulerAngles += new Vector3(0f, 0f, -MainManager.framestep * 20f);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);
            flameBomb.transform.position = endPos;

            entity.animstate = 0;

            int amount = 4;
            int damage = 3;

            if (!usedByEnemy && MainManager.BadgeIsEquipped((int)MainManager.BadgeTypes.BombPlus))
                damage += 2;

            Transform[] fireballs = new Transform[2];

            for (int i = 0; i < amount; i++)
            {
                if (MainManager.GetAlivePlayerAmmount() == 0 || battle.AliveEnemies() == 0)
                    break;
                MainManager.PlayParticle("explosion", endPos);
                MainManager.PlaySound("Explosion");

                for (int j = 0; j < fireballs.Length; j++)
                {
                    fireballs[j] = (Instantiate(Resources.Load("Prefabs/Particles/Fireball"), endPos, Quaternion.identity, battle.battlemap.transform) as GameObject).transform;
                    MainManager.PlaySound("WaspKingMFireball1");
                    int target = -1;
                    if (j == 0)
                    {
                        battle.GetSingleTarget();
                        target = battle.playertargetID;
                    }
                    else
                    {
                        int[] ids = battle.enemydata.Select((e, index) => (e.hp > 0 && e.position != BattlePosition.Underground) ? index : -1)
                        .Where(index => index != -1).ToArray();
                        if (ids.Length > 0)
                            target = ids[UnityEngine.Random.Range(0, ids.Length)];
                    }

                    if (target != -1)
                        StartCoroutine(DoFireballProj(target, fireballs[j], j == 1, damage));
                    else
                        Destroy(fireballs[j].gameObject);
                }
                yield return EventControl.halfsec;
                yield return EventControl.quartersec;
            }

            if (battle.enemy)
            {
                FixEnemyDiedOnItemUse();
            }
            Destroy(flameBomb.gameObject);
        }

        void FixEnemyDiedOnItemUse()
        {
            bool enemyDied = false;
            for (int i = 0; i < battle.enemydata.Length; i++)
            {
                if (battle.enemydata[i].hp <= 0)
                {
                    enemyDied = true;
                }
            }

            if (battle.enemydata[actionID].hitaction)
            {
                battle.enemy = false;
            }

            if (enemyDied)
            {
                //abomb user did not die
                if (battle.enemydata[actionID].hp > 0)
                {
                    //im not sure what to do here, the main problem is reorg getting called in endenemyturn, im scared to remove it so
                    //im just doing a fake endenemyturn after the abomb use without the reorg
                    //best solution is probably adding a if in endenemyturn to not reorg if an enemy died here.
                    if (!MainManager.BadgeIsEquipped(11) && !battle.enemydata[actionID].hitaction && !battle.enemydata[actionID].notired)
                    {
                        battle.enemydata[actionID].tired++;
                    }
                    if (!battle.enemydata[actionID].hitaction)
                    {
                        battle.enemydata[actionID].cantmove++;
                    }

                    battle.enemydata[actionID].hitaction = false;
                    battle.enemydata[actionID].blockTimes = 0;
                    battle.RefreshAllData();
                }
                battle.selfsacrifice = true;
            }
            battle.enemydata[actionID].hitaction = false;

            //this is to prevent all enemies from not attacking if its a firststrike and an enemy gets his hitaction activated by abomb
            if (battle.firststrike)
            {
                for (int i = 0; i < battle.enemydata.Length; i++)
                {
                    battle.enemydata[i].hitaction = false;
                }
            }
        }

        IEnumerator DoFireballProj(int targetId, Transform fireball, bool targetIsEnemy, int damage)
        {
            EntityControl entity = targetIsEnemy ? battle.enemydata[targetId].battleentity : MainManager.instance.playerdata[targetId].battleentity;

            yield return MainManager.ArcMovement(fireball.gameObject, fireball.position, entity.transform.position + Vector3.up * entity.height, new Vector3(0, 0, 20), 10, 30, true);

            if (targetIsEnemy)
            {
                battle.DoDamage(null, ref battle.enemydata[targetId], damage, AttackProperty.Fire, false);
            }
            else
            {
                battle.DoDamage(null, ref MainManager.instance.playerdata[targetId], damage, AttackProperty.Fire, false);
            }
        }

        IEnumerator DoRainDance(EntityControl entity)
        {
            int target = battle.target;
            EntityControl friend = MainManager.instance.playerdata[target].battleentity;
            EntityControl[] bugs = { entity, friend };

            battle.dontusecharge = true;

            yield return EventControl.tenthsec;
            var data = new float[] { 6, 1f };
            MainManager.battle.StartCoroutine(battle.DoCommand(275f, ActionCommands.SequentialKeys, data));
            GameObject rainCloud = Instance.CreateRainCloud(entity.transform.position + new Vector3(-1, 7, 0), entity, 180f);
            ParticleSystem rain = rainCloud.GetComponentInChildren<ParticleSystem>();

            yield return null;

            friend.flip = false;
            friend.animstate = (int)MainManager.Animations.ItemGet;

            for (int i = 0; i < bugs.Length; i++)
            {
                bugs[i].LockRigid(true);
                bugs[i].overrideanim = true;
                bugs[i].overrridejump = true;
            }

            Coroutine[] jumpRoutines = new Coroutine[2];
            Vector3[] basePositions = { bugs[0].transform.position, bugs[1].transform.position };
            while (MainManager.battle.doingaction)
            {
                for (int i = 0; i < bugs.Length; i++)
                {
                    bugs[i].flip = !bugs[i].flip;
                    bugs[i].animstate = (int)MainManager.Animations.ItemGet;
                    if (jumpRoutines[i] == null)
                    {
                        Vector3 targetPos = UnityEngine.Random.Range(0, 2) == 0 ? Vector3.right : Vector3.left;
                        jumpRoutines[i] = StartCoroutine(DoRainJump(jumpRoutines, i, bugs[i], basePositions[i] + targetPos, 30f));
                    }
                    yield return EventControl.tenthsec;
                }
                yield return EventControl.halfsec;
            }
            for (int i = 0; i < bugs.Length; i++)
            {
                bugs[i].flip = true;
                bugs[i].animstate = (int)MainManager.Animations.ItemGet;
                StartCoroutine(DoRainJump(jumpRoutines, i, bugs[i], basePositions[i], 30f));
            }
            yield return EventControl.sec;

            for (int i = 0; i < bugs.Length; i++)
            {
                bugs[i].overrideanim = false;
                bugs[i].overrridejump = false;
                bugs[i].transform.position = basePositions[i];
                bugs[i].LockRigid(false);
                bugs[i].animstate = (int)MainManager.Animations.ItemGet;
            }

            rain.Play();
            MainManager.PlaySound("Water0", 1.2f, 0.5f);
            yield return EventControl.halfsec;

            MainManager.PlaySound("Heal");
            MainManager.PlaySound("Heal3");
            int healAmount = Mathf.FloorToInt(battle.barfill * 4) + MainManager.BadgeHowManyEquipped(74, MainManager.instance.playerdata[battle.currentturn].trueid);
            healAmount = Mathf.Clamp(healAmount, 1, 99);

            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.instance.playerdata[i].hp > 0)
                {
                    battle.Heal(ref MainManager.instance.playerdata[i], healAmount, false);
                    Instance.CureNegativeStatus(ref MainManager.instance.playerdata[i]);
                    MainManager.PlayParticle("MagicUp", MainManager.instance.playerdata[i].battleentity.transform.position);
                }
            }
            yield return EventControl.sec;
            StartStylishTimer(3, 20);
            MainManager.StopSound("Water0");
            yield return EventControl.halfsec;
            yield return DestroyRainCloud(rainCloud, entity);
            battle.MultiSkillMove(new int[] { battle.currentturn, target });
        }

        IEnumerator DoRainJump(Coroutine[] coroutines, int index, EntityControl bug, Vector3 targetPos, float time)
        {
            bug.PlaySound("Jump");
            yield return StartCoroutine(MainManager.ArcMovement(bug.gameObject, targetPos, 4, time));
            bug.spin = new Vector3(0, 20, 0);
            yield return EventControl.quartersec;
            bug.spin = Vector3.zero;
            coroutines[index] = null;
        }

        public GameObject CreateRainCloud(Vector3 position, EntityControl entity, float growTime)
        {
            GameObject rainCloud = Instantiate(MainManager_Ext.assetBundle.LoadAsset<GameObject>("Clouds"));
            rainCloud.transform.position = position;
            Sprite cloudSprite = Resources.Load<Sprite>("sprites/particles/sprite-smoke-sheet");
            foreach (Transform cloud in rainCloud.transform)
            {
                if (cloud.name == "Cloud")
                {
                    var sr = cloud.gameObject.AddComponent<SpriteRenderer>();
                    sr.sprite = cloudSprite;
                    cloud.localScale = Vector3.zero;
                    entity.StartCoroutine(GrowCloud(cloud.gameObject, growTime, Vector3.one, sr, Color.gray, true));
                }
            }
            return rainCloud;
        }

        IEnumerator GrowCloud(GameObject cloud, float endTime, Vector3 endScale, SpriteRenderer renderer, Color targetColor, bool start)
        {
            float a = 0;
            Color startColor = renderer.color;
            Vector3 startScale = cloud.transform.localScale;
            do
            {
                cloud.transform.localScale = Vector3.Lerp(startScale, endScale, a / endTime);
                renderer.color = Color.Lerp(startColor, targetColor, a / endTime);
                a += MainManager.TieFramerate(1f);
                yield return null;

            } while (a < endTime);
            if (start)
            {
                SpriteBounce sb = cloud.AddComponent<SpriteBounce>();
                sb.frequency = 0.05f;
                sb.speed = 10f;
            }
        }

        public IEnumerator DestroyRainCloud(GameObject rainCloud, EntityControl entity)
        {
            foreach (Transform cloud in rainCloud.transform)
            {
                if (cloud.name == "Cloud")
                {
                    entity.StartCoroutine(GrowCloud(cloud.gameObject, 30f, Vector3.zero, cloud.gameObject.GetComponent<SpriteRenderer>(), new Color(0.5f, 0.5f, 0.5f, 0), false));
                }
            }
            yield return EventControl.halfsec;
            Destroy(rainCloud);
        }

        IEnumerator DoPointSwap(EntityControl entity, bool usedByEnemy, int target = -1)
        {
            battle.dontusecharge = true;
            Vector3 targetPos = Vector3.zero;
            int enemyTarget = 0;
            int playerTarget = 0;

            if (usedByEnemy)
            {
                if (target == -1)
                    enemyTarget = UnityEngine.Random.Range(0, battle.enemydata.Length);
                else
                    enemyTarget = target;
                targetPos = battle.enemydata[enemyTarget].battleentity.transform.position + entity.height * Vector3.up;
            }
            else
            {
                playerTarget = battle.target;
                targetPos = MainManager.instance.playerdata[playerTarget].battleentity.transform.position;
            }
            targetPos += Vector3.up * 3;
            GameObject holder = new GameObject("holder");
            holder.transform.position = targetPos;

            GameObject hpIcon = MainManager.NewUIObject("hpIcon", holder.transform, new Vector3(1, 0, 0), new Vector3(0.5f, 0.5f), MainManager.guisprites[24]);
            GameObject tpIcon = MainManager.NewUIObject("tpIcon", holder.transform, new Vector3(-1, 0, 0), new Vector3(0.75f, 0.75f), MainManager.guisprites[28]);
            MainManager.NewUIObject("swapIcon", holder.transform, new Vector3(0, 0), new Vector3(1, 1, 1), MainManager.guisprites[(int)NewGui.PointSwap]);

            yield return EventControl.halfsec;

            MainManager.PlaySound("Spin6");
            float a = 0;
            float b = 30f;
            Vector3 startRot = holder.transform.localEulerAngles;
            do
            {
                holder.transform.localEulerAngles = Vector3.Lerp(startRot, new Vector3(0, 0, 180), a / b);
                Quaternion inverseRotation = Quaternion.Inverse(transform.rotation);
                hpIcon.transform.rotation = inverseRotation;
                tpIcon.transform.rotation = inverseRotation;
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);

            yield return EventControl.halfsec;
            Destroy(holder);
            int hpAmount;
            if (usedByEnemy)
            {
                hpAmount = battle.enemydata[enemyTarget].hp;
                battle.enemydata[enemyTarget].hp = Mathf.Clamp(MainManager.instance.tp, 1, battle.enemydata[enemyTarget].maxhp);
            }
            else
            {
                hpAmount = MainManager.instance.playerdata[playerTarget].hp;
                MainManager.instance.playerdata[playerTarget].hp = Mathf.Clamp(MainManager.instance.tp, 1, MainManager.instance.playerdata[playerTarget].maxhp);
            }
            MainManager.PlaySound("HealPing");
            MainManager.instance.tp = Mathf.Clamp(hpAmount, 1, MainManager.instance.maxtp);
            yield return EventControl.quartersec;
        }

        static IEnumerator CheckNewSkills()
        {
            if (!MainManager.battle.enemy && actionID >= 0)
            {
                EntityControl playerEntity = MainManager.instance.playerdata[MainManager.battle.currentturn].battleentity;
                switch ((NewSkill)actionID)
                {
                    case NewSkill.SleepSchedule:
                        yield return Instance.DoSleepSchedule(playerEntity);
                        break;
                    case NewSkill.VitiationLite:
                    case NewSkill.Vitiation:
                        yield return Instance.DoVitiation(playerEntity);
                        break;
                    case NewSkill.SeedlingWhistle:
                        yield return Instance.DoSeedlingStampede(false);
                        break;
                    case NewSkill.Steal:
                        yield return Instance.DoStealSkill(playerEntity);
                        break;
                    case NewSkill.Lecture:
                        yield return Instance.DoLecture(playerEntity);
                        break;
                    case NewSkill.CordycepsLeech:
                        yield return Instance.DoCordycepsLeech(playerEntity);
                        break;

                    case NewSkill.ThrowableItems:
                        yield return Instance.DoPlayerThrowable(playerEntity);
                        break;

                    case NewSkill.InkTrap:
                        yield return Instance.DoInkTrap(false, playerEntity, MainManager.instance.playerdata[MainManager.battle.currentturn]);
                        break;

                    case NewSkill.StickyBomb:
                        yield return Instance.DoStickyBomb(false, playerEntity, MainManager.instance.playerdata[MainManager.battle.currentturn]);
                        break;

                    case NewSkill.RainDance:
                        yield return Instance.DoRainDance(playerEntity);
                        break;

                    case NewSkill.PointSwap:
                        yield return Instance.DoPointSwap(playerEntity, false);
                        break;

                    case NewSkill.FlameBomb:
                        yield return Instance.DoFlameBomb(playerEntity, false);
                        break;
                }
            }
        }

        static int GetChargeAttack(MainManager.BattleData? attacker, ref MainManager.BattleData target)
        {
            if (target.battleentity.CompareTag("Player") && MainManager.BadgeIsEquipped((int)Medal.ChargeGuard, target.trueid))
            {
                if (target.charge > 0)
                {
                    MainManager.battle.StartCoroutine(battle.ItemSpinAnim(target.battleentity.transform.position + Vector3.up, MainManager.itemsprites[1, (int)Medal.ChargeGuard], true));
                }
                int charge = target.charge;
                target.charge = 0;
                return -charge;
            }

            if (attacker != null)
            {
                bool attackerIsPlayer = attacker.Value.battleentity.CompareTag("Player");

                if ((attackerIsPlayer && !CanUseCharge(attacker.Value.battleentity.battleid)) || (!attackerIsPlayer && attacker.Value.animid == (int)NewEnemies.LeafbugShaman))
                {
                    battle.dontusecharge = true;
                    return 0;
                }
            }

            return attacker.Value.charge;
        }

        static bool CheckRecharge() => CheckRecharge(MainManager.battle.currentturn);

        static bool CheckRecharge(int playerID)
        {
            if (MainManager.BadgeIsEquipped((int)Medal.Adrenaline, MainManager.instance.playerdata[playerID].trueid) && MainManager.instance.playerdata[playerID].hp <= 4)
            {
                return false;
            }

            if (MainManager.BadgeIsEquipped((int)Medal.Recharge, MainManager.instance.playerdata[playerID].trueid) && MainManager.instance.playerdata[playerID].charge > 0)
            {
                MainManager.instance.playerdata[playerID].charge--;

                if (MainManager.battle.currentturn == playerID)
                {
                    battle.dontusecharge = true;
                }
                return false;
            }
            return true;
        }


        static bool CanUseCharge(int playerID)
        {
            if (MainManager.BadgeIsEquipped((int)Medal.Recharge, MainManager.instance.playerdata[playerID].trueid) || MainManager.BadgeIsEquipped((int)Medal.ChargeGuard, MainManager.instance.playerdata[playerID].trueid))
            {
                if (MainManager.battle.currentturn == playerID)
                {
                    battle.dontusecharge = true;
                }
                return false;
            }
            return true;
        }

        static int CheckPoisonDamage(ref MainManager.BattleData target)
        {
            bool isPlayer = target.battleentity.CompareTag("Player");
            if (isPlayer)
            {
                int basePoisonDamage = 1 + MainManager.BadgeHowManyEquipped((int)MainManager.BadgeTypes.PoisonAttacker, target.trueid);
                return Mathf.Clamp(Mathf.CeilToInt((float)target.maxhp / 10f) - 1 + basePoisonDamage, basePoisonDamage, 99);
            }
            return Mathf.Clamp(Mathf.CeilToInt((float)target.maxhp / 10f) - 1, 1, 2);
        }

        static IEnumerator WaitForEnemyDrop()
        {
            battle.startdrop = true;
            while (battle.EnemyDropping())
            {
                yield return null;
            }
            battle.startdrop = false;
            if (battle.mainturn == null && battle.chompyattack == null)
            {
                battle.action = false;
            }
        }

        IEnumerator DoSeedlingStampede(bool usedByEnemy)
        {
            battle.dontusecharge = true;
            EntityControl[] seedlings = new EntityControl[40];
            Vector3 basePosition = new Vector3(usedByEnemy ? 20f : -20f, 0f);
            List<int> possibleIDs = new List<int>();
            possibleIDs.Add((int)MainManager.AnimIDs.Seedling);
            yield return null;

            switch (MainManager.map.areaid)
            {
                case MainManager.Areas.GoldenWay:
                case MainManager.Areas.GoldenSettlement:
                case MainManager.Areas.GoldenHills:
                case MainManager.Areas.ChomperCaves:
                    possibleIDs.Add((int)MainManager.AnimIDs.Acornling);
                    break;
                case MainManager.Areas.BarrenLands:
                case MainManager.Areas.TermiteCity:
                    possibleIDs.Add((int)MainManager.AnimIDs.Plumpling);
                    break;
                case MainManager.Areas.BugariaOutskirts:
                    possibleIDs.Add((int)MainManager.AnimIDs.Underling);
                    break;
                case MainManager.Areas.Desert:
                    possibleIDs.Add((int)MainManager.AnimIDs.Underling);
                    possibleIDs.Add((int)MainManager.AnimIDs.Cactus);
                    break;
                case MainManager.Areas.BanditHideout:
                case MainManager.Areas.StreamMountain:
                case MainManager.Areas.HoneyFactory:
                case MainManager.Areas.SandCastle:
                    possibleIDs.Add((int)MainManager.AnimIDs.Cactus);
                    break;
            }

            if ((NewMaps)MainManager.map.mapid == NewMaps.Pit100BaseRoom)
            {
                possibleIDs.Add((int)NewEnemies.Caveling);
                possibleIDs.Add((int)NewEnemies.Spineling);
            }

            MainManager.ShakeScreen(0.1f, -1f);
            MainManager.PlaySound("Whistle");
            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.instance.playerdata[i].hp > 0)
                {
                    MainManager.instance.playerdata[i].battleentity.overrideanim = true;
                    MainManager.instance.playerdata[i].battleentity.animstate = (int)MainManager.Animations.Surprized;
                    MainManager.instance.playerdata[i].battleentity.Emoticon(MainManager.Emoticons.Exclamation, 45);
                }
            }
            yield return EventControl.sec;
            bool gotGoldie = false;
            MainManager.PlaySound("Rumble", 0, 1.4f, 2f, true);

            for (int i = 0; i != seedlings.Length; i++)
            {
                int id;
                int goldieChance = MainManager.map.mapid == MainManager.Maps.SeedlingHaven ? 50 : 0;
                if (MainManager.BadgeIsEquipped((int)MainManager.BadgeTypes.Seedling))
                    goldieChance += 10;

                string name = "seedling" + i;
                if (UnityEngine.Random.Range(0, 200) >= goldieChance)
                {
                    id = possibleIDs[UnityEngine.Random.Range(0, possibleIDs.Count)];

                    if (id == (int)NewEnemies.Caveling)
                    {
                        name = "Caveling";
                        id = (int)MainManager.AnimIDs.Seedling;
                    }

                    if (id == (int)NewEnemies.Spineling)
                    {
                        name = "Spineling";
                        id = (int)MainManager.AnimIDs.Cactus;
                    }
                }
                else
                {
                    id = (int)MainManager.AnimIDs.GoldenSeedling;
                    gotGoldie = true;
                }

                Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-2f, 2.5f));
                Vector3 seedPos = basePosition + randomPos;
                seedlings[i] = EntityControl.CreateNewEntity(name, id - 1, seedPos);

                seedlings[i].transform.parent = MainManager.battle.battlemap.transform;
                seedlings[i].height = 0;
                seedlings[i].gameObject.layer = 9;
                seedlings[i].flip = !usedByEnemy;
                seedlings[i].alwaysflip = !usedByEnemy;

                yield return null;
                seedlings[i].MoveTowards(new Vector3(usedByEnemy ? -15f : 15f, 0f, seedPos.z), 2f, 23, 0);
            }

            EntityControl farthestSeed = null;
            if (!usedByEnemy)
                farthestSeed = seedlings.OrderByDescending(t => t.transform.position.x).FirstOrDefault();
            else
                farthestSeed = seedlings.OrderBy(t => t.transform.position.x).FirstOrDefault();

            while (seedlings.Any(s => s.forcemove))
            {
                for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                {
                    if (MainManager.instance.playerdata[i].hp > 0)
                    {
                        bool seedPassed = false;

                        if (usedByEnemy)
                        {
                            seedPassed = farthestSeed.transform.position.x <= MainManager.instance.playerdata[i].battleentity.transform.position.x;
                        }
                        else
                        {
                            seedPassed = farthestSeed.transform.position.x >= MainManager.instance.playerdata[i].battleentity.transform.position.x;
                        }

                        if (seedPassed)
                        {
                            MainManager.instance.playerdata[i].battleentity.animstate = (int)MainManager.Animations.Hurt;
                            MainManager.instance.playerdata[i].battleentity.spin = new Vector3(0, 20);
                        }
                    }
                }

                foreach (var enemy in MainManager.battle.enemydata)
                {

                    bool seedPassed = false;

                    if (usedByEnemy)
                    {
                        seedPassed = farthestSeed.transform.position.x <= enemy.battleentity.transform.position.x;
                    }
                    else
                    {
                        seedPassed = farthestSeed.transform.position.x >= enemy.battleentity.transform.position.x;
                    }

                    if (enemy.position == BattleControl.BattlePosition.Ground && seedPassed)
                    {
                        enemy.battleentity.animstate = (int)MainManager.Animations.Hurt;
                        enemy.battleentity.spin = new Vector3(0, 20);
                    }
                }
                var randomSeed = seedlings[UnityEngine.Random.Range(0, seedlings.Length - 1)];
                if (randomSeed.transform.position.y <= 0 && randomSeed.height == 0)
                {
                    randomSeed.overrridejump = true;
                    randomSeed.Jump();
                }
                yield return null;
            }
            MainManager.screenshake = Vector3.zero;
            MainManager.StopSound("Rumble");
            foreach (var seedling in seedlings)
            {
                Destroy(seedling.gameObject);
            }

            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                MainManager.instance.playerdata[i].battleentity.overrideanim = false;
                MainManager.instance.playerdata[i].battleentity.spin = Vector3.zero;
            }

            foreach (var enemy in MainManager.battle.enemydata)
            {
                enemy.battleentity.spin = Vector3.zero;
            }

            int damage = gotGoldie ? 8 : 5;

            if (!usedByEnemy)
            {
                for (int i = 0; i < MainManager.battle.enemydata.Length; i++)
                {
                    if (MainManager.battle.enemydata[i].hp > 0 && MainManager.battle.enemydata[i].position == BattleControl.BattlePosition.Ground)
                    {
                        battle.DoDamage(ref MainManager.battle.enemydata[i], damage, null);
                    }
                }
            }
            else
            {
                for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                {
                    if (MainManager.instance.playerdata[i].hp > 0)
                    {
                        battle.DoDamage(null, ref MainManager.instance.playerdata[i], damage, null, null, MainManager.battle.commandsuccess);
                    }
                }
            }

        }

        static int CheckNewItemAction()
        {
            switch (battle.selecteditem)
            {
                case (int)NewItem.SeedlingWhistle:
                    return (int)NewSkill.SeedlingWhistle;

                case (int)NewItem.WebWad:
                case (int)NewItem.InkBomb:
                case (int)NewItem.SucculentSeed:
                case (int)NewItem.SquashSeed:
                case (int)NewItem.BeeBattery:
                case (int)NewItem.MysterySeed:
                case (int)NewItem.MysteryBomb:
                    return (int)NewSkill.ThrowableItems;

                case (int)NewItem.InkTrap:
                    return (int)NewSkill.InkTrap;

                case (int)NewItem.StickyBomb:
                    return (int)NewSkill.StickyBomb;
                case (int)NewItem.PointSwap:
                    return (int)NewSkill.PointSwap;
                case (int)NewItem.FlameBomb:
                    return (int)NewSkill.FlameBomb;

            }
            return -1;
        }

        //Weak stomach and flavor charger sprite when selecting player
        static string GetItemSelectSprite()
        {
            MainManager.BattleData player = MainManager.instance.playerdata[MainManager.battle.option];
            int playerid = player.trueid;
            string text = "|center|" + player.entityname;

            if (MainManager.battle.currentchoice == BattleControl.Actions.Item)
            {
                if (MainManager.BadgeIsEquipped(24, playerid) || MainManager.BadgeIsEquipped((int)Medal.FlavorCharger, playerid))
                {
                    text += " |size,1,0.6|";
                    if (MainManager.BadgeIsEquipped(24, playerid))
                        text += "|icon,184|";
                    if (MainManager.BadgeIsEquipped((int)Medal.FlavorCharger, playerid))
                        text += $"|icon,{(int)NewGui.FlavorCharger}|";
                }
            }
            return text;
        }

        //Vitiation Stuff
        public int realDamage = 0;

        static int GetHardHitsDMG(int baseValue)
        {
            Instance.realDamage = Mathf.FloorToInt((float)Instance.realDamage * 1.25f) + 1;
            return Mathf.FloorToInt((float)baseValue * 1.25f) + 1;
        }

        static int GetHardHitsClampDMG(int baseValue)
        {
            Instance.realDamage = Mathf.Clamp(Instance.realDamage, 2, 99);
            return Mathf.Clamp(baseValue, 2, 99);
        }

        static int SetRealDamage(int value)
        {
            Instance.realDamage += value;
            return value;
        }


        static void CalculateRealDamage(MainManager.BattleData? attacker, ref MainManager.BattleData target, int basevalue, bool block, BattleControl.AttackProperty? property, ref bool weaknesshit)
        {
            if (attacker != null && attacker.Value.animid == 110)
            {
                basevalue += 2;
            }

            if (attacker != null)
            {
                basevalue -= attacker.Value.tired;
                if (!attacker.Value.battleentity.CompareTag("Player"))
                {
                    if (MainManager.instance.flags[614] && MainManager.instance.flags[88])
                    {
                        basevalue++;
                    }
                    basevalue += attacker.Value.hardatk;
                    if (MainManager.BadgeIsEquipped(30))
                    {
                        basevalue = Mathf.Clamp(basevalue, 2, 99);
                    }
                }
                basevalue += attacker.Value.charge;
            }

            //need check ooverride noicebreak
            if (MainManager.HasCondition(MainManager.BattleCondition.Freeze, target) > -1)
            {
                basevalue++;
            }
        }

        static IEnumerator CheckRevengarang()
        {
            var battle = MainManager.battle;
            if (Instance.revengarangIsActive && battle.enemydata[actionID].position != BattlePosition.Underground)
            {
                EntityControl vi = MainManager.instance.playerdata[0].battleentity;
                int baseState = vi.animstate;
                if (!battle.IsStopped(MainManager.instance.playerdata[0]))
                {
                    vi.overrideanim = true;
                    vi.animstate = 105;
                }
                MainManager.PlaySound("Woosh", 8, 1.1f, 1f, true);
                GameObject beerang = Instantiate<GameObject>(Resources.Load("Prefabs/Objects/BeerangBattle") as GameObject);
                beerang.transform.position = vi.transform.position + Vector3.up;
                Vector3 targetPos = battle.enemydata[actionID].battleentity.sprite.transform.position + Vector3.up * 0.75f;
                Vector3 start;
                float a = 0;
                int hits = MainManager.BadgeIsEquipped((int)MainManager.BadgeTypes.Beemerang2) ? 2 : 1;

                for (int i = 0; i < hits; i++)
                {
                    float b = i == 0 ? 30f : 15f;
                    a = 0;
                    start = beerang.transform.position;
                    do
                    {
                        a += MainManager.framestep;
                        beerang.transform.position = MainManager.BeizierCurve3(start, targetPos, targetPos + Vector3.up * 2.5f + Vector3.back * 3f, a / b);
                        beerang.transform.localEulerAngles = new Vector3(80f, 0f, beerang.transform.localEulerAngles.z - MainManager.framestep * 20f);
                        yield return null;
                    }
                    while (a < b);

                    battle.DoDamage(null, ref battle.enemydata[actionID], Instance.revengarangDMG, i == 0 ? AttackProperty.Pierce : AttackProperty.NoExceptions, null, false);
                    Instance.revengarangDMG = Mathf.Clamp(Instance.revengarangDMG / 2, 1, 99);
                    if (i == 0 && hits > 1)
                    {
                        a = 0;
                        b = 15f;
                        start = beerang.transform.position;
                        do
                        {
                            a += MainManager.framestep;
                            beerang.transform.position = MainManager.BeizierCurve3(start, targetPos + Vector3.up * 3f, targetPos + Vector3.up * 2f + Vector3.forward * 2f, a / b);
                            beerang.transform.localEulerAngles = new Vector3(80f, 0f, beerang.transform.localEulerAngles.z - MainManager.framestep * 20f);
                            yield return null;
                        }
                        while (a < b);
                    }

                    AudioSource audioSource = MainManager.sounds[8];
                    audioSource.pitch += 0.07f;
                }

                Destroy(beerang);
                MainManager.StopSound(8, 0.1f);
                vi.overrideanim = false;
                vi.animstate = baseState;
                yield return EventControl.halfsec;

                if (battle.enemydata[actionID].hp <= 0)
                {
                    battle.selfsacrifice = true;
                }

            }
            Instance.revengarangIsActive = false;
        }

        public void CheckStrikeBlasters(BattleControl __instance, MainManager.BattleData target, int beforeDoDamageHp)
        {
            if (MainManager.BadgeIsEquipped((int)Medal.StrikeBlaster) && target.hp == 0 && target.eventondeath == -1 && !__instance.inevent && __instance.enemydata.Length > 1)
            {
                if (!strikeBlasters.Where(s => s.entity == target.battleentity).Any())
                {
                    strikeBlasters.Add(new BattleControl_Ext.StrikeBlaster() { dmg = beforeDoDamageHp, entity = target.battleentity });
                    if (strikeBlasterManager == null)
                        strikeBlasterManager = StartCoroutine(ManageStrikeBlasters());
                }
            }
        }

        IEnumerator ManageStrikeBlasters()
        {
            for (int i = 0; i < strikeBlasters.Count; i++)
            {
                var strikeBlaster = strikeBlasters[i];
                var battle = MainManager.battle;

                yield return new WaitUntil(() => strikeBlaster.entity.dead);

                if (battle.enemydata.Length == 0)
                    break;

                yield return EventControl.halfsec;

                if (strikeBlaster.entity != null)
                {
                    MainManager.PlayParticle("explosionsmall", strikeBlaster.entity.transform.position);
                    MainManager.PlaySound("Explosion");

                    yield return EventControl.tenthsec;

                    var hitEnemies = battle.enemydata.Where(e => e.hp > 0 && e.position != BattleControl.BattlePosition.Underground).ToArray();
                    if (hitEnemies.Length > 0)
                    {
                        int rest = strikeBlaster.dmg % hitEnemies.Length;
                        int damagePerEnemy = strikeBlaster.dmg / hitEnemies.Length;
                        for (int j = 0; j < hitEnemies.Length; j++)
                        {
                            int damageAmount = damagePerEnemy;
                            int id = hitEnemies[j].battleentity.battleid;

                            if (rest > 0)
                            {
                                damageAmount++;
                                rest--;
                            }

                            if (damageAmount > 0)
                            {
                                battle.DoDamage(null, ref battle.enemydata[id], damageAmount, BattleControl.AttackProperty.None, new DamageOverride[] { DamageOverride.NoFall }, false);
                            }
                        }
                    }

                    if (battle.checkingdead != null)
                        battle.StopCoroutine(battle.checkingdead);

                    battle.StartCoroutine(battle.CheckDead());
                    yield return EventControl.quartersec;
                }
            }
            strikeBlasterManager = null;
            yield return null;
        }


        class StrikeBlaster
        {
            public int dmg;
            public EntityControl entity;
        }


        static void DoTrustFall()
        {
            var battle = MainManager.battle;
            Vector3 position = MainManager.instance.playerdata[battle.currentturn].battleentity.transform.position + new Vector3(0f, 0.5f);
            Instance.RemoveTP(-MainManager.instance.tp, position, position + Vector3.up * 2);
            MainManager.instance.tp = 0;

            Instance.trustFallTurn = battle.turns;

            battle.EndPlayerTurn();
            battle.CancelList();
        }

        static void CheckEnemyPos()
        {
            if (!MainManager.instance.inevent)
            {
                BattleControl_Ext.Instance.CheckEnemyItems();
            }
            else
            {
                if (!MainManager.instance.flags[901])
                {
                    var superbosses = MainManager_Ext.GetSuperBosses();
                    for (int i = 0; i < battle.enemydata.Length; i++)
                    {
                        if (superbosses.Contains(battle.enemydata[i].animid))
                        {
                            MainManager.instance.flags[901] = true;
                            break;
                        }
                    }
                }
            }

            //really fucking annoying but theres a light that they deactivate in the stratos delilah event before the fight
            if (MainManager.instance.flags[162])
            {
                int battleMap = battle.sdata.stage;

                if (battleMap == (int)MainManager.BattleMaps.UndergroundBar)
                {
                    MainManager.battle.battlemap.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                }

                if (battleMap == (int)MainManager.BattleMaps.FinalBoss2)
                {
                    RenderSettings.fogColor = Color.Lerp(Color.black, Color.green, 0.25f);
                }

                if (battleMap == (int)MainManager.BattleMaps.AssociationHQ)
                {
                    /*for (int i = 0; i < battle.enemydata.Length; i++)
                    {
                        battle.enemydata[i].battleentity.hologram = false;
                        battle.enemydata[i].battleentity.UpdateSpriteMat();
                    }*/
                    battle.enemydata[1].battlepos += new Vector3(0, 0, 0.85f);
                    RenderSettings.skybox = Resources.Load<Material>("materials/skybox/Black");
                }
            }

            if (MainManager.battle.enemydata.Any(e => e.animid == (int)NewEnemies.Mars) && MainManager.battle.enemydata.Length == 3)
            {
                MainManager.battle.enemydata[0].battlepos = new Vector3(0.6f, 0f, 0.35f);
                MainManager.battle.enemydata[1].battlepos = new Vector3(3.5f, 0f, 1.25f);
                MainManager.battle.enemydata[2].battlepos = new Vector3(5.4f, 0f, -0.8f);
            }

            if (MainManager.battle.enemydata.Any(e => e.animid == (int)NewEnemies.DynamoSpore) && MainManager.battle.enemydata.Length == 3)
            {
                MainManager.battle.enemydata[0].battlepos = new Vector3(0.9f, 0f, 0f);
                MainManager.battle.enemydata[1].battlepos = new Vector3(3.5f, 0f, 0.15f);
                MainManager.battle.enemydata[2].battlepos = new Vector3(6.2f, 0f, 0.3f);
            }

            if (MainManager.battle.enemydata.Any(e => e.animid == (int)NewEnemies.LeafbugShaman) && MainManager.battle.enemydata.Length == 3)
            {
                MainManager.battle.enemydata[0].battlepos = new Vector3(0.9f, 0f, 0f);
                MainManager.battle.enemydata[1].battlepos = new Vector3(4f, 0f, 0.15f);
                MainManager.battle.enemydata[2].battlepos = new Vector3(6.8f, 0f, 0.3f);
            }

            if (MainManager.battle.enemydata.Any(e => e.animid == (int)NewEnemies.Patton) && MainManager.battle.enemydata.Length == 3)
            {
                //
            }
        }


        static int CheckStartState(int original_startState)
        {
            int state = Instance.startState;
            Instance.startState = -1;

            if (battle.enemy)
            {
                if (Entity_Ext.GetEntity_Ext(battle.enemydata[battle.actionid].entity).overrideDamageAnim)
                {
                    battle.enemydata[battle.actionid].entity.overrideanim = true;
                }
            }

            return state == -1 ? original_startState : state;
        }

        public static IEnumerator LerpPosition(float endTime, Vector3 startPos, Vector3 endPos, Transform obj)
        {
            yield return LerpStuff(endTime, startPos, endPos, obj, (startPosition, endPosition, transform, time, end) =>
            {
                obj.position = Vector3.Lerp(startPos, endPos, time / endTime);
            });
            obj.position = endPos;
        }

        public static IEnumerator LerpScale(float endTime, Vector3 startScale, Vector3 endScale, Transform obj)
        {
            yield return LerpStuff(endTime, startScale, endScale, obj, (startSize, endSize, transform, time, end) =>
            {
                transform.localScale = Vector3.Lerp(startSize, endSize, time / end);
            });
        }

        public static IEnumerator LerpStuff(float endTime, Vector3 startPos, Vector3 endPos, Transform obj, Action<Vector3, Vector3, Transform, float, float> func)
        {
            float a = 0;
            do
            {
                func(startPos, endPos, obj, a, endTime);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < endTime);
        }


        static void StartStylishTimer(float startFrames, float endFrames, int stylishID = 0, bool commandSuccess = true)
        {
            MainManager.battle.StartCoroutine(Instance.StylishTimer(startFrames, endFrames, stylishID, commandSuccess));
        }

        IEnumerator StylishTimer(float startFrames, float endFrames, int stylishID, bool commandSuccess)
        {
            if (commandSuccess && !MainManager.battle.commandsuccess)
            {
                yield break;
            }

            float a = 0f;
            Instance.failedStylish = false;

            EntityControl entity = Instance.entityAttacking;
            if (MainManager.battle.chompyattack != null)
            {
                entity = battle.chompy;
            }

            do
            {
                if (a >= 1)
                {
                    if (a < startFrames)
                    {
                        if (MainManager.GetKey(4, false))
                        {
                            Instance.failedStylish = true;
                            break;
                        }
                    }
                    else
                    {
                        if (MainManager.BadgeIsEquipped((int)Medal.TimingTutor))
                        {
                            entity.Emoticon(MainManager.Emoticons.Exclamation);
                        }

                        if (MainManager.GetKey(4, false))
                        {
                            Instance.failedStylish = false;
                            entity.Emoticon(MainManager.Emoticons.None);
                            yield return MainManager.battle.StartCoroutine(DoStylish(stylishID));
                            break;
                        }
                    }
                }
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < endFrames);
            entity.Emoticon(MainManager.Emoticons.None);
        }

        IStylish GetStylishType(int animid, int stylishID)
        {
            if (actionID == 5 || actionID == 26 || actionID == 27 || actionID == 31 || actionID == 46 || actionID == (int)NewSkill.RainDance)
                return new TeamStylish();

            switch (animid)
            {
                case 0:
                    return new ViStylish();
                case 1:
                    return new KabbuStylish();
                case 2:
                    return new LeifStylish();
            }

            return null;
        }

        IEnumerator DoStylish(int stylishID)
        {
            Instance.inStylish = true;

            if (MainManager.battle.chompyattack != null)
            {
                yield return DoChompyStylish();
            }
            else
            {
                yield return GetStylishType(Instance.entityAttacking.animid, stylishID).DoStylish(actionID, stylishID);
            }
            Instance.inStylish = false;
        }



        IEnumerator DoChompyStylish()
        {
            EntityControl chompy = battle.chompy;
            StylishUtils.ShowStylish(1.2f, chompy);
            chompy.overrideflip = false;
            chompy.rigid.useGravity = true;
            chompy.animstate = (int)MainManager.Animations.Happy;
            chompy.Jump();
            chompy.spin = new Vector3(0, 20, 0);

            while (!chompy.onground)
            {
                yield return null;
            }
            yield return EventControl.halfsec;
            chompy.spin = Vector3.zero;
        }


        static IEnumerator WaitStylish(float waitTime)
        {
            if (waitTime > 0)
                yield return new WaitForSeconds(waitTime);

            if (!Instance.failedStylish)
            {
                yield return new WaitUntil(() => !Instance.inStylish);
            }

            Instance.failedStylish = false;
        }

        public IEnumerator ShowStylishMessage(EntityControl entity, Vector3? off = null)
        {
            var battle = MainManager.battle;

            Vector3 up = Vector3.up * (Instance.entityAttacking == null ? 0 : Instance.entityAttacking.height);
            Vector3 offset = new Vector3(1f, 1.5f) + up + Vector3.forward * 10f;

            if (off != null)
                offset += off.Value;

            SpriteRenderer stylyshWord = MainManager.NewUIObject("word", battle.battlemap.transform, entity.transform.position + offset).AddComponent<SpriteRenderer>();
            stylyshWord.material.renderQueue = 50000;
            stylyshWord.transform.localScale = Vector3.zero;
            DialogueAnim dialogueAnim = stylyshWord.gameObject.AddComponent<DialogueAnim>();
            dialogueAnim.targetscale = Vector3.one * 0.5f;
            stylyshWord.sprite = MainManager_Ext.assetBundle.LoadAsset<Sprite>("Stylish");

            if (MainManager.BadgeIsEquipped(76))
            {
                stylyshWord.sprite = MainManager.battlemessage[3];
            }
            yield return EventControl.sec;
            dialogueAnim.targetscale = new Vector3(0.5f, 0f, 0.5f);
            dialogueAnim.shrink = true;
            Destroy(stylyshWord.gameObject, 1f);
            yield break;
        }

        static bool CheckTutorialStylish()
        {
            return Instance.inStylishTutorial;
        }

        public IEnumerator DoStylishTutorial(IStylish stylish)
        {
            ButtonSprite button = new GameObject().AddComponent<ButtonSprite>().SetUp(4, -1, "", new Vector3(0f, -3f, 10f), Vector3.one, 1, MainManager.GUICamera.transform);
            MainManager.battle.DestroyHelpBox();
            yield return EventControl.tenthsec;
            while (!MainManager.GetKey(4))
            {
                if (button.basesprite != null)
                {
                    button.basesprite.color = Mathf.Sin(Time.time * 10f) * 10f > 0f ? Color.white : Color.gray;
                }
                yield return null;
            }
            Destroy(button.gameObject);
            Instance.inStylish = true;
            battle.StartCoroutine(WaitTutorialStylish(stylish));
        }

        IEnumerator WaitTutorialStylish(IStylish stylish)
        {
            yield return stylish.DoStylish(-1, 0);
            Instance.inStylish = false;
        }

        static bool CanReUseItem()
        {
            return MainManager.instance.items[0].Count < MainManager.instance.maxitems;
        }

        public int CheckKineticEnergy(MainManager.BattleData attacker)
        {
            if (MainManager.BadgeIsEquipped((int)Medal.KineticEnergy, attacker.trueid))
            {
                int moves = -1 * attacker.cantmove + 1;
                return Math.Abs(moves / 3);
            }
            return 0;
        }

        public int CheckTeamGleam()
        {
            if (MainManager.BadgeIsEquipped((int)Medal.TeamGleam) && MainManager.instance.tp == MainManager.instance.maxtp)
                return 1;
            return 0;
        }

        public int CheckOddWarrior(BattleControl __instance, MainManager.BattleData attacker)
        {
            if (MainManager.BadgeIsEquipped((int)Medal.OddWarrior, attacker.trueid))
            {
                return (battle.turns + 1) % 2 == 0 ? -1 : 1;
            }
            return 0;
        }

        public int CalculateCleanseDamage(MainManager.BattleData target)
        {
            int liquidateMultiplier = 0;
            int infiniteStatus = 0;

            List<int> unclearables = new List<int>()
            {
                (int)MainManager.BattleCondition.EventStop,(int)MainManager.BattleCondition.Eaten,(int)MainManager.BattleCondition.Flipped,
                (int)MainManager.BattleCondition.Taunted, (int)MainManager.BattleCondition.Sturdy
            };

            if (MainManager.BadgeIsEquipped((int)Medal.PermanentInk))
            {
                unclearables.Add((int)MainManager.BattleCondition.Inked);
            }

            if (MainManager.BadgeIsEquipped((int)Medal.SturdyStrands))
            {
                unclearables.Add((int)MainManager.BattleCondition.Sticky);
            }

            int statusAmount = 0;
            foreach (var condition in target.condition)
            {
                if (condition[1] > 999)
                {
                    infiniteStatus++;
                    continue;
                }
                else
                {
                    if (!unclearables.Contains(condition[0]))
                    {
                        liquidateMultiplier += condition[1];
                    }
                }

                if (!unclearables.Contains(condition[0]))
                {
                    statusAmount++;
                }
            }

            if (target.charge > 0)
                statusAmount++;

            damageDeepCleanse = statusAmount * DAMAGE_DEEPCLEANSE;
            tpRegenCleanse = statusAmount * TP_REGEN_CLEANSE;
            if (MainManager.BadgeIsEquipped((int)Medal.Liquidate))
            {
                damageDeepCleanse = (DAMAGE_DEEPCLEANSE - 1) * liquidateMultiplier;
                tpRegenCleanse = (TP_REGEN_CLEANSE - 1) * liquidateMultiplier;
            }
            return statusAmount;
        }

        public void DealCleanseDamage(BattleControl __instance, ref MainManager.BattleData target)
        {
            if (MainManager.BadgeIsEquipped((int)Medal.DeepCleaning) && damageDeepCleanse > 0)
            {
                int id = battle.GetEnemyID(target.battleentity.transform);
                battle.DoDamage(ref __instance.enemydata[id], damageDeepCleanse, BattleControl.AttackProperty.Pierce);
            }

            if (MainManager.BadgeIsEquipped((int)Medal.RinseRegen) && tpRegenCleanse > 0)
            {
                var player = MainManager.instance.playerdata[__instance.currentturn];

                MainManager.PlaySound("Heal2");
                MainManager.instance.tp = Mathf.Clamp(MainManager.instance.tp + tpRegenCleanse, 0, MainManager.instance.maxtp);
                battle.ShowDamageCounter(2, tpRegenCleanse, player.battleentity.transform.position + player.cursoroffset + Vector3.up, player.battleentity.transform.position + player.cursoroffset + Vector3.up * 2);
            }
        }

        public IEnumerator ResetHoloID(BattleControl __instance)
        {
            holoSkillID = -1;
            if (oldAnimID != -1)
            {
                EntityControl playerEntity = MainManager.instance.playerdata[__instance.currentturn].battleentity;
                playerEntity.spin = new Vector3(0, 30, 0);
                yield return EventControl.quartersec;
                playerEntity.animid = oldAnimID;
                playerEntity.animstate = playerEntity.basestate;
                if (!MainManager.BadgeIsEquipped((int)MainManager.BadgeTypes.HoloCloak))
                {
                    playerEntity.hologram = false;
                    playerEntity.UpdateSpriteMat();
                }
                oldAnimID = -1;
                yield return EventControl.tenthsec;
                playerEntity.spin = Vector3.zero;
            }
        }

        static IEnumerator CheckNewEventDialogue(int id)
        {
            BattleControl battle = MainManager.battle;

            switch (id)
            {
                case (int)NewEventDialogue.MarsDeath:
                    int marsIndex = battle.EnemyInField((int)NewEnemies.Mars);

                    battle.enemydata[marsIndex].eventondeath = -1;
                    for (int i = 0; i < battle.enemydata.Length; i++)
                    {
                        if (i != marsIndex && battle.enemydata[i].battleentity.deathcoroutine == null)
                        {
                            battle.enemydata[i].battleentity.StartDeath();
                        }
                        battle.enemydata[i].hp = 0;
                    }

                    if (!battle.alreadyending)
                    {
                        battle.EndBattleWon(true, null);
                    }
                    yield return null;
                    break;

                case (int)NewEventDialogue.JesterSpitout:

                    int jesterId = battle.EnemyInField((int)NewEnemies.Jester);
                    bool dead = battle.enemydata[jesterId].hp <= 0;

                    if (dead)
                    {
                        battle.enemydata[jesterId].eventondeath = -1;
                    }

                    if (battle.enemydata[jesterId].ate != null)
                    {
                        yield return JesterAI.DoBugBullseye(battle.enemydata[jesterId].battleentity, jesterId, true);
                    }

                    if (dead)
                    {
                        battle.enemydata[jesterId].battleentity.iskill = true;
                        battle.enemydata[jesterId].battleentity.dead = true;
                        battle.enemydata[jesterId].battleentity.StartDeath();
                        yield return EventControl.sec;
                        yield return EventControl.halfsec;
                        if (!battle.alreadyending)
                        {
                            battle.EndBattleWon(true, null);
                        }
                    }
                    break;


                case (int)NewEventDialogue.PattonDeath:
                    EntityControl patton = battle.enemydata[battle.EnemyInField((int)NewEnemies.Patton)].battleentity;
                    patton.BreakIce();
                    patton.spin = new Vector3(0, 0, 20);
                    MainManager.PlaySound("ChargeDown2");
                    yield return patton.SlowSpinStop(new Vector3(0, 15), 60);

                    MainManager.PlaySound("Death3");
                    patton.spin = Vector3.zero;
                    patton.spritetransform.localEulerAngles = new Vector3(0, 0, -90);
                    patton.LockRigid(true);
                    patton.transform.localPosition += new Vector3(-1, 0.5f);

                    if (battle.AliveEnemies() == 0)
                        battle.EndBattleWon(true, null);
                    break;

                case (int)NewEventDialogue.StylishTutorial:
                    yield return Instance.DoStylishTutorialEvent();
                    break;
            }
        }

        IEnumerator DoStylishTutorialEvent()
        {
            battle.StartCoroutine(MainManager.SetText(MainManager.commondialogue[203], true, Vector3.zero, MainManager.instance.playerdata[1].battleentity.transform, null));
            yield return new WaitUntil(() => !MainManager.instance.message);


            //wants to do the stylish tutorial
            if (MainManager.instance.option == 0)
            {
                MainManager.instance.flagvar[11] = 4;
                Instance.inStylishTutorial = true;
                battle.StartCoroutine(MainManager.SetText(MainManager.commondialogue[204], true, Vector3.zero, MainManager.instance.playerdata[1].battleentity.transform, null));
                yield return new WaitUntil(() => !MainManager.instance.message);

                battle.demomode = true;
                battle.target = 0;
                battle.avaliabletargets = battle.enemydata;
                battle.currentturn = 0;

                //vi stylishes
                battle.StartCoroutine(battle.DoAction(MainManager.instance.playerdata[0].battleentity, -1));
                yield return new WaitUntil(() => !battle.action);

                var startAngle = MainManager.instance.playerdata[1].battleentity.spritetransform.eulerAngles;

                battle.StartCoroutine(MainManager.SetText(MainManager.commondialogue[208], true, Vector3.zero, MainManager.instance.playerdata[0].battleentity.transform, null));
                yield return new WaitUntil(() => !MainManager.instance.message);

                //kabbu stylishes
                battle.currentturn = 1;
                battle.StartCoroutine(battle.DoAction(MainManager.instance.playerdata[1].battleentity, -1));
                yield return new WaitUntil(() => !battle.action);

                MainManager.instance.playerdata[1].battleentity.spritetransform.eulerAngles = startAngle;
                MainManager.instance.playerdata[1].battleentity.animstate = (int)MainManager.Animations.BattleIdle;

                battle.StartCoroutine(MainManager.SetText(MainManager.commondialogue[205], true, Vector3.zero, MainManager.instance.playerdata[1].battleentity.transform, null));
                yield return new WaitUntil(() => !MainManager.instance.message);

                for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                {
                    MainManager.instance.playerdata[i].cantmove = 1;
                }
                battle.demomode = false;
                Instance.inStylishTutorial = false;
            }
            else
            {
                battle.StartCoroutine(MainManager.SetText(MainManager.commondialogue[210], true, Vector3.zero, MainManager.instance.playerdata[0].battleentity.transform, null));
                yield return new WaitUntil(() => !MainManager.instance.message);
            }
            MainManager.instance.flags[963] = true;
            MainManager.instance.flagvar[11] = 3;
        }

        public void CreateStylishBar()
        {
            if (stylishBarHolder == null)
            {
                stylishBarHolder = MainManager.NewUIObject("barholder", null, new Vector3(5.29f, 3.38f, 9.75f), new Vector3(1f, 1f, 1f), MainManager.guisprites[81]).GetComponent<SpriteRenderer>();
                stylishBar = MainManager.NewUIObject("bar", stylishBarHolder.transform, Vector3.zero, new Vector3(stylishBarAmount, 1f, 1f), MainManager.guisprites[82]).GetComponent<SpriteRenderer>();

                if (stylishReward == StylishReward.None)
                {
                    GetStylishReward();
                }
                else
                {
                    ChangeStylishRewardIcon();
                }
                stylishBarHolder.transform.localScale = Vector3.one * 0.35f;
                stylishBarHolder.transform.rotation = Quaternion.Euler(0, 0, 358);
                stylishBar.sortingOrder = 1;
                stylishBar.color = Color.yellow;
                stylishBarHolder.transform.parent = MainManager.instance.hud[0];
                stylishBarHolder.transform.localPosition = new Vector3(12.29f, -0.92f, -0.25f);
            }
        }

        IEnumerator StylishStarMovement(GameObject star, float amount)
        {
            yield return StartCoroutine(MainManager.ArcMovement(star, star.transform.position, stylishBar.transform.position + new Vector3(1, 0.5f), new Vector3(0, 0, 15), 5, UnityEngine.Random.Range(15, 30f), true));
            float a = 0f;
            float b = 20f;
            stylishBarAmount = Mathf.Clamp(amount + stylishBarAmount, 0, 1);
            do
            {
                stylishBar.transform.localScale = new Vector3(Mathf.Lerp(stylishBar.transform.localScale.x, stylishBarAmount, a / b), 1f, 1f);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);

            if (stylishBarAmount == 1)
            {
                stylishBar.color = Color.green;
            }
        }

        public IEnumerator IncreaseStylishBar(float amount, EntityControl entity)
        {
            SpriteRenderer[] stars = new SpriteRenderer[(int)(amount * 100)];
            float amountPerStar = amount / stars.Length;
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i] = new GameObject().AddComponent<SpriteRenderer>();
                stars[i].transform.position = entity.transform.position + MainManager.RandomVector(0.5f, 0.5f, 0.5f);
                stars[i].sprite = MainManager.guisprites[100];
                stars[i].material = MainManager.spritemat;
                stars[i].material.renderQueue = 50000;
                stars[i].gameObject.layer = 14;
                stars[i].material.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.65f);
                stars[i].transform.localScale = Vector3.one * 0.45f;
                StartCoroutine(StylishStarMovement(stars[i].gameObject, amountPerStar));
                yield return null;
            }
        }

        void GetStylishReward()
        {
            Dictionary<StylishReward, int> rewards = new Dictionary<StylishReward, int>()
            {
                { StylishReward.HPRegen, 3},
                { StylishReward.TPRegen, 3},
                { StylishReward.Buff, 1},
                { StylishReward.Debuff, 1},
                { StylishReward.Berries, 1}
            };
            stylishReward = MainManager_Ext.GetWeightedResult(rewards);
            ChangeStylishRewardIcon();
        }

        void ChangeStylishRewardIcon()
        {
            if (rewardIcon != null)
            {
                Destroy(rewardIcon.gameObject);
            }

            Dictionary<StylishReward, Sprite> icons = new Dictionary<StylishReward, Sprite>()
            {
                { StylishReward.HPRegen, MainManager.guisprites[120]},
                { StylishReward.TPRegen, MainManager.guisprites[119]},
                { StylishReward.Buff, MainManager.guisprites[(int)NewGui.BuffStylish]},
                { StylishReward.Debuff, MainManager.guisprites[(int)NewGui.DebuffStylish]},
                { StylishReward.Berries, MainManager.guisprites[29]}
            };

            Vector3 scale = Vector3.one * 1.5f;

            if (stylishReward == StylishReward.Buff || stylishReward == StylishReward.Debuff)
                scale = Vector3.one * 2;

            rewardIcon = MainManager.NewUIObject("icon", stylishBarHolder.transform, new Vector3(8.8f, 0, -0.1f), scale, icons[stylishReward]).GetComponent<SpriteRenderer>();
            rewardIcon.sortingOrder = 2;
        }

        IEnumerator DoStylishReward()
        {
            stylishBarAmount = 0;
            switch (stylishReward)
            {
                case StylishReward.HPRegen:
                    for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                    {
                        if (MainManager.instance.playerdata[i].hp > 0)
                            battle.Heal(ref MainManager.instance.playerdata[i], 1, false);
                    }
                    break;
                case StylishReward.TPRegen:
                    battle.HealTP(3);
                    break;
                case StylishReward.Buff:
                    GetRandomPlayerBuff();
                    break;
                case StylishReward.Debuff:
                    GetRandomEnemyDebuff();
                    break;
                case StylishReward.Berries:

                    for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                    {
                        if (MainManager.instance.playerdata[i].hp > 0)
                        {
                            yield return DoFallingObject(MainManager.instance.playerdata[i].battleentity, MainManager.itemsprites[0, (int)MainManager.Items.MoneyBig]);
                            MainManager.PlaySound("Money");
                            MainManager.instance.money = Mathf.Clamp(MainManager.instance.money + 10, 0, 999);
                            MainManager.instance.showmoney = 250f;
                            break;
                        }
                    }
                    break;
            }
            stylishBar.transform.localScale = new Vector3(0, 1, 1);
            stylishBar.color = Color.yellow;
            GetStylishReward();

            yield return EventControl.halfsec;
        }

        IEnumerator DoFallingObject(EntityControl target, Sprite objectSprite)
        {
            SpriteRenderer fallingObject = new GameObject().AddComponent<SpriteRenderer>();
            fallingObject.transform.position = target.transform.position + Vector3.up * 10;
            fallingObject.sprite = objectSprite;
            fallingObject.material = MainManager.spritemat;
            fallingObject.material.renderQueue = 50000;
            fallingObject.gameObject.layer = 14;
            fallingObject.gameObject.AddComponent<SpinAround>().itself = new Vector3(0, 0, 15);
            yield return LerpPosition(30, fallingObject.transform.position, target.transform.position + target.height * Vector3.up, fallingObject.transform);
            Destroy(fallingObject.gameObject);
        }

        void GetRandomEnemyDebuff()
        {
            MainManager.BattleCondition[] debuffs = { BattleCondition.AttackDown, BattleCondition.DefenseDown };
            int[] enemyIndexes = battle.enemydata.Select((e, index) => new { e.hp, index })
            .Where(x => x.hp > 0)
            .Select(x => x.index)
            .ToArray();

            if (enemyIndexes.Length > 0)
            {
                int enemyID = enemyIndexes[UnityEngine.Random.Range(0, enemyIndexes.Length)];
                MainManager.BattleCondition debuff = debuffs[UnityEngine.Random.Range(0, debuffs.Length)];
                int arrow = debuff == BattleCondition.AttackDown ? 2 : 3;
                MainManager.SetCondition(debuff, ref battle.enemydata[enemyID], 2);
                MainManager.PlaySound("StatDown");
                battle.StartCoroutine(battle.StatEffect(battle.enemydata[enemyID].battleentity, arrow));
            }
        }

        void GetRandomPlayerBuff()
        {
            MainManager.BattleCondition[] buffs = { BattleCondition.AttackUp, BattleCondition.DefenseUp };
            int[] partyIndexes = MainManager.instance.playerdata.Select((p, index) => new { p.hp, index })
            .Where(x => x.hp > 0)
            .Select(x => x.index)
            .ToArray();

            int partyId = partyIndexes[UnityEngine.Random.Range(0, partyIndexes.Length)];
            MainManager.BattleCondition buff = buffs[UnityEngine.Random.Range(0, buffs.Length)];
            MainManager.SetCondition(buff, ref MainManager.instance.playerdata[partyId], 2);
            MainManager.PlaySound("StatUp");
            battle.StartCoroutine(battle.StatEffect(MainManager.instance.playerdata[partyId].battleentity, buff == BattleCondition.AttackUp ? 0 : 1));
        }

        public void HideStylishBar()
        {
            stylishBarHolder.gameObject.SetActive(false);
        }

        public bool GetTauntedBy(ref int __result, BattleControl __instance)
        {
            if (BattleControl_Ext.actionID >= 0 && __instance.enemy && BattleControl_Ext.actionID < __instance.enemydata.Length)
            {
                var entityExt = Entity_Ext.GetEntity_Ext(__instance.enemydata[BattleControl_Ext.actionID].battleentity);
                if (entityExt.tauntedBy != -1)
                {
                    __result = entityExt.tauntedBy;
                    return false;
                }
            }
            return true;
        }

        public void RemoveTP(int tp, Vector3 startPos, Vector3 endPos)
        {
            battle.ShowDamageCounter(3, Mathf.Abs(tp), startPos, endPos);
            MainManager.instance.tp = Mathf.Clamp(MainManager.instance.tp + tp, 0, 99);
            MainManager.PlaySound("Heal2", -1, 0.5f, 1f, false);
        }

        public bool IsStatusImmune(MainManager.BattleData target, MainManager.BattleCondition condition)
        {
            MainManager.BattleCondition[] conditions = new MainManager.BattleCondition[] { MainManager.BattleCondition.Poison, MainManager.BattleCondition.Fire, MainManager.BattleCondition.Freeze, MainManager.BattleCondition.Numb, MainManager.BattleCondition.Sleep, MainManager.BattleCondition.Taunted, MainManager.BattleCondition.DefenseDown, MainManager.BattleCondition.AttackDown, MainManager.BattleCondition.Inked, MainManager.BattleCondition.Sticky };

            if (target.battleentity != null && !target.battleentity.playerentity)
            {
                if (MainManager.HasCondition(MainManager.BattleCondition.Sturdy, target) > -1 && conditions.Contains(condition))
                {
                    return true;
                }

                if (target.battleentity.animid == (int)NewAnimID.IronSuit && condition == BattleCondition.Fire && target.battleentity.GetComponent<IronSuit>().currentSuit == IronSuit.Suit.Heart)
                {
                    return true;
                }
            }
            return false;
        }

        public void GoToItemList()
        {
            battle.UpdateAnim();
            battle.currentaction = BattleControl.Pick.ItemList;
            battle.itemarea = AttackArea.None;
            battle.excludeself = false;
            MainManager.SetUpList(0, true, false);
            MainManager.listammount = 5;
            MainManager.ShowItemList(0, MainManager.defaultlistpos, true, false);
            battle.UpdateText();
        }

        public IEnumerator WaitForActionGourmet()
        {
            yield return new WaitUntil(() => !MainManager.battle.action);
            BattleControl_Ext.Instance.gourmetItemUse--;
            BattleControl_Ext.Instance.GoToItemList();
        }

        public void AddDelProjsPlayer(GameObject obj, DelProjType type, int targetpos, int damage, int turnstohit, int areadamage, BattleControl.AttackProperty? property, float framespeed, MainManager.BattleData summonedby, string hitsound, string hitparticle, string whilesound)
        {
            BattleControl.DelayedProjectileData delayedProjectileData = default(BattleControl.DelayedProjectileData);
            delayedProjectileData.obj = obj;
            delayedProjectileData.calledby = summonedby;
            delayedProjectileData.damage = damage;
            delayedProjectileData.turns = turnstohit + 1;
            delayedProjectileData.framestep = framespeed;
            delayedProjectileData.position = targetpos;
            delayedProjectileData.deathparticle = hitparticle;
            delayedProjectileData.deathsound = hitsound;
            delayedProjectileData.whilesound = whilesound;
            delayedProjectileData.areadamage = areadamage;
            delayedProjectileData.property = property;
            delayedProjectileData.obj.transform.parent = battle.battlemap.transform;
            delProjsPlayer.Add((delayedProjectileData, type));
        }

        IEnumerator DoDelProjPlayer()
        {
            if (delProjsPlayer.Count != 0 && MainManager.GetAlivePlayerAmmount() > 0 && battle.AliveEnemies() > 0)
            {
                bool any = false;
                battle.nonphyscal = true;

                List<(DelayedProjectileData data, DelProjType type)> projToRemove = new List<(DelayedProjectileData data, DelProjType type)>();
                for (int i = 0; i < delProjsPlayer.Count; i++)
                {
                    var (data, type) = delProjsPlayer[i];
                    data.turns -= 1;
                    delProjsPlayer[i] = (data, type);

                    if (delProjsPlayer[i].data.turns <= 0)
                    {

                        projToRemove.Add(delProjsPlayer[i]);
                        int target = delProjsPlayer[i].data.position;
                        any = true;

                        if (delProjsPlayer[i].data.whilesound != null)
                        {
                            if (delProjsPlayer[i].data.whilesound[0] == '@')
                            {
                                MainManager.PlaySound(delProjsPlayer[i].data.whilesound.Replace("@", ""), -1, 1f, 1f);
                            }
                            else
                            {
                                MainManager.PlaySound(delProjsPlayer[i].data.whilesound, -1, 1f, 1f, true);
                            }
                        }

                        if (target >= battle.enemydata.Length)
                        {
                            target = -1;
                            for (int j = 0; j < battle.enemydata.Length; j++)
                            {
                                if (battle.enemydata[j].hp > 0 && (battle.enemydata[j].position == BattlePosition.Ground || battle.enemydata[j].position == BattlePosition.Underground))
                                {
                                    target = j;
                                }
                            }
                        }

                        bool noShadow = false;
                        Vector3 offset = Vector3.up;
                        Vector3 partoffset = Vector3.zero;
                        if (delProjsPlayer[i].data.args != null)
                        {
                            string[] array2 = delProjsPlayer[i].data.args.Split(new char[] { '@' });
                            for (int j = 0; j < array2.Length; j++)
                            {
                                string[] array3 = array2[j].Split(new char[] { ',' });
                                string text = array3[0];
                                if (!(text == "partoff"))
                                {
                                    if (!(text == "move"))
                                    {
                                        if (text == "noshadow")
                                        {
                                            noShadow = true;
                                        }
                                    }
                                    else
                                    {
                                        offset = MainManager.VectorFromString(new string[]
                                        {
                                        array3[1],
                                        array3[2],
                                        array3[3]
                                        });
                                    }
                                }
                                else
                                {
                                    partoffset = MainManager.VectorFromString(new string[]
                                    {
                                    array3[1],
                                    array3[2],
                                    array3[3]
                                    });
                                }
                            }
                        }
                        if (!noShadow)
                        {
                            delProjsPlayer[i].data.obj.AddComponent<ShadowLite>();
                        }

                        float a = 0f;
                        Vector3 startPos = delProjsPlayer[i].data.obj.transform.position;

                        if (target != -1 && delProjsPlayer[i].type != DelProjType.StickyBomb)
                        {
                            do
                            {
                                delProjsPlayer[i].data.obj.transform.position = Vector3.Lerp(startPos, battle.enemydata[target].battleentity.transform.position + offset, a / delProjsPlayer[i].data.framestep);
                                a += MainManager.framestep;
                                yield return null;
                            }
                            while (a < delProjsPlayer[i].data.framestep);
                        }

                        if (delProjsPlayer[i].data.whilesound != null)
                        {
                            MainManager.StopSound(delProjsPlayer[i].data.whilesound);
                        }
                        if (delProjsPlayer[i].data.deathsound != null)
                        {
                            MainManager.PlaySound(delProjsPlayer[i].data.deathsound);
                        }
                        if (delProjsPlayer[i].data.deathparticle != null)
                        {
                            MainManager.PlayParticle(delProjsPlayer[i].data.deathparticle, delProjsPlayer[i].data.obj.transform.position + partoffset);
                        }

                        if (target != -1)
                        {
                            if (delProjsPlayer[i].type == DelProjType.InkTrap)
                            {
                                if (battle.enemydata[target].hp > 0 && (battle.enemydata[target].position == BattlePosition.Ground || battle.enemydata[target].position == BattlePosition.Underground))
                                {
                                    battle.DoDamage(null, ref battle.enemydata[target], delProjsPlayer[i].data.damage, delProjsPlayer[i].data.property, null, false);
                                }
                            }
                        }

                        if (delProjsPlayer[i].type == DelProjType.StickyBomb)
                        {
                            MainManager.ShakeScreen(Vector3.one * 0.1f, 0.15f);

                            if (delProjsPlayer[i].data.areadamage > 0)
                            {
                                for (int j = 0; j < battle.enemydata.Length; j++)
                                {
                                    EntityControl targetEntity = battle.enemydata[j].battleentity;
                                    bool isClose = MainManager.GetSqrDistance(targetEntity.transform.position + targetEntity.freezeoffset + Vector3.up * targetEntity.height, delProjsPlayer[i].data.obj.transform.position) <= 15.5f;

                                    if (isClose && battle.enemydata[j].hp > 0 && battle.enemydata[j].position != BattlePosition.Underground)
                                    {
                                        int damage = j == target ? delProjsPlayer[i].data.damage : delProjsPlayer[i].data.areadamage;
                                        battle.DoDamage(null, ref battle.enemydata[j], damage, null, null, false);
                                        MainManager.SetCondition(BattleCondition.Sticky, ref battle.enemydata[j], 4);
                                        MainManager.PlayParticle("StickyGet", battle.enemydata[j].battleentity.transform.position + Vector3.up);
                                        MainManager.PlaySound("WaterSplash2", -1, 0.8f, 1f);
                                    }
                                }
                            }
                        }

                        Destroy(delProjsPlayer[i].data.obj);
                        yield return new WaitForSeconds(0.6f);
                    }
                }

                foreach (var proj in projToRemove)
                    delProjsPlayer.Remove(proj);

                if (any)
                {
                    BattleControl.SetDefaultCamera();
                    yield return StartCoroutine(battle.CheckDead());
                    battle.action = true;

                    yield return EventControl.halfsec;
                }
            }
        }

        public void ApplyStatus(MainManager.BattleCondition condition, ref MainManager.BattleData target, int turns, string sound, float soundPitch, float soundVolume, string particle, Vector3 particlePos, Vector3 particleScale)
        {
            MainManager.SetCondition(condition, ref target, turns);
            if (sound != null)
                MainManager.PlaySound(sound, -1, soundPitch, soundVolume);

            if (particle != null)
            {
                if (particleScale == null)
                    particleScale = Vector3.one;
                MainManager.PlayParticle(particle, particlePos).transform.localScale = particleScale;
            }
        }

        public void DoSmearcharge(ref MainManager.BattleData target)
        {
            MainManager.battle.StartCoroutine(battle.ItemSpinAnim(target.battleentity.transform.position + Vector3.up, MainManager.itemsprites[1, (int)Medal.Smearcharge], true));
            int chargeBuff = MainManager.BadgeHowManyEquipped((int)Medal.Smearcharge, target.trueid);
            target.charge = Mathf.Clamp(target.charge + chargeBuff, 1, MainManager_Ext.CheckMaxCharge(target.trueid));
        }

        static IEnumerator CheckDelayedConditionsPlayer()
        {
            MainManager.BattleData target = MainManager.instance.playerdata[battle.currentturn];
            EntityControl playerEntity = MainManager.instance.playerdata[battle.currentturn].battleentity;

            if (target.delayedcondition != null && target.delayedcondition.Count > 0)
            {
                MainManager.BattleCondition[] bannedConditions = { MainManager.BattleCondition.Topple, BattleCondition.Flipped, BattleCondition.Eaten, BattleCondition.EventStop };
                foreach (var condition in target.delayedcondition)
                {
                    MainManager.BattleCondition battleCondition = (MainManager.BattleCondition)condition;

                    switch (battleCondition)
                    {
                        //technically impossible
                        case BattleCondition.Freeze:
                            playerEntity.Freeze();
                            Instance.ApplyStatus(battleCondition, ref MainManager.instance.playerdata[battle.currentturn], 1, null, 1, 1, "mothicenormal", playerEntity.transform.position + Vector3.up, Vector3.one * 1.5f);
                            break;
                        case BattleCondition.Numb:
                            Instance.ApplyStatus(battleCondition, ref MainManager.instance.playerdata[battle.currentturn], 1, "Numb", 1, 1, null, Vector3.one, Vector3.one);
                            break;

                        //technically impossible
                        case BattleCondition.Sleep:
                            Instance.ApplyStatus(battleCondition, ref MainManager.instance.playerdata[battle.currentturn], 1, "Sleep", 1, 1, null, Vector3.one, Vector3.one);
                            MainManager.DeathSmoke(playerEntity.transform.position + Vector3.up, Vector3.one * 2f);
                            MainManager.instance.playerdata[battle.currentturn].isasleep = true;
                            break;

                        case BattleCondition.AttackDown:
                        case BattleCondition.AttackUp:
                        case BattleCondition.DefenseDown:
                        case BattleCondition.DefenseUp:
                            battle.StatusEffect(MainManager.instance.playerdata[battle.currentturn], battleCondition, 1, true, false);
                            break;

                        case BattleCondition.Poison:
                            Instance.ApplyStatus(battleCondition, ref MainManager.instance.playerdata[battle.currentturn], 1, "Poison", 1, 1, "PoisonEffect", playerEntity.transform.position, Vector3.one);
                            break;

                        case BattleCondition.Fire:
                            Instance.ApplyStatus(battleCondition, ref MainManager.instance.playerdata[battle.currentturn], 1, "Flame", 1, 1, "Fire", playerEntity.transform.position, Vector3.one);
                            break;

                        case BattleCondition.Inked:
                            Instance.ApplyStatus(battleCondition, ref MainManager.instance.playerdata[battle.currentturn], 1, "WaterSplash2", 1, 1, "InkGet", playerEntity.transform.position, Vector3.one);
                            break;

                        case BattleCondition.Sticky:
                            Instance.ApplyStatus(battleCondition, ref MainManager.instance.playerdata[battle.currentturn], 1, "AhoneynationSpit", 1, 1, "StickyGet", playerEntity.transform.position, Vector3.one);
                            break;

                        default:
                            if (!bannedConditions.Contains(battleCondition))
                                MainManager.SetCondition(battleCondition, ref MainManager.instance.playerdata[battle.currentturn], 1);
                            break;
                    }
                }
                MainManager.instance.playerdata[battle.currentturn].delayedcondition = null;
                yield return EventControl.halfsec;
            }
        }

        public void DoLoomLegsCheck(ref MainManager.BattleData target, bool superguarded)
        {
            if (MainManager.BadgeIsEquipped((int)Medal.Loomlegs))
            {
                if (superguarded)
                {
                    loomLegProgress++;
                    if (loomLegProgress >= 3)
                    {
                        MainManager.battle.StartCoroutine(battle.ItemSpinAnim(target.battleentity.transform.position + Vector3.up, MainManager.itemsprites[1, (int)Medal.Loomlegs], true));
                        if (MainManager.HasCondition(MainManager.BattleCondition.Sturdy, target) == -1)
                        {
                            ApplyStatus(BattleCondition.Sticky, ref target, 3, "AhoneynationSpit", 1, 1, "StickyGet", target.battleentity.transform.position, Vector3.one);
                        }
                        loomLegProgress = 0;
                    }
                }
            }
        }

        public void DoHoneyWebCheck(ref MainManager.BattleData target, bool superguarded)
        {
            if (MainManager.BadgeIsEquipped((int)Medal.Honeyweb, target.trueid) && superguarded)
            {
                battle.StartCoroutine(battle.ItemSpinAnim(target.battleentity.transform.position + Vector3.up, MainManager.itemsprites[1, (int)Medal.Honeyweb], true));
                MainManager.PlaySound("Heal2");
                int tpRegen = 1;
                tpRegen += MainManager.BadgeHowManyEquipped((int)BadgeTypes.SuperBlock, target.trueid);
                MainManager.instance.tp = Mathf.Clamp(MainManager.instance.tp + tpRegen, 0, MainManager.instance.maxtp);
                battle.ShowDamageCounter(2, tpRegen, target.battleentity.transform.position + target.cursoroffset + Vector3.up, target.battleentity.transform.position + target.cursoroffset + Vector3.up * 2);
            }
        }

        public void DoPurifyingPulseCheck(ref MainManager.BattleData target, int conditionCleared)
        {
            if (MainManager.BadgeIsEquipped((int)Medal.PurifyingPulse, target.trueid))
            {
                battle.StartCoroutine(battle.ItemSpinAnim(target.battleentity.transform.position + Vector3.up, MainManager.itemsprites[1, (int)Medal.PurifyingPulse], false));
                battle.Heal(ref target, 2 * conditionCleared, false);
            }
        }

        public void DoRevitalizingRippleCheck(ref MainManager.BattleData target, int conditionCleared)
        {
            if (MainManager.BadgeIsEquipped((int)Medal.RevitalizingRipple, target.trueid))
            {
                battle.StartCoroutine(battle.ItemSpinAnim(target.battleentity.transform.position + Vector3.up, MainManager.itemsprites[1, (int)Medal.RevitalizingRipple], false));
                MainManager.PlaySound("StatUp", -1, 0.9f, 1f);
                battle.StartCoroutine(battle.StatEffect(target.battleentity, 4));
                target.charge = Mathf.Clamp(target.charge + conditionCleared, 1, MainManager_Ext.CheckMaxCharge(target.trueid));
            }
        }

        public void CheckHDWGHConditionAmount(MainManager.BattleData player, Entity_Ext entity_Ext)
        {
            int otherConditions = 0;

            if (player.charge > 0)
                otherConditions++;

            if (player.moreturnnextturn > 0)
                otherConditions++;

            if (entity_Ext.slugskinActive)
                otherConditions++;

            if (MainManager.HasCondition(MainManager.BattleCondition.Shield, player) == -1 && entity_Ext.vitiation)
                otherConditions++;

            if (player.condition.Count + otherConditions > MainManager.instance.flagvar[(int)NewFlagVar.MaxConditions])
            {
                MainManager.instance.flagvar[(int)NewFlagVar.MaxConditions] = player.condition.Count + otherConditions;

                if (MainManager.instance.flagvar[(int)NewFlagVar.MaxConditions] >= MainManager_Ext.HDWGH_CONDITIONS)
                {
                    MainManager.UpdateJounal(MainManager.Library.Logbook, (int)NewAchievement.HDWGH);
                }
            }
        }

        public void CheckEntitiesSprites()
        {
            int entityCount = battle.enemydata.Length + MainManager.instance.playerdata.Length;

            if (entityCount != entity_Exts.Count)
            {
                entity_Exts.Clear();
                for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                {
                    if (MainManager.instance.playerdata[i].battleentity != null)
                        entity_Exts.Add(Entity_Ext.GetEntity_Ext(MainManager.instance.playerdata[i].battleentity));
                }

                for (int i = 0; i < battle.enemydata.Length; i++)
                {
                    if (battle.enemydata[i].battleentity != null)
                        entity_Exts.Add(Entity_Ext.GetEntity_Ext(battle.enemydata[i].battleentity));
                }
            }


            for (int i = 0; i < entity_Exts.Count; i++)
            {
                if (entity_Exts[i].entity != null && entity_Exts[i].entity.model)
                {
                    entity_Exts[i].UpdateModelSprite();
                }
            }
        }

        public void DoFakeDamage(int targetId, int damage)
        {
            MainManager.PlaySound("Damage0", -1, 0.8f, 0.5f);
            battle.enemydata[targetId].hp = Mathf.Clamp(battle.enemydata[targetId].hp - damage, 1, battle.enemydata[targetId].maxhp);
            Vector3 startPos = battle.enemydata[targetId].battleentity.transform.position + battle.enemydata[targetId].cursoroffset - new Vector3(0, 0.5f, 0);
            battle.ShowDamageCounter(0, damage, startPos, Vector3.up + Vector3.right);
        }

        public void CheckThinIce(EntityControl entity)
        {
            if (MainManager.BadgeIsEquipped((int)Medal.ThinIce, entity.battleid))
            {
                MainManager.battle.StartCoroutine(battle.ItemSpinAnim(entity.transform.position + Vector3.up, MainManager.itemsprites[1, (int)Medal.ThinIce], true));

                int heal = 1 + MainManager.BadgeHowManyEquipped((int)MainManager.BadgeTypes.HealPlus, entity.battleid);
                for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                {
                    if (i != entity.battleid && MainManager.instance.playerdata[i].hp > 0)
                    {
                        MainManager.battle.Heal(ref MainManager.instance.playerdata[i], heal);
                    }
                }
            }
        }

        bool CheckHailstorm()
        {
            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.instance.playerdata[i].hp > 0 && MainManager.HasCondition(BattleCondition.Freeze, MainManager.instance.playerdata[i]) != -1)
                    return true;
            }

            for (int i = 0; i < battle.enemydata.Length; i++)
            {
                if (battle.enemydata[i].hp > 0 && MainManager.HasCondition(BattleCondition.Freeze, battle.enemydata[i]) != -1)
                    return true;
            }

            return false;
        }

        IEnumerator DoHailStorm(bool usedByEnemy)
        {
            GameObject storm = Instantiate(MainManager_Ext.assetBundle.LoadAsset<GameObject>("Hailstorm"));
            storm.transform.position = new Vector3(usedByEnemy ? 0 : -8, 10, -2f);

            Transform[] particles = { storm.transform, storm.transform.GetChild(0) };

            foreach (var part in particles)
            {
                ParticleSystem ps = part.GetComponent<ParticleSystem>();
                var velocity = ps.velocityOverLifetime;

                var velocityMultiplier = usedByEnemy ? -1 : 1;
                velocity.x = new ParticleSystem.MinMaxCurve(velocity.x.constantMin * velocityMultiplier, velocity.x.constantMax * velocityMultiplier);

                var collision = ps.collision;
                collision.SetPlane(0, battle.battlemap.transform.GetChild(0));
            }
            storm.GetComponent<ParticleSystem>().Play();

            MainManager.PlaySound(MainManager_Ext.assetBundle.LoadAsset<AudioClip>("Snowstorm"), -1, 1.2f);
            yield return new WaitForSeconds(2.5f);

            int stormDamage = 3;

            AttackProperty property;
            if (usedByEnemy)
            {
                for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                {
                    if (MainManager.instance.playerdata[i].hp > 0 && MainManager.instance.playerdata[i].eatenby == null)
                    {
                        property = UnityEngine.Random.Range(0, 2) == 0 ? AttackProperty.None : AttackProperty.Freeze;
                        battle.DoDamage(null, ref MainManager.instance.playerdata[i], stormDamage, property, battle.commandsuccess);
                        yield return EventControl.tenthsec;
                    }
                }
            }
            else
            {
                for (int i = 0; i < battle.enemydata.Length; i++)
                {
                    if (battle.enemydata[i].hp > 0 && battle.enemydata[i].position != BattlePosition.Underground)
                    {
                        property = UnityEngine.Random.Range(0, 2) == 0 ? AttackProperty.None : AttackProperty.Freeze;
                        battle.DoDamage(null, ref battle.enemydata[i], stormDamage, property, false);
                        yield return EventControl.tenthsec;
                    }
                }
            }
            yield return EventControl.halfsec;
            Destroy(storm, 5);
        }

        static int GetMultiHitDamage(int hitCount, int index, int baseDamage)
        {
            float damageMultiplier;
            damageMultiplier = baseDamage < 4 ? 2 : 1.5f;

            int maxClamp = 99;

            int totalDamage = (int)Math.Round(baseDamage * damageMultiplier, MidpointRounding.AwayFromZero);
            int baseHit = totalDamage / hitCount;
            int remainder = totalDamage % hitCount;


            if (totalDamage <= 3)
                maxClamp = 1;
            return Mathf.Clamp(baseHit + (index < remainder ? 1 : 0), 1, maxClamp);
        }
    }
}
