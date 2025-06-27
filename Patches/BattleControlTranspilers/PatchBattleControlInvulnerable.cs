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
using UnityEngine;

namespace BFPlus.Patches.BattleControlTranspilers
{
    /// <summary>
    /// Remove the shock trooper check in invulnerable
    /// </summary>
    public class PatchBattleControlInvulnerable : PatchBaseBattleControlInvulnerable
    {
        public PatchBattleControlInvulnerable()
        {
            priority = 30;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdloc0());
            Utils.RemoveUntilInst(cursor, i => i.MatchLdcI4(0), i => i.MatchRet());
        }
    }
}
