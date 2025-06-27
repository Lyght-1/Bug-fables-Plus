using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Maps.DeepCave
{

    public class DeepCave1Map : DeepCaveMap
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            map.camlimitneg = new Vector3(-16, 0, -12);
            map.camlimitpos = new Vector3(19, 25, 14);

            var baseObject = map.transform.Find("Base");

            Transform platform = baseObject.Find("Platform");
            //platform.GetComponent<MeshRenderer>().materials = new Material[] { MainManager.Fade3D, MainManager.outlinemain };
            Fader fader = platform.gameObject.AddComponent<Fader>();
            fader.pivotoffset = new Vector3(0, 0, 7.9f);
            fader.zdistance = 0;


            Transform movingPlat = baseObject.Find("WoodenPlatform");
            movingPlat.gameObject.AddComponent<Fader>().pivotoffset = new Vector3(0,0,15);

            AddScrewPlatform(movingPlat.gameObject, new Vector3(0, -17, 0), new int[] { 0 }, Vector3.zero, "", ScrewPlatform.Type.Platform, false,false);

            ConditionChecker checker = baseObject.Find("VinesDoor").gameObject.AddComponent<ConditionChecker>();
            checker.limit = new int[] { 861 };
        }
    }
}
