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

namespace BFPlus.Patches.StartMenuTranspilers
{
    public class PatchLoadNewTitle : PatchBaseStartMenuIntro
    {
        public PatchLoadNewTitle()
        {
            priority = 131;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdstr(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "LoadNewTitle"));
            cursor.RemoveRange(5);
        }
    }
}
