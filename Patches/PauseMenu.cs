using HarmonyLib;
using System;
using System.IO;
using UnityEngine;
using BFPlus.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using InputIOManager;
using System.Reflection;

namespace BFPlus.Patches
{
    [HarmonyPatch(typeof(PauseMenu), "Start")]
    public class PatchPauseMenuStart
    {
        static void Postfix(PauseMenu __instance)
        {
            PauseMenu_Ext.SetEnemyData();
        }
    }

    [HarmonyPatch(typeof(PauseMenu), "ChangeWindow")]
    public class PatchPauseMenuChangeWindow
    {
        static void prefix(PauseMenu __instance)
        {
            PauseMenu_Ext.Instance.chooseMedalCategory = -1;
            PauseMenu_Ext.Instance.DestroyMedalCategoryIcon();
        }
    }

    [HarmonyPatch(typeof(PauseMenu), "UseKeyItem")]
    public class PatchPauseMenuUseKeyItem
    {
        static bool Prefix(PauseMenu __instance)
        {
            if (MainManager.instance.items[__instance.option].ToArray()[MainManager.instance.option] == (int)NewItem.MusicPlayer)
            {
                MainManager.PlaySound("Confirm", 10);
                UnityEngine.Object.Destroy(MainManager.pausemenu.gameObject);
                MainManager.instance.pause = false;
                if (MainManager.instance.cursor != null)
                {
                    UnityEngine.Object.Destroy(MainManager.instance.cursor.gameObject);
                }
                MainManager.ResetList();
                __instance.StartCoroutine(MainManager.SetText("|hide||event,401|", null, null));
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PauseMenu), "UpdateText")]
    public class PatchPauseMenuUpdateText
    {
        static void Prefix(PauseMenu __instance)
        {
            MainManager_Ext.Instance.CheckSwitcheroo();

            if(__instance.windowid == 6)
            {
                Transform textHolder = __instance.boxes[0].transform.GetChild(0).GetChild(0);
                MainManager.DestroyText(textHolder);
                if (__instance.option > -1)
                {
                    int cbPercent = PauseMenu_Ext.Instance.areaItems[__instance.option].GetCBPercent();
                    __instance.StartCoroutine(MainManager.SetText(
                        "|sort,30||size,0.45,0.4||icon,83|", 
                        new Vector3(-2.8f,-0.3f), textHolder));
                    __instance.StartCoroutine(MainManager.SetText(
                        "|sort,30||size,0.7|" + (cbPercent.ToString() + "%").PadLeft(4),
                        new Vector3(-2.15f, -0.13f), textHolder));

                    int discPercent = PauseMenu_Ext.Instance.areaItems[__instance.option].GetDiscoveriesPercent();
                    __instance.StartCoroutine(MainManager.SetText(
                        "|sort,30||size,0.26,0.21||icon,78|",
                        new Vector3(-1f, -0.6f), textHolder));
                    __instance.StartCoroutine(MainManager.SetText(
                        "|sort,30||size,0.7|" + (discPercent.ToString() + "%").PadLeft(4),
                        new Vector3(-0.21f, -0.13f), textHolder));

                    int lorePercent = PauseMenu_Ext.Instance.areaItems[__instance.option].GetLoreBookPercent();
                    __instance.StartCoroutine(MainManager.SetText(
                        $"|sort,30||size,0.65,0.55||icon,{(int)NewGui.LoreBook}|",
                        new Vector3(1.13f, -0.2f), textHolder));
                    __instance.StartCoroutine(MainManager.SetText(
                        "|sort,30||size,0.7|" + (lorePercent.ToString() + "%").PadLeft(4),
                        new Vector3(1.68f, -0.13f), textHolder));
                }
            }
        }
    }
}
