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
    public class PatchExtraEnemiesBug : PatchBaseCheckDead
    {
        public PatchExtraEnemiesBug()
        {
            priority = 156411;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchCall(AccessTools.Method(typeof(BattleControl), "EnemiesAreNotMoving")));
            cursor.GotoNext(MoveType.After,i => i.MatchStfld(AccessTools.Field(typeof(BattleControl), "action")));
            cursor.Emit(OpCodes.Ldloc_1);
            cursor.Emit(OpCodes.Ldc_I4_1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl), "ReorganizeEnemies", new Type[] { typeof(bool) }));         
        }

    }
}
