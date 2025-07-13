using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static FlappyBee;

namespace BFPlus.Patches
{
    public class PatchSetSkillListType : PatchBaseMainManagerGetTPCost
    {
        public PatchSetSkillListType()
        {
            priority = 0;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdfld(typeof(MainManager).GetField("playerdata")));
            cursor.GotoNext(MoveType.After,i => i.MatchLdfld(typeof(MainManager).GetField("playerdata")));
            cursor.Emit(OpCodes.Ldsfld, AccessTools.Field(typeof(MainManager_Ext), "skillListType"));
            cursor.Remove();
        }
    }

    /// <summary>
    /// Change tp cost for some of our new medals for certain skills
    /// </summary>
    public class PatchCheckSkillTPCost : PatchBaseMainManagerGetTPCost
    {
        public PatchCheckSkillTPCost()
        {
            priority = 127;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdloc1(), i=>i.MatchLdcI4(25));
            cursor.GotoNext(i => i.MatchLdcI4(25));
            cursor.Emit(OpCodes.Ldarg_1);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchCheckSkillTPCost), "GetNewTPCost"));
            cursor.Emit(OpCodes.Stloc_1);
            cursor.Emit(OpCodes.Ldloc_1);
        }

        static int GetNewTPCost(int tpCost, int skillId, int playerId)
        {
            if (skillId == (int)MainManager.Skills.Cleanse && MainManager.BadgeIsEquipped((int)Medal.RinseRegen))
            {
                tpCost += 1;
            }

            if (skillId == (int)MainManager.Skills.Cleanse && MainManager.BadgeIsEquipped((int)Medal.Liquidate))
            {
                tpCost += 4;
            }

            if (skillId == (int)MainManager.Skills.PeebleToss)
            {
                if (MainManager.BadgeIsEquipped((int)Medal.GrumbleGravel))
                {
                    tpCost += 1;
                }

                if (MainManager.BadgeIsEquipped((int)Medal.SkippingStone))
                {
                    tpCost += 2;
                }

                if (MainManager.BadgeIsEquipped((int)Medal.Avalanche))
                {
                    tpCost += 1;
                }

                if (MainManager.BadgeIsEquipped((int)Medal.TanjyToss))
                {
                    tpCost += 1;
                }

                if (MainManager.BadgeIsEquipped((int)Medal.RockyRampUp) && MainManager.battle != null)
                {
                    tpCost += Mathf.FloorToInt(BattleControl_Ext.Instance.rockyRampUpDmg / 2);
                }
            }

            if (skillId == (int)MainManager.Skills.HardCharge)
            {
                tpCost += 3 * MainManager.BadgeHowManyEquipped((int)Medal.Powerbank, playerId);
            }

            if (MainManager.HasCondition(MainManager.BattleCondition.Inked, MainManager.instance.playerdata[playerId]) > -1 && MainManager.BadgeIsEquipped((int)Medal.InvisibleInk))
                tpCost *= 2;

            return tpCost;
        }
    }
}
