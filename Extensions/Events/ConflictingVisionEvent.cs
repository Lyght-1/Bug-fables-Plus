using BFPlus.Extensions.EnemyAI;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events
{
    public class ConflictingVisionEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            while (MainManager.instance.message)
                yield return null;
            MainManager.SetCamera(new Vector3(-1,0,0),0.02f);

            EntityControl[] party = MainManager.GetPartyEntities(true);
            EntityControl artia = MainManager.GetEntity(5);
            EntityControl jaune = MainManager.GetEntity(1);

            Vector3[] posArray = new Vector3[]
            {
                new Vector3(2, 0, 0),
                new Vector3(3.5f, 0,0),
                new Vector3(5, 0, 0),
                new Vector3(6f, 0,0)
            };

            for(int i = 0; i < party.Length; i++)
            {
                party[i].transform.position = posArray[i];
                party[i].FaceTowards(artia.transform.position);
            }

            if(MainManager.map.chompy != null)
            {
                MainManager.map.chompy.transform.position = posArray[3];
                MainManager.map.chompy.FaceTowards(artia.transform.position);
            }

            yield return EventControl.sec;

            artia.FaceTowards(jaune.transform.position);
            MainManager.DialogueText(MainManager.map.dialogues[42], artia.transform, artia.npcdata);
            while (MainManager.instance.message)
                yield return null;

            MainManager.FadeIn();
            yield return EventControl.sec;
            MainManager.PlaySound("WaterSplash2", 1.2f, 1);
            yield return EventControl.halfsec;
            MainManager.PlaySound("WaterSplash", 1.1f, 1);
            yield return EventControl.halfsec;
            MainManager.PlaySound("WaterSplash2", 0.9f, 1);

            yield return EventControl.sec;
            Transform painting = MainManager.map.mainmesh.Find("CatalystPainting");
            painting.transform.position = new Vector3(-1.4f,2.3f,3.2f);
            yield return EventControl.sec;

            MainManager.FadeOut();
            yield return EventControl.sec;

            MainManager.SetCamera(painting.transform.position, 0.02f);
            yield return EventControl.sec;
            yield return EventControl.sec;

            MainManager.SetCamera(new Vector3(-1, 0, 0), 0.02f);
            yield return EventControl.quartersec;
            MainManager.DialogueText(MainManager.map.dialogues[43], artia.transform, artia.npcdata);

            while (MainManager.instance.message)
                yield return null;

            MainManager.FadeIn();
            yield return EventControl.sec;

            MainManager.LoadMap();
            MainManager.ResetCamera(true);
            yield return EventControl.halfsec;

            MainManager.FadeOut();
            yield return EventControl.sec;

            MainManager.ChangeMusic("Bee");
            MainManager.CompleteQuest((int)NewQuest.ConflictingVisions);
            yield return null;
        }

    }
}
