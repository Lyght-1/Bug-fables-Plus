using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Patches.BattleControlTranspilers.AddExperiencePatches
{
    public class PatchExp : PatchBaseAddExperience
    {
        public PatchExp()
        {
            priority = 13462;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            ILLabel label = null;
            cursor.GotoNext(i=>i.MatchBge(out label),i => i.MatchLdsfld(out _), i => i.MatchLdfld(out _), i => i.MatchLdcI4(656));
            cursor.GotoNext();
            cursor.Next.OpCode = OpCodes.Nop;
            cursor.GotoNext();
            cursor.Next.OpCode = OpCodes.Nop;
            cursor.GotoNext();
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchExp), "CheckExp"));
            cursor.Emit(OpCodes.Br, label);

            int cursorIndex = cursor.Index;

            cursor.GotoNext(i => i.MatchLdstr(out _));
            int removeIndex = cursor.Index;
            cursor.Goto(cursorIndex);
            cursor.RemoveRange(removeIndex - cursorIndex);
        }
        static void CheckExp()
        {
            if (MainManager.instance.partylevel >= 39)
            {
                MainManager.instance.neededexp += 10;
            }
            else if (MainManager.instance.flags[656] || MainManager.instance.partylevel >= 32)
            {
                MainManager.instance.neededexp += 5;
            }
            else if (MainManager.instance.partylevel >= 24)
            {
                MainManager.instance.neededexp += 3;
            }
            else if (MainManager.instance.partylevel >= 15)
            {
                MainManager.instance.neededexp += 2;
            }
            else
            {
                MainManager.instance.neededexp++;
            }
        }
    }
}
