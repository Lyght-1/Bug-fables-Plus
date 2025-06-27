using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MonoMod.Cil;
using UnityEngine;
using Mono.Cecil.Cil;

namespace BFPlus.Patches.ShowItemListPatches
{
    public class PatchNeedlesIcon : PatchBaseShowItemList
    {
        public PatchNeedlesIcon()
        {
            priority = 72421;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchLdcI4(45));

            cursor.GotoNext(i => i.MatchLdstr(" |size,0.55,0.6|"));
            cursor.Next.Operand = "|size,0.45,0.50|";

            cursor.GotoNext(MoveType.After, i => i.MatchLdstr("|icon,193|"), i => i.MatchCall(out _), i => i.MatchStloc(out _));

            var labelIcon = cursor.DefineLabel();
            var otherLabelIcon = cursor.DefineLabel();
            var labelSkip = cursor.DefineLabel();

            var badgeIsEquipRef = cursor.Body.Instructions[cursor.Index - 6];
            var readText3Ref = cursor.Body.Instructions[cursor.Index];
            var concatRef = cursor.Body.Instructions[cursor.Index - 2];
            var writeText3Ref = cursor.Body.Instructions[cursor.Index - 1];

            cursor.Body.Instructions[cursor.Index - 5].Operand = otherLabelIcon;
            cursor.Emit(OpCodes.Ldc_I4, (int)Medal.FrostNeedles);
            cursor.Emit(badgeIsEquipRef.OpCode, badgeIsEquipRef.Operand);
            cursor.Emit(OpCodes.Brfalse, labelIcon);

            cursor.Emit(readText3Ref.OpCode, readText3Ref.Operand);
            cursor.Emit(OpCodes.Ldstr, $"|icon,{(int)NewGui.FrostNeedles}|");
            cursor.Emit(concatRef.OpCode, concatRef.Operand);
            cursor.Emit(writeText3Ref.OpCode, writeText3Ref.Operand);

            cursor.Emit(OpCodes.Ldc_I4, (int)Medal.FireNeedles);

            cursor.Emit(badgeIsEquipRef.OpCode, badgeIsEquipRef.Operand);
            cursor.Emit(OpCodes.Brfalse, labelSkip);

            cursor.Emit(readText3Ref.OpCode, readText3Ref.Operand);
            cursor.Emit(OpCodes.Ldstr, $"|icon,{(int)NewGui.FireNeedles}|");
            cursor.Emit(concatRef.OpCode, concatRef.Operand);
            cursor.Emit(writeText3Ref.OpCode, writeText3Ref.Operand);

            cursor.MarkLabel(labelSkip);
            cursor.GotoPrev(i => i.MatchLdcI4((int)Medal.FireNeedles)).MarkLabel(labelIcon);
            cursor.GotoPrev(i => i.MatchLdcI4((int)Medal.FrostNeedles)).MarkLabel(otherLabelIcon);

            //adbp icon
            cursor.GotoNext(i => i.MatchLdcI4(28));
            cursor.GotoPrev(i => i.MatchLdstr(" |size,0.55,0.6|"));
            cursor.Next.Operand = "|size,0.35,0.40|";
        }
    }
}
