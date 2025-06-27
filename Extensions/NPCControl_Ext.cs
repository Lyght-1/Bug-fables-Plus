using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions
{
    public class NPCControl_Ext : MonoBehaviour
    {
        public int[] items = new int[] { -1,-1,-1,-1 };
        public bool[] usedItem = new bool[] { false, false, false, false };
        public bool rolledItem = false;
        public static NPCControl_Ext GetNPCControl_Ext(NPCControl npc)
        {
            if (npc.GetComponent<NPCControl_Ext>() == null)
            {
                npc.gameObject.AddComponent<NPCControl_Ext>();
            }
            return npc.GetComponent<NPCControl_Ext>();
        }
    }
}
