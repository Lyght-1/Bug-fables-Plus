using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions
{
    [Serializable]
    public class AreaData
    {
        public int areaID;
        public int mapID;
        public List<EntityData> entities = new List<EntityData>();
        public Vector3 cameraTargetPos;
        public Vector3 camTargetAngle;
        public Vector3 camOffset = new Vector3(0, 2.25f, -8.25f);
    }

    [Serializable]
    public class EntityData
    {
        public int animID;
        public Vector3 position;
        public int progress;
        public bool flip;
        public bool beforeProgress;
        public string name = "";
    }
}

