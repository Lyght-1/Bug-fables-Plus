using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Patches.BattleControlTranspilers
{
    public class PatchBattleControlRelay : PatchBaseBattleControlRelay
    {
        public PatchBattleControlRelay()
        {
            priority = 21;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(45));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchBattleControlRelay), "CheckSpiderBait"));


            //Add check max charge for status relay with charges
            cursor.GotoNext(i => i.MatchAdd(), i => i.MatchLdcI4(0), i => i.MatchLdcI4(3));
            cursor.GotoNext(i => i.MatchLdcI4(3));
            cursor.Emit(OpCodes.Ldloc_1);
            cursor.Emit(OpCodes.Ldfld, AccessTools.Field(typeof(BattleControl), "option"));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "CheckMaxCharge"));
            cursor.Remove();
        }

        static void CheckSpiderBait()
        {
            if (MainManager.HasCondition(MainManager.BattleCondition.Sticky, MainManager.instance.playerdata[MainManager.battle.currentturn]) > -1 && MainManager.BadgeIsEquipped((int)Medal.SpiderBait, MainManager.instance.playerdata[MainManager.battle.currentturn].trueid))
            {
                MainManager.battle.StartCoroutine(MainManager.battle.ItemSpinAnim(MainManager.instance.playerdata[MainManager.battle.currentturn].battleentity.transform.position + Vector3.up, MainManager.itemsprites[1, (int)Medal.SpiderBait], true));
                MainManager.PlaySound("StatUp", -1, 1.25f, 1f);
                MainManager.battle.StartCoroutine(MainManager.battle.StatEffect(MainManager.instance.playerdata[MainManager.battle.option].battleentity, 4));
                MainManager.instance.playerdata[MainManager.battle.option].charge = Mathf.Clamp(MainManager.instance.playerdata[MainManager.battle.option].charge + 1, 0, MainManager_Ext.CheckMaxCharge(MainManager.battle.option));
            }
        }
    }
}
