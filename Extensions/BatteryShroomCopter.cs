using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BFPlus.Extensions
{
    public class BatteryShroomCopter : MonoBehaviour
    {
        EntityControl entity;
        void Start()
        {
            entity = GetComponent<EntityControl>();
        }

        // Update is called once per frame
        void Update()
        {
            if(entity.extras != null && entity.extras.Length > 0 && entity.extras[0] != null )
            {
                if (entity.animstate == 100)
                {
                    ChangeCopter(new Vector3(0.16f, 0.19f, 0.04f), new Vector3(112f, -124.998f, entity.extras[0].transform.localEulerAngles.z));
                }
                else if (entity.animstate == 101)
                {
                    ChangeCopter(new Vector3(-0.19f, 0.21f, -0.212f), new Vector3(62.56f, -140f, entity.extras[0].transform.localEulerAngles.z));
                }
                else if (entity.flyinganim)
                {
                    ChangeCopter(new Vector3(0.02f, 0, 0.1f), new Vector3(90f, entity.extras[0].transform.localEulerAngles.y, entity.extras[0].transform.localEulerAngles.z));
                }
            }
        }

        void ChangeCopter(Vector3 pos, Vector3 rotation)
        {
            entity.extras[0].transform.localPosition = pos;
            entity.extras[0].transform.localEulerAngles = rotation;
        }
    }

}
