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
    public class SplotchSpider
    {
        static int GetSplotchJumpDamage()
        {
            if (MainManager.battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.SplotchSpider)
            {
                return 6;
            }
            return 3;
        }

        static BattleControl.AttackProperty? GetSplotchJumpProperty()
        {
            if (MainManager.battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.SplotchSpider)
            {
                return BattleControl.AttackProperty.Ink;
            }
            return null;
        }


        static void DoSplotchBigJumpDamage()
        {
            var partyDamageRef = AccessTools.Method(typeof(BattleControl), "PartyDamage", new Type[] { typeof(int), typeof(int), typeof(BattleControl.AttackProperty?), typeof(bool) });
            partyDamageRef.Invoke(MainManager.battle, new object[] { BattleControl_Ext.actionID, 6, new BattleControl.AttackProperty?(BattleControl.AttackProperty.InkOnBlock), MainManager.battle.commandsuccess });
        }

        public static bool IsSplotchSpider()
        {
            return MainManager.battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.SplotchSpider;
        }

        static Vector3 GetSplotchTarget()
        {
            if (IsSplotchSpider())
            {
                var partymiddleRef = AccessTools.FieldRefAccess<BattleControl, Vector3>("partymiddle");
                var playertargetID = AccessTools.FieldRefAccess<BattleControl, int>("playertargetID");
                playertargetID(MainManager.battle) = -1;
                return partymiddleRef(MainManager.battle);
            }
            var playerTargetEntityRef = (EntityControl)typeof(BattleControl).GetField("playertargetentity", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(MainManager.battle);
            return playerTargetEntityRef.transform.position;
        }
    }

    public class PatchSplotchSpiderJumpAtkDMG : PatchBaseDoAction
    {
        public PatchSplotchSpiderJumpAtkDMG()
        {
            priority = 95008;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(756));
            cursor.GotoNext(MoveType.After, i => i.MatchLdfld(AccessTools.Field(typeof(BattleControl), "playertargetID")));

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(SplotchSpider), "GetSplotchJumpDamage"));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(SplotchSpider), "GetSplotchJumpProperty"));
            cursor.RemoveRange(4);
        }
    }
    
    public class PatchSplotchSpiderTarget : PatchBaseDoAction
    {
        public PatchSplotchSpiderTarget()
        {
            priority = 95146;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(757));
            ChangePositionTarget(cursor);
            cursor.GotoNext(MoveType.After,i => i.MatchCall(AccessTools.Method(typeof(MainManager), "ShakeScreen", new Type[] { typeof(float), typeof(float), typeof(bool) })));

            var labelJump = cursor.DefineLabel();
            cursor.MarkLabel(labelJump);

            var labelBigJump = cursor.DefineLabel();

            int cursorIndex = cursor.Index;
            cursor.GotoNext(i => i.MatchLdstr(out _));
            cursor.MarkLabel(labelBigJump);

            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(SplotchSpider), "IsSplotchSpider"));
            cursor.Emit(OpCodes.Brfalse, labelJump);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(SplotchSpider), "DoSplotchBigJumpDamage"));
            cursor.Emit(OpCodes.Br, labelBigJump);
            ChangePositionTarget(cursor);
            ChangePositionTarget(cursor);
        }

        void ChangePositionTarget(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdloc1(), i => i.MatchLdfld(AccessTools.Field(typeof(BattleControl), "playertargetentity")));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(SplotchSpider), "GetSplotchTarget"));
            cursor.RemoveRange(4);
        }
    }
}
