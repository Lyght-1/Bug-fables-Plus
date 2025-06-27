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
    public class PatchTrustFallCheck : PatchBaseSetItem
    {
        public PatchTrustFallCheck()
        {
            priority = 191414;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchSwitch(out _));
            var label = cursor.DefineLabel();
            var jumpLabel = cursor.Body.Instructions[cursor.Index + 2].Operand;

            cursor.Emit(OpCodes.Ldarg_1);
            cursor.Emit(OpCodes.Ldc_I4, (int)NewMenuText.TrustFall);
            cursor.Emit(OpCodes.Bne_Un, label);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "DoTrustFall"));
            cursor.Emit(OpCodes.Br, jumpLabel);
            cursor.MarkLabel(label);
        }

    }
}
