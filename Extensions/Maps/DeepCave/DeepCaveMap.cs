using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MainManager;
using UnityEngine;
using static UnityEngine.Object;
namespace BFPlus.Extensions.Maps.DeepCave
{
    public class DeepCaveMap : CustomMap
    {
        protected override void LoadCustomData()
        {
            SetBaseMapData(Color.gray, new Vector3(0, 0f, 0f), new Vector3(15, 0, 0), MainManager.Areas.FarGrasslands, Resources.Load("Audio/Music/Cave1") as AudioClip);
            map.skyboxmat = Resources.Load<Material>("materials/skybox/Cavern2");
            map.battleleafcolor = new Color(0.28f, 0.8f, 0.63f);
            map.globallight = new Color(0.29f, 0.56f,0.298f);
            map.canfollowID = new int[0];
            
            AddCorrectMaterials(map.transform);
        }

        public override GameObject LoadBattleMap()
        {
            GameObject battleMap = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("DeepCaveBattle")) as GameObject;
            RenderSettings.skybox = Resources.Load<Material>("materials/skybox/Cavern2");
            RenderSettings.ambientLight = new Color(0.29f, 0.56f, 0.298f);

            AddCorrectMaterials(battleMap.transform);
            return battleMap;
        }
    }
}
