using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions
{
    public class DartTarget : MonoBehaviour
    {
        bool itemspitout;
        int flag = 57;
        Vector3 itempos = new Vector3(4.5f, 2f, -9.4f);
        Vector3 bouncepos = new Vector3(10f,10f,0f);


        void Start()
        {
            if (!MainManager.instance.crystalbflags[flag] && MainManager.BadgeIsEquipped(2))
            {
                MainManager.map.hiddenitem = new int?(100);
            }
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("BeeRang"))
            {
                MainManager.HitPart(col.transform.position + Vector3.up);
                MainManager.PlaySound("Damage0", -1, 0.7f, 1f);
                if (!itemspitout && !MainManager.instance.crystalbflags[flag])
                {
                    MainManager.DeathSmoke(itempos);
                    itemspitout = true;
                    MainManager.PlaySound("PingDown");
                    NPCControl npccontrol = EntityControl.CreateItem(itempos, 3, flag, bouncepos, -1);
                    npccontrol.insideid = MainManager.instance.insideid;
                }
            }
        }
    }
}
