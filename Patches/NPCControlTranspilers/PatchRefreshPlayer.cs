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

namespace BFPlus.Patches.NPCControlTranspilers
{
    //This is for the seedling minigame, too many lost noise iirc
    public class PatchEnemySound : PatchBaseNPCControlRefreshPlayer
    {
        public PatchEnemySound()
        {
            priority = 57;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchCallvirt(AccessTools.Method(typeof(EntityControl), "PlaySound", new Type[] { typeof(string) })));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "CheckNPCEnemySound"));
            cursor.Remove();
        }
    }
}
