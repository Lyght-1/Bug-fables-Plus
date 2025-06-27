using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MainManager;
using UnityEngine;

namespace BFPlus.Extensions.Maps.PitMaps
{
    public class Pit100RewardMap : Pit100BaseRoomMap
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            AddCorrectMaterials(map.transform);
            Material mat = new Material(Shader.Find("World of Zero/Hologram"));
            mat.color = Color.white;
            map.transform.Find("Holopad").GetComponent<MeshRenderer>().materials = new Material[] { MainManager.Main3D, MainManager.outlinemain, mat };
        }
    }
}
