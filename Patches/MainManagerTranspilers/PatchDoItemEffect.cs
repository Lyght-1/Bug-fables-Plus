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

namespace BFPlus.Patches.MainManagerTranspilers
{
    public class PatchCheckMaxCharge : PatchBaseMainManagerDoItemEffect
    {
        public PatchCheckMaxCharge()
        {
            priority = 447;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdfld(AccessTools.Field(typeof(MainManager.BattleData), "charge")));
            cursor.Emit(OpCodes.Ldarg_1);
            cursor.Remove();
            cursor.GotoPrev(i => i.MatchLdarga(out _));

            var characterIdRef = cursor.Next.Operand;
            var callRef = cursor.Instrs[cursor.Index + 1].Operand;

            cursor.GotoNext(i => i.MatchLdcI4(3));
            cursor.Emit(OpCodes.Ldarga, characterIdRef);
            cursor.Emit(OpCodes.Call, callRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "CheckMaxCharge"));
            cursor.Remove();
        }
    }
}
