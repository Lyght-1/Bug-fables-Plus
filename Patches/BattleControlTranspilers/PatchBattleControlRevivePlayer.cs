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

namespace BFPlus.Patches.BattleControlTranspilers
{
    public class PatchRemoveLastWindRevivePlayer : PatchBaseBattleControlRevivePlayer
    {
        public PatchRemoveLastWindRevivePlayer()
        {
            priority = 69;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(87));
            cursor.Next.OpCode = OpCodes.Nop;
            cursor.GotoNext(i => i.MatchLdsfld(out _));
            Utils.RemoveUntilInst(cursor, i => i.MatchLdcI4(0));
            cursor.GotoNext(i => i.MatchLdcI4(-1));
            cursor.RemoveRange(2);
        }

    }
}
