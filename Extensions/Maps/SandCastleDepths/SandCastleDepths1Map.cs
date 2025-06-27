using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Maps.SandCastleDepths
{
    public class SandCastleDepths1Map : SandCastleDepths
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            map.camlimitneg = new Vector3(-5, -50, -3);
            map.camlimitpos = new Vector3(50, 999, 999);

            var baseObject = map.transform.Find("Base");
            foreach (Transform sandStream in baseObject.Find("SandStream"))
            {
                var mr = sandStream.GetComponent<MeshRenderer>();
                mr.materials = new Material[] { mr.materials[0], MainManager.outlinemain };
                sandStream.gameObject.AddComponent<StaticModelAnim>().speed = new Vector2(0, 0.015f);
            }
        }
    }
}
