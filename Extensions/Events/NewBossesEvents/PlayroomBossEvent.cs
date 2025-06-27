using BFPlus.Extensions.EnemyAI;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MainManager;

namespace BFPlus.Extensions.Events.NewBossesEvents
{

    public class PlayroomBossEvent : StartBossFightEvent
    {
        protected override IEnumerator ApproachingBoss(EventControl instance, NPCControl caller)
        {
            EntityControl jester = MainManager.GetEntity(1);
            EntityControl[] party = MainManager.GetPartyEntities(true);
            if (MainManager.map.chompy != null)
                party = party.AddToArray(MainManager.map.chompy);

            Vector3[] posArray = new Vector3[]
            {
                new Vector3(26,0.1f,6f),
                new Vector3(24, 0.1f,5f),
                new Vector3(22f, 0.1f, 6f),
                new Vector3(23.5f, 0.1f, 6.5f)
            };

            Transform tempJester = MainManager.map.mainmesh.Find("Jester");
            jester.transform.position = tempJester.position + Vector3.down;
            tempJester.transform.position -= Vector3.down*30;
            jester.animstate = 100;
            MainManager.SetCamera(jester.transform.position + new Vector3(-5, 5f, -5), 0.01f);

            for (int i = 0; i < party.Length; i++)
            {
                party[i].transform.position = posArray[i];
                party[i].animstate = (int)MainManager.Animations.BattleIdle;
                party[i].FaceTowards(jester.transform.position);
            }

            yield return EventControl.sec;

            MainManager.PlaySound("Charge7");
            jester.StartCoroutine(jester.ShakeSprite(0.2f, 120f));
            yield return EventControl.sec;
            yield return EventControl.sec;
            jester.animstate = 102;
            MainManager.PlaySound("Boing1");
            for (int i = 0; i < party.Length; i++)
            {
                party[i].animstate = (int)MainManager.Animations.Surprized;
            }

            yield return EventControl.sec;
            yield return StartBattle(new int[] { (int)NewEnemies.Jester });

            MainManager.SetCamera(jester.transform.position + new Vector3(-5, 5f, -5), 1f);

            jester.animstate = 128;
            foreach (var p in party)
                p.animstate = (int)MainManager.Animations.BattleIdle;

            yield return null;
            MainManager.FadeOut();
            yield return EventControl.sec;


            MainManager.DialogueText(MainManager.map.dialogues[9], jester.transform, jester.npcdata);
            while (MainManager.instance.message)
                yield return null;

            jester.animstate = 131;
            yield return EventControl.sec;

            jester.gameObject.SetActive(false);
            tempJester.transform.position = jester.transform.position + Vector3.up;

            Transform handle = tempJester.GetChild(1).GetChild(0);

            MainManager.SetCamera(handle.transform.position + new Vector3(0, 0f, -2), 0.1f);
            yield return EventControl.halfsec;

            float a = 0;
            float b = 30;
            Vector3 startPos = handle.transform.position;
            Vector3 endPos = handle.transform.position + new Vector3(-0.2f,-1f,-2f);
            Vector3 startRot = handle.transform.localEulerAngles;
            Vector3 endRot = new Vector3(300, 150, 0);
            MainManager.PlaySound("BridgeRope");
            do
            {
                handle.position = Vector3.Lerp(startPos, endPos, a/b);
                handle.localEulerAngles = Vector3.Lerp(startRot, endRot, a/b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);

            MainManager.PlaySound("ArtifactBounce");
            yield return EventControl.halfsec;

            for(int i = 0; i < 5; i++)
            {
                handle.gameObject.SetActive(!handle.gameObject.activeSelf);
                yield return EventControl.quartersec;
            }

            GameObject item = EntityControl.CreateItem(handle.transform.position, 1, (int)NewItem.SilverHandle, Vector3.zero, -1).gameObject;
            yield return EventControl.halfsec;

            foreach (var p in party)
            {
                p.animstate = 0;
            }

            Vector3 basePos = party[0].transform.position;

            party[0].MoveTowards(item.transform.position, 1, (int)MainManager.Animations.Walk, 0);
            while (party[0].forcemove)
            {
                yield return null;
            }
            UnityEngine.Object.Destroy(item.gameObject);
            instance.GiveItem(1, (int)NewItem.SilverHandle, -4);
            while (MainManager.instance.message)
            {
                yield return null;
            }
            MainManager.ResetCamera();
            party[0].MoveTowards(basePos, 1.5f, (int)MainManager.Animations.Walk, 0);
            while (party[0].forcemove)
            {
                yield return null;
            }
            yield return EventControl.halfsec;

            MainManager.DialogueText(MainManager.map.dialogues[10], party[0].transform, party[0].npcdata);
            while (MainManager.instance.message)
                yield return null;

            MainManager.instance.flags[927] = true;
            MainManager.FadeIn();
            yield return EventControl.sec;
            MainManager.ResetCamera();
            MainManager.LoadMap();
            yield return EventControl.tenthsec;
            MainManager.FadeOut();
            yield return EventControl.halfsec;
            MainManager.ChangeMusic();
        }
    }
}
