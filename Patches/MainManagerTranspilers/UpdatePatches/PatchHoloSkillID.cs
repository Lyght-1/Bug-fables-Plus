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
    public class PatchHoloSkillID : PatchBaseMainManagerUpdate
    {
        public PatchHoloSkillID()
        {
            priority = 2047;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            var fixHoloSkillIDRef = AccessTools.Method(typeof(MainManager_Ext), "FixHoloSkillID");
            cursor.GotoNext(i => i.MatchCall(AccessTools.Method(typeof(MainManager), "LoadLangSpecific")));
            cursor.GotoNext(i => i.MatchCall(out _));

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "FixHoloSkillID"));
            cursor.RemoveRange(3);
        }
    }
}
