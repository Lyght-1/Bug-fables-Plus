using BFPlus.Extensions.Events.DarkTeamSnakemouthEvents;
using BFPlus.Patches.DoActionPatches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

namespace BFPlus.Extensions.Events.GourmetRaceEvents
{
    public class EndGourmetRaceEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            MainManager.PlaySound("Whistle3");
            MainManager.FadeMusic(0.015f);
            MainManager.PlayTransition(0, 0, 0.025f, Color.black);

            GourmetRace raceObj = GameObject.FindObjectOfType<GourmetRace>();
            raceObj.HideUI();
            (int AnimId, int Points)[] contestants = raceObj.GetContestantsPoints();
            yield return null;

            SpriteRenderer s = MainManager.GetTransitionSprite();
            while (s.color.a < 0.95f || MainManager.musiccoroutine != null)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.75f);
            MainManager.ChangeParty(new int[] { 0, 1, 2 }, true, true);


            //can tattle flag
            MainManager.instance.flags[10] = true;
            yield return null;
            MainManager.LoadMap((int)MainManager.map.mapid, true);
            yield return EventControl.halfsec;

            int count = 0;
            for (int i = MainManager.instance.playerdata.Length-1; i >= 0; i--)
            {
                MainManager.instance.playerdata[i].entity.transform.position = new Vector3(-1.4f + 1.5f * (float)count, 0f, 2.39f);
                MainManager.instance.playerdata[i].entity.flip = false;
                count++;
            }

            if (MainManager.map.chompy != null)
            {
                MainManager.map.chompy.transform.position = new Vector3(3, 0f, 2.5f);
                MainManager.map.chompy.flip = false;
            }

            List<EntityControl> contestantEntities = MainManager.GetEntities(new int[] { 3, 4 }).ToList();
            foreach (var e in contestantEntities)
                e.alwaysactive = true;

            contestantEntities[0].transform.position = new Vector3(0f, 0f, 1);
            contestantEntities[1].transform.position = new Vector3(-6f, 0f, 1.5f);
            contestantEntities[1].flip = true;

            contestantEntities.Add(MainManager.GetEntity(-6));
            EntityControl judge = MainManager.GetEntity(2);
            judge.flip = true;
            MainManager.SetCamera(judge.transform.position, 0.01f);
            CreateAudience(MainManager.map.transform, 15, Audience.Type.OnlyBee, new Vector2(1, 0.1f), new Vector3(10, 1), new Vector3(-3, 0, -2), new Vector3(0, 0, 0));

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

            if (MainManager.instance.flagvar[1] > MainManager.instance.flagvar[(int)NewFlagVar.Gourmet_Highscore])
            {
                MainManager.instance.flagvar[(int)NewFlagVar.Gourmet_Highscore] = MainManager.instance.flagvar[1];
            }

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[8], true, Vector3.zero, judge.transform, judge.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            Dictionary<int, int> animIdToDialogueLine = new Dictionary<int, int>()
            {
                 { 2, 14 },
                 { (int)MainManager.AnimIDs.Zasp-1, 13 },
                 { (int)MainManager.AnimIDs.Chubee-1, 15 },
            };

            for (int i = 0; i < contestants.Length; i++)
            {
                int animId = contestants[i].AnimId;
                int points = contestants[i].Points;
                EntityControl entity = contestantEntities.FirstOrDefault(e => e.animid == animId);

                MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[9+i] + points + " |call,16|", true, Vector3.zero, judge.transform, judge.npcdata));
                while (MainManager.instance.message)
                {
                    yield return null;
                }
                int dialogueLine = animIdToDialogueLine[animId];

                //set camera on contestant
                MainManager.SetCamera(entity.transform, entity.transform.position,0.1f, new Vector3(0,2.25f,-5f));
                entity.animstate = (int)MainManager.Animations.ItemGet;

                MainManager.PlaySound("CrowdClapping");
                if (i == 2)
                    MainManager.PlaySound("CrowdCheer3", -1, 1.1f, 0.8f);

                MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[dialogueLine], true, Vector3.zero, judge.transform, judge.npcdata));
                while (MainManager.instance.message)
                {
                    yield return null;
                }

                yield return EventControl.quartersec;
                entity.animstate = (int)MainManager.Animations.Idle;
                MainManager.SetCamera(judge.transform, judge.transform.position, 0.05f, new Vector3(0, 2.25f, -8.25f));
            }

            MainManager.SetCamera(judge.transform.position, 0.01f);
            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[12], true, Vector3.zero, judge.transform, judge.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }
            yield return EventControl.tenthsec;

            MainManager.FadeIn();
            yield return EventControl.sec;

            MainManager.LoadMap((int)MainManager.map.mapid, true);
            yield return null;

            judge = MainManager.GetEntity(2);
            judge.flip = true;
            MainManager.SetCamera(judge.transform.position, 0.01f);
            count = 0;
            for (int i = MainManager.instance.playerdata.Length - 1; i >= 0; i--)
            {
                MainManager.instance.playerdata[i].entity.transform.position = new Vector3(-1.4f + 1.5f * (float)count, 0f, 2.39f);
                MainManager.instance.playerdata[i].entity.flip = false;
                count++;
            }

            if (MainManager.map.chompy != null)
            {
                MainManager.map.chompy.transform.position = new Vector3(3, 0f, 2.5f);
                MainManager.map.chompy.flip = false;
            }

            MainManager.PlayTransition(1, 0, 0.025f, Color.black);
            yield return null;
            s = MainManager.GetTransitionSprite();
            while (s.color.a > 0.1f)
            {
                yield return null;
            }
            yield return EventControl.sec;

            int playerPosition = contestants.Select((participant, index) => new { participant.AnimId, Index = index }).FirstOrDefault(x => x.AnimId == 2).Index;

            int berryReward = Mathf.Clamp(Mathf.FloorToInt((float)MainManager.instance.flagvar[1] * 1.15f), 1, MainManager.instance.flagvar[1]);
            int reward = playerPosition == 2 ? (int)MainManager.Items.TangyJam : (int)MainManager.Items.HoneyPancake;

            string text = "";

            int nextDialogue = playerPosition > 0 ? 18 : 19;

            if (MainManager.instance.flagvar[1] >= 50 && !MainManager.instance.flags[875])
            {
                nextDialogue = 24;
                MainManager.map.transform.Find("gourmetStomach").gameObject.SetActive(false);
            }
            if (MainManager.instance.flagvar[1] > 0)
            {
                text = "|break||giveitem,-1," + Mathf.Clamp(Mathf.FloorToInt((float)MainManager.instance.flagvar[1] * 1.15f), 1, MainManager.instance.flagvar[1]) + $",{nextDialogue},-6|";
            }

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[17] + text, true, Vector3.zero, judge.transform, judge.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            if (playerPosition > 0)
            {
                bool inventoryFull = MainManager.instance.items[0].Count >= MainManager.instance.maxitems;
                text = "";
                if (!inventoryFull)
                {
                    text = $"|break||giveitem,0,{reward},19,-6|";
                }

                int dialoguePositionReward = playerPosition == 2 ? 20 : 21;
                MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[dialoguePositionReward]+text, true, Vector3.zero, judge.transform, judge.npcdata));
                while (MainManager.instance.message)
                {
                    yield return null;
                }

                if (inventoryFull)
                {
                    text = $"|createitem,0,{reward},-6,0,1,-1||break||goto,19|";
                    MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[22] + text, true, Vector3.zero, judge.transform, judge.npcdata));
                    while (MainManager.instance.message)
                    {
                        yield return null;
                    }
                }
            }

            MainManager.ResetCamera();
        }
    }
}
