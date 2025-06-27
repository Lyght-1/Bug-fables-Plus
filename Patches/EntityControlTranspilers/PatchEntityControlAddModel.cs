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

namespace BFPlus.Patches.EntityControlTranspilers
{
    /// <summary>
    /// Check if its one of our new model anim id and load it
    /// </summary>
    public class PatchCheckNewModel : PatchBaseEntityControlAddModel
    {
        public PatchCheckNewModel()
        {
            priority = 0;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "CheckModel"));
            cursor.RemoveRange(2);
        }
    }
}
