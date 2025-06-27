using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Patches.ShowItemListPatches
{
    public class PatchStrageyListValues : PatchBaseShowItemList
    {
        public PatchStrageyListValues()
        {
            priority = 70969;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(5));
            cursor.GotoNext(MoveType.After,i => i.MatchStsfld(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "SetStrategyValues"));
        }
    }

    public class PatchStrategyTextValues : PatchBaseShowItemList
    {
        public PatchStrategyTextValues()
        {
            priority = 74697;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(9), i=> i.MatchBneUn(out _));
            cursor.GotoNext(MoveType.Before, i => i.MatchStloc(out _), i=> i.MatchLdloc(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "SetStrategyTextValues"));

            cursor.GotoNext(MoveType.After, i => i.MatchStloc(out _), i => i.MatchLdloc(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "CheckTrustFallColor"));
            cursor.Emit(OpCodes.Brtrue, cursor.Body.Instructions[cursor.Index+4].Operand);
            cursor.Emit(OpCodes.Ldloc, cursor.Body.Instructions[cursor.Index - 3].Operand);

            //change index of flee to 5
            cursor.GotoNext(i => i.MatchLdcI4(4));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchStrategyTextValues), "GetFleeIndex"));
            cursor.Remove();

            cursor.GotoNext(i => i.MatchLdstr("|icon,189|"));
            cursor.GotoNext(i => i.MatchLdcI4(4));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchStrategyTextValues), "GetFleeIndex"));
            cursor.Remove();
        }

        static int GetFleeIndex()
        {
            return MainManager.BadgeIsEquipped((int)Medal.TrustFall) ? 5 : 4;
        }
    }

    public class PatchStrategyDescValues : PatchBaseShowItemList
    {
        public PatchStrategyDescValues()
        {
            priority = 75304;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(9), i => i.MatchBneUn(out _));
            cursor.GotoNext(MoveType.Before, i => i.MatchStloc(out _), i => i.MatchLdsfld(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "SetStrategyDescValues"));
        }
    }
}
