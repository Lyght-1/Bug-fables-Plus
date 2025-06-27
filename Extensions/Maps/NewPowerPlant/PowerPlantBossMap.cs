using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Object;
namespace BFPlus.Extensions.Maps.NewPowerPlant
{
    public class PowerPlantBossMap : PowerPlantMaps
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();

            map.camlimitneg = new Vector3(-5, 0, -9);
            map.camlimitpos = new Vector3(5, 999, 999);
            FixPowerPlantBoss(map.transform);

            var baseObject = map.transform.Find("Base");
            var checker = baseObject.Find("ElecParticles").gameObject.AddComponent<ConditionChecker>();
            checker.limit = new int[] { 847 };
        }

        void FixPowerPlantBoss(Transform map)
        {
            AddCorrectMaterials(map.transform);
            var baseObject = map.transform.Find("Base");
            AddPowerPlantsComponents(baseObject.transform);
        }

        public override GameObject LoadBattleMap()
        {
            GameObject battleMap = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("PowerPlantBossBattle")) as GameObject;
            FixPowerPlantBoss(battleMap.transform);
            RenderSettings.skybox  = Resources.Load<Material>("materials/skybox/Black");
            battleMap.transform.position = new Vector3(0, 0, 6);
            return battleMap;
        }
    }
}
