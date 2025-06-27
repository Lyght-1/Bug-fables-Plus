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
    /// <summary>
    /// We make sure that ink and sticky status turns get added instead of being set
    /// </summary>
    public class PatchStatusTurns : PatchBaseMainManagerSetCondition
    {
        public PatchStatusTurns()
        {
            priority = 50;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            ILLabel[] iLLabels = null;
            cursor.GotoNext(i=>i.MatchSwitch(out iLLabels));
            var jumpList = iLLabels.ToList();
            jumpList.AddRange(new ILLabel[] { iLLabels[4], iLLabels[4] });
            cursor.Next.Operand = jumpList.ToArray();
        }
    }
}
