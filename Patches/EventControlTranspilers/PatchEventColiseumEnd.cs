using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Patches.EventControlTranspilers
{

    public class PatchEventColiseumEnd : PatchBaseColiseumEnd
    {
        public PatchEventColiseumEnd()
        {
            priority = 0;
        }
        protected override void ApplyPatch(ILCursor cursor)
        { 
            cursor.GotoNext(i => i.MatchLdstr("CrowdChatter"));
            int cursorIndex = cursor.Index;
            cursor.GotoNext(i => i.MatchLdfld(out _));
            var winVar = cursor.Next.Operand;
            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, winVar);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchEventColiseumEnd), "CheckColiseumWinFlag"));
        }

        static void CheckColiseumWinFlag(bool win)
        {
            if (MainManager.instance.flags[871])
            {
                MainManager.instance.flags[872] = win;
            }
        }
    }
}
