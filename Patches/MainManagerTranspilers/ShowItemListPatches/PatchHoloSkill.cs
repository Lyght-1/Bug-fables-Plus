using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Patches.ShowItemListPatches
{
    public class PatchHoloSkillID : PatchBaseShowItemList
    {
        public PatchHoloSkillID()
        {
            priority = 72179;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdstr("All"));
            cursor.GotoNext(MoveType.Before, i => i.MatchCall(out _), i => i.MatchLdcI4(1));
            cursor.RemoveRange(3);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "FixHoloSkillID"));
        }
    }

    public class PatchHoloSkillText : PatchBaseShowItemList
    {
        public PatchHoloSkillText()
        {
            priority = 72330;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchCall(AccessTools.Method(typeof(MainManager), "HasSkillCost")));
            cursor.GotoNext(MoveType.Before,i => i.MatchLdsfld(out _));

            var localText2 = cursor.Body.Instructions[cursor.Index - 3];
            cursor.Emit(OpCodes.Ldloc_S, localText2.Operand);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "SetHoloSkillTextEffect"));
            cursor.Emit(OpCodes.Stloc_S, localText2.Operand);
        }
    }
}
