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

namespace BFPlus.Patches.EventControlTranspilers
{
    /// <summary>
    /// SPUDER CH1 BATTLE EVENT
    /// </summary>
    public class PatchSpuderBattleHoaxeIntermissionEvent : PatchBaseEvent26
    {
        public PatchSpuderBattleHoaxeIntermissionEvent()
        {
            priority = 29502;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchLdcI4(24), i => i.MatchLdcI4(1), i => i.MatchStelemI1());
            ILLabel label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchSpuderBattleHoaxeIntermissionEvent), "CheckHoaxeIntermission"));     
            cursor.Emit(OpCodes.Brtrue,label);

            cursor.GotoNext(MoveType.After,i => i.MatchLdcI4(41),i=>i.MatchLdcI4(1), i=>i.MatchStelemI1());
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchSpuderBattleHoaxeIntermissionEvent), "StartIntermission1"));
            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.Ret);

            cursor.GotoNext(i => i.MatchLdcI4(36), i => i.MatchLdcI4(1));
            cursor.MarkLabel(label);
        }

        static bool CheckHoaxeIntermission()
        {
            return MainManager.instance.flags[41];
        }

        static void StartIntermission1()
        {
            MainManager.events.StartEvent((int)NewEvents.HoaxeIntermission1, null);
        }
    }
}
