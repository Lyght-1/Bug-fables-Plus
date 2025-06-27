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

namespace BFPlus.Patches.BattleControlTranspilers.CheckDeadPatches
{
    public class PatchPhoenixCheck : PatchBaseCheckDead
    {
        public PatchPhoenixCheck()
        {
            priority = 155710;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdfld(AccessTools.Field(typeof(EntityControl), "dead")), i=>i.MatchBrtrue(out _));

            var jumpLabel = cursor.Prev.Operand;
            var label = cursor.DefineLabel();
            var idRef = cursor.Body.Instructions[cursor.Index - 5];

            cursor.Emit(idRef.OpCode, idRef.Operand);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "DoInkBlotPlayer"));

            cursor.Emit(idRef.OpCode, idRef.Operand);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckPhoenix"));
            cursor.Emit(OpCodes.Brfalse, label);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(idRef.OpCode, idRef.Operand);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "DoPhoenix"));
            Utils.InsertYieldReturn(cursor);
            cursor.Emit(OpCodes.Br, jumpLabel);
            cursor.MarkLabel(label);
        }

    }
}
