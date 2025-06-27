using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Object;
namespace BFPlus.Extensions.Events.PitEvents
{
    public class LeavePitBossRoomEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            EntityControl[] party = MainManager.GetPartyEntities(true);

            Vector3[] posArray = new Vector3[]
            {
                new Vector3(-1.5f, 0.46f, 45f),
                new Vector3(0f, 0.46f, 45f),
                new Vector3(1.5f, 0.46f, 45f),
                new Vector3(2.5f, 0.46f, 45f)
            };
            instance.MoveParty(posArray);

            for (int num6 = 0; num6 < party.Length; num6++)
            {
                party[num6].forcejump = true;
            }
            var chompy = MainManager.map.chompy;
            if (chompy != null)
            {
                chompy.forcejump = true;
            }

            while (!MainManager.PartyIsNotMoving() || (chompy != null && chompy.forcemove))
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.75f);
            var platform = GameObject.Find("AncientPlatform");
            AudioClip platformMove = Resources.Load<AudioClip>("Audio/Sounds/PlatformMove");

            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                MainManager.instance.playerdata[i].entity.transform.parent = platform.transform;
            }

            if (MainManager.map.chompy != null)
            {
                MainManager.map.chompy.transform.parent = platform.transform;
            }

            MainManager.PlaySound(platformMove, -1, 1f, 1f, true);
            yield return null;
            MainManager.PlayTransition(0, 0, 0.075f, Color.black);
            float a = 0f;
            float b = 120;
            var platPos = platform.transform.position;
            do
            {
                platform.transform.position = MainManager.SmoothLerp(platPos, platPos + new Vector3(0f, 10), a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a <= b);

            Destroy(MainManager.instance.GetComponent<PitData>());
            MainManager.LoadMap((int)MainManager.Maps.CaveOfTrials);
            MainManager.SetCamera(MainManager.player.transform, null, 1);

            //Times completed + streak
            MainManager.instance.flagvar[39]++;
            MainManager.instance.flagvar[38]++;

            posArray = new Vector3[]
            {
                new Vector3(-1.5f, 1.5f, 9f),
                new Vector3(0f, 1.5f, 9f),
                new Vector3(1.5f, 1.5f, 9f),
                new Vector3(2.5f, 1.5f, 9f)
            };

            party = MainManager.GetPartyEntities(true);
            for (int i = 0; i < party.Length; i++)
            {
                party[i].transform.position = posArray[i];
            }

            if (MainManager.map.chompy != null)
            {
                MainManager.map.chompy.transform.position = posArray[4];
            }

            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                MainManager.instance.playerdata[i].entity.transform.parent = null;
            }

            if (MainManager.map.chompy != null)
            {
                MainManager.map.chompy.transform.parent =MainManager.map.transform;
            }
            yield return EventControl.sec;
            MainManager.StopSound(platformMove);
            MainManager.PlayTransition(1, 0, 0.075f, Color.black);
            SpriteRenderer dimmer = MainManager.instance.transitionobj[0].GetComponent<SpriteRenderer>();
            while (dimmer.color.a > 0.1f)
            {
                yield return null;
            }
            yield return EventControl.quartersec;
        }
    }
}
