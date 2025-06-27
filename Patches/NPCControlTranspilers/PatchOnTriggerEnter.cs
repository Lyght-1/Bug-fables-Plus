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

    public class PatchFreezeImmune : PatchBaseNPCControlOnTriggerEnter
    {
        public PatchFreezeImmune()
        {
            priority = 2146;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            ILLabel label = null;
            cursor.GotoNext(MoveType.After,i => i.MatchLdcI4(400), i=>i.MatchBeq(out label));

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchFreezeImmune), "IsFreezeImmune"));
            cursor.Emit(OpCodes.Brtrue, label);
        }

        static bool IsFreezeImmune(NPCControl npc)
        {
            return MainManager_Ext.IsNewEnemy(npc.entity, NewEnemies.FireAnt) || MainManager_Ext.IsNewEnemy(npc.entity, NewEnemies.Moeruki)
                || npc.entity.animid == (int)NewAnimID.FirePopper;
        }
    }
}
