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
    public class PatchHeatingUp : PatchBaseAdvanceTurnEntity
    {
        public PatchHeatingUp()
        {
            priority = 220;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdstr("Flame"));

            // This is for jester fire healing him instead of damaging
            ILLabel label = null;
            cursor.GotoNext(i => i.MatchBrtrue(out label));
            cursor.GotoPrev(i=>i.MatchLdarg0());

            cursor.Emit(OpCodes.Ldarg_1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchHeatingUp), "CheckFireDamage"));
            cursor.Emit(OpCodes.Brfalse, label);

            //this is for heating up and supporting fire
            cursor.GotoNext(i => i.MatchLdfld(out _));
            cursor.Emit(OpCodes.Ldobj, typeof(MainManager.BattleData));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "DoHeatingUp"));
            Utils.RemoveUntilInst(cursor, i => i.MatchLdcI4(13));
        }

        static bool CheckFireDamage(ref MainManager.BattleData target)
        {
            if(target.animid == (int)NewEnemies.Jester)
            {
                MainManager.battle.Heal(ref target, 3);
                return false;
            }

            if (target.animid == (int)NewEnemies.Moeruki)
            {
                MainManager.PlaySound("StatUp");
                target.charge = Mathf.Clamp(target.charge + 1, 1, 3);
                MainManager.battle.StartCoroutine(MainManager.battle.StatEffect(target.battleentity, 4));
                return false;
            }

            return true;
        }

    }
}
