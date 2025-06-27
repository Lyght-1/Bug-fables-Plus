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

namespace BFPlus.Patches.EntityControlTranspilers
{
    /// <summary>
    /// change text color for sticky
    /// </summary>
    public class PatchUpdateConditionBubbles : PatchBaseUpdateConditionBubbles
    {
        public PatchUpdateConditionBubbles()
        {
            priority = 528;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdstr("|color,4|"),i=>i.MatchBr(out _), i=>i.MatchLdstr(""), i=>i.MatchStloc(out _));
            var colorTextRef = cursor.Prev.Operand;
            var arrayRef = cursor.Next.Operand;

            cursor.Emit(OpCodes.Ldloc, colorTextRef);
            cursor.Emit(OpCodes.Ldloc, arrayRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchUpdateConditionBubbles), "CheckTextColor"));
            cursor.Emit(OpCodes.Stloc, colorTextRef);
        }

        static string CheckTextColor(string text, int[] condition)
        {
            if (condition[0] == 19)
            {
                text = "|color,4||dropshadow,0.05,-0.05|";
            }

            return text;
        }
    }
}
