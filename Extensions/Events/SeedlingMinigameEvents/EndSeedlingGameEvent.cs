using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events.SeedlingMinigameEvents
{
    public class EndSeedlingGameEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            MainManager.PlaySound("Whistle3");
            MainManager.FadeMusic(0.015f);
            MainManager.PlayTransition(0, 0, 0.025f, Color.black);
            yield return null;
            SpriteRenderer s = MainManager.GetTransitionSprite();
            while (s.color.a < 0.95f || MainManager.musiccoroutine != null)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.75f);
            MainManager.ChangeParty(new int[] { 0, 1, 2 }, true, true);

            UnityEngine.Object.Destroy(GameObject.FindObjectOfType<SeedlingMinigame>().gameObject);

            MainManager_Ext.inSeedlingMinigame = false;

            //can tattle flag
            MainManager.instance.flags[10] = true;
            yield return null;
            MainManager.LoadMap((int)MainManager.map.mapid, true);
            yield return null;

            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                MainManager.instance.playerdata[i].entity.transform.position = new Vector3(-10f + 1.5f * (float)i, 0f, 9.8f);
                MainManager.instance.playerdata[i].entity.flip = false;
            }

            if (MainManager.map.chompy != null)
            {
                MainManager.map.chompy.transform.position = new Vector3(-6, 0f, 10f);
                MainManager.map.chompy.flip = false;
            }

            MainManager.ResetCamera(true);

            EntityControl npc = MainManager.GetEntity(2);
            npc.flip = true;
            yield return null;
            while (MainManager.musiccoroutine != null)
            {
                yield return null;
            }
            yield return EventControl.tenthsec;

            MainManager.ChangeMusic(MainManager.map.music[0]);
            MainManager.music[0].pitch = 1f;
            MainManager.PlayTransition(1, 0, 0.035f, Color.black);
            yield return null;

            s = MainManager.GetTransitionSprite();
            while (s.color.a > 0.1f)
            {
                yield return null;
            }

            if (MainManager.instance.flagvar[1] > MainManager.instance.flagvar[(int)NewFlagVar.Seedling_Highscore])
            {
                MainManager.instance.flagvar[(int)NewFlagVar.Seedling_Highscore] = MainManager.instance.flagvar[1];
            }

            string text = "";
            int dialogueID = 8;
            int dialogueEndID = 9;


            if (MainManager.instance.flagvar[1] >= 20)
            {
                dialogueEndID = 11;
            }

            if (MainManager.instance.flagvar[1] >= 30 && !MainManager.instance.flags[838])
            {
                text = "|break||giveitem,2," + (int)Medal.Avalanche + $",{dialogueEndID}|";
                dialogueID = 10;
                MainManager.instance.flags[838] = true;
            }
            else
            {
                if (MainManager.instance.flagvar[1] > 0)
                {
                    text = "|break||giveitem,-1," + Mathf.Clamp(Mathf.FloorToInt((float)MainManager.instance.flagvar[1] * 1.15f), 1, MainManager.instance.flagvar[1]) + $",{dialogueEndID}|";
                }
            }

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[dialogueID] + text, true, Vector3.zero, npc.transform, npc.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }
        }
    }
}
