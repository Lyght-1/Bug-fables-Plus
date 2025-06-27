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
    /// replace max level check by our nex max level
    /// </summary>
    public class PatchBumpMaxLevel : PatchBaseNPCControlCheckBump
    {
        public PatchBumpMaxLevel()
        {
            priority = 0;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            for(int j = 0; j < 2; j++)
            {
                cursor.GotoNext(i => i.MatchLdcI4(27));
                cursor.Emit(OpCodes.Ldc_I4, MainManager_Ext.newMaxLevel);
                cursor.Remove();
            }
        }
    }
}
