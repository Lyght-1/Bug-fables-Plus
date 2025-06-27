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

namespace BFPlus.Patches.BattleControlTranspilers.EventDialoguePatches
{
    public class PatchWaspKingDeathEvent : PatchBaseEventDialogue
    {
        public PatchWaspKingDeathEvent()
        {
            priority = 8670;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(116));

            for(int j = 0; j < 2; j++)
            {
                cursor.GotoNext(i => i.MatchLdfld(AccessTools.Field(typeof(EntityControl), "hologram")));
                cursor.GotoPrev(i => i.MatchLdloc1());
                Utils.RemoveUntilInst(cursor, i => i.MatchBrtrue(out _));
                cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "IsInBoss"));
            }
        }

    }
}
