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
using UnityEngine;

namespace BFPlus.Patches.EntityControlTranspilers
{
    /// <summary>
    /// Add a check for perkfectionist item drops
    /// </summary>
    public class PatchCheckPerkfectItemDrop : PatchBaseEntityControlDeath
    {
        public PatchCheckPerkfectItemDrop()
        {
            priority = 5171;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdfld(AccessTools.Field(typeof(NPCControl), "vectordata")));

            cursor.GotoNext(i => i.MatchLdcI4(614));
            //5294
            cursor.GotoNext(i => i.MatchStloc(out _));
            cursor.Emit(OpCodes.Ldloc_1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CheckPerkfectItemDrop"));

        }
    }

    /// <summary>
    /// Replace the check for special item drop
    /// </summary>
    public class PatchCheckSpecialItemDrop : PatchBaseEntityControlDeath
    {
        public PatchCheckSpecialItemDrop()
        {
            priority = 5327;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdfld(AccessTools.Field(typeof(MainManager), "lastdefeated")));
            cursor.Prev.OpCode = OpCodes.Nop;
            cursor.Emit(OpCodes.Ldloc_1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchCheckSpecialItemDrop), "CheckSpecialEnemyItemDrop"));
            Utils.RemoveUntilInst(cursor, i => i.MatchLdsfld(out _), i => i.MatchNewobj(out _));
        }

        static void CheckSpecialEnemyItemDrop(EntityControl instance)
        {
            var recipepool = (int[][])typeof(EntityControl).GetField("recipepool", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            var specialEnemies = (int[][])typeof(EntityControl).GetField("specialenemy", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            for (int i = MainManager.instance.lastdefeated.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < specialEnemies[1].Length; j++)
                {
                    if (MainManager.instance.lastdefeated[i] == specialEnemies[1][j])
                    {
                        if (UnityEngine.Random.Range(0, 100) < specialEnemies[0][j])
                        {
                            Vector3 vector2 = MainManager.RandomItemBounce(4f, 10f);
                            NPCControl npccontrol2 = EntityControl.CreateItem(instance.spritetransform.position + Vector3.up * 0.5f, 0, recipepool[j][UnityEngine.Random.Range(0, recipepool[j].Length)], vector2, 600);
                            npccontrol2.entity.TempIgnoreColision(instance.ccol, 5f);
                            npccontrol2.entity.LateVelocity(vector2);
                        }
                        MainManager.instance.lastdefeated.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }

    public class PatchTermiteKnightDeathSound : PatchBaseEntityControlDeath
    {
        public PatchTermiteKnightDeathSound()
        {
            priority = 4559;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdstr("Death2"));
            cursor.Prev.OpCode = OpCodes.Nop;
            cursor.Emit(OpCodes.Ldloc_1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchTermiteKnightDeathSound), "GetDeathSound"));

            cursor.GotoNext(MoveType.After, i => i.MatchLdstr("Death2"));
            cursor.Prev.OpCode = OpCodes.Nop;
            cursor.Emit(OpCodes.Ldloc_1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchTermiteKnightDeathSound), "GetDeathSound"));
        }

        static string GetDeathSound(EntityControl entity)
        {
            if (entity.animid == (int)NewAnimID.TermiteKnight)
            {
                MainManager.FadeMusic(0.1f);
                return "MiteKnight";
            }
            return "Death2";
        }
    }
}
