using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events.NewDungeonsEvents
{
    public class SandCastleDepthsIcePuzzleEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            MainManager.SetCamera(new Vector3(-18.9f, 0, -4.05f), 0.035f);
            NPCControl key = EntityControl.CreateItem(new Vector3(-18.9f, 10, -4.05f), 1, (int)MainManager.Items.SandCastleBossKey, Vector3.down, -1);
            key.activationflag = 851;

            yield return EventControl.sec;
            yield return EventControl.halfsec;

            MainManager.instance.flags[850] = true;
            MainManager.ResetCamera();
            yield return null;
        }
    }
}
