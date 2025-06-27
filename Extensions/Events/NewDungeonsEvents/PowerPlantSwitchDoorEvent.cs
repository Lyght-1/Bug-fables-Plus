using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events.NewDungeonsEvents
{
    public class PowerPlantSwitchDoorEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            Transform door = MainManager.map.mainmesh.Find("DoorE");

            MainManager.SetCamera(door, null, 0.035f);
            yield return EventControl.sec;

            yield return instance.SlideDoorOpen(door, true, 1f, true);
            yield return EventControl.halfsec;
            MainManager.ResetCamera();
            MainManager.instance.flags[845] = true;
            yield return null;
        }
    }
}
