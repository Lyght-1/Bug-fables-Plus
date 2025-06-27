using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MainManager;

namespace BFPlus.Extensions
{
    public class ItemTree : MonoBehaviour
    {
        bool spitItem = false;
        public int flag;
        public Vector3 itemPos;
        Vector3 bouncepos;
        public int itemId;
        public int itemType = 2;
        GameObject item;


        void Start()
        {
            if (!MainManager.instance.flags[flag])
            {
                item = new GameObject("itemTree");

                Sprite itemSprite;

                if(itemType == 2)
                {
                    itemSprite = !MainManager.instance.flags[681] ? MainManager.itemsprites[1, itemId] : MainManager.guisprites[190];
                }
                else
                {
                    itemSprite = MainManager.itemsprites[0, itemId];
                }

                item.AddComponent<SpriteRenderer>().sprite = itemSprite;
                item.transform.position = itemPos;
                item.transform.parent = transform;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("BeetleHorn"))
            {
                MainManager.HitPart(other.transform.position + Vector3.up);
                MainManager.PlaySound("Damage0", -1, 0.7f, 1f);
                
                if (!spitItem && !MainManager.instance.flags[flag])
                {
                    Destroy(item);
                    spitItem = true;
                    MainManager.PlaySound("Fall");
                    NPCControl npccontrol = EntityControl.CreateItem(itemPos, itemType, itemId, bouncepos, -1);
                    npccontrol.activationflag = flag;
                }
            }
        }

        public static void CreateItemTree(Transform parent, int itemId, int itemType, Vector3 itemPos, Vector3 bouncePos, int flag)
        {
            ItemTree comp = parent.gameObject.AddComponent<ItemTree>();
            comp.itemId = itemId;
            comp.itemType = itemType;
            comp.itemPos = itemPos;
            comp.bouncepos = bouncePos;
            comp.flag = flag;   
        }
    }


}
