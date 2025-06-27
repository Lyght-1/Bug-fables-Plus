using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace BFPlus.Extensions
{
    public class DelayedProjExtra : MonoBehaviour
    {
        public Action<int, int[], int> extraAction;

        public int[] data;

        public void DoExtraEffect(int damageDone, int projId)
        {
            extraAction(projId, data, damageDone);
        }

        public static DelayedProjExtra AddDelayedProjExtra(GameObject obj, int[] data, Action<int, int[],int> extraAction)
        {
            GameObject delayedExtraObj = new GameObject("delayedExtra");
            delayedExtraObj.transform.parent = obj.transform;
            DelayedProjExtra extra = delayedExtraObj.AddComponent<DelayedProjExtra>();
            extra.data = data;
            extra.extraAction = extraAction;
            return extra;
        }
    }
}
