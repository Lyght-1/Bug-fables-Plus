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
        SleepOrAttack,
        Sleep,
        NumbOrAttack
    }
    public class CardGame_Ext
    {
        static int[] atkBuff = new int[2];
        static int[] defBuff = new int[2];
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
                        cardGame.attacknextturn[playedId] += card.effects[effectIndex, 1];
                    break;

                case NewCardEffect.DmgOrAttackNextTurn:
                    heads = UnityEngine.Random.Range(0, 2) == 0;
                    yield return cardGame.CoinEffect(hand[cardIndex].cardobj.transform.position, heads, true);
                    if (heads)
                        atkBuff[playedId] += card.effects[effectIndex, 1];
                    else
                        cardGame.attacknextturn[playedId] += card.effects[effectIndex, 2];
                    break;

                case NewCardEffect.NumbOrAttack:
                    heads = UnityEngine.Random.Range(0, 2) == 0;
                    yield return cardGame.CoinEffect(hand[cardIndex].cardobj.transform.position, heads, true);
                    if (heads)
                        atkBuff[playedId] += card.effects[effectIndex, 1];
                    else
                        yield return DoNumb(playedId == 0 ? 1 : 0, cardGame, hand[cardIndex]);
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

        static IEnumerator DoNumb(int handId, CardGame cardGame, CardGame.Cards owner)
        {
            for (int i = 0; i < cardGame.playedcards[handId].Count; i++)
            {
                CardGame.Cards card = cardGame.playedcards[handId][i];
                if (cardGame.carddata[card.cardid].type == CardGame.Type.Attacker && !card.flipped)
                {
                    MainManager.PlaySound("Lazer");
                    card.flipped = true;
                    atkBuff[handId] -= cardGame.carddata[card.cardid].attack;
                    cardGame.playedcards[handId].Insert(i, card);
                    cardGame.playedcards[handId].RemoveAt(i + 1);
                    yield return cardGame.Shine(owner);
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
                            case NewCardEffect.SleepOrAttack:
                                heads = UnityEngine.Random.Range(0, 2) == 0;
                                yield return cardGame.CoinEffect(hand[i].cardobj.transform.position, heads, true);
                                if (heads)
                                    atkBuff[playedId] += card.effects[j, 1];
                                else
                                    yield return DoSleep(playedId == 0 ? 1 : 0, cardGame, hand[i]);
                                break;

                            case NewCardEffect.Sleep:
                                yield return DoSleep(playedId == 0 ? 1 : 0, cardGame, hand[i]);
                                break;
                        }
                    }
                }
            }
        }

        static void SetBuffs(ref int[] atk, ref int[] def, int playedId)
        {
            playedId = playedId == 1 ? 0 : 1;
            for (int i = 0; i < atk.Length; i++) 
            {
                atk[i] = Mathf.Clamp(atk[i] + atkBuff[i], 0, 99);
                def[i] = Mathf.Clamp(def[i] + defBuff[i], 0, 99);
                atkBuff[i] = 0;
                defBuff[i] = 0;
            }
        }

        static bool CardIsFlipped(CardGame.Cards[] hand ,int index) => hand[index].flipped;
    }
}
