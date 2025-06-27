using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Object;

namespace BFPlus.Extensions.Maps
{
    public class AntPalaceTrainingRoomMap : CustomMap
    {
        protected override void LoadCustomData()
        {
            SetBaseMapData(Color.gray, new Vector3(0, 0f, 0f), new Vector3(20, 0, 0), MainManager.Areas.BugariaCity, Resources.Load("Audio/Music/Inside2") as AudioClip);
            FixAntTrainingRoom(map.gameObject);
            map.skyboxmat = Resources.Load<Material>("materials/skybox/Black");
            map.canfollowID = new int[0];
            var baseObject = map.transform.Find("Base");
            baseObject.Find("DartTarget").Find("BullseyeTrigger").gameObject.AddComponent<DartTarget>();

            CheckDiscovery(NewDiscoveries.TrainingGrounds);

        }

        void FixAntTrainingRoom(GameObject map)
        {
            AddCorrectMaterials(map.transform);

            var baseObject = map.transform.Find("Base");
            baseObject.Find("palacewindows_0 (1)").GetComponent<SpriteRenderer>().materials = new Material[] { MainManager.spritedefaultunity };
            baseObject.Find("LightBeams").GetChild(0).GetComponent<SpriteRenderer>().materials = new Material[] { MainManager.spritedefaultunity };
        }

        public override GameObject LoadBattleMap()
        {
            GameObject battleMap = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("AntPalaceTrainingRoom")) as GameObject;
            FixAntTrainingRoom(battleMap);
            RenderSettings.skybox = Resources.Load<Material>("materials/skybox/Black");

            battleMap.transform.position = new Vector3(-17.7f, 0, 0);
            return battleMap;
        }
    }
}
