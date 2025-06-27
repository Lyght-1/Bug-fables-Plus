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

namespace BFPlus.Patches.BattleControlTranspilers.CheckDeadPatches
{
    public class PatchInkBlotCheck : PatchBaseCheckDead
    {
        public PatchInkBlotCheck()
        {
            priority = 156182;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchCall(AccessTools.Method(typeof(BattleControl), "ReorganizeEnemies", new Type[] { })));
            cursor.Prev.OpCode = OpCodes.Nop;
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "DoInkBlotEnemy"));
            cursor.Emit(OpCodes.Ldloc_1);
        }
    }
}
