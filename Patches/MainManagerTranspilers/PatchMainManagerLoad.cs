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
    public class PatchFlagVar : PatchBaseMainManagerLoad
    {
        public PatchFlagVar()
        {
            priority = 1349;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(70));
            cursor.Next.OpCode = OpCodes.Ldc_I4;
            cursor.Next.Operand = MainManager_Ext.FlagVarNumber;
        }
    }
}
