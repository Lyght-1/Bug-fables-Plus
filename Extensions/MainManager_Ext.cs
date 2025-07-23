
using BFPlus.Extensions.EnemyAI;
using BFPlus.Extensions.Maps;
using HarmonyLib;
using InputIOManager;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BFPlus.Extensions
{
    public enum NewParticle
    {
        HeartHit,
        ClubHit,
        DiamondHit,
        SpadeHit,
        DiamondParticle,
        ClubParticle,
        HeartParticle,
        SpadeParticle,
        SpadeExplosion,
        SuitSphere,
        SuitHit,
        HoaxeDiamond
    }

    public enum NewCode
    {
        BIGFABLE = 831,
        EVEN=832, 
        COMMAND=841,
        SCAVENGE=843
    }
    public enum Medal
    {
        Vengeance = 91, FlavorCharger, Blightfury,ThinIce, FrostNeedles, FireNeedles, TanjyToss, KineticEnergy, PotentialEnergy,Adrenaline, BugBattery, 
        FlashFreeze, SpuderCard, HoloSkill, Powerbank,Wildfire, HeatingUp, Phoenix, FierySpirit, IgnitedMite, TeamEffort, Perkfectionist,
        Cryostatis, DestinyDream, TPComa, SleepSchedule, Yawn, SweetDreams,DeepCleaning, RinseRegen, Liquidate, LifeLust, GrumbleGravel, 
        SkippingStone, RockyRampUp,Avalanche, TeamGleam, Recharge, ViolentVitiation, ChargeGuard, TwinedFate, HPDown, Switcheroo,
        GamerFX, Nightmare, TeamCheer, OddWarrior, MPPlus, Revengarang, StrikeBlaster,NoPainNoGain,TrustFall, TimingTutor, GourmetStomach,
        InkBubble,Inkblot,Smearcharge,CatalystSpill,PermanentInk,InvisibleInk,Inkwell,WebSheet,Loomlegs,ThickSilk,SpiderBait,SturdyStrands,
        FlavorlessAdhesive,Honeyweb,Slugskin,TribalDance,PurifyingPulse,RevitalizingRipple,FieryHeart,EverlastingFlame,Mothflower,Hailstorm
    }

    public enum LeifSpell
    {
        Fortify,
        ChargeUp,
        Empower,
        Break,
        Enfeeble
    }
    public enum NewAnimID
    {
        Worm = 407,
        WormSwarm =408,
        Mars=409,
        MarsSummon=410,
        TermiteKnight=411,
        LeafbugShaman=412,
        Jester=413,
        Hoaxe,
        IronSuit,
        HoaxeMom,
        BabyHoaxe,
        FirePopper,
        AlexOld,
        HoaxeCrown
    }

    public enum NewItem
    {
        MusicPlayer = 188,
        SeedlingWhistle,
        SucculentCookies2,
        SucculentCookie1,
        GoldenLeaf,
        SquashCandy2,
        SquashCandy1,
        WingRibbon,
        MysteryPouch,
        InkBomb,
        InkTrap,
        WebWad,
        StickyBomb,
        SplotchScramble,
        MurkyPizza,
        MurkyPizza2,
        MurkyPizza1,
        InkblotGravy,
        LeafbugSkewer,
        StickySoup,
        Arachnomuffins,
        Arachnomuffins2,
        Arachnomuffins1,
        Cottoncap,
        BanditDelights,
        PointSwap,
        SilverClaw,
        SilverFuse,
        SilverFossil,
        SilverCard,
        RejuvenativePunch,
        BolsteringBlend,
        SoothingJuice,
        SucculentSeed,
        SquashberrySoda,
        SleepingSquash,
        SquashSeed,
        Napcap,
        BeeBattery,
        EnergyBar,
        SurgingSpud,
        FlameBomb,
        CleanPotato,
        WaterPitcher,
        MysterySeed,
        MysteryBomb,
        MysteryCake,
        JoltMush,
        DynamoDish,
        SilverHandle,
        TauntBerry,
        HeartyBreakfast2
    }

    public enum NewGui
    {
        FrostNeedles = 228,
        FireNeedles,
        FlavorCharger,
        DeepCleaning,
        Liquidate,
        RinseRegen,
        HoloSkill,
        HoloVi,
        HoloKabbu,
        HoloLeif,
        GrumbleGravel,
        Avalanche,
        RockyRampUp,
        SkippingStone,
        TanjyToss,
        TrustFall,
        TPLossBack,
        TPLossFront,
        DebuffStylish,
        BuffStylish,
        LoreBook,
        ItemLeaf,
        Sticky,
        PointSwap,
        SpycardsTen,
        SpycardsEleven,
        SpycardsTwelve
    }

    public enum NewSkill
    {
        HoloVi = 50,
        HoloKabbu = 51,
        HoloLeif = 52,
        SleepSchedule = 53,
        VitiationLite = 54,
        Vitiation = 55,
        Steal = 56,
        Lecture,
        CordycepsLeech,
        RainDance,
        SeedlingWhistle,
        ThrowableItems,
        InkTrap,
        StickyBomb,
        PointSwap,
        FlameBomb
    }

    public enum NewEnemies
    {
        DarkVi=116,
        DarkKabbu,
        DarkLeif,
        Caveling,
        FlyingCaveling,
        Frostfly,
        PirahnaChomp,
        Moeruki,
        Abomiberry,
        SplotchSpider,
        Worm,
        WormSwarm,
        Spineling,
        Dewling,
        FireAnt,
        Belosslow,
        DynamoSpore,
        BatteryShroom,
        DullScorp,
        IronSuit,
        Mars,
        MarsSprout,
        Levi,
        Celia,
        Mothmite,
        MarsBud,
        TermiteKnight,
        LeafbugShaman,
        Jester,
        FirePopper,
        Patton,
        LonglegsSpider
    }

    public enum NewMaps
    {
        Pit100BaseRoom = 246,
        Pit100Reward = 247,
        PitBossRoom=248,
        SeedlingMinigame=249,
        AntPalaceTrainingRoom=250,
        PowerPlantExtra=251,
        PowerPlantBigRoom=252,
        PowerPlantElecPuzzle=253,
        PowerPlantBoss=254,
        SandCastleDepths1=255,
        SandCastleDepthsWall=256,
        SandCastleDepthsIcePuzzle = 257,
        SandCastleDepthsMain=258,
        SandCastleDepthsBoss=259,
        DeepCaveEntrance=260,
        DeepCave1,
        DeepCave2,
        DeepCaveBoss,
        AbandonedTower,
        AbandonedTower1,
        AbandonedTower2,
        AbandonedTower3,
        AbandonedTowerBoss,
        AbandonedTowerCards,
        BeehiveMinigame,
        LeafbugVillage,
        LeafbugShamanHut,
        GiantLairPlayroom1,
        GiantLairPlayroom2,
        GiantLairPlayroom3,
        GiantLairPlayroomBoss
    }

    public enum NewMenuText
    {
        TextSkip=283,
        TrustFall,
        TrustFallDesc,
        ShowResistance,
        NewBattleThemes
    }

    public enum NewItemUse
    {
        MultiUse=41,
        MultiUseRandom=42,
        AddInk,
        AddInkParty,
        AddSticky,
        AddStickyParty,
        RandomBuff,
        RandomBuffParty,
        RandomDebuff,
        RandomDebuffParty,
        AddSturdy,
        AddAtkDown,
        AddDefDown,
        AddTaunt,
        AddFire,
        ChargeMax
    }

    public enum NewMusic
    {
        MarsTheme=75,
        PlusBosses,
        KabbuTheme,
        ViTheme,
        AngryViTheme,
        SadKabbuTheme,
        BattleRubberPrison,
        BattleGoldenHills,
        BattleLostSands,
        Playroom,
        BattleFactory,
        BattleOutskirts,
        DarkSnek,
        BattleMetalLake,
        BattleForsakenLands,
        BattleCaves,
        BattleSandCastle,
        BattleFarGrasslands,
        BattleSwamplands,
        BattleSnakemouthLab,
        HoaxeSadTheme
    }

    public enum NewListType
    {
        GourmetItem=37,
        MedalCategories,
        BadgeShops
    }

    public enum NewDiscoveries
    {
        PitOfTrials=50,
        SandCastleDepths,
        PowerPlantExtra,
        LushAbyss,
        IronTower,
        TrainingGrounds,
        SeedlingSnatch,
        GourmetRace,
        LeafbugVillage,
        GiantsPlayroom
    }

    public enum NewRecipes
    {
        InkBomb = 70,
        StickyBomb,
        SplotchScramble,
        MurkyPizza,
        InkblotGravy,
        StickySoup,
        Arachnomuffins,
        Cottoncap,
        RejuvenativePunch,
        BolsteringBlend,
        SoothingJuice,
        SucculentSeed,
        SquashberrySoda,
        SleepingSquash,
        SquashSeed,
        Napcap,
        EnergyBar,
        SurgingSpud,
        FlameBomb,
        CleanPotato,
        WaterPitcher,
        MysterySeed,
        MysteryBomb,
        MysteryCake,
        JoltMush,
        DynamoDish
    }

    public enum NewFlagVar
    {
        Aria_Reward=68,
        Pit_Floor,
        Seedling_Highscore,
        Gourmet_Highscore,
        TermiteKnight_Reward,
        LeafbugShaman_Reward,
        MaxConditions,
        Patton_Reward,
        Intermission5RichNPCTalked,
        PitEnemyDeadLastFloor,
        TeamCelia_Reward,
        Mars_Reward
    }

    public enum NewPrizeFlag
    {
        Aria=23,
        Termite=24,
        Shaman=25,
        Patton,
        TeamCelia,
        Mars
    }

    public enum NewQuest
    {
        TeamSnakemouth=64,
        WormInfestation,
        RoughCough,
        CaveInvestigation,
        InNeedOfTraining,
        AWorthyOpponent,
        NewAnalysis,
        ConflictingVisions,
        BountyDullScorp,
        BountyDynamoSpore,
        BountyBelosslow,
        BountyIronSuit,
        BountyJester,
        HerFavoriteSweet,
        MyNewestExperiment
    }

    public enum NewAchievement
    {
        GodofWar=30,
        UndergroundExplorer,
        HDWGH,
        SuperBug,
        OverKill,
        WellRested,
        NothingButAHoaxe
    }

    public enum NewCommand
    {
        CheckShop=219
    }

    public class MainManager_Ext : MonoBehaviour
    {
        public static AssetBundle assetBundle;
        public static AssetBundle mapPrefabs;
        public static int DiscoveriesRewardAmount = 12;
        public static int FlagNumber = 1000;
        public static int CBFlagNumber = 67;
        public static int DashFlag = 699;
        public static int FlagVarNumber = 80;
        public static Medal[] medalDupes = new Medal[] { Medal.Powerbank, Medal.HPDown, Medal.MPPlus, Medal.MPPlus, Medal.MPPlus,
            Medal.InkBubble, Medal.Smearcharge };
        public static string[] enemyData;
        public static bool fastText = false;
        public static bool showResistance = false;
        public static bool newBattleThemes = false;
        public bool guiSwapped = false;
        public static int skillListType = -1;
        public bool musicPlayer = false;
        public static int mpPlusBonus = 0;
        public const int newMaxLevel = 40;
        public static bool[] newUnlocks = new bool[Enum.GetNames(typeof(NewCode)).Length];
        public static bool newCodeUsed = false;
        public static bool inSeedlingMinigame = false;
        public static bool noJump = false;
        public int minibossAmount = -1;
        public int bossAmount = -1;
        public const int SUPERBOSS_AMOUNT = 12;
        public const int HDWGH_CONDITIONS = 10; //20 was max, nerfed cause not fun
        public int newBossMap = -1;
        List<Sprite> oldGui;
        public static AreaData backgroundData;
        static MainManager_Ext instance;
        public SavedRenderSettings savedRenderSettings;
        public static int oldOutline = -1;
        public const int MYSTERY_SHADE_PRICE = 3;
        public static MainManager_Ext Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.instance.gameObject.AddComponent<MainManager_Ext>();
                }
                return instance;
            }
        }

        public static void CheckSpuderCardEffect(Transform entity)
        {
            Transform goldStars = entity.transform.Find("GoldStars");
            if (MainManager.BadgeIsEquipped((int)Medal.SpuderCard) && !MainManager.instance.flags[916])
            {
                if (goldStars == null)
                {
                    Transform transform = (Instantiate(Resources.Load("Prefabs/Particles/GoldStars"), entity.transform.position, Quaternion.identity) as GameObject).transform;
                    transform.name = "GoldStars";
                    transform.parent = entity.transform;
                    transform.localPosition = new Vector3(0f, 0.5f, -0.2f);
                }
            }
            else
            {
                Destroy(goldStars?.gameObject);
            }
        }

        static void ResetInputCooldown() => MainManager.instance.inputcooldown = 1f;

        static void SetOptionsText(int settingType, SpriteRenderer spriteRenderer)
        {
            Dictionary<int, bool> settingsType = new Dictionary<int, bool>() 
            { 
                { (int)NewMenuText.TextSkip, fastText },
                { (int)NewMenuText.ShowResistance, showResistance },
                { (int)NewMenuText.NewBattleThemes, newBattleThemes },
            };

            if (settingsType.ContainsKey(settingType))
            {
                MainManager.instance.StartCoroutine(MainManager.SetText("|center||size,0.75|" + MainManager.menutext[settingsType[settingType] ? 38 : 39],
                    new Vector3(6.25f, -0.15f), spriteRenderer.transform));
            }
        }

        static string TestSkillIndex(int skillIndex, SpriteRenderer bar)
        {
            int skillID = MainManager.listvar[skillIndex];
            string text = " |size,0.55,0.6|";
            if (skillID == (int)MainManager.Skills.Cleanse)
            {
                if (MainManager.BadgeIsEquipped((int)Medal.DeepCleaning))
                {
                    text += $"|icon,{(int)NewGui.DeepCleaning}|";
                }

                if (MainManager.BadgeIsEquipped((int)Medal.RinseRegen))
                {
                    text += $"|icon,{(int)NewGui.RinseRegen}|";
                }

                if (MainManager.BadgeIsEquipped((int)Medal.Liquidate))
                {
                    text += $"|icon,{(int)NewGui.Liquidate}|";
                }
            }

            if (skillID == (int)NewSkill.RainDance)
            {
                if (MainManager.BadgeIsEquipped((int)MainManager.BadgeTypes.HealPlus, MainManager.instance.playerdata[MainManager.battle.currentturn].trueid))
                {
                    text += $"|icon,218|";
                }
            }

            if (skillID == (int)MainManager.Skills.PeebleToss)
            {
                text = " |size,0.48,0.53|";
                if (MainManager.BadgeIsEquipped((int)Medal.SkippingStone))
                {
                    text += $"|icon,{(int)NewGui.SkippingStone}|";
                }

                if (MainManager.BadgeIsEquipped((int)Medal.GrumbleGravel))
                {
                    text += $"|icon,{(int)NewGui.GrumbleGravel}|";
                }

                if (MainManager.BadgeIsEquipped((int)Medal.RockyRampUp))
                {
                    text += $"|icon,{(int)NewGui.RockyRampUp}|";
                }

                if (MainManager.BadgeIsEquipped((int)Medal.Avalanche))
                {
                    text += $"|icon,{(int)NewGui.Avalanche}|";
                }

                if (MainManager.BadgeIsEquipped((int)Medal.TanjyToss))
                {
                    text += $"|icon,{(int)NewGui.TanjyToss}|";
                }
            }

            if (BattleControl_Ext.Instance.holoSkillID != -1)
            {
                bar.color = Color.black;
            }

            if(skillID >= (int)NewSkill.HoloVi  && skillID <= (int)NewSkill.HoloLeif)
            {
                var guiID = 0;

                if (skillID == (int)NewSkill.HoloVi)
                    guiID = (int)NewGui.HoloVi;
                else if (skillID == (int)NewSkill.HoloKabbu)
                    guiID = (int)NewGui.HoloKabbu;
                else if (skillID == (int)NewSkill.HoloLeif)
                    guiID = (int)NewGui.HoloLeif;

                bar.color = Color.black;

                foreach(Transform child in bar.transform)
                {
                    Destroy(child.gameObject);
                }

                int destinyDreamBug = BattleControl_Ext.GetDestinyDreamBug();
                if (destinyDreamBug != -1)
                {
                    var destinyDreamIcon = bar.transform.Find("destinyDream");
                    Destroy(destinyDreamIcon.gameObject);
                }

                MainManager.NewUIObject("tp", bar.transform, new Vector3(2.25f, 0.27f), new Vector3(0.3f, 0.32f, 0.21f), MainManager.guisprites[guiID], 10).GetComponent<SpriteRenderer>();
            }

            return text;
        }

        public static string SetHoloSkillTextEffect(string text)
        {
            if (BattleControl_Ext.Instance.holoSkillID != -1)
            {
                if (string.IsNullOrEmpty(text))
                {
                    text += "|color,5||glitchy|";
                }
                else
                {
                    text += "|glitchy|";
                }
            }
            return text;
        }

        public static string[] GetNewItems(string[] items, string assetName, string delimiter, bool completeReplace, bool addEmpty)
        {
            if (MainManager_Ext.assetBundle == null)
            {
                MainManager_Ext.assetBundle = AssetBundle.LoadFromMemory(Properties.Resources.vengeance);
            }
            if (!completeReplace)
            {
                List<string> oldItems = items.ToList();
                if(assetName != "Items" && assetName != "ItemData")
                    oldItems.RemoveAll(string.IsNullOrEmpty);
                
                var newItems = assetBundle.LoadAsset<TextAsset>(assetName).ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i != newItems.Length; i++)
                {
                    if (newItems[i].Contains(delimiter))
                    {
                        string[] replacedData = newItems[i].Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
                        int replaceLine = int.Parse(replacedData[0]);
                        oldItems.RemoveAt(replaceLine);
                        oldItems.Insert(replaceLine, replacedData[1]);
                        continue;
                    }
                    oldItems.Add(newItems[i]);
                }
                if(addEmpty)
                    oldItems.Add("");
                return oldItems.ToArray();
            }

            return assetBundle.LoadAsset<TextAsset>(assetName).ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] GetNewItems(string[] items, string assetName, bool addEmpty) => GetNewItems(items, assetName, "{", false, addEmpty);

        public static void SetEnemyData()
        {
            enemyData = assetBundle.LoadAsset<TextAsset>("EnemyData").ToString().Split(new char[] { '\n' });
            MainManager.enemydata = new string[enemyData.Length, enemyData[0].Split(',').Length];
            for (int i = 0; i != enemyData.Length; i++)
            {
                var data = enemyData[i].Split(',');
                for (int j = 0; j != data.Length; j++)
                {
                    MainManager.enemydata[i, j] = data[j];
                }
            }
        }

        public static void SetQuestChecks()
        {
            var questChecksRef = typeof(MainManager).GetField("questchecks", BindingFlags.NonPublic | BindingFlags.Static);

            List<int[]> questChecks = new List<int[]>();
            var temp = assetBundle.LoadAsset<TextAsset>("QuestChecks").ToString().Split(new char[] { '\n' });

            for (int i = 0; i < temp.Length; i++)
            {
                var quest = temp[i].Split('@');
                List<int> questData = new List<int>();
                for (int j = 0; j < quest.Length; j++)
                {
                    questData.Add(Convert.ToInt32(quest[j]));
                }
                questChecks.Add(questData.ToArray());
            }

            questChecksRef.SetValue(MainManager.instance, questChecks.ToArray());
        }

        public static void SetEnemyPortrait()
        {
            List<Sprite> newsprites = new List<Sprite>(MainManager.librarysprites);
            newsprites.AddRange(assetBundle.LoadAllAssets<Sprite>().Where(sprite => sprite.name.Split('_')[0] == "EnemyPortrait").OrderBy(sprite => sprite.name.Split('_')[1]).ToArray());
            MainManager.librarysprites = newsprites.ToArray();
        }

        public static int CheckMaxCharge(int index)
        {
            return 3 + MainManager.BadgeHowManyEquipped((int)Medal.Powerbank, MainManager.instance.playerdata[index].trueid);
        }

        //need to change menutext to be loaded from textfile instead
        public static void SetMenuText()
        {
            List<string> tempList = new List<string>(MainManager.menutext);
            tempList.Add("Text Skip");
            tempList.Add($"Trust Fall |single||size,0.55,0.6||icon,{(int)NewGui.TrustFall}|");
            tempList.Add("Reduces the teams TP to 0.|line|Next turn, the strongest ally attack will recover as much TP as the damage it deals.");
            tempList.Add("Show Resistance");
            tempList.Add("New Battle Themes");
            MainManager.menutext = tempList.ToArray();

            List<int> temp = new List<int>(MainManager.settingsindex);
            temp.Add((int)NewMenuText.TextSkip);
            temp.Add((int)NewMenuText.ShowResistance);
            temp.Add((int)NewMenuText.NewBattleThemes);
            MainManager.settingsindex = temp.ToArray();
        }

        public static int FixHoloSkillID(int type)
        {
            skillListType = Mathf.Abs(type) - 1;
            if (MainManager.instance.inbattle)
            {
                return MainManager.battle.currentturn;
            }
            return skillListType;
        }


        public static void CreatePrefabParticle(GameObject gameObject, string objectPath, int maxAmount, float speed, float liveFrames, float cooldown, float shrinkSpeed, Vector3 limit, Vector3 maxSize, Vector3 childspin)
        {
            PrefabParticle particles = gameObject.AddComponent<PrefabParticle>();
            particles.prefabpart = Resources.Load(objectPath) as GameObject;
            particles.maxammount = maxAmount;
            particles.speed = speed;
            particles.liveframes = liveFrames;
            particles.cooldown = cooldown;
            particles.shrinkspeed = shrinkSpeed;
            particles.limits = limit;
            particles.maxsize = maxSize;
            particles.childspin = childspin;
        }

        static void CheckCustomMap(int id)
        {
            if(id <= Enum.GetValues(typeof(MainManager.Maps)).Cast<int>().Max())
            {
                GameObject gameObject = Instantiate(Resources.Load("Prefabs/Maps/" + (MainManager.Maps)id)) as GameObject;
                MainManager.map = gameObject.GetComponent<MapControl>();
                gameObject.name = id.ToString();
            }
            else
            {
                NewMaps newMap = (NewMaps)id;
                MapFactory.CreateMap(newMap).LoadMap(newMap);
            }
        }

        public static bool IsCustomMap()
        {
            return Enum.IsDefined(typeof(NewMaps), (int)MainManager.map.mapid);
        }

        public static bool HasCustomBattleMap()
        {
            Instance.savedRenderSettings = new SavedRenderSettings(MainManager.map.skyboxmat, MainManager.map.fogcolor, MainManager.map.fogend, MainManager.map.skycolor, MainManager.map.globallight);

            int battleMap = MainManager.battle.sdata.stage;
            if (MainManager.map.mapid == MainManager.Maps.HBsLab && battleMap != (int)MainManager.BattleMaps.HBsLab && battleMap != -1)
            {
                GameObject gameObject = Instantiate(Resources.Load("Prefabs/Maps/" + (MainManager.Maps)Instance.GetMapFromBattleMap((MainManager.BattleMaps)battleMap))) as GameObject;
                MapControl mapControl = gameObject.GetComponent<MapControl>();
                Instance.SetRenderSettingsFromMap(mapControl);
                Destroy(gameObject);
            }

            return IsCustomMap() || (MainManager.map.mapid == MainManager.Maps.HBsLab && Instance.newBossMap != -1 && IsInBoss()) || MainManager.map.mapid == MainManager.Maps.SnakemouthEmpty || MainManager.map.mapid == MainManager.Maps.GoldenSettlement3 || MainManager.map.mapid == MainManager.Maps.BarrenLandsEntrance;
        }

        public void SetRenderSettingsFromMap(MapControl mapControl)
        {
            RenderSettings.skybox = mapControl.skyboxmat;
            RenderSettings.fogColor = mapControl.fogcolor;
            RenderSettings.fogEndDistance = mapControl.fogend;
            RenderSettings.ambientSkyColor = mapControl.skycolor;
            RenderSettings.ambientLight = mapControl.globallight;
        }

        public static bool IsValidMap()
        {
            int mapID = (int)((MainManager.map.readdatafromothermap != MainManager.Maps.TestRoom) ? MainManager.map.readdatafromothermap : MainManager.map.mapid);
            return Resources.Load<TextAsset>("Data/EntityData/" + mapID) != null || IsCustomMap();
        }

        static GameObject LoadCustomBattleMap()
        {
            int mapId = (int)MainManager.map.mapid;

            if (mapId == (int)MainManager.Maps.HBsLab && Instance.newBossMap != -1 && MainManager.lastevent == 85)
            {
                mapId = Instance.newBossMap;
            }

            if (mapId == (int)MainManager.Maps.SnakemouthEmpty)
            {
                GameObject stage = Instantiate(Resources.Load("Prefabs/maps/SnakemouthEmpty")) as GameObject;
                MapControl mapControl = stage.GetComponent<MapControl>();
                Instance.SetRenderSettingsFromMap(mapControl);
                Destroy(stage.GetComponent<MapControl>());
                stage.transform.position = new Vector3(0, -2.5f, 0);
                return stage;
            }

            if (mapId == (int)MainManager.Maps.GoldenSettlement3)
            {
                GameObject stage = Instantiate(Resources.Load("Prefabs/maps/GoldenSettlement3")) as GameObject;
                MapControl mapControl = stage.GetComponent<MapControl>();
                Instance.SetRenderSettingsFromMap(mapControl);
                Destroy(stage.GetComponent<MapControl>());
                stage.transform.position = new Vector3(-1.6f, 0f, 1.1f);
                if (MainManager.instance.flags[682] && MainManager.instance.flags[555])
                {
                    var cerise = EntityControl.CreateNewEntity("cerise", 403, new Vector3(-1.8955f, 0, 3.9845f));
                    cerise.transform.parent = stage.transform;
                    cerise.animstate = 6;
                }
                return stage;
            }

            if (mapId == (int)MainManager.Maps.BarrenLandsEntrance)
            {
                GameObject stage = Instantiate(Resources.Load("Prefabs/maps/BarrenLandsEntrance")) as GameObject;
                MapControl mapControl = stage.GetComponent<MapControl>();
                Instance.SetRenderSettingsFromMap(mapControl);
                Destroy(stage.GetComponent<MapControl>());
                stage.transform.position = new Vector3(-7.25f, -0.1f, -0.061f);
                stage.transform.localEulerAngles = new Vector3(0f, 19.7419f, 0f);

                Transform pattonHouse = stage.transform.GetChild(1);
                pattonHouse.GetChild(0).gameObject.SetActive(false);
                pattonHouse.localPosition = new Vector3(4.52f, -0.04f, 3.16f);
                pattonHouse.localEulerAngles = new Vector3(270f, 338.47f, 0);
                pattonHouse.localScale = new Vector3(1.5f, 1.30f, 1.34f);

                GameObject tubeMesh = Resources.Load<GameObject>("prefabs/maps/UpperSnekMiddleRoom");
                pattonHouse.GetChild(5).GetChild(0).gameObject.GetComponent<MeshFilter>().mesh = tubeMesh.transform.GetChild(0).Find("Tube (4)").GetChild(0).GetComponent<MeshFilter>().mesh;

                return stage;
            }
            NewMaps newMap = (NewMaps)mapId;

            MainManager_Ext.LoadMapsBundle();
            GameObject battleMap = MapFactory.CreateMap(newMap).LoadBattleMap();
            UnloadMapsBundle();
            return battleMap;
        }

        public static void CreateItemEntity(int itemID, EntityControl entity, Vector3 startPos, int itemType)
        {
            Vector3 itemBounce = MainManager.RandomItemBounce(4f, 10f);
            NPCControl itemEntity = EntityControl.CreateItem(startPos + Vector3.up * 0.5f, itemType, itemID, itemBounce, 600);
            itemEntity.entity.TempIgnoreColision(entity.ccol, 5f);
            itemEntity.entity.LateVelocity(itemBounce);
        }

        public static void CreateMusicParticle()
        {
            if (MainManager.player != null && MainManager.player.transform.Find("MusicSimple(Clone)") == null)
            {
                var musicParticles = Instantiate(Resources.Load("Prefabs/Particles/MusicSimple")) as GameObject;
                musicParticles.transform.parent = MainManager.player.entity.transform;
                musicParticles.transform.localPosition = new Vector3(0f, 2f, -0.1f);
            }
        }
        
        static string[] GetEntityValues()
        {
            if (assetBundle == null)
            {
                assetBundle = AssetBundle.LoadFromMemory(Properties.Resources.vengeance);
            }
            string[] baseValues = Resources.Load<TextAsset>("Data/EntityValues").ToString().Split(new char[]{'\n'});
            string[] newValues = assetBundle.LoadAsset<TextAsset>("NewEntityValues").ToString().Replace("\r\n", "\n").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return baseValues.AddRangeToArray(newValues);
        }

        static int GetEntityValuesAmount()
        {
            return Enum.GetNames(typeof(MainManager.AnimIDs)).Length + Enum.GetNames(typeof(NewAnimID)).Length;
        }


        public void CheckSwitcheroo()
        {
            if (MainManager.player != null && MainManager.instance.badges != null && MainManager.guisprites != null && assetBundle != null && !MainManager.instance.flags[916])
            {
                if (MainManager.BadgeIsEquipped((int)Medal.Switcheroo) && !guiSwapped)
                {
                    if (oldGui == null)
                    {
                        oldGui = new List<Sprite>(MainManager.guisprites);
                    }

                    guiSwapped = true;
                    List<Sprite> altGui = assetBundle.LoadAssetWithSubAssets<Sprite>("alt_gui1").ToList();
                    altGui.AddRange(assetBundle.LoadAssetWithSubAssets<Sprite>("alt_gui2").ToList());

                    foreach (var sprite in altGui)
                    {
                        int index = int.Parse(sprite.name.Split(new char[] { '_' })[1]);
                        MainManager.guisprites[index] = sprite;
                    }
                    RefreshHudSprites();

                    foreach(var player in MainManager.instance.playerdata)
                    {
                        player.entity.SetAnimator();
                    }
                }

                if (oldGui != null && !MainManager.BadgeIsEquipped((int)Medal.Switcheroo) && guiSwapped)
                {
                    MainManager.guisprites = oldGui.ToArray();
                    RefreshHudSprites();
                    oldGui = null;
                    guiSwapped = false;
                    foreach (var player in MainManager.instance.playerdata)
                    {
                        player.entity.SetAnimator();
                    }
                }
            }
        }

        static void RefreshHudSprites()
        {
            var partyArray = MainManager.instance.playerdata.Select(p => p.animid).OrderBy(p => p).ToArray();
            for (int i = 0; i < partyArray.Length; i++)
            {
                var facesprite = MainManager.instance.hud[i].GetChild(0).Find("facesprite");
                facesprite.GetComponent<SpriteRenderer>().sprite = MainManager.guisprites[partyArray[i] + 5];
            }
        }

        static string[] GetNewFortuneTellerClues(string[] oldClues)
        {
            string[] newClues = new string[0];
            switch (MainManager.instance.option)
            {
                case 0:
                    //i apologize to the bf community for this irational sin of hardcoding this string
                    oldClues[39] = "Ah yes!, In the Sacred Hills, in a bush!";

                    newClues = assetBundle.LoadAsset<TextAsset>("FortuneTeller0").ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    break;
                case 2:
                    newClues = assetBundle.LoadAsset<TextAsset>("FortuneTeller2").ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    break;
            }
            return oldClues.AddRangeToArray(newClues);
        }

        static int[][] GetNewFortuneTellerFlags(int[][] oldFlags)
        {
            if (MainManager.instance.option == 2)
            {
                int[] newFlags = new int[]
                {
                    750,
                    751,
                    752,
                    753,
                    856,
                    755,
                    757,
                    758,
                    760,
                    761,
                    765,
                    772,
                    773,
                    767,
                    783,
                    842,
                    762, //spuder card
                    784,  //liquidate
                    889, //ink bubble
                    897, //inkblot
                    898, //smearcharge
                    884, //permanent ink
                    885, //inkwell
                    879, //web sheet
                    882, //loom legs
                    880, //sturdy strands
                    881, //honey web
                    883, //slugskin
                    964, //hailstorm
                };
                oldFlags[1] = oldFlags[1].AddRangeToArray(newFlags);
            }
            return oldFlags;
        }

        public static void CheckGamerFX(ref AudioClip soundclip)
        {
            if (soundclip != null && MainManager.basicload&& MainManager.player != null && MainManager.BadgeIsEquipped((int)Medal.GamerFX) && !MainManager.instance.flags[916])
            {
                bool replaceSound = false;
                string[] validSounds = new string[] { "Damage0", "OverworldIce", "Cut", "WoodHit", "ShieldHit" };

                foreach(var sound in validSounds)
                {
                    if (soundclip.name == sound)
                    {
                        replaceSound = true;
                        break;
                    }
                }
                if (MainManager.instance.inbattle && MainManager.battle != null && MainManager.battle.enemy)
                    replaceSound = false;

                if (replaceSound)
                {
                    string[] gamerSounds = new string[] { "MkHit", "MkHit2", "FBPoint", "FBFlower", "FBStart", "FBDeath", "MkDeath", "MKKey", "MKPotion" };
                    soundclip = Resources.Load<AudioClip>("Audio/Sounds/" + gamerSounds[UnityEngine.Random.Range(0, gamerSounds.Length)]);
                }
            }
        }

        static Sprite GetDewlingCopter(EntityControl entity)
        {
            if(entity.animid == (int)MainManager.AnimIDs.Flowering - 1)
            {
                if (IsNewEnemy(entity, NewEnemies.Dewling))
                    return assetBundle.LoadAssetWithSubAssets<Sprite>("Dewling").Where(s => s.name == "seedlingking_46").FirstOrDefault();
                else
                    return MainManager.instance.projectilepsrites[11];
            }
            return MainManager.instance.projectilepsrites[1];
        }

        static Vector3 GetFloweringCopterScale(EntityControl entity)
        {
            if (IsNewEnemy(entity, NewEnemies.Dewling))
                return Vector3.one;
            else
                return Vector3.one * 0.75f;
        }

        static Sprite[] LoadNewTitle()
        {
            if(assetBundle == null)
                assetBundle = AssetBundle.LoadFromMemory(Properties.Resources.vengeance);
            return new Sprite[] { assetBundle.LoadAsset<Sprite>("BFPlusTitle") };
        }

        static Sprite GetNewBossPortrait()
        {
            int[] enemyIds = new int[] { MainManager.listvar[MainManager.instance.option] };
            int index = 0;

            if (enemyIds[0] == (int)NewEnemies.Mars || enemyIds[0] == (int)NewEnemies.LeafbugShaman || enemyIds[0] == (int)NewEnemies.DynamoSpore || enemyIds[0] == (int)NewEnemies.Patton)
            {
                return MainManager.librarysprites[MainManager.GetEnemyPortrait(enemyIds[0])];
            }

            GetNewBossIds(ref enemyIds);
            if (enemyIds.Length == 3)
            {
                var time = Mathf.Sin(Time.time * 2.5f);
                if (time <= -0.4f)
                    index = 0;
                else if (time > -0.4 && time <= 0.4f)
                    index = 1;
                else
                    index = 2;
            }
            else if (enemyIds.Length == 2)
            {
                index = Mathf.Sin(Time.time * 2.5f) > 0 ? 0 : 1;

            }
            
            return MainManager.librarysprites[MainManager.GetEnemyPortrait(enemyIds[index])];
        }

        static void GetNewBossIds(ref int[] enemyIds)
        {
            switch (enemyIds[0])
            {
                case (int)NewEnemies.DarkVi:
                    enemyIds = new int[] { (int)NewEnemies.DarkVi, (int)NewEnemies.DarkKabbu, (int)NewEnemies.DarkLeif };
                    break;

                case (int)NewEnemies.Mars:
                    enemyIds = new int[] { (int)NewEnemies.MarsSprout, (int)NewEnemies.Mars, (int)NewEnemies.MarsSprout };
                    break;

                case (int)NewEnemies.Levi:
                    enemyIds = new int[] { (int)NewEnemies.Levi, (int)NewEnemies.Celia};
                    break;

                case (int)NewEnemies.LeafbugShaman:
                    enemyIds = new int[] { (int)MainManager.Enemies.LeafbugNinja,(int)NewEnemies.LeafbugShaman, (int)MainManager.Enemies.LeafbugArcher };
                    break;
                case (int)NewEnemies.DynamoSpore:
                    enemyIds = new int[] { (int)NewEnemies.BatteryShroom, (int)NewEnemies.DynamoSpore, (int)NewEnemies.BatteryShroom };
                    break;

                case (int)MainManager.Enemies.Stratos:
                    enemyIds = new int[] { (int)MainManager.Enemies.Stratos, (int)MainManager.Enemies.Delilah };
                    break;

                case (int)MainManager.Enemies.DeadLanderA:
                    enemyIds = new int[] { (int)MainManager.Enemies.DeadLanderA, (int)MainManager.Enemies.DeadLanderB, (int)MainManager.Enemies.DeadLanderG };
                    break;
                case (int)MainManager.Enemies.Maki:
                    enemyIds = new int[] { (int)MainManager.Enemies.Maki, (int)MainManager.Enemies.Kina, (int)MainManager.Enemies.Yin };
                    break;

                case (int)MainManager.Enemies.TermiteSoldier:
                    enemyIds = new int[] { (int)MainManager.Enemies.TermiteSoldier, (int)MainManager.Enemies.TermiteNasute};
                    break;

                case (int)NewEnemies.Patton:
                    enemyIds = new int[] { (int)NewEnemies.Abomiberry, (int)NewEnemies.LonglegsSpider, (int)NewEnemies.Patton };
                    break;
            }
        }

        static void SetNewBossEntity(int[] enemyIDs,EntityControl enemy, ref string music, ref Vector3 epos, ref int mapId)
        {
            Instance.newBossMap = -1; 
            if (MainManager.instance.flags[899])
            {
                int bossMap = Instance.GetBossMap(enemyIDs[0]);

                if (bossMap != -1)
                {
                    mapId = bossMap;
                }
                else
                {
                    Instance.newBossMap = Instance.GetNewBossMap(enemyIDs[0]);
                }
 
            }

            switch (enemyIDs[0])
            {
                case (int)MainManager.Enemies.DeadLanderA:
                    music = "Alert";
                    break;
                case (int)MainManager.Enemies.TANGYBUG:
                    music = "Battle1";
                    break;

                case (int)MainManager.Enemies.Stratos:
                case (int)MainManager.Enemies.Maki:
                    music = "Bounty";
                    break;

                case (int)NewEnemies.TermiteKnight:
                    music = "MiteKnight";
                    break;

                case (int)NewEnemies.DullScorp:
                case (int)MainManager.Enemies.LeafbugNinja:
                case (int)NewEnemies.Levi:
                case (int)NewEnemies.IronSuit:
                case (int)NewEnemies.BatteryShroom:
                case (int)NewEnemies.Belosslow:
                case (int)NewEnemies.Jester:
                case (int)NewEnemies.Abomiberry:
                    music = NewMusic.PlusBosses.ToString();
                    break;
            }

            if (enemyIDs[0] == (int)MainManager.Enemies.LeafbugNinja)
            {
                Vector3 offset = new Vector3(3f, 0, 1f);
                int animid = (int)MainManager.AnimIDs.LeafbugNinja;

                for (int i = 0; i < 2; i++)
                {
                    if (i == 1)
                    {
                        offset = new Vector3(-3f, 0, 1f);
                        animid = (int)MainManager.AnimIDs.LeafbugArcher;
                    }
                    EntityControl leafbug = EntityControl.CreateNewEntity("leafbug", animid - 1, epos + offset);
                    leafbug.transform.parent = enemy.transform;
                    leafbug.hologram = true;
                }
            }

            if (enemyIDs[0] == (int)MainManager.Enemies.DeadLanderA)
            {
                epos = new Vector3(2f, 0.75f, 11.85f);
                int animid = (int)MainManager.AnimIDs.DeadLanderB;

                for (int i = 0; i < 2; i++)
                {
                    if (i == 1)
                    {
                        animid = (int)MainManager.AnimIDs.DeadLanderC;
                    }
                    EntityControl deadlander = EntityControl.CreateNewEntity("deadlander", animid - 1, epos + new Vector3(3f + 1+ i, 0, 1f));
                    deadlander.transform.parent = enemy.transform;
                    deadlander.height = i == 0 ? 1 : 0;
                    deadlander.hologram = true;
                }
            }

            if (enemyIDs[0] == (int)MainManager.Enemies.Maki)
            {
                epos = new Vector3(2f, 0.75f, 11.85f);
                int animid = (int)MainManager.AnimIDs.Kina;

                for (int i = 0; i < 2; i++)
                {
                    if (i == 1)
                    {
                        animid = (int)MainManager.AnimIDs.YinMoth;
                    }
                    EntityControl bug = EntityControl.CreateNewEntity("bug", animid - 1, epos + new Vector3(3f + (i*2f), 0, 0f));
                    bug.transform.parent = enemy.transform;
                    bug.hologram = true;
                }
            }

            if (enemyIDs[0] == (int)MainManager.Enemies.Stratos)
            {
                Vector3 offset = new Vector3(3f, 0, 1f);
                EntityControl delilah = EntityControl.CreateNewEntity("delilah", (int)MainManager.AnimIDs.Delilah - 1, epos + offset);
                delilah.transform.parent = enemy.transform;
                delilah.hologram = true;
                delilah.animstate = (int)MainManager.Animations.BattleIdle;
            }

            if (enemyIDs[0] == (int)MainManager.Enemies.TermiteSoldier)
            {
                Vector3 offset = new Vector3(3f, 0, 1f);
                EntityControl poi = EntityControl.CreateNewEntity("delilah", (int)MainManager.AnimIDs.TermiteNasute - 1, epos + offset);
                poi.transform.parent = enemy.transform;
                poi.hologram = true;
                poi.animstate = (int)MainManager.Animations.BattleIdle;
            }

            if (Enum.IsDefined(typeof(NewEnemies), enemyIDs[0]))
            {
                enemy.name = "enemy" + enemyIDs[0];

                if (enemyIDs[0] == (int)NewEnemies.DarkVi)
                {
                    music = NewMusic.DarkSnek.ToString();
                    epos = new Vector3(2f, 0.75f, 11.85f);
                    EntityControl[] entities = new EntityControl[] { enemy, null, null };

                    for (int i = 0; i < entities.Length; i++)
                    {
                        if (i > 0)
                        {
                            Vector3 offset = new Vector3(3f, 0, 1f);
                            if (i == 2)
                            {
                                offset = new Vector3(4f, 0, 0f);
                            }

                            entities[i] = EntityControl.CreateNewEntity("enemy" + enemyIDs[i], i, epos + offset);
                            entities[i].transform.parent = enemy.transform;
                            entities[i].hologram = true;
                        }
                        entities[i].animstate = (int)MainManager.Animations.BattleIdle;
                    }
                }

                if (enemyIDs[0] == (int)NewEnemies.Levi)
                {
                    Vector3 offset = new Vector3(3f, 0, 1f);
                    EntityControl celia = EntityControl.CreateNewEntity("celia", (int)MainManager.AnimIDs.ShielderAnt - 1, epos + offset);
                    celia.transform.parent = enemy.transform;
                    celia.hologram = true;
                    celia.animstate = (int)MainManager.Animations.BattleIdle;
                }


                if (enemyIDs[0] == (int)NewEnemies.BatteryShroom)
                {
                    enemy.name = "enemy" + (int)NewEnemies.DynamoSpore;
                    for (int i = 0; i < 2; i++)
                    {
                        Vector3 offset = new Vector3(i == 0 ? -2f : 2, 0, 1f);
                        EntityControl batteryShroom = EntityControl.CreateNewEntity("enemy" + (int)NewEnemies.BatteryShroom, (int)MainManager.AnimIDs.Mushroom - 1, epos + offset);
                        batteryShroom.transform.parent = enemy.transform;
                        batteryShroom.hologram = true;
                    }
                }

                if (enemyIDs[0] == (int)NewEnemies.MarsSprout)
                {
                    music = "MarsTheme";
                    for (int i = 0; i < 2; i++)
                    {
                        Vector3 offset = new Vector3(i == 0 ? -3f : 3f, 0, 1f);
                        EntityControl bud = EntityControl.CreateNewEntity("bud", (int)NewAnimID.MarsSummon, epos + offset);
                        bud.transform.parent = enemy.transform;
                        bud.hologram = true;
                    }
                }

                if (enemyIDs[0] == (int)NewEnemies.Abomiberry)
                {
                    epos = new Vector3(5f, 0.7865f, 11.85f);
                    int animid = (int)MainManager.AnimIDs.LongLegs;

                    for (int i = 0; i < 2; i++)
                    {
                        string name = "spider";
                        if (i == 1)
                        {
                            animid = (int)MainManager.AnimIDs.Abomihoney;
                            name = NewEnemies.Abomiberry.ToString();
                        }
                        EntityControl bug = EntityControl.CreateNewEntity(name, animid - 1, epos + new Vector3(-3.5f *(1+(i*0.5f)), 0, 1f));
                        bug.transform.parent = enemy.transform;
                        bug.hologram = true;
                    }
                }
            }
        }

        static string GetBossNames(int enemyId)
        {
            switch (enemyId)
            {
                case (int)NewEnemies.DarkVi:
                    return "Dark Team Snakemouth";

                case (int)NewEnemies.Levi:
                    return "Team Celia";

                case (int)MainManager.Enemies.DeadLanderA:
                    return "Dead Lander Trio";

                case (int)MainManager.Enemies.Stratos:
                    return "Team Slacker";

                case (int)MainManager.Enemies.TermiteSoldier:
                    return "Cross and Poi";
            }
            return MainManager.enemynames[enemyId];
        }

        static int[] CheckBossList(List<int> bosslist)
        {
            bosslist.Remove(-2);
            Instance.bossAmount = bosslist.Count;
            return bosslist.ToArray();
        }

        static int[] CheckMinibossList(int[] minibosslist)
        {
            var tempList = new List<int>(minibosslist);
            tempList.AddRange(new int[] {
                (int)MainManager.Enemies.TermiteSoldier,
                (int)MainManager.Enemies.PrimalWeevil,
                (int)NewEnemies.Levi,
                (int)NewEnemies.LeafbugShaman,
                (int)NewEnemies.Patton,
                (int)MainManager.Enemies.DeadLanderA
            });
            Instance.minibossAmount = tempList.Count;
            return tempList.ToArray();
        }

        public static int[] GetSuperBosses()
        {
            return new int[] {
                (int)NewEnemies.DynamoSpore,
                (int)NewEnemies.DullScorp,
                (int)NewEnemies.Belosslow,
                (int)NewEnemies.IronSuit,
                (int)NewEnemies.Jester,
                (int)MainManager.Enemies.Maki,
                (int)MainManager.Enemies.Stratos,
                -2, //holo team snek
                (int)MainManager.Enemies.TANGYBUG,
                (int)NewEnemies.TermiteKnight,
                (int)NewEnemies.Mars,
                (int)NewEnemies.DarkVi
            };
        }

        static void CheckSuperbossList()
        {
            if (MainManager.instance.flagvar[1] == 2)
            {
                MainManager.instance.multilist = GetSuperBosses();
            }
        }

        static void CreateHeaderIcons(int challengeIndex, DialogueAnim[] saves, int saveIndex, Sprite sprite, float sizeDif, int challengeAmount, int totalChallenge)
        {
            switch (challengeIndex)
            {
                //BIGFABLE
                case 6:
                    sprite = MainManager.itemsprites[0, 142];
                    sizeDif = 0.7f;
                    break;

                //EVEN
                case 7:
                    sprite = MainManager.itemsprites[0, 102];
                    sizeDif = 0.7f;
                    break;

                //COMMAND
                case 8:
                    sprite = MainManager.guisprites[13];
                    sizeDif = 0.5f;
                    break;

                //SCAVENGE
                case 9:
                    sprite = MainManager.guisprites[22];
                    sizeDif = 0.45f;
                    break;
            }

            MainManager.NewUIObject("mode" + challengeIndex, saves[saveIndex].transform, new Vector3(5.65f, Mathf.Lerp(1f, -1.5f, (float)challengeAmount / (float)totalChallenge)), Vector3.one * sizeDif, sprite, 30 - challengeIndex + saveIndex * 5);
        }

        /// <summary>
        /// Called in Transpiler EventControl.Event8, makes new prompt with new codes for savefile.
        /// </summary>
        /// <returns></returns>
        static IEnumerator DoNewChallengesPrompt()
        {
            int newChallengesUnlocked = newUnlocks.Where(a => a).ToArray().Length;

            if (newChallengesUnlocked > 0)
            {

                int id = 0;
                NewCode[] newCodes = (NewCode[])Enum.GetValues(typeof(NewCode));
                int[] selections = new int[newChallengesUnlocked];
                bool firsttime = false;

                do
                {
                    string prompt = "|prompt,map,$0.25," + (newChallengesUnlocked + 1);
                    string a = string.Empty;
                    for (int i = 0; i < newUnlocks.Length; i++)
                    {
                        if (newUnlocks[i])
                        {
                            prompt += ",-11";
                            a += "," + "@" + newCodes[i].ToString();

                            if (!firsttime)
                            {
                                selections[id] = i;
                                id++;
                            }
                            a = a + " - " + ((!MainManager.instance.flags[(int)newCodes[i]]) ? MainManager.menutext[39] : MainManager.menutext[38]);
                        }
                    }
                    firsttime = true;
                    prompt += ",-11";
                    string text = prompt;
                    prompt = string.Concat(new string[]
                    {
                        text,
                        a,
                        ",@",
                        MainManager.menutext[42],
                        ",none|"
                    });
                    MainManager.DialogueText("|boxstyle,5||center||rainbow||font,1||spd,0||halfline|SECRET" + prompt, null, null);
                    while (MainManager.instance.message)
                    {
                        yield return null;
                    }
                    if (MainManager.instance.option < newChallengesUnlocked)
                    {
                        MainManager.instance.flags[(int)newCodes[selections[MainManager.instance.option]]] = !MainManager.instance.flags[(int)newCodes[selections[MainManager.instance.option]]];
                        yield return null;
                    }
                }
                while (MainManager.instance.option < newChallengesUnlocked);
                MainManager.PlaySound("ATKSuccess");
            }
            yield return EventControl.halfsec;
        }

        /// <summary>
        /// Called in Transpiler Event8, Checks if the filename is equals to a new custom codes and activates the proper flags.
        /// </summary>
        /// <returns></returns>
        static IEnumerator CheckNewCodeString()
        {
            NewCode[] newCodes = (NewCode[])Enum.GetValues(typeof(NewCode));
            for (int i = 0; i < newCodes.Length; i++)
            {
                if (!MainManager.instance.flags[(int)newCodes[i]] && MainManager.instance.flagstring[10] == newCodes[i].ToString())
                {
                    MainManager.PlaySound("ATKSuccess");
                    MainManager.instance.flags[(int)newCodes[i]] = true;
                    MainManager.instance.flagstring[10] = string.Empty;
                    yield return null;
                    if (!newUnlocks[i])
                    {
                        newUnlocks[i] = true;
                        InputIO.LoadSettings(true);
                    }
                    newCodeUsed = true;
                    break;
                }
            }
        }

        static bool NewCodeUsed()
        {
            if (newCodeUsed)
            {
                newCodeUsed = false;
                return true;
            }
            return false;
        }

        public static bool IsNewEnemy(EntityControl entity, NewEnemies enemyType)
        {
            return entity.name.Contains(enemyType.ToString()) || entity.name == ("enemy" + (int)enemyType);
        }

        static void SetStrategyValues()
        {
            if (MainManager.BadgeIsEquipped((int)Medal.TrustFall))
            {
                MainManager.listvar = new int[] { 0, 271, 1, 2, (int)NewMenuText.TrustFall, 3 };
            }
        }

        static int[] SetStrategyDescValues(int[] descValues)
        {
            if (MainManager.BadgeIsEquipped((int)Medal.TrustFall))
            {
                List<int> temp = descValues.ToList();
                temp.Insert(descValues.Length - 1, (int)NewMenuText.TrustFallDesc);
                return temp.ToArray();
            }
            return descValues;
        }

        static bool CheckTrustFallColor(int index)
        {
            return MainManager.BadgeIsEquipped((int)Medal.TrustFall) && index == 4 && BattleControl_Ext.Instance.trustFallTurn > -1;
        }

        static int[] SetStrategyTextValues(int[] values)
        {
            if (MainManager.BadgeIsEquipped((int)Medal.TrustFall))
            {
                List<int> temp = values.ToList();
                temp.Insert(values.Length - 1, (int)NewMenuText.TrustFall);
                return temp.ToArray();
            }
            return values;
        }

        static bool CanUseTrustFall()
        {
            return BattleControl_Ext.Instance.trustFallTurn == -1 && MainManager.instance.option == 4;
        }

        static int GetMaxMedals()
        {
            return 120 + Enum.GetValues(typeof(Medal)).Length + medalDupes.Length - 3; //Tp coma & Everlasting Flame && Pebble Toss;
        }

        static void CheckCustomAI()
        {
            //add entity ext to everyone, this is after both array are initiated with entities so its perfect right here
            foreach (var player in MainManager.instance.playerdata)
            {
                var entityExt = Entity_Ext.GetEntity_Ext(player.battleentity);
                entityExt.id = player.trueid;
                entityExt.lastHp = player.hp; //lifelust medal
                entityExt.lastTurnHp = player.hp; //determination medal
            }

            foreach (var enemy in MainManager.battle.enemydata)
            {
                var entityExt = Entity_Ext.GetEntity_Ext(enemy.battleentity);
            }

            if (MainManager.HasFollower(MainManager.AnimIDs.AntCapitain) && ((NewMaps)MainManager.map.mapid == NewMaps.Pit100BaseRoom || (NewMaps)MainManager.map.mapid == NewMaps.Pit100Reward) && MainManager.instance.flags[834] && !MainManager.instance.flags[835])
            {
                MainManager.battle.AddAI((int)MainManager.AnimIDs.AntCapitain - 1, (int)MainManager.Animations.BattleIdle);
            }
        }

        static UnityEngine.Object CheckModel(EntityControl entity,string path)
        {
            if(entity.animid == (int)NewAnimID.Mars || entity.animid==(int)NewAnimID.MarsSummon || entity.animid == (int)NewAnimID.Jester || entity.animid == (int)NewAnimID.FirePopper)
            {
                return assetBundle.LoadAsset(((NewAnimID)entity.animid).ToString());
            }

            return Resources.Load(path);
        }

        public static void AddJesterComponent(Transform obj, int linkCount, Vector3 middle, Transform parent, bool jester=true)
        {
            JesterSprings midPos = obj.gameObject.AddComponent<JesterSprings>();
            var links = new List<Transform>();

            for (int i = 0; i < linkCount; i++)
            {
                links.Add(obj.transform.GetChild(i));
            }
            //head attach
            links.Add(obj.transform.GetChild(linkCount).GetChild(jester ? 1 : 0));

            midPos.links = links.ToArray();
            midPos.middle = middle;
            midPos.localpos = false;
        }

        //temp
        static void CheckNPCEnemySound(EntityControl entity, string sound)
        {
            float volume = 1;
            if (inSeedlingMinigame)
            {
                if (sound == "Lost")
                {
                    return;
                }
                volume = 0.1f;
            }
            entity.PlaySound(sound,volume);
        }

        public static void DoNewItemUse(NewItemUse itemUse, int value, int? characterid)
        {
            switch (itemUse)
            {
                case NewItemUse.MultiUse:
                    MainManager.instance.items[0].Add(value);
                    break;
                case NewItemUse.MultiUseRandom:
                    if(UnityEngine.Random.Range(0,100) < 50)
                    {
                        MainManager.instance.items[0].Add(value);
                    }

                    break;
                case NewItemUse.AddInk:
                    if (!MainManager.instance.inbattle)
                    {
                        return;
                    }
                    MainManager.PlaySound("WaterSplash2");
                    MainManager.SetCondition(MainManager.BattleCondition.Inked, ref MainManager.instance.playerdata[characterid.Value], value);

                    break;
                case NewItemUse.AddInkParty:
                    if (!MainManager.instance.inbattle)
                    {
                        return;
                    }
                    MainManager.PlaySound("WaterSplash2");
                    for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                    {
                        if(MainManager.instance.playerdata[i].hp > 0 && MainManager.instance.playerdata[i].eatenby == null)
                            MainManager.SetCondition(MainManager.BattleCondition.Inked, ref MainManager.instance.playerdata[i], value);
                    }
                    break;
                case NewItemUse.AddSticky:
                    if (!MainManager.instance.inbattle)
                    {
                        return;
                    }
                    MainManager.PlaySound("AhoneynationSpit");
                    MainManager.SetCondition(MainManager.BattleCondition.Sticky, ref MainManager.instance.playerdata[characterid.Value], value);
                    break;
                case NewItemUse.AddStickyParty:
                    if (!MainManager.instance.inbattle)
                    {
                        return;
                    }
                    MainManager.PlaySound("AhoneynationSpit");
                    for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                    {
                        if (MainManager.instance.playerdata[i].hp > 0 && MainManager.instance.playerdata[i].eatenby == null)
                            MainManager.SetCondition(MainManager.BattleCondition.Sticky, ref MainManager.instance.playerdata[i], value);
                    }
                    break;

 
                case NewItemUse.RandomBuffParty:
                case NewItemUse.RandomBuff:
                    if (!MainManager.instance.inbattle)
                    {
                        return;
                    }
                    for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                    {
                        bool once = itemUse == NewItemUse.RandomBuff;
                        if (once)
                            i = characterid.Value;
                        if (MainManager.instance.playerdata[i].hp > 0 && MainManager.instance.playerdata[i].eatenby == null)
                        {
                            MainManager_Ext.Instance.DoRandomMysteryBuff(i, value, true);
                        }
                        if (once)
                            break;
                    }
                    break;
                case NewItemUse.RandomDebuff:
                case NewItemUse.RandomDebuffParty:

                    if (!MainManager.instance.inbattle)
                    {
                        return;
                    }
                    for(int i = 0; i < MainManager.instance.playerdata.Length; i++)
                    {
                        bool once = itemUse == NewItemUse.RandomDebuff;
                        if (once)
                            i = characterid.Value;

                        if (MainManager.instance.playerdata[i].hp > 0 && MainManager.instance.playerdata[i].eatenby == null)
                        {
                            MainManager_Ext.Instance.DoRandomMysteryDebuff(i, value, true);
                        }

                        if (once)
                            break;
                    }
                    break;

                case NewItemUse.AddDefDown:
                    if (!MainManager.instance.inbattle)
                    {
                        return;
                    }
                    MainManager.battle.StatusEffect(MainManager.instance.playerdata[characterid.Value], MainManager.BattleCondition.DefenseDown, value, true, false);
                    break;
                case NewItemUse.AddAtkDown:
                    if (!MainManager.instance.inbattle)
                    {
                        return;
                    }
                    MainManager.battle.StatusEffect(MainManager.instance.playerdata[characterid.Value], MainManager.BattleCondition.AttackDown, value, true, false);
                    break;

                case NewItemUse.AddTaunt:
                    if (!MainManager.instance.inbattle)
                    {
                        return;
                    }
                    MainManager.SetCondition(MainManager.BattleCondition.Taunted, ref MainManager.instance.playerdata[characterid.Value], value);
                    MainManager.PlaySound("Taunt");
                    break;
                case NewItemUse.AddSturdy:
                    if (!MainManager.instance.inbattle)
                    {
                        return;
                    }
                    MainManager.SetCondition(MainManager.BattleCondition.Sturdy, ref MainManager.instance.playerdata[characterid.Value], value);
                    MainManager.PlaySound("MagicUp");
                    break;

                case NewItemUse.AddFire:
                    if (!MainManager.instance.inbattle)
                    {
                        return;
                    }
                    MainManager.SetCondition(MainManager.BattleCondition.Fire, ref MainManager.instance.playerdata[characterid.Value], value);
                    MainManager.PlaySound("Flame");
                    MainManager.PlayParticle("Fire", MainManager.instance.playerdata[characterid.Value].battleentity.transform.position, 1f);
                    break;

                case NewItemUse.ChargeMax:
                    if (!MainManager.instance.inbattle)
                    {
                        return;
                    }
                    MainManager.PlaySound("StatUp");
                    MainManager.instance.playerdata[characterid.Value].charge = CheckMaxCharge(characterid.Value);
                    MainManager.battle.StartCoroutine(MainManager.battle.StatEffect(MainManager.instance.playerdata[characterid.Value].battleentity, 4));
                    break;
            }
        }

        public void DoRandomMysteryDebuff(int characterid, int value, bool player)
        {
            var usages = new MainManager.ItemUsage[]{ (MainManager.ItemUsage)NewItemUse.AddAtkDown, (MainManager.ItemUsage)NewItemUse.AddDefDown,
                        MainManager.ItemUsage.AddPoison,MainManager.ItemUsage.AddSleep, MainManager.ItemUsage.AddNumb, MainManager.ItemUsage.AddFreeze
                        ,(MainManager.ItemUsage)NewItemUse.AddTaunt,(MainManager.ItemUsage)NewItemUse.AddSticky,(MainManager.ItemUsage)NewItemUse.AddInk,
                        (MainManager.ItemUsage)NewItemUse.AddFire
                    };
            Instance.DoRandomItemEffect(usages, characterid, value, player);
        }

        public void DoRandomMysteryBuff(int characterid, int value, bool player)
        {
            MainManager.ItemUsage[] usages = { MainManager.ItemUsage.AtkUpStat, MainManager.ItemUsage.DefUpStat,
                        MainManager.ItemUsage.GradualHP,MainManager.ItemUsage.GradualTP, MainManager.ItemUsage.ChargeUp, MainManager.ItemUsage.TurnNextTurn
                        ,(MainManager.ItemUsage)NewItemUse.AddSturdy
                    };
            Instance.DoRandomItemEffect(usages, characterid, value, player);
        }

        void DoRandomItemEffect(MainManager.ItemUsage[] effects, int characterid, int value, bool player)
        {
            MainManager.ItemUsage randomBuff = effects[UnityEngine.Random.Range(0, effects.Length)];

            if (randomBuff == MainManager.ItemUsage.TurnNextTurn || randomBuff == (MainManager.ItemUsage)NewItemUse.AddSturdy || randomBuff == (MainManager.ItemUsage)NewItemUse.AddTaunt)
                value = 1;

            if (player)
            {
                if (randomBuff == MainManager.ItemUsage.AtkUpStat)
                {
                    MainManager.battle.StatusEffect(MainManager.instance.playerdata[characterid], MainManager.BattleCondition.AttackUp, value, true, false);

                }
                else if (randomBuff == MainManager.ItemUsage.DefUpStat)
                {
                    MainManager.battle.StatusEffect(MainManager.instance.playerdata[characterid], MainManager.BattleCondition.DefenseUp, value, true, false);
                }else if(randomBuff == MainManager.ItemUsage.AddFreeze)
                {
                    MainManager.PlayParticle("mothicenormal", null, MainManager.instance.playerdata[characterid].battleentity.transform.position + Vector3.up).transform.localScale = Vector3.one * 1.5f;
                    if (MainManager.HasCondition(MainManager.BattleCondition.Freeze, MainManager.instance.playerdata[characterid]) > -1 && (MainManager.instance.playerdata[characterid].battleentity.icecube == null || !MainManager.instance.playerdata[characterid].battleentity.icecube.activeInHierarchy))
                    {
                        MainManager.instance.playerdata[characterid].battleentity.Freeze();
                    }
                }
                else
                {
                    MainManager.DoItemEffect(randomBuff, value, characterid);
                }
            }
            else
            {
                StartCoroutine(BattleControl_Ext.Instance.DoItemEffect(randomBuff, value, characterid, -1, MainManager.Items.None));
            }
        }

        public static void CheckEnemyVariantAnimator(EntityControl __instance) 
        {
            NewEnemies[] variants = new NewEnemies[] 
            {
                NewEnemies.Caveling,
                NewEnemies.FlyingCaveling,
                NewEnemies.Frostfly, 
                NewEnemies.PirahnaChomp, 
                NewEnemies.Moeruki, 
                NewEnemies.Abomiberry,
                NewEnemies.SplotchSpider,
                NewEnemies.Spineling,
                NewEnemies.Dewling,
                NewEnemies.FireAnt,
                NewEnemies.DynamoSpore,
                NewEnemies.Belosslow,
                NewEnemies.BatteryShroom,
                NewEnemies.Mothmite,
                NewEnemies.MarsBud
            }; 

            foreach (var v in variants)
            {
                if (IsNewEnemy(__instance, v))
                {
                    NewEnemies enemyId = v;
                    if (enemyId == NewEnemies.FlyingCaveling)
                        enemyId = NewEnemies.Caveling;

                    MainManager.AnimIDs animID = (MainManager.AnimIDs)__instance.animid+1;

                    if (enemyId == NewEnemies.Abomiberry && __instance.animid == (int)MainManager.AnimIDs.Abombhoney - 1)
                    {
                        __instance.anim.runtimeAnimatorController = assetBundle.LoadAsset<RuntimeAnimatorController>("Abombberry");
                        break;
                    }

                    if (__instance.anim.runtimeAnimatorController.name == animID.ToString())
                    {
                        __instance.anim.runtimeAnimatorController = assetBundle.LoadAsset<RuntimeAnimatorController>(enemyId.ToString());
                        break;
                    }

                }
            }
        }

        public static void LoadMapsBundle()
        {
            if(mapPrefabs == null)
                mapPrefabs = AssetBundle.LoadFromMemory(Properties.Resources.mapprefabs);
        }

        public static void UnloadMapsBundle()
        {
            mapPrefabs?.Unload(false);
        }

        //probably a dogshit approah, i just dont want to add any areas to existing arrays stuff
        public static string GetNewAreaNames(string __result, int mapid)
        {
            //Pit maps -> Pit of Trials
            //PowerPlant maps -> PowerPlant
            //Abandoned Tower maps -> Iron Tower
            //Deep Cave -> Deep Cave
            //Metal Lake -> Peacock Island
            string[] areaNames = new string[] { "Pit of Trials", "Power Plant", "Iron Tower", "Lush Abyss", "Peacock Island", "Sand Castle Depths", "Leafbug Village", "Giant's Playroom" };
            int areaId = GetNewAreaId(mapid);

            if(areaId != -1)
            {
                __result = areaNames[areaId];
            }
            return __result;
        }


        public static int GetNewAreaId(int mapId)
        {
            int[][] newAreasId = new int[][]
            {
                new int[]{ (int)NewMaps.Pit100BaseRoom, (int)NewMaps.Pit100Reward, (int)NewMaps.PitBossRoom, (int)MainManager.Maps.CaveOfTrials},
                new int[]{ (int)NewMaps.PowerPlantExtra, (int)NewMaps.PowerPlantBoss, (int)NewMaps.PowerPlantBigRoom, (int)NewMaps.PowerPlantElecPuzzle, (int)MainManager.Maps.PowerPlant},
                new int[]{ (int)NewMaps.AbandonedTower, (int)NewMaps.AbandonedTower1, (int)NewMaps.AbandonedTower2, (int)NewMaps.AbandonedTower3, (int)NewMaps.AbandonedTowerBoss, (int)NewMaps.AbandonedTowerCards},
                new int[]{ (int)NewMaps.DeepCave1, (int)NewMaps.DeepCave2, (int)NewMaps.DeepCaveBoss, (int)NewMaps.DeepCaveEntrance},
                new int[]{ (int)MainManager.Maps.MysteryIsland, (int)MainManager.Maps.MysteryIslandInside},
                new int[]{ (int)NewMaps.SandCastleDepthsBoss, (int)NewMaps.SandCastleDepthsIcePuzzle, (int)NewMaps.SandCastleDepthsMain, (int)NewMaps.SandCastleDepthsWall},
                new int[]{ (int)NewMaps.LeafbugVillage},
                new int[]{ (int)NewMaps.GiantLairPlayroom1,(int)NewMaps.GiantLairPlayroom2,(int)NewMaps.GiantLairPlayroom3,(int)NewMaps.GiantLairPlayroomBoss},
            };

            for(int i = 0; i < newAreasId.GetLength(0); i++)
            {
                if (newAreasId[i].Contains(mapId))
                    return i;
            }
            return -1;
        }

        public static bool CheckNewMusic(string musicClip, float fadeSpeed, int id, bool seamless)
        {
            if (musicClip != null)
            {
                if (Enum.TryParse(musicClip, out NewMusic result))
                {
                    MainManager.ChangeMusic(MainManager_Ext.assetBundle.LoadAsset<AudioClip>(musicClip), 0.1f, 0);
                    return false;
                }
            }
            return true;
        }

        public static int CheckMusicId(string musicClip)
        {
            if (Enum.TryParse(musicClip, out NewMusic result))
            {
                return (int)result;
            }
            return (int)Enum.Parse(typeof(MainManager.Musics), musicClip);
        }

        public static IEnumerator LoadNewMusicAsync(int musicId)
        {
            if (Enum.IsDefined(typeof(NewMusic), musicId))
            {
                var request = MainManager_Ext.assetBundle.LoadAssetAsync<AudioClip>(((NewMusic)musicId).ToString());
                while (!request.isDone)
                {
                    yield return null;
                }
                MainManager.ChangeMusic((AudioClip)request.asset);
            }
            else
            {
                ResourceRequest r = Resources.LoadAsync<AudioClip>("Audio/Music/" + ((MainManager.Musics)musicId).ToString());
                while (!r.isDone)
                {
                    yield return null;
                }
                MainManager.ChangeMusic((AudioClip)r.asset);
            }

        }

        static bool CheckListType(int type)
        {
            if (type == (int)NewListType.GourmetItem)
            {
                MainManager.listvar = MainManager.GradualFill(2);
                return true;
            }

            if (type == (int)NewListType.MedalCategories)
            {
                MainManager.listvar = PauseMenu_Ext.Instance.GetObtainedCategories();
                return true;
            }

            if (type == (int)NewListType.BadgeShops)
            {
                int badgeId = 0;

                if (MainManager.map.mapid == MainManager.Maps.UndergroundBar)
                    badgeId = 1;

                MainManager.listvar = MainManager.instance.badgeshops[badgeId].ToArray();
                return true;
            }
            return false;
        }

        static string GetNewListText(int type, int index, SpriteRenderer listBar, ref float barYOffset, ref float textSizeX, ref float textSizeY)
        {
            if (type == (int)NewListType.GourmetItem)
            {
                string[] listText = new string[2] { "Items", "Double Dip" };
                int tpCost = GetDoubleDipCost();

                string colorText = "";
                if (MainManager.instance.tp < tpCost && index == 1)
                {
                    colorText = "|color,1|";
                }
                Sprite barSprite = index == 0 ? MainManager.guisprites[(int)NewGui.ItemLeaf] : MainManager.itemsprites[1, (int)Medal.GourmetStomach];
                Vector3 size = index == 0 ? new Vector3(0.55f, 0.6f, 1f) : new Vector3(0.45f, 0.5f, 1f);
                MainManager.NewUIObject("barSprite", listBar.transform, new Vector3(-2.5f, 0f), size, barSprite, 10);

                if(index == 1)
                {
                    MainManager.instance.StartCoroutine(MainManager.SetText("|sort,10||size,0.75||font,0|" + colorText + tpCost.ToString().PadLeft(2, ' '), new Vector3(1.6f, -0.15f), listBar.transform));
                    MainManager.NewUIObject("tp", listBar.transform, new Vector3(2.55f, 0f), new Vector3(0.45f, 0.5f, 1f), MainManager.guisprites[28], 10);
                }

                return colorText + listText[index];
            }

            if (type == (int)NewListType.MedalCategories)
            {
                PauseMenu_Ext.MedalCategory category = PauseMenu_Ext.Instance.medalCategories[MainManager.listvar[index]];
                string text = "|size,1,1||font,0||single|" + category.name + " Medals";
                textSizeX = 0.35f;
                textSizeY = -0.3f;
                barYOffset -= 0.2f;

                SpriteRenderer icon = new GameObject("itemsprite").AddComponent<SpriteRenderer>();
                icon.sprite = category.iconId >= 0 ? PauseMenu_Ext.Instance.categoryIcons[category.iconId] : MainManager.guisprites[Mathf.Abs(category.iconId)];
                icon.transform.parent = listBar.transform;
                icon.gameObject.layer = 5;
                icon.transform.localScale = Vector3.one*0.45f;
                icon.transform.localPosition = new Vector2(8.8f, category.iconId >= 0 ? 0f : -0.45f);

                return text;
            }

            return "";
        }

        static string GetNewListDesc(int type)
        {
            if (type == (int)NewListType.GourmetItem)
            {
                string[] descText = new string[2] { "Use an Item.", "Lets you use up to 2 items in a row in one action." };
                return descText[MainManager.instance.option];
            }

            if(type == (int)NewListType.MedalCategories)
            {
                int categoryIndex = MainManager.listvar[MainManager.instance.option];

                if (categoryIndex == 0)
                    return "Browse all of your medals!";

                PauseMenu_Ext.MedalCategory category = PauseMenu_Ext.Instance.medalCategories[MainManager.listvar[MainManager.instance.option]];
                return $"Browse your {category.name} medals!";
            }
            return "";
        }

        public static int GetDoubleDipCost()
        {
            if (MainManager.instance.inevent && MainManager.lastevent == 42)
                return 0;

            int tpCost = 4;

            if (MainManager.battle != null && MainManager.BadgeIsEquipped((int)MainManager.BadgeTypes.TPSaver, MainManager.instance.playerdata[MainManager.battle.currentturn].trueid))
            {
                tpCost--;
            }
            return tpCost;
        }

        static void CheckCancelListDoubleDip()
        {
            if (MainManager.listtype == 0 && BattleControl_Ext.Instance.gourmetItemUse >= 0)
            {
                if (BattleControl_Ext.Instance.gourmetItemUse == 1)
                {
                    MainManager.instance.tp = Mathf.Clamp(MainManager.instance.tp + MainManager_Ext.GetDoubleDipCost(), 0, MainManager.instance.maxtp);
                }
                else
                {
                    MainManager.battle.EndPlayerTurn();
                }
                BattleControl_Ext.Instance.gourmetItemUse = -1;
            }
        }

        static int CheckResistanceValue(int res, MainManager.BattleData data)
        {
            if (MainManager.HasCondition(MainManager.BattleCondition.Inked, data) > -1)
                res -= 25;
            return res;
        }

        static int GetAllQuestsAmount()
        {
            return Enum.GetNames(typeof(MainManager.BoardQuests)).Length - 1 + Enum.GetNames(typeof(NewQuest)).Length;
        }

        static bool CantUseSkillInked()
        {
            bool inked = MainManager.HasCondition(MainManager.BattleCondition.Inked, MainManager.instance.playerdata[MainManager.battle.currentturn]) == -1;
            return inked || (!inked && MainManager.BadgeIsEquipped((int)Medal.InvisibleInk));
        }
        static bool CantUseSkillSticky()
        {
            bool stickied = MainManager.HasCondition(MainManager.BattleCondition.Sticky, MainManager.instance.playerdata[MainManager.battle.currentturn]) == -1;
            return stickied || (!stickied && MainManager.BadgeIsEquipped((int)Medal.FlavorlessAdhesive));
        }

        public static void SetupNewShops()
        {
            List<List<int>> newShops = new List<List<int>>()
            {
                new List<int>(){(int)Medal.Wildfire,(int)Medal.HeatingUp,(int)Medal.Phoenix,(int)Medal.FierySpirit,(int)Medal.FireNeedles },
                new List<int>(){(int)Medal.FlavorlessAdhesive,(int)Medal.ThickSilk,(int)Medal.SpiderBait},
                new List<int>(){(int)Medal.InkBubble,(int)Medal.Smearcharge,(int)Medal.InvisibleInk}
            };

            if(MainManager.instance.avaliablebadgepool.Length < 2 + newShops.Count)
            {
                Array.Resize(ref MainManager.instance.avaliablebadgepool, 2 + newShops.Count);
                Array.Resize(ref MainManager.instance.badgeshops, 2 + newShops.Count);

                for (int i = 0; i < newShops.Count; i++)
                {
                    if (MainManager.instance.badgeshops[2+i] == null)
                    {
                        MainManager.instance.badgeshops[2 + i] = new List<int>(newShops[i]);
                        MainManager.instance.avaliablebadgepool[2 + i] = new List<int>();
                    }
                }
            }
        }

        static BattleControl.AttackProperty? GetStickyProperty()
        {
            return BattleControl.AttackProperty.Sticky;
        }

        public int GetBossMap(int boss)
        {
            Dictionary<int, MainManager.BattleMaps> bossToMap = new Dictionary<int, MainManager.BattleMaps> ()
            {
                {(int)MainManager.Enemies.Acolyte, MainManager.BattleMaps.GoldenSettlementArena },
                {(int)MainManager.Enemies.Ahoneynation, MainManager.BattleMaps.FactoryS2 },
                {(int)MainManager.Enemies.BanditLeader, MainManager.BattleMaps.HideoutAstotheles },
                {(int)MainManager.Enemies.Scorpion, MainManager.BattleMaps.Desert1},
                {(int)MainManager.Enemies.Scarlet, MainManager.BattleMaps.Cave0},
                {(int)MainManager.Enemies.Beetle, MainManager.BattleMaps.KaliShop},
                {(int)MainManager.Enemies.Carmina, MainManager.BattleMaps.CarminaRoom},
                {(int)MainManager.Enemies.Fisherman, MainManager.BattleMaps.FarGrasslands},
                {(int)MainManager.Enemies.Cenn, MainManager.BattleMaps.Grasslands1},
                {(int)MainManager.Enemies.WaspDriller, MainManager.BattleMaps.WaspThrone},
                {(int)MainManager.Enemies.Zasp, MainManager.BattleMaps.GoldenBattle2},
                {(int)MainManager.Enemies.PrimalWeevil, MainManager.BattleMaps.BarrenLands},
                {(int)MainManager.Enemies.DeadLanderA, MainManager.BattleMaps.GiantLair2},
                {(int)MainManager.Enemies.SpuderReal, MainManager.BattleMaps.Snakemouth3},
                {(int)MainManager.Enemies.VenusBoss, MainManager.BattleMaps.GoldenHillsBoss},
                {(int)MainManager.Enemies.BeeBoss, MainManager.BattleMaps.FactoryC},
                {(int)MainManager.Enemies.ZombieRoach, MainManager.BattleMaps.SandCastleBoss},
                {(int)MainManager.Enemies.Centipede, MainManager.BattleMaps.Swamplands},
                {(int)MainManager.Enemies.UltimaxTank, MainManager.BattleMaps.RubberPrisonBoss},
                {(int)MainManager.Enemies.FlyTrap, MainManager.BattleMaps.ChomperCavesBoss},
                {(int)MainManager.Enemies.MidgeBroodmother, MainManager.BattleMaps.Broodmother},
                {(int)MainManager.Enemies.SeedlingKing, MainManager.BattleMaps.Grasslands2},
                {(int)MainManager.Enemies.SandWyrm, MainManager.BattleMaps.StreamMountainBoss},
                {(int)MainManager.Enemies.PeacockSpider, MainManager.BattleMaps.MysteryIslandInside},
                {(int)MainManager.Enemies.FalseMonarch, MainManager.BattleMaps.AbandonedTent},
                {(int)MainManager.Enemies.Pitcher, MainManager.BattleMaps.PitcherPlant},
                {(int)MainManager.Enemies.Zommoth, MainManager.BattleMaps.UpperSnekBoss},
                {(int)MainManager.Enemies.WaspKing, MainManager.BattleMaps.FinalBoss1},
                {(int)MainManager.Enemies.EverlastingKing, MainManager.BattleMaps.FinalBoss2},
                {(int)MainManager.Enemies.Maki, MainManager.BattleMaps.AntBridge},
                {(int)MainManager.Enemies.Stratos, MainManager.BattleMaps.UndergroundBar},
                {(int)MainManager.Enemies.HoloVi, MainManager.BattleMaps.AssociationHQ},
                {(int)NewEnemies.TermiteKnight, MainManager.BattleMaps.TermiteColiseum },
                {(int)MainManager.Enemies.TermiteSoldier, MainManager.BattleMaps.TermiteColiseum },
            };

            if (bossToMap.ContainsKey(boss))
            {
                if (boss == (int)MainManager.Enemies.Zasp && MainManager.instance.flags[409])
                    return (int)MainManager.BattleMaps.TermiteColiseum;

                return (int)bossToMap[boss];
            }

            return -1;
        }

        public int GetNewBossMap(int boss)
        {
            Dictionary<int, int> bossToMap = new Dictionary<int, int>()
            {
                {(int)NewEnemies.Levi, (int)NewMaps.AntPalaceTrainingRoom },
                {(int)MainManager.Enemies.LeafbugNinja, (int)NewMaps.LeafbugShamanHut },
                {(int)NewEnemies.DullScorp, (int)NewMaps.SandCastleDepthsBoss },
                {(int)NewEnemies.BatteryShroom, (int)NewMaps.PowerPlantBoss },
                {(int)NewEnemies.Belosslow, (int)NewMaps.DeepCaveBoss },
                {(int)NewEnemies.IronSuit, (int)NewMaps.AbandonedTowerBoss },
                 {(int)NewEnemies.Jester, (int)NewMaps.GiantLairPlayroomBoss },
                {(int)MainManager.Enemies.TANGYBUG, (int)MainManager.Maps.GoldenSettlement3 },
                {(int)NewEnemies.MarsSprout, (int)NewMaps.PitBossRoom },
                {(int)NewEnemies.DarkVi, (int)MainManager.Maps.SnakemouthEmpty },
                //pattons fight
                {(int)NewEnemies.Abomiberry, (int)MainManager.Maps.BarrenLandsEntrance },
            };

            if (bossToMap.ContainsKey(boss))
                return bossToMap[boss];

            return -1;
        }

        public static bool IsHolo()
        {
            return (MainManager.instance.flags[162] && MainManager_Ext.Instance.newBossMap == -1 && MainManager.battle.sdata.stage == (int)MainManager.BattleMaps.HBsLab);
        }

        public int GetMapFromBattleMap(MainManager.BattleMaps battleMap)
        {
            Dictionary<MainManager.BattleMaps, MainManager.Maps> battleMapToMap = new Dictionary<MainManager.BattleMaps, MainManager.Maps>()
            {
                {MainManager.BattleMaps.GoldenSettlementArena, MainManager.Maps.GoldenSettlement1Night },
                {MainManager.BattleMaps.FactoryS2, MainManager.Maps.FactoryStorageMiniboss },
                {MainManager.BattleMaps.HideoutAstotheles, MainManager.Maps.HideoutEntrance },
                {MainManager.BattleMaps.Desert1, MainManager.Maps.DesertScorpion },
                {MainManager.BattleMaps.Cave0, MainManager.Maps.GoldenPathTunnel },
                {MainManager.BattleMaps.KaliShop, MainManager.Maps.DefiantRoot3 },
                {MainManager.BattleMaps.CarminaRoom, MainManager.Maps.MetalIsland2 },
                {MainManager.BattleMaps.FarGrasslands, MainManager.Maps.FarGrasslandsOutsideVillage },
                {MainManager.BattleMaps.Grasslands1, MainManager.Maps.BugariaOutskirtsOutsideCity },
                {MainManager.BattleMaps.WaspThrone, MainManager.Maps.WaspKingdomThrone },
                {MainManager.BattleMaps.GoldenBattle2, MainManager.Maps.GoldenHillsDungeonEntrance },
                {MainManager.BattleMaps.BarrenLands, MainManager.Maps.BarrenLandsMiniboss },
                {MainManager.BattleMaps.GiantLair2, MainManager.Maps.GiantLairFridgeOutside },
                {MainManager.BattleMaps.Snakemouth3, MainManager.Maps.SnakemouthTreasureRoom },
                {MainManager.BattleMaps.GoldenHillsBoss, MainManager.Maps.GoldenHillsDungeonBoss },
                {MainManager.BattleMaps.FactoryC, MainManager.Maps.HoneyFactoryCore },
                {MainManager.BattleMaps.SandCastleBoss, MainManager.Maps.SandCastleBossRoom },
                {MainManager.BattleMaps.Swamplands, MainManager.Maps.SwamplandsBoss },
                {MainManager.BattleMaps.RubberPrisonBoss, MainManager.Maps.RubberPrisonGiantLairBridge },
                {MainManager.BattleMaps.ChomperCavesBoss, MainManager.Maps.ChomperCaves3 },
                {MainManager.BattleMaps.Broodmother, MainManager.Maps.BroodmotherLair },
                {MainManager.BattleMaps.Grasslands2, MainManager.Maps.SeedlingHaven },
                {MainManager.BattleMaps.StreamMountainBoss, MainManager.Maps.StreamMountain5 },
                {MainManager.BattleMaps.MysteryIslandInside, MainManager.Maps.MysteryIslandInside },
                {MainManager.BattleMaps.AbandonedTent, MainManager.Maps.AbandonedCityTent },
                {MainManager.BattleMaps.PitcherPlant, MainManager.Maps.PitcherPlantArena },
                {MainManager.BattleMaps.UpperSnekBoss, MainManager.Maps.UpperSnekBossRoom },
                {MainManager.BattleMaps.FinalBoss1, MainManager.Maps.GiantLairSaplingPlains},
                {MainManager.BattleMaps.FinalBoss2, MainManager.Maps.GiantLairSaplingPlains },
                {MainManager.BattleMaps.AntBridge, MainManager.Maps.AntBridge },
                {MainManager.BattleMaps.UndergroundBar, MainManager.Maps.UndergroundBar },
                {MainManager.BattleMaps.AssociationHQ, MainManager.Maps.BugariaOutskirtsOutsideCity },
                {MainManager.BattleMaps.TermiteColiseum, MainManager.Maps.TermiteColiseum2 },
            };

            if (battleMapToMap.ContainsKey(battleMap))
                return (int)battleMapToMap[battleMap];

            return -1;
        }

        public static void ResetRenderSettings()
        {
            RenderSettings.skybox = Instance.savedRenderSettings.skyboxMat;
            RenderSettings.fogColor = Instance.savedRenderSettings.fogColor;
            RenderSettings.fogEndDistance = Instance.savedRenderSettings.fogEndDistance;
            RenderSettings.ambientSkyColor = Instance.savedRenderSettings.ambientSkyColor;
            RenderSettings.ambientLight = Instance.savedRenderSettings.globalLight;
            Instance.savedRenderSettings = null;
        }

        static bool IsInBoss() => MainManager.instance.flags[162];
        public class SavedRenderSettings
        {
            public Material skyboxMat;
            public Color fogColor;
            public float fogEndDistance;
            public Color ambientSkyColor;
            public Color globalLight;

            public SavedRenderSettings(Material skyBox, Color fog, float endDistance, Color skyColor, Color ambientLight)
            {
                skyboxMat = skyBox;
                fogColor = fog;
                fogEndDistance = endDistance; 
                ambientSkyColor = skyColor;       
                globalLight = ambientLight;
            }
        }

        public static void CheckWellRestedAchievement(bool bedbug)
        {
            Dictionary<MainManager.Maps, int> mapToFlag = new Dictionary<MainManager.Maps, int>()
            {
                {MainManager.Maps.BugariaMainPlaza,  903},
                {MainManager.Maps.ChucksAbode,  904},
                {MainManager.Maps.DefiantRoot2,  905},
                {MainManager.Maps.GiantLairRoachVillage,  906},
                {MainManager.Maps.GoldenSettlement1,  907},
                {MainManager.Maps.JaunesGallery,  909},
                {MainManager.Maps.TermiteIndustrial,  910},
                {MainManager.Maps.MetalIsland2,  911},
            };


            if (!bedbug)
            {
                if (mapToFlag.ContainsKey(MainManager.map.mapid))
                    MainManager.instance.flags[mapToFlag[MainManager.map.mapid]] = true;
            }
            else
            {
                MainManager.instance.flags[912] = true;
            }

            if (!MainManager.instance.librarystuff[3, (int)NewAchievement.WellRested])
            {
                if (mapToFlag.All(m => MainManager.instance.flags[m.Value]) && MainManager.instance.flags[912])
                {
                    MainManager.instance.flags[913] = true;
                }
            }
        }

        public static T GetWeightedResult<T>(Dictionary<T,int> dictionnary)
        {
            float totalWeight = dictionnary.Sum(r => r.Value);
            float randomValue = UnityEngine.Random.Range(0, totalWeight);
            foreach (var r in dictionnary)
            {
                if (randomValue < r.Value)
                {
                    return r.Key;
                }
                randomValue -= r.Value;
            }
            return dictionnary.Keys.First();
        }

        public RuntimeAnimatorController GetSwitcherooAnim(int id)
        {
            Dictionary<int, string> idToAnimator = new Dictionary<int, string>()
            {
                { 0, "altVi" },
                { 1, "altKabbu" },
                { 2, "altLeif" },
            };
            return MainManager_Ext.assetBundle.LoadAsset<RuntimeAnimatorController>(idToAnimator[id]);
        }

        public static int[] GetNewBounties()
        {
            return new int[]{ (int)NewQuest.BountyDullScorp, (int)NewQuest.BountyDynamoSpore, (int)NewQuest.BountyBelosslow, 
                (int)NewQuest.BountyIronSuit, (int)NewQuest.BountyJester };

        }

        public static IEnumerator LerpSpriteColor(SpriteRenderer sprite, float endFrames, Color targetColor)
        {
            float a = 0f;
            Color startColor = sprite.color;
            do
            {
                sprite.color = Color.Lerp(startColor, targetColor, a / endFrames);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < endFrames);
            sprite.color = targetColor;
        }

        public void CheckSuperBugAchievement()
        {
            int[] superbosses = GetSuperBosses();
            int[] holoTeamSnek = { (int)MainManager.Enemies.HoloVi, (int)MainManager.Enemies.HoloKabbu, (int)MainManager.Enemies.HoloLeif };

            foreach (var boss in superbosses)
            {
                if(boss == -2)
                {
                    for(int i = 0; i < holoTeamSnek.Length; i++)
                    {
                        if (MainManager.instance.enemyencounter[holoTeamSnek[i], 1] <= 0)
                        {
                            return;
                        }
                    }
                    continue;
                }

                if (MainManager.instance.enemyencounter[boss, 1] <= 0)
                {
                    return;
                }
 
            }
            MainManager.UpdateJounal(MainManager.Library.Logbook, (int)NewAchievement.SuperBug);
        }
    }


}
