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

namespace BFPlus.Patches.MainManagerTranspilers
{
    public class PatchNewItemUse : PatchBaseMainManagerGetItemUse
    {
        public PatchNewItemUse()
        {
            priority = 0;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdloca(out _));
            var itemUseRef = cursor.Next.Operand;
            cursor.GotoNext(MoveType.After,i=>i.MatchStloc3());

            var label = cursor.DefineLabel();

            cursor.Emit(OpCodes.Ldloc_2);
            cursor.Emit(OpCodes.Ldloca_S, itemUseRef);
            cursor.Emit(OpCodes.Ldloc_3);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchNewItemUse), "CheckItemUse"));
            cursor.Emit(OpCodes.Brfalse, label);

            cursor.GotoNext(i=>i.MatchStelemI4(),i => i.MatchLdloc0());
            cursor.GotoNext().MarkLabel(label);
        }

        static bool CheckItemUse(int index, ref MainManager.ItemUse itemUse, string[] data)
        {
            if (Enum.TryParse<NewItemUse>(data[0], out NewItemUse result))
            {
                itemUse.usetype[index] = (MainManager.ItemUsage)result;
                return false;
            }
            return true;
        }
    }
}
