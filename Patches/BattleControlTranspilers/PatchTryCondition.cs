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
    //Remove every update anim call
    public class PatchTryCondition : PatchBaseBattleControlTryCondition
    {
        public PatchTryCondition()
        {
            priority = 0;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.Goto(0);

            var updateAnimRef = AccessTools.Method(typeof(BattleControl), "UpdateAnim", new Type[] { });
            while (cursor.TryGotoNext(i => i.MatchLdarg0(), i=>i.MatchCall(updateAnimRef)))
            {
                for(int i = 0; i < 2; i++)
                {
                    cursor.Next.OpCode = OpCodes.Nop;
                    cursor.GotoNext();
                }
            }
            cursor.Goto(0);
        }

    }
}
