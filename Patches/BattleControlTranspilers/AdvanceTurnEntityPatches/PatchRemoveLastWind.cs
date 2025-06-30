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

namespace BFPlus.Patches.BattleControlTranspilers.AdvanceTurnEntityPatches
{
    public class PatchRemoveLastWindAdvanceTurnEntity : PatchBaseAdvanceTurnEntity
    {
        public PatchRemoveLastWindAdvanceTurnEntity()
        {
            priority = 867;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(87));
            Utils.RemoveUntilInst(cursor, i => i.MatchBr(out _));
        }
    }
}
