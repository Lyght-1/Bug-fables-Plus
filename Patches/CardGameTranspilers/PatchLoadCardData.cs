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
using UnityEngine;

namespace BFPlus.Patches.CardGameTranspilers
{
    public class PatchCardData : PatchBaseCardGameLoadCardData
    {
        public PatchCardData()
        {
            priority = 24;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            var patchers = new DataPatcher[]
            {
                new DataPatcher() { loader = OpCodes.Ldloc_0, name = "CardText", foundString = "/CardText", setter = OpCodes.Stloc_0, addEmpty = OpCodes.Ldc_I4_0 },
                new DataPatcher() { loader = OpCodes.Ldloc_1, name = "CardData", foundString = "Data/CardData", setter = OpCodes.Stloc_1,addEmpty = OpCodes.Ldc_I4_0 },
            };

            foreach (var patch in patchers)
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
