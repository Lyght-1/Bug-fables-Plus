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

namespace BFPlus.Patches
{
    public class PatchSetSkillListType : PatchBaseMainManagerGetTPCost
    {
        public PatchSetSkillListType()
        {
            priority = 0;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdfld(typeof(MainManager).GetField("playerdata")));
            cursor.GotoNext(MoveType.After,i => i.MatchLdfld(typeof(MainManager).GetField("playerdata")));
            cursor.Emit(OpCodes.Ldsfld, AccessTools.Field(typeof(MainManager_Ext), "skillListType"));
            cursor.Remove();
        }
    }
}
