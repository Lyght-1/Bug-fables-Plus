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

namespace BFPlus.Patches.BattleControlTranspilers
{
    /// <summary>
    /// Reset a bunch of stuff when gameover is called
    /// </summary>
    public class PatchResetStuff : PatchBaseBattleControlGameover
    {
        public PatchResetStuff()
        {
            priority = 0;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdstr("|boxstyle,-1|"), i => i.MatchStloc2());
            cursor.Emit(OpCodes.Call,AccessTools.Method(typeof(PatchResetStuff), "ResetStuff"));
        }

        static void ResetStuff()
        {
            BattleControl_Ext.Instance.ResetStuff();
            
            //This is because of pattons messing max hp around
            MainManager.ApplyBadges();
            MainManager.ApplyStatBonus();
        }
    }
}
