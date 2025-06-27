using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace BFPlus.Extensions.Events
{
    public class HerFavoriteSweetEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            while (MainManager.instance.message)
                yield return null;

            EntityControl[] party = MainManager.GetPartyEntities(true);
            EntityControl[] entities = MainManager.GetEntities(new int[] {39,41,49 });


            MainManager.DialogueText(MainManager.map.dialogues[92], entities[0].transform, entities[0].npcdata);
            while (MainManager.instance.message)
                yield return null;
            MainManager.FadeIn();

            yield return EventControl.sec;

            MainManager.instance.insideid = -1;

            MainManager.LoadMap();
            yield return EventControl.quartersec;
            party = MainManager.GetPartyEntities(true);
            entities = MainManager.GetEntities(new int[] { 39, 41, 53 });

            for(int i = 0; i < entities.Length; i++)
            {
                entities[i].alwaysactive = true;
                entities[i].FacePlayer();
                entities[i].gameObject.SetActive(true);
                entities[i].iskill = false;
            }

            instance.SetPartyPos(new Vector3[]
            {
                new Vector3(-47f, 0, -7f),
                new Vector3(-45.5f, 0, -7f),
                new Vector3(-44f, 0, -7f),
                new Vector3(-43.5f, 0, -7f),
            });

            Vector3[] pos = new Vector3[]
            {
                new Vector3(-50f, 0, -7.22f),
                new Vector3(-52f, 0, -6.42f),
                new Vector3(-54.4f, 0, -16f)
            };

            for (int i = 0; i < entities.Length; i++) 
            {
                entities[i].transform.position = pos[i];
                entities[i].startpos = entities[i].transform.position;
            }

            MainManager.SetCamera(entities[0].transform.position + new Vector3(1,2,-3),1);
            yield return EventControl.tenthsec;

            for (int i = 0; i < party.Length; i++)
            {
                party[i].FaceTowards(entities[0].transform.position, true);
            }

            MainManager.FadeOut();
            yield return EventControl.sec;

            MainManager.DialogueText(MainManager.map.dialogues[93], entities[0].transform, entities[0].npcdata);
            while (MainManager.instance.message)
                yield return null;

            MainManager.PlaySound("ElevatorEnd");
            yield return EventControl.halfsec;

            for (int i = 0; i < entities.Length - 1; i++) 
            {
                entities[i].Emoticon(MainManager.Emoticons.Exclamation, 60);
                entities[i].flip = false;
            }

            for (int i = 0; i < party.Length; i++)
            {
                party[i].Emoticon(MainManager.Emoticons.Exclamation, 60);
            }

            yield return EventControl.halfsec;

            MainManager.PlaySound("WoodEnd");

            entities[2].MoveTowards(new Vector3(-53.82f, 0, -9f));
            yield return null;

            while (entities[2].forcemove)
                yield return null;

            MainManager.DialogueText(MainManager.map.dialogues[94], entities[2].transform, entities[2].npcdata);
            while (MainManager.instance.message)
                yield return null;

            SpriteRenderer cake = MainManager.NewSpriteObject(entities[0].transform.position + new Vector3(0,1,-0.1f), MainManager.map.transform, MainManager.itemsprites[0, (int)NewItem.MysteryCake]);
            MainManager.PlaySound("ItemHold");

            yield return EventControl.halfsec;
            yield return BattleControl_Ext.LerpPosition(40, cake.transform.position, entities[2].transform.position + new Vector3(0, 1, -0.1f), cake.transform);
            UnityEngine.Object.Destroy(cake.gameObject);

            MainManager.DialogueText(MainManager.map.dialogues[95], entities[2].transform, entities[2].npcdata);
            while (MainManager.instance.message)
                yield return null;

            MainManager.FadeIn(0.1f);
            yield return EventControl.sec;
            MainManager.PlaySound("Eat", 9, 1, 1, true);

            SpriteRenderer transitionObj = MainManager.instance.transitionobj[0].GetComponent<SpriteRenderer>();
            transitionObj.color = Color.black;
            transitionObj.sortingOrder = -10;
            MiniBubble minibubble = MiniBubble.SetUp(MainManager.map.dialogues[96], entities[2], new Vector3(0f, 1f, 10f), 0);

            yield return EventControl.sec;
            MainManager.StopSound(9);

            while (minibubble != null)
            {
                yield return null;
            }
            MainManager.PlaySound("Clomp");
            MainManager.FadeOut();
            yield return EventControl.halfsec;

            MainManager.DialogueText(MainManager.map.dialogues[97], entities[2].transform, entities[2].npcdata);
            while (MainManager.instance.message)
                yield return null;

            MainManager.instance.flags[932] = true;
            MainManager.FadeIn();
            yield return EventControl.sec;
            UnityEngine.Object.Destroy(MainManager.map.gameObject);
            GC.Collect();
            Resources.UnloadUnusedAssets();
            yield return null;
            MainManager.LoadMap((int)MainManager.Maps.GiantLairRoachVillage);
            yield return EventControl.sec;
            MainManager.ResetCamera(true);
            MainManager.FadeOut();
            yield return EventControl.halfsec;
            MainManager.ChangeMusic();
            MainManager.CompleteQuest((int)NewQuest.HerFavoriteSweet);
        }
    }
}
