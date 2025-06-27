using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Maps.GiantLairPlayroom
{
    public class GiantLairPlayroomBossMap : GiantLairPlayroom
    {
        protected override void LoadCustomData()
        {
            base.LoadCustomData();
            map.camlimitneg = new Vector3(0, 0, -10);
            map.camlimitpos = new Vector3(50, 999, 24);

            var baseObject = map.transform.Find("Base");

            Transform box = baseObject.Find("Jester").GetChild(1);
            box.gameObject.AddComponent<BoxCollider>();

            Transform handle = box.GetChild(0);

            if (!MainManager.instance.flags[927])
            {
                MusicSpinner spinner = handle.gameObject.AddComponent<MusicSpinner>();
                spinner.rotation = new Vector3[] { new Vector3(0, 0, 1) };
                spinner.spinner = new Transform[] { handle };
                spinner.itemspitout = true;
                spinner.notes = new Vector2[] {
                    new Vector2(0.1f, 1.875f),
                    new Vector2(0.35f, 2f),
                    new Vector2(0.7f, 1.333f),
                    new Vector2(1.6f, 1.875f),
                    new Vector2(1.95f, 2f),
                    new Vector2(2.3f, 1.333f),
                    new Vector2(2.9f, 1.25f),
                    new Vector2(3.3f, 1.125f),
                    new Vector2(3.7f, 1.5f),
                    new Vector2(4.5f, 1.875f),
                    new Vector2(4.85f, 2f),
                    new Vector2(5.2f, 1.333f),
                    new Vector2(6.1f, 1.875f),
                    new Vector2(6.45f, 2f),
                    new Vector2(6.7f, 1.333f),
                    new Vector2(7.3f, 1.25f),
                    new Vector2(7.7f, 1.125f),
                    new Vector2(8.1f, 1f)
                };
                spinner.maxtime = 540;
                spinner.maxspin = 6;
                spinner.spinstop = 0.01f;
                spinner.spinlimit = 6;
                spinner.soundclip = Resources.Load<AudioClip>("audio/sounds/MusicBox");
            }
            else
            {
                UnityEngine.Object.Destroy(handle.gameObject);
            }

            foreach (var renderer in box.GetComponentsInChildren<MeshRenderer>())
            {
                Material mainMat = renderer.material;
                renderer.materials = new Material[] { mainMat, MainManager.outlinemain };
            }

            CheckCraneRotation(927, baseObject);
        }

        public override GameObject LoadBattleMap()
        {
            GameObject battleMap = UnityEngine.Object.Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("GiantLairPlayroomBoss")) as GameObject;
            RenderSettings.skybox = Resources.Load<Material>("materials/skybox/Black");
            RenderSettings.fogColor = new Color(0.1029f, 0.0098f, 0.0098f);
            RenderSettings.fogEndDistance = 30;
            AddCorrectMaterials(battleMap.transform);
            UnityEngine.Object.Destroy(battleMap.transform.Find("Base").Find("Jester").gameObject);
            battleMap.transform.position = new Vector3(-24, -0.1f, -2);
            return battleMap;
        }
    }
}
