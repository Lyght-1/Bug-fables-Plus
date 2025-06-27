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
    /// Loads the correct copter sprite depending on what type of flowerling/dewling it is
    /// </summary>
    public class PatchDewlingCopter : PatchBaseEntityControlAnimSpecificQuirks
    {
        public PatchDewlingCopter()
        {
            priority = 846;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdstr("copter"));
            cursor.GotoNext(i => i.MatchLdsfld(out _));
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "GetDewlingCopter"));
            Utils.RemoveUntilInst(cursor, i => i.MatchLdarg0());
        }
    }
    /// <summary>
    /// Patch correct scale on copter depending of type of dewling or flowerling
    /// </summary>
    public class PatchCopterScale : PatchBaseEntityControlAnimSpecificQuirks
    {
        public PatchCopterScale()
        {
            priority = 913;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchCall(out _),i => i.MatchLdcR4(0.75f));
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "GetFloweringCopterScale"));
            Utils.RemoveUntilInst(cursor, i => i.MatchCallvirt(out _));
        }
    }
}
