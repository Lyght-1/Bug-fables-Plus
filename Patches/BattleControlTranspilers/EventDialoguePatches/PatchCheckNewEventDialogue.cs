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

namespace BFPlus.Patches.BattleControlTranspilers.EventDialoguePatches
{
    public class PatchCheckNewEventDialogue : PatchBaseEventDialogue
    {
        public PatchCheckNewEventDialogue()
        {
            priority = 9036;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchCall(AccessTools.Method(typeof(BattleControl), "ReorganizeEnemies", new Type[] {typeof(bool)})));
            cursor.GotoPrev(MoveType.After,i => i.MatchLdloc1());
            cursor.Prev.OpCode = OpCodes.Nop;
            int cursorIndex = cursor.Index;

            cursor.Goto(0);
            cursor.GotoNext(MoveType.After, i => i.MatchCall(AccessTools.Method(typeof(BattleControl), "RefreshEnemyHP")));
            cursor.GotoNext(MoveType.After, i => i.MatchLdarg0(), i => i.MatchLdfld(out _));
            var idRef = cursor.Prev.Operand;

            cursor.Goto(cursorIndex);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, idRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckNewEventDialogue"));
            Utils.InsertYieldReturn(cursor);
            cursor.Emit(OpCodes.Ldloc_1);
        }
    }
}
