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

namespace BFPlus.Patches.BattleControlTranspilers.DoActionPatches
{
    /// <summary>
    /// We call setLastturn after the hitaction, in vanilla, hitaction cant stun the enemy, but here since any enemy
    /// could use any items, it is possible that the hitaction stuns a party member making so we have to reset the setlastturn
    /// </summary>
    public class PatchHitActionSetLastTurn : PatchBaseDoAction
    {
        public PatchHitActionSetLastTurn()
        {
            priority =150826;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchLdcI4(0), i => i.MatchStfld(AccessTools.Field(typeof(BattleControl), "enemy")));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchHitActionSetLastTurn), "FixLastTurns"));
        }

        static void FixLastTurns()
        {
            if(MainManager.battle.chompyattack == null && !BattleControl_Ext.Instance.inAiAttack)
            {
                MainManager.battle.currentturn = -1;
                MainManager.battle.SetLastTurns();
            }
        }
    }
}
