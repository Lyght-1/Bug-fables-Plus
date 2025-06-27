using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MainManager;
using static UnityEngine.Experimental.UIElements.EventDispatcher;

namespace BFPlus.Extensions.Maps
{
    public class BeehiveMinigameMap : CustomMap
    {
        protected override void LoadCustomData()
        {
            AddCorrectMaterials(map.transform);
            SetBaseMapData(Color.gray, Vector3.zero, Vector3.zero, MainManager.Areas.Beehive, Resources.Load("Audio/Music/Bee") as AudioClip);
            map.camlimitneg = new Vector3(-9f, -4f, -24f);
            map.camlimitpos = new Vector3(11f, 999f, 999f);
            map.skycolor = new Color(0.8f,0.64f,0.41f,1);
            map.skyboxmat = Resources.Load<Material>("materials/skybox/Black");
            map.canfollowID = new int[0];
            var baseObject = map.transform.Find("Base");
            var honey = baseObject.Find("Fountain").Find("Honey");
            var sma = honey.gameObject.AddComponent<StaticModelAnim>();
            sma.bobspeed = new Vector2(0, 3);
            sma.bobfreq = new Vector3(0f, 0.025f, 0f);
            map.transform.Find("Light").gameObject.AddComponent<FaceCamera>();

            SpriteRenderer gourmetStomachItem = new GameObject("gourmetStomach").AddComponent<SpriteRenderer>();
            gourmetStomachItem.gameObject.AddComponent<ConditionChecker>().limit = new int[] { 875 };
            gourmetStomachItem.sprite = !MainManager.instance.flags[681] ? MainManager.itemsprites[1, (int)Medal.GourmetStomach] : MainManager.guisprites[190];
            gourmetStomachItem.transform.parent = map.transform;
            gourmetStomachItem.transform.position = new Vector3(-9.3f, 1.95f, 1f);
        }


        public override GameObject LoadBattleMap()
        {
            return UnityEngine.Object.Instantiate(Resources.Load("Prefabs/BattleMaps/" + MainManager.BattleMaps.Grasslands1) as GameObject);
        }
    }
}
