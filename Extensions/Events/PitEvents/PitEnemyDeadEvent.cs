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
    public class PitEnemyDeadEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            if(caller != null && caller.gameObject.activeSelf)
            {
                while (caller.entity.deathcoroutine != null)
                {
                    yield return null;
                }
            }

            yield return new WaitForSeconds(0.5f);
            MainManager.ShakeScreen(Vector3.one * 0.1f, -1f);
            MainManager.PlaySound("Rumble", 9, 1f, 1f, true);

            GameObject blueSwitch = GameObject.Find("switch");
            var switchPos = blueSwitch.transform.position;
            Vector3 destination = new Vector3(switchPos.x, 0f, switchPos.z);
            MainManager.instance.camtarget = null;
            MainManager.instance.camtargetpos = new Vector3?(destination);
            MainManager.instance.camspeed = 0.025f;

            float a = 0f;
            float b = 60;
            do
            {
                blueSwitch.transform.position = MainManager.SmoothLerp(switchPos, destination, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a <= b);

            MainManager.StopSound("Rumble", 0.1f);
            MainManager.screenshake = Vector3.zero;
            yield return new WaitForSeconds(0.5f);
            MainManager.ResetCamera();
            MainManager.ChangeMusic();
            yield return null;
        }
    }
}
