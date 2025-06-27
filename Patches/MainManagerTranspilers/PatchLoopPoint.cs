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
using UnityEngine;

namespace BFPlus.Patches.MainManagerTranspilers
{
    public class PatchLoopPoint : PatchBaseMainManagerLoopPoint
    {
        public PatchLoopPoint()
        {
            priority = 26;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchStloc0());
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchLoopPoint), "AddNewLoopPoints"));
        }

        static string[] AddNewLoopPoints(string[] oldLoopPoints)
        {
            return oldLoopPoints.AddRangeToArray(MainManager_Ext.assetBundle.LoadAsset<TextAsset>("LoopPoints").ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
