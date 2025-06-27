using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BFPlus.Extensions;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace BFPlus.Patches.DoActionPatches.EnemyPatches
{
    public class Abomiberry
    {
        static int GetAbomiberryBiteDamage()
        {
            int baseBiteDmg = 3;
            if (MainManager.battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.Abomiberry)
            {
                var battle = MainManager.battle;
                baseBiteDmg = 5;
                bool hardmode = battle.HardMode();

                if (MainManager.instance.tp > 0)
                {
                    int tpReduction = !battle.commandsuccess ? baseBiteDmg : baseBiteDmg - 2;

                    if (hardmode)
                        tpReduction++;

                    EntityControl playerTarget = battle.playertargetentity;
                    Vector3 startPos = playerTarget.transform.position + Vector3.up * 3f;
                    Vector3 endPos = playerTarget.transform.position + Vector3.up * 5f;
                    BattleControl_Ext.Instance.RemoveTP(-tpReduction, startPos, endPos);
                }
            }
            return baseBiteDmg;

        }

        static int CheckAbomiberryExplosionDamage(bool hasShield)
        {
            int baseExplosionDmg = 10;
            if (MainManager.battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.Abomiberry)
            {
                baseExplosionDmg = 20;
                if (MainManager.instance.tp > 0)
                {
                    var battle = MainManager.battle;
                    var playerid = battle.partypointer[1];
                    Vector3 startPos = MainManager.instance.playerdata[playerid].battleentity.transform.position + Vector3.up * 2f;
                    Vector3 endPos = MainManager.instance.playerdata[playerid].battleentity.transform.position + Vector3.up * 5f;
                    BattleControl_Ext.Instance.RemoveTP(-99, startPos, endPos);
                }
            }
            return (!hasShield) ? baseExplosionDmg : baseExplosionDmg / 2;
        }

        static int GetAbomiberryExplosionHeal()
        {
            int baseHeal = 10;
            if (MainManager.battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.Abomiberry)
            {
                baseHeal = 20;
            }
            return baseHeal;

        }
    }

    public class PatchAbomiberryUnderBiteDMG : PatchBaseDoAction
    {
        public PatchAbomiberryUnderBiteDMG()
        {
            priority = 70152;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(341));
            cursor.GotoNext(MoveType.After, i => i.MatchLdfld(AccessTools.Field(typeof(BattleControl), "playertargetID")));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(Abomiberry), "GetAbomiberryBiteDamage"));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "GetStickyProperty"));
            cursor.RemoveRange(3);
        }
    }
    public class PatchAbomiberryExplosion : PatchBaseDoAction
    {
        public PatchAbomiberryExplosion()
        {
            priority = 70601;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(350));
            cursor.GotoNext(i => i.MatchBrtrue(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(Abomiberry), "CheckAbomiberryExplosionDamage"));
            cursor.RemoveRange(4);
        }
    }

    public class PatchAbomiberryExplosionHealCheck : PatchBaseDoAction
    {
        public PatchAbomiberryExplosionHealCheck()
        {
            priority = 70752;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(
                MoveType.After,
                i => i.MatchLdcI4(42),
                i=> i.MatchBeq(out _)            
            );

            Instruction beqInst = cursor.Prev;
            int cursorIndex = cursor.Index;
            for (int j =7; j >2; j--)
            {
                cursor.Emit(cursor.Body.Instructions[cursorIndex - j].OpCode, cursor.Body.Instructions[cursorIndex - j].Operand);
            }

            cursor.Emit(OpCodes.Ldc_I4, (int)NewEnemies.Abomiberry);
            cursor.Emit(beqInst.OpCode, beqInst.Operand);
        }
    }
    
    public class PatchAbomiberryExplosionHealAmount : PatchBaseDoAction
    {
        public PatchAbomiberryExplosionHealAmount()
        {
            priority = 70768;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(10));
            cursor.Remove();
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(Abomiberry), "GetAbomiberryExplosionHeal"));
            cursor.GotoNext(i => i.MatchLdcI4(10));
            cursor.Remove();
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(Abomiberry), "GetAbomiberryExplosionHeal"));
        }
    }

    public class PatchAbomiberryBite : PatchBaseDoAction
    {
        public PatchAbomiberryBite() 
        {
            priority = 71435;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(359));
            cursor.GotoNext(MoveType.After, i => i.MatchLdfld(AccessTools.Field(typeof(BattleControl), "playertargetID")));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(Abomiberry), "GetAbomiberryBiteDamage"));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "GetStickyProperty"));
            cursor.RemoveRange(4);
        }
    }
}
