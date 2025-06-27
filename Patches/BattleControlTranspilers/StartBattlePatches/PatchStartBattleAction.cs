using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Patches.BattleControlTranspilers.StartBattlePatches
{
    /// <summary>
    /// This fucks with action if you get firstroke into all party gets stopped, resulting in doaction getting called multiple times
    /// </summary>
    public class PatchStartBattleAction : PatchBaseStartBattle
    {
        public PatchStartBattleAction()
        {
            priority = 4189;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,
                i => i.MatchStfld(AccessTools.Field(typeof(BattleControl), "currentturn")),
                i => i.MatchLdsfld(out _),
                i => i.MatchLdcI4(0),
                i => i.MatchStfld(AccessTools.Field(typeof(BattleControl), "action")));

            cursor.GotoPrev(i => i.MatchLdsfld(out _));
            cursor.RemoveRange(3);
        }

    }
}
