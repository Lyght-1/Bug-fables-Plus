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

namespace BFPlus.Patches.BattleControlTranspilers.AdvanceMainTurnPatches
{ 
    public class PatchEndOfTurnChecks : PatchBaseAdvanceMainTurn
    {
        public PatchEndOfTurnChecks()
        {
            priority = 9943;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchStfld(AccessTools.Field(typeof(BattleControl), "action")));
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "EndOfTurnCheck"));
            Utils.InsertYieldReturn(cursor);
        }
    }
}
