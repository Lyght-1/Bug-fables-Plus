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
using UnityEngine;
using BFPlus.Extensions.Stylish;
using System.Collections;

namespace BFPlus.Patches.DoActionPatches.StylishPatches
{
    public class PatchKabbuBasicAttackSetStylish : PatchBaseDoAction
    {
        public PatchKabbuBasicAttackSetStylish()
        {
            priority = 45123;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdcR4(-7f), i=>i.MatchNewobj(out _), i=>i.MatchStfld(AccessTools.Field(typeof(MainManager),"camoffset")));

            ILLabel label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckTutorialStylish"));
            cursor.Emit(OpCodes.Brfalse, label);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchKabbuBasicAttackSetStylish), "DoKabbuStylishTutorial"));
            Utils.InsertYieldReturn(cursor);
            cursor.MarkLabel(label);

            Utils.InsertStartStylishTimer(cursor, 4f, 12f);
        }

        static IEnumerator DoKabbuStylishTutorial()
        {
            yield return BattleControl_Ext.Instance.DoStylishTutorial(new KabbuStylish());
        }
    }
    
    public class PatchKabbuBasicAttackWaitStylish : PatchBaseDoAction
    {
        public PatchKabbuBasicAttackWaitStylish()
        {
            priority = 45384;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchStfld(AccessTools.Field(typeof(EntityControl), "overrridejump")));
            cursor.GotoNext(MoveType.After, i => i.MatchStfld(AccessTools.Field(typeof(EntityControl),"overrridejump")));
            cursor.Emit(OpCodes.Ldarg_0);
            Utils.InsertWaitStylish(cursor);
        }
    }

    
    public class PatchKabbuUnderStrikeSetStylish : PatchBaseDoAction
    {
        public PatchKabbuUnderStrikeSetStylish()
        {
            priority = 62092;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(193));
            cursor.GotoNext(MoveType.After,i => i.MatchStfld(AccessTools.Field(typeof(EntityControl), "overrideheight")));
            Utils.InsertStartStylishTimer(cursor, 4f, 10f);
            cursor.Emit(OpCodes.Ldarg_0);
            Utils.InsertWaitStylish(cursor, 0.2f);
        }
    }
    
    public class PatchKabbuDashThroughSetStylish : PatchBaseDoAction
    {
        public PatchKabbuDashThroughSetStylish()
        {
            priority = 60543;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdarg0(),i => i.MatchLdcI4(172));

            cursor.GotoNext(MoveType.After,i => i.MatchLdnull(), i=>i.MatchStfld(out _));
            Utils.InsertStartStylishTimer(cursor, 4f, 10f);
            cursor.Emit(OpCodes.Ldarg_0);
            Utils.InsertWaitStylish(cursor, 0.2f);
        }
    }
    
    public class PatchKabbuBoulderTossSetStylish : PatchBaseDoAction
    {
        public PatchKabbuBoulderTossSetStylish()
        {
            priority = 50710;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(75));
            cursor.GotoNext(MoveType.After,i => i.MatchLdcI4(104), i=>i.MatchStfld(out _));
            Utils.InsertStartStylishTimer(cursor, 4f, 10f);
        }
    }
    
    public class PatchKabbuBoulderTossWaitStylish : PatchBaseDoAction
    {
        public PatchKabbuBoulderTossWaitStylish()
        {
            priority = 51101;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(78));
            cursor.GotoNext(i => i.MatchLdnull());
            Utils.InsertWaitStylish(cursor);
            cursor.Emit(OpCodes.Ldarg_0);
        }
    }
    
    public class PatchKabbuFrozenDrillSetStylish : PatchBaseDoAction
    {
        public PatchKabbuFrozenDrillSetStylish()
        {
            priority = 55017;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdarg0(),i => i.MatchLdcI4(109));
            cursor.GotoNext(MoveType.After,i => i.MatchCall(AccessTools.Method(typeof(BattleControl), "MultiSkillMove")));

            Utils.InsertStartStylishTimer(cursor, 4f, 20f);
            cursor.Emit(OpCodes.Ldarg_0);
            Utils.InsertWaitStylish(cursor, 0.4f);
        }
    }
    
    public class PatchKabbuPepTalkSetStylish : PatchBaseDoAction
    {
        public PatchKabbuPepTalkSetStylish()
        {
            priority = 62727;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(205));
            cursor.GotoNext(MoveType.After,i => i.MatchCallvirt(out _));
            Utils.InsertStartStylishTimer(cursor,4f,20f,commandSuccess: false);
        }
    }
}
