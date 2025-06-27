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
using static MainManager;
using UnityEngine;

namespace BFPlus.Patches.BattleControlTranspilers.AdvanceMainTurnPatches
{
    /// <summary>
    /// Dont move the proj if the position target is -1
    /// </summary>
    public class PatchDelProjMovement : PatchBaseAdvanceMainTurn
    {
        public PatchDelProjMovement()
        {
            priority = 10231;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchPop(), i=>i.MatchLdloc1());
            cursor.Prev.OpCode = OpCodes.Nop;
            ILLabel label = cursor.DefineLabel();
            int cursorIndex = cursor.Index;

            cursor.GotoNext(MoveType.After,i => i.MatchLdarg0(), i => i.MatchLdfld(out _));
            var indexRef = cursor.Prev.Operand;
            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, indexRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchDelProjMovement), "CheckSkipMovement"));
            cursor.Emit(OpCodes.Brtrue, label);
            cursor.Emit(OpCodes.Ldloc_1);

            cursor.GotoNext(i => i.MatchLdfld(AccessTools.Field(typeof(BattleControl.DelayedProjectileData), "whilesound")));
            cursor.GotoPrev(i => i.MatchLdloc1());
            cursor.MarkLabel(label);

        }

        static bool CheckSkipMovement(int index)
        {
            return MainManager.battle.delprojs[index].areadamage >0;
        }

    }

    public class PatchDelProjAreaDamage : PatchBaseAdvanceMainTurn
    {
        public PatchDelProjAreaDamage()
        {
            priority = 10360;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {

            cursor.GotoNext(i => i.MatchCall(AccessTools.Method(typeof(UnityEngine.Object), "Destroy", new Type[] { typeof(UnityEngine.Object) })));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchDelProjAreaDamage), "GetCurrentDelayedProjExtra"));
            cursor.GotoNext(MoveType.After, i => i.MatchCall(out _));

            int cursorIndex = cursor.Index;
            cursor.GotoPrev(MoveType.After, i => i.MatchLdarg0(), i => i.MatchLdfld(out _));
            var indexRef = cursor.Prev.Operand;
            cursor.Goto(cursorIndex);

            ILLabel label = cursor.DefineLabel();
            ILLabel jumpLabel = cursor.DefineLabel();
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, indexRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchDelProjMovement), "CheckSkipMovement"));
            cursor.Emit(OpCodes.Brfalse, label);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, indexRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchDelProjAreaDamage), "DoAreaDamageProj"));
            cursor.Emit(OpCodes.Br, jumpLabel);

            cursor.MarkLabel(label);

            cursor.GotoNext(MoveType.After,i => i.MatchLdfld(AccessTools.Field(typeof(BattleControl), "commandsuccess")), i => i.MatchCall(out _));
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, indexRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchDelProjAreaDamage), "DoExtraEffect"));
            cursor.Remove();

            cursor.GotoNext(MoveType.After,i=>i.MatchStfld(AccessTools.Field(typeof(MainManager.BattleData), "turnssincedeath")));
            cursor.MarkLabel(jumpLabel);
        }

        static void DoAreaDamageProj(int index)
        {
            for (int i = 0; i < MainManager.instance.playerdata.Length; i++) 
            {
                if (MainManager.instance.playerdata[i].hp > 0)
                {
                    battle.DoDamage(null, ref MainManager.instance.playerdata[i], MainManager.battle.delprojs[index].areadamage, null, null, false);
                    MainManager.SetCondition(BattleCondition.Sticky, ref MainManager.instance.playerdata[i], 4);
                    MainManager.PlayParticle("StickyGet", MainManager.instance.playerdata[i].battleentity.transform.position + Vector3.up);
                    MainManager.PlaySound("WaterSplash2", -1, 0.8f, 1f);
                }
            
            }
        }

        static GameObject GetCurrentDelayedProjExtra(GameObject obj)
        {
            BattleControl_Ext.Instance.currentDelayedProj = obj.GetComponentInChildren<DelayedProjExtra>();
            if (BattleControl_Ext.Instance.currentDelayedProj != null)
            {
                BattleControl_Ext.Instance.currentDelayedProj.transform.parent = battle.battlemap.transform;
            }
            return obj;
        }

        static void DoExtraEffect(int damageDone, int projIndex)
        {
            if(BattleControl_Ext.Instance.currentDelayedProj != null)
            {
                BattleControl_Ext.Instance.currentDelayedProj.DoExtraEffect(damageDone, projIndex);
                UnityEngine.Object.Destroy(BattleControl_Ext.Instance.currentDelayedProj.gameObject);
            }
        }

    }
}
