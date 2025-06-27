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
    public class PatchCatalystInk : PatchBaseAdvanceTurnEntity
    {
        public PatchCatalystInk()
        {
            priority = 58;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdloc(out _),i => i.MatchSwitch(out _));
            int cursorIndex = cursor.Index;
            cursor.GotoPrev(i => i.MatchLdloc(out _));

            var indexRef = cursor.Next.Operand;
            ILLabel jumpLabel = null;
            cursor.GotoNext(i => i.MatchBr(out jumpLabel));

            cursor.Goto(cursorIndex);

            ILLabel label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Ldarg_1);
            cursor.Emit(OpCodes.Ldloc, indexRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchCatalystInk), "CheckCatalystSpill"));
            cursor.Emit(OpCodes.Brfalse, label);

            cursor.Emit(OpCodes.Ldarg_1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchCatalystInk), "DoCatalystSpill"));
            cursor.Emit(OpCodes.Ldarg_2);
            cursor.Emit(OpCodes.Ldc_I4_1);
            cursor.Emit(OpCodes.Stind_I1);

            cursor.Emit(OpCodes.Br, jumpLabel);
            cursor.MarkLabel(label);
        }

        static void DoCatalystSpill(ref MainManager.BattleData target)
        {
            var damageOverrides = new BattleControl.DamageOverride[] { BattleControl.DamageOverride.NoFall, BattleControl.DamageOverride.NoIceBreak, BattleControl.DamageOverride.FakeAnim, BattleControl.DamageOverride.DontAwake, BattleControl.DamageOverride.IgnoreNumb };
            int conditionAmount = target.condition.Count - 1;
            int damage = conditionAmount * MainManager.BadgeHowManyEquipped((int)Medal.CatalystSpill);

            MainManager.battle.DoDamage(null, ref target, damage, BattleControl.AttackProperty.NoExceptions, damageOverrides, false);
            if (target.hp == 0)
            {
                target.battleentity.overrideanim = false;
                target.battleentity.Invoke("OverrideOver", 1f);
            }
            target.hp = Mathf.Clamp(target.hp, 1, target.maxhp);
        }

        static bool CheckCatalystSpill(ref MainManager.BattleData target, int index)
        {
            return target.condition[index][0] == (int)MainManager.BattleCondition.Inked && target.condition.Count > 1 && MainManager.BadgeIsEquipped((int)Medal.CatalystSpill);
        }

    }
}
