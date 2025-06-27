using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Object;
namespace BFPlus.Extensions.Maps.AbandonedTower
{
    public class AbandonedTower1Map : AbandonedTowerMap
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            map.camangle = new Vector3(15, 0, 0);
            map.camlimitneg = new Vector3(-20, -9,-44);
            map.camlimitpos = new Vector3(20, 0, 29);

            var baseObject = map.transform.Find("Base");
            foreach (Transform obj in baseObject.Find("FaderStuff"))
            {
                var fader = obj.gameObject.AddComponent<Fader>();
                fader.alwaysfade = true;
                fader.zdistance = 5;
            }

            foreach (Transform obj in baseObject)
            {
                if(obj.name.Contains("Card"))
                {
                    var mr = obj.GetComponent<MeshRenderer>();
                    mr.materials = new Material[] { mr.materials[0], MainManager.outlinemain };
                }
            }
        }
    }
}
