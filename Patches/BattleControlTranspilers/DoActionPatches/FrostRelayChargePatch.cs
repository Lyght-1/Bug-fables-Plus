using BFPlus.Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace BFPlus.Patches.DoActionPatches
{
    public class FrostRelayChargePatch : PatchBaseDoAction
    {
        public FrostRelayChargePatch()
        {
            priority = 48615;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchPop(),i => i.MatchLdstr("IceMothHit"), i=>i.MatchCall(out _));
            cursor.GotoNext(i => i.MatchPop(), i=>i.MatchLdsfld(out _));
            cursor.GotoNext();
            SkipChargeReset(cursor);

            cursor.GotoNext(i => i.MatchPop(), i => i.MatchLdsfld(out _));
            cursor.GotoNext();
            SkipChargeReset(cursor);
        }

        void SkipChargeReset(ILCursor cursor) 
        {
            var label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(cursor.Body.Instructions[cursor.Index + 3].OpCode, cursor.Body.Instructions[cursor.Index + 3].Operand);
            cursor.Emit(cursor.Body.Instructions[cursor.Index + 4].OpCode, cursor.Body.Instructions[cursor.Index + 4].Operand);
            cursor.Emit(cursor.Body.Instructions[cursor.Index + 5].OpCode, cursor.Body.Instructions[cursor.Index + 5].Operand);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CanUseCharge"));
            cursor.Emit(OpCodes.Brfalse, label);
            cursor.GotoNext(MoveType.After, i => i.MatchStfld(typeof(MainManager.BattleData).GetField("charge")));
            cursor.MarkLabel(label);
        }
    }
}
