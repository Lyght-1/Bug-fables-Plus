using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions
{
    public class RockLimit : MonoBehaviour
    {
        void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("PushRock"))
            {
                var npc = col.GetComponentInParent<NPCControl>();
                if (npc != null && npc.objecttype == NPCControl.ObjectTypes.PushRock && (npc.data.Length == 2 || npc.data[2] == 0))
                {
                    MainManager.PlayParticle("IceShatter", null, npc.transform.position + Vector3.up, new Vector3(-90f, 0f), 1f);
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Audio/Sounds/IceBreak"), npc.transform.position, MainManager.GetSoundDistance(npc.transform.position) * MainManager.soundvolume);
                    base.StartCoroutine(MainManager.DelayedPosition(npc.transform, npc.GetComponent<EntityControl>().startpos.Value + Vector3.up * 5f, -1f, false));
                }
            }
        }
    }
}
