using HarmonyLib;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Mono.Cecil.Cil;
using OpCodes = Mono.Cecil.Cil.OpCodes;
using Mono.Cecil;
using MonoMod.Utils;
namespace BFPlus.Extensions
{
    public static class Utils
    {
        public static void InsertYieldReturn(this ILGenerator gen, List<CodeInstruction> instructions, int index, List<CodeInstruction> oldInstructions)
        {
            var declaringType = ((FieldInfo)instructions[1].operand).DeclaringType;

            var f_state = declaringType.GetField("<>1__state", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var f_current = declaringType.GetField("<>2__current", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var jumpTable = instructions.Find(ins => ins.opcode == System.Reflection.Emit.OpCodes.Switch);
            var jumpTableLabels = ((Label[])jumpTable.operand).ToList();

            int nextReturnIndex = jumpTableLabels.Count;

            var newLabel = gen.DefineLabel();
            jumpTableLabels.Add(newLabel);
            jumpTable.operand = jumpTableLabels.ToArray();

            var newInstructions = new List<CodeInstruction>()
            {
                new CodeInstruction(System.Reflection.Emit.OpCodes.Stfld, f_current),
                new CodeInstruction(System.Reflection.Emit.OpCodes.Ldarg_0),

                // Save indexInserted of the new label and return
                new CodeInstruction(System.Reflection.Emit.OpCodes.Ldc_I4, nextReturnIndex),
                new CodeInstruction(System.Reflection.Emit.OpCodes.Stfld, f_state),
                new CodeInstruction(System.Reflection.Emit.OpCodes.Ldc_I4_1),
                new CodeInstruction(System.Reflection.Emit.OpCodes.Ret),

                // Jump here when the method is called again
                new CodeInstruction(System.Reflection.Emit.OpCodes.Ldarg_0).WithLabels(newLabel),
                new CodeInstruction(System.Reflection.Emit.OpCodes.Ldc_I4_M1),
                new CodeInstruction(System.Reflection.Emit.OpCodes.Stfld, f_state)
            };
            instructions.InsertRange(index, newInstructions);
        }

        public static void InsertYieldReturn(ILCursor cursor)
        {
            var declaringType = ((FieldReference)cursor.Body.Instructions[1].Operand).DeclaringType.Resolve();

            var f_state = declaringType.FindField("<>1__state");
            var f_current = declaringType.FindField("<>2__current");

            int cursorIndex = cursor.Index;

            cursor.Goto(0);
            ILLabel[] labels = new ILLabel[0];
            cursor.GotoNext(i => i.MatchSwitch(out labels));
            var tempList = labels.ToList();
            int nextReturnIndex = tempList.Count;

            var newLabel = cursor.DefineLabel();
            tempList.Add(newLabel);
            cursor.Next.Operand = tempList.ToArray();
            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Stfld, f_current);
            cursor.Emit(OpCodes.Ldarg_0);

            cursor.Emit(OpCodes.Ldc_I4, nextReturnIndex);
            cursor.Emit(OpCodes.Stfld, f_state);
            cursor.Emit(OpCodes.Ldc_I4_1);
            cursor.Emit(OpCodes.Ret).MarkLabel(newLabel);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldc_I4_M1);
            cursor.Emit(OpCodes.Stfld, f_state);
        }

        public static int GetILIndex(List<CodeInstruction> instructions, Func<List<CodeInstruction>, int, bool> pattern, int startIndex)
        {
            int j = 0;
            do
            {
                j++;
            } while (!pattern(instructions,j+startIndex));
            return j;
        }

        public static void InsertWaitStylish(List<CodeInstruction> newInstructions, List<CodeInstruction> instructionsList, ILGenerator generator, float waitFrames=0f)
        {
            var waitStylish = AccessTools.Method(typeof(BattleControl_Ext), "WaitStylish");
            newInstructions.Add(new CodeInstruction(System.Reflection.Emit.OpCodes.Ldc_R4, waitFrames));
            newInstructions.Add(new CodeInstruction(System.Reflection.Emit.OpCodes.Call, waitStylish));
            generator.InsertYieldReturn(newInstructions, newInstructions.Count, instructionsList);
        }

        public static void InsertStartStylishTimer(List<CodeInstruction> instructions, float startFrames, float endFrames, int stylishID =0, bool commandSuccess=true)
        {
            var startStylishTimer = AccessTools.Method(typeof(BattleControl_Ext), "StartStylishTimer");
            instructions.Add(new CodeInstruction(System.Reflection.Emit.OpCodes.Ldc_R4, startFrames));
            instructions.Add(new CodeInstruction(System.Reflection.Emit.OpCodes.Ldc_R4, endFrames));
            instructions.Add(new CodeInstruction(System.Reflection.Emit.OpCodes.Ldc_I4, stylishID));
            instructions.Add(new CodeInstruction(commandSuccess ? System.Reflection.Emit.OpCodes.Ldc_I4_1 : System.Reflection.Emit.OpCodes.Ldc_I4_0));
            instructions.Add(new CodeInstruction(System.Reflection.Emit.OpCodes.Call, startStylishTimer));
        }

        public static void InsertStartStylishTimer(ILCursor cursor, float startFrames, float endFrames, int stylishID = 0, bool commandSuccess = true)
        {
            cursor.Emit(OpCodes.Ldc_R4, startFrames);
            cursor.Emit(OpCodes.Ldc_R4, endFrames);
            cursor.Emit(OpCodes.Ldc_I4, stylishID);
            cursor.Emit(commandSuccess ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "StartStylishTimer"));
        }

        public static void InsertWaitStylish(ILCursor cursor, float waitFrames = 0f)
        {
            cursor.Emit(OpCodes.Ldc_R4, waitFrames);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "WaitStylish"));
            InsertYieldReturn(cursor);
        }

        public static void RemoveUntilInst(ILCursor cursor, params Func<Instruction,bool>[] predicates)
        {
            int cursorIndex = cursor.Index;
            cursor.GotoNext(predicates);
            int matchIndex = cursor.Index;
            cursor.Goto(cursorIndex);
            cursor.RemoveRange(matchIndex - cursorIndex);
        }
    }
}
