using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BFPlus.Extensions;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace BFPlus.Patches.DoActionPatches.EnemyPatches
{
    public class PatchPirahnaChompChangeSummon : PatchBaseDoAction
    {
        public PatchPirahnaChompChangeSummon()
        {
            priority = 83658;
        }

        static int ChangeChomperSummon()
        {
            if (MainManager.battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.PirahnaChomp)
                return (int)NewEnemies.PirahnaChomp;
            return (int)MainManager.Enemies.FlyTrap;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(566));
            cursor.GotoNext(i => i.MatchLdcI4(20));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchPirahnaChompChangeSummon), "ChangeChomperSummon"));
            cursor.Remove();
        }
    }

    public class PatchPirahnaChompBiteDamage : PatchBaseDoAction
    {
        public PatchPirahnaChompBiteDamage()
        {
            priority = 83211;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(559));

            cursor.GotoNext(MoveType.After, i => i.MatchLdfld(AccessTools.Field(typeof(BattleControl), "playertargetID")));

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchPirahnaChompBiteDamage), "GetChomperDamage"));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchPirahnaChompBiteDamage), "GetChomperBiteProperty"));
            cursor.RemoveRange(13);
        }

        static int GetChomperDamage()
        {
            if (MainManager.battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.PirahnaChomp)
            {
                return (!MainManager.battle.enemydata[BattleControl_Ext.actionID].locktri) ? 4 : 2;
            }
            return (!MainManager.battle.enemydata[BattleControl_Ext.actionID].locktri) ? 2 : 1;
        }

        static BattleControl.AttackProperty? GetChomperBiteProperty()
        {
            if (MainManager.battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.PirahnaChomp)
            {
                return BattleControl.AttackProperty.Poison;
            }
            return null;
        }
    }
}
