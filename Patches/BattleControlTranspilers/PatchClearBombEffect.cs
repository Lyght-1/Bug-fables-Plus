using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Patches.BattleControlTranspilers
{
    public class PatchClearBombEffect : PatchBaseClearBombEffect
    {
        public PatchClearBombEffect()
        {
            priority = 0;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdstr("IceBreak"));
            cursor.Prev.OpCode = OpCodes.Nop;

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchClearBombEffect), "Prefix"));
            cursor.Emit(OpCodes.Ldstr, "IceBreak");
        }

        static void Prefix()
        {
            var __instance = MainManager.battle;
            if (!__instance.enemy)
            {
                for (int i = 0; i != MainManager.instance.playerdata.Length; i++)
                {
                    if (MainManager.instance.playerdata[i].hp > 0)
                    {
                        __instance.Heal(ref MainManager.instance.playerdata[i], 5, false);
                    }
                }
            }
            else
            {
                for (int i = 0; i != __instance.enemydata.Length; i++)
                {
                    if (__instance.enemydata[i].hp > 0)
                    {
                        __instance.Heal(ref __instance.enemydata[i], 5, false);
                    }
                }
            }
        }
    }
}
