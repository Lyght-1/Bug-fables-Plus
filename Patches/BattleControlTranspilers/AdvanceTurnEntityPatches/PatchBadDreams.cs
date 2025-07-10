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

namespace BFPlus.Patches.BattleControlTranspilers.AdvanceTurnEntityPatches
{
    public class PatchBadDreams : PatchBaseAdvanceTurnEntity
    {
        public PatchBadDreams()
        {
            priority = 437;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            ILLabel label = cursor.DefineLabel();
            cursor.GotoNext(MoveType.After,i => i.MatchLdcI4(81), i=>i.MatchCall(out _));
            cursor.Remove();

            int cursorIndex = cursor.Index;

            ILLabel jumpLabel = null;
            cursor.GotoNext(i => i.MatchBge(out jumpLabel));
            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Brfalse, label);

            cursor.Emit(OpCodes.Ldarg_1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchBadDreams), "DoBadDream"));
            cursor.Emit(OpCodes.Ldarg_2);
            cursor.Emit(OpCodes.Ldc_I4_1);
            cursor.Emit(OpCodes.Stind_I1);

            cursor.Emit(OpCodes.Br, jumpLabel);
            cursor.MarkLabel(label);
        }

        static void DoBadDream(ref MainManager.BattleData target)
        {
            var damageOverrides = new BattleControl.DamageOverride[] { BattleControl.DamageOverride.NoFall, BattleControl.DamageOverride.NoIceBreak, BattleControl.DamageOverride.FakeAnim, BattleControl.DamageOverride.DontAwake, BattleControl.DamageOverride.IgnoreNumb };
            int damage = Mathf.Clamp(Mathf.CeilToInt((float)target.maxhp / 7.5f) - 1, 2, 3);

            MainManager.battle.DoDamage(null, ref target, damage, BattleControl.AttackProperty.NoExceptions, damageOverrides, false);
            if (target.hp == 0)
            {
                target.battleentity.overrideanim = false;
                target.battleentity.Invoke("OverrideOver", 1f);
            }
            target.hp = Mathf.Clamp(target.hp, 1, target.maxhp);
        }
    }
}
