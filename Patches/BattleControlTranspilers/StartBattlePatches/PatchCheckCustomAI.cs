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
    public class PatchCheckCustomAI : PatchBaseStartBattle
    {
        public PatchCheckCustomAI()
        {
            priority = 2932;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,
                i => i.MatchLdnull(), 
                i => i.MatchStfld(AccessTools.Field(typeof(BattleControl), "aiparty")));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "CheckCustomAI"));
        }

    }
}
