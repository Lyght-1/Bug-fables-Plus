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
    public class DeepCaveVineDoorEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            Transform vineDoor = MainManager.map.mainmesh.Find("VinesDoor");
            MainManager.SetCamera(null, vineDoor.transform.position + new Vector3(8,4,-1), 0.035f);
            yield return EventControl.sec;

            MainManager.PlaySound("Rumble");
            MainManager.ShakeScreen(Vector3.one * 0.1f, -1f);
            yield return EventControl.sec;
            foreach (Transform vine in vineDoor)
            {
                vine.gameObject.isStatic = false;
                instance.StartCoroutine(MainManager.MoveTowards(vine, new Vector3(vine.localPosition.x, -10, vine.localPosition.z), 40f, true, true));
            }
            yield return EventControl.sec;
            MainManager.StopSound("Rumble");
            UnityEngine.Object.Destroy(vineDoor.gameObject);
            yield return EventControl.quartersec;
            MainManager.screenshake = Vector3.zero;
            MainManager.ResetCamera();

            int flag = 861;
            if ((int)MainManager.map.mapid == (int)NewMaps.DeepCave2)
                flag = 863;
            MainManager.instance.flags[flag] = true;
            yield return null;
        }
    }
}
