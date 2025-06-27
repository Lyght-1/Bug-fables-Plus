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
    public class PatchMaxLevelAchievement : PatchBaseMainManagerCheckAchievement
    {
        public PatchMaxLevelAchievement()
        {
            priority = 309;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchLdfld(AccessTools.Field(typeof(MainManager), "partylevel")));
            cursor.Emit(OpCodes.Ldc_I4, MainManager_Ext.newMaxLevel);
            cursor.Remove();
        }
    }

    public class PatchAllMedalsAchievement : PatchBaseMainManagerCheckAchievement
    {
        public PatchAllMedalsAchievement()
        {
            priority = 280;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(120));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "GetMaxMedals"));
            cursor.Remove();
        }
    }

    public class PatchAllCrystalBerriesAchievement : PatchBaseMainManagerCheckAchievement
    {
        public PatchAllCrystalBerriesAchievement()
        {
            priority = 263;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(50));
            cursor.Emit(OpCodes.Ldsfld, AccessTools.Field(typeof(MainManager_Ext), "CBFlagNumber"));
            cursor.Remove();
        }
    }

    public class PatchAllQuestsAchievement : PatchBaseMainManagerCheckAchievement
    {
        public PatchAllQuestsAchievement()
        {
            priority = 325;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdtoken(out _));
            Utils.RemoveUntilInst(cursor, i => i.MatchBlt(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "GetAllQuestsAmount"));
        }
    }
}
