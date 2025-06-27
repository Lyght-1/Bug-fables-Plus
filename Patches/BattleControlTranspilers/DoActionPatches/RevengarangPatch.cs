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
    public class PatchCheckRevengarang : PatchBaseDoAction
    {
        public PatchCheckRevengarang() 
        {
            priority = 150484;
        
        }

        protected override void ApplyPatch(ILCursor cursor)
        {

            cursor.GotoNext(i => i.MatchCall(AccessTools.Method(typeof(BattleControl), "EndPlayerTurn")));
            cursor.GotoNext(MoveType.After,i => i.MatchBr(out _));
            cursor.Next.OpCode = OpCodes.Nop;
            cursor.GotoNext();
            cursor.GotoNext();
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckRevengarang"));
            Utils.InsertYieldReturn(cursor);
            cursor.Emit(OpCodes.Ldloc_1);
        }
    }
}
