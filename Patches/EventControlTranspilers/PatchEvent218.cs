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

namespace BFPlus.Patches.EventControlTranspilers
{
    //CHECKING MEDALS IN HB'S LAB EVENT
    /// <summary>
    /// Add holocloak to the medal that you can check in hb's lab
    /// </summary>
    public class PatchHoloCloakCheck : PatchBaseEvent218
    {
        public PatchHoloCloakCheck()
        {
            priority = 257617;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdstr("|face,party,caller|"));
            cursor.Emit(OpCodes.Ldloc_1);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchHoloCloakCheck), "CheckMedalID"));
            cursor.Emit(OpCodes.Stloc_1);
        }

        static int CheckMedalID(int num)
        {
            var callRef = AccessTools.StaticFieldRefAccess<NPCControl>(typeof(EventControl), "call");

            if(MainManager.map.mapid == MainManager.Maps.HBsLab)
            {
                if (callRef.mapid == 6)
                {
                    return (int)MainManager.BadgeTypes.HoloCloak;
                }
            }

            return num;
        }
    }
}
