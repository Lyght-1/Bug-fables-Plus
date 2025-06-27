using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Maps.SandCastleDepths
{

    public class SandCastleDepthsIcePuzzleMap : SandCastleDepthsIced
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            map.camangle = new Vector3(15, 0, 0);
            map.camlimitneg = new Vector3(-18, -8, -6);
            map.camlimitpos = new Vector3(20, 7, 999);
            map.fogend = 30;
            var baseObject = map.transform.Find("Base");

            var roofSpikes = baseObject.Find("RoofSpikes");
            foreach (Transform iceSpike in roofSpikes)
            {
                var renderQueue = iceSpike.gameObject.AddComponent<RenderQueue>();
                renderQueue.queue = 3001;
                renderQueue.materials = new int[1] { 0 };

            }

            var puzzleSpikes = baseObject.Find("IcePuzzleSpikes");
            foreach (Transform iceSpike in puzzleSpikes)
            {
                iceSpike.gameObject.AddComponent<Fader>();
            }
            baseObject.Find("CastlePillar (2)").gameObject.AddComponent<Fader>();

            map.transform.Find("RockLimit").gameObject.AddComponent<RockLimit>();


        }
    }
}
