using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Patches.NPCControlTranspilers
{
    public class PatchChargeAndAttack : PatchBaseChargeAndAttack
    {
        public PatchChargeAndAttack()
        {
            priority = 0;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdloc1());

            var label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Ldloc_1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchChargeAndAttack), "IsWorm"));
            cursor.Emit(OpCodes.Brfalse, label);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldloc_1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchChargeAndAttack), "DoWormAttackBehavior"));
            Utils.InsertYieldReturn(cursor);
            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.Ret);
            cursor.MarkLabel(label);
        }

        static bool IsWorm(NPCControl npc)
        {
            return npc.entity.animid == (int)NewAnimID.Worm || npc.entity.animid == (int)NewAnimID.WormSwarm;
        }

        static AccessTools.FieldRef<NPCControl, float> dirtCdRef = AccessTools.FieldRefAccess<NPCControl, float>("dirtcd");
        static IEnumerator DoWormAttackBehavior(NPCControl npc)
        {
            bool isSwarm = npc.entity.animid == (int)NewAnimID.WormSwarm;
            List<EntityControl> worms = new List<EntityControl>() { npc.entity };
            if (isSwarm)
            {
                worms.AddRange(npc.entity.subentity);
            }
            npc.entity.StopForceMove();
            npc.entity.overrideanim = true;

            npc.entity.animstate = 100;
            foreach (var worm in worms)
                worm.digging = false;
            MainManager.PlaySound("DigPop2");
            npc.attacking = true;
            if (npc.entity.digpart != null && npc.entity.digpart.Length > 0 && npc.entity.digpart[1] != null)
            {
                npc.entity.digpart[1].transform.position = new Vector3(0f, -9999f);
            }
            if (dirtCdRef(npc) <= 0f)
            {
                MainManager.PlayParticle("DirtExplodeLight", npc.transform.position, 1f).transform.localScale = Vector3.one * 0.75f;
            }
            dirtCdRef(npc) = 30f;

            yield return EventControl.quartersec;
            while (MainManager.IsPaused())
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
            while (MainManager.IsPaused())
            {
                yield return null;
            }
            if (Vector3.Distance(npc.transform.position, MainManager.player.transform.position) < (isSwarm ? 1.4f : 1.2f) && !MainManager.instance.minipause)
            {
                npc.StartBattle();
                npc.entity.overrideanim = false;
                npc.StopForceBehavior();
                yield break;
            }
            yield return EventControl.tenthsec;
            if (npc.inrange)
            {
                npc.attacking = false;
                foreach (var worm in worms)
                    worm.digging = false;
                npc.entity.digtime = 100f;
            }
            yield return new WaitForSeconds(0.5f);
            while (MainManager.IsPaused())
            {
                yield return null;
            }
            if (!npc.inrange)
            {
                npc.entity.Emoticon(1, 60);
            }
            npc.entity.animstate = 0;
            npc.entity.overrideanim = false;
            npc.attacking = false;
            npc.StopForceBehavior();
            yield break;
        }

    }
}
