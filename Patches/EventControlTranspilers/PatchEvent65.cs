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
    //Guy that sells Spies Event

    /// <summary>
    /// Remove mars sprout from the potential ids, and re-add tanjy
    /// </summary>
    public class PatchSpyGuyExcludeIds : PatchBaseEvent65
    {
        public PatchSpyGuyExcludeIds()
        {
            priority = 71465;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdfld(AccessTools.Field(typeof(EventControl), "excludeids")));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchSpyGuyExcludeIds), "GetExcludeIds"));
        }

        static List<int> GetExcludeIds(List<int> excludeIds)
        {
            excludeIds.RemoveAll(e=> e == (int)MainManager.Enemies.TANGYBUG || e == (int)MainManager.Enemies.HoloKabbu || e == (int)MainManager.Enemies.HoloLeif|| e== (int)MainManager.Enemies.HoloVi);
            excludeIds.Add((int)NewEnemies.MarsSprout);
            return excludeIds;
        }
    }
}
