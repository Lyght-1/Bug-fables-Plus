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

namespace BFPlus.Patches.EventControlTranspilers
{
    /// <summary>
    /// Maki outside swamp event, post-beast
    /// </summary>

    public class PatchChapter5HoaxeIntermissionEvent : PatchBaseEvent148
    {
        public PatchChapter5HoaxeIntermissionEvent()
        {
            priority = 173744;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchCall(AccessTools.Method(typeof(MainManager), "GetPartyEntities", new Type[] { typeof(bool) })),
                i => i.MatchStfld(out _));

            ILLabel label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Ldc_I4, 942);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchVeGuBattleHoaxeIntermissionEvent), "CheckHoaxeIntermission"));
            cursor.Emit(OpCodes.Brtrue, label);

            cursor.GotoNext(i => i.MatchLdcR4(0.65f));
            cursor.GotoNext(i => i.MatchLdarg0(), i=>i.MatchLdfld(out _));

            var transitionRef = cursor.Instrs[cursor.Index + 1].Operand;
            cursor.Emit(OpCodes.Ldc_I4, (int)NewEvents.HoaxeIntermission5);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchVeGuBattleHoaxeIntermissionEvent), "StartIntermission"));
            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.Ret).MarkLabel(label);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager), "GetTransitionSprite", new Type[] { }));
            cursor.Emit(OpCodes.Stfld, transitionRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchChapter5HoaxeIntermissionEvent), "ResetStuff"));
        }

        static void ResetStuff()
        {
            EntityControl[] party = MainManager.GetPartyEntities(true);

            Vector3[] positions = new Vector3[]
            {
                new Vector3(3.8f, 4f, 62.15f),
                new Vector3(6.26f, 4f, 61.7f),
                new Vector3(5f, 4f, 62.9f)
            };
            for (int i = 0; i < party.Length; i++)
            {
                party[i].transform.position = positions[i];
            }

            if (MainManager.map.chompy != null)
                MainManager.map.chompy.transform.position = positions[1] + new Vector3(0,0,0.1f);

            MainManager.ChangeMusic();
        }
    }
}
