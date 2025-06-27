using BFPlus.Extensions;
using BFPlus.Patches.BattleControlTranspilers.SetItemPatches;
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

namespace BFPlus.Patches.MainManagerTranspilers.UpdatePatches
{
    public class PatchGourmetList : PatchBaseMainManagerUpdate
    {
        public PatchGourmetList()
        {
            priority = 2182;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdsfld(AccessTools.Field(typeof(MainManager), "battle")));
            cursor.Prev.OpCode = OpCodes.Nop;

            int cursorIndex = cursor.Index;
            ILLabel label = null;
            cursor.GotoNext(i => i.MatchBr(out label));
            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchGourmetList), "DoGourmetList"));
            cursor.Emit(OpCodes.Brtrue, label);
            cursor.Emit(OpCodes.Ldsfld, AccessTools.Field(typeof(MainManager), "battle"));
        }

        static bool DoGourmetList()
        {
            if (MainManager.listtype == (int)NewListType.GourmetItem)
            {
                if (MainManager.listvar[MainManager.instance.option] == 0)
                {
                    AccessTools.Method(typeof(MainManager), "DestroyList").Invoke(MainManager.instance, null);
                    MainManager.battle.currentaction = BattleControl.Pick.ItemList;
                    MainManager.battle.excludeself = false;
                    MainManager.SetUpList(0, true, false);
                    MainManager.listammount = 5;
                    MainManager.ShowItemList(0, MainManager.defaultlistpos, true, false);
                }
                else
                {
                    int tpCost = MainManager_Ext.GetDoubleDipCost();
                    if (MainManager.instance.tp >= tpCost)
                    {
                        MainManager.instance.tp = Mathf.Clamp(MainManager.instance.tp- tpCost, 0,MainManager.instance.maxtp);
                        BattleControl_Ext.Instance.gourmetItemUse = 1;
                        AccessTools.Method(typeof(MainManager), "DestroyList").Invoke(MainManager.instance, null);
                        MainManager.battle.currentaction = BattleControl.Pick.ItemList;
                        MainManager.battle.excludeself = false;
                        MainManager.SetUpList(0, true, false);
                        MainManager.listammount = 5;
                        MainManager.ShowItemList(0, MainManager.defaultlistpos, true, false);
                    }
                    else
                    {
                        MainManager.PlayBuzzer();
                        //MainManager.hudsprites[MainManager.BadgeIsEquipped(72, num6) ? num6 : 3].color = Color.red;
                    }
                }
                return true;
            }
            return false;
        }
    }

    public class PatchCancelListGourmet : PatchBaseMainManagerUpdate
    {
        public PatchCancelListGourmet()
        {
            priority = 2398;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchCallvirt(AccessTools.Method(typeof(BattleControl), "CancelList")));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "CheckCancelListDoubleDip"));
        }
    }
}
