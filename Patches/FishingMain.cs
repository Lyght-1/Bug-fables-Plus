using BFPlus.Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Patches
{
    [HarmonyPatch(typeof(FishingMain), "Start")]
    public class PatchFishingMainStart
    {
        static void Prefix(FishingMain __instance)
        {
            if (!MainManager.instance.flags[616])
            {
                for (int i = 0; i < __instance.party.Length; i++) 
                {
                    if(i <= 2)
                    {
                        if (MainManager.BadgeIsEquipped((int)Medal.Switcheroo))
                        {
                            int animId = 0;

                            if (i > 0)
                            {
                                animId = i == 1 ? 2 : 1;
                            }

                            __instance.party[i].runtimeAnimatorController = MainManager_Ext.Instance.GetSwitcherooAnim(animId);
                        }
                    }
                }
            }
        }
    }
}
