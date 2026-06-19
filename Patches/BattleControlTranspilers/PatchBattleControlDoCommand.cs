using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace BFPlus.Patches.BattleControlTranspilers
{
    /// <summary>
    /// Always set commandsuccess to true for DoCommand's LongRandomBar for COMMAND menucode<br/>
    /// Note that it will always succeed even if failing one or all inputs
    /// </summary>
    public class PatchDoCommandLongRandomBar : PatchBaseBattleControlDoCommand
    {
        public PatchDoCommandLongRandomBar()
        {
            priority = 15;
        }
        protected override void ApplyPatch(ILCursor cursor, ILContext context)
        {
            cursor.GotoNext(MoveType.After,
                i => i.MatchLdcR4(1f),
                i => i.MatchBlt(out _))
            .Emit(OpCodes.Ldloc_1)
            .Emit(OpCodes.Ldc_I4_1)
            .Emit(OpCodes.Stfld, AccessTools.Field(typeof(BattleControl), nameof(BattleControl.commandsuccess)));
        }
    }
}