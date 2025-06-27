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

namespace BFPlus.Patches.BattleControlTranspilers.DoActionPatches
{
    public class CheckDelayedConditionPlayer : PatchBaseDoAction
    {
        public CheckDelayedConditionPlayer()
        {
            priority = 150469;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdloc1(), i => i.MatchCall(AccessTools.Method(typeof(BattleControl), "EndPlayerTurn")));
            cursor.Next.OpCode = OpCodes.Nop;
            cursor.GotoNext(i => i.MatchCall(out _));
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckDelayedConditionsPlayer"));
            Utils.InsertYieldReturn(cursor);
            cursor.Emit(OpCodes.Ldloc_1);
        }

    }
}
