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

namespace BFPlus.Patches.NPCControlTranspilers
{
    public class PatchShootProjectile : PatchBaseNPCControlShootProjectile
    {
        public PatchShootProjectile()
        {
            priority = 1877;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdcI4(182),i=>i.MatchBgt(out _));

            var localVar = cursor.Next.Operand;
            int cursorIndex = cursor.Index;

            cursor.GotoNext(i => i.MatchBeq(out _));
            var jumpTo = cursor.Next.Operand;

            cursor.Goto(cursorIndex);
            cursor.Emit(OpCodes.Ldloc, localVar);
            cursor.Emit(OpCodes.Ldloc_1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchShootProjectile), "IsNewProjEnemy"));
            cursor.Emit(OpCodes.Brtrue, jumpTo);

            cursor.GotoNext(MoveType.After, i => i.MatchStloc(out _),i => i.MatchLdloc(out _), i=>i.MatchLdcI4(59),i=>i.MatchBeq(out _));

            cursorIndex = cursor.Index;
            cursor.GotoNext(i => i.MatchStfld(out _));
            var projOffest = cursor.Next.Operand;

            cursor.GotoNext(i => i.MatchStfld(out _));
            var tossSound = cursor.Next.Operand;

            cursor.GotoNext(i => i.MatchStfld(out _));
            var targetAnim = cursor.Next.Operand;

            cursor.GotoNext(i => i.MatchStfld(out _));
            var waitTimes = cursor.Next.Operand;

            cursor.GotoNext(i => i.MatchStfld(out _));
            var projId = cursor.Next.Operand;
            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Ldloc, localVar);
            cursor.Emit(OpCodes.Ldloc_1);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldflda, projOffest);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldflda, targetAnim);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldflda, waitTimes);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldflda, projId);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldflda, tossSound);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchShootProjectile), "CheckProjData"));
        }

        static bool IsNewProjEnemy(int animid, NPCControl npc)
        {
            if (animid == (int)MainManager.AnimIDs.AngryPlant)
                return true;

            if (MainManager_Ext.IsNewEnemy(npc.entity, NewEnemies.FireAnt))
            {
                return true;
            }

            return false;
        }

        static void CheckProjData(int animId, NPCControl npc, ref Vector3 projOffset, ref int targetAnim, ref float waitTime, ref int projId, ref string tossSound)
        {
            if (animId == (int)MainManager.AnimIDs.AngryPlant)
            {
                projOffset = new Vector3(0, 1.3f);
                tossSound = "Spit";
                projId = 5;
                waitTime = 0.25f;
                targetAnim = 103;
                npc.entity.PlaySound("Blosh");
                npc.entity.animstate = 100;
            }

            if (MainManager_Ext.IsNewEnemy(npc.entity, NewEnemies.FireAnt))
            {
                projOffset = new Vector3(0, 1.3f);
                tossSound = "Chew";
                projId = 18;
                waitTime = 0.1f;
                targetAnim = 101;
                npc.entity.PlaySound("WaspKingMFireball1");
                npc.entity.PlaySound("Blosh");
                npc.entity.animstate = 100;
            }
        }
    }
}
