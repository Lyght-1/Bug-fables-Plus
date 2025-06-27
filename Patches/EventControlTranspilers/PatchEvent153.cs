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
    //Submarine get off/get on event

    /// <summary>
    /// Patch the correct name for our new island when we approach it with the submarine
    /// </summary>
    public class PatchIslandName : PatchBaseEvent153
    {
        public PatchIslandName()
        {
            priority = 178555;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchLdfld(AccessTools.Field(typeof(NPCControl), "mapid")));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchIslandName), "CheckIslandName"));
            
        }

        static int CheckIslandName(int mapid)
        {
            if (mapid == 14)
            {
                MainManager.instance.flagstring[0] = "Iron Tower";
            }
            return mapid;
        }
    }

    /// <summary>
    /// Check if we are on a new map and loads it
    /// </summary>
    public class PatchNewIslandMap : PatchBaseEvent153
    {
        public PatchNewIslandMap()
        {
            priority = 178825;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(
                i=>i.MatchSwitch(out _),
                i=>i.MatchBr(out _),
                i=>i.MatchLdsfld(AccessTools.Field(typeof(MainManager),"player"))
                );

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchNewIslandMap), "CheckIslandMap"));

        }

        static int CheckIslandMap(int mapid)
        {
            if (mapid == 14)
            {
                MainManager.player.transform.position = new Vector3(-3.8f, 0, -24);
                MainManager.LoadMap((int)NewMaps.AbandonedTower);
            }
            return mapid;
        }
    }
    /// <summary>
    /// Patch where we end up on the lake if we exit with the sub from a new island
    /// </summary>
    public class PatchLakePos : PatchBaseEvent153
    {
        public PatchLakePos()
        {
            priority = 179210;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdloc(out _), i => i.MatchLdcI4(54));

            var originalMapRef = cursor.Next.Operand;

            cursor.GotoNext(MoveType.After,i => i.MatchLdstr("Prefabs/Objects/Submarine"));

            cursor.Prev.OpCode = OpCodes.Nop;
            cursor.Emit(OpCodes.Ldloc_S, originalMapRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchLakePos), "CheckLakePos"));
            cursor.Emit(OpCodes.Ldstr, "Prefabs/Objects/Submarine");
        }

        static void CheckLakePos(MainManager.Maps mapid)
        {
            if ((int)mapid == (int)NewMaps.AbandonedTower)
            {
                MainManager.player.transform.position = new Vector3(16.87f, -0.37f, -8.7f);
            }
        }
    }
}
