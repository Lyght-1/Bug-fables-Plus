using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BFPlus.Extensions;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Text.RegularExpressions;
using System.Reflection;

namespace BFPlus.Patches.DoActionPatches
{
    public class PatchCheckNewEnemyVariant : PatchBaseDoAction
    {

        public PatchCheckNewEnemyVariant()
        {
            priority = 62966;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(
                MoveType.Before,
                i => i.MatchSwitch(out _),
                i => i.MatchBr(out _),
                i => i.MatchLdloc1(),
                i =>i.MatchLdcI4(1),
                i =>i.MatchStfld(AccessTools.Field(typeof(BattleControl),"nonphyscal"))
            );

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "GetNewEnemySwap"));
        }
    }

    public class PatchCheckNewEnemy : PatchBaseDoAction
    {    
        public PatchCheckNewEnemy()
        {
            priority = 62947;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(
                MoveType.After,
                i => i.MatchLdarg0(),
                i => i.MatchLdfld(out _),
                i => i.MatchLdfld(AccessTools.Field(typeof(EntityControl),"onground")),
                i=>  i.MatchBrfalse(out _)
            );

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckUseItem"));
            Utils.InsertYieldReturn(cursor);
            var label = cursor.DefineLabel();

            cursor.Emit(OpCodes.Ldsfld, AccessTools.Field(typeof(BattleControl_Ext), "enemyUsedItem"));
            cursor.Emit(OpCodes.Brfalse, label);

            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.Stsfld, AccessTools.Field(typeof(BattleControl_Ext), "enemyUsedItem"));

            int originalIndex = cursor.Index;
            ILLabel getOutLabel = null;

            cursor.GotoNext(i => i.MatchBr(out getOutLabel));
            cursor.Goto(originalIndex);
            cursor.Emit(OpCodes.Br, getOutLabel).MarkLabel(label);

            //Custom enemy check
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckCustomEnemyAI"));
            Utils.InsertYieldReturn(cursor);
        }
    }
}
