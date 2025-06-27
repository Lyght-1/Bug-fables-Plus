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

namespace BFPlus.Patches.BattleControlTranspilers.DoActionPatches.EnemyPatches
{
    /// <summary>
    /// add sticky property to wasp needles instead of poison
    /// </summary>
    public class PatchWaspScoutNeedlesProperty : PatchBaseDoAction
    {
        public PatchWaspScoutNeedlesProperty()
        {
            priority = 76805;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(449));
            cursor.GotoNext(i => i.MatchLdloca(out _), i=>i.MatchLdcI4(3));
            cursor.GotoNext();
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, 26);
        }
    }
}
