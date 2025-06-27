using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Object;
namespace BFPlus.Extensions.Maps.GiantLairPlayroom
{

    public class GiantLairPlayroom : CustomMap
    {
        protected override void LoadCustomData()
        {
            SetBaseMapData(Color.gray, new Vector3(0, 0f, 0f), new Vector3(20, 0, 0), MainManager.Areas.GiantLair, MainManager_Ext.assetBundle.LoadAsset<AudioClip>("Playroom"));
            map.skyboxmat = Resources.Load<Material>("materials/skybox/Black");
            map.fogcolor = new Color(0.1029f, 0.0098f, 0.0098f);
            map.skycolor = Color.white;
            map.battleleafcolor = new Color(0.3396f, 0, 0.0328f);
            map.fogend = 30f;
            AddCorrectMaterials(map.transform);
            AddPlayroomComponents(map.transform);
        }

        public override GameObject LoadBattleMap()
        {
            GameObject battleMap = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("GiantLairPlayroomBattle")) as GameObject;
            RenderSettings.skybox = Resources.Load<Material>("materials/skybox/Black");
            RenderSettings.fogColor = new Color(0.1029f, 0.0098f, 0.0098f);
            RenderSettings.fogEndDistance = 30;
            AddCorrectMaterials(battleMap.transform);
            return battleMap;
        }

        void AddPlayroomComponents(Transform parent)
        {
            foreach (Transform child in parent)
            {
                if (child.name.Contains("ShortGrassLight") || child.name.Contains("BigSpikyPlant") || child.name.Contains("Dry3DBush"))
                {
                    var matColor = child.gameObject.AddComponent<MaterialColor>();
                    matColor.color = new Color(0.2941f, 0.2941f, 0.2941f);
                    matColor.settag = true;
                    matColor.materialid = 0;
                }

                if (child.name.Contains("glass"))
                {
                    var queue = child.gameObject.AddComponent<RenderQueue>();
                    queue.queue = 3001;
                    queue.materials = new int[] { 1 };
                }
                AddPlayroomComponents(child);
            }
        }

        protected void CheckCraneRotation(int flag, Transform baseObject)
        {
            if (MainManager.instance.flags[flag])
            {
                Transform crane = baseObject.Find("ToyCrane").GetChild(0);
                crane.localEulerAngles = new Vector3(crane.localEulerAngles.x, 385, crane.localEulerAngles.z);
            }
        }


    }
}
