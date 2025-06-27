using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Maps.AbandonedTower
{
    public class AbandonedTowerBossMap : AbandonedTowerMap
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            map.camangle = new Vector3(15, 0, 0);
            map.camlimitneg = new Vector3(-44, 0, -23);
            map.camlimitpos = new Vector3(25, 999, 16);

        }

        public override GameObject LoadBattleMap()
        {
            GameObject battleMap = UnityEngine.Object.Instantiate(MainManager_Ext.mapPrefabs.LoadAsset(NewMaps.AbandonedTowerBoss.ToString())) as GameObject;
            AddCorrectMaterials(battleMap.transform);
            RenderSettings.skybox = Resources.Load<Material>("materials/skybox/Fog");
            RenderSettings.fogColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            RenderSettings.fogEndDistance = 50;
            battleMap.transform.position = new Vector3(0, 0, 0);
            return battleMap;
        }
    }
}
