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

namespace BFPlus.Patches.BattleControlTranspilers.AIAttackPatches
{
    public class PatchAntLieutenantCamera : PatchBaseAIAttack
    {
        public PatchAntLieutenantCamera()
        {
            priority = 38960;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,
                i =>i.MatchLdcI4(13),
                i => i.MatchCallvirt(AccessTools.Method(typeof(EntityControl), "MoveTowards", new Type[] { typeof(Vector3), typeof(float), typeof(int), typeof(int) })));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl), "SetDefaultCamera", new Type[] { }));
        }

    }

    public class PatchInAIAttackCheck : PatchBaseAIAttack
    {
        public PatchInAIAttackCheck()
        {
            priority = 0;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchStfld(AccessTools.Field(typeof(BattleControl), "action")));
            cursor.Emit(OpCodes.Ldc_I4_1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchInAIAttackCheck), "SetInAIAttack"));
        }


        static void SetInAIAttack(bool value)
        {
            BattleControl_Ext.Instance.inAiAttack = value;
        }
    }
}
