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

namespace BFPlus.Patches.MainManagerTranspilers.SetTextPatches
{

    public class PatchNewCommandsParse : PatchBaseSetText
    {
        public PatchNewCommandsParse()
        {
            priority = 16294;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            //Parse new commands
            cursor.GotoNext(i => i.MatchLdcI4(44));
            cursor.GotoNext(MoveType.After,i => i.MatchStfld(out _));

            var tempRef = cursor.Prev.Operand;

            int cursorIndex = cursor.Index;

            cursor.GotoNext(MoveType.After,i => i.MatchUnboxAny(out _), i => i.MatchStfld(out _));
            var comRef = cursor.Prev.Operand;

            cursor.Goto(cursorIndex);

            ILLabel label = cursor.DefineLabel();
            ILLabel jumpLabel = cursor.DefineLabel();
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, tempRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchNewCommandsParse), "IsNewCommand"));
            cursor.Emit(OpCodes.Brfalse, label);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, tempRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchNewCommandsParse), "GetNewCommand"));
            cursor.Emit(OpCodes.Stfld, comRef);
            cursor.Emit(OpCodes.Br, jumpLabel);
            cursor.MarkLabel(label);

            cursor.GotoNext(MoveType.After, i => i.MatchUnboxAny(out _), i => i.MatchStfld(out _));
            cursor.MarkLabel(jumpLabel);


            /// Now we do our new commands

            cursor.GotoNext(i => i.MatchLdcI4(151));
            cursor.GotoNext(i => i.MatchLdstr("line"));
            cursor.GotoNext(MoveType.After,i => i.MatchLdfld(out _));

            cursorIndex = cursor.Index;

            cursor.GotoNext(i => i.MatchLdstr("full"));
            cursor.GotoNext(i => i.MatchLdstr("|blank|"));

            cursor.GotoNext(i => i.MatchLdflda(out _));
            var lineBreakRef = cursor.Next.Operand;

            cursor.GotoNext(i => i.MatchLdflda(out _));
            var sizeRef = cursor.Next.Operand;

            cursor.GotoNext(MoveType.After,i=>i.MatchLdarg0(),i => i.MatchLdfld(out _));
            var fontTypeRef = cursor.Prev.Operand;

            cursor.GotoNext(i => i.MatchStfld(out _));
            var textRef = cursor.Next.Operand;

            cursor.GotoNext(i => i.MatchStfld(out _));
            var indexRef = cursor.Next.Operand;

            cursor.GotoNext(i => i.MatchStfld(out _));
            var skipIRef = cursor.Next.Operand;

            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, tempRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldflda, textRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldflda, skipIRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldflda, indexRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldflda, lineBreakRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldflda, sizeRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldflda, fontTypeRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchNewCommandsParse), "CheckNewCommand"));
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, comRef);
        }

        static bool IsNewCommand(string[] temp)
        {
            return Enum.IsDefined(typeof(NewCommand), temp[0].Replace(".", ""));
        }

        static MainManager.Commands GetNewCommand(string[] temp)
        {
            return (MainManager.Commands)Enum.Parse(typeof(NewCommand), temp[0].Replace(".", ""), true);
        }

        static void CheckNewCommand(MainManager.Commands command,string[] temp, ref string text, ref bool skipi, ref int index, ref float? lineBreak, ref Vector2 size, ref int fontType)
        {
            if(Enum.IsDefined(typeof(NewCommand), (int)command))
            {
                NewCommand newCom = (NewCommand)command;
                switch (newCom)
                {
                    case NewCommand.CheckShop:
                        int shopId = Convert.ToInt32(temp[1]);

                        if (MainManager.instance.badgeshops[shopId].Count == 0)
                        {
                            text = MainManager.OrganizeLines(MainManager.GetDialogueText(Convert.ToInt32(temp[2])), lineBreak.Value, size.x, fontType);
                            MainManager.instance.skiptext = false;
                            skipi = true;
                            index = -1;
                        }
                        break;
                }
            }
        }
    }

}
