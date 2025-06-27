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

namespace BFPlus.Patches.BattleControlTranspilers.SetItemPatches
{

    public class PatchCheckHoloSkill : PatchBaseSetItem
    {
        public PatchCheckHoloSkill()
        {
            priority = 191596;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchLdcI4(6), i => i.MatchBneUn(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckHoloSkill"));
            Utils.RemoveUntilInst(cursor, i => i.MatchBr(out _));
        }

    }
}
