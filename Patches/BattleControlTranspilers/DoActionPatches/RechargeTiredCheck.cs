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
    public class PatchCheckRechargeTired : PatchBaseDoAction
    {
        public PatchCheckRechargeTired()
        {
            priority = 150415;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdflda(typeof(MainManager.BattleData).GetField("tired")));

            ILLabel label = null;
            cursor.GotoPrev(MoveType.After, i => i.MatchBeq(out label));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckRecharge", new Type[] { }));
            cursor.Emit(OpCodes.Brfalse, label);

            cursor.GotoNext(i => i.MatchLdflda(typeof(MainManager.BattleData).GetField("tired")));
            cursor.GotoNext(i => i.MatchLdflda(typeof(MainManager.BattleData).GetField("tired")));
            cursor.GotoPrev(MoveType.After,i => i.MatchBrtrue(out label));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckRecharge", new Type[] { }));
            cursor.Emit(OpCodes.Brfalse, label);
        }
    }
}
