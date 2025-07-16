using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using BFPlus.Patches.MainManagerTranspilers;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static BFPlus.Extensions.CardGame_Ext;
using static FlappyBee;

namespace BFPlus.Patches.CardGameTranspilers
{
    public class PatchGetHighestCost : PatchBaseCardGamePullCard
    {
        public PatchGetHighestCost()
        {
            priority = 8019;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i=>i.MatchStfld(AccessTools.Field(typeof(CardGame), "maxoptions")));

            cursor.Emit(OpCodes.Ldloc_1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchGetHighestCost), "GetHighestCostCard"));
        }

        static void GetHighestCostCard(CardGame cardGame)
        {
            if(CardGame_Ext.Instance.inkBuff != 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    int[] tpCosts = new int[cardGame.handcards[i].Count];

                    for(int j =0;j < cardGame.handcards[i].Count; j++)
                    {
                        var data_Ext = cardGame.handcards[i][j].cardobj.GetComponent<CardData_Ext>();
                        if (data_Ext == null)
                            tpCosts[j] = cardGame.carddata[cardGame.handcards[i][j].cardid].tp;
                        else
                            tpCosts[j] = data_Ext.tpCost;
                    }

                    int highestIndex = tpCosts.Select((c, j) => new { Cost = c, Index = j })
                    .OrderByDescending(x => x.Cost)
                    .FirstOrDefault().Index;

                    CardGame_Ext.Instance.inkedCards[i] = highestIndex;

                    var card = cardGame.handcards[i][highestIndex];
                    var data_ext = CardData_Ext.GetCardData_Ext(card.cardobj.gameObject, card.cardid, i, cardGame.carddata[card.cardid].tp);
                    data_ext.ApplyBuff();

                    SpriteRenderer tpCostSprite = data_ext.transform.Find("tp").GetChild(0).GetComponent<SpriteRenderer>();
                    tpCostSprite.sprite = GetGuiSprite(data_ext.tpCost);
                    tpCostSprite.material.color = Color.green;
                }
            }

            CardGame_Ext.Instance.inkBuff = 0;
        }
    }

    public class PatchTpRegen : PatchBaseCardGamePullCard
    {
        public PatchTpRegen()
        {
            priority = 7759;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchStfld(AccessTools.Field(typeof(CardGame), "tp")));

            cursor.Emit(OpCodes.Ldloc_1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchTpRegen), "GetTpRegen"));
        }

        static void GetTpRegen(CardGame cardGame)
        {
            for(int i = 0; i < cardGame.tp.Length; i++)
            {
                cardGame.tp[i] += CardGame_Ext.Instance.tpRegen[i];
                CardGame_Ext.Instance.tpRegen[i] = 0;
            }
        }
    }
}
