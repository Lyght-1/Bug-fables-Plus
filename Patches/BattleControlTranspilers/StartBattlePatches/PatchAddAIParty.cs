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
    public class PatchAddAIParty : PatchBaseStartBattle
    {
        public PatchAddAIParty()
        {
            priority = 3184;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(189));
            cursor.GotoNext(i => i.MatchLdloc(out _));

            var listRef = cursor.Next.Operand;

            cursor.GotoNext(i => i.MatchLdcI4(0), i => i.MatchLdloc(out _));
            cursor.Emit(OpCodes.Ldloc, listRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchAddAIParty), "CheckNewAi"));
            cursor.Emit(OpCodes.Stloc, listRef);
        }

        static List<int[]> CheckNewAi(List<int[]> aiList)
        {
            if (MainManager.BadgeIsEquipped((int)Medal.EverlastingFlame))
                return new List<int[]> { new int[] { (int)NewAnimID.Hoaxe, (int)MainManager.Animations.Flustered } };

            return aiList;
        }

    }
}
