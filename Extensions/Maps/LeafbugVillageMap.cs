using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Maps
{
    public class LeafbugVillageMap : CustomMap
    {
        protected override void LoadCustomData()
        {
            AddCorrectMaterials(map.transform);
            map.skyboxmat = Resources.Load<Material>("materials/skybox/Grass5");
            SetBaseMapData(Color.gray, Vector3.zero, Vector3.zero, MainManager.Areas.WildGrasslands, Resources.Load("Audio/Music/Dungeon4") as AudioClip);
            map.battleleafcolor = new Color(0.72f, 0.18f, 0.71f);
            map.camlimitneg = new Vector3(-999, -20, -999);
            map.canfollowID = new int[0];

            var baseObject = map.transform.Find("Base");
            List<Fader> faders = new List<Fader>();
            foreach (Transform obj in baseObject.Find("MainTrunks"))
            {
                faders.Add(obj.gameObject.AddComponent<Fader>());
            }

            var shopdoorChecker = baseObject.Find("Door").gameObject.AddComponent<ConditionChecker>();
            shopdoorChecker.limit = new int[] { 886 };


            var inkSplatters = baseObject.Find("InkSplatters");

            foreach(Transform splatter in inkSplatters)
            {
                splatter.GetComponent<SpriteRenderer>().material.color = new Color(0.19f,0,0.51f);
            }


            map.insides = new GameObject[] { map.transform.Find("LeafbugHouse").gameObject };
            map.insidetypes = new MapControl.InsideType[] { MapControl.InsideType.Stretch };

            Transform fallHazard = map.transform.Find("fall");
            fallHazard.gameObject.AddComponent<Hazards>().type = Hazards.Type.Hole;
            CheckDiscovery(NewDiscoveries.LeafbugVillage);

            GameObject[] hideInsides = new GameObject[] { baseObject.Find("dragonfly_39 (3)").gameObject, 
                baseObject.Find("dragonfly_39 (4)").gameObject,baseObject.Find("Chest (2)").gameObject };

            foreach (GameObject go in hideInsides) 
            {
                InsideSorter sorter = go.AddComponent<InsideSorter>();
                sorter.insideid = 0;
                sorter.child = go;
            }
        
        }

        public override GameObject LoadBattleMap()
        {
            GameObject battleMap = UnityEngine.Object.Instantiate(MainManager_Ext.mapPrefabs.LoadAsset(NewMaps.LeafbugVillage.ToString())) as GameObject;
            AddCorrectMaterials(battleMap.transform);
            battleMap.transform.Find("Base").Find("BridgeRopes").gameObject.SetActive(false);
            battleMap.transform.position = new Vector3(0,3.85f, 15);
            return battleMap;
        }
    }
}
