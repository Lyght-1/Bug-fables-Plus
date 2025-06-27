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
    public class PatchMaxCharge : PatchBaseAdvanceMainTurn
    {
        public PatchMaxCharge()
        {
            priority = 11037;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdfld(AccessTools.Field(typeof(MainManager.BattleData), "charge")));
            cursor.Emit(OpCodes.Ldloc, cursor.Body.Instructions[cursor.Index-3].Operand);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "CheckMaxCharge"));
            cursor.Remove();
        }

    }
}
