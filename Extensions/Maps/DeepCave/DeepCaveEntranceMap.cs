using BFPlus.Extensions.Maps.SandCastleDepths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MainManager;
using UnityEngine;

namespace BFPlus.Extensions.Maps.DeepCave
{
    public class DeepCaveEntranceMap : DeepCaveMap
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            map.camlimitneg = new Vector3(-40, -73, -4);
            map.camlimitpos = new Vector3(22, 0, 999);
            map.ylimit = -100;

            Transform fallHazard = map.transform.Find("fall");
            fallHazard.gameObject.AddComponent<Hazards>().type = Hazards.Type.Hole;

            var baseObject = map.transform.Find("Base");

            Transform fakeWall = baseObject.Find("FakeWall");
            var fader = fakeWall.gameObject.AddComponent<FaderRange>();
            fakeWall.GetComponent<MeshRenderer>().materials = new Material[] { MainManager.Fade3D, MainManager.outlinemain };
            fader.pivot = new Vector3(-26.93f, -40f, 0.5f);
            fader.mindistance = 0;
            fader.player = true;
            fader.maxdistance = 4f;
            fader.color = Color.gray;
            fader.fadedelay = 0.2f;
            fader.fadepercent = 0.4f;
            CheckDiscovery(NewDiscoveries.LushAbyss);


        }
    }
}
