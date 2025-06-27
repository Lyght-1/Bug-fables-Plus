using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static CardGame;

namespace BFPlus.Extensions
{
    public enum NewCardEffect
    {
        AttackNextTurnOnOtherCard=43,
        DmgOrAttackNextTurn,
        SleepOrNumb,
        Sleep
    }
    public class CardGame_Ext
    {
        static int atkBuff = 0;
        static int defBuff = 0;
        public static IEnumerator DoPostNewEffect(CardGame.CardData card, int playedId, int effectIndex, int cardIndex)
        {
            CardGame cardGame = MainManager.instance.cardgame;
            //idk why its wrong
            playedId = playedId == 1 ? 0 : 1;

            CardGame.Cards[] hand = cardGame.playedcards[playedId].ToArray();
            NewCardEffect effect = (NewCardEffect)card.effects[effectIndex,0];
            bool heads;
            switch (effect) 
            { 
                case NewCardEffect.AttackNextTurnOnOtherCard:
                    if (cardGame.GetCardQuantityID(card.effects[effectIndex, 2], playedId) > 0)
                    {
                        cardGame.attacknextturn[playedId] += card.effects[effectIndex, 1];
                    }
                    break;

                case NewCardEffect.DmgOrAttackNextTurn:
                    heads = UnityEngine.Random.Range(0, 2) == 0;
                    yield return cardGame.CoinEffect(hand[cardIndex].cardobj.transform.position, heads, true);
                    if (heads)
                    {
                        atkBuff += card.effects[effectIndex, 1];
                    }
                    else
                    {
                        cardGame.attacknextturn[playedId] += card.effects[effectIndex, 2];
                    }
                    break;
            }

            yield return null;
        }

        static IEnumerator DoSleep(int handId, CardGame cardGame, CardGame.Cards owner)
        {
            for (int i = cardGame.playedcards[handId].Count-1; i >=0; i--)
            {
                CardGame.Cards card = cardGame.playedcards[handId][i];
                if (cardGame.carddata[card.cardid].type == CardGame.Type.Effect && !card.flipped)
                {
                    MainManager.PlaySound("Sleep");
                    card.flipped = true;
                    cardGame.playedcards[handId][i] = card;
                    yield return cardGame.Shine(owner);
                    yield return EventControl.halfsec;
                    yield break;
                }
            }
        }

        public static IEnumerator DoPreCardLoadEffects(int playedId)
        {
            CardGame cardGame = MainManager.instance.cardgame;
            CardGame.Cards[] hand = cardGame.playedcards[playedId].ToArray();

            for(int i=0;i<hand.Length; i++)
            {
                CardGame.CardData card = cardGame.carddata[hand[i].cardid];
                if (card.type != CardGame.Type.Attacker)
                {
                    for(int j=0;j<card.effects.GetLength(0);j++)
                    {
                        NewCardEffect effect = (NewCardEffect)card.effects[j, 0];
                        bool heads;
                        switch (effect)
                        {
                            case NewCardEffect.SleepOrNumb:
                                heads = UnityEngine.Random.Range(0, 2) == 0;
                                yield return cardGame.CoinEffect(hand[i].cardobj.transform.position, heads, true);
                                if (heads)
                                {
                                    atkBuff += card.effects[j, 1];
                                }
                                else
                                {
                                    yield return DoSleep(playedId == 0 ? 1 : 0, cardGame, hand[i]);
                                }
                                break;

                            case NewCardEffect.Sleep:
                                yield return DoSleep(playedId == 0 ? 1 : 0, cardGame, hand[i]);
                                break;
                        }
                    }
                }
            }

            yield return null;
        }

        static void SetBuffs(ref int[] atk, ref int[] def, int playedId)
        {
            //idk why its wrong
            playedId = playedId == 1 ? 0 : 1;

            atk[playedId] += atkBuff;
            def[playedId] += defBuff;
            atkBuff = 0;
            defBuff = 0;
        }

        static bool CardIsFlipped(CardGame.Cards[] hand ,int index) => hand[index].flipped;
    }
}
