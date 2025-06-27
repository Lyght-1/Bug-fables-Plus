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
    public class PatchEnemyDropping : PatchBaseCheckDead
    {
        public PatchEnemyDropping()
        {
            priority = 156531;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdloc1(),i => i.MatchCall(AccessTools.Method(typeof(BattleControl), "EnemyDropping")));
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "WaitForEnemyDrop"));
            Utils.InsertYieldReturn(cursor);
            Utils.RemoveUntilInst(cursor, i => i.MatchStfld(out _));
            cursor.Remove();
        }

    }
}
