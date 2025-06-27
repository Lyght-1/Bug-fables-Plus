using BFPlus.Extensions;
using BFPlus.Patches.BattleControlTranspilers.GetChoiceInput;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Patches.BattleControlTranspilers
{

    public class PatchCheckCanUseSkill : PatchBaseBattleControlSetMaxOptions
    {
        public PatchCheckCanUseSkill()
        {
            priority = 50;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(18));
            Utils.RemoveUntilInst(cursor, i => i.MatchBle(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "CantUseSkillInked"));
            cursor.Next.OpCode = OpCodes.Brtrue;
        }

    }

    public class PatchCheckCanUseItem : PatchBaseBattleControlSetMaxOptions
    {
        public PatchCheckCanUseItem()
        {
            priority = 165;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(19));
            Utils.RemoveUntilInst(cursor, i => i.MatchBle(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "CantUseSkillSticky"));
            cursor.Next.OpCode = OpCodes.Brtrue;
        }

    }
}
