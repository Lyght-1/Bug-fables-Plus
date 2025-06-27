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

namespace BFPlus.Patches.EventControlTranspilers
{
    public class PatchTutorialMusic : PatchBaseEvent16
    {
        public PatchTutorialMusic()
        {
            priority = 23533;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(
                i => i.MatchLdnull(),
                i =>i.MatchLdnull(),
                i=>i.MatchLdcI4(0),
                i => i.MatchCall(AccessTools.Method(typeof(BattleControl), "StartBattle")));

            cursor.Emit(OpCodes.Ldstr, "Battle0");
            cursor.Remove();
        }
    }

    public class PatchFadeViTheme : PatchBaseEvent16
    {
        public PatchFadeViTheme()
        {
            priority = 23346;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(
                i => i.MatchLdsfld(out _),
                i => i.MatchLdsfld(out _),
                i => i.MatchLdfld(out _),
                i => i.MatchLdcI4(8));

            cursor.Emit(OpCodes.Ldc_R4, 0.1f);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager), "FadeMusic"));
        }
    }

    public class PatchFadeKabbuTheme : PatchBaseEvent16
    {
        public PatchFadeKabbuTheme()
        {
            priority = 24712;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(54));
            cursor.GotoNext(MoveType.After,i => i.MatchCallvirt(AccessTools.Method(typeof(EntityControl), "MoveTowards", new Type[] {typeof(Vector3), typeof(float)})));
            cursor.Emit(OpCodes.Ldc_R4, 0.05f);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager), "FadeMusic"));
        }
    }
}
