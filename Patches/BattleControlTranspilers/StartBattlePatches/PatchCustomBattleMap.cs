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

namespace BFPlus.Patches.BattleControlTranspilers.StartBattlePatches
{
    public class PatchCustomBattleMap : PatchBaseStartBattle
    {
        public PatchCustomBattleMap()
        {
            priority = 1317;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchLdstr("Battle"), i => i.MatchNewobj(out _), i => i.MatchStfld(out _));
            int cursorIndex = cursor.Index;

            cursor.GotoNext(i => i.MatchStloc(out _));
            var variableRef = cursor.Next.Operand;
            cursor.Goto(cursorIndex);

            ILLabel falseLabel = cursor.DefineLabel();
            ILLabel loadedLabel = cursor.DefineLabel();

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "HasCustomBattleMap"));
            cursor.Emit(OpCodes.Brfalse, falseLabel);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "LoadCustomBattleMap"));
            cursor.Emit(OpCodes.Stloc, variableRef);
            cursor.Emit(OpCodes.Br, loadedLabel);
            cursor.GotoNext(i => i.MatchLdstr("Prefabs/BattleMaps/"));
            cursor.MarkLabel(falseLabel);
            cursor.GotoNext(i => i.MatchLdloc(out _)).MarkLabel(loadedLabel);
        }

    }
}
