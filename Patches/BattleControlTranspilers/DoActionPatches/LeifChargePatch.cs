using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BFPlus.Extensions;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace BFPlus.Patches.DoActionPatches
{
    public class PatchLeifCharge : PatchBaseDoAction
    {
        public PatchLeifCharge()
        {
            priority = 51506;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdfld(typeof(MainManager.BattleData).GetField("charge")), i => i.MatchLdcI4(3));
            cursor.GotoNext();
            cursor.Emit(OpCodes.Ldarg_0);
            int cursorIndex = cursor.Index;
            cursor.Emit(cursor.Body.Instructions[cursorIndex - 6].OpCode, cursor.Body.Instructions[cursorIndex - 6].Operand);
            cursor.Emit(cursor.Body.Instructions[cursorIndex - 5].OpCode, cursor.Body.Instructions[cursorIndex - 5].Operand);
            cursor.Emit(cursor.Body.Instructions[cursorIndex - 4].OpCode, cursor.Body.Instructions[cursorIndex - 4].Operand);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "CheckMaxCharge"));
            cursor.Remove();
        }
    }
}
