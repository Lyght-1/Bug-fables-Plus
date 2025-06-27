using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MainManager;
using UnityEngine;

namespace BFPlus.Extensions.Maps.GiantLairPlayroom
{
    public class GiantLairPlayroom1Map : GiantLairPlayroom
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            map.camlimitneg = new Vector3(-50, 0, -10);
            map.camlimitpos = new Vector3(30, 999, 999);

            var baseObject = map.transform.Find("Base");

            foreach (Transform obj in baseObject.Find("ToyBlocks"))
            {
                var fader = obj.gameObject.AddComponent<Fader>();
                fader.alwaysfade = true;
                fader.zdistance = 3;
            }

            CheckCraneRotation(918, baseObject);
            CheckDiscovery(NewDiscoveries.GiantsPlayroom);

        }
    }
}
