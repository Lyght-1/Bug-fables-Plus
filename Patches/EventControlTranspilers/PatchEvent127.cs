using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Patches.EventControlTranspilers
{

    /// <summary>
    /// Adds our new island to the bugariafull prefab
    /// </summary>
    public class PatchNewIslandToTelescope : PatchBaseEvent127
    {
        public PatchNewIslandToTelescope()
        {
            priority = 151086;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdstr("Prefabs/Objects/BugariaFull"));
            cursor.GotoNext(i => i.MatchStfld(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchNewIslandToTelescope), "AddIslandtoBugariaFull"));
        }

        static GameObject AddIslandtoBugariaFull(GameObject bugariaFull)
        {
            MainManager_Ext.LoadMapsBundle();
            GameObject newIsland = UnityEngine.Object.Instantiate(MainManager_Ext.mapPrefabs.LoadAsset("AbandonedTower")) as GameObject;
            GameObject baseObject = newIsland.transform.GetChild(0).gameObject;

            baseObject.transform.SetParent(bugariaFull.transform);
            UnityEngine.Object.Destroy(newIsland);

            baseObject.transform.localScale = Vector3.one * 0.06f;
            baseObject.transform.position = new Vector3(42.17f, -0.93f, -22.5f);
            foreach (Transform child in baseObject.transform)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }
            MainManager_Ext.UnloadMapsBundle();
            return bugariaFull;
        }
    }
}
