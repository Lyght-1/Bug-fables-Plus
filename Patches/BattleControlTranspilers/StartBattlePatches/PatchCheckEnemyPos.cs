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

namespace BFPlus.Patches.BattleControlTranspilers.StartBattlePatches
{

    public class PatchCheckEnemyPos : PatchBaseStartBattle
    {
        public PatchCheckEnemyPos()
        {
            priority = 2360;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(
                i=> i.MatchLdarg0(),
                i=> i.MatchLdfld(out _),
                i => i.MatchLdcI4(95));

            cursor.Next.OpCode = OpCodes.Nop;
            cursor.GotoNext(i=>i.MatchLdfld(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckEnemyPos"));
            cursor.Emit(OpCodes.Ldarg_0);
        }

    }
}
