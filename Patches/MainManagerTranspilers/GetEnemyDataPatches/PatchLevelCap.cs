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

namespace BFPlus.Patches.MainManagerTranspilers.GetEnemyDataPatches
{
    public class PatchLevelCap : PatchBaseMainManagerGetEnemyData
    {
        public PatchLevelCap()
        {
            priority = 272;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdfld(AccessTools.Field(typeof(MainManager), "partylevel")));
            cursor.Emit(OpCodes.Ldc_I4, MainManager_Ext.newMaxLevel);
            cursor.Remove();
        }
    }
}
