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
    /// If we are in a first strike, make sure that we dont return out of update otherwise the action will be set to false in start battle. this happens
    /// when an enemy that is supposed to attack gets stopped by an item using an item that first struck
    /// </summary>
    public class PatchFirstStrikeCantMoved : PatchBaseBattleControlUpdate
    {
        public PatchFirstStrikeCantMoved()
        {
            priority = 586;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i=>i.MatchLdcI4(1),i => i.MatchStfld(AccessTools.Field(typeof(MainManager.BattleData), "cantmove")));
            int cursorIndex = cursor.Index;
            ILLabel label = null;
            cursor.GotoNext(i => i.MatchBlt(out label));
            cursor.Goto(cursorIndex);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, AccessTools.Field(typeof(BattleControl), "firststrike"));
            cursor.Emit(OpCodes.Brtrue, label);
        }
    }
}
