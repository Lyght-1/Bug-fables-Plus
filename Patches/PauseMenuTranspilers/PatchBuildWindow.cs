using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Patches.PauseMenuTranspilers
{
    public class PatchNewChallengeIcons : PatchBasePauseMenuBuildWindow
    {
        public PatchNewChallengeIcons()
        {
            priority = 1435;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdsfld(out _),i=>i.MatchLdfld(out _),i => i.MatchLdcI4(616));
            cursor.Next.OpCode = OpCodes.Nop;
            cursor.GotoNext();

            cursor.Emit(OpCodes.Ldloc, cursor.Instrs[cursor.Index + 4].Operand);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PauseMenu_Ext), "SetBigFableIcon"));
            cursor.Emit(OpCodes.Ldsfld, AccessTools.Field(typeof(MainManager), "instance"));
        }
    }

    public class PatchMedalListType : PatchBasePauseMenuBuildWindow
    {
        public PatchMedalListType()
        {
            priority = 2472;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(3), i => i.MatchLdcI4(1), i => i.MatchLdcI4(0), i=>i.MatchCall(AccessTools.Method(typeof(MainManager),"SetUpList", new Type[] {typeof(int), typeof(bool), typeof(bool)})));
            cursor.Next.OpCode = OpCodes.Ldc_I4;
            cursor.Next.Operand = (int)NewListType.MedalCategories;
        }
    }
}
