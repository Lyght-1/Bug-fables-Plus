using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Maps.NewPowerPlant
{
    public class PowerPlantBigRoomMap : PowerPlantMaps
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            map.camlimitneg = new Vector3(-15, -5, -28);
            map.camlimitpos = new Vector3(15, 999, 999);
            FixPowerPlantBigRoom(map.transform);

        }

        void FixPowerPlantBigRoom(Transform map)
        {
            AddCorrectMaterials(map);
            var baseObject = map.Find("Base");
            foreach (Transform g in baseObject.Find("WallGears"))
            {
                var spin = g.gameObject.AddComponent<SpinAround>();
                spin.itself = new Vector3(0, 0, -3f);
            }
            AddPowerPlantsComponents(baseObject.transform);
            var elecHazard = baseObject.Find("ElecHazard").Find("Hazard").gameObject.AddComponent<Hazards>();
            elecHazard.type = Hazards.Type.Spikes;
        }
    }
}
