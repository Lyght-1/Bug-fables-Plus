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

namespace BFPlus.Patches.BattleControlTranspilers.UseItemPatches
{
    public class PatchNewItemAction : PatchBaseUseItem
    {
        public PatchNewItemAction()
        {
            priority = 18348;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(13));
            cursor.GotoNext(i => i.OpCode == OpCodes.Ldc_I4_M1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckNewItemAction"));
            cursor.Remove();
        }

    }
}
