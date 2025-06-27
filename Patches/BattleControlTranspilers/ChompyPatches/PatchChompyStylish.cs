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

namespace BFPlus.Patches.ChompyPatches
{
    public class PatchChompyStylish : PatchBaseChompy
    {
        public PatchChompyStylish()
        {
            priority = 41401;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcR4(0.4f));
            cursor.Prev.OpCode = OpCodes.Nop;
            Utils.InsertStartStylishTimer(cursor, 10f, 20f);
            cursor.Emit(OpCodes.Ldarg_0);

            cursor.GotoNext(i => i.MatchLdloc1());
            cursor.Emit(OpCodes.Ldarg_0);
            Utils.InsertWaitStylish(cursor);
        }
    }
}
