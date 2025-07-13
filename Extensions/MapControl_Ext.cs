using BFPlus.Extensions.Events;
using BFPlus.Extensions.Maps;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace BFPlus.Extensions
{
    public class NewNpc
    {
        public string data;
        public string name;
        public int index;
        public NewNpc(string data, string name, int index)
        {
            this.data = data;
            this.name = name;
            this.index = index;
        }
    }
    class MapControl_Ext : MonoBehaviour
    {
        public static List<NewNpc> npcToAdd = new List<NewNpc>();
        static string[] entitiesData;
        static string[] entitiesName;
        public static void ChangeEntities()
        {
            List<string> tempListData = new List<string>();
            List<string> tempListName = new List<string>();
            entitiesData = new string[0];
            entitiesName = new string[0];
            if (MainManager_Ext.IsValidMap() && MainManager.map != null)
            {
                int mapId = (int)MainManager.map.mapid;
                if ((NewMaps)mapId == NewMaps.Pit100BaseRoom)
                {
                    PitData.GetPitData().GetPitFloorEntities(tempListData, tempListName);
                }
                else
                {
                    string[] data;
                    string[] names;
                    if (mapId > (int)NewMaps.Pit100BaseRoom)
                    {
                        string mapName = ((NewMaps)mapId).ToString();
                        data = MainManager_Ext.assetBundle.LoadAsset<TextAsset>(mapName + "Data").ToString().Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        names = MainManager_Ext.assetBundle.LoadAsset<TextAsset>(mapName + "Names").ToString().Split(new string[] { "\r\n", "\r", "\n" },StringSplitOptions.RemoveEmptyEntries);
                    }
                    else
                    {
                        if(MainManager.map.readdatafromothermap != MainManager.Maps.TestRoom)
                        {
                            mapId = (int)MainManager.map.readdatafromothermap;
                        }
                        
                        data = Resources.Load<TextAsset>("Data/EntityData/" + mapId).ToString().Split('\n');
                        names = Resources.Load<TextAsset>("Data/EntityData/Names/" + mapId + "names").ToString().Split('\n');
                    }

                    tempListData.AddRange(data);
                    tempListName.AddRange(names);

                    tempListData.Remove("");
                    tempListName.Remove("");

                    foreach (var npc in npcToAdd)
                    {
                        if (npc.index != -1)
                        {
                            tempListData.RemoveAt(npc.index);
                            tempListName.RemoveAt(npc.index);
                            tempListData.Insert(npc.index, npc.data);
                            tempListName.Insert(npc.index, npc.name);
                        }
                        else
                        {
                            tempListData.Add(npc.data);
                            tempListName.Add(npc.name);
                        }
                    }
                }

                tempListData.Add("");
                tempListName.Add("");

                entitiesData = tempListData.ToArray();
                entitiesName = tempListName.ToArray();
            }
        }

        public static void CheckIntermissionEntities(MapControl __instance)
        {
            switch (__instance.mapid)
            {
                case MainManager.Maps.GiantLairDeadLands1:
                    __instance.entities[7].gameObject.SetActive(false);
                    __instance.entities[0].GetComponent<BoxCollider>().isTrigger = false;
                    break;

                case MainManager.Maps.GiantLairEntrance:
                    __instance.entities[0].gameObject.SetActive(false);
                    __instance.entities[3].gameObject.SetActive(false);
                    __instance.entities[4].gameObject.SetActive(false);
                    __instance.entities[5].gameObject.SetActive(false);
                    break;

                case MainManager.Maps.WaspKingdomMainHall:
                    DesactivateEntities(__instance, 17);
                    break;
                case MainManager.Maps.WaspKingdomPrison:
                    DesactivateEntities(__instance, 12);
                    break;
                case MainManager.Maps.WaspKingdomJayde:
                    __instance.autoevent = new Vector2[0];
                    DesactivateEntities(__instance, 3);
                    break;

                case MainManager.Maps.WaspKingdomThrone:
                    DesactivateEntities(__instance, 13);
                    break;

                case MainManager.Maps.WaspKingdom3:
                    DesactivateEntities(__instance, 8);
                    break;

                case MainManager.Maps.FarGrasslandsLake:
                    DesactivateEntities(__instance, __instance.entities.Length);
                    break;

                case MainManager.Maps.WaspKingdom5:
                    DesactivateEntities(__instance, 9);
                    NPCControl loadzoneLeft = __instance.entities[0].npcdata;
                    loadzoneLeft.data[0] = (int)MainManager.Maps.WaspKingdom2;
                    loadzoneLeft.data[3] = 0;
                    loadzoneLeft.vectordata[1] = new Vector3(18.82f, 6, 30);
                    loadzoneLeft.vectordata[2] = new Vector3(13.97f, 6, 30);
                    break;

                case MainManager.Maps.WaspKingdom2:
                    DesactivateEntities(__instance, 10);

                    NPCControl lzRight = __instance.entities[8].npcdata;
                    lzRight.data[0] = (int)MainManager.Maps.WaspKingdom5;
                    lzRight.vectordata[1] = new Vector3(-18.9f, 0, 0);
                    lzRight.vectordata[2] = new Vector3(-14.89f, 0, 0);
                    break;

                case MainManager.Maps.BugariaPier:
                    DesactivateEntities(__instance, 28, new int[] { 3,4,6,13,23,26 });

                    //discovery boat thing
                    __instance.entities[26].npcdata.dialogues = new Vector3[] { new Vector3(-1, 64, 0) };
                    //waitress
                    __instance.entities[23].npcdata.dialogues = new Vector3[] { new Vector3(-1, 77, 0) };

                    //sailor girl
                    __instance.entities[6].npcdata.dialogues = new Vector3[] { new Vector3(-1, 91, 0) };

                    //sign price trip
                    __instance.entities[3].npcdata.dialogues = new Vector3[] { new Vector3(-1, 94, 0) };
                    break;

                case MainManager.Maps.MetalIsland1:
                    DesactivateEntities(__instance, 39, new int[] { 11,22,28,34,35 });
                    __instance.entities[4].GetComponent<BoxCollider>().isTrigger = false;

                    //old sailor
                    __instance.entities[22].npcdata.dialogues = new Vector3[] { new Vector3(-1, 112, 0), new Vector3(941, 120, 0) };

                    //tanjy seller
                    __instance.entities[11].npcdata.dialogues = new Vector3[] { new Vector3(-1, 132) };

                    break;


                case MainManager.Maps.WaspKingdomOutside:
                    DesactivateEntities(__instance, __instance.entities.Length);
                    __instance.entities[17].GetComponent<BoxCollider>().isTrigger = false;
                    break;
            }
        }

        static void DesactivateEntities(MapControl __instance, int maxEntity, int[] ignores=null)
        {
            for (int i = 0; i < maxEntity; i++)
            {
                if (__instance.entities[i].npcdata.entitytype == NPCControl.NPCType.Object && (__instance.entities[i].npcdata.objecttype == NPCControl.ObjectTypes.DoorOtherMap || __instance.entities[i].npcdata.objecttype == NPCControl.ObjectTypes.CameraChange || __instance.entities[i].npcdata.objecttype == NPCControl.ObjectTypes.DoorSameMap))
                    continue;

                if (ignores != null && ignores.Contains(i))
                {
                    continue;
                }
                /*__instance.entities[i].gameObject.SetActive(false);
                __instance.entities[i].transform.position = new Vector3(0, -1000);
                __instance.entities[i].startpos = new Vector3(0, -1000);*/
                Destroy(__instance.entities[i].gameObject);
            }
        }

        static void ChangeCollider(GameObject obj)
        {
            if(obj.GetComponent<Collider>() != null)
            {
                Destroy(obj.GetComponent<Collider>());
            }
            obj.AddComponent<MeshCollider>();
        }

        public static void ChangeWaspKingdomOutside(MapControl __instance)
        {
            if (MainManager.instance.flags[916])
            {
                Transform baseObject = __instance.transform.GetChild(0);

                GameObject canTent = baseObject.Find("CanTent").gameObject;

                CreateCloneObj(canTent, baseObject.transform, new Vector3(-8.5f, 0, -5.74f), new Vector3(0,0,0), Vector3.one);
                CreateCloneObj(canTent, baseObject.transform, new Vector3(8.5f, 0, -5.52f), new Vector3(0, 0, 180), Vector3.one);

                baseObject.Find("gate").gameObject.SetActive(false);
                baseObject.Find("gate (1)").gameObject.SetActive(false);

            }
        }

        public static void ChangeWaspKingdomThrone(MapControl __instance)
        {
            if (MainManager.instance.flags[945])
            {
                __instance.music[0] = Resources.Load<AudioClip>("audio/music/Tension3"); 
               __instance.musicflags = new Vector2Int[] { new Vector2Int(945,0) };
            }
        }

        public static void ChangeBugariaPier(MapControl __instance)
        {
            if (MainManager.instance.flags[916])
            {
                Transform boat = __instance.mainmesh.Find("Boat");
                Destroy(boat.gameObject.GetComponent<ConditionChecker>());
            }
        }


        public static void ChangeFarGrasslandsLake(MapControl __instance)
        {
            if (MainManager.instance.flags[916])
            {
                Transform baseObject = __instance.transform.GetChild(0);

                baseObject.Find("grate").gameObject.SetActive(false);
            }
        }

        public static void ChangeWaspKingdom2(MapControl __instance)
        {
            if (MainManager.instance.flags[916])
            {
                Transform baseObject = __instance.transform.GetChild(0);

                string[] objectsToDelete = new string[] { "Rock", "Pillar", "Stone", "trunk" };

                foreach (Transform child in baseObject.Cast<Transform>().Where(t => objectsToDelete.Any(obj => t.name.Contains(obj))))
                {
                    Destroy(child.gameObject);
                }

                GameObject waspHouseOpen = baseObject.Find("WaspHouseHollow").gameObject;
                Mesh openHouseMesh = waspHouseOpen.GetComponent<MeshFilter>().mesh;

                GameObject waspHouseBroken1 = baseObject.Find("WaspHouseBroken (1)").gameObject;
                waspHouseBroken1.GetComponent<MeshFilter>().mesh = openHouseMesh;
                waspHouseBroken1.transform.localEulerAngles = new Vector3(0, 0, 21);
                ChangeCollider(waspHouseBroken1);

                GameObject waspHouse = baseObject.Find("WaspHouse").gameObject;
                Mesh closeHouseMesh = waspHouse.GetComponent<MeshFilter>().mesh;

                GameObject waspHouseBroken = baseObject.Find("WaspHouseBroken").gameObject;
                waspHouseBroken.GetComponent<MeshFilter>().mesh = closeHouseMesh;

                GameObject hoaxeChanges = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("HoaxeWaspKingdom2Changes"),Vector3.zero,Quaternion.Euler(270,0,0),baseObject.transform) as GameObject;

                baseObject.Find("Lock").gameObject.SetActive(false);

            }
        }

        public static void ChangeWaspKingdom5(MapControl __instance)
        {
            if (MainManager.instance.flags[916])
            {
                Transform baseObject = __instance.transform.GetChild(0);

                GameObject crate = baseObject.Find("Crate").gameObject;
                CreateCloneObj(crate, baseObject, new Vector3(7.22f, 3.39f, 10.8f), new Vector3(0, 0, 350), crate.transform.localScale);

                GameObject rock = baseObject.Find("MossyRoundRock").gameObject;
                GameObject newRock = CreateCloneObj(rock, baseObject, new Vector3(16.48f, 0, -0.12f), new Vector3(0, 0, 0), new Vector3(92, 100, 128));
                ResetCollider(newRock);

                SpriteRenderer book = new GameObject("bookHoaxe").AddComponent<SpriteRenderer>();
                book.sprite = MainManager.itemsprites[0, (int)MainManager.Items.PrisonBookA];
                book.material = MainManager.spritemat;
                book.transform.parent = __instance.transform;
                book.transform.position = new Vector3(-8.51f, 1.4f, 8.72f);

                baseObject.Find("Lock").gameObject.SetActive(false);
            }
        }

        public static void ChangeWaspKingdomDrillRoom(MapControl __instance)
        {
            if (MainManager.instance.flags[916])
            {
                Transform baseObject = __instance.transform.GetChild(0);

                GameObject crate = baseObject.Find("UltimaxTank").gameObject;
                CreateCloneObj(crate, baseObject, new Vector3(-8.72f, 2, 4.8f), new Vector3(0, 270, 270), crate.transform.localScale);
            }
        }

        public static void ChangeWaspKingdomPrison(MapControl __instance)
        {
            if (MainManager.instance.flags[916])
            {
                GameObject newRoom = MainManager_Ext.mapPrefabs.LoadAsset("NewWaspKingdomPrison") as GameObject;

                Mesh floorMesh = newRoom.transform.GetComponent<MeshFilter>().mesh;
                __instance.mainmesh.GetComponent<MeshFilter>().mesh = floorMesh;
                __instance.mainmesh.GetComponent<MeshCollider>().sharedMesh = floorMesh;

                Transform door = __instance.mainmesh.Find("door");
                door.position = new Vector3(-26.4f, 0f,-2.2f);
                door.localEulerAngles = new Vector3(0,0,270);
            }
        }

        public static void ChangeWaspKingdomMainHall(MapControl __instance)
        {
            if (MainManager.instance.flags[916])
            {
                GameObject newRoom = MainManager_Ext.mapPrefabs.LoadAsset("NewWaspKingdomMainHall") as GameObject;

                Mesh floorMesh = newRoom.transform.GetComponent<MeshFilter>().mesh;
                __instance.mainmesh.GetComponent<MeshFilter>().mesh = floorMesh;
                __instance.mainmesh.GetComponent<MeshCollider>().sharedMesh = floorMesh;

                __instance.mainmesh.Find("door (1)").gameObject.SetActive(false);
                __instance.mainmesh.Find("door").gameObject.SetActive(false);


                Transform gate = __instance.mainmesh.Find("gate");
                Destroy(gate.gameObject.GetComponent<ConditionChecker>());

                if (MainManager.instance.flags[945])
                {
                    CreateCloneObj(gate.gameObject, __instance.mainmesh, new Vector3(-21.66f, -0.32f, 7.24f), new Vector3(0,0,90), gate.transform.localScale);
                    Destroy(gate.gameObject);
                }

                __instance.camlimitneg = new Vector3(-15, __instance.camlimitneg.y,__instance.camlimitneg.z);

                if (!MainManager.instance.flags[945])
                {
                    GameObject drillSound = new GameObject("drillSound");
                    drillSound.transform.parent = MainManager.map.transform;
                    drillSound.transform.position = new Vector3(16.218f, 0, 5.0415f);

                    AudioSource source = drillSound.AddComponent<AudioSource>();
                    source.clip = Resources.Load<AudioClip>("audio/sounds/WaspDrill");
                    source.loop = true;
                    source.playOnAwake = true;
                    source.minDistance = 5;
                    source.maxDistance = 15;
                    source.rolloffMode = AudioRolloffMode.Linear;
                    source.spatialBlend = 0.868f;
                    source.priority = 128;
                    source.volume = 0;
                    source.Play();
                    drillSound.AddComponent<SoundControl>().startvolume = 0.4f;
                }

                if (MainManager.instance.flags[945])
                    __instance.musicflags = new Vector2Int[] { };
                else
                    __instance.musicflags = new Vector2Int[] { new Vector2Int(916, 1) };

            }
        }

        public static void ChangeGiantLairBeforeBoss2(MapControl __instance)
        {
            GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;
            Destroy(baseObject.transform.Find("tomcan").gameObject);
            baseObject.transform.Find("tomcan (1)").transform.position = new Vector3(2.76f, -0.41f, 0.11f);

            string[] resetObj = { "tomcan (1)", "MedicineBox", "MedicineBox (1)", "GlassPot" };
            foreach(var obj in resetObj)
            {
                ResetCollider(baseObject.transform.Find(obj).gameObject);
            }
            Transform medBox = baseObject.transform.Find("MedicineBox (1)");
            medBox.localScale = new Vector3(213.87f, 221.79f, 169.62f);
            medBox.localEulerAngles = new Vector3(0, 0, 94f);
            medBox.position = new Vector3(3.23f, 5.18f, 7.52f);

            Transform marker = baseObject.transform.Find("Marker");
            Destroy(marker.GetComponent<BoxCollider>());
            marker.gameObject.AddComponent<MeshCollider>();

            Transform glassPot = baseObject.transform.Find("GlassPot");
            glassPot.localScale = new Vector3(1.64f, 1.67f, 1.95f);
            glassPot.localEulerAngles = new Vector3(0, 0, 12.69f);
            glassPot.position = new Vector3(11.87f, -0.37f, 6.34f);

            GameObject newRoom = MainManager_Ext.mapPrefabs.LoadAsset("newGiantLairBeforeBoss2") as GameObject;
            Mesh mesh = newRoom.transform.GetComponent<MeshFilter>().mesh;
            baseObject.GetComponent<MeshFilter>().mesh = mesh;
            baseObject.GetComponent<MeshCollider>().sharedMesh = mesh;

            __instance.camlimitpos = new Vector3(__instance.camlimitpos.x, 8, __instance.camlimitpos.z);
        }

        static Collider ResetCollider(GameObject obj)
        {
            Collider col = obj.GetComponent<Collider>();
            Type colliderType = col.GetType();
            Destroy(col);
            return obj.AddComponent(colliderType) as Collider;
        }


        public static void ChangeGiantLairDeadLands2(MapControl __instance)
        {
            GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;
            Destroy(baseObject.transform.Find("Gear (4)").GetComponent<MeshCollider>());
            Destroy(baseObject.transform.Find("trunk (4)").GetComponent<MeshCollider>());

            string[] objectsName = { "Bottle", "trunk", "Gear" };
            GameObject[] objects = new GameObject[objectsName.Length];
            for(int i=0;i<objects.Length; i++)
            {
                objects[i] = baseObject.transform.Find(objectsName[i]).gameObject;
                Destroy(objects[i].GetComponent<MeshCollider>());
                objects[i].AddComponent<MeshCollider>();
            }

            var boxCol = objects[0].GetComponent<BoxCollider>();
            boxCol.center = new Vector3(boxCol.center.x, boxCol.center.y, 2.5f);

            foreach(Transform child in baseObject.transform)
            {
                if(child.name == "trunk" && child.GetComponents<BoxCollider>().Length == 2)
                {
                    var boxCols = child.GetComponents<BoxCollider>();
                    if(boxCols.Length == 2)
                    {
                        foreach (var col in boxCols)
                        {
                            Destroy(col);
                        }
                    }
                }
            }
        }
        public static void ChangeBeehiveThroneRoom(MapControl __instance)
        {
            GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;

            GameObject temp = Resources.Load("prefabs/maps/JaunesGallery") as GameObject;
            GameObject catalystFrame = CreateCatalystPainting(temp.transform.Find("Base").Find("E1Painting (1)"), baseObject.transform, new Vector3(-14.4f, 5.12f, 4.8f), new Vector3(0, 0, 133));

            ConditionChecker checker = catalystFrame.AddComponent<ConditionChecker>();
            checker.requires = new int[] { 891 };
        }

        public static void ChangeJaunesGallery(MapControl __instance)
        {
            GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;
            CreateCatalystPainting(baseObject.transform.Find("E1Painting (1)"), baseObject.transform, new Vector3(-1.4f, -20f, 3.2f), new Vector3(17.5f, 0, 0));
        }

        static GameObject CreateCatalystPainting(Transform original, Transform parent, Vector3 pos, Vector3 angle)
        {
            GameObject catalystFrame = Instantiate(original).gameObject;
            catalystFrame.name = "CatalystPainting";
            catalystFrame.transform.parent = parent;
            catalystFrame.transform.localScale = new Vector3(0.7f, 0.6f, 0.51f);
            catalystFrame.transform.position = pos;
            catalystFrame.transform.localEulerAngles = angle;

            SpriteRenderer painting = catalystFrame.GetComponentInChildren<SpriteRenderer>();
            painting.sprite = MainManager_Ext.assetBundle.LoadAsset<Sprite>("CatalystPainting");
            painting.transform.localScale = new Vector3(1.72f, 2.55f, 0.71f);
            return catalystFrame;
        }

        public static void ChangeSwamplands4(MapControl __instance)
        {
            GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;

            GameObject plantPlatform = baseObject.transform.Find("PlantPlatform").gameObject;

            (Vector3 position, Vector3 rotation)[] plants = new (Vector3 position, Vector3 rotation)[]
            {
                (new Vector3(34.9f, -6.02f, -7.62f), new Vector3(0, 0, 100)),
                (new Vector3(29.95f, -6f, -7.83f), new Vector3(0, 0, 161)),
                (new Vector3(54.8f, -6.52f, -7.63f), new Vector3(0, 0, 100)),
                (new Vector3(63.87f, -10.42f, -2.24f), new Vector3(0, 0, 132)),
                (new Vector3(58.93f, -9f, 0.41f), new Vector3(0, 0, 90)),
                (new Vector3(56.3f, -7.56f, -3.64f), new Vector3(0, 0, 90))
            };

            for(int i=0; i< plants.Length; i++)
            {
                GameObject plant = Instantiate(plantPlatform);
                plant.transform.parent = baseObject.transform;
                plant.transform.position = plants[i].position;
                plant.transform.localEulerAngles = plants[i].rotation;
            }

        }

        public static void ChangeBarrenLandsEntrance(MapControl __instance)
        {
            if (MainManager.instance.flags[936] && !MainManager.instance.flags[934])
            {
                EventControl_Ext.CreatePattonsIngredients();
            }
        }

        public static void ChangeBarrenLandsMiniboss(MapControl __instance)
        {
            GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;
            GameObject shroom = Instantiate(baseObject.transform.Find("GrayShroom").gameObject);
            shroom.transform.parent = baseObject.transform;
            shroom.transform.position = new Vector3(-14.72f, 0.17f, 7.32f);
            shroom.transform.localEulerAngles = Vector3.zero;
            shroom.transform.localScale = new Vector3(1, 1, 1.5f);
        }

        public static void ChangeFarGrasslandsWizard(MapControl __instance)
        {
            GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;

            Transform tree = baseObject.transform.Find("ThistlePlant");

            GameObject medalTree = Instantiate(tree.gameObject);
            medalTree.transform.parent = baseObject.transform;
            medalTree.transform.position = new Vector3(39.77f, -0.6f, -15.35f);
            medalTree.transform.localEulerAngles = Vector3.zero;
            medalTree.AddComponent<ShakeHorn>();
            ItemTree.CreateItemTree(medalTree.transform, (int)Medal.SturdyStrands, 2, new Vector3(36.7112f, 5, -14.95f), new Vector3(0, 10, 0), 880);
        }

        public static void ChangeHideoutRightA(MapControl __instance)
        {
            GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;

            Transform shopTable = baseObject.transform.Find("Table (3)");
            shopTable.position = new Vector3(37.45f, -0.3f, -0.46f);

            GameObject medalTable = Instantiate(shopTable.gameObject);
            medalTable.transform.parent = baseObject.transform;
            medalTable.transform.position = new Vector3(32.59f, -0.37f, 4.1f);
            medalTable.transform.localEulerAngles = Vector3.zero;
            medalTable.transform.localScale = new Vector3(0.55f, 0.5f, 0.7f);
        }

        public static void ChangeTermiteColiseum1(MapControl __instance)
        {
            GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;

            if (MainManager.instance.flags[871])
            {
                baseObject.transform.Find("extraportraits_2").GetComponent<ConditionChecker>().limit = new int[] { 872 };
                baseObject.transform.Find("extraportraits_2 (1)").GetComponent<ConditionChecker>().requires = new int[] { 872 };
            }
        }

        public static void ChangeHBsLab(MapControl __instance)
        {
            GameObject holoCloakItem = Instantiate(__instance.transform.Find("items0_178").gameObject);
            holoCloakItem.name = "holoClockSprite";
            holoCloakItem.gameObject.GetComponent<ConditionChecker>().limit = new int[] { 865 };
            holoCloakItem.gameObject.GetComponent<SpriteRenderer>().sprite = MainManager.itemsprites[1, (int)MainManager.BadgeTypes.HoloCloak];

            holoCloakItem.transform.parent = __instance.transform;
            holoCloakItem.transform.position = new Vector3(-4.8f, 2.12f, 2.47f);
        }

        public static void ChangeMetalLake(MapControl __instance)
        {
            GameObject newIsland = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("AbandonedTower")) as GameObject;
            GameObject baseObject = newIsland.transform.GetChild(0).gameObject;

            baseObject.transform.SetParent(__instance.transform);
            Destroy(newIsland);

            baseObject.transform.localScale = Vector3.one * 0.06f;
            baseObject.transform.position = new Vector3(16.78f, 0.25f, -6.65f);
            foreach (Transform child in baseObject.transform)
            {
                Destroy(child.gameObject);
            }

        }

        public static void ChangeGiantLairFridgeInside(MapControl __instance)
        {
            GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;

            //Hailstorm stuff
            ResetCollider(baseObject.transform.Find("icecube (3)").gameObject);
            baseObject.transform.Find("Stalagmite (26)").gameObject.AddComponent<BoxCollider>();
        }

        public static void ChangeSandCastleMainRoom(MapControl __instance)
        {
            __instance.mainmesh.Find("SandDune (6)").gameObject.SetActive(false);
        }

        public static void ChangeAntMinesBreakRoom(MapControl __instance)
        {
            //crate with item on  top
            ResetCollider(__instance.mainmesh.GetChild(20).gameObject);         
        }

        public static void ChangeFactoryProcessingPump(MapControl __instance)
        {
            GameObject newPlatform = Instantiate(__instance.transform.Find("GratePlatform (1)").gameObject,__instance.transform);
            
            ScrewPlatform screwComp = newPlatform.GetComponent<ScrewPlatform>();
            screwComp.enabled = false;
            newPlatform.transform.position = new Vector3(-1f, -8f, 11f);
            screwComp.startpos = newPlatform.transform.position;
            screwComp.target = new Vector3(0, 8, 0);
            screwComp.enabled = true;
        }

        public static void ChangeFarGrasslands4(MapControl __instance)
        {
            GameObject newRoom = MainManager_Ext.mapPrefabs.LoadAsset("FarGrasslands4Cave") as GameObject;

            Mesh floorMesh = newRoom.transform.GetComponent<MeshFilter>().mesh;
            __instance.mainmesh.GetComponent<MeshFilter>().mesh = floorMesh;
            __instance.mainmesh.GetComponent<MeshCollider>().sharedMesh = floorMesh;

            Transform spikeBed = __instance.mainmesh.Find("SpikeBed");
            foreach(Transform spike in spikeBed)
            {
                spike.parent = __instance.mainmesh;
            }
            spikeBed.position = new Vector3(-64.93f, -2.4f, 3.8f);

            var rockCol = __instance.mainmesh.Find("PlainRockRound").GetComponent<CapsuleCollider>();
            rockCol.center = new Vector3(rockCol.center.x, rockCol.center.y, 0.69f);

            __instance.camlimitpos = new Vector3(__instance.camlimitpos.x, __instance.camlimitpos.y, 16f);


            Transform tree = __instance.mainmesh.Find("ThistlePlant (7)");
            ItemTree.CreateItemTree(tree, (int)NewItem.SucculentSeed, 0, new Vector3(-15.9f, 7.3f, 22.5f), new Vector3(0, 10, 0), 955);

        }

        public static void ChangePowerPlant(MapControl __instance)
        {
            GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;


            GameObject newRoom = MainManager_Ext.mapPrefabs.LoadAsset("newPowerPlant") as GameObject;

            Mesh floorMesh = newRoom.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
            baseObject.GetComponent<MeshFilter>().mesh = floorMesh;
            baseObject.GetComponent<MeshCollider>().sharedMesh = floorMesh;

            Mesh wallMesh = newRoom.transform.GetChild(0).GetChild(0).GetComponent<MeshFilter>().mesh;
            GameObject wallObject = baseObject.transform.Find("Base.001").gameObject;
            wallObject.GetComponent<MeshFilter>().mesh = wallMesh;
            wallObject.GetComponent<MeshCollider>().sharedMesh = wallMesh;

            Destroy(baseObject.transform.Find("Battery").gameObject);
            Destroy(baseObject.transform.Find("Battery (1)").gameObject);
            Destroy(baseObject.transform.Find("Battery (2)").gameObject);
            __instance.camlimitneg = new Vector3(-27, __instance.camlimitneg.y, __instance.camlimitneg.z);
        }

        public static void ChangeFactoryProcessingFirstRoom(MapControl __instance)
        {
            Transform crate = __instance.gameObject.transform.Find("Crate (3)");
            foreach(var col in crate.GetComponents<Collider>())
            {
                Destroy(col);
            }
            crate.position = new Vector3(-24.3f, -6, 2.16f);
            crate.gameObject.AddComponent<MeshCollider>();
        }

        public static void ChangeBeehiveMainArea(MapControl __instance)
        {
            Transform baseObject = __instance.gameObject.transform.Find("Base").transform;

            Transform stall = baseObject.Find("Stall");

            string[] objsToAddMeshCol = { "Pot (2)", "Crate", "Crate (1)", "Crate (2)", "SupportBeams", "SupportBeams (1)"};

            foreach(var obj in objsToAddMeshCol)
            {
                ChangeCollider(baseObject.Find(obj).gameObject);
            }
            stall.GetChild(0).gameObject.AddComponent<MeshCollider>();

            GameObject crateObj = baseObject.Find("Crate").gameObject;
            GameObject newCrate = Instantiate(crateObj, baseObject);
            newCrate.transform.position = new Vector3(-6f, 2f, 10.33f);
            Destroy(newCrate.GetComponent<BoxCollider>());
            //newCrate.transform.rotation = Quaternion.Euler(Vector3.zero);

            BoxCollider leafBagCol = baseObject.Find("LeafBag").gameObject.GetComponent<BoxCollider>();
            leafBagCol.size = new Vector3(0.0156f, 0.0118f, 0.04f);
            leafBagCol.center = Vector3.zero;


            GameObject newRoom = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("NewBeehiveMainAreaBase")) as GameObject;
            Mesh mesh = newRoom.GetComponent<MeshFilter>().mesh;
            baseObject.GetComponent<MeshFilter>().mesh = mesh;
            baseObject.GetComponent<MeshCollider>().sharedMesh = mesh;

            Mesh wallMesh = newRoom.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
            GameObject baseWall = baseObject.transform.GetChild(0).gameObject;
            baseWall.GetComponent<MeshFilter>().mesh = wallMesh;
            baseWall.GetComponent<MeshCollider>().sharedMesh = wallMesh;

            var wallLz = newRoom.transform.GetChild(0).GetChild(0).gameObject.transform;
            wallLz.gameObject.GetComponent<MeshRenderer>().materials = new Material[] { MainManager.Main3D };
            wallLz.gameObject.AddComponent<MeshCollider>();
            wallLz.parent = baseWall.transform;
            Destroy(newRoom.gameObject);
            var fader = wallLz.gameObject.AddComponent<Fader>();
            fader.fadedistance = -1;
            fader.zdistance = 8;
            fader.fadespeed = 0.075f;
            fader.yoffset = -1;
            fader.checkx = 0;
            fader.pivotoffset = new Vector3(0,0,0);

            //Gourmet Race Loading Zone Decorations
            int[] itemSprites = new int[] { (int)MainManager.Items.GlazedHoney, (int)MainManager.Items.HoneyPancake, (int)MainManager.Items.HoneyMilk };
            Vector3[] itemPos = new Vector3[] { new Vector3(14.86f, 1.5f, 18.9f), new Vector3(16.19f, 1.5f, 18f), new Vector3(17.4f, 1.5f, 17.38f) };
            Material[] spriteMat = null;
            for (int i = 0; i < itemSprites.Length; i++)
            {
                GameObject itemParent = new GameObject("item" + i);
                itemParent.transform.parent = __instance.transform;
                itemParent.transform.localEulerAngles = new Vector3(0, 214.3f, 0);
                itemParent.transform.position = itemPos[i];
                SpriteRenderer item = new GameObject("sprite").AddComponent<SpriteRenderer>();
                item.sprite = MainManager.itemsprites[0, itemSprites[i]];
                item.transform.parent = itemParent.transform;
                item.transform.localEulerAngles = Vector3.zero;
                item.transform.localPosition = Vector3.zero;
                item.gameObject.layer = 14;
                item.receiveShadows = true;
                item.sortingOrder = 1;
                if(spriteMat == null)
                    spriteMat = item.materials;
            }


            GameObject foodSign = Instantiate(baseObject.Find("HCSign")).gameObject;
            foodSign.transform.parent = baseObject.transform;
            foodSign.transform.position = new Vector3(11.84f, 5.32f, 20.9f);
            foodSign.transform.localEulerAngles = new Vector3(0, 0, 200);
            foreach(var sr in foodSign.gameObject.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.sprite = MainManager.itemsprites[0, (int)MainManager.Items.Donut];
                sr.materials = spriteMat;
            }

            GameObject table = Instantiate(baseObject.Find("Table"),baseObject.transform).gameObject;
            table.transform.position = new Vector3(16.2f, 0, 18.18f);
            table.transform.localEulerAngles = new Vector3(0, 0, 37);

            GameObject pillar = Instantiate(baseObject.Find("HexagonPillar")).gameObject;
            pillar.transform.parent = baseObject.transform;
            pillar.transform.position = new Vector3(8.85f, 0, 21.44f);
            pillar.transform.localEulerAngles = Vector3.zero;
        }

        public static void ChangeBeehiveOutside(MapControl __instance)
        {
            GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;

            GameObject branch = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("BeehiveBranch")) as GameObject;
            branch.transform.parent = baseObject.transform;
            branch.transform.localRotation = Quaternion.Euler(0, 0, 0);

            CustomMap.AddCorrectMaterials(branch.transform);

            GameObject newRoom = MainManager_Ext.mapPrefabs.LoadAsset("BeehiveOutsideNew") as GameObject;
            Mesh mesh = newRoom.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
            baseObject.GetComponent<MeshFilter>().mesh = mesh;
            baseObject.GetComponent<MeshCollider>().sharedMesh = mesh;

            Mesh newWallMesh = newRoom.transform.GetChild(0).GetChild(0).GetComponent<MeshFilter>().mesh;
            GameObject baseWalls = baseObject.transform.Find("BaseWalls").gameObject;
            baseWalls.GetComponent<MeshFilter>().mesh = newWallMesh;
            baseWalls.GetComponent<MeshCollider>().sharedMesh = newWallMesh;

            __instance.camlimitneg = new Vector3(__instance.camlimitneg.x, -25, -999);
            __instance.ylimit = -50;
        }

        public static void ChangeSnakemouthBridgeRoom(MapControl __instance)
        {
            GameObject gameObject = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("StonePillarNatural (10)")) as GameObject;
            gameObject.transform.parent = MainManager.map.gameObject.transform;
            GameObject pillarWaterfall = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("pillarWaterfall")) as GameObject;
            pillarWaterfall.transform.parent = MainManager.map.gameObject.transform;
            GameObject waterfalls = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("waterfalls")) as GameObject;
            waterfalls.transform.parent = MainManager.map.gameObject.transform;

            GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;
            baseObject.transform.Find("Cube").gameObject.SetActive(false);
            baseObject.transform.Find("water").gameObject.SetActive(false);
            foreach (Transform t in baseObject.transform.Find("River").transform)
            {
                t.gameObject.SetActive(false);
            };
            GameObject newRoom = MainManager_Ext.mapPrefabs.LoadAsset("SnakemouthBridgeNewMesh") as GameObject;
            Mesh mesh = newRoom.GetComponent<MeshFilter>().mesh;
            baseObject.GetComponent<MeshFilter>().mesh = mesh;
            baseObject.GetComponent<MeshCollider>().sharedMesh = mesh;


            GameObject temp = Resources.Load("prefabs/maps/" + MainManager.Maps.BugariaOutskirtsOutsideCity.ToString()) as GameObject;
            GameObject sign = null;
            foreach(Transform child in temp.transform.GetChild(0))
            {
                if(child.name == "Stick" && child.childCount == 2)
                {
                    sign = child.GetChild(0).gameObject;
                    break;
                }
            }

            ChangeCollider(CreateCloneObj(sign, __instance.transform.GetChild(0), new Vector3(-32.36f, 0.129f, -2.92f), new Vector3(76f, 67.208f, 247f), new Vector3(0.01f, 0.02f, 0.01f)));
        }

        public static void ChangeGoldenSMinigame(MapControl __instance)
        {
            __instance.battlemap = MainManager.BattleMaps.GoldenBattle1;
            
        }

        public static void ChangeTestRoom(MapControl __instance)
        {
            GameObject cube = Instantiate(__instance.transform.Find("Cube").gameObject);
            cube.transform.parent = __instance.transform;
            cube.transform.position = new Vector3(50, 100, 0);
            cube.transform.localScale = new Vector3(25, 1, 35);
                
            Medal[] medals = Enum.GetValues(typeof(Medal)).Cast<Medal>().ToArray();
            NewEnemies[] enemies = Enum.GetValues(typeof(NewEnemies)).Cast<NewEnemies>().Reverse().ToArray();
            NewItem[] items = Enum.GetValues(typeof(NewItem)).Cast<NewItem>().ToArray();


            BoxCollider boxCollider = cube.GetComponent<BoxCollider>();

            float totalWidth = boxCollider.size.x * cube.transform.localScale.x;
            float totalLength = boxCollider.size.z * cube.transform.localScale.z;
            float spacing = 1.5f;

            int columnCount = Mathf.FloorToInt(totalWidth / spacing)-1;
            int rowCount = Mathf.FloorToInt(totalLength / spacing);

            Vector3 startPosition = cube.transform.position - new Vector3((totalWidth / 2)-1, 0, (-totalLength / 2)+1);

            int medalCount = 0;
            int enemyCount = 0;
            int itemCount = 0;

            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < columnCount; col++)
                {
                    Vector3 position = startPosition + new Vector3(col * spacing, 1, -row * spacing);
                    if(medalCount < medals.Length)
                    {
                        if(medals[medalCount] != Medal.TPComa)
                            EntityControl.CreateItem(position, 2, (int)medals[medalCount], Vector3.zero, -1);
                        medalCount++;

                        if (medalCount >= medals.Length)
                        {
                            break;
                        }
                    }
                    else if(itemCount < items.Length)
                    {
                        EntityControl.CreateItem(position, 1, (int)items[itemCount], Vector3.zero, -1);
                        itemCount++;

                        if (itemCount >= items.Length)
                        {
                            row += 1;
                            break;
                        }
                    }
                    else
                    {
                        if (enemyCount < enemies.Length)
                        {
                            int enemy = (int)enemies[enemyCount];
                            int animID = Convert.ToInt32(MainManager.enemydata[enemy, 0]);
                            var entity = EntityControl.CreateNewEntity("enemy" + enemy, animID, position);
                            entity.transform.parent = __instance.transform;
                            if ((NewEnemies)enemy == NewEnemies.Frostfly || (NewEnemies)enemy == NewEnemies.FlyingCaveling || (NewEnemies)enemy == NewEnemies.Dewling)
                                entity.height = 1;
                            enemyCount++;
                            col+=2;                          
                        }
                    }
                }
            }

        }

        public static void ChangeDesertRoachVillage(MapControl __instance)
        {
            GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;

            Transform brokenHouse = baseObject.transform.Find("BrokenHouse");
            Destroy(brokenHouse.gameObject.GetComponent<BoxCollider>());

            Transform rock = baseObject.transform.Find("Big Plain Rock (1)");
            Destroy(rock.gameObject.GetComponent<BoxCollider>());
            rock.gameObject.AddComponent<MeshCollider>();

            baseObject.transform.Find("Big Plain Rock (2)").gameObject.AddComponent<MeshCollider>();
        }

        public static void ChangeAntPalace1(MapControl __instance)
        {
            GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;
            GameObject newRoom = MainManager_Ext.mapPrefabs.LoadAsset("AntPalace1NewMesh") as GameObject;
            Mesh mesh = newRoom.GetComponent<MeshFilter>().mesh;
            baseObject.GetComponent<MeshFilter>().mesh = mesh;
            baseObject.GetComponent<MeshCollider>().sharedMesh = mesh;
        }

        public static void ChangeSandCastleRockRoom(MapControl __instance)
        {
            GameObject newPlatforms = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("SandCastleChange")) as GameObject;
            newPlatforms.transform.parent = MainManager.map.gameObject.transform;
        }

        public static void ChangeCaveOfTrials(MapControl __instance)
        {
            GameObject baseObject = GameObject.Find("Arena (1)").gameObject;
            GameObject platformLight = GameObject.Find("Arena.001").gameObject;

            var mainAsset = MainManager_Ext.mapPrefabs.LoadAsset("CaveOfTrialsPlatform") as GameObject;
            var meshes = mainAsset.GetComponentsInChildren<MeshFilter>(true);

            Mesh newPlatform = meshes.First(m => m.mesh.name == "Arena__1_.001 Instance").mesh;
            baseObject.GetComponent<MeshFilter>().mesh = newPlatform;
            baseObject.GetComponent<MeshCollider>().sharedMesh = newPlatform;

            Mesh newPlatformLight = meshes.First(m => m.mesh.name == "Arena_001.001 Instance").mesh;
            platformLight.GetComponent<MeshFilter>().mesh = newPlatformLight;

            GameObject platform = Instantiate(Resources.Load("Prefabs/Objects/AncientPlatform"), MainManager.map.gameObject.transform, false) as GameObject;
            platform.name = "MainPlatform";
            platform.transform.localScale = new Vector3(1.013f, 1.0146f, 1f);
            platform.transform.localPosition = new Vector3(-0.0823f, 1.5709f, 10.5889f);


            mainAsset = MainManager_Ext.mapPrefabs.LoadAsset("MeshCaveOfTrials") as GameObject;

            baseObject = __instance.gameObject.transform.Find("Base").gameObject;
            var mesh = mainAsset.GetComponent<MeshFilter>().mesh;
            baseObject.GetComponent<MeshFilter>().mesh = mesh;
            baseObject.GetComponent<MeshCollider>().sharedMesh = mesh;

            __instance.canfollowID = __instance.canfollowID.AddItem((int)MainManager.AnimIDs.AntCapitain - 1).ToArray();
        }

        public static void ChangeMetalIsland1(MapControl __instance)
        {
            //remove the cube to access top of hotel
            GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;
            baseObject.transform.Find("Cube (3)").gameObject.SetActive(false);

            //Replace the fence collider with a mesh collider so that its not weird on top of hotel
            GameObject doubleFence = baseObject.transform.Find("DoubleFence").gameObject;
            Destroy(doubleFence.gameObject.GetComponent<BoxCollider>());
            doubleFence.AddComponent<MeshCollider>();

            //Add Collider to top of hotel twig thing, and the flower pot.
            GameObject hotel = baseObject.transform.Find("Hotel").gameObject;
            hotel.transform.Find("Twig2").gameObject.AddComponent<MeshCollider>();
            hotel.transform.Find("PalmTree (3)").gameObject.AddComponent<MeshCollider>();
            hotel.transform.Find("PalmTree (2)").gameObject.AddComponent<MeshCollider>();

            GameObject palmTree = hotel.transform.Find("PalmTree").gameObject;
            Destroy(palmTree.gameObject.GetComponent<CapsuleCollider>());
            palmTree.AddComponent<MeshCollider>();

            GameObject palmTree1 = hotel.transform.Find("PalmTree (1)").gameObject;
            Destroy(palmTree1.gameObject.GetComponent<CapsuleCollider>());
            palmTree1.AddComponent<MeshCollider>();

            //Remove the big colliders that blocks the top of the stall left of the hotel.
            GameObject plainRockRound = baseObject.transform.Find("PlainRockRound").gameObject;
            Destroy(plainRockRound.gameObject.GetComponent<BoxCollider>());
            plainRockRound.AddComponent<MeshCollider>();

            baseObject.transform.Find("Pot").gameObject.AddComponent<MeshCollider>();
            baseObject.transform.Find("MedicineBox").gameObject.AddComponent<MeshCollider>();
            baseObject.transform.Find("Stall (1)").gameObject.AddComponent<MeshCollider>();

            GameObject pot2 = baseObject.transform.Find("Pot2").gameObject;
            Destroy(pot2.gameObject.GetComponent<BoxCollider>());
            pot2.AddComponent<MeshCollider>();

            GameObject crate = baseObject.transform.GetComponentsInChildren<BoxCollider>().Where(b => b.name == "Crate" && Vector3.Distance(b.transform.position, new Vector3(-18.27f, -0.0577f, 10.5f)) < 0.01f).First().gameObject;
            Destroy(crate.gameObject.GetComponent<BoxCollider>());
            crate.AddComponent<MeshCollider>();

            GameObject crate1 = baseObject.transform.GetComponentsInChildren<BoxCollider>().Where(b => b.name == "Crate (1)" && Vector3.Distance(b.transform.position, new Vector3(-15.42f, 0.09f, 18.27f)) < 0.01f).First().gameObject;
            crate1.transform.position = new Vector3(-14.72f, 0.09f, 16.7702f);

            var conveyorSprite = GameObject.FindObjectOfType<ConveyorSprite>();
            conveyorSprite.sprites[1] = MainManager.itemsprites[0, 9];

            ResetCollider(CreateCloneObj(baseObject.transform.Find("Crate").gameObject, baseObject.transform, new Vector3(10.38f, 8.69f, 16.41f), Vector3.zero, new Vector3(0.8f, 0.5f, 0.5f)));

            //in hoaxe intermission
            if (MainManager.instance.flags[916])
            {
                Transform boat = baseObject.transform.Find("Boat (1)");
                Destroy(boat.gameObject.GetComponent<ConditionChecker>());

                Transform chest = null;
                foreach (Transform child in baseObject.transform)
                {
                    if (child.name == "Chest" && child.childCount == 20)
                    {
                        chest = child;
                        break;
                    }
                }

                GameObject teaChest = CreateCloneObj(chest.gameObject, baseObject.transform, new Vector3(12.28f, 0.15f, -22.53f), new Vector3(0, 0, 40), new Vector3(0.7f, 1.3f, 0.8f));

                var teas = teaChest.transform.GetComponentsInChildren<SpriteRenderer>();

                foreach(var tea in teas)
                {
                    tea.sprite = MainManager.itemsprites[0, UnityEngine.Random.Range(0, 2) == 0 ? (int)MainManager.Items.SpicyTea : (int)MainManager.Items.BurlyTea];
                }

                Destroy(baseObject.transform.Find("Chest (2)").gameObject);
                Destroy(baseObject.transform.Find("Chest (3)").gameObject);

                SpriteRenderer crown = MainManager.NewSpriteObject(new Vector3(-26.63f, 0.77f, -6.26f), __instance.transform, Resources.LoadAll<Sprite>("Sprites/Objects/artifacts")[6]);
                crown.name = "crown";
                crown.transform.localScale = Vector3.one * 0.7f;

                GameObject leafBag = CreateCloneObj(__instance.transform.Find("Restaurant").Find("LeafBag").gameObject, baseObject.transform, new Vector3(15.84f, 0.57f, 13.22f), new Vector3(0, 0, 225), new Vector3(130f, 108f, 108f));
                BoxCollider col = leafBag.AddComponent<BoxCollider>();
                col.size = new Vector3(col.size.x, col.size.y,0.02f);

                SpriteRenderer dish = MainManager.NewSpriteObject(leafBag.transform.position + Vector3.up*1.5f, __instance.transform, MainManager.itemsprites[0, (int)MainManager.Items.DangerDish]);
                dish.name = "DangerDish";

                Audience audience = NewEvent.CreateAudience(__instance.transform, 20, Audience.Type.MothAntBeetle, new Vector2(1, 0.1f), new Vector2(5, 2), new Vector3(-12f, 0.0308f, -22f), new Vector3(0, 0, 0));

                GameObject door = __instance.transform.Find("Log House").Find("Door").gameObject;
                Destroy(door.GetComponent<ConditionChecker>());

                CreateCloneObj(door, baseObject.transform, new Vector3(0.94f,-0.30f, 19.40f), Vector3.zero, new Vector3(1, 1, 1f));        
            }
        }

        public static void ChangeBugariaOutskirtsEast1(MapControl __instance)
        { 
            GameObject sideGrass = __instance.mainmesh.transform.GetComponentsInChildren<BoxCollider>().Where(b => b.name == "SideGrass" && Vector3.Distance(b.transform.position, new Vector3(41.98f, -0.13f, -10.97f)) < 0.01f).First().gameObject;
            Destroy(sideGrass.gameObject.GetComponent<BoxCollider>());

            //add ant capitain to follow whitelist
            __instance.canfollowID = __instance.canfollowID.AddItem((int)MainManager.AnimIDs.AntCapitain - 1).ToArray();
        }

        public static void ChangeBugariaOutskirtsEast2(MapControl __instance)
        {
            GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;

            GameObject newRoom = MainManager_Ext.mapPrefabs.LoadAsset("newoutskirtseast2") as GameObject;
            Mesh mesh = newRoom.GetComponent<MeshFilter>().mesh;
            baseObject.GetComponent<MeshFilter>().mesh = mesh;
            baseObject.GetComponent<MeshCollider>().sharedMesh = mesh;

            Mesh wallMesh = newRoom.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
            GameObject baseWall = baseObject.transform.GetChild(0).gameObject;
            baseWall.GetComponent<MeshFilter>().mesh = wallMesh;
            baseWall.GetComponent<MeshCollider>().sharedMesh = wallMesh;

            baseObject.transform.Find("SideGrass").gameObject.SetActive(false);

            __instance.camlimitneg = new Vector3(__instance.camlimitneg.x, 0, -20);
        }

        public static void ChangeBugariaMainPlaza(MapControl __instance)
        {
            BoxCollider[] colliders = __instance.transform.Find("Inn").GetComponents<BoxCollider>();
            colliders[1].size = new Vector3(0.004887934f, 0.1053846f, 0.02585206f);
            colliders[1].center = new Vector3(-0.03441775f, 0.0001384544f, 0.1398795f);

            GameObject model = __instance.transform.Find("Model").gameObject;
            GameObject theater = model.transform.Find("Theather").gameObject;
            Destroy(theater.GetComponent<BoxCollider>());
            var collider1 = theater.AddComponent<BoxCollider>();
            collider1.center = new Vector3(-0.1237779f, -0.08780891f, 0.09433088f);
            collider1.size = new Vector3(0.2863111f, 0.004382211f, 0.0649126f);

            var collider2 = theater.AddComponent<BoxCollider>();
            collider2.center = new Vector3(-0.06785459f, -0.0006006226f, 0.09433157f);
            collider2.size = new Vector3(0.004012824f, 0.1814803f, 0.0649126f);

            var collider3 = theater.AddComponent<BoxCollider>();
            collider3.center = new Vector3(-0.1386649f, 0.08883853f, 0.09433215f);
            collider3.size = new Vector3(0.2565432f, 0.005984651f, 0.0649126f);

            GameObject sideGrass = model.transform.GetComponentsInChildren<BoxCollider>().Where(b => b.name == "TallGrassHQ1 (2)" && Vector3.Distance(b.transform.position, new Vector3(8.03f, 0f, 23.9f)) < 0.01f).First().gameObject;
            var grassCol = sideGrass.GetComponent<BoxCollider>();
            grassCol.size = new Vector3(0.035f, 0.0149f, 0.12f);


            //trophies stuff
            GameObject house = __instance.transform.Find("JuiceHouse").gameObject;

            //Super Boss rush trophy
            GameObject bossRushTrophy = house.transform.Find("BottleCap1 (1)").gameObject;
            GameObject superBossRush = Instantiate(bossRushTrophy.gameObject);
            superBossRush.transform.parent = house.transform;
            superBossRush.transform.localScale = bossRushTrophy.transform.localScale;
            superBossRush.transform.localEulerAngles = bossRushTrophy.transform.localEulerAngles;
            superBossRush.transform.position = new Vector3(-13f, 5.21f, 11f);
            
            ConditionChecker cc = superBossRush.GetComponent<ConditionChecker>();
            cc.requires = new int[] { 900 };

            MaterialColor matColor = superBossRush.GetComponent<MaterialColor>();
            matColor.color = Color.red;

            Sprite[] faces = Resources.LoadAll<Sprite>("Sprites/Objects/pcface");
            SpriteRenderer renderer = superBossRush.GetComponentInChildren<SpriteRenderer>();
            renderer.sprite = faces[4];

            GameObject tanjyTrophy = house.transform.Find("artifacts_6 (2)").gameObject;

            //Mars Trophy
            GameObject marsTrophy = CreateCloneObj(tanjyTrophy, tanjyTrophy.transform.parent, tanjyTrophy.transform.position, tanjyTrophy.transform.localEulerAngles, tanjyTrophy.transform.localScale);
            marsTrophy.GetComponent<ConditionChecker>().requires = new int[] { 229, 857 };
            marsTrophy.GetComponent<SpriteRenderer>().sprite = MainManager.itemsprites[0, 143];

            //Move tanjy trophy
            tanjyTrophy.transform.localPosition = new Vector3(0.0457f, 0.0379f, 0.0668f);
            tanjyTrophy.GetComponent<ConditionChecker>().requires = new int[] { 229, 790 };

            //Dark Team Snek Trophy
            GameObject darkBeerang = Instantiate(Resources.Load<GameObject>("prefabs/objects/BeerangBattle"));
            darkBeerang.transform.parent = house.transform;
            darkBeerang.transform.localEulerAngles = new Vector3(90, 45, 0);
            darkBeerang.transform.position = new Vector3(-10.82f, 4,9.28f);
            cc = darkBeerang.AddComponent<ConditionChecker>();
            cc.requires = new int[] { 229, 763 };
            darkBeerang.GetComponentInChildren<SpriteRenderer>().material.color = Color.black;

            //Mr Tester Trophy
            if (MainManager.instance.flags[954])
            {
                var mrTesterTrophy = MainManager.NewSpriteObject(new Vector3(-0.0204f, 0.0029f, 0), marsTrophy.transform.parent, Resources.LoadAll<Sprite>("sprites/entities/mrtester")[0]);
                mrTesterTrophy.transform.localScale = new Vector3(0.008f, 0.008f, 0.008f);
                mrTesterTrophy.transform.localEulerAngles = marsTrophy.transform.localEulerAngles;
            }


            //move crown to the ground
            house.transform.Find("artifacts_6").position = new Vector3(-16.92f, 1.38f, 5.38f);

            GameObject newCrate = Instantiate(__instance.transform.Find("Inn").Find("Crate").gameObject);
            newCrate.transform.parent = house.transform;
            newCrate.transform.localScale = new Vector3(0.0074f, 0.005f, 0.0055f);
            newCrate.transform.position = new Vector3(-16.92f, 0.48f, 5f);
        }

        public static void ChangeBugariaResidential(MapControl __instance)
        {
            Transform mothhouse = __instance.transform.Find("MothHouse").Find("MothHouse_001");

            var fences = mothhouse.GetComponentsInChildren<BoxCollider>().Where(b => b.name.Contains("Fence") && !b.name.Contains('5') && !b.name.Contains('6') && !b.name.Contains('7'));

            foreach(var fence in fences)
            {
                GameObject go = fence.gameObject;
                Destroy(fence.gameObject.GetComponent<BoxCollider>());
                go.AddComponent<MeshCollider>();
            }
        }

        public static void ChangeSnakemouthUndergrondDoor(MapControl __instance)
        {
            GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;
            GameObject mushroomPlatforms = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("SnakemouthUndergroundParkour")) as GameObject;
            mushroomPlatforms.transform.parent = baseObject.transform;
            mushroomPlatforms.transform.rotation = new Quaternion(0, -0.7072f, -0.707f, 0);
            var lightCone = baseObject.transform.Find("LightCone (2)").gameObject;
            var lightMats = lightCone.GetComponent<MeshRenderer>().materials;

            foreach(Transform t in mushroomPlatforms.transform)
            {
                if(t.name == "LightCone")
                {
                    t.gameObject.AddComponent<LightSorter>();
                    var renderer = t.GetComponent<MeshRenderer>();
                    renderer.materials = lightMats;

                    foreach (Transform lc in t)
                    {
                        lc.gameObject.GetComponent<MeshRenderer>().materials = lightMats;
                    }
                }
            }
        }

        static void AddMeshRenderer(Transform parent)
        {
            foreach(Transform transform in parent)
            {
                var mr = transform.gameObject.AddComponent<MeshRenderer>();
                mr.materials = GameObject.Find("RoachHouse2").GetComponent<MeshRenderer>().materials;
                AddMeshRenderer(transform);
            }
        }

        public static void ChangeGiantLairRoachVillage(MapControl __instance)
        {
            if (MainManager.instance.flags[932])
            {
                
                GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;

                GameObject roachHouse2 = GameObject.Find("RoachHouse2").gameObject;

                var mainAsset = MainManager_Ext.mapPrefabs.LoadAsset("RoachHouse2") as GameObject;
                var meshes = mainAsset.GetComponentsInChildren<MeshFilter>(true);

                Mesh newHouse = meshes.FirstOrDefault(m => m.name == "RoachHouse2").mesh;
                roachHouse2.GetComponent<MeshFilter>().mesh = newHouse;
                roachHouse2.GetComponent<MeshCollider>().sharedMesh = newHouse;

                GameObject walls = roachHouse2.transform.Find("Collider").gameObject;
                Mesh newWalls = meshes.FirstOrDefault(m => m.name == "Collider").mesh;
                walls.GetComponent<MeshFilter>().mesh = newWalls;
                walls.GetComponent<MeshCollider>().sharedMesh = newWalls;

                roachHouse2.transform.localPosition = new Vector3(-46.38f, 0, 8.74f);
                roachHouse2.transform.rotation = new Quaternion(-0.6207058f, -0.3387099f, -0.3387099f, 0.6207058f);
                roachHouse2.transform.Find("BunkBed").gameObject.SetActive(false);

                //remove old chest
                Destroy(baseObject.transform.Find("Chest").gameObject);
                Destroy(baseObject.transform.Find("Cobweb (1)").gameObject);
                Destroy(baseObject.transform.Find("Cobweb (2)").gameObject);

                GameObject fireShop = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("FireShop 1")) as GameObject;
                fireShop.transform.SetParent(roachHouse2.transform);
                fireShop.transform.localPosition = new Vector3(-4.45f, -10.04f, 0.08f);
                fireShop.transform.localRotation = new Quaternion(0.001090228f, -0.000388056f, 0.5489026f, 0.8358855f);

                //flickering issue idfk im done
                AddMeshRenderer(fireShop.transform);

                GameObject medalSign = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("MedalSignpost")) as GameObject;
                medalSign.transform.SetParent(baseObject.transform);
                medalSign.transform.localPosition = new Vector3(-39.83f, -8.419999f, -0.1120014f);
                medalSign.transform.localRotation = Quaternion.Euler(0,0,-180);

            }

        }

        public static void ChangeSwamplands5(MapControl __instance)
        {
            GameObject leafbugWay = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("LeafbugVillageWay")) as GameObject;
            leafbugWay.transform.parent = __instance.transform;
            CustomMap.AddCorrectMaterials(leafbugWay.transform);
        }



        //Add stump for Bouncing Shroom for crystal berry
        public static void ChangeSwamplands2(MapControl __instance)
        {
            GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;
            GameObject newStump = Instantiate(baseObject.transform.Find("Stump").gameObject, baseObject.transform);

            GameObject invisWall = new GameObject("WALL");
            invisWall.transform.parent = baseObject.transform;
            invisWall.transform.position = new Vector3(22.3f, 23.2786f, 0);
            var boxCol = invisWall.AddComponent<BoxCollider>();
            boxCol.size = new Vector3(6.218f, 30.90f, 41.87f);
            boxCol.center = new Vector3(-2.61f, 0.02f, 20.43f);
            newStump.transform.position = new Vector3(8.1865f, 5.8782f, 12.54f);
        }


        public static void ChangeGiantLairDeadLands1(MapControl __instance)
        {
            GameObject baseObject = __instance.gameObject.transform.Find("Base").gameObject;
            GameObject newStonePillar = Instantiate(baseObject.transform.Find("StonePillarNatural").gameObject, baseObject.transform);
            newStonePillar.transform.position = new Vector3(54.78f,-16.93f,12.81f);
            newStonePillar.transform.rotation = Quaternion.Euler(270, 0, 0);

            GameObject eye = new GameObject("eye");
            eye.transform.parent = __instance.transform;
            eye.transform.position = new Vector3(54.87f, 19.97f, 19.95f);
            DeadLanderOmega omega = eye.AddComponent<DeadLanderOmega>();
            omega.enemyentityid = 1;
            omega.thisid = 3;
            omega.extras = new Transform[] { };
            omega.points = new Vector3[] { new Vector3(289.6235f, 4.553f, 0) };

            GameObject deadlanderZoneNightmare = new GameObject("newZone");
            deadlanderZoneNightmare.transform.parent = baseObject.transform;
            deadlanderZoneNightmare.transform.position = new Vector3(52.28f, -4.71f,7.98f);
            deadlanderZoneNightmare.transform.rotation = Quaternion.Euler(270, 90, 0);
            deadlanderZoneNightmare.transform.localScale = new Vector3(3.3f, 25, 40);
            DeadLanderZones zone = deadlanderZoneNightmare.AddComponent<DeadLanderZones>();
            zone.id = 3;
            deadlanderZoneNightmare.AddComponent<BoxCollider>().isTrigger = true;

            GameObject deadlanderZoneNormal = new GameObject("normalZone");
            deadlanderZoneNormal.transform.parent = baseObject.transform;
            deadlanderZoneNormal.transform.position = new Vector3(52.62f, -4.71f, 5.01f);
            deadlanderZoneNormal.transform.rotation = Quaternion.Euler(270, 90, 0);
            deadlanderZoneNormal.transform.localScale = new Vector3(1f, 30, 40);
            DeadLanderZones normalZone = deadlanderZoneNormal.AddComponent<DeadLanderZones>();
            normalZone.id = 1;
            deadlanderZoneNormal.AddComponent<BoxCollider>().isTrigger = true;


            if (MainManager.instance.flags[916])
            {
                __instance.transform.Find("Eye").gameObject.SetActive(false);
                __instance.transform.Find("Eye (1)").gameObject.SetActive(false);
                baseObject.transform.Find("Knife (1)").gameObject.SetActive(false);

                baseObject.transform.Find("Screw (1)").gameObject.SetActive(false);
                baseObject.transform.Find("Screw (2)").gameObject.SetActive(false);

                List<GameObject> resetColliders=  new List<GameObject>();
                GameObject squareStone = CreateCloneObj(baseObject.transform.Find("SquareStone").gameObject, baseObject.transform,
                    new Vector3(34.79f, -1.15f, 3.83f), new Vector3(0, 0, 300), new Vector3(265, 209, 350));
                resetColliders.Add(squareStone);

                GameObject caveRock = CreateCloneObj(baseObject.transform.Find("CaveRock").gameObject, baseObject.transform,
                    new Vector3(31.27f, -0.86f, -4.12f), new Vector3(0, 0, 188), new Vector3(420, 433, 330));
                resetColliders.Add(caveRock);

                GameObject brokenHouseRock = CreateCloneObj(baseObject.transform.Find("BrokenHouseRock (5)").gameObject, baseObject.transform,
                    new Vector3(29.65f, -0.19f, -11.75f), new Vector3(0, 0, 81f), new Vector3(2.14f, 1.61f, 2.51f));
                resetColliders.Add(brokenHouseRock);

                GameObject glass = CreateCloneObj(baseObject.transform.Find("glass (7)").gameObject, baseObject.transform,
                    new Vector3(12.13f, -0.4f, 13.94f), new Vector3(0, 0, 58.8f), Vector3.one);

                CreateCloneObj(glass, baseObject.transform, new Vector3(12.13f, -0.4f, 2.90f), new Vector3(0, 0, 58.8f), Vector3.one);

                foreach (var obj in resetColliders)
                {
                    Destroy(obj.GetComponent<BoxCollider>());
                    obj.AddComponent<BoxCollider>();
                }

                SpriteRenderer mysteryBerry = new GameObject("mysterryBerryHoaxe").AddComponent<SpriteRenderer>();
                mysteryBerry.sprite = MainManager.itemsprites[0, (int)MainManager.Items.Guarana];
                mysteryBerry.material = MainManager.spritemat;
                mysteryBerry.transform.parent = __instance.transform;
                mysteryBerry.transform.position = new Vector3(33.48f, 0.5f, 11.50f);
            }

        }

        static GameObject CreateCloneObj(GameObject original, Transform parent, Vector3 pos, Vector3 rotation, Vector3 scale)
        {
            GameObject clonedObj = Instantiate(original, parent);
            clonedObj.transform.position = pos;
            clonedObj.transform.localEulerAngles = rotation;
            clonedObj.transform.localScale = scale;
            return clonedObj;
        }
    }
}
