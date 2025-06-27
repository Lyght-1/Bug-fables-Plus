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

namespace BFPlus.Patches.BattleControlTranspilers.CalculateBaseDamagePatches
{
    public class PatchHardHitsClampDMG : PatchBaseCalculateBaseDamage
    {
        public PatchHardHitsClampDMG()
        {
            priority = 372;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(2), i => i.MatchLdcI4(99));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "GetHardHitsClampDMG"));
            cursor.RemoveRange(3);
        }

    }

    public class PatchHardHitsFloorDMG : PatchBaseCalculateBaseDamage
    {
        public PatchHardHitsFloorDMG()
        {
            priority = 1833;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchConvR4(),i => i.MatchLdcR4(1.25f));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "GetHardHitsDMG"));
            cursor.RemoveRange(7);
        }

    }
}
