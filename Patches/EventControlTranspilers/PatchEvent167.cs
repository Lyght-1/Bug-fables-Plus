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
    public class PatchWebSheetCheck : PatchBaseEvent167
    {
        public PatchWebSheetCheck()
        {
            priority = 200273;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdloc1(),i => i.MatchLdcI4(77));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchWebSheetCheck), "CheckWebSheetMedal"));
        }

        static void CheckWebSheetMedal()
        {
            if(MainManager.instance.flagvar[1] == (int)NewItem.Arachnomuffins)
            {
                if (!MainManager.instance.flags[879] && !MainManager.map.mapflags[1])
                {
                    NPCControl webSheet = EntityControl.CreateItem(new Vector3(-16f, 2f, 3.5f), 2, (int)Medal.WebSheet, new Vector3(5f * UnityEngine.Random.Range(1f, 1.25f), 10f, UnityEngine.Random.Range(-2.5f, 2.5f)), -1);
                    webSheet.activationflag = 879;
                    MainManager.map.mapflags[1] = true;
                }
                else
                {
                    EntityControl.CreateItem(new Vector3(-16f, 2f, 3.5f), 0, 186, new Vector3(5f * UnityEngine.Random.Range(1f, 1.5f), 10f, UnityEngine.Random.Range(-2.5f, 2.5f)), 300);
                }
            }
        }
    }
}
