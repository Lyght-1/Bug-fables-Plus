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

namespace BFPlus.Patches.BattleControlTranspilers.AddExperiencePatches
{
    public class PatchLevelCap : PatchBaseAddExperience
    {
        public PatchLevelCap()
        {
            priority = 12650;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            int cursorIndex = cursor.Index;
            for (int i = 0; i < 3; i++)
            {
                cursor.GotoNext(j => j.MatchLdfld(out _), j => j.MatchLdcI4(27));
                cursor.GotoNext();
                cursor.Emit(OpCodes.Ldc_I4, MainManager_Ext.newMaxLevel);
                cursor.Remove();
            }
            cursor.Goto(cursorIndex);
        }

    }
}
