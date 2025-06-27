using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events.NewDungeonsEvents.GiantLairPlayroomEvents
{
    public class PlayroomRaceStartEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            EntityControl[] party = MainManager.GetPartyEntities(true);

            Vector3[] posArray = new Vector3[]
            {
                new Vector3(-42f, -0.1f, 31f),
                new Vector3(-43f, -0.1f, 31f),
                new Vector3(-44f, -0.1f, 31f),
                new Vector3(-45f, -0.1f, 32f)
            };

            for (int i = 0; i < party.Length; i++) 
            {
                party[i].animstate = 0;
                party[i].transform.position = posArray[i];
            }

            if (MainManager.map.chompy != null)
                MainManager.map.chompy.transform.position = posArray[3];

            MainManager.PlaySound("Rumble");
            MainManager.ShakeScreen(Vector3.one * 0.1f, -1f);

            Transform entryGate = MainManager.map.mainmesh.Find("Maze").Find("PrisonGateLocal").GetChild(0);
            Transform raceGate = MainManager.map.mainmesh.Find("Maze").Find("PrisonGateLocal (2)").GetChild(0);
            Transform[] gates = { entryGate, raceGate };

            for (int i = 0; i < gates.Length; i++) 
            {
                Transform[] transforms = { gates[i].GetChild(3), gates[i].GetChild(4) };
                Vector3 offset = i ==0 ? Vector3.up : Vector3.down;
                foreach (Transform transform in transforms)
                {         
                    instance.StartCoroutine(BattleControl_Ext.LerpPosition(60f, transform.position, transform.position + offset * 4.5f, transform));
                }
            }
            OverworldTimer timer = OverworldTimer.SetUpTimer(70, (int)NewEvents.PlayroomRaceEnd, Vector3.down*1.5f, MainManager.map.transform);
            yield return EventControl.sec;
            MainManager.StopSound("Rumble");
            MainManager.screenshake = Vector3.zero;
            yield return null;
            timer.start = true;
            MainManager.map.cantcompass = true;
        }
    }
}
