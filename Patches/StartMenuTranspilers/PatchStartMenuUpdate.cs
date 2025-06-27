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

namespace BFPlus.Patches.StartMenuTranspilers
{
    public class PatchStartMenuUpdate : PatchBaseStartMenuUpdate
    {
        public PatchStartMenuUpdate()
        {
            priority = 0;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchStfld(AccessTools.Field(typeof(MainManager),"camspeed")));
            Utils.RemoveUntilInst(cursor, i=>i.MatchStfld(AccessTools.Field(typeof(MainManager), "camangleoffset")));
            cursor.Remove();
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchStartMenuUpdate), "SetMenuCamera"));
        }

        static void SetMenuCamera()
        {
            Vector3 defaultCamPos = new Vector3(0f, 0f, -5f);
            Vector3 defaultAngle = new Vector3(5, 10);
            if (MainManager_Ext.backgroundData != null)
            {
                defaultCamPos = MainManager_Ext.backgroundData.cameraTargetPos;
                defaultAngle = MainManager_Ext.backgroundData.camTargetAngle;
                MainManager.instance.camoffset = MainManager_Ext.backgroundData.camOffset;
            }

            MainManager.instance.camtargetpos = defaultCamPos;
            MainManager.instance.camangleoffset = new Vector3(defaultAngle.x + Mathf.Sin(Time.time / 7.5f) * 5,defaultAngle.y+ Mathf.Sin(Time.time / 5f) *10);
        
        }
    }
}
