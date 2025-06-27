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

namespace BFPlus.Patches.CardGameTranspilers
{

    public class PatchCardOrder : PatchBaseCardGameStartCard
    {
        public PatchCardOrder()
        {
            priority = 300;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdstr("Data/CardOrder"));
            cursor.GotoNext(MoveType.After,i => i.MatchStloc2());
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchCardOrder), "GetNewCardOrder"));
            cursor.Emit(OpCodes.Stloc_2);
        }

        static string[] GetNewCardOrder()
        {
           return MainManager_Ext.assetBundle.LoadAsset<TextAsset>("CardOrder").ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
