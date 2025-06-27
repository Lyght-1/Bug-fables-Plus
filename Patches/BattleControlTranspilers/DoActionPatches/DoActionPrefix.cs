using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Patches.BattleControlTranspilers.DoActionPatches
{
    public class DoActionPrefix : PatchBaseDoAction
    {
        public DoActionPrefix()
        {
            priority = 0;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdloc1());
            cursor.Next.OpCode = OpCodes.Nop;
            cursor.GotoNext(i => i.MatchCall(out _));
            int cursorIndex = cursor.Index;
            cursor.GotoNext(i => i.MatchLdstr("Player"));
            var entityRef = cursor.Prev.Operand;
            cursor.GotoNext(i => i.MatchLdfld(out _));
            var actionIdRef = cursor.Next.Operand;
            cursor.Goto(cursorIndex);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, entityRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, actionIdRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(DoActionPrefix), "Prefix"));
            cursor.Emit(OpCodes.Ldloc_1);
        }

        static void Prefix(EntityControl entity, int actionid)
        {
            Console.WriteLine($"do action prefix called, id : {actionid}");

            /*StackTrace stackTrace = new StackTrace();

            UnityEngine.Debug.Log("Call stack:");

            for (int i = 1; i < stackTrace.FrameCount; i++)
            {
                StackFrame frame = stackTrace.GetFrame(i);
                MethodBase method = frame.GetMethod();

                string className = method.DeclaringType?.FullName ?? "(unknown class)";
                string methodName = method.Name;

                UnityEngine.Debug.Log($"{i}: {className}.{methodName}");
            }*/

            BattleControl_Ext.Instance.inAiAttack = false;
            var __instance = MainManager.battle;
            BattleControl_Ext.Instance.entityAttacking = entity;
            BattleControl_Ext.actionID = actionid;
            BattleControl_Ext.Instance.firstHitMulti = false;
            int target = __instance.target;
            bool isPlayer = entity.CompareTag("Player");
            BattleControl.AttackArea itemareaRef = __instance.itemarea;

            if(!__instance.cancelupdate)
                __instance.action = true;


            if (isPlayer)
            {
                if (BattleControl_Ext.Instance.leifBuffSkillIds.Contains(actionid) && entity.animid == 2 && MainManager.BadgeIsEquipped((int)Medal.Blightfury))
                {
                    if (itemareaRef == BattleControl.AttackArea.SingleAlly)
                    {
                        BattleControl_Ext.Instance.DoPoison(ref MainManager.instance.playerdata[target]);
                    }
                    else if (itemareaRef == BattleControl.AttackArea.SingleEnemy)
                    {
                        BattleControl_Ext.Instance.DoPoison(ref __instance.enemydata[target]);
                    }
                    else if (itemareaRef == BattleControl.AttackArea.AllEnemies)
                    {
                        for (int i = 0; i != __instance.enemydata.Length; i++)
                        {
                            if (__instance.enemydata[i].position != BattleControl.BattlePosition.OutOfReach && __instance.enemydata[i].position != BattleControl.BattlePosition.Underground)
                            {
                                BattleControl_Ext.Instance.DoPoison(ref __instance.enemydata[i]);
                            }
                        }
                    }
                    else if (itemareaRef == BattleControl.AttackArea.AllParty)
                    {
                        for (int i = 0; i != MainManager.instance.playerdata.Length; i++)
                        {
                            if (MainManager.instance.playerdata[i].hp > 0)
                            {
                                BattleControl_Ext.Instance.DoPoison(ref MainManager.instance.playerdata[i]);
                            }
                        }
                    }
                }
            }

            if (__instance.enemy && actionid >=0 && actionid < __instance.enemydata.Length)
            {
                BattleControl_Ext.Instance.spinelingFlipped = __instance.enemydata[actionid].animid == (int)NewEnemies.Spineling && MainManager.HasCondition(MainManager.BattleCondition.Flipped, __instance.enemydata[actionid]) > -1;
            }
        }
    }
}
