using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions
{
    public class IronSuit : MonoBehaviour
    {
        public enum Suit
        {
            Spade,
            Club,
            Heart,
            Diamond
        }

        public Suit currentSuit = Suit.Spade;
        Sprite[][] sprites = new Sprite[4][];
        Sprite lastSprite;
        EntityControl entity;
        List<Suit> forms = new List<Suit>();
        void Start()
        {
            entity = GetComponent<EntityControl>();
            string[] names = Enum.GetNames(typeof(Suit));

            for (int i = 0; i < names.Length; i++)
            {
                sprites[i] = MainManager_Ext.assetBundle.LoadAssetWithSubAssets<Sprite>("IronSuit_" + names[i]);
            }
            forms = Enum.GetValues(typeof(Suit)).Cast<Suit>().ToList();
            forms.Remove(currentSuit);
        }

        void LateUpdate()
        {
            if (entity.sprite != null && entity.sprite.sprite != null) 
            {
                if (lastSprite != entity.sprite.sprite)
                {
                    entity.sprite.sprite = sprites[(int)currentSuit].FirstOrDefault(s => s.name == entity.sprite.sprite.name);
                    lastSprite = entity.sprite.sprite;
                }


            }
        }

        

        public Suit GetNewSuit()
        {
            Suit newSuit;
            if (forms.Contains(currentSuit))
            {
                var temp = new List<Suit>(forms);
                temp.Remove(currentSuit);
                newSuit = temp[UnityEngine.Random.Range(0, temp.Count)];
            }
            else
            {
                newSuit = forms[UnityEngine.Random.Range(0, forms.Count)];
            }

            forms.Remove(newSuit);
            if (forms.Count == 0)
                forms = Enum.GetValues(typeof(Suit)).Cast<Suit>().ToList();

            return newSuit;
        }
    }
}
