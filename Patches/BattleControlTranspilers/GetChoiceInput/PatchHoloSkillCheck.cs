using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BFPlus.Patches.BattleControlTranspilers.GetChoiceInput
{

    public class PatchHoloSkillCheck : PatchBaseGetChoiceInput
    {
        public PatchHoloSkillCheck()
        {
            priority = 728;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchStfld(AccessTools.Field(typeof(BattleControl), "lastskill")));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckHoloSkill"));
            Utils.RemoveUntilInst(cursor, i => i.MatchRet());

            //Exclude self skill check
            cursor.GotoNext(i => i.MatchLdsfld(AccessTools.Field(typeof(MainManager), "skilldata")));
            int cursorIndex = cursor.Index;

            ILLabel label = null;
            cursor.GotoNext(i => i.MatchBrtrue(out label));
            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchHoloSkillCheck), "CheckExcludeSelf"));
            cursor.Emit(OpCodes.Brtrue, label);

            //Pretty much only applies to rain dance, but make sure that if the targetted player is stopped, we cant do rain dance
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchHoloSkillCheck), "CheckSkillStopped"));
            cursor.Emit(OpCodes.Brfalse, label);

            cursor.GotoNext(i => i.MatchCall(AccessTools.Method(typeof(BattleControl), "DoAction")));
            cursor.GotoPrev(i => i.MatchLdarg0(), i => i.MatchLdarg0());
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckHoloSkill"));
            Utils.RemoveUntilInst(cursor, i => i.MatchRet());
        }

        static bool CheckExcludeSelf()
        {
            return MainManager.battle.excludeself && MainManager.battle.option == MainManager.battle.currentturn;
        }

        static bool CheckSkillStopped()
        {
            if (MainManager.battle.tempskill == (int)NewSkill.RainDance)
            {
                return !MainManager.battle.IsStopped(MainManager.instance.playerdata[MainManager.battle.target]);
            }

            return true;
        }

    }
}
