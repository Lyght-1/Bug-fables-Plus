using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using BFPlus.Extensions;
using MonoMod.Cil;

namespace BFPlus.Patches.DoActionPatches
{
    public class PatchEnemyDropping : PatchBaseDoAction
    {
        public PatchEnemyDropping()
        {

            priority = 150851;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchCall(AccessTools.Method(typeof(BattleControl), "EnemyDropping")));
            cursor.GotoNext(MoveType.Before,i=>i.MatchLdloc1(), i=> i.MatchLdcI4(1), i => i.MatchStfld(typeof(BattleControl).GetField("action")));
            cursor.RemoveRange(3);
            cursor.GotoNext(i => i.MatchCall(AccessTools.Method(typeof(BattleControl), "EnemyDropping")));
            cursor.GotoNext(MoveType.Before, i => i.MatchLdloc1(), i => i.MatchLdcI4(0), i => i.MatchStfld(typeof(BattleControl).GetField("action")));
            cursor.RemoveRange(3);
        }
    }
}
