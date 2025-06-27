using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Object;

namespace BFPlus.Extensions.Maps.PitMaps
{
    public class Pit100BaseRoomMap : CustomMap
    {
        protected override void LoadCustomData()
        {
            float floorLightDiff = 0.015f * PitData.GetCurrentFloor() / 10;
            Color globalLight = new Color(0.29f - floorLightDiff, 0.43f - floorLightDiff, 0.58f - floorLightDiff, 1 - floorLightDiff);

            string music = PitData.GetPitData().GetCurrentMusic();
            SetBaseMapData(globalLight, Vector3.zero, new Vector3(20, 0, 0), MainManager.Areas.BugariaOutskirts, Resources.Load("Audio/Music/" + music) as AudioClip);
            map.camlimitpos = new Vector3(5, 10, 999);
            map.camlimitneg = new Vector3(-5, 0, 0);

            map.nobattlemusic = true;

            map.fogcolor = map.globallight;
            map.fogend =40f;
            map.battleleafcolor = map.globallight;

            AddCorrectMaterials(map.transform);
            
            GameObject particleGo = new GameObject("spores");
            particleGo.transform.position = new Vector3(0f, 1f, 6.5f);

            MainManager_Ext.CreatePrefabParticle(particleGo, "prefabs/objects/Spore", 30, 0.01f, 250, 120, 0.02f, new Vector3(7.5f, 5f, 5f), new Vector3(0.75f, 0.75f, 0.75f), Vector3.zero);
            particleGo.transform.parent = map.transform;
            map.canfollowID = new int[] { (int)MainManager.AnimIDs.AntCapitain - 1 };

            CheckDiscovery(NewDiscoveries.PitOfTrials);

        }

        public override GameObject LoadBattleMap()
        {
            GameObject battleMap = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("Pit100BattleMap")) as GameObject;
            AddCorrectMaterials(battleMap.transform);

            battleMap.transform.GetChild(0).Find("AncientPlatform").GetChild(0).GetComponent<MeshRenderer>().material.renderQueue = 2500;
            return battleMap;
        }
    }
}
