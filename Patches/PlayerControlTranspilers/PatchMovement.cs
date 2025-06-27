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

namespace BFPlus.Patches.PlayerControlTranspilers
{

    public class PatchCheckPlayerWalkSound : PatchBasePlayerControlMovement
    {
        public PatchCheckPlayerWalkSound()
        {
            priority = 263;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdstr("Footstep"));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchCheckPlayerWalkSound), "CheckWalkSound"));
            cursor.RemoveRange(2);
        }

        static void CheckWalkSound(EntityControl entity)
        {
            if (MainManager.BadgeIsEquipped((int)Medal.GamerFX))
            {
                entity.PlaySound("MKWalk",0.25f,1.5f);

            }
            else
            {
                entity.PlaySound("Footstep");
            }
        }
    }
}
