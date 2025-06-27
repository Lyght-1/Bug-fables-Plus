using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace BFPlus.Extensions
{
    public enum MedalType
    {
        Overworld,
        Quest,
        MedalShop,
        ShadesShop,
        HMReward,
        Bounty
    }

    public class Badge
    {
        public MedalType type;
        public MainManager.BadgeTypes id;
        public int flag = -1;
        public MainManager.Maps map;
        public int eventID = -1;
        public int index;
        public int prizeIndex = -1;
    }

    public class NewMystery : MonoBehaviour
    {
        int seed;
        List<Badge> medals = new List<Badge>()
        {
            //HP PLUS
            new Badge() {type=MedalType.Overworld, flag=23,id=MainManager.BadgeTypes.HPPlus, map = MainManager.Maps.SnakemouthLake},//snakemouth den hp plus
            new Badge() {type=MedalType.Overworld, flag=137,id=MainManager.BadgeTypes.HPPlus, map = MainManager.Maps.BugariaOutskirtsEast1},//waterfall hp plus
            new Badge() {type=MedalType.Overworld, flag=413,id=MainManager.BadgeTypes.HPPlus, map = MainManager.Maps.DesertBadlands},//outside bandit hiedout hp plus
            new Badge() {type=MedalType.MedalShop,id=MainManager.BadgeTypes.HPPlus, map = MainManager.Maps.BugariaCommercial, eventID = 99},//hp plus - medal shop start
            new Badge() {type=MedalType.ShadesShop,id=MainManager.BadgeTypes.HPPlus, map = MainManager.Maps.UndergroundBar, eventID = 99},//hp plus - shades shop ch3 boss defeated     
            new Badge() {type=MedalType.HMReward,id=MainManager.BadgeTypes.HPPlus, prizeIndex = 16, eventID=175},//hp plus - artis beating fake explorer HM

            //TP PLUS
            new Badge() {type=MedalType.MedalShop,id=MainManager.BadgeTypes.TPPlus, map = MainManager.Maps.BugariaCommercial},//medal shop start
            new Badge() {type=MedalType.MedalShop,id=MainManager.BadgeTypes.TPPlus, map = MainManager.Maps.BugariaCommercial, eventID=120}, //ch5 starts
            new Badge() {type=MedalType.HMReward, flag=137,id=MainManager.BadgeTypes.TPPlus, prizeIndex=4}, //hm reward mothiva #2
            new Badge() {type=MedalType.Quest, eventID=209,id=MainManager.BadgeTypes.TPPlus}, //they took her! quest
            new Badge() {type=MedalType.Quest,id=MainManager.BadgeTypes.TPPlus, map = MainManager.Maps.UndergroundBar, eventID = 99},

            new Badge() {type=MedalType.MedalShop,id= MainManager.BadgeTypes.FreezeResistance, map = MainManager.Maps.BugariaCommercial, eventID = 99},//Freeze Res, ch3 boss defeated
            new Badge() {type=MedalType.MedalShop,id=MainManager.BadgeTypes.Meditation, map = MainManager.Maps.BugariaCommercial, eventID = 99},//Meditation, ch3 boss defeated
            new Badge() {type=MedalType.MedalShop,id=MainManager.BadgeTypes.HealPlus, map = MainManager.Maps.BugariaCommercial, eventID = 99},//Heal Plus, ch3 boss defeated
            new Badge() {type=MedalType.ShadesShop,id=MainManager.BadgeTypes.PowerExchange, map = MainManager.Maps.UndergroundBar, eventID = 99} //Power Exchange, ch3 boss defeated
        };

        List<int>[] badgeShopsCopy = new List<int>[3];

        public void GetRandomMedal(Badge originalBadge)
        {

        }

        void Start()
        {
            MainManager.instance.badgeshops.CopyTo(badgeShopsCopy,0);

            for(int i=0;i!= medals.Count; i++)
            {
                medals[i].index = i;
            }
        }

        void RandomizeMedals()
        {
            List<int> usedIndex = Enumerable.Range(0, medals.Count).ToList();

            for (int i = 0; i != medals.Count; i++)
            {
                medals[i].index = usedIndex[UnityEngine.Random.Range(0, usedIndex.Count)];
                usedIndex.RemoveAt(medals[i].index);
            }
        }

        void Update()
        {
            if (MainManager.instance.inevent)
            {
                for(int i=0;i!=badgeShopsCopy.Length;i++)
                {
                    if (badgeShopsCopy[i].Count != MainManager.instance.badgeshops[i].Count) 
                    { 
                       var newMedals = MainManager.instance.badgeshops[i].Except(badgeShopsCopy[i]);
                    }
                }
                MainManager.instance.badgeshops.CopyTo(badgeShopsCopy, 0);
            }
        }
    }
}
