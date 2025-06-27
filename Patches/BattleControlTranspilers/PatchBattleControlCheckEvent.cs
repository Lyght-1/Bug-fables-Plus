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

namespace BFPlus.Patches.BattleControlTranspilers
{
    /// <summary>
    /// We add our check for the stylish tutorial before the exit battle thing in maki tutorial fight
    /// </summary>
    public class PatchStylishTutorial : PatchBaseBattleControlCheckEvent
    {
        public PatchStylishTutorial()
        {
            priority = 74;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdsfld(out _), i=>i.MatchLdfld(out _), i=>i.MatchLdcI4(11), i=>i.MatchLdelemI4(), i=>i.MatchLdcI4(3));
            cursor.Next.OpCode = OpCodes.Nop;
            cursor.GotoNext(i => i.MatchLdfld(out _));

            ILLabel label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchStylishTutorial), "CheckStylishTutorial"));
            cursor.Emit(OpCodes.Brfalse, label);
            cursor.Emit(OpCodes.Ret).MarkLabel(label);

            cursor.Emit(OpCodes.Ldsfld, AccessTools.Field(typeof(MainManager), "instance"));
        }


        static bool CheckStylishTutorial()
        {
            if(MainManager.battle.turns ==2 && !MainManager.instance.flags[963])
            {
                MainManager.battle.StartCoroutine(MainManager.battle.EventDialogue((int)NewEventDialogue.StylishTutorial));
                return true;
            }
            return false;
        }
    }
}
