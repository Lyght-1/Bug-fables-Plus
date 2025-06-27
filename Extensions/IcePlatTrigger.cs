using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions
{
    public class IcePlatTrigger : MonoBehaviour
    {
        void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("PushRock"))
            {
                var npc = col.GetComponentInParent<NPCControl>();
                if (npc != null && npc.objecttype == NPCControl.ObjectTypes.PushRock && (npc.data.Length > 2))
                {
                    this.transform.parent = npc.transform;
                }
            }
        }

        void OnTriggerExit(Collider col)
        {
            if (col.CompareTag("PushRock"))
            {
                var npc = col.GetComponentInParent<NPCControl>();
                if (npc != null && npc.objecttype == NPCControl.ObjectTypes.PushRock && (npc.data.Length > 2))
                {
                    this.transform.parent = MainManager.map.transform;
                }
            }
        }
    }
}
