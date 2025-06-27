using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using BFPlus.Patches.DoActionPatches.EnemyPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Patches.EventControlTranspilers
{

    /// <summary>
    /// We patch the battlelost field if the termite knight was taken
    /// </summary>
    public class PatchColoseumBattleLoss : PatchBaseEvent163
    {
        public PatchColoseumBattleLoss()
        {
            priority = 191442;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(409));
            cursor.GotoPrev(i => i.MatchLdsfld(out _));
            cursor.RemoveRange(4);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchColoseumBattleLoss), "CheckBattleLossState"));
        }

        static bool CheckBattleLossState()
        {
            return MainManager.instance.flags[409] && (!MainManager.instance.flags[869] || MainManager.instance.flags[871]);
        }
    }


    /// <summary>
    /// Change the dialogue line said if we are in the mite knight quest or after completing it
    /// </summary>
    public class PatchPrimalAnnounceDialogue : PatchBaseEvent163
    {
        public PatchPrimalAnnounceDialogue()
        {
            priority = 193057;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdcI4(58));
            cursor.Prev.OpCode = OpCodes.Nop;
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchPrimalAnnounceDialogue), "CheckAnnounceDialogue"));
        }

        static int CheckAnnounceDialogue()
        {
            if (MainManager.instance.flags[869] || MainManager.instance.flags[871])
                return 59;
            return 58;
        }
    }

    /// <summary>
    /// Patch in our custom mite knight event after the primal weevil fight
    /// </summary>
    public class PatchMiteKnightQuest : PatchBaseEvent163
    {
        public PatchMiteKnightQuest()
        {
            priority = 195386;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdfld(out _),i => i.MatchLdcI4(52));
            cursor.GotoPrev(i => i.MatchLdsfld(out _), i=>i.MatchLdsfld(out _));

            var label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchMiteKnightQuest), "HasTakenQuest"));
            cursor.Emit(OpCodes.Brfalse, label);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(EventControl_Ext), "TermiteKnightEvent"));
            Utils.InsertYieldReturn(cursor);
            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.Ret);

            cursor.GotoNext(i => i.MatchLdsfld(AccessTools.Field(typeof(MainManager), "instance")));
            cursor.MarkLabel(label);
        }

        static bool HasTakenQuest()
        {
            return MainManager.instance.flags[869];
        }
    }
}
