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

namespace BFPlus.Patches.EntityControlTranspilers
{

    /// <summary>
    /// Make sure that everlasting flame sprite doesnt get the mystery sprite one
    /// </summary>
    public class PatchEverLastingFlameMysterySprite : PatchBaseEntityControlUpdateItem
    {
        public PatchEverLastingFlameMysterySprite()
        {
            priority = 59;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            ILLabel label = null;
            cursor.GotoNext(MoveType.After, i => i.MatchLdcI4(59), i=>i.MatchBeq(out label));
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, AccessTools.Field(typeof(EntityControl), "itemstate"));
            cursor.Emit(OpCodes.Ldc_I4, (int)Medal.EverlastingFlame);
            cursor.Emit(OpCodes.Beq, label);
        }
    }
}
