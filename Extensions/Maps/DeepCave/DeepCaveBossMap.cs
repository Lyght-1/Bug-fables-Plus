using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Maps.DeepCave
{
    public class DeepCaveBossMap : DeepCaveMap
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            map.camlimitneg = new Vector3(-67, -21, -5.5f);
            map.camlimitpos = new Vector3(1, 999, 999);
            var baseObject = map.transform.Find("Base");
        }
    }
}
