using HarmonyLib;
using System;
using System.IO;
using UnityEngine;
using BFPlus.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InputIOManager;
using System.Reflection;
using System.ComponentModel;
using System.Threading;
using System.Reflection.Emit;

namespace BFPlus.Patches
{

    [HarmonyPatch(typeof(MainManager), "FixSamira")]
    public class PatchMainManagerFixSamira
    {
        static void Prefix()
        {
            if (MainManager.instance.samiramusics != null && MainManager.instance.samiramusics.Count > 0)
            {
                MainManager.instance.samiramusics.RemoveAll(s => 
                    (MainManager.Musics)s[0] == (MainManager.Musics)NewMusic.AngryViTheme ||
                    (MainManager.Musics)s[0] == (MainManager.Musics)NewMusic.SadKabbuTheme ||
                    (MainManager.Musics)s[0] == (MainManager.Musics)NewMusic.Playroom ||
                    (MainManager.Musics)s[0] == (MainManager.Musics)NewMusic.DarkSnek
                );
            }
        }
    }

    [HarmonyPatch(typeof(MainManager), "SetUpBadges")]
    public class PatchMainManagerSetUpBadges
    {
        static void Prefix(MainManager __instance)
        {
            //Tp plus from can't sleep quest.
            __instance.badgeshops[0].AddRange(new int[]
            {
                (int)MainManager.BadgeTypes.TPPlus, (int)Medal.HPDown, (int)Medal.MPPlus, (int)Medal.TimingTutor
            });

            __instance.badgeshops[1].AddRange(new int[]
            {
                (int)Medal.HPDown, (int)Medal.Powerbank, (int)Medal.PurifyingPulse, (int)Medal.RevitalizingRipple
            });
            MainManager.instance.flags[768] = true;
            MainManager_Ext.SetupNewShops();
        }
    }

    [HarmonyPatch(typeof(MainManager), "GetQuestsBoard")]
    public class PatchMainManagerGetQuestsBoard
    {
        static void Postfix(int type, ref int[] __result)
        {
            if (type == 0 && MainManager.map.mapid != MainManager.Maps.UndergroundBar)
            {
                List<int> tempList = __result.ToList();
                int[] questToRemove = MainManager_Ext.GetNewBounties();

                foreach (var quest in questToRemove)
                    tempList.Remove(quest);

                if (tempList.Count == 0)
                    tempList.Add(0);

                __result = tempList.ToArray();
            }
        }
    }


    [HarmonyPatch(typeof(MainManager), "GetSettings")]
    public class PatchMainManagerGetSettings
    {
        static void Postfix(ref int[] __result)
        {
            List<int> tempList = new List<int>(__result);
            tempList.InsertRange(tempList.FindIndex(setting => setting == 16), new int[] { 26, 27, 28 });
            __result = tempList.ToArray();
        }
    }

    [HarmonyPatch(typeof(MainManager), "EndOfMessage")]
    public class PatchMainManagerEndOfMessage
    {
        static void Postfix()
        {
            if (MainManager.instance.flags[347] && MainManager.instance.badgeshops.Length > 2)
            {
                if (MainManager.instance.badgeshops[2].Count == 0)
                {
                    //when you bough all fire medals from the shop
                    MainManager.instance.flags[830] = true;
                }
            }
        }
    }

    [HarmonyPatch(typeof(MainManager), "ReadSettings")]
    public class PatchMainManagerReadSettings
    {
        static void Prefix(string[] c)
        {
            if (c.Length < 48)
            {
                MainManager_Ext.fastText = false;
                MainManager_Ext.showResistance = true;
                MainManager_Ext.newBattleThemes = true;
            }
            else
            {
                MainManager_Ext.fastText = Convert.ToBoolean(c[44]);

                var codes = c[45].Split(',');

                if (codes.Length == MainManager_Ext.newUnlocks.Length)
                {
                    for (int i = 0; i < MainManager_Ext.newUnlocks.Length; i++)
                    {
                        MainManager_Ext.newUnlocks[i] = Convert.ToBoolean(codes[i]);
                    }
                }
                MainManager_Ext.showResistance = Convert.ToBoolean(c[46]);
                MainManager_Ext.newBattleThemes = Convert.ToBoolean(c[47]);
            }
        }

        //Force english language
        static void Postfix()
        {
            MainManager.languageid = 0;
        }
    }

    [HarmonyPatch(typeof(MainManager), "HasSkillCost")]
    public class PatchMainManagerHasSkillCost
    {
        static void Postfix(int tpcost, int playerid, ref bool __result)
        {
            int destinyDreamHolder = BattleControl_Ext.GetDestinyDreamBug();

            if (destinyDreamHolder != -1)
            {
                __result = MainManager.instance.playerdata[destinyDreamHolder].hp > tpcost;
            }
        }
    }

    [HarmonyPatch(typeof(MainManager), "SaveSettings")]
    public class PatchMainManagerSaveSettings
    {
        static void Postfix(ref string __result)
        {
            string newResult = string.Concat(new object[] {
                __result,
                "\n",
                MainManager_Ext.fastText,
                "\n",
                string.Join(",", MainManager_Ext.newUnlocks.Select(a => a.ToString()).ToArray()),
                "\n",
                MainManager_Ext.showResistance,
                "\n",
                MainManager_Ext.newBattleThemes
            });
            __result = newResult;
        }
    }

    [HarmonyPatch(typeof(MainManager), "GetEnemyData", new Type[] { typeof(int), typeof(bool), typeof(bool) })]
    public class PatchMainManagerGetEnemyData
    {
        static void Postfix(ref MainManager.BattleData __result)
        {
            if (__result.deathtype == 13)
            {
                __result.entity.destroytype = NPCControl.DeathType.PlayerDeath;
            }

            if (__result.animid == (int)NewEnemies.LonglegsSpider)
            {
                __result.position = BattleControl.BattlePosition.Flying;
            }
        }
    }



    [HarmonyPatch(typeof(MainManager), "RefreshSkills", new Type[] { })]
    public class PatchMainManagerRefreshSkills
    {
        static void Postfix()
        {
            int[] playerIds = MainManager.instance.playerdata.Select(p => p.trueid).ToArray();
            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                int trueid = MainManager.instance.playerdata[i].trueid;

                if (trueid == 0)
                {
                    if (MainManager.BadgeIsEquipped((int)Medal.HoloSkill, trueid))
                    {
                        if (playerIds.Contains(1))
                            MainManager.instance.playerdata[i].skills.Add((int)NewSkill.HoloKabbu);
                        if (playerIds.Contains(2))
                            MainManager.instance.playerdata[i].skills.Add((int)NewSkill.HoloLeif);
                    }

                    if (MainManager.instance.inbattle && BattleControl_Ext.Instance.holoSkillID != -1)
                    {
                        MainManager.instance.playerdata[i].skills.Remove((int)MainManager.Skills.BeeFly);
                        MainManager.instance.playerdata[i].skills.Remove((int)MainManager.Skills.IceBeemerang);
                        MainManager.instance.playerdata[i].skills.Remove((int)MainManager.Skills.IceSphere);
                    }

                    if (MainManager.instance.partylevel >= 8)
                    {
                        int index = MainManager.instance.inbattle ? 2 : 5;
                        if (MainManager.instance.playerdata[i].skills.Count > index)
                            MainManager.instance.playerdata[i].skills.Insert(index, (int)NewSkill.Steal);
                        else
                            MainManager.instance.playerdata[i].skills.Add((int)NewSkill.Steal);
                    }
                }
                else if (trueid == 1)
                {
                    if (MainManager.BadgeIsEquipped((int)Medal.HoloSkill, trueid))
                    {
                        if (playerIds.Contains(0))
                            MainManager.instance.playerdata[i].skills.Add((int)NewSkill.HoloVi);

                        if (playerIds.Contains(2))
                            MainManager.instance.playerdata[i].skills.Add((int)NewSkill.HoloLeif);
                    }
                    if (MainManager.instance.inbattle && BattleControl_Ext.Instance.holoSkillID != -1)
                    {
                        MainManager.instance.playerdata[i].skills.Remove((int)MainManager.Skills.BeeFly);
                        MainManager.instance.playerdata[i].skills.Remove((int)MainManager.Skills.IceDrill);
                        MainManager.instance.playerdata[i].skills.Remove((int)MainManager.Skills.IceSphere);
                    }

                    if (MainManager.instance.partylevel >= 13)
                    {
                        int index = MainManager.instance.inbattle ? 4 : 7;
                        if (MainManager.instance.playerdata[i].skills.Count > index)
                            MainManager.instance.playerdata[i].skills.Insert(index, (int)NewSkill.Lecture);
                        else
                            MainManager.instance.playerdata[i].skills.Add((int)NewSkill.Lecture);
                    }
                }
                else if (trueid == 2)
                {
                    if (MainManager.BadgeIsEquipped((int)Medal.HoloSkill, trueid))
                    {
                        if (playerIds.Contains(0))
                            MainManager.instance.playerdata[i].skills.Add((int)NewSkill.HoloVi);

                        if (playerIds.Contains(1))
                            MainManager.instance.playerdata[i].skills.Add((int)NewSkill.HoloKabbu);
                    }

                    if (MainManager.instance.inbattle && BattleControl_Ext.Instance.holoSkillID != -1)
                    {
                        MainManager.instance.playerdata[i].skills.Remove((int)MainManager.Skills.IceBeemerang);
                        MainManager.instance.playerdata[i].skills.Remove((int)MainManager.Skills.IceDrill);
                        MainManager.instance.playerdata[i].skills.Remove((int)MainManager.Skills.IceSphere);
                    }

                    if (MainManager.BadgeIsEquipped((int)Medal.ViolentVitiation))
                    {
                        if (MainManager.instance.flags[160])
                        {
                            MainManager.instance.playerdata[i].skills.Add((int)NewSkill.VitiationLite);
                            MainManager.instance.playerdata[i].skills.Remove((int)MainManager.Skills.BubbleShieldLite);
                        }

                        if (MainManager.instance.flags[20])
                        {
                            MainManager.instance.playerdata[i].skills.Add((int)NewSkill.Vitiation);
                            MainManager.instance.playerdata[i].skills.Remove((int)MainManager.Skills.BubbleShield);
                        }
                    }

                    if (!MainManager.instance.playerdata[i].skills.Contains((int)MainManager.Skills.Cleanse))
                    {
                        MainManager.instance.playerdata[i].skills.Add((int)MainManager.Skills.Cleanse);
                    }

                    if (MainManager.instance.flags[878])
                    {
                        int index = MainManager.instance.inbattle ? 3 : 6;
                        if (MainManager.instance.playerdata[i].skills.Count > index)
                            MainManager.instance.playerdata[i].skills.Insert(index, (int)NewSkill.CordycepsLeech);
                        else
                            MainManager.instance.playerdata[i].skills.Add((int)NewSkill.CordycepsLeech);
                    }
                }

                if (MainManager.BadgeIsEquipped((int)Medal.SleepSchedule, trueid))
                {
                    MainManager.instance.playerdata[i].skills.Add((int)NewSkill.SleepSchedule);
                }

                if (MainManager.BadgeIsEquipped((int)Medal.TribalDance, trueid))
                {
                    MainManager.instance.playerdata[i].skills.Add((int)NewSkill.RainDance);
                }
            }
        }
    }

    [HarmonyPatch(typeof(MainManager), "ApplyBadges")]
    public class PatchMainManagerApplyBadges
    {
        static void Prefix()
        {
            if (MainManager.instance?.playerdata != null)
            {
                foreach (var player in MainManager.instance.playerdata)
                {
                    Transform entity = MainManager.instance.inbattle ? player.battleentity?.transform : player.entity?.transform;
                    if (entity != null)
                    {
                        MainManager_Ext.CheckSpuderCardEffect(entity);
                    }
                }
            }
        }

        static void Postfix()
        {
            //10/10 TP 5/5 BP | 7 TP 8 BP
            //MP PLUS CHECK
            //int maxBP = MainManager.instance.maxbp; //5 BP

            int mpPlusBuff = 3 * MainManager.BadgeHowManyEquipped((int)Medal.MPPlus);
            MainManager.instance.maxtp -= mpPlusBuff;
            MainManager.instance.tp = Mathf.Clamp(MainManager.instance.tp, 0, MainManager.instance.maxtp);

            //3 old new 6
            if (MainManager_Ext.mpPlusBonus != mpPlusBuff)
            {
                int mpPlusDebuff = MainManager_Ext.mpPlusBonus;
                MainManager.instance.maxbp += mpPlusBuff - mpPlusDebuff;
                MainManager.instance.bp = Mathf.Clamp(MainManager.instance.bp + mpPlusBuff - mpPlusDebuff, 0, MainManager.instance.maxbp);

                MainManager_Ext.mpPlusBonus = mpPlusBuff;
            }
        }
    }

    [HarmonyPatch(typeof(MainManager), "DestroyList")]
    public class PatchMainManagerDestroyList
    {
        static void Postfix()
        {
            if (MainManager.battle != null)
                BattleControl_Ext.Instance.destroyedList = true;
        }
    }

    [HarmonyPatch(typeof(MainManager), "ChangeMusic", new Type[] { typeof(AudioClip), typeof(float), typeof(int), typeof(bool) })]
    public class PatchMainManagerChangeMusic
    {
        static bool Prefix(AudioClip musicclip, float fadespeed, int id, bool seamless)
        {
            if (MainManager_Ext.Instance.musicPlayer)
            {
                if (MainManager.lastevent == 28)
                {
                    MainManager_Ext.Instance.musicPlayer = false;
                    if (MainManager.player.transform.Find("MusicSimple(Clone)") != null)
                    {
                        UnityEngine.Object.Destroy(MainManager.player.transform.Find("MusicSimple(Clone)").gameObject);
                    }
                    return true;
                }

                if (MainManager.music[0] != null && MainManager.music[0].volume != MainManager.musicvolume)
                {
                    MainManager.music[0].volume = MainManager.musicvolume;
                }

                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(MainManager), "Reset")]
    public class PatchMainManagerReset
    {
        static void Prefix()
        {
            BattleControl_Ext.stylishBarAmount = 0;
        }
    }

    [HarmonyPatch(typeof(MainManager), "GetTPCost", new Type[] { typeof(int), typeof(int), typeof(bool) })]
    public class PatchMainManagerGetTPCost
    {
        static void Postfix(ref int __result, int player, int id, bool matchid)
        {
            if (MainManager.instance.inevent && MainManager.lastevent == 42)
                __result = 0;
        }
    }

    [HarmonyPatch(typeof(MainManager), "LoadMap", new Type[] { typeof(int) })]
    public class PatchMainManagerLoadMap
    {
        static void Postfix()
        {
            if (MainManager.BadgeIsEquipped((int)Medal.SpuderCard))
            {
                if (MainManager.instance?.playerdata != null)
                {
                    foreach (var player in MainManager.instance.playerdata)
                    {
                        Transform entity = player.entity?.transform;
                        if (entity != null)
                        {
                            MainManager_Ext.CheckSpuderCardEffect(entity);
                        }
                    }
                }
            }

            if (MainManager_Ext.Instance.musicPlayer)
            {
                MainManager_Ext.CreateMusicParticle();
            }

            MainManager.instance.inmusicrange = -1; //incorrectly not set to -1 by loadmap, only by transfermap, lead to issues with using
                                                    //ant compass in a music range entity
        }
    }

    [HarmonyPatch(typeof(MainManager), "SetDefaultStats")]
    public class PatchMainManagerSetDefaultStats
    {
        static void Postfix(MainManager __instance, ref MainManager.BattleData __result)
        {
            if (__result.animid == 2)
            {
                __result.hp = 8;
                __result.maxhp = __result.hp;
                __result.basehp = __result.hp;
            }
        }
    }

    [HarmonyPatch(typeof(MainManager), "ResetStats")]
    public class PatchMainManagerResetStats
    {
        static void Postfix()
        {
            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.instance.playerdata[i].trueid == 2)
                {
                    MainManager.instance.playerdata[i].basehp = 8;
                }
            }
        }
    }


    [HarmonyPatch(typeof(MainManager), "SetVariables")]
    public class PatchMainManagerSetVariables
    {
        static void Prefix(MainManager __instance)
        {
            if (__instance.GetComponent<MainManager_Ext>() == null)
                __instance.gameObject.AddComponent<MainManager_Ext>();

            if (MainManager_Ext.assetBundle == null)
            {
                MainManager_Ext.assetBundle = AssetBundle.LoadFromMemory(Properties.Resources.vengeance);
            }

            var specialEnemies = (int[][])typeof(EntityControl).GetField("specialenemy", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

            if (specialEnemies[1].Length == 3)
            {
                var itemsOdds = specialEnemies[0].ToList();
                itemsOdds.Add(50); //frostfly 50%

                specialEnemies[0] = itemsOdds.ToArray();

                var enemyIds = specialEnemies[1].ToList();
                enemyIds.Add((int)NewEnemies.Frostfly);
                specialEnemies[1] = enemyIds.ToArray();


                var recipepool = (int[][])typeof(EntityControl).GetField("recipepool", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
                var recipes = recipepool.ToList();
                recipes.Add(new int[] { (int)MainManager.Items.ShavedIce, (int)MainManager.Items.Ice });

                typeof(EntityControl).GetField("recipepool", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, recipes.ToArray());
            }
            MainManager.librarylimit[0] = 50 + Enum.GetNames(typeof(NewDiscoveries)).Length;
            MainManager.librarylimit[1] = 92 + Enum.GetNames(typeof(NewEnemies)).Length - 2 + 5;
            MainManager.librarylimit[2] = 70 + Enum.GetNames(typeof(NewRecipes)).Length;
            MainManager.librarylimit[3] = 30 + Enum.GetNames(typeof(NewAchievement)).Length;
        }
        static void Postfix()
        {
            Array.Resize(ref MainManager.instance.crystalbflags, MainManager_Ext.CBFlagNumber);

            var prizeenemyids = MainManager.instance.prizeenemyids.ToList();
            prizeenemyids.Add((int)MainManager.Enemies.Acolyte);
            prizeenemyids.Add((int)NewEnemies.TermiteKnight);
            prizeenemyids.Add((int)NewEnemies.LeafbugShaman);
            prizeenemyids.Add((int)NewEnemies.Patton);
            prizeenemyids.Add((int)NewEnemies.Levi);
            prizeenemyids.Add((int)NewEnemies.Mars);
            MainManager.instance.prizeenemyids = prizeenemyids.ToArray();

            var prizeflags = MainManager.instance.prizeflags.ToList();
            prizeflags.Add((int)NewFlagVar.Aria_Reward);
            prizeflags.Add((int)NewFlagVar.TermiteKnight_Reward);
            prizeflags.Add((int)NewFlagVar.LeafbugShaman_Reward);
            prizeflags.Add((int)NewFlagVar.Patton_Reward);
            prizeflags.Add((int)NewFlagVar.TeamCelia_Reward);
            prizeflags.Add((int)NewFlagVar.Mars_Reward);
            MainManager.instance.prizeflags = prizeflags.ToArray();

            var prizeids = MainManager.instance.prizeids.ToList();
            prizeids.Add((int)Medal.Yawn);
            prizeids.Add((int)Medal.IgnitedMite);
            prizeids.Add((int)Medal.TribalDance);
            prizeids.Add((int)Medal.Adrenaline);
            prizeids.Add((int)Medal.MPPlus);
            prizeids.Add((int)Medal.Mothflower);
            MainManager.instance.prizeids = prizeids.ToArray();

            MainManager_Ext.SetMenuText();
            MainManager_Ext.SetEnemyData();
            MainManager_Ext.SetEnemyPortrait();
            MainManager_Ext.SetQuestChecks();

            MainManager.instance.flags = new bool[MainManager_Ext.FlagNumber];

            MainManager.instance.flags[411] = true;
            MainManager.instance.flags[414] = true;

            Sprite[] sprites = MainManager_Ext.assetBundle.LoadAllAssets<Sprite>().Where(sprite => sprite.name.Split('_')[0] == "medal").OrderBy(sprite => sprite.name.Split('_')[1]).ToArray();
            for (int i = 0; i != sprites.Length; i++)
                MainManager.itemsprites[1, int.Parse(sprites[i].name.Split('_')[1])] = sprites[i];

            sprites = MainManager_Ext.assetBundle.LoadAllAssets<Sprite>().Where(sprite => sprite.name.Split('_')[0] == "item").OrderBy(sprite => sprite.name.Split('_')[1]).ToArray();
            for (int i = 0; i != sprites.Length; i++)
                MainManager.itemsprites[0, int.Parse(sprites[i].name.Split('_')[1])] = sprites[i];

            List<Sprite> newsprites = new List<Sprite>(MainManager.guisprites);
            sprites = MainManager_Ext.assetBundle.LoadAllAssets<Sprite>().Where(sprite => sprite.name.Split('_')[0] == "newGui").OrderBy(sprite => sprite.name.Split('_')[1]).ToArray();
            newsprites.AddRange(sprites);
            MainManager.guisprites = newsprites.ToArray();

            Sprite newStickyIcon = MainManager_Ext.assetBundle.LoadAsset<Sprite>("newGui_246");

            MainManager.guisprites[199] = newStickyIcon;
            MainManager.instance.conditionsprites[20] = newStickyIcon;

            MainManager.commandhelptext = MainManager.commandhelptext.AddRangeToArray(MainManager_Ext.assetBundle.LoadAsset<TextAsset>("ActionCommands").ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));
            MainManager.musicnames = MainManager.musicnames.AddRangeToArray(MainManager_Ext.assetBundle.LoadAsset<TextAsset>("MusicList").ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));
            MainManager.commondialogue = MainManager.commondialogue.AddRangeToArray(MainManager_Ext.assetBundle.LoadAsset<TextAsset>("CommonDialogues").ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));

            MainManager.musicnames[1] = "FIGHT!";
        }
    }


    [HarmonyPatch(typeof(MainManager), "Load")]
    public class PatchMainManagerLoad
    {
        /*static Exception Finalizer()
        {
            return null;
        }*/
        static void Postfix(ref MainManager.LoadData? __result, int file, bool lite)
        {
            if (MainManager.instance != null && __result != null)
            {
                string[] data = new string[] { string.Empty };
                if (InputIO.IsConsole)
                    data = InputIO.ReadFile("save" + file + ".dat").Split('\n');
                else
                {
                    try
                    {
                        data = InputIO.Encrypt(InputIO.ReadFile("save" + file + ".dat")).Split('\n');
                    }
                    catch
                    {
                        Console.WriteLine("Cant load file for some reason :/");
                        return;
                    }
                }

                if (lite)
                {

                    string[] header = data[0].Split(',');
                    MainManager.LoadData value = __result.Value;
                    if (__result.Value.challenges.Length < 7)
                    {
                        var temp = __result.Value.challenges.ToList();
                        for (int i = 0; i < MainManager_Ext.newUnlocks.Length; i++)
                            temp.Add(false);
                        value.challenges = temp.ToArray();
                    }

                    for (int i = 0; i < MainManager_Ext.newUnlocks.Length; i++)
                    {
                        if (header.Length > 10 + i)
                        {
                            value.challenges[6 + i] = header[10 + i] == "True";
                        }
                    }
                    __result = value;

                }
                else
                {
                    MainManager.instance.flags = new bool[MainManager_Ext.FlagNumber];
                    MainManager.instance.crystalbflags = new bool[MainManager_Ext.CBFlagNumber];
                    MainManager.instance.flagvar = new int[MainManager_Ext.FlagVarNumber];
                    string[] flags = data[11].Split(',');

                    for (int i = 0; i < flags.Length; i++)
                    {
                        if (i < MainManager.instance.flags.Length)
                            MainManager.instance.flags[i] = Convert.ToBoolean(flags[i]);
                    }

                    string[] cbFlags = data[15].Split(',');
                    for (int i = 0; i < cbFlags.Length; i++)
                    {
                        if (i < MainManager.instance.crystalbflags.Length)
                            MainManager.instance.crystalbflags[i] = Convert.ToBoolean(cbFlags[i]);
                    }

                    string[] flagvar = data[13].Split(new char[] { ',' });
                    for (int i = 0; i < flagvar.Length; i++)
                    {
                        MainManager.instance.flagvar[i] = Convert.ToInt32(flagvar[i]);
                    }

                    MainManager.instance.flags[411] = true;
                    MainManager.instance.flags[414] = true;
                    BattleControl_Ext.stylishBarAmount = 0;
                    BattleControl_Ext.stylishReward = StylishReward.None;
                    MainManager_Ext.SetupNewShops();

                    if (!MainManager.instance.flags[901])
                    {
                        var superbosses = MainManager_Ext.GetSuperBosses();
                        for (int i = 0; i < superbosses.Length; i++)
                        {
                            if (superbosses[i] == -2)
                                superbosses[i] = (int)MainManager.Enemies.HoloVi;

                            if (MainManager.instance.enemyencounter[superbosses[i], 0] > 0)
                            {
                                MainManager.instance.flags[901] = true;
                                break;
                            }
                        }
                    }

                    if (flags.Length < 900)
                    {
                        if (MainManager.instance.flags[627])
                        {
                            MainManager.instance.badgeshops[0].Add((int)Medal.SleepSchedule);
                        }
                        else
                        {
                            MainManager.instance.badgeshops[0].Add((int)MainManager.BadgeTypes.TPPlus);
                        }

                        if (MainManager.instance.flags[103])
                        {
                            MainManager.AddPrizeMedal((int)NewPrizeFlag.Aria);
                        }

                        if (MainManager.instance.flags[299])
                        {
                            MainManager.instance.badgeshops[1].Add((int)Medal.ChargeGuard);
                        }

                        if (MainManager.instance.flags[348])
                        {
                            MainManager.instance.badgeshops[1].Add((int)Medal.Recharge);
                        }

                        MainManager.instance.badgeshops[0].Add((int)Medal.HPDown);
                        MainManager.instance.badgeshops[0].Add((int)Medal.MPPlus);
                        MainManager.instance.badgeshops[0].Add((int)Medal.TimingTutor);
                        MainManager.instance.badgeshops[1].Add((int)Medal.HPDown);
                        MainManager.instance.badgeshops[1].Add((int)Medal.Powerbank);
                        MainManager.instance.badgeshops[1].Add((int)Medal.RevitalizingRipple);
                        MainManager.instance.badgeshops[1].Add((int)Medal.PurifyingPulse);
                        MainManager.instance.flags[587] = false;
                        MainManager.instance.flags[588] = false;
                        MainManager.instance.flags[612] = false;
                        if (MainManager.instance.flags[411])
                        {
                            MainManager.instance.flags[238] = false;
                        }

                        //our job is done flag
                        MainManager.instance.flags[63] = false;

                        //completed all quests
                        MainManager.instance.flags[671] = false;

                        int[] recordsId = { 1,2,3,4,7,8,9,10,27,28 };
                        
                        foreach(var record in recordsId)
                        {
                            MainManager.instance.librarystuff[(int)MainManager.Library.Logbook, record] = false;
                        }

                        Console.WriteLine("save is new to bf plus, adding medals in badge shop, resetting records flags");
                    }
                    else
                    {
                        if (data.Length > 18)
                        {
                            string[] stylishData = data[18].Split(new char[] { '@' });
                            BattleControl_Ext.stylishBarAmount = float.Parse(stylishData[0]);

                            if (Enum.TryParse(stylishData[1], out StylishReward reward))
                            {
                                BattleControl_Ext.stylishReward = reward;
                            }
                        }
                    }
                }
            }

        }
    }

    [HarmonyPatch(typeof(MainManager), "SaveFile")]
    public class PatchMainManagerSaveFile
    {
        static void Postfix(ref string __result)
        {
            string[] data = __result.Split('\n');
            foreach (int code in Enum.GetValues(typeof(NewCode)))
            {
                data[0] = data[0] + ',' + MainManager.instance.flags[code];
            }
            List<string> temp = data.ToList();
            temp.Add(BattleControl_Ext.stylishBarAmount + "@" + BattleControl_Ext.stylishReward);
            __result = String.Join("\n", temp.ToArray());
        }
    }

    [HarmonyPatch(typeof(MainManager), "DoItemEffect")]
    public class PatchMainManagerDoItemEffect
    {
        static int[] conditionAmount = new int[3];
        static void Prefix(MainManager.ItemUsage type, ref int value, int? characterid)
        {
            if (MainManager.instance.inbattle && MainManager.battle != null)
            {
                for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                {
                    conditionAmount[i] = MainManager.instance.playerdata[i].condition.Count;
                }

                if (characterid != null)
                {
                    if (!MainManager.battle.enemy && MainManager.HasCondition(MainManager.BattleCondition.Sticky, MainManager.instance.playerdata[MainManager.battle.currentturn]) > -1 && MainManager.BadgeIsEquipped((int)Medal.FlavorlessAdhesive))
                    {
                        MainManager.ItemUsage[] bannedUsage = {
                        (MainManager.ItemUsage)NewItemUse.MultiUse, (MainManager.ItemUsage)NewItemUse.MultiUseRandom,
                        MainManager.ItemUsage.Battle, MainManager.ItemUsage.None,
                    };

                        if (!bannedUsage.Contains(type))
                        {
                            value = Mathf.Clamp(Mathf.CeilToInt(value / 2),1,99);
                        }
                    }

                    if (MainManager.BadgeIsEquipped((int)Medal.FlavorCharger))
                    {
                        BattleControl_Ext.Instance.CheckFlavorCharger(type, characterid);
                    }
                }
            }

            if (Enum.TryParse<NewItemUse>(type.ToString(), out NewItemUse result))
            {
                MainManager_Ext.DoNewItemUse(result, value, characterid);
            }
        }

        static void Postfix(MainManager.ItemUsage type, ref int value, int? characterid)
        {
            if (MainManager.instance.inbattle && MainManager.battle != null)
            {
                List<MainManager.ItemUsage> cureType = new List<MainManager.ItemUsage>() { MainManager.ItemUsage.CureAll, MainManager.ItemUsage.CureFire, MainManager.ItemUsage.CureFreeze, MainManager.ItemUsage.CureNumb, MainManager.ItemUsage.CureParty, MainManager.ItemUsage.CurePoison, MainManager.ItemUsage.CurePoisonAll, MainManager.ItemUsage.CureSleep };

                if (cureType.Contains(type))
                {
                    for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                    {
                        if (conditionAmount[i] > MainManager.instance.playerdata[i].condition.Count)
                        {
                            int amountCleared = conditionAmount[i] - MainManager.instance.playerdata[i].condition.Count;

                            if (amountCleared > 0 && MainManager.instance.playerdata[i].hp > 0)
                            {
                                BattleControl_Ext.Instance.DoPurifyingPulseCheck(ref MainManager.instance.playerdata[i], amountCleared);
                                BattleControl_Ext.Instance.DoRevitalizingRippleCheck(ref MainManager.instance.playerdata[i], amountCleared);
                            }
                        }
                    }

                }
            }
        }
    }
    [HarmonyPatch(typeof(MainManager), "SetCondition", new Type[] { typeof(MainManager.BattleCondition), typeof(MainManager.BattleData), typeof(int), typeof(int) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal })]
    public class PatchMainManagerSetCondition
    {
        static bool Prefix(ref MainManager.BattleCondition condition, ref MainManager.BattleData entity, ref int turns, int fromplayer)
        {
            if(condition == MainManager.BattleCondition.Sturdy)
            {
                if(entity.delayedcondition != null)
                {
                    entity.delayedcondition.Clear();

                    if (entity.frostbitep != null)
                        UnityEngine.Object.Destroy(entity.frostbitep.gameObject);
                }
            }


            if (BattleControl_Ext.Instance.IsStatusImmune(entity, condition))
            {
                return false;
            }

            //if the bug has sticky any other condition gets buffed by 1 turn.
            if (MainManager.HasCondition(MainManager.BattleCondition.Sticky, entity) > -1)
            {
                int stickyBuff = 1 + MainManager.BadgeHowManyEquipped((int)Medal.SturdyStrands);
                if (turns > 0)
                    turns += stickyBuff;
                else
                    turns -= stickyBuff;
            }
            return true;
        }

        static void Postfix(MainManager.BattleCondition condition, ref MainManager.BattleData entity, int turns, int fromplayer)
        {
            var animid = entity.animid;
            Func<int, bool> medalCondition = (i) => animid != MainManager.instance.playerdata[i].animid && MainManager.HasCondition(MainManager.BattleCondition.Sleep, MainManager.instance.playerdata[i]) == -1;
            int yawnBug = BattleControl_Ext.GetEquippedMedalBug(Medal.Yawn, medalCondition);
            if (condition == MainManager.BattleCondition.Sleep && yawnBug != -1 && MainManager.HasCondition(MainManager.BattleCondition.Sturdy, MainManager.instance.playerdata[yawnBug]) == -1)
            {
                MainManager.SetCondition(MainManager.BattleCondition.Sleep, ref MainManager.instance.playerdata[yawnBug], 2);
            }

            var entityExt = Entity_Ext.GetEntity_Ext(entity.battleentity);
            if (MainManager.HasCondition(MainManager.BattleCondition.Inked, entity) > -1 && !entityExt.inkDebuffed)
            {
                entityExt.CheckInkDebuff(ref entity);
                entityExt.permanentInkTriggered = false;
            }
        }
    }

    [HarmonyPatch(typeof(MainManager), "GetRandomMedal", new Type[] { typeof(bool), typeof(bool) })]
    public class PatchMainManagerGetRandomMedal
    {
        static void Prefix(bool dontremove, bool random)
        {
            if (dontremove && random)
            {
                var randomMedals = new List<int>(Enum.GetValues(typeof(Medal)).Cast<int>());
                randomMedals.Remove((int)Medal.TPComa);
                randomMedals.Remove((int)Medal.EverlastingFlame);
                randomMedals.AddRange(MainManager_Ext.medalDupes.Cast<int>());
                var stringBadges = ',' + String.Join(",", Array.ConvertAll(randomMedals.ToArray(), x => x.ToString()));
                MainManager.instance.flagstring[13] += stringBadges;
            }
        }
    }

    [HarmonyPatch(typeof(MainManager), "CreateHUD")]
    public class PatchMainManagerCreateHUD
    {
        static void Postfix()
        {
            MainManager_Ext.Instance.CheckSwitcheroo();
        }
    }

    [HarmonyPatch(typeof(MainManager), "PlaySound", new Type[] { typeof(AudioClip), typeof(int), typeof(float), typeof(float), typeof(bool) })]
    public class PatchMainManagerPlaySound
    {
        static void Prefix(ref AudioClip soundclip)
        {
            MainManager_Ext.CheckGamerFX(ref soundclip);
        }
    }

    [HarmonyPatch(typeof(EntityControl), "PlaySound", new Type[] { typeof(AudioClip), typeof(float), typeof(float) })]
    public class PatchEntityControlPlaySound
    {
        static void Prefix(ref AudioClip clip)
        {
            MainManager_Ext.CheckGamerFX(ref clip);
        }
    }


    [HarmonyPatch(typeof(MainManager), "GetPlayerDataNullable")]
    public class PatchMainManagerGetPlayerDataNullable
    {
        static void Prefix(ref int id)
        {
            if (BattleControl_Ext.Instance.holoSkillID != -1)
            {
                id = MainManager.battle.currentturn;
            }
        }
    }

    [HarmonyPatch(typeof(MainManager), "Update")]
    public class PatchMainManagerUpdate
    {
        static void Prefix()
        {
            //dirty fix for weird scale on text holder when big fable code
            if (MainManager.instance.message && MainManager.instance.flags[(int)NewCode.BIGFABLE])
            {
                if (MainManager.instance.textbox != null)
                {
                    MainManager.instance.textbox.transform.localScale = Vector3.one;
                }
            }
        }
    }


    [HarmonyPatch(typeof(MainManager), "ChangeMusic", new Type[] { typeof(string) })]
    public class PatchMainManagerChangeMusicCheck
    {
        static bool Prefix(string musicclip)
        {
            return MainManager_Ext.CheckNewMusic(musicclip, 0.1f, 0, false);
        }
    }

    [HarmonyPatch(typeof(MainManager), "ChangeMusic", new Type[] { typeof(string), typeof(float) })]
    public class PatchMainManagerChangeMusicCheck2
    {
        static bool Prefix(string musicclip, float fadespeed)
        {
            return MainManager_Ext.CheckNewMusic(musicclip, fadespeed, 0, false);
        }
    }

    [HarmonyPatch(typeof(MainManager), "ChangeMusic", new Type[] { typeof(string), typeof(float), typeof(int) })]
    public class PatchMainManagerChangeMusicCheck3
    {
        static bool Prefix(string musicclip, float fadespeed, int id)
        {
            return MainManager_Ext.CheckNewMusic(musicclip, fadespeed, id, false);
        }
    }

    [HarmonyPatch(typeof(MainManager), "ChangeMusic", new Type[] { typeof(string), typeof(float), typeof(int), typeof(bool) })]
    public class PatchMainManagerChangeMusicCheck4
    {
        static bool Prefix(string musicclip, float fadespeed, int id, bool seamless)
        {
            return MainManager_Ext.CheckNewMusic(musicclip, fadespeed, id, seamless);
        }
    }

    [HarmonyPatch(typeof(MainManager), "SamiraGotAll")]
    public class PatchMainManagerSamiraGotAll
    {
        static void Postfix(ref bool __result)
        {
            __result = MainManager.PurchasedMusicAmmount() >= Enum.GetNames(typeof(MainManager.Musics)).Length - 8 + Enum.GetNames(typeof(NewMusic)).Length -4;
        }
    }

    [HarmonyPatch(typeof(MainManager), "RemoveCondition")]
    public class PatchMainManagerRemoveCondition
    {
        static bool Prefix(MainManager.BattleCondition condition, MainManager.BattleData entity)
        {
            if (MainManager.BadgeIsEquipped((int)Medal.PermanentInk) && condition == MainManager.BattleCondition.Inked)
            {
                return false;
            }

            if (MainManager.BadgeIsEquipped((int)Medal.SturdyStrands) && condition == MainManager.BattleCondition.Sticky)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(MainManager), "CheckAchievement")]
    public class PatchMainManagerCheckAchievement
    {
        static void Prefix(MainManager __instance)
        {
            if (__instance.started)
            {
                if (!__instance.librarystuff[3, (int)NewAchievement.HDWGH] && __instance.flagvar[(int)NewFlagVar.MaxConditions] >= MainManager_Ext.HDWGH_CONDITIONS)
                {
                    MainManager.UpdateJounal(MainManager.Library.Logbook, (int)NewAchievement.HDWGH);
                }

                if (!__instance.librarystuff[3, (int)NewAchievement.OverKill] && __instance.flagvar[41] >= 100)
                {
                    MainManager.UpdateJounal(MainManager.Library.Logbook, (int)NewAchievement.OverKill);
                }

                if (!__instance.librarystuff[3, (int)NewAchievement.WellRested] && __instance.flags[913])
                {
                    MainManager.UpdateJounal(MainManager.Library.Logbook, (int)NewAchievement.WellRested);
                }

                if (!__instance.librarystuff[3, (int)NewAchievement.UndergroundExplorer] && __instance.flags[857])
                {
                    MainManager.UpdateJounal(MainManager.Library.Logbook, (int)NewAchievement.UndergroundExplorer);
                }
            }
        }
    }

    [HarmonyPatch(typeof(InputIO), "Achivement")]
    public class PatchInputIOAchivement
    {
        static bool Prefix(int id)
        {
            if (id < InputIO.achivements.Length)
            {
                return true;
            }
            return false;
        }
    }

    //force english
    [HarmonyPatch(typeof(InputIO), "LoadSettings")]
    public class PatchInputIOLoadSettings
    {
        static void Prefix()
        {
            MainManager.languageid = 0;
        }
    }


    [HarmonyPatch(typeof(MainManager), "ShowHUD")]
    public class PatchMainManagerShowHUD
    {
        static bool Prefix(bool show)
        {
            if (MainManager.instance.flags[916])
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(MainManager), "UpdateArea")]
    public class PatchMainManagerUpdateArea
    {
        static bool Prefix(int newarea)
        {
            if (MainManager.instance.flags[916])
            {
                return false;
            }
            return true;
        }
    }
}


