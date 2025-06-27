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
using UnityEngine;

namespace BFPlus.Patches.EventControlTranspilers
{
    /// <summary>
    /// Ch4 get artifact event
    /// </summary>
    public class PatchChapter4HoaxeIntermissionEvent : PatchBaseEvent118
    {
        public PatchChapter4HoaxeIntermissionEvent()
        {
            priority = 139999;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchCall(AccessTools.Method(typeof(MainManager), "GetPartyEntities", new Type[] {typeof(bool)})),
                i=>i.MatchStfld(out _));

            ILLabel label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Ldc_I4, 937);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchVeGuBattleHoaxeIntermissionEvent), "CheckHoaxeIntermission"));
            cursor.Emit(OpCodes.Brtrue, label);

            cursor.GotoNext(i => i.MatchLdcI4(9));

            cursor.GotoNext(i => i.MatchLdcR4(out _));

            cursor.Emit(OpCodes.Ldc_I4, (int)NewEvents.HoaxeIntermission4);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchVeGuBattleHoaxeIntermissionEvent), "StartIntermission"));
            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.Ret).MarkLabel(label);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchChapter4HoaxeIntermissionEvent), "ResetStuff"));
        }

        static void ResetStuff()
        {
            EntityControl[] party = MainManager.GetPartyEntities(true);

            Vector3[] positions = new Vector3[]
            {
                new Vector3(0f, 1f, -2.25f),
                new Vector3(-2f, 1f, -1.5f),
                new Vector3(2f, 1f, -1.5f)
            };
            for (int i = 0; i < party.Length; i++)
            {
                party[i].transform.position = positions[i];
            }

            if (MainManager.map.chompy != null)
                MainManager.map.chompy.transform.position = positions[0];

            //unlock recharge
            MainManager.instance.badgeshops[1].Add((int)Medal.Recharge);
            UnityEngine.Object.Destroy(MainManager.map.transform.Find("artifacts_0").gameObject);
            EventControl.call = MainManager.GetEntity(2).npcdata;
        }
    }
}
