using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Object;

namespace BFPlus.Extensions.Maps
{
    public class PowerPlantMaps : CustomMap
    {
        protected override void LoadCustomData()
        {
            SetBaseMapData(Color.gray, new Vector3(0, 0f, 0f), new Vector3(15, 0, 0), MainManager.Areas.GoldenSettlement, Resources.Load("Audio/Music/Dungeon2b") as AudioClip);
            map.skyboxmat = Resources.Load<Material>("materials/skybox/Black");

        }

        protected void AddPowerPlantsComponents(Transform transform)
        {
            foreach (Transform child in transform)
            {
                if (child.name.Contains("LPipe"))
                {
                    var sb = child.gameObject.AddComponent<SpriteBounce>();
                    sb.startscale = true;
                    sb.frequency = 0.02f;
                    sb.speed = 20f;
                }


                if (child.name == "graph")
                {
                    var sma = child.gameObject.AddComponent<StaticModelAnim>();
                    sma.speed = new Vector2(0.01f, 0);
                    child.GetComponent<SpriteRenderer>().material = MainManager.spritematlit;

                }
                AddPowerPlantsComponents(child);
            }
        }

        protected void FixPowerPlantExtra(Transform map)
        {
            AddCorrectMaterials(map);
            var baseObject = map.Find("Base");

            foreach (Transform g in baseObject.Find("PlatformGears"))
            {
                var spin = g.gameObject.AddComponent<SpinAround>();
                spin.itself = new Vector3(0, 0, 0.5f);
                g.gameObject.AddComponent<GearPlatform>();
            }

            foreach (Transform g in baseObject.Find("WallGears"))
            {
                var spin = g.gameObject.AddComponent<SpinAround>();
                spin.itself = new Vector3(0, 0, -3f);
            }

            AddPowerPlantsComponents(baseObject.transform);
            Transform door = baseObject.Find("DoorE");
            ConditionChecker cc = door.gameObject.AddComponent<ConditionChecker>();
            cc.limit = new int[] { 845 };
        }

        public override GameObject LoadBattleMap()
        {
            GameObject battleMap = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("PowerPlantExtra")) as GameObject;
            FixPowerPlantExtra(battleMap.transform);
            battleMap.transform.position = new Vector3(0, 16.2f, 0);
            return battleMap;
        }
    }


    public class PowerPlantExtra : PowerPlantMaps
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            FixPowerPlantExtra(map.transform);
            map.camlimitneg = new Vector3(-18, -25, -10);
            map.camlimitpos = new Vector3(15, 999, 15.5f);
            CheckDiscovery(NewDiscoveries.PowerPlantExtra);

        }
    }
}
