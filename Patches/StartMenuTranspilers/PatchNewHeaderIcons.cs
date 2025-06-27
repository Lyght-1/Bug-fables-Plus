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
using UnityEngine;

namespace BFPlus.Patches.StartMenuTranspilers
{
    public class PatchNewHeaderIcons : PatchBaseStartMenuShowSaves
    {
        public PatchNewHeaderIcons()
        {
            priority = 480;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdstr("mode"));

            var spriteRef = cursor.Instrs[cursor.Index - 2].Operand;
            var scaleRef = cursor.Instrs[cursor.Index - 4].Operand;

            cursor.Prev.OpCode = OpCodes.Nop;
            cursor.GotoNext(i=>i.MatchBox(out _));

            int cursorIndex = cursor.Index;
            cursor.GotoNext(i => i.MatchLdcR4(1f));
            cursor.GotoNext(i => i.MatchLdloc(out _));
            var indexRef = cursor.Next.Operand;

            cursor.GotoNext(i => i.MatchLdloc(out _));
            var index2Ref = cursor.Next.Operand;

            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, AccessTools.Field(typeof(StartMenu), "saves"));
            cursor.Emit(OpCodes.Ldloc_2);
            cursor.Emit(OpCodes.Ldloc_S, scaleRef);
            cursor.Emit(OpCodes.Ldloc_S, spriteRef);
            cursor.Emit(OpCodes.Ldloc_S, indexRef);
            cursor.Emit(OpCodes.Ldloc_S, index2Ref);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "CreateHeaderIcons"));
            Utils.RemoveUntilInst(cursor, i => i.MatchPop());
            cursor.Remove();
        }
    }
}
