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
    public class PatchDestinyDreamLifecast : PatchBaseDoAction
    {
        public PatchDestinyDreamLifecast()
        {
            priority = 44304;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(72));
            var label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "DestinyDreamSetHP"));
            cursor.Emit(OpCodes.Brtrue, label);
            cursor.GotoNext(MoveType.Before, i => i.MatchLdarg0(), i => i.MatchLdsfld(out _), i => i.MatchLdfld(typeof(MainManager).GetField("flagvar")));
            cursor.MarkLabel(label);
        }
    }
}
