using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Maps.AbandonedTower
{

    public class AbandonedTower2Map : AbandonedTowerMap
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            map.camangle = new Vector3(0, 0, 0);
            map.camoffset = new Vector3(0, 4, -6f);
            map.camlimitneg = new Vector3(-999, 0, -999);
            map.camlimitpos = new Vector3(999, 999, 999);
            map.roundways = new Transform[0];
            map.rotatecam = true;
            map.tieYtoplayer = true;
            map.skyboxmat = Resources.Load<Material>("materials/skybox/Black");

            var baseObject = map.transform.Find("Base");
            baseObject.GetComponent<MeshRenderer>().materials = new Material[] { MainManager.Main3D };

            foreach(Transform wall in baseObject.transform.Find("WallsLz"))
            {
                wall.GetComponent<MeshRenderer>().materials = new Material[] { MainManager.Main3D };
                Fader fader = wall.gameObject.AddComponent<Fader>();
                fader.fadedistance = -1;
                fader.zdistance = 2;
                fader.pivotoffset = new Vector3(0, 0, 14);
            }
        }
    }
}
