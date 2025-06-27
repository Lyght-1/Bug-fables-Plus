using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Maps.DeepCave
{
    public class DeepCave2Map : DeepCaveMap
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            map.camlimitneg = new Vector3(-20, 0, -4);
            map.camlimitpos = new Vector3(20, 999,999);
            map.camangle = new Vector3(15, 0, 0);
            var baseObject = map.transform.Find("Base");

            Transform fallHazard = map.transform.Find("fall");
            fallHazard.gameObject.AddComponent<Hazards>().type = Hazards.Type.Hole;

            ConditionChecker checker = baseObject.Find("VinesDoor").gameObject.AddComponent<ConditionChecker>();
            checker.limit = new int[] { 863 };

            Transform puzzlePlats = baseObject.Find("puzzleBase");

            AddScrewPlatform(puzzlePlats.Find("WoodenPlatform (4)").gameObject, new Vector3(0, 10, 0), new int[] { 1 }, Vector3.zero, "", ScrewPlatform.Type.Platform, true, true);
            AddScrewPlatform(puzzlePlats.Find("WoodenPlatform (5)").gameObject, new Vector3(0, 10, 0), new int[] { 3,4 }, Vector3.zero, "", ScrewPlatform.Type.Platform, true, true);
            AddScrewPlatform(puzzlePlats.Find("WoodenPlatform (1)").gameObject, new Vector3(0, -10, 0), new int[] { 1,5 }, Vector3.zero, "", ScrewPlatform.Type.Platform, true, true);
            AddScrewPlatform(puzzlePlats.Find("WoodenPlatform (2)").gameObject, new Vector3(0, 10, 0), new int[] { 3 }, Vector3.zero, "", ScrewPlatform.Type.Platform, true, true);
            AddScrewPlatform(puzzlePlats.Find("WoodenPlatform (7)").gameObject, new Vector3(0, -10, 0), new int[] { 6 }, Vector3.zero, "", ScrewPlatform.Type.Platform, true, true);
            
            ScrewPlatform screwPlat = AddScrewPlatform(map.transform.Find("RopePlatform").gameObject, new Vector3(0, 0, -7), new int[] { 10 }, Vector3.zero, "", ScrewPlatform.Type.Platform, false, false,180f);
            screwPlat.smoothmovement = true;
        }
    }
}
