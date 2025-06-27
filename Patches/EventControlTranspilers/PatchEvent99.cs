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
    /// B33 Fight Event
    /// </summary>
    public class PatchB33BattleHoaxeIntermissionEvent : PatchBaseEvent99
    {
        public PatchB33BattleHoaxeIntermissionEvent()
        {
            priority = 114334;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchBr(out _));

            ILLabel label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Ldc_I4, 929);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchVeGuBattleHoaxeIntermissionEvent), "CheckHoaxeIntermission"));
            cursor.Emit(OpCodes.Brtrue, label);

            cursor.GotoNext(MoveType.After, i => i.MatchLdcI4(54));

            cursor.GotoNext(i => i.MatchLdarg0(), i => i.MatchLdfld(out _));

            cursor.Emit(OpCodes.Ldc_I4, (int)NewEvents.HoaxeIntermission3);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchVeGuBattleHoaxeIntermissionEvent), "StartIntermission"));
            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.Ret).MarkLabel(label);

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchB33BattleHoaxeIntermissionEvent), "ResetStuff"));
        }

        static void ResetStuff()
        {
            //desactivate overseer;
            MainManager.GetEntity(1).gameObject.SetActive(false);
            EntityControl[] party = MainManager.GetPartyEntities(true);

            Vector3[] positions = new Vector3[]
            {
                new Vector3(-1, 0, 16.57f),
                new Vector3(-2.63f, 0, 17.37f),
                new Vector3(-4.44f, 0, 16.95f)
            };
            for(int i=0; i<party.Length; i++)
            {
                party[i].transform.position = positions[i];
            }

            //eri pos fix
            MainManager.GetEntity(7).transform.position = new Vector3(-6.05f, 0f, 18.75f);

            //gate wall to core need to be desactivated
            MainManager.map.mainmesh.GetChild(0).GetChild(0).gameObject.SetActive(false);

            //add chargue guard reward after b33 boss
            MainManager.instance.badgeshops[1].Add((int)Medal.ChargeGuard);
        }
    }
}
