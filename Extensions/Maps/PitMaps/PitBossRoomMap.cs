using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Object;
namespace BFPlus.Extensions.Maps.PitMaps
{
    public class PitBossRoomMap : CustomMap
    {
        protected override void LoadCustomData()
        {
            FixPitBossRoom(map.gameObject);
            map.fogcolor = Color.black;
            map.fogend = 75;
            map.canfollowID = new int[0];
            SetBaseMapData(new Color(0.227f, 0.317f, 0.403f, 1), new Vector3(0, 2.25f, -10.25f), new Vector3(25, 0, 0), MainManager.Areas.BugariaOutskirts, null);
        }

        void FixPitBossRoom(GameObject map)
        {
            AddCorrectMaterials(map.transform);
            GameObject particleGo = new GameObject("spores");
            particleGo.transform.position = new Vector3(0f, 25f, 0f);
            MainManager_Ext.CreatePrefabParticle(particleGo, "prefabs/objects/Spore", 100, 0.01f, 250, 120, 0.02f, new Vector3(50f, 25f, 50f), new Vector3(0.75f, 0.75f, 0.75f), Vector3.zero);
            particleGo.transform.parent = map.transform;
        }

        public override GameObject LoadBattleMap()
        {
            GameObject battleMap = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("PitBossRoom")) as GameObject;
            FixPitBossRoom(battleMap);
            RenderSettings.fogEndDistance = 75;
            RenderSettings.fogColor = Color.black;
            return battleMap;
        }
    }
}
