using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Maps.NewPowerPlant
{
    public class PowerPlantElecPuzzleMap : PowerPlantMaps
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();

            map.faderchange = true;
            map.camlimitneg = new Vector3(-15, 0, -10);
            map.camlimitpos = new Vector3(20, 999, 999);
            AddCorrectMaterials(map.transform);
            var baseObject = map.transform.Find("Base");
            AddPowerPlantsComponents(baseObject.transform);

            int switchEntity = 0;
            foreach (Transform elec in baseObject.Find("TopHazard"))
            {
                elec.gameObject.AddComponent<ElecHazard>().linkEntity = switchEntity;
            }

            foreach (Transform elec in baseObject.Find("BottomHazard"))
            {
                elec.gameObject.AddComponent<ElecHazard>();
            }

            foreach (Transform glow in baseObject.Find("GlowEnergy"))
            {
                glow.gameObject.AddComponent<GlowTrigger>().targetentityid = switchEntity;
            }

            foreach (Transform battery in baseObject.Find("FadedBattery"))
            {
                battery.GetComponent<MeshRenderer>().materials = new Material[] { MainManager.Fade3D, MainManager.outlinemain };
                battery.gameObject.AddComponent<Fader>();
            }
        }
    }
}
