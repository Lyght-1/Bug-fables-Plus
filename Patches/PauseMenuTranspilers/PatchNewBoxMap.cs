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

namespace BFPlus.Patches.PauseMenuTranspilers
{

    public class PatchNewBoxMap : PatchBaseMapSetup
    {
        static AccessTools.FieldRef<PauseMenu, DialogueAnim[]> boxesRef = AccessTools.FieldRefAccess<PauseMenu, DialogueAnim[]>("boxes");
        public PatchNewBoxMap()
        {
            priority = 105;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchStfld(AccessTools.Field(typeof(PauseMenu),"boxes")));
            cursor.GotoNext(MoveType.After,i=>i.MatchNewobj(out _),i => i.MatchCallvirt(out _));

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchNewBoxMap), "CreateCrystalBox"));
        }

        static void CreateCrystalBox(PauseMenu pauseMenu)
        {
            MainManager.NewUIObject("textholder2", boxesRef(pauseMenu)[0].transform.GetChild(0).transform, new Vector3(6, 0), new Vector3(1f, 1f), MainManager.guisprites[0], 20).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.65f);
            boxesRef(pauseMenu)[0].transform.GetChild(0).transform.localPosition = new Vector3(-3.4f, 8.45f);

            PauseMenu_Ext.Instance.SetupAreaItemData();

        }
    }
}
