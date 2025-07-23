using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using BFPlus.Patches.MainManagerTranspilers;
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
    //EETL RUIGEE EVENT
    public class PatchLevelCap : PatchBaseEvent208
    {
        public PatchLevelCap()
        {
            priority = 249299;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.Goto(0);
            while (cursor.TryGotoNext(i => i.MatchLdcI4(27)))
            {
                cursor.Emit(OpCodes.Ldc_I4, MainManager_Ext.newMaxLevel);
                cursor.Remove();
                cursor.GotoNext();
            }
        }
    }

    //Add the MP Plus Medal bonuses from the max mp in ruigee, preventing losing mp if you have the MP Plus medal at level 1
    public class PatchMPPlusBonus : PatchBaseEvent208
    {
        public PatchMPPlusBonus()
        {
            priority = 0;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.Goto(0);
            cursor.GotoNext(i => i.MatchStfld(AccessTools.Field(typeof(MainManager), "maxbp")));
            cursor.Emit(OpCodes.Ldsfld, AccessTools.Field(typeof(MainManager_Ext), "mpPlusBonus"));
            cursor.Emit(OpCodes.Add);
        }
    }

    public class PatchLevelDataRuigee : PatchBaseEvent208
    {
        public PatchLevelDataRuigee()
        {
            priority = 249559;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.Goto(0);
            var patcher = new DataPatcher() { loader = OpCodes.Ldfld, name = "LevelData", foundString = "Data/LevelData", setter = OpCodes.Stfld, completeReplace = OpCodes.Ldc_I4_1 };

            cursor.GotoNext(i => i.MatchLdstr(patcher.foundString));
            cursor.GotoNext(i => i.OpCode == patcher.setter);
            cursor.Emit(OpCodes.Ldstr, patcher.name);
            cursor.Emit(OpCodes.Ldstr, patcher.delimiter);
            cursor.Emit(patcher.completeReplace);
            cursor.Emit(patcher.addEmpty);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "GetNewItems", new Type[] { typeof(string[]), typeof(string), typeof(string), typeof(bool), typeof(bool) }));
        }
    }
}
