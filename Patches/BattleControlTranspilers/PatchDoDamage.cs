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
    public class PatchCheckSpiderBait : PatchBaseBattleControlDoDamage
    {
        public PatchCheckSpiderBait()
        {
            priority = 115;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            int blockRef = -1;

            ILLabel label = cursor.DefineLabel();
            cursor.GotoNext(MoveType.After,i => i.MatchStarg(out blockRef), i=>i.MatchLdcI4(0));
            cursor.Prev.OpCode = OpCodes.Nop;

            cursor.Emit(OpCodes.Ldarg_2);
            cursor.Emit(OpCodes.Ldarg, blockRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchCheckSpiderBait), "CheckSpiderBait"));
            cursor.Emit(OpCodes.Brfalse, label);
            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.Starg, blockRef).MarkLabel(label);
            cursor.Emit(OpCodes.Ldc_I4_0);
        }

        static bool CheckSpiderBait(ref MainManager.BattleData target, bool block)
        {
            var getSuperBlockRef = AccessTools.Method(typeof(BattleControl), "GetSuperBlock");
            bool superblocked = (bool)getSuperBlockRef.Invoke(MainManager.battle, new object[] { target.battleentity.animid});
            return block && MainManager.HasCondition(MainManager.BattleCondition.Sticky, target) > -1 && MainManager.BadgeIsEquipped((int)Medal.SpiderBait, target.trueid) && !superblocked;
        }
    }

    public class PatchCheckSlugskinSpikyBod : PatchBaseBattleControlDoDamage
    {
        public PatchCheckSlugskinSpikyBod()
        {
            priority = 428;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdstr("Damage0"));
            cursor.GotoNext(i => i.MatchLdcI4(1));
            cursor.Emit(OpCodes.Ldarg_2);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchCheckSlugskinSpikyBod), "CheckSlugskin"));
            cursor.Remove();
        }

        static int CheckSlugskin(ref MainManager.BattleData target)
        {
            if (Entity_Ext.GetEntity_Ext(target.battleentity).slugskinActive)
                return 2;
            return 1;        
        }
    }

    public class PatchFixSleepCantMove : PatchBaseBattleControlDoDamage
    {
        public PatchFixSleepCantMove()
        {
            priority = 1176;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdcI4(1),i => i.MatchStfld(AccessTools.Field(typeof(MainManager.BattleData),"cantmove")));

            int cursorIndex = cursor.Index;
            cursor.GotoPrev(i=>i.MatchBrfalse(out _));
            var playerFlag = cursor.Prev.Operand;
            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Ldloc, playerFlag);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchFixSleepCantMove), "CheckSleepCantMove"));
            cursor.Remove();
        }

        static int CheckSleepCantMove(bool isPlayer)
        {
            if (isPlayer)
                return 1;
            return 0;
        }
    }
}
