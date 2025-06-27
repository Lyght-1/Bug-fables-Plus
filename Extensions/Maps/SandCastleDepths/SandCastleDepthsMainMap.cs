using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Maps.SandCastleDepths
{

    public class SandCastleDepthsMainMap : SandCastleDepthsIced
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            map.camangle = new Vector3(15, 0, 0);
            map.camlimitneg = new Vector3(-20, -7, -13);
            map.camlimitpos = new Vector3(23, 999, 999);
            map.fogend = 40;

            var baseObject = map.transform.Find("Base");
            foreach(Transform statue in baseObject.Find("Statues"))
            {
                statue.GetComponent<MeshRenderer>().materials = new Material[] { MainManager.Main3D, MainManager.outlinemain };
            }

            Transform key1 = baseObject.Find("key1");
            key1.gameObject.AddComponent<ConditionChecker>().limit = new int[] { 852 };

            Transform bosskey = baseObject.Find("bosskey");
            bosskey.gameObject.AddComponent<ConditionChecker>().limit = new int[] { 853 };
            CheckDiscovery(NewDiscoveries.SandCastleDepths);

        }
    }
}
