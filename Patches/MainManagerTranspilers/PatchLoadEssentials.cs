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
    public class DataPatcher
    {
        public OpCode loader;
        public string name;
        public OpCode setter;
        public OpCode completeReplace = OpCodes.Ldc_I4_0;
        public string foundString;
        public string delimiter = "{";
        public bool tattleList = false;
        public OpCode addEmpty = OpCodes.Ldc_I4_1;
    }
    public class PatchNewData : PatchBaseMainManagerLoadEssentials
    {
        public PatchNewData()
        {
            priority = 0;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            var patchers = new DataPatcher[]
            {
                new DataPatcher() { loader = OpCodes.Ldloc_2, name = "Termacade", foundString = "Data/Termacade", setter = OpCodes.Stloc_2, addEmpty = OpCodes.Ldc_I4_0, completeReplace = OpCodes.Ldc_I4_1 },
                new DataPatcher() { loader = OpCodes.Ldloc_2, name = "RecipeData", foundString = "Data/RecipeData", setter = OpCodes.Stloc_2 },
            };

            foreach(var patch in patchers)
            {
                cursor.GotoNext(i => i.MatchLdstr(patch.foundString));
                cursor.GotoNext(MoveType.After, i => i.OpCode == patch.setter);
                cursor.Emit(patch.loader);
                cursor.Emit(OpCodes.Ldstr, patch.name);
                cursor.Emit(OpCodes.Ldstr, patch.delimiter);
                cursor.Emit(patch.completeReplace);
                cursor.Emit(patch.addEmpty);
                cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "GetNewItems", new Type[] { typeof(string[]), typeof(string), typeof(string), typeof(bool), typeof(bool) }));
                cursor.Emit(patch.setter);
            }
        }
    }
    
}
