using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Patches.BattleControlTranspilers.AdvanceTurnEntityPatches
{
    /// <summary>
    /// Reduces Heavy sleeper healing from *3 to *2
    /// </summary>
    public class PatchHeavySleeper : PatchBaseAdvanceTurnEntity
    {
        public PatchHeavySleeper()
        {
            priority = 472;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(47));
            cursor.GotoNext(i => i.MatchLdcI4(3));
            cursor.Emit(OpCodes.Ldc_I4, 2);
            cursor.Remove();
        }
    }
}
