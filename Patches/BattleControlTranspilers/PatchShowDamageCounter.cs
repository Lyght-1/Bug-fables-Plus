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

    public class PatchShowDamageCounter : PatchBaseShowDamageCounter
    {
        public PatchShowDamageCounter()
        {
            priority = 0;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdfld(AccessTools.Field(typeof(BattleControl), "counterspriteindex")), i=>i.MatchLdarg1());
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchShowDamageCounter), "GetCounterSprite"));
            cursor.Remove();
        }


        static int GetCounterSprite(int[] counterSprite, int type)
        {
            if(type == 3)
            {
                return (int)NewGui.TPLossFront;
            }
            return counterSprite[type];
        }
    }
}
