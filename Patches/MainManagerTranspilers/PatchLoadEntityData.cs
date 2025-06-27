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

namespace BFPlus.Patches.MainManagerTranspilers
{
    public class PatchNewEntityData : PatchBaseMainManagerLoadEntityData
    {
        public PatchNewEntityData()
        {
            priority = 0;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdstr("Data/EntityValues"));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "GetEntityValues"));
            Utils.RemoveUntilInst(cursor, i=>i.MatchStloc0());

            cursor.GotoNext(i => i.MatchLdtoken(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "GetEntityValuesAmount"));
            Utils.RemoveUntilInst(cursor, i => i.MatchNewarr(out _));
        }
    }
}
