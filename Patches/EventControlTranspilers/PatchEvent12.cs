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
    //Event used everywhere to fake-block a loading zone with an event

    /// <summary>
    /// Before seeing the queen after beating spuder, we need to make sure that the move towards call from the new blocker
    /// to training grounds has the right position
    /// </summary>
    public class PatchAntPalace1TrainingBlocker : PatchBaseEvent12
    {
        public PatchAntPalace1TrainingBlocker()
        {
            priority = 20315;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdcR4(9.6f));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchAntPalace1TrainingBlocker), "GetMovePos"));
            Utils.RemoveUntilInst(cursor, i => i.MatchCallvirt(AccessTools.Method(typeof(EntityControl), "MoveTowards", new Type[] { typeof(Vector3) })));
        }

        static Vector3 GetMovePos()
        {
            if(MainManager.player.entity.transform.position.z < 12)
            {
                return new Vector3(15.5f, 0, 8.86f);
            }
            return new Vector3(9.6f * (float)((MainManager.player.entity.transform.position.x > 0f) ? 1 : (-1)), 0f, 18.42f);
        }
    }
}
