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

namespace BFPlus.Patches.EventControlTranspilers
{
    //LockedDoor Event (key items)

    public class PatchMapCheckLockedDoor : PatchBaseEvent59
    {
        public PatchMapCheckLockedDoor()
        {
            priority = 66952;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdstr("Click"));
            cursor.GotoPrev(MoveType.After,i => i.MatchStloc3());
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchMapCheckLockedDoor), "CheckMapLockedDoor"));
        }

        static void CheckMapLockedDoor()
        {
            if((int)MainManager.map.mapid == (int)NewMaps.GiantLairPlayroom2)
            {
                MainManager.PlaySound("Click", -1, 0.5f, 1f);
                MainManager.GetEntity(3).SetPosition(EventControl.call.transform.position);
            }
        }
    }
}
