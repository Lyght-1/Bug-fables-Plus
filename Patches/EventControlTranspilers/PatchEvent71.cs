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

namespace BFPlus.Patches.EventControlTranspilers
{
    public class PatchGetDestinyDream : PatchBaseEvent71
    {
        public PatchGetDestinyDream()
        {
            priority = 77487;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdsfld(out _),i=>i.MatchLdfld(out _),i=>i.MatchLdcI4(39));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(EventControl_Ext), "CheckDestinyDreamGet"));
            Utils.RemoveUntilInst(cursor, i=>i.MatchBr(out _));
        }
    }

    public class PatchNewClues : PatchBaseEvent71
    {
        public PatchNewClues()
        {
            priority = 77690;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdstr("/FortuneTeller"));
            cursor.GotoNext(MoveType.After,i => i.MatchStfld(out _));

            var dataRef = cursor.Prev.Operand;
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, dataRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "GetNewFortuneTellerClues"));
            cursor.Emit(OpCodes.Stfld, dataRef);
        }
    }

    public class PatchNewCluesFlags : PatchBaseEvent71
    {
        public PatchNewCluesFlags()
        {
            priority = 77717;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(26));
            cursor.GotoNext(MoveType.After,i => i.MatchStfld(out _));

            var dataRef = cursor.Prev.Operand;
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, dataRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "GetNewFortuneTellerFlags"));
            cursor.Emit(OpCodes.Stfld, dataRef);
        }
    }
}
