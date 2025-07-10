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

namespace BFPlus.Patches.BattleControlTranspilers.DoActionPatches.SkillPatches
{
    /// <summary>
    /// We insert our new formula for ttoss damages
    /// </summary>
    public class PatchViTornadoToss : PatchBaseDoAction
    {
        public PatchViTornadoToss()
        {
            priority = 59785;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdfld(AccessTools.Field(typeof(BattleControl),"lastdamage")));
            cursor.GotoNext(i => i.MatchSub(), i => i.MatchStloc(out _));
            cursor.Remove();
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchViTornadoToss), "GetTornadoHits"));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "GetMultiHitDamage"));
        }

        static int GetTornadoHits()
        {
            return MainManager.BadgeIsEquipped((int)MainManager.BadgeTypes.Beemerang2) ? 5 : 4;
        }
    }
}
