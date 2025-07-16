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

    public class PatchSpyGuyAddNewBosses : PatchBaseEvent65
    {
        public PatchSpyGuyAddNewBosses()
        {
            priority = 71433;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchLdcI4(32), i=>i.MatchCallvirt(out _));

            cursor.Emit(OpCodes.Ldloc_3);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchSpyGuyAddNewBosses), "AddNewBosses"));
            cursor.Emit(OpCodes.Stloc_3);
        }

        static List<int> AddNewBosses(List<int> bosses)
        {
            bosses.AddRange(new int[] {
                (int)NewEnemies.DynamoSpore,
                (int)NewEnemies.DullScorp,
                (int)NewEnemies.Belosslow,
                (int)NewEnemies.IronSuit,
                (int)NewEnemies.Jester,
                (int)MainManager.Enemies.HoloVi,
                (int)MainManager.Enemies.HoloKabbu,
                (int)MainManager.Enemies.HoloLeif,
                (int)MainManager.Enemies.TANGYBUG,
                (int)NewEnemies.TermiteKnight,
                (int)NewEnemies.Mars,
                (int)NewEnemies.DarkVi,
                (int)NewEnemies.DarkKabbu,
                (int)NewEnemies.DarkLeif,
                (int)NewEnemies.LeafbugShaman,
                (int)NewEnemies.Patton,
                (int)NewEnemies.Levi,
                (int)NewEnemies.Celia,
            });
            return bosses;

        }
    }
}
