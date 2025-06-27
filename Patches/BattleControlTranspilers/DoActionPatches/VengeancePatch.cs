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
    public class PatchVengeanceCharge: PatchBaseDoAction
    {
        public PatchVengeanceCharge()
        {
            priority = 44153;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,
                i => i.MatchStfld(AccessTools.Field(typeof(BattleControl), "hasblocked")),
                i => i.MatchLdloc1()
            );

            cursor.Remove();
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckVengeanceCharge"));
        }
    }
}