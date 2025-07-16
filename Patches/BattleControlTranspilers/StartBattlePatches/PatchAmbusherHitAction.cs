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
    public class PatchAmbusherHitAction : PatchBaseStartBattle
    {
        public PatchAmbusherHitAction()
        {
            priority = 3785;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, j => j.MatchLdcI4(86));
            cursor.GotoNext(MoveType.After, j => j.MatchLdcI4(21));
            cursor.GotoNext(MoveType.After, i => i.MatchLdnull(), i=>i.MatchStfld(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchAmbusherHitAction), "SetHitAction"));
        }

        static void SetHitAction()
        {
            for(int i = 0; i < MainManager.battle.enemydata.Length; i++)
            {
                MainManager.battle.enemydata[i].hitaction = false;
            }
        }
    }
}
