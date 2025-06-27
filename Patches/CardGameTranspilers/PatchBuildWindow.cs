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
    public class PatchPostNewEffect : PatchBaseCardGameBuildWindow
    {
        public PatchPostNewEffect()
        {
            priority = 4319;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            //New Card Effects / Post attackers effects
            cursor.GotoNext(i => i.MatchSwitch(out _), i=>i.MatchLdloc(out _), i=>i.MatchLdcI4(19));
            cursor.GotoPrev(i => i.MatchLdloc(out _));

            int index = cursor.Index;

            cursor.GotoPrev(i => i.MatchLdfld(out _));
            var effectIndex = cursor.Next.Operand;
            cursor.GotoPrev(i => i.MatchLdflda(out _));
            var cardRef = cursor.Next.Operand;


            ILLabel quitLabel = null;
            cursor.GotoPrev(MoveType.After,i => i.MatchBrfalse(out quitLabel));
            int flippedCheckIndex = cursor.Index;

            cursor.GotoPrev(i => i.MatchLdarg0(), i => i.MatchLdfld(out _));
            var indexRef = cursor.Instrs[cursor.Index + 1].Operand;

            cursor.GotoPrev(i => i.MatchLdarg0(), i => i.MatchLdfld(out _));
            var handRef = cursor.Instrs[cursor.Index + 1].Operand;

            cursor.Goto(flippedCheckIndex);

            //Prevent card to do their effect if flipped
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, handRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, indexRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(CardGame_Ext), "CardIsFlipped"));
            cursor.Emit(OpCodes.Brtrue, quitLabel);

            cursor.GotoPrev(i => i.MatchStfld(out _),i=>i.MatchLdarg0(),i=>i.MatchLdloc1());
            var playedRef = cursor.Next.Operand;

            cursor.GotoPrev(i => i.MatchLdcI4(48));
            cursor.GotoPrev(i => i.MatchLdelemRef());
            cursor.GotoNext(i => i.MatchLdfld(out _));
            var defRef = cursor.Next.Operand;

            cursor.GotoPrev(i => i.MatchLdcI4(48));
            cursor.GotoPrev(i => i.MatchLdelemRef());
            cursor.GotoNext(i => i.MatchLdfld(out _));
            var atkRef = cursor.Next.Operand;

            cursor.GotoNext(i => i.MatchSwitch(out _), i => i.MatchLdloc(out _), i => i.MatchLdcI4(19));
            cursor.GotoPrev(i => i.MatchLdloc(out _));

            ILLabel label = cursor.DefineLabel();

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, cardRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, effectIndex);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchPostNewEffect), "IsNewCardEffect"));
            cursor.Emit(OpCodes.Brfalse, label);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, cardRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, playedRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, effectIndex);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, indexRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(CardGame_Ext), "DoPostNewEffect"));
            Utils.InsertYieldReturn(cursor);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldflda, atkRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldflda, defRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, playedRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(CardGame_Ext), "SetBuffs"));
            cursor.MarkLabel(label);
        }

        static bool IsNewCardEffect(CardGame.CardData card, int effectIndex)
        {
            return Enum.IsDefined(typeof(NewCardEffect), card.effects[effectIndex, 0]);
        }
    }

    public class PatchPreCardLoad : PatchBaseCardGameBuildWindow
    {
        public PatchPreCardLoad()
        {
            priority = 3360;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            //Pre Card Loading effects (sleep)
            cursor.GotoNext(i => i.MatchLdfld(AccessTools.Field(typeof(CardGame), "attacknextturn")), i => i.MatchLdarg0());
            cursor.GotoPrev(i => i.MatchLdfld(out _));
            var indexRef = cursor.Next.Operand;
            cursor.GotoPrev(i => i.MatchLdarg0(), i => i.MatchLdloc1());

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, indexRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(CardGame_Ext), "DoPreCardLoadEffects"));
            Utils.InsertYieldReturn(cursor);
        }
    }

    /// <summary>
    /// Attacker calculation and some support effect, skip if card is flipped
    /// </summary>
    public class PatchSkipCardFlipped : PatchBaseCardGameBuildWindow
    {
        public PatchSkipCardFlipped()
        {
            priority = 3393;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            ILLabel label = null;
            cursor.GotoNext(i=>i.MatchLdarg0(), i=>i.MatchLdflda(out _),i=>i.MatchLdfld(out _),i => i.MatchBrtrue(out _));

            int index = cursor.Index;

            cursor.GotoPrev(i => i.MatchLdarg0(), i => i.MatchLdfld(out _));
            var indexRef = cursor.Instrs[cursor.Index + 1].Operand;

            cursor.GotoPrev(i => i.MatchLdarg0(), i => i.MatchLdfld(out _));
            var handRef = cursor.Instrs[cursor.Index + 1].Operand;

            cursor.GotoNext(i => i.MatchBr(out label));
            cursor.Goto(index);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, handRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, indexRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(CardGame_Ext), "CardIsFlipped"));
            cursor.Emit(OpCodes.Brtrue, label);
        }
    }

    public class PatchSkipCardFlippedSummon : PatchBaseCardGameBuildWindow
    {
        public PatchSkipCardFlippedSummon()
        {
            priority = 2730;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            ILLabel label = null;
            cursor.GotoNext(i => i.MatchLdcI4(39));
            cursor.GotoPrev(MoveType.After,i => i.MatchBrfalse(out label));
            int index = cursor.Index;

            cursor.GotoPrev(i => i.MatchLdarg0(), i => i.MatchLdfld(out _));
            var indexRef = cursor.Instrs[cursor.Index + 1].Operand;

            cursor.GotoPrev(i => i.MatchLdarg0(), i => i.MatchLdfld(out _));
            var handRef = cursor.Instrs[cursor.Index + 1].Operand;

            cursor.Goto(index);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, handRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, indexRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(CardGame_Ext), "CardIsFlipped"));
            cursor.Emit(OpCodes.Brtrue, label);
        }
    }
}
