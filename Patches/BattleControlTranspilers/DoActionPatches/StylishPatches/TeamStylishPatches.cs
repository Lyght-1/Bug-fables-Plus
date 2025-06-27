using BFPlus.Extensions;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Patches.DoActionPatches.StylishPatches
{
    public class PatchRoyalDecreeSetStylish : PatchBaseDoAction
    {
        public PatchRoyalDecreeSetStylish()
        {
            priority = 62423;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(197));
            cursor.GotoNext(MoveType.After, i=>i.MatchBlt(out _),i => i.MatchLdarg0());
            cursor.Prev.OpCode = OpCodes.Nop;
            Utils.InsertStartStylishTimer(cursor,3f,15f,commandSuccess:false);
            cursor.Emit(OpCodes.Ldarg_0);
        }
    }
}
