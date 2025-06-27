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

namespace BFPlus.Patches.MapControlTranspilers
{

    public class PatchCheckCustomMap : PatchBaseMapControlCreateEntities
    {
        public PatchCheckCustomMap()
        {
            priority = 50;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            var label = cursor.DefineLabel();

            cursor.GotoNext(i => i.MatchLdstr(out _));
            cursor.RemoveRange(7);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "IsValidMap"));

            cursor.GotoNext(i => i.MatchLdstr(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "IsCustomMap"));
            cursor.Emit(OpCodes.Brtrue, label);

            cursor.GotoNext(i => i.MatchNewobj(out _)).MarkLabel(label);
            cursor.GotoNext(i => i.OpCode == OpCodes.Stloc_S);

            //Patch custom entities
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MapControl_Ext), "ChangeEntities"));
            cursor.Emit(OpCodes.Ldsfld, AccessTools.Field(typeof(MapControl_Ext), "entitiesData"));
            cursor.Emit(OpCodes.Stloc_1);
            cursor.Emit(OpCodes.Ldsfld, AccessTools.Field(typeof(MapControl_Ext), "entitiesName"));
            cursor.Emit(OpCodes.Stloc_2);
        }
    }
}
