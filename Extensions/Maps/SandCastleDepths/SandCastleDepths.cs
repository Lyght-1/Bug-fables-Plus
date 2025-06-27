using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Object;
namespace BFPlus.Extensions.Maps.SandCastleDepths
{
    public class SandCastleDepths : CustomMap
    {
        protected override void LoadCustomData()
        {
            SetBaseMapData(Color.gray, new Vector3(0, 0f, 0f), new Vector3(20, 0, 0), MainManager.Areas.SandCastle, Resources.Load("Audio/Music/Dungeon3") as AudioClip);
            map.skyboxmat = Resources.Load<Material>("materials/skybox/Desert1");
            Material[] mats = new Material[] { Resources.Load<Material>("Materials/3DMain2"), MainManager.outlinemain };
            AddSandCastleMats(map.transform, mats);
        }

        void AddSandCastleMats(Transform parent, Material[] mats)
        {
            foreach (Transform transform in parent)
            {
                var mr = transform.gameObject.GetComponent<MeshRenderer>();

                if(mr != null && mr.material != null)
                {
                    if (mr.material.name.Contains("MainPlane"))
                    {
                        mr.material = MainManager.mainPlane;
                    }

                    if (!transform.name.Contains("SandStream") && !transform.name.Contains("Stalagmite") && !transform.name.Contains("Coffin") && mr.materials.Length > 1)
                    {
                        mr.materials = mats;
                    }
                }

                var sr = transform.gameObject.GetComponent<SpriteRenderer>();

                if (sr != null)
                {
                    sr.material = MainManager.spritemat;
                }
                AddSandCastleMats(transform, mats);
            }
        }

        public override GameObject LoadBattleMap()
        {
            return Instantiate(Resources.Load("Prefabs/BattleMaps/" + MainManager.BattleMaps.SandCastleDarkIce) as GameObject);
        }
    }
}
