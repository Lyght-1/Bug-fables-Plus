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
    public class PatchClearStatus : PatchBaseClearStatus
    {
        public PatchClearStatus()
        {
            priority = 47;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchStloc1());
            cursor.Emit(OpCodes.Ldloc_1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchClearStatus), "CheckUnClearableStatus"));
            cursor.Emit(OpCodes.Stloc_1);
        }

        static MainManager.BattleCondition[] CheckUnClearableStatus(MainManager.BattleCondition[] conditions) 
        {
            var tempList = conditions.ToList();
            tempList.Add(MainManager.BattleCondition.Sturdy);

            if (MainManager.BadgeIsEquipped((int)Medal.PermanentInk))
            {
                tempList.Add(MainManager.BattleCondition.Inked);
            }

            if (MainManager.BadgeIsEquipped((int)Medal.SturdyStrands))
            {
                tempList.Add(MainManager.BattleCondition.Sticky);
            }

            return tempList.ToArray();
        }
    }
}
