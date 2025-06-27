using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Object;
namespace BFPlus.Extensions.Maps.AbandonedTower
{ 
    public class AbandonedTowerMap : CustomMap
    {
        protected override void LoadCustomData()
        {
            SetBaseMapData(Color.gray, new Vector3(0, 0f, 0f), new Vector3(15, 0, 0), MainManager.Areas.MetalLake, Resources.Load("Audio/Music/Field4") as AudioClip);
            map.skyboxmat = Resources.Load<Material>("materials/skybox/Fog");
            map.battleleafcolor = Color.gray;
            map.canfollowID = new int[0];
            map.fogend = 50;
            map.fogcolor = new Color(0.5f,0.5f,0.5f,0.5f);
            AddCorrectMaterials(map.transform);
        }

        public override GameObject LoadBattleMap()
        {
            GameObject battleMap = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset(NewMaps.AbandonedTower1.ToString())) as GameObject;
            AddCorrectMaterials(battleMap.transform);
            battleMap.transform.position = new Vector3(0, 8.9f, 0);
            return battleMap;
        }
    }
}
