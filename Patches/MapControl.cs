using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using BFPlus.Extensions;
using System.Collections;

namespace BFPlus.Patches
{
    [HarmonyPatch(typeof(MapControl), "CreateEntities")]
    public class PatchMapControlCreateEntities
    {
        static void Prefix(MapControl __instance)
        {
            if (MainManager.map != null)
            {
                if (__instance.GetComponent<MapControl_Ext>() == null)
                    __instance.gameObject.AddComponent<MapControl_Ext>();
                MapControl_Ext.npcToAdd = new List<NewNpc>();
                TextAsset mapChanges = MainManager_Ext.assetBundle.LoadAllAssets<TextAsset>().Where(asset => asset.name.Contains("mapData") && asset.name.Split('_')[1] == "mapData" && asset.name.Split('_')[0] == __instance.mapid.ToString()).FirstOrDefault();
                if(mapChanges != null)
                {
                    int result = 0;
                    string[] data = mapChanges.ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i != data.Length; i++)
                    {
                        string[] entity = data[i].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        if (!int.TryParse(entity[2], out result) && result != -1)
                            MapControl_Ext.npcToAdd.Add(new NewNpc(entity[2], entity[0], int.Parse(entity[1])));
                    }
                }

                TextAsset mapDialogues = MainManager_Ext.assetBundle.LoadAllAssets<TextAsset>().FirstOrDefault(asset => asset.name.Contains("mapDialogue") && asset.name.Split('_')[0] == __instance.mapid.ToString());
                if (mapDialogues != null)
                {
                    string[] dialogues = mapDialogues.ToString().Split('\n');
                    var currentMapDialogues = __instance.dialogues.ToList();
                    for(int i=0; i != dialogues.Length; i++)
                    {
                        string[] line = dialogues[i].Split(new char[] { '[' }, StringSplitOptions.RemoveEmptyEntries);
                        int replaceLine = int.Parse(line[0]);
                        if(replaceLine != -1)
                        {
                            currentMapDialogues.RemoveAt(replaceLine);
                            currentMapDialogues.Insert(replaceLine, line[1]);
                            continue;
                        }
                        currentMapDialogues.Add(line[1]);
                    }
                    __instance.dialogues = currentMapDialogues.ToArray();
                }


                if(MainManager_Ext.IsCustomMap())
                {
                    mapDialogues = MainManager_Ext.assetBundle.LoadAsset<TextAsset>(((NewMaps)__instance.mapid).ToString() +"Dialogue");
                    __instance.dialogues = mapDialogues.ToString().Split('\n');
                }
            }
        }

        static void Postfix(MapControl __instance)
        {
            if ((NewMaps)__instance.mapid == NewMaps.Pit100Reward)
            {
                var pitData = PitData.GetPitData();
                pitData.CheckPitReward(__instance);
                pitData.SetupShop(__instance);
            }

            if ((NewMaps)__instance.mapid == NewMaps.Pit100BaseRoom)
            {
                var pitData = PitData.GetPitData();

                if (!pitData.moverFloor)
                {
                    if(PitData.GetCurrentFloor() != 99)
                    {
                        var enemy = MainManager.map.entities.First(e => e.npcdata.entitytype == NPCControl.NPCType.Enemy);
                        enemy.npcdata.battleids = pitData.GetCurrentFloorEnemies();
                        enemy.startpos = pitData.GetRandomEnemyPos();
                        enemy.transform.position = enemy.startpos.Value;
                        enemy.npcdata.vectordata = pitData.GetRandomFloorEnemyItemDrop();
                        enemy.npcdata.teleportradius = 30f;
                        enemy.npcdata.radiuslimit = 20f;
                    }
                    else
                    {
                        EntityControl[] enemies = MainManager.map.entities.Where(e => e.npcdata.entitytype == NPCControl.NPCType.Enemy).ToArray();      
                        pitData.GetEnemiesPos(enemies);

                        for(int i=0;i<enemies.Length; i++)
                        {
                            enemies[i].npcdata.vectordata = pitData.GetRandomFloorEnemyItemDrop();
                            enemies[i].npcdata.teleportradius = 30f;
                            enemies[i].npcdata.radiuslimit = 20f;
                        }
                    }
                }
                else
                {
                    Vector3 switchPos = __instance.entities[1].transform.position;
                    switchPos = new Vector3(switchPos.x, 0, switchPos.z);
                    __instance.entities[1].transform.position = switchPos;
                    __instance.entities[1].startpos = switchPos;
                }
                if (PitData.GetCurrentFloor() != 99)
                    pitData.CreateBushEntities(__instance);

                __instance.battleleaftype = pitData.GetCurrentBattleLeaf();
            }

            if ((NewMaps)__instance.mapid == NewMaps.SandCastleDepthsIcePuzzle)
            {
                __instance.entities[4].gameObject.AddComponent<IcePlatTrigger>();
            }
        }
    }

    [HarmonyPatch(typeof(MapControl), "Start")]
    public class PatchMapControlStart
    {
        static void Prefix(MapControl __instance)
        {
            var changeMethod = typeof(MapControl_Ext).GetMethod("Change" + __instance.mapid);

            if(changeMethod != null)
            {
                MainManager_Ext.LoadMapsBundle();
                changeMethod?.Invoke(null, new object[] { __instance });
                MainManager_Ext.UnloadMapsBundle();
            }


            if (__instance.mapid == MainManager.Maps.SnakemouthEmpty)
            {
                __instance.useglobalcommand = false;
            }
        }

        static void Postfix(MapControl __instance)
        {
            //temporary, deactivate ant gaurd in front of new loading zone
            if (__instance.mapid == MainManager.Maps.AntPalace1)
            {
                __instance.entities[2].gameObject.SetActive(false);
            }

            //temporary, deactivate holoroach
            if (__instance.mapid == MainManager.Maps.CaveOfTrials)
            {
                __instance.entities[4].gameObject.SetActive(false);
                __instance.entities[5].gameObject.SetActive(false);
            }

            //temporary, deactivate judge bee from eating contest
            if (__instance.mapid == MainManager.Maps.BeehiveBalcony)
            {
                __instance.entities[6].gameObject.SetActive(false);
            }

            //temporary, deactivate bush in the way, and clearbomb stuff
            if (__instance.mapid == MainManager.Maps.Swamplands5)
            {
                __instance.entities[11].gameObject.SetActive(false);
                __instance.entities[30].gameObject.SetActive(false);
                __instance.entities[31].gameObject.SetActive(false);
            }

            if ((int)__instance.mapid == (int)NewMaps.AntPalaceTrainingRoom)
            {
                __instance.entities[9].gameObject.AddComponent<TrainingNPC>();
                __instance.entities[11].gameObject.AddComponent<TrainingNPC>();
                __instance.entities[2].gameObject.AddComponent<TrainingNPC>();
            }

            if((int)__instance.mapid == (int)NewMaps.LeafbugVillage && MainManager.instance.flags[886])
            {
                BadmintonObj badminton =new GameObject("badmintonObj").AddComponent<BadmintonObj>();
                badminton.transform.parent =  __instance.transform;
            }

            if (MainManager.instance.flags[916])
            {
                MapControl_Ext.CheckIntermissionEntities(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(MapControl), "Test")]
    public class PatchMapControlTest
    {
        static bool Prefix(MapControl __instance)
        {
            return false;
        }
    }

    /// <summary>
    /// If we are in a intermission, dont check discoveries (detector medal)
    /// </summary>
    [HarmonyPatch(typeof(MapControl), "CheckDisc")]
    public class PatchMapControlCheckDisc
    {
        static bool Prefix(MapControl __instance)
        {
            return !MainManager.instance.flags[916];
        }
    }
}
