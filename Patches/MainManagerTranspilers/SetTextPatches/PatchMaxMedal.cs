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

namespace BFPlus.Patches.SetTextPatches
{
    public class PatchMaxMedal : PatchBaseSetText
    {
        public PatchMaxMedal()
        {
            priority = 25027;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.Before, i => i.MatchLdcI4(120), i=> i.MatchBox(out _));
            cursor.Remove();
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "GetMaxMedals"));
        }
    }
}
