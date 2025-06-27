using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Maps.GiantLairPlayroom
{
    public class GiantLairPlayroom3Map : GiantLairPlayroom
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            map.camlimitneg = new Vector3(-58, 0, -7);
            map.camlimitpos = new Vector3(17, 999, 999);

            var baseObject = map.transform.Find("Base");
            Transform maze = baseObject.Find("Maze");

            Transform faders = maze.Find("Fader");
            foreach(Transform child in faders)
            {
                Vector3 pivot = new Vector3(0, -5, 8);
                float maxDistance = 10;
                if(child.name == "BigWoodChunk (3)")
                {
                    maxDistance = 12;
                    pivot = new Vector3(0, -10, 8);
                }

                if (child.name == "Crate (12)" || child.name == "Crate (10)")
                {
                    pivot = new Vector3(0, -5, 9);
                    maxDistance = 8;
                }

                AddFaderRange(child.gameObject, pivot, 0, maxDistance, true, 0.2f, 0.4f);
                child.GetComponent<MeshRenderer>().materials = new Material[] { MainManager.Fade3D };
            }

            foreach (Transform obj in maze)
            {
                if (obj.name.Contains("PrisonGateLocal"))
                {
                    obj.Find("Black").GetComponent<MeshRenderer>().materials = new Material[] { Resources.Load<Material>("materials/BlackFull") };
                }
            }

            GameObject toyCrane = maze.Find("ToyCrane (1)").GetChild(0).gameObject;
            ScrewPlatform screwPlat = AddScrewPlatform(toyCrane, new Vector3(0, 350, 0), new int[] { 4,5 }, Vector3.zero, "OmegaMove", ScrewPlatform.Type.Platform, false, true, 30);
            screwPlat.cardinaldir = ScrewPlatform.Cardinal.X;
            screwPlat.platformrotate = true;
            screwPlat.rotatelocal = true;
            screwPlat.invertYZforentity = true;

            GameObject gate = maze.Find("PrisonGateLocal (3)").GetChild(0).gameObject;
            Transform[] gates = { gate.transform.GetChild(3), gate.transform.GetChild(4) };

            foreach(Transform obj in gates)
            {
                AddScrewPlatform(obj.gameObject, new Vector3(0, -5, 0), new int[] { 7 }, Vector3.zero, "", ScrewPlatform.Type.Platform, false, true, 30);
            }


            if (MainManager.instance.flags[926])
            {
                Transform raceGate = maze.Find("PrisonGateLocal (2)").GetChild(0);
                Transform raceEndGate = maze.Find("PrisonGateLocal (1)").GetChild(0);
                Transform[] raceGates = { raceEndGate, raceGate };

                for (int i = 0; i < raceGates.Length; i++)
                {
                    Transform[] transforms = { raceGates[i].GetChild(3), raceGates[i].GetChild(4) };
                    foreach (Transform transform in transforms)
                    {
                        UnityEngine.Object.Destroy(transform.gameObject);
                    }
                }
            }

            CheckCraneRotation(926,baseObject);
        }
    }
}
