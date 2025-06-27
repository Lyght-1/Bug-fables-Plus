using BFPlus.Extensions;
using BFPlus.Patches.BattleControlTranspilers.AddExperiencePatches;
using BFPlus.Patches.DoActionPatches;
using BFPlus.Patches.MainManagerTranspilers;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Patches
{
    public class PatchMainManagerLevelUpMessage : PatchBaseMainManagerLevelUpMessage
    {
        public PatchMainManagerLevelUpMessage()
        {
            priority = 0;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
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
