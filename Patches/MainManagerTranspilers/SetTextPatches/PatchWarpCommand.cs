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

namespace BFPlus.Patches.MainManagerTranspilers.SetTextPatches
{
    /// <summary>
    /// Adds new maps enum amount to correct warp value
    /// </summary>
    public class PatchWarpCommand : PatchBaseSetText
    {
        public PatchWarpCommand()
        {
            priority = 27290;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.Before, i => i.MatchCall(AccessTools.Method(typeof(MainManager), "LoadMap", new Type[] {})));
            cursor.GotoPrev(i => i.MatchLdtoken(out _));
            cursor.GotoNext(MoveType.After,i => i.MatchConvI4());
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchWarpCommand), "GetNewMapsAmount"));
            cursor.Emit(OpCodes.Add);
        }

        static int GetNewMapsAmount()
        {
            return Enum.GetNames(typeof(NewMaps)).Length;
        }
    }
}
