using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Object;
namespace BFPlus.Extensions.Maps
{
    public class SeedlingMinigameMap : CustomMap
    {
        protected override void LoadCustomData()
        {
            AddCorrectMaterials(map.transform);
            map.skyboxmat = Resources.Load<Material>("materials/skybox/Grass1");
            map.canfollowID = new int[0];
            SetBaseMapData(Color.gray, Vector3.zero, Vector3.zero, MainManager.Areas.BugariaOutskirts, Resources.Load("Audio/Music/Field0") as AudioClip);
        }


        public override GameObject LoadBattleMap()
        {
            return Instantiate(Resources.Load("Prefabs/BattleMaps/" + MainManager.BattleMaps.Grasslands1) as GameObject);
        }
    }
}
