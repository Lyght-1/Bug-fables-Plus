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

namespace BFPlus.Patches.MainManagerTranspilers.UpdatePatches
{
    public class PatchDestinyDreamHUD : PatchBaseMainManagerUpdate
    {
        public PatchDestinyDreamHUD()
        {
            priority = 2100;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            ILLabel label = null;
            cursor.GotoNext(MoveType.After,i => i.MatchLdcI4(48), i => i.MatchBeq(out label));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "DestinyDreamChangeHud"));
            cursor.Emit(OpCodes.Brtrue, label);
        }
    }
}
