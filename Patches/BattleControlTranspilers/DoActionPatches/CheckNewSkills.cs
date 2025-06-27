using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BFPlus.Extensions;
using MonoMod.Cil;
using System.Reflection;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace BFPlus.Patches.DoActionPatches
{
    public class PatchCheckNewSkills : PatchBaseDoAction
    {
        public PatchCheckNewSkills()
        {
            priority = 44427;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, 
                i => i.MatchLdfld(typeof(BattleControl).GetField("checkingdead")), 
                i => i.MatchBrtrue(out _));
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext),"CheckNewSkills"));
            Utils.InsertYieldReturn(cursor);
        }

    }
}
