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

namespace BFPlus.Patches.BattleControlTranspilers.CalculateBaseDamagePatches
{
    public class PatchGetRealDamage : PatchBaseCalculateBaseDamage
    {
        public PatchGetRealDamage()
        {
            priority = 0;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.Goto(0);

            Instruction baseValue = null;
            while (cursor.TryGotoNext(i=>i.MatchAdd()))
            {
                if (baseValue == null) 
                {
                    baseValue = cursor.Body.Instructions[cursor.Index + 2];
                    continue;
                }

                if(cursor.Body.Instructions[cursor.Index + 1].Match(baseValue.OpCode, baseValue.Operand))
                {
                    cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "SetRealDamage"));
                    cursor.GotoNext();
                }
            }
            cursor.Goto(0);
        }

    }
}
