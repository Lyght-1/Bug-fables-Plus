using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events.NewDungeonsEvents.GiantLairPlayroomEvents
{
    public class ToyTruckEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            yield return null;
            Transform crane = MainManager.map.mainmesh.Find("ToyCrane").GetChild(0);
            MainManager.SetCamera(crane.GetChild(0).GetChild(3).transform.position + new Vector3(0,2,-3), 0.02f);
            yield return EventControl.sec;
            yield return EventControl.sec;
            MainManager.PlaySound("OmegaMove",1.2f, 1);
            Vector3 startRot = crane.transform.localEulerAngles;
            Vector3 targetRot =new Vector3(startRot.x, 385, startRot.z);
            float a = 0;
            float b = 120;
            do
            {
                crane.transform.localEulerAngles = Vector3.Lerp(startRot, targetRot,a/b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a<b);

            if((int)MainManager.map.mapid == (int)NewMaps.GiantLairPlayroom1)
                MainManager.instance.flags[918] = true;
            else if ((int)MainManager.map.mapid == (int)NewMaps.GiantLairPlayroom2)
                MainManager.instance.flags[921] = true;
            else if ((int)MainManager.map.mapid == (int)NewMaps.GiantLairPlayroom3)
                MainManager.instance.flags[926] = true;

            yield return EventControl.halfsec;

            MainManager.ResetCamera();
            yield return EventControl.sec;
        }
    }
}
