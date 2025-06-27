using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BFPlus.Extensions;
using MonoMod.Cil;
using Mono.Cecil.Cil;
namespace BFPlus.Patches.DoActionPatches
{
    public class PatchHardChargeMaxAmount : PatchBaseDoAction
    {
        public PatchHardChargeMaxAmount()
        {
            priority = 51202;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdcI4(81));
            cursor.GotoNext(i=>i.MatchLdcI4(3),i => i.MatchStfld(typeof(MainManager.BattleData).GetField("charge")));

            cursor.Remove();
            cursor.Emit(OpCodes.Ldloc_1);
            cursor.Emit(cursor.Body.Instructions[cursor.Index-3].OpCode, cursor.Body.Instructions[cursor.Index - 3].Operand);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "CheckMaxCharge"));
        }
    }
}
