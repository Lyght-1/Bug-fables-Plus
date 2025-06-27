using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Patches.BattleControlTranspilers.CalculateBaseDamagePatches
{
    public class PatchFrostbiteDamage : PatchBaseCalculateBaseDamage
    {
        public PatchFrostbiteDamage()
        {
            priority = 2108;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(0), i => i.MatchRet());
            cursor.RemoveRange(2);

            //slugskin check
            cursor.GotoPrev(MoveType.After,i => i.MatchLdarg3());
            cursor.Emit(OpCodes.Ldarg_2);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchFrostbiteDamage), "CheckFrostBiteDamage"));
        }


        static int CheckFrostBiteDamage(int baseValue, ref MainManager.BattleData target)
        {
            if (Entity_Ext.GetEntity_Ext(target.battleentity).slugskinActive)
                return baseValue+1;
            return baseValue;
        }
    }
}
