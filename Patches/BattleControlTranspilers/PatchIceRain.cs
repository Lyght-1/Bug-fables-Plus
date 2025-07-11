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

namespace BFPlus.Patches.BattleControlTranspilers
{
    public class PatchIceRainDamage : PatchBaseBattleControlIceRain
    {
        public PatchIceRainDamage()
        {
            priority = 158343;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdfld(AccessTools.Field(typeof(MainManager.BattleData), "atk")));
            cursor.GotoPrev(i => i.MatchLdsfld(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchIceRainDamage), "GetIceRainDamage"));
            Utils.RemoveUntilInst(cursor, i => i.MatchLdcI4(2));
            cursor.GotoNext(MoveType.After,i => i.MatchLdcR4(-999f), i=>i.MatchNewobj(out _), i=>i.MatchCallvirt(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchIceRainDamage), "ResetIceRainCharge"));
        }
        static int GetIceRainDamage()
        {
            return MainManager.instance.playerdata[MainManager.battle.currentturn].atk - BattleControl_Ext.Instance.iceRainHits;
        }

        static void ResetIceRainCharge()
        {
            BattleControl_Ext.Instance.iceRainHits++;
            MainManager.instance.playerdata[MainManager.battle.currentturn].charge = 0;
        }

    }
}
