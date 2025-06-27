using BFPlus.Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Patches.SetTextPatches
{
    //Make it so chuck always give mighty pebble instead of randomize it in MYSTERY?
    //still not sure if its the way to go
    /*public class PatchPebbleTossGiveItem : PatchBaseSetText
    {
        bool foundIndex;

        public override bool Condition(List<CodeInstruction> instructions, int index)
        {
            if (instructions[index].LoadsConstant(681))
                foundIndex = true;

            return foundIndex && instructions[index].LoadsConstant(11);
        }

        public override void InsertPatch(List<CodeInstruction> newInstructions, List<CodeInstruction> oldInstructions, ILGenerator generator, ref int index)
        {
            newInstructions.Add(oldInstructions[index]);
            newInstructions.Add(oldInstructions[index+1]);

            newInstructions.Add(oldInstructions[index-1]);
            newInstructions.Add(new CodeInstruction(OpCodes.Ldc_I4_S, (int)MainManager.BadgeTypes.MightyPeeble));
            newInstructions.Add(oldInstructions[index + 1]);

            index++;
        }
    }*/
}
