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
using UnityEngine;

namespace BFPlus.Patches.CardGameTranspilers
{
    public class PatchGuiSprite : PatchBaseCardGameCreateCard
    {
        public PatchGuiSprite()
        {
            priority = 151;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdsfld(out _), i => i.MatchLdcI4(48));

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(CardGame_Ext), "GetGuiSprite", new Type[] {typeof(CardGame), typeof(int)}));
            Utils.RemoveUntilInst(cursor, i => i.MatchCallvirt(out _));

            cursor.GotoNext(i => i.MatchLdsfld(out _), i => i.MatchLdcI4(48));

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchGuiSprite), "GetAttackSprite"));
            Utils.RemoveUntilInst(cursor, i => i.MatchCallvirt(out _));
        }

        static Sprite GetAttackSprite(CardGame cardGame, int id)
        {
            return CardGame_Ext.GetGuiSprite(cardGame.carddata[id].attack);
        }
    }
}
