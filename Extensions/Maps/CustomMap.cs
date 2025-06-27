using BFPlus.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MainManager;
using UnityEngine;
using static UnityEngine.Object;
using Steamworks;
using BFPlus.Extensions.Maps.PitMaps;
using BFPlus.Extensions.Maps.NewPowerPlant;
using BFPlus.Extensions.Maps.SandCastleDepths;
using BFPlus.Extensions.Maps.DeepCave;
using BFPlus.Extensions.Maps.AbandonedTower;
using BFPlus.Extensions.Maps.GiantLairPlayroom;
using MonoMod.RuntimeDetour;

namespace BFPlus.Extensions.Maps
{
    public abstract class CustomMap
    {
        protected MapControl map;

        public void LoadMap(NewMaps mapID)
        {
            MainManager_Ext.LoadMapsBundle();
            GameObject gameObject = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset(mapID.ToString())) as GameObject;
            gameObject.SetActive(false);
            map = gameObject.AddComponent<MapControl>();
            gameObject.name = ((int)mapID).ToString();
            map.mapid = (MainManager.Maps)mapID;
            map.actualcenter = Vector3.zero;
            map.centralpoint = Vector3.zero;
            map.mapflags = new bool[0];
            map.entities = new EntityControl[0];
            map.insides = new GameObject[0];
            map.insidetypes = new MapControl.InsideType[0];
            map.tempfollowers = new List<EntityControl>();
            map.discoveryids = new int[0];
            map.autoevent = new Vector2[0];
            map.music = new AudioClip[1];
            map.musicflags = new Vector2Int[0];
            map.musicid = 0;

            LoadCustomData();

            map.gameObject.SetActive(true);
            MainManager.map = map;
            MainManager_Ext.UnloadMapsBundle();
        }

        public abstract GameObject LoadBattleMap();


        protected abstract void LoadCustomData();

        protected void SetBaseMapData(Color globalLight, Vector3 camOffset, Vector3 camAngle, MainManager.Areas areaID, AudioClip music)
        {
            map.music[0] = music;
            map.areaid = areaID;
            map.camangle = camAngle;
            map.globallight = globalLight;
            map.camoffset = Vector3.zero;
        }

        public static void AddCorrectMaterials(Transform parent)
        {
            foreach (Transform transform in parent)
            {
                var mr = transform.gameObject.GetComponent<MeshRenderer>();
                var sr = transform.gameObject.GetComponent<SpriteRenderer>();
                if (mr != null)
                {
                    if (mr.material != null && mr.material.name.Contains("MainPlane"))
                    {
                        mr.material = MainManager.mainPlane;
                    }
                    else
                    {
                        if(mr.material != null && mr.material.name.Contains("3DMain"))
                        {
                            mr.materials = new Material[] { MainManager.Main3D, MainManager.outlinemain };
                        }
                    }
                }

                if (sr != null)
                {
                    sr.material = MainManager.spritemat;
                }
                AddCorrectMaterials(transform);
            }
        }

        protected ScrewPlatform AddScrewPlatform(GameObject gameObject, Vector3 target, int[] linkEntities, Vector3 shakeWhenMoving, string soundActive, ScrewPlatform.Type type, bool or, bool nonscrewswitch, float timerActive=60f)
        {
            ScrewPlatform screwPlat = gameObject.AddComponent<ScrewPlatform>();
            screwPlat.target = target;
            screwPlat.linkedentities = linkEntities;
            screwPlat.shakewhenmoving = shakeWhenMoving;
            screwPlat.soundonactive = soundActive;
            screwPlat.type = type;
            screwPlat.or = or;
            screwPlat.nonscrewswitch = nonscrewswitch;
            screwPlat.timeractive = timerActive;
            return screwPlat;
        }

        protected FaderRange AddFaderRange(GameObject gameObject, Vector3 pivot, float minDistance, float maxDistance, bool player, float fadeDelay, float fadePercent)
        {
            FaderRange fader = gameObject.gameObject.AddComponent<FaderRange>();
            gameObject.GetComponent<MeshRenderer>().materials = new Material[] { MainManager.Fade3D, MainManager.outlinemain };
            fader.pivot = pivot;
            fader.mindistance = minDistance;
            fader.player = player;
            fader.maxdistance = maxDistance;
            fader.color = Color.gray;
            fader.fadedelay = fadeDelay;
            fader.fadepercent = fadePercent;
            return fader;
        }

        protected void CheckDiscovery(NewDiscoveries discovery)
        {
            if (MainManager.instance != null && MainManager.player != null && MainManager.instance.librarystuff != null && !MainManager.instance.librarystuff[0, (int)discovery])
            {
                MainManager.UpdateJounal(MainManager.Library.Discovery, (int)discovery);
            }
        }
    }


    public static class MapFactory
    {
        private static readonly Dictionary<NewMaps, Type> MapTypeToClass = new Dictionary<NewMaps, Type>
        {
            { NewMaps.PitBossRoom, typeof(PitBossRoomMap) },
            { NewMaps.Pit100BaseRoom, typeof(Pit100BaseRoomMap) },
            { NewMaps.Pit100Reward, typeof(Pit100RewardMap) },
            { NewMaps.SeedlingMinigame, typeof(SeedlingMinigameMap) },
            { NewMaps.AntPalaceTrainingRoom, typeof(AntPalaceTrainingRoomMap) },
            { NewMaps.PowerPlantExtra, typeof(PowerPlantExtra) },
            { NewMaps.PowerPlantBigRoom, typeof(PowerPlantBigRoomMap) },
            { NewMaps.PowerPlantElecPuzzle, typeof(PowerPlantElecPuzzleMap) },
            { NewMaps.PowerPlantBoss, typeof(PowerPlantBossMap) },
            { NewMaps.SandCastleDepths1, typeof(SandCastleDepths1Map) },
            { NewMaps.SandCastleDepthsWall, typeof(SandCastleDepthsWallMap) },
            { NewMaps.SandCastleDepthsIcePuzzle, typeof(SandCastleDepthsIcePuzzleMap) },
            { NewMaps.SandCastleDepthsMain, typeof(SandCastleDepthsMainMap) },
            { NewMaps.SandCastleDepthsBoss, typeof(SandCastleDepthsBossMap) },
            { NewMaps.DeepCaveEntrance, typeof(DeepCaveEntranceMap) },
            { NewMaps.DeepCave1, typeof(DeepCave1Map) },
            { NewMaps.DeepCave2, typeof(DeepCave2Map) },
            { NewMaps.DeepCaveBoss, typeof(DeepCaveBossMap) },
            { NewMaps.AbandonedTower, typeof(AbandonedTowerOutsideMap) },
            { NewMaps.AbandonedTower1, typeof(AbandonedTower1Map) },
            { NewMaps.AbandonedTower2, typeof(AbandonedTower2Map) },
            { NewMaps.AbandonedTower3, typeof(AbandonedTower3Map) },
            { NewMaps.AbandonedTowerBoss, typeof(AbandonedTowerBossMap) },
            { NewMaps.AbandonedTowerCards, typeof(AbandonedTowerCardsMap) },
            { NewMaps.BeehiveMinigame, typeof(BeehiveMinigameMap) },
            { NewMaps.LeafbugVillage, typeof(LeafbugVillageMap) },
            { NewMaps.LeafbugShamanHut, typeof(LeafbugShamanHutMap) },
            { NewMaps.GiantLairPlayroom1, typeof(GiantLairPlayroom1Map) },
            { NewMaps.GiantLairPlayroom2, typeof(GiantLairPlayroom2Map) },
            { NewMaps.GiantLairPlayroom3, typeof(GiantLairPlayroom3Map) },
            { NewMaps.GiantLairPlayroomBoss, typeof(GiantLairPlayroomBossMap) },

        };

        public static CustomMap CreateMap(NewMaps mapType)
        {
            if (MapTypeToClass.TryGetValue(mapType, out var mapClassType))
            {
                return (CustomMap)Activator.CreateInstance(mapClassType);
            }
            throw new ArgumentException($"No map class found for map type {mapType}");
        }
    }

}
