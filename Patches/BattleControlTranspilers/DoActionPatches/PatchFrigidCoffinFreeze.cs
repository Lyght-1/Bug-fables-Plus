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

namespace BFPlus.Patches.BattleControlTranspilers.DoActionPatches
{
    public class PatchFrigidCoffinFreeze : PatchBaseDoAction
    {

        public PatchFrigidCoffinFreeze()
        {
            priority = 46135;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            ILLabel label = null;
            cursor.GotoNext(MoveType.After, i => i.MatchLdcI4(116), i => i.MatchStfld(out _));
            cursor.GotoNext(MoveType.After, i => i.MatchLdcI4(2), i => i.MatchBle(out label));

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchFrigidCoffinFreeze), "CheckFrigidFreeze"));
            cursor.Emit(OpCodes.Brtrue, label);
        }

        static bool CheckFrigidFreeze()
        {
            return BattleControl_Ext.Instance.IsStatusImmune(MainManager.battle.enemydata[MainManager.battle.target], MainManager.BattleCondition.Freeze);
        }
    }
}
