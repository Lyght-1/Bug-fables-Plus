using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using BFPlus.Patches.ShowItemListPatches;
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
    public class PatchTrustFallOption : PatchBaseMainManagerUpdate
    {
        public PatchTrustFallOption()
        {
            priority = 2135;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            ILLabel label = null;
            cursor.GotoNext(
                i=>i.MatchBneUn(out label),
                i => i.MatchLdsfld(out _),
                i => i.MatchLdfld(AccessTools.Field(typeof(BattleControl), "currentaction")));

            cursor.GotoNext(i => i.MatchLdarg0());
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "CanUseTrustFall"));
            cursor.Emit(OpCodes.Brtrue, label);

            //push the flee index to 5 if trust fall is equipped
            cursor.GotoNext(i => i.MatchLdcI4(4));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchStrategyTextValues), "GetFleeIndex"));
            cursor.Remove();
        }
    }
}
