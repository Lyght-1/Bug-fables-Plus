using BFPlus.Patches.DoActionPatches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
        NumbOrAttack,
        Ink,
        InkTribeAmount,
        Freeze,
        Poison,
        Sticky,
        Burn,
        BurnOrPoison,
        FreezeOrLifeSteal,
        StickyOrPierce,
        SleepOrNumb,
        ExactHPAttack,
        ExactAtkDefAttackNextTurn,
        TPRegen,
        AttackNextTurnPerDef,
        HealPerAttack,
        MultiplyCard,
        ExodiaPerCard
    }
    public class CardGame_Ext : MonoBehaviour
    {
        int[] atkBuff = new int[2];
        int[] defBuff = new int[2];
        public int inkBuff = 0;
        public int[] inkedCards = new int[2];
        public int[] freezes = new int[2];
        public int[] tpRegen = new int[2];
        static CardGame_Ext instance = null;
        public static CardGame_Ext Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.instance.gameObject.AddComponent<CardGame_Ext>();
                }
                return instance;
            }
        }
        public static IEnumerator DoPostNewEffect(CardGame.CardData card, int playedId, int effectIndex, int cardIndex, int[] atks, int[] defs)
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
                        Instance.atkBuff[playedId] += card.effects[effectIndex, 1];
                    else
                        cardGame.attacknextturn[playedId] += card.effects[effectIndex, 2];
                    break;

                case NewCardEffect.NumbOrAttack:
                    heads = UnityEngine.Random.Range(0, 2) == 0;
                    yield return cardGame.CoinEffect(hand[cardIndex].cardobj.transform.position, heads, true);
                    if (heads)
                        Instance.atkBuff[playedId] += card.effects[effectIndex, 1];
                    else
                        yield return Instance.DoNumb(playedId == 0 ? 1 : 0, cardGame, hand[cardIndex]);
                    break;

                case NewCardEffect.Ink:
                    yield return Instance.DoInk(cardGame, hand[cardIndex], card.effects[effectIndex,1]);
                    break;

                case NewCardEffect.InkTribeAmount:
                    if (cardGame.GetCardQuantityTribe((CardGame.Tribe)card.effects[effectIndex, 2], playedId) >= card.effects[effectIndex, 1])
                    {
                        yield return Instance.DoInk(cardGame, hand[cardIndex], 2);
                    }
                    break;

                case NewCardEffect.Freeze:
                    yield return Instance.DoFreeze(cardGame, hand[cardIndex], card.effects[effectIndex, 1], playedId);
                    break;

                case NewCardEffect.Poison:
                    yield return Instance.DoPoison(cardGame, hand[cardIndex], playedId == 0 ? 1 : 0);
                    break;

                case NewCardEffect.Sticky:
                    yield return Instance.DoSticky(cardGame, hand[cardIndex], playedId);
                    break;

                case NewCardEffect.Burn:
                    yield return Instance.DoBurn(cardGame, hand[cardIndex], playedId == 0 ? 1 : 0);
                    break;

                case NewCardEffect.BurnOrPoison:
                    heads = UnityEngine.Random.Range(0, 2) == 0;
                    yield return cardGame.CoinEffect(hand[cardIndex].cardobj.transform.position, heads, true);
                    if (heads)
                        yield return Instance.DoPoison(cardGame, hand[cardIndex], playedId == 0 ? 1 : 0);
                    else
                        yield return Instance.DoBurn(cardGame, hand[cardIndex], playedId == 0 ? 1 : 0);
                    break;

                case NewCardEffect.FreezeOrLifeSteal:
                    heads = UnityEngine.Random.Range(0, 2) == 0;
                    yield return cardGame.CoinEffect(hand[cardIndex].cardobj.transform.position, heads, true);
                    if (heads)
                        yield return Instance.DoFreeze(cardGame, hand[cardIndex], card.effects[effectIndex, 1], playedId);
                    else
                        cardGame.carddata[hand[cardIndex].cardid].effects[effectIndex, 0] = (int)CardGame.Effects.HealIfWin;
                    break;

                case NewCardEffect.ExactHPAttack:
                    if (cardGame.hp[playedId] == card.effects[effectIndex, 1])
                    {
                        Instance.atkBuff[playedId] += card.effects[effectIndex, 2];
                        yield return cardGame.Shine(hand[cardIndex]);
                    }
                    break;

                case NewCardEffect.ExactAtkDefAttackNextTurn:
                    if (atks[playedId] == defs[playedId])
                    {
                        cardGame.attacknextturn[playedId] += card.effects[effectIndex, 1];
                        yield return cardGame.Shine(hand[cardIndex]);
                    }
                    break;

                case NewCardEffect.TPRegen:
                    yield return Instance.DoTpRegen(cardGame, hand[cardIndex], playedId, card.effects[effectIndex, 1]);
                    break;

                case NewCardEffect.AttackNextTurnPerDef:
                    int timesDef = Mathf.FloorToInt(defs[playedId] / card.effects[effectIndex, 1]);
                    if (timesDef >0)
                    {
                        cardGame.attacknextturn[playedId] += card.effects[effectIndex, 2] * timesDef;
                        yield return cardGame.Shine(hand[cardIndex]);
                    }

                    break;

                case NewCardEffect.HealPerAttack:
                    int timesAtk = Mathf.FloorToInt(atks[playedId] / card.effects[effectIndex, 2]);
                    if (timesAtk >0)
                    {
                        if (effectIndex + 1< card.effects.GetLength(0))
                        {
                            card.effects[effectIndex + 1, 1] *= timesAtk;
                        }
                        yield return cardGame.Shine(hand[cardIndex]);
                    }
                    break;

                case NewCardEffect.MultiplyCard:
                    yield return Instance.DoMultiplyCard(cardGame, playedId, hand[cardIndex]);
                    break;

                case NewCardEffect.ExodiaPerCard:
                    yield return Instance.DoExodia(cardGame, playedId, card, effectIndex, hand[cardIndex]);
                    break;
            }

            yield return null;
        }

        IEnumerator DoMultiplyCard(CardGame cardGame, int playedId, Cards card)
        {
            int[] indexes = cardGame.cards[playedId].Select((c, j) => new { CardId = c, Index = j }).Where(c => c.CardId != card.cardid).Select(j => j.Index).ToArray();
            if(indexes.Length > 0)
            {
                cardGame.cards[playedId][indexes[UnityEngine.Random.Range(0, indexes.Length)]] = card.cardid;
                yield return cardGame.Shine(card);
            }
        }


        IEnumerator DoExodia(CardGame cardGame, int playedId, CardData cardData, int effectIndex, Cards card)
        {
            int amount = cardGame.playedcards[playedId].Count(c => c.cardid == card.cardid);
            if (amount >= cardData.effects[effectIndex, 1])
            {
                Instance.freezes[playedId] = 5;
                Instance.atkBuff[playedId] = 99;
                yield return cardGame.Shine(card);
            }
        }

        IEnumerator DoSleep(int handId, CardGame cardGame, CardGame.Cards owner)
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

        IEnumerator DoNumb(int handId, CardGame cardGame, CardGame.Cards owner)
        {
            for (int i = 0; i < cardGame.playedcards[handId].Count; i++)
            {
                CardGame.Cards card = cardGame.playedcards[handId][i];
                if (cardGame.carddata[card.cardid].type == CardGame.Type.Attacker && !card.flipped)
                {
                    MainManager.PlaySound("Lazer");
                    card.flipped = true;
                    Instance.atkBuff[handId] -= cardGame.carddata[card.cardid].attack;
                    cardGame.playedcards[handId].Insert(i, card);
                    cardGame.playedcards[handId].RemoveAt(i + 1);
                    yield return cardGame.Shine(owner);
                    yield break;
                }
            }
        }

        IEnumerator DoTpRegen(CardGame cardGame, CardGame.Cards owner, int playedId, int tpRegen)
        {
            MainManager.PlaySound("Heal2");
            Instance.tpRegen[playedId] += tpRegen;
            yield return cardGame.Shine(owner);
        }

        IEnumerator DoInk(CardGame cardGame, CardGame.Cards owner, int inkBuff)
        {
            MainManager.PlaySound("WaterSplash2");
            Instance.inkBuff += inkBuff;
            yield return cardGame.Shine(owner);
        }

        IEnumerator DoSticky(CardGame cardGame, CardGame.Cards owner, int playedId)
        {
            MainManager.PlaySound("AhoneynationSpit");
            int[] tpCosts = GetTpCosts(cardGame, playedId == 0 ? 1 : 0);
            if(tpCosts.Length > 0)
                Instance.defBuff[playedId] += tpCosts.Min();
            yield return cardGame.Shine(owner);
        }

        IEnumerator DoBurn(CardGame cardGame, CardGame.Cards owner, int opponentId)
        {
            MainManager.PlaySound("Flame");
            Instance.atkBuff[opponentId == 0 ? 1 : 0] += cardGame.playedcards[opponentId].Count;
            yield return cardGame.Shine(owner);
        }

        IEnumerator DoPoison(CardGame cardGame, CardGame.Cards owner, int opponentId)
        {
            MainManager.PlaySound("Poison");
            int[] tpCosts = GetTpCosts(cardGame, opponentId);
            if(tpCosts.Length > 0)
            {
                Instance.atkBuff[opponentId == 0 ? 1 : 0] += Mathf.Clamp(Mathf.CeilToInt(tpCosts.Max() / 2), 1, 99);
            }
            yield return cardGame.Shine(owner);
        }

        int[] GetTpCosts(CardGame cardGame, int playerId)
        {
            int[] tpCosts = new int[cardGame.playedcards[playerId].Count];
            for (int j = 0; j < cardGame.playedcards[playerId].Count; j++)
            {
                var data_Ext = cardGame.playedcards[playerId][j].cardobj.GetComponent<CardData_Ext>();
                if (data_Ext == null)
                    tpCosts[j] = cardGame.carddata[cardGame.playedcards[playerId][j].cardid].tp;
                else
                    tpCosts[j] = data_Ext.tpCost;
            }
            return tpCosts;
        }

        IEnumerator DoFreeze(CardGame cardGame, CardGame.Cards owner, int freezeBuff, int playedId)
        {
            MainManager.PlaySound("Freeze");
            Instance.freezes[playedId] += freezeBuff;
            yield return cardGame.Shine(owner);
        }

        static IEnumerator DoPreCardLoadEffects(int playedId)
        {
            Instance.freezes[playedId] = 0;
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
                                    Instance.atkBuff[playedId] += card.effects[j, 1];
                                else
                                    yield return Instance.DoSleep(playedId == 0 ? 1 : 0, cardGame, hand[i]);
                                break;

                            case NewCardEffect.Sleep:
                                yield return Instance.DoSleep(playedId == 0 ? 1 : 0, cardGame, hand[i]);
                                break;

                            case NewCardEffect.StickyOrPierce:
                                heads = UnityEngine.Random.Range(0, 2) == 0;
                                yield return cardGame.CoinEffect(hand[i].cardobj.transform.position, heads, true);
                                if (heads)
                                    yield return Instance.DoSticky(cardGame, hand[i], playedId);
                                else
                                    cardGame.carddata[hand[i].cardid].effects[j, 0] = (int)CardGame.Effects.IgnoreDefense;
                                break;

                            case NewCardEffect.SleepOrNumb:
                                heads = UnityEngine.Random.Range(0, 2) == 0;
                                yield return cardGame.CoinEffect(hand[i].cardobj.transform.position, heads, true);
                                if (heads)
                                    yield return Instance.DoSleep(playedId == 0 ? 1 : 0, cardGame, hand[i]);
                                else
                                    cardGame.carddata[hand[i].cardid].effects[j, 0] = (int)CardGame.Effects.NumbFront;
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
                atk[i] = Mathf.Clamp(atk[i] + Instance.atkBuff[i], 0, 99);
                def[i] = Mathf.Clamp(def[i] + Instance.defBuff[i], 0, 99);
                Instance.atkBuff[i] = 0;
                Instance.defBuff[i] = 0;
            }
        }

        public static Sprite GetGuiSprite(CardGame cardGame, int id)
        {
            return GetGuiSprite(cardGame.carddata[id].tp);
        }

        public static Sprite GetGuiSprite(int number)
        {
            if (number > 9)
            {
                return MainManager.guisprites[(int)NewGui.SpycardsTen + number % 10];
            }
            return MainManager.guisprites[48 + number];
        }

        static bool CardIsFlipped(CardGame.Cards[] hand ,int index) => hand[index].flipped;

        public class CardData_Ext : MonoBehaviour
        {
            public int tpCost;
            public int cardId;
            public int playerId;

            public static CardData_Ext GetCardData_Ext(GameObject cardObj, int cardId, int playedId, int tpCost)
            {
                if (cardObj.GetComponent<CardData_Ext>() == null)
                {
                    var data_Ext = cardObj.gameObject.AddComponent<CardData_Ext>();
                    data_Ext.cardId = cardId;
                    data_Ext.playerId = playedId;
                    data_Ext.tpCost = tpCost;
                }
                return cardObj.GetComponent<CardData_Ext>();
            }

            public void ApplyBuff()
            {
                tpCost = Mathf.Clamp(tpCost - Instance.inkBuff, 0, 99);
            }

            public static int GetTpCost(CardGame cardGame, int playedId, int cardId)
            {
                var data_Ext = cardGame.handcards[playedId][cardId].cardobj.GetComponent<CardData_Ext>();
                if (data_Ext == null)
                    return cardGame.carddata[cardGame.handcards[playedId][cardId].cardid].tp;
                else
                    return data_Ext.tpCost;
            }
        }
    }
}
