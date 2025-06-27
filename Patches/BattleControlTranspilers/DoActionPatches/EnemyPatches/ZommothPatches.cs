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

namespace BFPlus.Patches.BattleControlTranspilers.DoActionPatches.EnemyPatches
{
    public class PatchZommothCutscene : PatchBaseDoAction
    {
        public PatchZommothCutscene()
        {
            priority = 116407;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(1120));

            for(int i = 0; i < 4; i++)
            {
                cursor.GotoNext(j => j.MatchLdarg0(), j => j.MatchLdfld(out _), j => j.MatchLdfld(AccessTools.Field(typeof(EntityControl), "hologram")));
                cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "IsInBoss"));
                cursor.RemoveRange(3);
            }
        }
    }
}
