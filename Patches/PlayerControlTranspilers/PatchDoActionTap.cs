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

namespace BFPlus.Patches.PlayerControlTranspilers
{
    public class PatchKabbuDashFlag : PatchBasePlayerControlDoActionTap
    {
        public PatchKabbuDashFlag()
        {
            priority = 671;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdsfld(out _),i=>i.MatchLdfld(out _),i => i.MatchLdcI4(699));
            cursor.Next.OpCode = OpCodes.Nop;
            cursor.GotoNext(i => i.MatchLdfld(out _));
            Utils.RemoveUntilInst(cursor, i => i.MatchLdcI4(5));
        }
    }

    public class PatchHoaxeAbility : PatchBasePlayerControlDoActionTap
    {
        public PatchHoaxeAbility()
        {
            priority = 426;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,
                i => i.MatchCall(AccessTools.Method(typeof(PlayerControl),"GetAngle")), 
                i => i.MatchStfld(out _));

            ILLabel label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchHoaxeAbility), "IsHoaxeCrown"));
            cursor.Emit(OpCodes.Brfalse,label);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchHoaxeAbility), "DoHoaxeActionTap"));
            Utils.InsertYieldReturn(cursor);


            cursor.MarkLabel(label);
        }

        static bool IsHoaxeCrown()
        {
            return MainManager.instance.playerdata[0].animid == (int)NewAnimID.HoaxeCrown;
        }


        static IEnumerator DoHoaxeActionTap()
        {
            EntityControl hoaxe = MainManager.player.entity;
            hoaxe.animstate = 100;
            hoaxe.overrideflip = true;
            hoaxe.sprite.transform.localEulerAngles = new Vector3(0f, MainManager.player.GetAngle(), 0f);

            Vector3 pillarPos = hoaxe.transform.position + Vector3.up + MainManager.player.lastdelta * 2f;
            MainManager.PlaySound("WaspKingMFireball2");
            DialogueAnim pillar = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Objects/FirePillar 1"), pillarPos, Quaternion.identity) as GameObject).AddComponent<DialogueAnim>();
            pillar.transform.localScale = new Vector3(0f, 0.5f, 0f);
            pillar.targetscale = new Vector3(0.45f, 0.5f, 0.45f);
            pillar.shrink = false;
            pillar.shrinkspeed = 0.05f;

            pillar.transform.GetChild(3).localScale = Vector3.one * 0.5f;
            pillar.transform.GetChild(4).localScale = Vector3.one * 0.5f;

            yield return EventControl.tenthsec;
            BoxCollider boxCollider = pillar.gameObject.AddComponent<BoxCollider>();
            pillar.tag = "Icecle";
            boxCollider.size = new Vector3(5f, 5f, 5f);
            boxCollider.center = new Vector3(0.5f, 2f);
            boxCollider.transform.eulerAngles = new Vector3(0f, hoaxe.detect.transform.eulerAngles.y + 90f);
            boxCollider.isTrigger = true;

            yield return new WaitForSeconds(0.4f);
            pillar.shrinkspeed = 0.1f;
            pillar.targetscale = new Vector3(0, 1f, 0);

            UnityEngine.Object.Destroy(boxCollider, 0.25f);
            UnityEngine.Object.Destroy(pillar.gameObject, 0.35f);
            yield return EventControl.halfsec;
        }
    }
}
