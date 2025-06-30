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

namespace BFPlus.Patches.BattleControlTranspilers.StartBattlePatches
{

    public class PatchRemoveLastWindStartBattle : PatchBaseStartBattle
    {
        public PatchRemoveLastWindStartBattle()
        {
            priority = 2842;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(87));
            cursor.Next.OpCode = OpCodes.Nop;
            cursor.GotoNext(i => i.MatchLdsfld(out _));
            Utils.RemoveUntilInst(cursor, i => i.MatchStfld(out _));
            cursor.Remove();
        }

    }
}
