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
    public class PatchChapter6HoaxeIntermissionEvent : PatchBaseEvent194
    {
        public PatchChapter6HoaxeIntermissionEvent()
        {
            priority = 226415;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchStfld(AccessTools.Field(typeof(MainManager), "overridefollower")));

            ILLabel label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Ldc_I4, 946);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchVeGuBattleHoaxeIntermissionEvent), "CheckHoaxeIntermission"));
            cursor.Emit(OpCodes.Brtrue, label);

            cursor.GotoNext(i => i.MatchCall(out _), i=>i.MatchLdcI4(-10));

            cursor.Emit(OpCodes.Ldc_I4, (int)NewEvents.HoaxeIntermission6);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchVeGuBattleHoaxeIntermissionEvent), "StartIntermission"));
            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.Ret).MarkLabel(label);
        }
    }
}
