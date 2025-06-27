using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions
{
    public class BadmintonObj : MonoBehaviour
    {
        float duration = 60f;
        float time = 0;
        SpriteRenderer sr;

        EntityControl[] entities = new EntityControl[2];
        Vector3[] offsets = new Vector3[2] { new Vector3(-0.6f, 3, -0.1f), new Vector3(0.6f, 3, -0.1f) };
        int fromTarget = 0;
        void Start()
        {
            entities[0] = MainManager.map.entities[22];
            entities[1] = MainManager.map.entities[23];

            sr = gameObject.AddComponent<SpriteRenderer>();
            sr.sprite = MainManager.itemsprites[0, (int)NewItem.InkBomb];
            transform.position = entities[0].transform.position + offsets[0];         
        }

        void Update()
        {
            if(time < duration)
            {
                int target = fromTarget == 0 ? 1 : 0;
                time += MainManager.TieFramerate(1f);
                transform.position = MainManager.BeizierCurve3(entities[fromTarget].transform.position+ offsets[fromTarget], entities[target].transform.position+ offsets[target], 5, time / duration);
                transform.localEulerAngles += new Vector3(0, 0, 20);
            }
            else
            {
                time = 0;
                fromTarget = fromTarget == 0 ? 1 : 0;
                StartCoroutine(DoAnim(entities[fromTarget]));
            }
        }

        IEnumerator DoAnim(EntityControl entity)
        {
            entity.animstate = 102;
            entity.PlaySound("Death3",0.5f);
            yield return EventControl.tenthsec;
            entity.animstate = 100;
        }

        
    }
}
