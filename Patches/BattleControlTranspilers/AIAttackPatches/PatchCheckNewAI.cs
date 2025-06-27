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

namespace BFPlus.Patches.BattleControlTranspilers.AIAttackPatches
{
    public class PatchCheckNewAI : PatchBaseAIAttack
    {
        public PatchCheckNewAI()
        {
            priority = 37141;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            ILLabel label = cursor.DefineLabel();

            cursor.GotoNext(MoveType.After,i => i.MatchStloc2());

            int cursorIndex = cursor.Index;

            ILLabel labelJump = null;
            cursor.GotoNext(i => i.MatchBr(out labelJump));
            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchCheckNewAI), "CheckNewAI"));
            cursor.Emit(OpCodes.Brfalse,label);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchCheckNewAI), "DoHoaxeAI"));
            Utils.InsertYieldReturn(cursor);
            cursor.Emit(OpCodes.Br, labelJump);
            cursor.MarkLabel(label);
        }

        static bool CheckNewAI()
        {
            //normaly you,ll have an array/registry of animid but im le lazy 
            return MainManager.battle.aiparty.animid == (int)NewAnimID.Hoaxe;
        }

        static IEnumerator DoHoaxeAI()
        {
            BattleControl battle = MainManager.battle;
            EntityControl hoaxe = battle.aiparty;
            int hoaxeDamage = 4;

            int targetId = UnityEngine.Random.Range(0, battle.enemydata.Length);
            
            hoaxe.animstate = 113;
            yield return EventControl.halfsec;

            MainManager.PlaySound("FirePillar");
            DialogueAnim pillar = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Objects/FirePillar 1"), battle.enemydata[targetId].battleentity.transform.position, Quaternion.identity) as GameObject).AddComponent<DialogueAnim>();
            pillar.transform.parent = battle.battlemap.transform;
            pillar.transform.localScale = new Vector3(0f, 1f, 0f);
            pillar.targetscale = new Vector3(0.35f, 1f, 0.35f);
            pillar.shrink = false;
            pillar.shrinkspeed = 0.015f;
            yield return new WaitForSeconds(0.65f);
            pillar.shrinkspeed = 0.2f;
            yield return new WaitForSeconds(0.2f);
            MainManager.ShakeScreen(0.25f, 0.75f);
            battle.DoDamage(null, ref battle.enemydata[targetId], hoaxeDamage, BattleControl.AttackProperty.Fire, false);

            yield return new WaitForSeconds(1.15f);
            pillar.targetscale = new Vector3(0f, 1f, 0f);
            ParticleSystem[] componentsInChildren = pillar.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].Stop();
            }
            pillar.shrinkspeed = 0.05f;
            UnityEngine.Object.Destroy(pillar.gameObject, 3f);
            hoaxe.animstate = 115;

            yield return EventControl.halfsec;
            hoaxe.animstate = (int)MainManager.Animations.Flustered;

            if (!MainManager.instance.flags[962])
            {
                //put in common dialogue file later lul
                string line = "Soooo, do either of you have any idea why HE is here?|next,1|I... I really have no idea. Could it be because..?|next,2|We'd rather not think too much  about the implications here.";
                MainManager.instance.StartCoroutine(MainManager.SetText(line, true, Vector3.zero, MainManager.instance.playerdata[0].battleentity.transform, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }

                MainManager.instance.flags[962] = true;
            }
        }

    }
}
