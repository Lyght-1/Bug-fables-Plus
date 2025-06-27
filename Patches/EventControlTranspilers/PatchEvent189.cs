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

namespace BFPlus.Patches.EventControlTranspilers
{
    //Libram Discovery reward event
    public class PatchNewDiscoveryReward : PatchBaseEvent189
    {
        public PatchNewDiscoveryReward()
        {
            priority = 219675;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            //changes reward amount for dialogue check
            cursor.GotoNext(i => i.MatchLdcI4(10));
            cursor.Next.Operand = MainManager_Ext.DiscoveriesRewardAmount;
            cursor.Next.OpCode = OpCodes.Ldc_I4;

            cursor.GotoNext(i => i.MatchLdcI4(10));
            cursor.GotoNext(i => i.MatchStfld(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchNewDiscoveryReward), "AddNewRewards"));

            //changes reward amount for dialogue check
            cursor.GotoNext(i => i.MatchLdcI4(10));
            cursor.Next.Operand = MainManager_Ext.DiscoveriesRewardAmount;
            cursor.Next.OpCode = OpCodes.Ldc_I4;
        }

        static int[] AddNewRewards(int[] rewards)
        {
            var tempList = rewards.ToList();

            //Tp saver at 40 discoveries
            tempList.Insert(7, (int)MainManager.BadgeTypes.TPSaver);
            tempList.Add((int)MainManager.Items.SuperHPPotion + 2000);
            return tempList.ToArray();
        }
    }
}
