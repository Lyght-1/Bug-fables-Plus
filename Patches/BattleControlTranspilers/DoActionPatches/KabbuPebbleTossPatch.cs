using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BFPlus.Extensions;
using MonoMod.Cil;
using System.Reflection;
using Mono.Cecil.Cil;
namespace BFPlus.Patches.DoActionPatches
{
    public class PatchKabbuPebbleToss : PatchBaseDoAction
    {
        public PatchKabbuPebbleToss()
        {
            priority = 52083;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdcI4(88));
            cursor.GotoNext(i => i.MatchBr(out _));

            var breakLabel = cursor.Next.Operand;
            cursor.GotoNext(MoveType.After,i => i.MatchLdcI4(28),i=>i.MatchStfld(out _));
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "DoPebbleToss"));
            Utils.InsertYieldReturn(cursor);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "IsUsingItem"));
            cursor.Emit(OpCodes.Brfalse, breakLabel);
        }
    }
}
