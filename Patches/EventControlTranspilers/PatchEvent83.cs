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
    /// <summary>
    /// Add new bounty reward in the array
    /// </summary>
    public class PatchNewBountyRewards : PatchBaseEvent83
    {
        public PatchNewBountyRewards()
        {
            priority = 91351;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(42));
            cursor.GotoNext(i => i.MatchStfld(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchNewBountyRewards), "AddBountyReward"));
        }

        static int[][] AddBountyReward(int[][] data)
        {
            int[][] newData = new int[][]
            {
                new int[] { (int)NewItem.SilverClaw, -(int)Medal.FrostNeedles, (int)NewEnemies.DullScorp, (int)NewQuest.BountyDullScorp },
                new int[] { (int)NewItem.SilverFuse, -(int)Medal.Powerbank, (int)NewEnemies.DynamoSpore, (int)NewQuest.BountyDynamoSpore },
                new int[] { (int)NewItem.SilverFossil, -(int)Medal.NoPainNoGain, (int)NewEnemies.Belosslow, (int)NewQuest.BountyBelosslow },
                new int[] { (int)NewItem.SilverCard, -(int)Medal.OddWarrior, (int)NewEnemies.IronSuit, (int)NewQuest.BountyIronSuit },
                new int[] { (int)NewItem.SilverHandle, -(int)Medal.FieryHeart, (int)NewEnemies.Jester, (int)NewQuest.BountyJester },
            };
            return data.AddRangeToArray(newData);
        }
    }

    /// <summary>
    /// add new bounty amount (from 5 -> 10)
    /// </summary>
    public class PatchNewBountyAmount : PatchBaseEvent83
    {
        public PatchNewBountyAmount()
        {
            priority = 91643;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdcI4(35), i=>i.MatchLdelemI4());
            cursor.Emit(OpCodes.Ldc_I4, 10);
            cursor.Remove();
        }
    }

    /// <summary>
    /// Make sure we remove the last quest too if we didnt take it
    /// </summary>
    public class PatchBountyQuestbug : PatchBaseEvent83
    {
        public PatchBountyQuestbug()
        {
            priority = 91701;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(612));
            ILLabel label = cursor.DefineLabel();
            cursor.GotoNext(i => i.MatchBr(out _));
            cursor.Next.Operand = label;

            cursor.GotoNext(i => i.MatchLdsfld(out _), i => i.MatchLdfld(AccessTools.Field(typeof(MainManager), "boardquests")));
            cursor.MarkLabel(label);
        }
    }
}
