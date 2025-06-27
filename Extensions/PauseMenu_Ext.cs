using HarmonyLib;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using BFPlus.Patches;

namespace BFPlus.Extensions
{
    class PauseMenu_Ext : MonoBehaviour
    {
        public AreaItem[] areaItems;
        public MedalCategory[] medalCategories;
        public Sprite[] categoryIcons;
        public GameObject medalCategoryIcon = null;
        public int chooseMedalCategory = -1;
        public static PauseMenu_Ext Instance
        {
            get
            {
                if (MainManager.pausemenu.GetComponent<PauseMenu_Ext>() == null)
                {
                    return MainManager.pausemenu.gameObject.AddComponent<PauseMenu_Ext>();
                }
                return MainManager.pausemenu.GetComponent<PauseMenu_Ext>();
            }
        }

        static void CheckFastTextState()
        {
            int settingID = MainManager.settingsindex[MainManager.listvar[MainManager.instance.option]];
            if (settingID == (int)NewMenuText.TextSkip)
            {
                MainManager.pausemenu.SettingsToggleSound();
                MainManager_Ext.fastText = !MainManager_Ext.fastText;
                MainManager.pausemenu.UpdateText();
            }

            if (settingID == (int)NewMenuText.ShowResistance)
            {
                MainManager.pausemenu.SettingsToggleSound();
                MainManager_Ext.showResistance = !MainManager_Ext.showResistance;
                MainManager.pausemenu.UpdateText();
            }

            if (settingID == (int)NewMenuText.NewBattleThemes)
            {
                MainManager.pausemenu.SettingsToggleSound();
                MainManager_Ext.newBattleThemes = !MainManager_Ext.newBattleThemes;
                MainManager.pausemenu.UpdateText();
            }
        }

        public static void SetEnemyData()
        {
            MainManager.pausemenu.enemydata = MainManager_Ext.enemyData;
        }

        static void SetBigFableIcon(List<Transform> icons)
        {
            if (MainManager.instance.flags[(int)NewCode.BIGFABLE])
            {
                icons.Add(MainManager.NewUIObject("bigFable", MainManager.pausemenu.boxes[1].transform, new Vector3(2f, 4.5f, -0.1f), Vector3.one * 0.8f, MainManager.itemsprites[0,142]).transform);
            }

            if (MainManager.instance.flags[(int)NewCode.EVEN])
            {
                icons.Add(MainManager.NewUIObject("even", MainManager.pausemenu.boxes[1].transform, new Vector3(2f, 4.5f, -0.1f), Vector3.one * 0.8f, MainManager.itemsprites[0, 102]).transform);
            }

            if (MainManager.instance.flags[(int)NewCode.COMMAND])
            {
                icons.Add(MainManager.NewUIObject("command", MainManager.pausemenu.boxes[1].transform, new Vector3(2f, 4.5f, -0.1f), Vector3.one * 0.6f, MainManager.guisprites[13]).transform);
            }

            if (MainManager.instance.flags[(int)NewCode.SCAVENGE])
            {
                icons.Add(MainManager.NewUIObject("scavenge",  MainManager.pausemenu.boxes[1].transform, new Vector3(2f, 4.5f, -0.1f), Vector3.one * 0.5f, MainManager.guisprites[22]).transform);
            }
        }


        static bool CanDequip(int[] medal)
        {
            return !(medal[0] == (int)Medal.MPPlus && MainManager.instance.bp < 3);
        }

        static bool CanEquip(int[] medal)
        {
            return (medal[0] == (int)Medal.MPPlus && MainManager.instance.maxtp > 3) || medal[0] != (int)Medal.MPPlus;
        }

        public void SetupAreaItemData()
        {
            string[] allAreasDatas = MainManager_Ext.assetBundle.LoadAsset<TextAsset>("AreaItemsData").ToString().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            areaItems = new AreaItem[allAreasDatas.Length];
            for(int i=0;i <allAreasDatas.Length; i++)
            {
                areaItems[i] = new AreaItem();
                string[] areaData = allAreasDatas[i].Split(new char[] { '{' });
                if (areaData[0].Length > 0 && areaData[0] != "")
                {
                    areaItems[i].crystalBerries = areaData[0].Split(',').Select(int.Parse).ToArray();
                }

                if (areaData[1].Length > 0 && areaData[1] != "")
                {
                    areaItems[i].discoveries = areaData[1].Split(',').Select(int.Parse).ToArray();
                }

                if (areaData[2].Length > 0 && areaData[2] != "")
                {
                    areaItems[i].loreBooks = areaData[2].Split(',').Select(int.Parse).ToArray();
                }
            }
        }

        public void SetupMedalCategoriesData()
        {
            string[] allMedalCategories = MainManager_Ext.assetBundle.LoadAsset<TextAsset>("MedalCategories").ToString().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            medalCategories = new MedalCategory[allMedalCategories.Length];
            for (int i = 0; i < allMedalCategories.Length; i++)
            {
                medalCategories[i] = new MedalCategory();
                string[] categoryData = allMedalCategories[i].Split(new char[] { ';' });
                medalCategories[i].name = categoryData[0];
                medalCategories[i].iconId = int.Parse(categoryData[1]);

                var sizes = categoryData[2].Split(',').Select(float.Parse).ToArray();
                medalCategories[i].iconSize = new Vector2(sizes[0], sizes[1]);

                if (categoryData[3].Length > 0 && categoryData[3] != "")
                {
                    medalCategories[i].medals = categoryData[3].Split(',').Select(int.Parse).ToArray();
                }
            }

            if(categoryIcons == null)
            {
                categoryIcons = MainManager_Ext.assetBundle.LoadAssetWithSubAssets<Sprite>("medalIcons").OrderBy(s => int.Parse(s.name.Split('_')[1])).ToArray();
            }
        }

        public int[] GetObtainedCategories()
        {
            if (medalCategories == null)
                SetupMedalCategoriesData();

            List<int> obtainedCategories = new List<int>() { 0 };

            for (int i = 0; i< medalCategories.Length; i++)
            {
                for (int j = 0; j < MainManager.instance.badges.Count; j++)
                {
                    if (medalCategories[i].medals != null && medalCategories[i].medals.Contains(MainManager.instance.badges[j][0]))
                    {
                        obtainedCategories.Add(i);
                        break;
                    }
                }
            }
            return obtainedCategories.ToArray();
        }

        public int[] GetCategoryMedals()
        {
            if (MainManager.instance.badges.Count != 0)
            {
                MedalCategory category = medalCategories[MainManager.listvar[MainManager.instance.option]];
                List<int> list = new List<int>();
                int[][] array = MainManager.instance.badges.ToArray();

                for (int i = 0; i < array.Length; i++)
                {
                    if ((category.medals != null && category.medals.Contains(array[i][0])) || MainManager.listvar[MainManager.instance.option] == 0)
                    {
                        list.Add(i);
                    }
                }
                return list.ToArray();
            }
            return new int[0];
        }

        public void DestroyMedalCategoryIcon()
        {
            if (Instance.medalCategoryIcon != null)
            {
                Destroy(Instance.medalCategoryIcon);
            }
        }

        public class MedalCategory
        {
            public int[] medals;
            public string name;
            public int iconId;
            public Vector2 iconSize;
        }

        public class AreaItem
        {
            public int[] crystalBerries;
            public int[] loreBooks;
            public int[] discoveries;

            public int GetCBPercent()
            {
                if (crystalBerries != null)
                {
                    int count = 0;
                    for (int i = 0; i < crystalBerries.Length; i++)
                    {
                        if (MainManager.instance.crystalbflags[crystalBerries[i]])
                        {
                            count++;
                        }
                    }
                    return (int)((float)count / crystalBerries.Length * 100);
                }
                return 100;
            }

            public int GetDiscoveriesPercent()
            {
                if (discoveries != null)
                {
                    int count = 0;
                    for (int i = 0; i < discoveries.Length; i++)
                    {
                        if (MainManager.instance.librarystuff[0, discoveries[i]])
                        {
                            count++;
                        }
                    }
                    return (int)((float)count / discoveries.Length * 100);
                }
                return 100;
            }
            //had the data wrong, i need to do flag based
            public int GetLoreBookPercent()
            {
                if (loreBooks != null)
                {
                    int count = 0;
                    for (int i = 0; i < loreBooks.Length; i++)
                    {
                        if (MainManager.instance.flags[loreBooks[i]])
                        {
                            count++;
                        }
                    }
                    return (int)((float)count / loreBooks.Length * 100);
                }
                return 100;
            }
        }
    }
}
