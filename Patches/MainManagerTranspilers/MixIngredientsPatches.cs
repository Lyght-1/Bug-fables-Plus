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

namespace BFPlus.Patches.MainManagerTranspilers
{
    /// <summary>
    /// Completely removes the mystery berry cooking stuff, which was overriding our new recipes with mystery berry
    /// </summary>
    public class MixIngredientsPatches : PatchBaseMainManagerMixIngredients
    {
        public MixIngredientsPatches()
        {
            priority = 0;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            ILLabel label = null;
            cursor.GotoNext(i => i.MatchLdcI4(156), i=>i.MatchBeq(out _), i=>i.MatchLdarg1(), i=>i.MatchLdcI4(156), i=>i.MatchBneUn(out label));
            cursor.Goto(0);
            cursor.GotoNext(i => i.MatchBle(out _));
            cursor.Next.Operand = label;

            cursor.GotoNext(MoveType.After,i => i.MatchBgt(out _));
            cursor.Prev.OpCode = OpCodes.Ble;
            cursor.Prev.Operand = label;

            cursor.RemoveRange(3);
            cursor.GotoNext(i => i.MatchLdarg0(), i => i.MatchLdcI4(156));
            Utils.RemoveUntilInst(cursor, i => i.MatchLdcI4(0));
        }
    }
}
