using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Maps.AbandonedTower
{
    public class AbandonedTowerCardsMap : AbandonedTowerMap
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            map.camangle = new Vector3(15, 0, 0);
            map.camlimitneg = new Vector3(-18, 0, -13);
            map.camlimitpos = new Vector3(20, 999, 12);

        }
    }
}
