using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BFPlus.Extensions;
namespace BFPlus.Patches.EventControlTranspilers
{
    /// <summary>
    /// Jaunes red pain trading evnet
    /// </summary>
    public class PatchJauneViThemes : PatchBaseEvent162
    {
        public PatchJauneViThemes()
        {
            priority = 187454;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(67),i => i.MatchCall(out _));
            cursor.GotoNext(MoveType.After, i => i.MatchLdcR4(0.05f), i => i.MatchCall(out _));

            cursor.Emit(OpCodes.Ldstr, NewMusic.ViTheme.ToString());
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager), "ChangeMusic", new Type[] { typeof(string) }));
        }
    }
}
