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

namespace BFPlus.Patches.NPCControlTranspilers
{
    /// <summary>
    /// Removes the check for setting the crystal berry price at shades. just set it to 3 per medal in mystery?
    /// </summary>
    public class PatchCreateDescWindow : PatchBaseNPCControlCreateDescWindow
    {
        public PatchCreateDescWindow()
        {
            priority = 178;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(681));
            cursor.GotoNext(i => i.MatchLdcI4(681));

            cursor.GotoNext(i => i.MatchLdsfld(out _), i=>i.MatchLdfld(out _));
            cursor.Next.OpCode = OpCodes.Nop;
            cursor.GotoNext(i => i.MatchLdfld(out _));
            Utils.RemoveUntilInst(cursor, i => i.MatchStelemI4());
            cursor.Emit(OpCodes.Ldc_I4, MainManager_Ext.MYSTERY_SHADE_PRICE);
        }
    }
}
