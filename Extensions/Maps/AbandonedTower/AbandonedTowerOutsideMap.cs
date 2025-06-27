using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using static UnityEngine.Object;
namespace BFPlus.Extensions.Maps.AbandonedTower
{
    public class AbandonedTowerOutsideMap : CustomMap
    {
        protected override void LoadCustomData()
        {
            SetBaseMapData(Color.gray, new Vector3(0, 0f, 0f), new Vector3(15, 0, 0), MainManager.Areas.MetalLake, Resources.Load("Audio/Music/Water") as AudioClip);
            map.skyboxmat = Resources.Load<Material>("materials/skybox/Water");
            map.canfollowID = new int[0];
            map.screeneffect = MapControl.ScreenEffects.SunRaysTopRight;

            map.camangle = new Vector3(5, 0, 0);
            map.camlimitneg = new Vector3(-20, 0, -25);
            map.camlimitpos = new Vector3(20, 0, 0);
            AddCorrectMaterials(map.transform);

            var baseObject = map.transform.Find("Base");

            foreach(Transform wind in map.transform.Find("Winds"))
            {
                Wind windComp = wind.gameObject.AddComponent<Wind>();
                windComp.bobammount = 2;
                windComp.randomizer = 120f;
                windComp.limit = -65.2f;
                windComp.center = Vector3.zero;
            }

            Transform water = baseObject.Find("Circle");
            water.GetComponent<MeshRenderer>().materials = new Material[] { Resources.Load<Material>("materials/OutlinedWater3") };
            Hazards hazard = water.gameObject.AddComponent<Hazards>();
            hazard.type = Hazards.Type.Water;
            hazard.respawnentities = new int[0];
            hazard.waterfloats = new Transform[0];
            hazard.flagfloats = new int[0];

            StaticModelAnim sma = water.gameObject.AddComponent<StaticModelAnim>();
            sma.speed = new Vector2(0.0003f,0);
            sma.current = new Vector2(0.2239262f, 0);
            sma.bobspeed = new Vector3(0, 2, 0);
            sma.bobfreq = new Vector3(0, 0.1f, 0);

            Transform waterChild = water.Find("Water");

            waterChild.GetComponent<MeshRenderer>().materials = new Material[] { Resources.Load<Material>("materials/OutlinedWater1") };
            sma = waterChild.gameObject.AddComponent<StaticModelAnim>();
            sma.speed = new Vector2(0, 0.005f);

            CheckDiscovery(NewDiscoveries.IronTower);
        }

        public override GameObject LoadBattleMap()
        {
            return Instantiate(Resources.Load("Prefabs/BattleMaps/" + MainManager.BattleMaps.MetalLake) as GameObject);
        }
    }
}
