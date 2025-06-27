using BFPlus.Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Patches
{
    [HarmonyPatch(typeof(MusicSpinner), "OnTriggerEnter")]
    public class PatchMusicSpinnerOnTriggerEnter
    {
        static void Postfix(MusicSpinner __instance, Collider other)
        {
            if((int)MainManager.map.mapid == (int)NewMaps.GiantLairPlayroomBoss)
            {
                if (__instance.spin + __instance.spinhit > __instance.spinlimit)
                {
                    MainManager.events.StartEvent((int)NewEvents.PlayroomBoss, null);
                    UnityEngine.Object.Destroy(__instance);
                }
            }
        }
    }
}
