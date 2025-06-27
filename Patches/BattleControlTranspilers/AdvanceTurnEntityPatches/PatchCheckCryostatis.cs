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

namespace BFPlus.Patches.BattleControlTranspilers.AdvanceTurnEntityPatches
{
    public class PatchCheckCryostatis : PatchBaseAdvanceTurnEntity
    {
        public PatchCheckCryostatis()
        {
            priority = 572;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(9));
            cursor.GotoNext(i => i.MatchLdfld(out _));

            int cursorIndex = cursor.Index;

            cursor.GotoNext(i => i.MatchLdloc(out _));
            var indexCondition = cursor.Next.Operand;

            cursor.Goto(cursorIndex);

            var label = cursor.Body.Instructions[cursor.Index - 2].Operand;
            cursor.Prev.OpCode = OpCodes.Nop;

            cursor.Emit(OpCodes.Ldarg_1);
            cursor.Emit(OpCodes.Ldobj, typeof(MainManager.BattleData));
            cursor.Emit(OpCodes.Ldloc, indexCondition);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckCryostatis"));
            cursor.Emit(OpCodes.Brtrue, label);
            cursor.Emit(OpCodes.Ldarg_1);
        }

    }
}
