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
    public class AddDelayedConditionRes : PatchBaseDoAction
    {
        public AddDelayedConditionRes()
        {
            priority = 150495;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdfld(AccessTools.Field(typeof(MainManager.BattleData), "delayedcondition")));
            cursor.GotoNext(MoveType.After,i => i.MatchCallvirt(AccessTools.Method(typeof(EntityControl), "Freeze")));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(AddDelayedConditionRes), "AddFreezeRes"));

            cursor.GotoNext(MoveType.After, i => i.MatchLdstr("Numb"),i=>i.MatchCall(out _), i=>i.MatchPop());
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(AddDelayedConditionRes), "AddNumbRes"));

            cursor.GotoNext(MoveType.After, i => i.MatchLdstr("Sleep"), i => i.MatchCall(out _), i => i.MatchPop());
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(AddDelayedConditionRes), "AddSleepRes"));
        }
        static void AddFreezeRes()
        {
            MainManager.battle.enemydata[MainManager.battle.actionid].freezeres += 10;
        }

        static void AddNumbRes()
        {
            MainManager.battle.enemydata[MainManager.battle.actionid].numbres += 10;
        }

        static void AddSleepRes()
        {
            MainManager.battle.enemydata[MainManager.battle.actionid].sleepres += 10;
        }
    }

}
