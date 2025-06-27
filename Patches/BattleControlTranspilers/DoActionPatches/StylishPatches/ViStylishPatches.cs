using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BFPlus.Extensions;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Collections;
using BFPlus.Extensions.Stylish;

namespace BFPlus.Patches.DoActionPatches.StylishPatches
{
    public class PatchViBasicAttackSetStylish : PatchBaseDoAction
    {
        public PatchViBasicAttackSetStylish()
        {
            priority = 44847;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchLdcR4(0.1f), i => i.MatchCall(out _));

            ILLabel label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckTutorialStylish"));
            cursor.Emit(OpCodes.Brfalse, label);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchViBasicAttackSetStylish), "DoViStylishTutorial"));
            Utils.InsertYieldReturn(cursor);
            cursor.MarkLabel(label);
            Utils.InsertStartStylishTimer(cursor, 3f, 12f);
        }

        static IEnumerator DoViStylishTutorial()
        {
            yield return BattleControl_Ext.Instance.DoStylishTutorial(new ViStylish());
        }
    }
    
    public class PatchViBasicAttackWaitStylish : PatchBaseDoAction
    {
        public PatchViBasicAttackWaitStylish()
        {
            priority = 44877;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdfld(out _),i => i.MatchLdcI4(13));
            Utils.InsertWaitStylish(cursor);
            cursor.Emit(OpCodes.Ldarg_0);
        }
    }
    
    public class PatchViTornadoTossHitsSetStylish : PatchBaseDoAction
    {
        public PatchViTornadoTossHitsSetStylish()
        {
            priority = 59737;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdcI4(163));
            cursor.GotoNext(MoveType.After,i => i.MatchBlt(out _), i=>i.MatchLdarg0());
            cursor.Prev.OpCode = OpCodes.Nop;
            Utils.InsertStartStylishTimer(cursor, 3f, 12f, stylishID:1);
            cursor.Emit(OpCodes.Ldarg_0);
        }
    }
    
    public class PatchViTornadoTossEndSetStylish : PatchBaseDoAction
    {
        public PatchViTornadoTossEndSetStylish()
        {
            priority = 60004;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(165));
            cursor.GotoNext(MoveType.After, i => i.MatchLdcI4(8), i => i.MatchCall(out _));
            Utils.InsertStartStylishTimer(cursor, 4f, 12f);
        }
    }
    
    public class PatchViTornadoTossEndWaitStylish : PatchBaseDoAction
    {
        public PatchViTornadoTossEndWaitStylish()
        {
            priority = 60051;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(166));
            cursor.GotoNext(i => i.MatchLdnull(), i => i.MatchStfld(out _));
            Utils.InsertWaitStylish(cursor);
            cursor.Emit(OpCodes.Ldarg_0);
        }
    }

    
    public class PatchViFlyJumpSetStylish : PatchBaseDoAction
    {
        public PatchViFlyJumpSetStylish()
        {
            priority = 56089;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(117));
            cursor.GotoNext(MoveType.After, i => i.MatchCall(AccessTools.Method(typeof(BattleControl), "MultiSkillMove")));

            Utils.InsertStartStylishTimer(cursor, 4f, 12f);
            cursor.Emit(OpCodes.Ldarg_0);
            Utils.InsertWaitStylish(cursor, 0.2f);
        }
    }
    
    public class PatchViStashSetStylish : PatchBaseDoAction
    {
        public PatchViStashSetStylish()
        {
            priority = 60912;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(176));
            cursor.GotoNext(MoveType.After,i => i.MatchConvI4(), i=>i.MatchBlt(out _), i=>i.MatchLdarg0());
            cursor.Prev.OpCode = OpCodes.Nop;
            Utils.InsertStartStylishTimer(cursor,4f,14f,commandSuccess:false);
            cursor.Emit(OpCodes.Ldarg_0);
        }
    }
    
    public class PatchViNeedleTossSetStylish : PatchBaseDoAction
    {
        public PatchViNeedleTossSetStylish()
        {
            priority = 57025;
        }


        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(123));
            cursor.GotoNext(MoveType.After,i => i.MatchLdcI4(119), i=>i.MatchStfld(out _));
            Utils.InsertStartStylishTimer(cursor,10f,20f,commandSuccess: false);
        }
    }
    
    public class PatchViHurricaneTossSetStylish : PatchBaseDoAction
    {
        public PatchViHurricaneTossSetStylish()
        {
            priority = 59337;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdcI4(156));
            cursor.GotoNext(MoveType.After,i => i.MatchLdcI4(8), i=>i.MatchCall(out _));
            Utils.InsertStartStylishTimer(cursor,4f,12f,commandSuccess: false);
        }
    }
    
    public class PatchViHurricaneTossWaitStylish : PatchBaseDoAction
    {
        public PatchViHurricaneTossWaitStylish()
        {
            priority = 59386;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(158));
            cursor.GotoNext(i => i.MatchLdnull());

            Utils.InsertWaitStylish(cursor);
            cursor.Emit(OpCodes.Ldarg_0);
        }
    }
    
    public class PatchViNeedlePincerWaitStylish : PatchBaseDoAction
    {
        public PatchViNeedlePincerWaitStylish() : base()
        {
            priority = 58096;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(140));
            cursor.GotoPrev(i => i.MatchLdarg0(), i => i.MatchLdsfld(out _));

            cursor.Next.OpCode = OpCodes.Nop;
            cursor.GotoNext(i => i.MatchLdsfld(out _));
            Utils.InsertStartStylishTimer(cursor, 4f, 15f, commandSuccess: true);
            cursor.Emit(OpCodes.Ldarg_0);

            cursor.GotoNext(MoveType.After,i => i.MatchBr(out _));
            cursor.Prev.OpCode = OpCodes.Nop;

            var label = cursor.Prev.Operand;
            cursor.Emit(OpCodes.Ldarg_0);
            Utils.InsertWaitStylish(cursor);
            cursor.Emit(OpCodes.Br, label);
        }
    }
    
    public class PatchViFrostRelaySetStylish : PatchBaseDoAction
    {
        public PatchViFrostRelaySetStylish()
        {
            priority = 49266;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(57));
            cursor.GotoNext(MoveType.After, i => i.MatchCall(AccessTools.Method(typeof(UnityEngine.Object), "Destroy", new Type[] { typeof(GameObject) })));
            Utils.InsertStartStylishTimer(cursor, 3f, 15f);
        }
    }
    
    public class PatchViFrostRelayWaitStylish : PatchBaseDoAction
    {
        public PatchViFrostRelayWaitStylish()
        {
            priority = 49325;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(58));
            cursor.GotoNext(MoveType.After, i => i.MatchCall(out _));
            cursor.Emit(OpCodes.Ldarg_0);
            Utils.InsertWaitStylish(cursor);
        }
    }
    
    public class PatchViHeavyThrowSetStylish : PatchBaseDoAction
    {
        public PatchViHeavyThrowSetStylish()
        {
            priority = 58550;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdstr("DLGammaStep"));
            cursor.GotoNext(MoveType.After,i => i.MatchLdcI4(100), i => i.MatchStfld(out _));
            Utils.InsertStartStylishTimer(cursor, 3f, 15f);
        }
    }
}

