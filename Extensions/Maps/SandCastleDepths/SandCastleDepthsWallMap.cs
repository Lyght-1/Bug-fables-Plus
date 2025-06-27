using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Maps.SandCastleDepths
{

    public class SandCastleDepthsWallMap : SandCastleDepthsIced
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            map.camangle = new Vector3(15, 0, 0);      
            map.camlimitneg = new Vector3(-8, -8, -50);
            map.camlimitpos = new Vector3(50, 999, 999);
            map.fogend = 100;

            var baseObject = map.transform.Find("Base");
            baseObject.Find("NewRoachStatue").GetComponent<MeshRenderer>().materials = new Material[] { MainManager.Main3D, MainManager.outlinemain };

            var walls = baseObject.Find("MovingWalls");
            for (int i = 0; i < walls.childCount; i++)
            {
                Transform wall = walls.GetChild(i);
                ScrewPlatform screwPlat = AddScrewPlatform(wall.gameObject, new Vector3(i == 0 ? 12f : -12f, wall.position.y, wall.position.z), new int[] { 0, 1 }, Vector3.zero, "Rumble", ScrewPlatform.Type.Platform, true, true,30f);          
                screwPlat.smoothmovement = true;
                wall.GetChild(0).gameObject.AddComponent<Hazards>().type = Hazards.Type.Spikes;
            }

            foreach (Transform faderWall in baseObject.Find("FaderWalls"))
            {
                var fader = faderWall.gameObject.AddComponent<Fader>();
                fader.zdistance = 11;
            }
        }
    }

    public class SandCastleDepthsIced : SandCastleDepths
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            map.fogcolor = new Color(0.8924f, 0.9875f, 0.99f);
            map.skycolor = new Color(0.8924f, 0.9875f, 0.99f);
            map.battleleafcolor = new Color(0.5613f, 0.9192f, 1f);
            map.icemap = true;
        }
    }
}
