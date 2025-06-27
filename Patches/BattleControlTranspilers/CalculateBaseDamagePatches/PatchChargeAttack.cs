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
    public class PatchChargeAttack : PatchBaseCalculateBaseDamage
    {
        public PatchChargeAttack()
        {
            priority = 380;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(
                i => i.MatchLdarga(out _),
                i => i.MatchCall(out _),
                i => i.MatchLdfld(AccessTools.Field(typeof(MainManager.BattleData), "charge")));
            cursor.Emit(OpCodes.Ldarg_1);
            cursor.Emit(OpCodes.Ldarg_2);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "GetChargeAttack"));
            cursor.RemoveRange(3);
        }

    }
}
