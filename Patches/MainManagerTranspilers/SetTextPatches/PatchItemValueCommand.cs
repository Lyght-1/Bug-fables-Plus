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

namespace BFPlus.Patches.MainManagerTranspilers.SetTextPatches
{
    /// <summary>
    /// Make sure that we set the flagvar 10 value (cb price of medal) when setting itemvalue (in that case medal value)
    /// </summary>
    public class PatchItemValueCommand : PatchBaseSetText
    {
        public PatchItemValueCommand()
        {
            priority = 29576;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdcI4(681), i=>i.MatchLdelemU1(), i=>i.MatchBrtrue(out _));
            cursor.GotoNext(i => i.MatchLdcI4(681), i => i.MatchLdelemU1(), i => i.MatchBrtrue(out _));
            cursor.GotoNext(MoveType.After,i => i.MatchStelemI4());
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchItemValueCommand), "SetCrystalBerryPrice"));
        }

        static void SetCrystalBerryPrice()
        {
            if (MainManager.instance.flags[681])
            {
                MainManager.instance.flagvar[10] = MainManager.instance.flagvar[66] >= 2 ? 4 : 3;
                return;
            }

            MainManager.instance.flagvar[10] = Convert.ToInt32(MainManager.badgedata[MainManager.instance.flagvar[0], 7]);
        }
    }
}
