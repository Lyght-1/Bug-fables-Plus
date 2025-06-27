using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BFPlus.Extensions;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace BFPlus.Patches.DoActionPatches.EnemyPatches
{
    public class PatchFlyingCavelingDamage : PatchBaseDoAction
    {
        public PatchFlyingCavelingDamage()
        {
            priority = 92256;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(705));
            cursor.GotoNext(MoveType.After, i => i.MatchLdfld(AccessTools.Field(typeof(BattleControl), "playertargetID")));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckSeedlingDamage"));
            cursor.RemoveRange(6);
        }
    }

    public class PatchWeevilCheckEnemy : PatchBaseDoAction
    {
        public PatchWeevilCheckEnemy()
        {
            priority = 86427;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(612));
            cursor.GotoNext(MoveType.After,
                i => i.MatchCall(AccessTools.Method(typeof(BattleControl), "EnemyInField", new Type[] { typeof(int), typeof(int[]) })),
                i => i.MatchStfld(out _)
            );
            Instruction stfldInst = cursor.Prev;

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckEnemyWeevilRef"));
            cursor.Emit(stfldInst.OpCode, stfldInst.Operand);
        }
    }
    
    public class PatchWeevilBuffCheck : PatchBaseDoAction
    {
        public PatchWeevilBuffCheck()
        {
            priority = 86824;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdcI4(617));

            //change weevil bite property
            cursor.GotoNext(MoveType.After, i => i.MatchLdfld(AccessTools.Field(typeof(BattleControl), "playertargetID")), i => i.MatchLdcI4(3));
            cursor.RemoveRange(3);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "GetStickyProperty"));

            cursor.GotoNext(MoveType.After, 
                i => i.MatchCall(AccessTools.Method(typeof(BattleControl), "Heal", new Type[] { typeof(MainManager.BattleData).MakeByRefType(), typeof(int?) })));
            Instruction ldfldRef = cursor.Body.Instructions[cursor.Index+3];       
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(ldfldRef.OpCode, ldfldRef.Operand);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckWeevilEatBuff"));
        }
    }
}
