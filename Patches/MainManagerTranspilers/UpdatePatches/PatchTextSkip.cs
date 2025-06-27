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

namespace BFPlus.Patches.MainManagerTranspilers.UpdatePatches
{

    public class PatchTextSkip : PatchBaseMainManagerUpdate
    {
        public PatchTextSkip()
        {
            priority = 100;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdcR4(16f), i => i.MatchStfld(out _));
            var label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Ldsfld, typeof(MainManager_Ext).GetField("fastText"));
            cursor.Emit(OpCodes.Brfalse, label);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "ResetInputCooldown"));
            cursor.MarkLabel(label);
        }
    }
}
