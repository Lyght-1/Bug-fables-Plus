using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Experimental.UIElements.EventDispatcher;

namespace BFPlus.Extensions.Maps.AbandonedTower
{
    public class AbandonedTower3Map : AbandonedTowerMap
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            map.camangle = new Vector3(15, 0, 0);
            map.camlimitneg = new Vector3(-19, 0, -15);
            map.camlimitpos = new Vector3(19, 999, 15);

            var baseObject = map.transform.Find("Base");

            Transform water = baseObject.Find("Water");

            water.GetComponent<MeshRenderer>().materials = new Material[] { Resources.Load<Material>("materials/OutlinedWater1") };
            var sma = water.gameObject.AddComponent<StaticModelAnim>();
            sma.speed = new Vector2(0, 0.0001f);
            sma.current = new Vector2(0, 0);

            Hazards hazard = water.gameObject.AddComponent<Hazards>();
            hazard.type = Hazards.Type.Water;
            hazard.respawnentities = new int[0];
            hazard.waterfloats = new Transform[0];
            hazard.flagfloats = new int[0];
            hazard.riverammount = new Vector3(0.02f, 0, 0);
            hazard.freezetime = 2000;
            AddPipesComponents(baseObject.transform);


            foreach(Transform pipe in baseObject.Find("DownPipes"))
            {
                pipe.gameObject.GetComponent<SpriteBounce>().requiresentity = 13;
            }
        }

        protected void AddPipesComponents(Transform transform)
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
                AddPipesComponents(child);
            }
        }
    }
}
