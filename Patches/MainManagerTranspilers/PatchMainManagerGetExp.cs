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

    public class PatchExpModifier : PatchBaseMainManagerGetEXP
    {

        public PatchExpModifier()
        {
            priority = 96;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcR4(2.5f), i => i.MatchMul(), i => i.MatchSub());

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchExpModifier), "GetExpModifier"));
            cursor.Remove();
        }

        static float GetExpModifier()
        {
            if ((int)MainManager.map.mapid == (int)NewMaps.Pit100BaseRoom)
                return 2.75f;
            return 1.75f;
        }
    }
}
