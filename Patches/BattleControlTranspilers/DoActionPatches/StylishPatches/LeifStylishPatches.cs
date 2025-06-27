using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BFPlus.Extensions;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace BFPlus.Patches.DoActionPatches.StylishPatches
{
    public class PatchLeifBasicAttackSetStylish : PatchBaseDoAction
    {
        public PatchLeifBasicAttackSetStylish()
        {
            priority = 45842;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchLdstr("IceMothHit"), i => i.MatchCall(out _), i => i.MatchPop());
            Utils.InsertStartStylishTimer(cursor, 15f, 30f);
        }
    }

    public class PatchLeifBasicAttackWaitStylish : PatchBaseDoAction
    {

        public PatchLeifBasicAttackWaitStylish()
        {
            priority = 46648;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdcR4(0.05f), i => i.MatchStfld(out _), i => i.MatchLdarg0());
            Utils.InsertWaitStylish(cursor);
            cursor.Emit(OpCodes.Ldarg_0);
        }
    }
    
    public class PatchLeifIcefallSetStylish : PatchBaseDoAction
    {

        public PatchLeifIcefallSetStylish() : base()
        {
            priority = 50134;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i=>i.MatchLdcI4(107), i=>i.MatchStfld(out _), i=>i.MatchLdarg0());
            Utils.InsertStartStylishTimer(cursor,20f,30f,commandSuccess:false);
        }
    }
    
    public class PatchLeifIcefallWaitStylish : PatchBaseDoAction
    {
        public PatchLeifIcefallWaitStylish()
        {
            priority = 50363;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchInitobj(out _));
            cursor.Emit(OpCodes.Ldarg_0);
            Utils.InsertWaitStylish(cursor);
        }
    }
    
    public class PatchLeifFrigidEndSetStylish : PatchBaseDoAction
    {

        public PatchLeifFrigidEndSetStylish()
        {
            priority = 46441;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdcI4(118), i => i.MatchStfld(out _));
            Utils.InsertStartStylishTimer(cursor,4f,20f,stylishID:1);
        }
    }
    
    public class PatchLeifBubbleSetStylish : PatchBaseDoAction
    {
        public PatchLeifBubbleSetStylish()
        {
            priority = 61100;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(178));
            cursor.GotoNext(MoveType.After,i => i.MatchLdcI4(102), i=>i.MatchStfld(out _));
            Utils.InsertStartStylishTimer(cursor,20f,30f,commandSuccess:false);
        }
    }
    
    public class PatchLeifBuffsSetStylish : PatchBaseDoAction
    {
        public PatchLeifBuffsSetStylish()
        {
            priority = 51312;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchLdstr("Magic"), i => i.MatchCall(out _), i => i.MatchPop());
            Utils.InsertStartStylishTimer(cursor,20f,30f,commandSuccess:false);
        }
    }
    
    public class PatchLeifDebuffsSetStylish : PatchBaseDoAction
    {
        public PatchLeifDebuffsSetStylish()
        {
            priority = 51655;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchLdcI4(111), i => i.MatchStfld(out _), i=>i.MatchLdstr("FastWoosh"), i=>i.MatchCall(out _), i=>i.MatchPop());
            Utils.InsertStartStylishTimer(cursor, 15f, 25f, commandSuccess: false);
        }
    }
    
    public class PatchLeifIceRainSetStylish : PatchBaseDoAction
    {
        public PatchLeifIceRainSetStylish()
        {
            priority = 49685;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(62));
            cursor.GotoNext(MoveType.After, i => i.MatchLdcI4(100), i => i.MatchStfld(out _));
            Utils.InsertStartStylishTimer(cursor, 4f, 12f, commandSuccess: false);
        }
    }
    
    public class PatchFrostBowlingSetStylish : PatchBaseDoAction
    {
        public PatchFrostBowlingSetStylish()
        {
            priority = 48218;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(42));
            cursor.GotoNext(MoveType.After,i => i.MatchLdnull(), i=>i.MatchStfld(out _));
            Utils.InsertStartStylishTimer(cursor, 3f, 15f);
            cursor.Emit(OpCodes.Ldarg_0);
            Utils.InsertWaitStylish(cursor, 0.2f);
        }
    }
}
