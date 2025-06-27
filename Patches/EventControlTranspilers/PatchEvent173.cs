using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Patches.EventControlTranspilers
{
    /// <summary>
    /// Patch the kali change loadout softlock by adding a setplayer yield call after change party
    /// </summary>
    public class PatchKaliSoftlock : PatchBaseEvent173
    {
        public PatchKaliSoftlock()
        {
            priority = 205120;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            for(int i=0;i<3;i++)
                cursor.GotoNext(j => j.MatchLdstr("Miniboss"));
            
            cursor.GotoNext(i => i.MatchLdcI4(0), i => i.MatchStsfld(out _));
            int cursorIndex = cursor.Index;

            cursor.GotoPrev(MoveType.After,i => i.MatchCall(AccessTools.Method(typeof(MainManager), "GetPartyPos", new Type[] { typeof(bool)})), i=>i.MatchStfld(out _));
            var posRef = cursor.Prev.Operand;

            cursor.Goto(cursorIndex);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, posRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchKaliSoftlock), "SetPlayersCall"));
            Utils.InsertYieldReturn(cursor);
        }

        static IEnumerator SetPlayersCall(Vector3[] playerPos)
        {
            MainManager.SetPlayers(playerPos);
            yield return null;
        }

    }
}
