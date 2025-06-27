using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Patches.BattleControlTranspilers
{
    public class PatchCounterAnimation : PatchBaseCounterAnimation
    {
        public PatchCounterAnimation()
        {
            priority = 26542;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchBleUn(out _));
            cursor.Next.OpCode = OpCodes.Br;
            cursor.GotoPrev(i => i.MatchLdloc2());
            cursor.RemoveRange(4);
 
            cursor.GotoNext(i => i.MatchLdcR4(20f));
            cursor.GotoNext(MoveType.After,i => i.MatchLdnull(),i=>i.MatchStfld(out _));

            var sRef = cursor.Prev.Operand;
            var typeRef = cursor.Instrs[cursor.Index+1].Operand;

            int cursorIndex=  cursor.Index;

            cursor.GotoNext(i => i.MatchLdcI4(102));

            cursor.GotoNext(i => i.MatchLdfld(out _));
            var counterRef = cursor.Next.Operand;
            cursor.Goto(cursorIndex);
            cursor.GotoPrev(i => i.MatchLdnull());
            cursor.Emit(OpCodes.Ldloc_2);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, sRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, counterRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchCounterAnimation), "GetBackCounterSprite"));
            cursor.Remove();
        }


        static Transform GetBackCounterSprite(int type, Transform sprite, GameObject counter)
        {
            if (type == 3)
            {
                sprite = MainManager.NewUIObject("insidewheel", counter.GetComponentInChildren<SpriteRenderer>().transform, new Vector3(0f, 0f, -0.01f), Vector3.one, MainManager.guisprites[(int)NewGui.TPLossBack], counter.GetComponentInChildren<SpriteRenderer>().sortingOrder - 1).transform;
                sprite.gameObject.layer = 15;
            }
            return sprite;
        }
    }
}
