using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using BFPlus.Extensions;
using System.Runtime.Remoting.Lifetime;

namespace BFPlus.Patches
{
    /*just testing stuff lul
    [HarmonyPatch(typeof(NPCControl), "DoBehavior", new Type[] { typeof(NPCControl.ActionBehaviors), typeof(float)},new ArgumentType[] { ArgumentType.Ref,ArgumentType.Normal})]
    public class PatchNPCControlDoBehavior
    {
        static bool Prefix(NPCControl __instance)
        {
            if(LightningManager.timeOfDay >= 20 || LightningManager.timeOfDay <= 5)
            {
                __instance.entity.animstate = (int)MainManager.Animations.Sleep;
                return false;
            }
            return true;
        }
    }*/


    [HarmonyPatch(typeof(NPCControl), "StartBattle", new Type[] { })]
    public class PatchNPCControlStartBattle
    {      
        static bool Prefix(NPCControl __instance)
        {
            if (MainManager_Ext.inSeedlingMinigame)
                return false;

            if ((__instance.entity.animid == (int)NewAnimID.Worm || __instance.entity.animid == (int)NewAnimID.WormSwarm  )&& __instance.entity.digging)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(NPCControl), "CheckBump")]
    public class PatchNPCControlCheckBump
    {
        static void Postfix(NPCControl __instance, ref bool __result)
        {
            if((NewMaps)MainManager.map.mapid == NewMaps.Pit100BaseRoom || (NewMaps)MainManager.map.mapid == NewMaps.Pit100Reward || MainManager.map.mapid == MainManager.Maps.GoldenSMinigame)
            {
                __result = false;
            }
        }
    }


    [HarmonyPatch(typeof(NPCControl), "SetUp")]
    public class PatchNPCControlSetUp
    {
        static void Postfix(NPCControl __instance)
        {
            if (__instance.entity.animid == (int)NewAnimID.Mars)
            {
                __instance.entity.model.localScale = Vector3.one * 0.7f;
                __instance.entity.model.Find("MarsPot").gameObject.AddComponent<BoxCollider>();
            }

            if (__instance.entity.animid == (int)NewAnimID.Jester)
            {
                __instance.entity.model.localScale = Vector3.one;
            }

            if ((int)MainManager.map.mapid == (int)NewMaps.SandCastleDepthsIcePuzzle && __instance.objecttype == NPCControl.ObjectTypes.PushRock && __instance.data.Length <=2)
            {
                __instance.entity.model.GetComponent<MeshRenderer>().materials = new Material[] { Resources.Load("materials/Ice") as Material, MainManager.outlinemain };
            }
        }
    }

    /// <summary>
    /// fixes a weird bug that causes an inside badge shop to flicker with hp plus if the badge pool is empty
    /// </summary>
    [HarmonyPatch(typeof(NPCControl), "SetBadgeShop")]
    public class PatchNPCControlSetBadgeShop
    {
        static void Postfix(NPCControl __instance, bool refresh)
        {
            if (__instance.shopitems != null)
            {
                foreach (var item in __instance.shopitems)
                {
                    if (item.iskill && item.npcdata != null)
                        item.npcdata.insideid = -1;
                }

                if (refresh && MainManager.instance.insideid != -1 && __instance.shopitems != null)
                {
                    foreach (var item in __instance.shopitems)
                    {
                        if (item.iskill && item.npcdata != null)
                        {
                            item.npcdata.insideid = MainManager.instance.insideid;
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(NPCControl), "OnTriggerEnter")]
    public class PatchNPCControlOnTriggerEnter
    {
        static void Postfix(NPCControl __instance, Collider other)
        {
            if(MainManager.instance.flags[945] && __instance.entitytype == NPCControl.NPCType.NPC && other.CompareTag("Icecle"))
            {
                if (__instance.collisionammount == 0 && !__instance.entity.dead && !__instance.entity.digging && __instance.touchcooldown <= 0f)
                {
                    __instance.StartCoroutine(__instance.entity.TempIgnoreColision(other, 2));
                    __instance.collisionammount++;
                    __instance.StartCoroutine(DoFirePillarHitAnim(__instance));
                }
            }
        }

        static IEnumerator DoFirePillarHitAnim(NPCControl __instance)
        {
            __instance.entity.PlaySound("Damage0");
            __instance.entity.animstate = (int)MainManager.Animations.Hurt;
            __instance.interactcd = 120;
            yield return __instance.Dizzy(120, Vector3.zero, false);
            yield return EventControl.sec;
            __instance.entity.animstate = __instance.entity.basestate;
        }
    }

    /// <summary>
    /// If we are in a intermission, dont check hidden item (detector medal)
    /// </summary>
    [HarmonyPatch(typeof(NPCControl), "CheckHidden")]
    public class PatchNPCControlCheckHidden
    {
        static bool Prefix(NPCControl __instance)
        {
            return !MainManager.instance.flags[916];
        }
    }
}
