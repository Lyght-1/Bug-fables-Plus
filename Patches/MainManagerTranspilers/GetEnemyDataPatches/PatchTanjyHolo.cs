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

namespace BFPlus.Patches.MainManagerTranspilers.GetEnemyDataPatches
{
    public class PatchTanjyHolo : PatchBaseMainManagerGetEnemyData
    {
        public PatchTanjyHolo()
        {
            priority = 819;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdloc0(),i=>i.MatchLdfld(out _),i => i.MatchLdcI4(110), i => i.MatchBneUn(out _));
            cursor.RemoveRange(3);
            cursor.Next.OpCode = OpCodes.Br;
        }
    }
}
