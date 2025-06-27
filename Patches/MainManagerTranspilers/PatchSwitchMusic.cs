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
    /// <summary>
    /// Add a call to our own music check function to correctly parse new songs ids
    /// </summary>
    public class PatchSwitchMusic : PatchBaseMainManagerSwitchMusic
    {
        public PatchSwitchMusic()
        {
            priority = 7086;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdtoken(out _));
            cursor.RemoveRange(2);
            cursor.GotoNext(MoveType.After,i => i.MatchCallvirt(out _), i => i.MatchCallvirt(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "CheckMusicId"));
            cursor.RemoveRange(2);
        }
    }
}
