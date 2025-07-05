using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions
{
    public class ElecHazard : MonoBehaviour
    {
        public int linkEntity = -1;
        GameObject childHazard;
        bool rockBlocks = false;
        void Start()
        {
            childHazard = transform.GetChild(0).gameObject;
            childHazard.AddComponent<Hazards>().type = Hazards.Type.Spikes;
        }      
        
        void LateUpdate()
        {
            if(linkEntity != -1 && !rockBlocks && !MainManager.instance.inbattle &&MainManager.GetEntity(linkEntity) != null && MainManager.GetEntity(linkEntity).npcdata != null)
            {
                childHazard.SetActive(MainManager.GetEntity(linkEntity).npcdata.hit);
            }
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("PushRock") && !MainManager.instance.minipause)
            {
                childHazard.SetActive(false);
                rockBlocks = true;
            }
        }

        void OnTriggerExit(Collider col)
        {
            if (col.CompareTag("PushRock"))
            {
                childHazard.SetActive(true);
                rockBlocks = false;
            }
        }
    }
}