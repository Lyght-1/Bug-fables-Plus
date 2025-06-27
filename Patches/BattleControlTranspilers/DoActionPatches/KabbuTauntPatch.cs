using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BFPlus.Extensions;
using MonoMod.Cil;
using Mono.Cecil.Cil;
namespace BFPlus.Patches.DoActionPatches
{
    public class PatchKabbuTaunt : PatchBaseDoAction
    {
        public PatchKabbuTaunt()
        {
            priority = 56223;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdstr("Taunt2"));
            var br = cursor.Body.Instructions[cursor.Index - 8];

            cursor.GotoNext(MoveType.After,i => i.MatchCall(AccessTools.Method(typeof(BattleControl), "SetDefaultCamera", new Type[] { })));
            var label = cursor.DefineLabel();

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(Extensions.EnemyAI.DarkTeamSnakemouth), "CantTauntDTS"));
            cursor.Emit(OpCodes.Brfalse, label);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(Extensions.EnemyAI.DarkTeamSnakemouth), "DoAntiTaunt"));
            Utils.InsertYieldReturn(cursor);
            cursor.Emit(OpCodes.Br, br.Operand);

            cursor.Emit(OpCodes.Nop).MarkLabel(label);
            Utils.InsertStartStylishTimer(cursor,20f,30f,commandSuccess: false);
        }
    }

    public class PatchKabbuTauntWaitStylish : PatchBaseDoAction
    {
        public PatchKabbuTauntWaitStylish()
        {
            priority = 56579;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(
                MoveType.After,
                i => i.MatchLdfld(AccessTools.Field(typeof(BattleControl), "currentturn")), 
                i => i.MatchStfld(AccessTools.Field(typeof(BattleControl), "forceattack"))
            );

            cursor.Emit(OpCodes.Ldarg_0);
            Utils.InsertWaitStylish(cursor);
        }
    }
}
