using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Maps.GiantLairPlayroom
{
    public class GiantLairPlayroom2Map : GiantLairPlayroom
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            map.camlimitneg = new Vector3(-70, 0, -63);
            map.camlimitpos = new Vector3(97, 999, 999);

            var baseObject = map.transform.Find("Base");

            Hazards hazard = map.transform.Find("hole").gameObject.AddComponent<Hazards>();
            hazard.type = Hazards.Type.Hole;
            hazard.respawnentities = new int[0];
            hazard.waterfloats = new Transform[0];
            hazard.flagfloats = new int[0];
            hazard.riverammount = new Vector3(0f, 0, 0);
            hazard.freezetime = 0;
            hazard.speed = 0.1f;
            hazard.holdspace = -5;
            
            GameObject cranePlat = baseObject.transform.Find("CranePlatform").gameObject;

            GameObject toyCrane = baseObject.transform.Find("ToyCrane (1)").GetChild(0).gameObject;

            AddScrewPlatform(cranePlat, new Vector3(0, 12, 0), new int[] { 2,3 }, Vector3.zero, "", ScrewPlatform.Type.Platform, true, false, 100);

            ScrewPlatform screwPlat = AddScrewPlatform(toyCrane, new Vector3(0, -63, 0), new int[] { 6 }, Vector3.zero, "OmegaMove", ScrewPlatform.Type.Platform, false, true, 300);
            screwPlat.cardinaldir = ScrewPlatform.Cardinal.X;
            screwPlat.platformrotate = true;
            screwPlat.rotatelocal = true;
            screwPlat.invertYZforentity = true;

            CheckCraneRotation(921, baseObject);
        }
    }
}
