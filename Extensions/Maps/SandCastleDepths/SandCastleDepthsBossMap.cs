using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MainManager;
using static UnityEngine.Object;
namespace BFPlus.Extensions.Maps.SandCastleDepths
{
    public class SandCastleDepthsBossMap : SandCastleDepthsIced
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            map.camangle = new Vector3(15, 0, 0);
            map.camlimitneg = new Vector3(-3, -7, -16);
            map.camlimitpos = new Vector3(3, 999, 999);
            map.fogend = 35;

            Transform scorpIce = map.transform.Find("scorpIce");
            scorpIce.gameObject.AddComponent<ConditionChecker>().limit = new int[] { 856 };

            FixBossRoom(map.gameObject);
        }

        public override GameObject LoadBattleMap()
        {
            GameObject battleMap = Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("SandCastleDepthsBossBattle")) as GameObject;
            FixBossRoom(battleMap);
            RenderSettings.fogColor = new Color(0.8924f, 0.9875f, 0.99f);
            RenderSettings.ambientSkyColor = new Color(0.8924f, 0.9875f, 0.99f);
            RenderSettings.fogEndDistance = 35;

            battleMap.transform.position = new Vector3(0,10.19f, 0);
            return battleMap;
        }

        void FixBossRoom(GameObject map)
        {
            var baseObject = map.transform.Find("Base");
            foreach (Transform statue in baseObject.Find("Statues"))
            {
                statue.GetComponent<MeshRenderer>().materials = new Material[] { MainManager.Main3D, MainManager.outlinemain };
            }
        }
    }
}
