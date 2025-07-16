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
using static CardGame;

namespace BFPlus.Patches.CardGameTranspilers
{

    public class PatchCardTpCost : PatchBaseCardGameGetInput
    {
        public PatchCardTpCost()
        {
            priority = 10178;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchLdfld(AccessTools.Field(typeof(CardGame.CardData), "tp")));
            cursor.GotoPrev(i => i.MatchLdarg0());

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchCardTpCost), "GetTpCostHand"));  
            Utils.RemoveUntilInst(cursor, i => i.MatchBlt(out _));

            cursor.GotoNext(i => i.MatchLdstr("Confirm"));
            cursor.GotoNext(i => i.MatchSub());
            cursor.GotoPrev(i => i.MatchLdarg0());

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchCardTpCost), "GetTpCostHand"));
            Utils.RemoveUntilInst(cursor, i => i.MatchSub());

            cursor.GotoNext(i => i.MatchLdstr("PageFlip"));
            cursor.GotoNext(i => i.MatchAdd());
            cursor.GotoPrev(i => i.MatchLdarg0());

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchCardTpCost), "GetTpCostPlayer"));
            Utils.RemoveUntilInst(cursor, i => i.MatchAdd());
        }

        static int GetTpCostHand(CardGame cardGame)
        {
            return GetTpCost(cardGame, cardGame.handcards[0][cardGame.option]);
        }

        static int GetTpCostPlayer(CardGame cardGame)
        {
            return GetTpCost(cardGame, cardGame.playedcards[0][cardGame.playedcards[0].Count - 1]);
        }

        public static int GetTpCost(CardGame cardGame, CardGame.Cards card)
        {
            var data_Ext = card.cardobj.GetComponent<CardGame_Ext.CardData_Ext>();
            return data_Ext == null ? cardGame.carddata[card.cardid].tp : data_Ext.tpCost;
        }
    }
}
