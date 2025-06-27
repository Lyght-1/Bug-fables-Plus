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

namespace BFPlus.Patches.BattleControlTranspilers
{
    public class PatchCanUseChargeTeamMove : PatchBaseGetMultiDamage
    {
        public PatchCanUseChargeTeamMove()
        {
            priority = 35;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdfld(AccessTools.Field(typeof(MainManager.BattleData), "charge")));
            cursor.GotoPrev(i => i.MatchLdloc0());

            var label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Ldloc_1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CanUseCharge", new Type[] { typeof(int) }));
            cursor.Emit(OpCodes.Brfalse, label);
            cursor.GotoNext(i => i.MatchLdcI4(4)).MarkLabel(label);
        }

    }
}
