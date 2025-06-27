using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static BattleControl;

namespace BFPlus.Patches.ChompyPatches
{
    public class PatchChompyRibbonsOptions : PatchBaseChompy
    {

        public PatchChompyRibbonsOptions()
        {
            priority = 40462;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdcI4(185), i=>i.MatchBeq(out _));

            var beqInst = cursor.Prev;

            int cursorIndex = cursor.Index;

            for (int j = -6; j < -2; j++)
            {
                cursor.Emit(cursor.Body.Instructions[cursorIndex + j].OpCode, cursor.Body.Instructions[cursorIndex + j].Operand);
            }
            cursor.Emit(OpCodes.Ldc_I4, (int)NewItem.WingRibbon);
            cursor.Emit(beqInst.OpCode, beqInst.Operand);
        }
    }
    
    public class PatchChompyCheckJump : PatchBaseChompy
    {
        public PatchChompyCheckJump()
        {
            priority = 41198;
        }
        public static bool CanJumpAttack(int target)
        {
            return MainManager.battle.enemydata[target].position == BattleControl.BattlePosition.Flying && MainManager.instance.flagvar[56] == (int)NewItem.WingRibbon;
        }

        static IEnumerator DoJumpAttack(int target)
        {
            EntityControl chompy = AccessTools.FieldRefAccess<BattleControl, EntityControl>(MainManager.battle, "chompy");
            chompy.LockRigid(true);

            float height = MainManager.battle.enemydata[target].battleentity.height;
            Vector3 startPos = chompy.transform.position;
            Vector3 endPos = startPos + Vector3.up * height;

            chompy.animstate = (int)MainManager.Animations.Jump;
            chompy.PlaySound("Jump");
            yield return BattleControl_Ext.LerpPosition(20f, startPos, endPos, chompy.transform);
        }

        static bool IsReachable(BattleControl.BattlePosition pos)
        {
            if (pos == BattlePosition.Ground)
                return true;

            if (pos == BattleControl.BattlePosition.Flying && MainManager.instance.flagvar[56] == (int)NewItem.WingRibbon)
                return true;


            MainManager.battle.commandsuccess = false;
            return false;
        }

        static void GetChompyBackDown()
        {
            EntityControl chompy = AccessTools.FieldRefAccess<BattleControl, EntityControl>(MainManager.battle, "chompy");
            Vector3 startPos = chompy.transform.position;
            Vector3 endPos = new Vector3(startPos.x, 0, startPos.z);
            chompy.LockRigid(false);

            chompy.StartCoroutine(BattleControl_Ext.LerpPosition(15, startPos, endPos, chompy.transform));
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,
                i => i.MatchLdfld(AccessTools.Field(typeof(BattleControl), "chompy")),
                i => i.MatchLdfld(AccessTools.Field(typeof(EntityControl), "forcemove")),
                i => i.MatchBrtrue(out _),
                i => i.MatchLdloc1()
                );

            cursor.Prev.OpCode = OpCodes.Nop;
            int cursorIndex = cursor.Index;

            cursor.GotoNext(MoveType.After, i => i.MatchLdarg0(), i => i.MatchLdfld(out _));
            var targetRef = cursor.Prev.Operand;
            cursor.Goto(cursorIndex);
            var label = cursor.DefineLabel();

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, targetRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchChompyCheckJump), "CanJumpAttack"));
            cursor.Emit(OpCodes.Brfalse, label);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, targetRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchChompyCheckJump), "DoJumpAttack"));
            Utils.InsertYieldReturn(cursor);

            cursor.MarkLabel(label);
            cursor.Emit(OpCodes.Ldloc_1);

            cursor.GotoNext(i => i.MatchLdstr("Chew"));
            cursor.GotoNext(i => i.MatchBrtrue(out _));

            var brLabel = cursor.Next.Operand;
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchChompyCheckJump), "IsReachable"));
            cursor.Emit(OpCodes.Brfalse, brLabel);
            cursor.Remove();
            cursor.GotoNext(i => i.MatchBrtrue(out _), i => i.MatchLdloc1());
            brLabel = cursor.Next.Operand;
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchChompyCheckJump), "IsReachable"));
            cursor.Emit(OpCodes.Brfalse, brLabel);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchChompyCheckJump), "GetChompyBackDown"));
            cursor.Remove();
        }
    }
    
    public class PatchChompyGetNextGrounded : PatchBaseChompy
    {
        public PatchChompyGetNextGrounded()
        {
            priority = 41121;
        }

        static bool HasWingRibbon()
        {
            return MainManager.instance.flagvar[56] == (int)NewItem.WingRibbon;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdcI4(0),i=>i.MatchLdcI4(1),i => i.MatchCall(AccessTools.Method(typeof(BattleControl), "GetNextGrounded")));

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchChompyGetNextGrounded), "HasWingRibbon"));
            cursor.Emit(OpCodes.Ldc_I4_1);
            cursor.RemoveRange(2);
        }
    }
}
