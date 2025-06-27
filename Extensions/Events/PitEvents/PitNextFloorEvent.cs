using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events.PitEvents
{
    public class PitNextFloorEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            EntityControl[] party = MainManager.GetPartyEntities(true);
            Vector3[] posArray = new Vector3[]
            {
                new Vector3(-1.5f, 0f, 6.5f),
                new Vector3(0f, 0f, 6.5f),
                new Vector3(1.5f, 0f, 6.5f),
                new Vector3(2.5f, 0f, 6.5f)
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

            EntityControl lieutenant = MainManager.map.tempfollowers?.FirstOrDefault(f => f?.animid == (int)MainManager.AnimIDs.AntCapitain - 1);

            lieutenant?.MoveTowards(new Vector3(2.5f, 0f, 7.5f));

            while (!MainManager.PartyIsNotMoving() || (chompy != null && chompy.forcemove) || (lieutenant != null && lieutenant.forcemoving != null))
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.75f);
            var platform = GameObject.Find("AncientPlatform");
            AudioClip platformMove = Resources.Load<AudioClip>("Audio/Sounds/PlatformMove");

            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                MainManager.instance.playerdata[i].entity.transform.parent = platform.transform;
                MainManager.instance.playerdata[i].entity.LockRigid(true);
                MainManager.instance.playerdata[i].entity.ccol.enabled = false;
            }

            if (MainManager.map.chompy != null)
            {
                MainManager.map.chompy.transform.parent = platform.transform;
            }

            if (lieutenant != null)
            {
                lieutenant.transform.parent = platform.transform;
            }

            MainManager.PlaySound(platformMove, -1, 1f, 1f, true);
            yield return null;
            MainManager.PlayTransition(0, 0, 0.075f, Color.black);
            float a = 0f;
            float b = 120;
            var platPos = platform.transform.position;
            do
            {
                platform.transform.position = MainManager.SmoothLerp(platPos, platPos + new Vector3(0f, -5), a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a <= b);
            yield return LoadPitRoom(instance);
        }
    }
}
