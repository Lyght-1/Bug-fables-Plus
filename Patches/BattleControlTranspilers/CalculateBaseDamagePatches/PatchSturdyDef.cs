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

namespace BFPlus.Patches.BattleControlTranspilers.CalculateBaseDamagePatches
{

    /// <summary>
    /// Remove the -3 dmg if the target has sturdy and isnt a player, def remains if it is a player
    /// </summary>
    public class PatchSturdyDef : PatchBaseCalculateBaseDamage
    {
        public PatchSturdyDef()
        {
            priority = 1953;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdarg3(),i => i.MatchLdcI4(3), i => i.MatchSub());
            var brJump = cursor.Prev.Operand;
            cursor.Emit(OpCodes.Ldloc_0);
            cursor.Emit(OpCodes.Brfalse, brJump);
        }

    }
}
