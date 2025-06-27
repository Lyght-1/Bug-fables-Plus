using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BFPlus.Extensions;

namespace BFPlus.Patches
{
    [HarmonyPatch(typeof(PlayerControl), "DashBehavior")]
    public class PatchPlayerControlDashBehavior
    {
        static void Prefix(PlayerControl __instance)
        {
			if (MainManager.GetKey(7, false) && __instance.canpause)
			{
				if (MainManager.instance.hudcooldown <= 0f)
				{
					MainManager.PlaySound("HudDown", 13, 1f, 0.15f);
					MainManager.instance.hudcooldown = 300f;
					MainManager.instance.showmoney = 300f;
				}
				else
				{
					MainManager.PlaySound("HudUp", 13, 1f, 0.15f);
					MainManager.instance.hudcooldown = 1f;
					if (MainManager.instance.money == MainManager.instance.moneyt)
					{
						MainManager.instance.showmoney = 1f;
					}
				}
			}
		}
    }

    [HarmonyPatch(typeof(PlayerControl), "DoActionHold")]
    public class PatchPlayerControlDoActionHold
    {
        static bool Prefix(PlayerControl __instance)
        {
			if (MainManager_Ext.inSeedlingMinigame)
				return false;
			return true;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), "Start")]
    public class PatchPlayerControlStart
    {
        static void Postfix(PlayerControl __instance)
        {
            if (MainManager_Ext.Instance.musicPlayer)
            {
                MainManager_Ext.CreateMusicParticle();
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), "DoJump")]
    public class PatchPlayerControlDoJump
    {
        static bool Prefix(PlayerControl __instance)
        {
            if (MainManager_Ext.noJump)
                return false;
            return true;
        }
    }

}
