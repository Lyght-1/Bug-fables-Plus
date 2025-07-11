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

namespace BFPlus.Patches.BattleControlTranspilers.DoActionPatches.SkillPatches
{
    /// <summary>
    /// Ice rain hits 4 => 3
    /// </summary>
    public class PatchLeifIceRainHits : PatchBaseDoAction
    {
        public PatchLeifIceRainHits()
        {
            priority = 49495;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdstr("crosshair"));
            cursor.GotoNext(i => i.MatchLdcI4(4));
            cursor.Emit(OpCodes.Ldc_I4, 3);
            cursor.Remove();
        }
    }


    /// <summary>
    /// We reset the icerain hits count and we make ice rain cost 2 turns
    /// </summary>
    public class PatchLeifResetIceRainHits : PatchBaseDoAction
    {
        public PatchLeifResetIceRainHits()
        {
            priority = 49796;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdcI4(102), i=>i.MatchStfld(out _));
            cursor.GotoNext(i => i.MatchLdcI4(65));
            cursor.GotoNext(i => i.MatchLdnull(), i => i.MatchStfld(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchLeifResetIceRainHits), "ResetIceRainHits"));
        
        }

        static void ResetIceRainHits()
        {
            MainManager.instance.playerdata[MainManager.battle.currentturn].cantmove++;
            BattleControl_Ext.Instance.iceRainHits = 0;
        }
    }
}
