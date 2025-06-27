using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static BFPlus.Extensions.PitData;

namespace BFPlus.Extensions.Events.PitEvents
{
    public class PitStartEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            MainManager.instance.flagvar[43]++;
            MainManager.instance.flagvar[(int)NewFlagVar.Pit_Floor] = 0;
            MainManager.instance.flagvar[(int)NewFlagVar.PitEnemyDeadLastFloor] = 0;

            MainManager.SetCamera(new Vector3(0f, 1f, 10f), 0.035f);
            Vector3[] posArray = new Vector3[]
            {
                new Vector3(-1.5f, 1.5f, 9f),
                new Vector3(0f, 1.5f, 9f),
                new Vector3(1.5f, 1.5f, 9f),
                new Vector3(2.2f, 1.5f, 9f)
            };
            EntityControl[] party = MainManager.GetPartyEntities(true);

            for (int i = 0; i < party.Length; i++)
            {
                party[i].transform.position = posArray[i];
                party[i].animstate = (int)MainManager.Animations.Idle;
            }

            var chompy = MainManager.map.chompy;
            if (chompy != null)
            {
                chompy.transform.position = posArray[3];
            }

            EntityControl lieutenant = null;
            if (MainManager.map.tempfollowers != null)
            {
                lieutenant = MainManager.map.tempfollowers.FirstOrDefault(f => f.animid == (int)MainManager.AnimIDs.AntCapitain - 1);
                if(lieutenant != null)
                    lieutenant.transform.position = new Vector3(1.5f, 1.5f, 10f);
            }

            AudioClip rumble = Resources.Load<AudioClip>("Audio/Sounds/Rumble");
            MainManager.ShakeScreen(Vector3.one * 0.1f, -1f);
            MainManager.PlaySound(rumble, 9, 1f, 0.5f, true);
            yield return new WaitForSeconds(2f);
            MainManager.StopSound("Rumble", 0.1f);
            MainManager.screenshake = Vector3.zero;
            yield return new WaitForSeconds(0.75f);
            MainManager.PlaySound("Glow");
            yield return null;
            var platform = GameObject.Find("MainPlatform");
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
