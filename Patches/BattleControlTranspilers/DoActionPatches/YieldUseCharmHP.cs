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

    public class YieldUseCharmHP : PatchBaseDoAction
    {
        public YieldUseCharmHP()
        {
            priority = 150794;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,
                i=>i.MatchLdcI4(3),
                i => i.MatchCall(AccessTools.Method(typeof(BattleControl), "UseCharm"))
            );
            cursor.Remove();
            cursor.Remove();
            Utils.InsertYieldReturn(cursor);
            int cursorIndex = cursor.Index;

            cursor.GotoPrev(i => i.MatchLdloc1(), i => i.MatchLdloc1(), i => i.MatchLdloc1());
            cursor.Remove();
            cursor.Remove();
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Goto(cursorIndex);
        }
    }

    public class YieldUseCharmDef : PatchBaseDoAction
    {
        public YieldUseCharmDef()
        {
            priority = 62804;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,
                i => i.MatchLdcI4(0),
                i => i.MatchCall(AccessTools.Method(typeof(BattleControl), "UseCharm"))
            );
            cursor.Remove();
            cursor.Remove();
            Utils.InsertYieldReturn(cursor);
            int cursorIndex = cursor.Index;

            cursor.GotoPrev(i => i.MatchLdloc1(), i => i.MatchLdloc1(), i => i.MatchLdloc1());
            cursor.Remove();
            cursor.Remove();
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Goto(cursorIndex);
        }
    }
}
