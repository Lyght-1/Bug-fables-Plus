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

namespace BFPlus.Patches.BattleControlTranspilers.CheckDeadPatches
{
    public class PatchCotUnknownCheck : PatchBaseCheckDead
    {
        public PatchCotUnknownCheck()
        {
            priority = 155929;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdfld(AccessTools.Field(typeof(EntityControl), "cotunknown")));
            cursor.GotoPrev(i => i.MatchLdloc1());
            Utils.RemoveUntilInst(cursor, i => i.MatchLdsfld(out _));
        }

    }
}
