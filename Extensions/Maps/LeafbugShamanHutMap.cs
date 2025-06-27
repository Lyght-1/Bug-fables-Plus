using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Maps
{
    public class LeafbugShamanHutMap : CustomMap
    {
        protected override void LoadCustomData()
        {
            AddCorrectMaterials(map.transform);
            map.skyboxmat = Resources.Load<Material>("materials/skybox/Black");
            SetBaseMapData(Color.gray, Vector3.zero, Vector3.zero, MainManager.Areas.WildGrasslands, Resources.Load("Audio/Music/Dungeon4") as AudioClip);
            map.battleleafcolor = new Color(0.72f, 0.18f, 0.71f);
            map.camlimitneg = new Vector3(-999, -20, -999);
            map.canfollowID = new int[0];
        }

        public override GameObject LoadBattleMap()
        {
            GameObject battleMap = UnityEngine.Object.Instantiate(MainManager_Ext.mapPrefabs.LoadAsset(NewMaps.LeafbugShamanHut.ToString())) as GameObject;
            AddCorrectMaterials(battleMap.transform);
            RenderSettings.skybox = Resources.Load<Material>("materials/skybox/Black");

            battleMap.transform.position = new Vector3(0, -0.2f, 3.7f);
            battleMap.transform.localScale = new Vector3(1.25f, 1.25f, 1);
            return battleMap;
        }
    }
}
