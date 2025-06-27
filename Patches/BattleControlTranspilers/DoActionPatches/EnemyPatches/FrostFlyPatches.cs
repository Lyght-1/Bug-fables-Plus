using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BFPlus.Extensions;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace BFPlus.Patches.DoActionPatches.EnemyPatches
{
    public class PatchFrostFlyKissDMG : PatchBaseDoAction
    { 
        public PatchFrostFlyKissDMG()
        {
            priority = 149770;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(1662));
            cursor.GotoNext(MoveType.After, i => i.MatchLdfld(AccessTools.Field(typeof(BattleControl), "playertargetID")));

            var frostFlyLabel = cursor.DefineLabel();
            var notFrostFlyLabel = cursor.DefineLabel();
            cursor.MarkLabel(notFrostFlyLabel);

            int cursorIndex = cursor.Index;
            cursor.GotoNext(i => i.MatchNewobj(out _));

            cursor.MarkLabel(frostFlyLabel);
            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "IsFrostfly"));
            cursor.Emit(OpCodes.Brfalse, notFrostFlyLabel);
            cursor.Emit(OpCodes.Ldc_I4_4);
            cursor.Emit(OpCodes.Ldc_I4_2);
            cursor.Emit(OpCodes.Br, frostFlyLabel);
        }
    }
    
    public class PatchFrostFlySwoopDMG : PatchBaseDoAction
    {
        public PatchFrostFlySwoopDMG()
        {
            priority = 149417;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(1657));
            cursor.GotoNext(MoveType.After, i => i.MatchLdfld(AccessTools.Field(typeof(BattleControl), "playertargetID")));

            var frostFlyLabel = cursor.DefineLabel();
            var notFrostFlyLabel = cursor.DefineLabel();

            cursor.MarkLabel(notFrostFlyLabel);

            int cursorIndex = cursor.Index;
            cursor.GotoNext(i => i.MatchNewobj(out _));
            cursor.MarkLabel(frostFlyLabel);
            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "IsFrostfly"));
            cursor.Emit(OpCodes.Brfalse, notFrostFlyLabel);
            cursor.Emit(OpCodes.Ldc_I4_2); //2 attack dmg
            cursor.Emit(OpCodes.Ldc_I4_2);//freeze property
            cursor.Emit(OpCodes.Br, frostFlyLabel);
        }
    }
}
