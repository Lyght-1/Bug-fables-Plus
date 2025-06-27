using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events.NewDungeonsEvents
{
    public class AncientTowerWindSwitchEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            List<EntityControl> downPipes = new List<EntityControl>();
            foreach(var entity in MainManager.map.entities)
            {
                if(entity.name.Contains("windDown"))
                    downPipes.Add(entity);
            }

            MainManager.SetCamera(null, downPipes[0].transform.position + new Vector3(0, -3, -6), 0.02f);
            yield return EventControl.sec;
            MainManager.PlaySound("Rumble");
            MainManager.ShakeScreen(Vector3.one * 0.1f, -1f);
            yield return EventControl.sec;
            foreach (EntityControl entity in downPipes)
            {
                entity.npcdata.internalparticle[0].Stop();
            }

            yield return EventControl.sec;
            foreach (EntityControl entity in downPipes)
            {
                UnityEngine.Object.Destroy(entity.gameObject);
            }
            MainManager.StopSound("Rumble");
            yield return EventControl.quartersec;
            MainManager.screenshake = Vector3.zero;
            MainManager.ResetCamera();
            MainManager.instance.flags[866] = true;
            yield return null;
        }
    }
}
