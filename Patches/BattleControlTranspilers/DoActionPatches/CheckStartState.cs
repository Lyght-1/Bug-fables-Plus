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
    public class PatchCheckStartState : PatchBaseDoAction
    {
        public PatchCheckStartState()
        {
            priority = 150051;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchLdfld(typeof(EntityControl).GetField("walkstate")), i => i.MatchLdarg0(), i => i.MatchLdfld(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckStartState"));
        }
    }
}
