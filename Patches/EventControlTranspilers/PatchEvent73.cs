using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Patches.EventControlTranspilers
{
    /// <summary>
    /// VEGU FIGHT EVENT
    /// </summary>
    public class PatchVeGuBattleHoaxeIntermissionEvent : PatchBaseEvent73
    {
        public PatchVeGuBattleHoaxeIntermissionEvent()
        {
            priority = 80162;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdnull());

            ILLabel label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Ldc_I4, 928);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchVeGuBattleHoaxeIntermissionEvent), "CheckHoaxeIntermission"));
            cursor.Emit(OpCodes.Brtrue, label);

            cursor.GotoNext(MoveType.After, i=>i.MatchLdfld(out _),i => i.MatchLdcI4(12));

            cursor.GotoNext(i => i.MatchLdnull(), i => i.MatchLdcR4(0.15f));

            cursor.Emit(OpCodes.Ldc_I4, (int)NewEvents.HoaxeIntermission2);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchVeGuBattleHoaxeIntermissionEvent), "StartIntermission"));
            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.Ret).MarkLabel(label);

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchVeGuBattleHoaxeIntermissionEvent), "ResetStuff"));
        }

        static bool CheckHoaxeIntermission(int flag)
        {
            return MainManager.instance.flags[flag];
        }

        static void StartIntermission(int eventId)
        {
            MainManager.events.StartEvent(eventId, null);
        }

        static void ResetStuff()
        {
            //reset venus animstate
            EntityControl venus = MainManager.GetEntity(1);
            venus.animstate = 0;

            //desactivate event trigger
            MainManager.GetEntity(2).gameObject.SetActive(false);

            EntityControl[] party = MainManager.GetPartyEntities();

            foreach(var p in party)
            {
                p.FaceTowards(venus.transform.position);
            }
        }
    }
}
