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
using UnityEngine;

namespace BFPlus.Patches.MainManagerTranspilers
{
    public class PatchPebbleTossRequirement: PatchBaseMainManagerRefreshSkills
    {
        public PatchPebbleTossRequirement()
        {
            priority = 433;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdcI4(13),i => i.MatchCall(AccessTools.Method(typeof(MainManager), "BadgeIsEquipped", new Type[] { typeof(int)})));
            cursor.Next.OpCode = OpCodes.Nop;
            
            cursor.GotoNext(i => i.MatchCall(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchPebbleTossRequirement), "CheckPebbleToss"));
            cursor.Remove();
        }

        static bool CheckPebbleToss()
        {
            return MainManager.instance.partylevel >= 3;
        }
    }
}
