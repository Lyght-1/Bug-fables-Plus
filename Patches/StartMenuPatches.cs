using BFPlus.Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MainManager;
using System.Reflection;
using System.Collections;
using BFPlus.Extensions.Maps;

namespace BFPlus.Patches
{
    [HarmonyPatch(typeof(StartMenu), "LoadModel")]
    public class PatchStartMenuLoadModel
    {
        static bool Prefix(StartMenu __instance)
        {
            if (LoadNewBackground(__instance)) 
            {
                return false;
            }
            MainManager_Ext.backgroundData = null;
            return true;
        }

        static bool LoadNewBackground(StartMenu __instance)
        {
            if (MainManager_Ext.assetBundle == null)
            {
                MainManager_Ext.assetBundle = AssetBundle.LoadFromMemory(Properties.Resources.vengeance);
            }

            int lastSaved = GetLastSavedFile();

            if(lastSaved == -1)
            {
                return false;
            }

            int areaId = StartMenu.savedata[lastSaved].Value.areaid;
            int mapId = StartMenu.savedata[lastSaved].Value.mapid;

            bool loadMap = false;
            for(int i=0;i < StartMenu.extraareas.Length; i++)
            {
                if (StartMenu.extraareas[i][0] == mapId)
                {
                    loadMap = true;
                    break;
                }
            }

            string areaName = ((MainManager.Areas)areaId).ToString();
            if (loadMap)
            {
                areaName = ((MainManager.Maps)mapId).ToString();
            }

            int newAreaId = MainManager_Ext.GetNewAreaId(mapId);
            if (newAreaId != -1)
            {
                int[] mapToLoad = new int[] 
                {
                    (int)MainManager.Maps.CaveOfTrials,
                    (int)MainManager.Maps.PowerPlant,
                    (int)NewMaps.AbandonedTower3,
                    (int)NewMaps.DeepCave1,
                    (int)MainManager.Maps.MysteryIsland,
                    (int)NewMaps.SandCastleDepthsMain,
                    (int)NewMaps.LeafbugVillage,
                    (int)NewMaps.GiantLairPlayroom1,
                };
                if (newAreaId < mapToLoad.Length)            
                {
                    loadMap = true;
                    mapId = mapToLoad[newAreaId];
                    Type enumType = Enum.IsDefined(typeof(NewMaps), mapId) ? typeof(NewMaps) : typeof(MainManager.Maps);
                    areaName = Enum.ToObject(enumType, mapId).ToString();
                }
            }

            TextAsset asset = MainManager_Ext.assetBundle.LoadAsset<TextAsset>(areaName);
            if (asset != null) 
            {
                int saveDataProgress = StartMenu.savedata[lastSaved].Value.progression;
                AreaData areaData = LoadCSV(asset);
                GameObject map = SetupBackgroundMap(__instance, areaData, saveDataProgress);
                __instance.model = map.transform;

                SetupBackgroundMapEntities(__instance, areaData, saveDataProgress);
                MainManager_Ext.backgroundData = areaData;

                MainManager.SetCameraInstant(areaData.cameraTargetPos);
                Color paperColor = __instance.sprites[2].color;
                __instance.sprites[2].color = new Color(paperColor.r, paperColor.g, paperColor.b, 0.25f);
                return true;
            }
            return false;
        }

        static AreaData LoadCSV(TextAsset asset)
        {
            string[] lines = asset.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            AreaData areaData = new AreaData();
            areaData.entities = new List<EntityData>();

            bool isAreaDataProcessed = false;

            foreach (string line in lines)
            {
                string[] columns = line.Split(',');

                if (!isAreaDataProcessed)
                {
                    areaData.areaID = int.Parse(columns[0]);
                    areaData.mapID = int.Parse(columns[1]);
                    areaData.cameraTargetPos = new Vector3(float.Parse(columns[2]), float.Parse(columns[3]), float.Parse(columns[4]));
                    areaData.camTargetAngle = new Vector3(float.Parse(columns[5]), float.Parse(columns[6]), float.Parse(columns[7]));
                    areaData.camOffset = new Vector3(float.Parse(columns[8]), float.Parse(columns[9]), float.Parse(columns[10]));
                    isAreaDataProcessed = true;
                }
                else
                {
                    EntityData entity = new EntityData();
                    entity.animID = int.Parse(columns[0]);
                    entity.position = new Vector3(float.Parse(columns[1]), float.Parse(columns[2]), float.Parse(columns[3]));
                    entity.progress = int.Parse(columns[4]);
                    entity.flip = bool.Parse(columns[5]);
                    entity.beforeProgress = bool.Parse(columns[6]);
                    entity.name = columns.Length > 7 ? columns[7] : "";
                    areaData.entities.Add(entity);
                }
            }
            return areaData;
        }

        static void DestroyAll(Type type,GameObject obj)
        {
            Component[] objects = obj.transform.GetComponentsInChildren(type, true);
            foreach (Component component in objects)
            {
                UnityEngine.Object.Destroy(component);
            }
        }

        static int GetLastSavedFile()
        {
            int latestFile = -1;
            DateTime latestTime = DateTime.MinValue;
            for(int i = 0; i < 3; i++)
            {
                string filePath = "save" + i + ".dat";
                if (File.Exists(filePath))
                {
                    DateTime lastWriteTime = File.GetLastWriteTime(filePath);
                    if (lastWriteTime > latestTime)
                    {
                        latestTime = lastWriteTime;
                        latestFile = i;
                    }
                }
            }
            return latestFile;
        }
        
        static GameObject SetupBackgroundMap(StartMenu __instance, AreaData areaData, int progress)
        {
            GameObject map;
            if (Enum.IsDefined(typeof(NewMaps), areaData.mapID))
            {
                NewMaps newMap = (NewMaps)areaData.mapID;
                MapFactory.CreateMap(newMap).LoadMap(newMap);
                map = MainManager.map.gameObject;
            }
            else
            {
                map = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Maps/" + ((MainManager.Maps)areaData.mapID).ToString())) as GameObject;
            }

            MapControl mapControl = map.GetComponent<MapControl>();
            RenderSettings.skybox = mapControl.skyboxmat;
            RenderSettings.fogColor = mapControl.fogcolor;
            RenderSettings.fogEndDistance = mapControl.fogend;
            RenderSettings.ambientSkyColor = mapControl.skycolor;
            RenderSettings.ambientLight = mapControl.globallight;
            UnityEngine.Object.Destroy(mapControl);

            Type[] toDestroy = { typeof(Hazards), typeof(AudioSource), typeof(SoundControl), typeof(ScrewPlatform), typeof(DeadLanderOmega), typeof(ConditionChecker), typeof(SpriteBounce) };

            foreach (Type type in toDestroy)
            {
                DestroyAll(type, map.gameObject);
            }
            DoMapSpecific(__instance, areaData, progress, map);
            return map;
        }

        static void SetupBackgroundMapEntities(StartMenu __instance, AreaData areaData, int progress)
        {
            List<EntityControl> entities = new List<EntityControl>();
            foreach (var e in areaData.entities)
            {
                bool canExist;

                if (e.beforeProgress)
                {
                    canExist = progress < e.progress;
                }
                else
                {
                    canExist = e.progress == -1 || progress >= e.progress;
                }

                if (canExist)
                {
                    string name = e.name == "" ? ((MainManager.AnimIDs)e.animID + 1).ToString() : e.name;
                    EntityControl entity = EntityControl.CreateNewEntity("Fixed" + name, e.animID, e.position);
                    entity.transform.parent = __instance.model;
                    entity.flip = e.flip;
                    entities.Add(entity);
                }
            }

            __instance.entities = entities.ToArray();
            __instance.entitycd = new float[entities.Count];
        }

        static void DoMapSpecific(StartMenu __instance, AreaData areaData, int progress, GameObject map)
        {
            if (areaData.mapID == (int)MainManager.Maps.AntPalace2)
            {
                Transform[] antPalaces = new Transform[3];
                string[] names = new string[] { "Base", "Base (1)", "Base (2)" };

                for (int i = 0; i < antPalaces.Length; i++)
                {
                    antPalaces[i] = map.transform.Find(names[i]);
                    antPalaces[i].gameObject.SetActive(false);
                }

                int palaceIndex = 0;
                if (progress == 6 || progress == 5)
                {
                    palaceIndex = 1;
                }

                if (progress > 6)
                {
                    palaceIndex = 2;
                }

                antPalaces[palaceIndex].gameObject.SetActive(true);
                antPalaces[palaceIndex].transform.position = Vector3.zero;
            }else if (areaData.areaID == (int)MainManager.Areas.GiantLair)
            {
                Transform eye = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Objects/Eye")) as GameObject).transform;
                eye.parent = map.transform;
                eye.transform.localPosition = Vector3.zero;
                eye.transform.localScale = Vector3.one * 2f;

                Renderer[] componentsInChildren = eye.GetChild(2).GetComponentsInChildren<Renderer>();
                for (int k = 0; k < componentsInChildren.Length; k++)
                {
                    componentsInChildren[k].material.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, componentsInChildren[k].material.color.a);
                }

                eye.position = new Vector3(48.7f, 15.3f, 16);
                eye.rotation = Quaternion.Euler(317, 149, 180);
            } 
            else if (areaData.mapID == (int)MainManager.Maps.CaveOfTrials)
            {
                map.transform.Find("Base").Find("Pillar").gameObject.SetActive(false);
            } 
            else if (areaData.mapID == (int)MainManager.Maps.BugariaPier)
            {
                if (progress >= 5)
                {
                    map.transform.Find("Base").Find("Boat").gameObject.SetActive(false);
                }
            }
            else if(areaData.mapID == (int)NewMaps.SandCastleDepthsMain)
            {
                Shader.SetGlobalFloat("GlobalIceRadius", 1000 / 2f);
                Shader.SetGlobalVector("CentralIcePoint", Vector3.zero);
            }else if(areaData.mapID == (int)MainManager.Maps.BugariaMainPlaza)
            {
                if (progress == 5)
                {
                    map.transform.Find("Model").Find("Statue").gameObject.SetActive(false);
                }
                else
                {
                    map.transform.Find("Model").Find("brokenstuff").gameObject.SetActive(false);
                }
            }
        }
    }

    [HarmonyPatch(typeof(StartMenu), "GetArea")]
    public class PatchStartMenuGetArea
    {
        static void Postfix(StartMenu __instance, MainManager.LoadData t, ref string __result)
        {
            __result = MainManager_Ext.GetNewAreaNames(__result, t.mapid);
        }
    }

}
