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

namespace BFPlus.Patches.PauseMenuTranspilers.UpdatePatches
{
    public class PatchTextSkipState : PatchBasePauseMenuUpdate
    {
        public PatchTextSkipState()
        {
            priority = 2095;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchStloc1(), i => i.MatchLdloc1(), i => i.MatchLdcI4(183));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PauseMenu_Ext), "CheckFastTextState"));
        }
    }

    public class PatchEnemyData : PatchBasePauseMenuUpdate
    {
        public PatchEnemyData()
        {
            priority = 3905;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchStfld(AccessTools.Field(typeof(PauseMenu), "enemydata")));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PauseMenu_Ext), "SetEnemyData"));
        }
    }

    public class PatchMpPlusEquip : PatchBasePauseMenuUpdate
    {
        public PatchMpPlusEquip()
        {
            priority = 1005;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdstr("BadgeDequip"));
            int cursorIndex = cursor.Index;

            cursor.GotoPrev(i => i.MatchLdloc(out _));

            var arrayRef = cursor.Next.Operand;
            ILLabel label = null;        
            
            cursor.GotoNext(i => i.MatchBlt(out label));

            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Ldloc_S, arrayRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PauseMenu_Ext), "CanDequip"));
            cursor.Emit(OpCodes.Brfalse, label);

            cursor.GotoNext(i=>i.MatchLdcI4(-1), i=>i.MatchStsfld(out _),i => i.MatchLdstr("BadgeEquip"));
            cursor.Emit(OpCodes.Ldloc_S, arrayRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PauseMenu_Ext), "CanEquip"));
            cursor.Emit(OpCodes.Brfalse, label);
        }
    }

    public class PatchChooseMedalCategory : PatchBasePauseMenuUpdate
    {
        public PatchChooseMedalCategory()
        {
            priority = 988;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcR4(5), i=>i.MatchStfld(AccessTools.Field(typeof(MainManager), "inputcooldown")), i=>i.MatchLdsfld(out _));
            cursor.GotoNext(MoveType.After,i => i.MatchStfld(out _));
            int cursorIndex = cursor.Index;

            cursor.GotoNext(MoveType.After,i => i.MatchCall(AccessTools.Method(typeof(MainManager), "ApplyBadges")));
            var jumpLabel = cursor.Next.Operand;
            cursor.Goto(cursorIndex);

            ILLabel label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchChooseMedalCategory), "CanChooseMedalCategory"));
            cursor.Emit(OpCodes.Brfalse, label);
            cursor.Emit(OpCodes.Br, jumpLabel);
            cursor.MarkLabel(label);
        }

        static bool CanChooseMedalCategory()
        {
            if(MainManager.listtype == (int)NewListType.MedalCategories && MainManager.pausemenu.page == 0 && PauseMenu_Ext.Instance.chooseMedalCategory == -1 && MainManager.instance.option < MainManager.listvar.Length)
            {
                PauseMenu_Ext.Instance.chooseMedalCategory = MainManager.listvar[MainManager.instance.option];
                MainManager.listY = -1;

                MainManager.instance.multilist = PauseMenu_Ext.Instance.GetCategoryMedals();
                MainManager.pausemenu.lastmedal = MainManager.SaveList();
                MainManager.ResetList();
                MainManager.pausemenu.UpdateText();
                MainManager.PlaySound("PageFlip");

                PauseMenu_Ext.MedalCategory category = PauseMenu_Ext.Instance.medalCategories[PauseMenu_Ext.Instance.chooseMedalCategory];

                if(PauseMenu_Ext.Instance.medalCategoryIcon == null)
                {
                    var sprite = category.iconId >= 0 ? PauseMenu_Ext.Instance.categoryIcons[category.iconId] : MainManager.guisprites[Mathf.Abs(category.iconId)];
                    Vector3 position = category.iconId >= 0 ? new Vector2(-1.3f, 4.45f) : new Vector2(-1.35f, 4f);

                    PauseMenu_Ext.Instance.medalCategoryIcon = MainManager.NewUIObject("CategoryIcon", MainManager.pausemenu.boxes[0].transform, position, Vector3.one*0.45f, sprite);
                }

                return true;
            }
            return false;
        }
    }

    public class PatchExitMedalCategory : PatchBasePauseMenuUpdate
    {
        public PatchExitMedalCategory()
        {
            priority = 850;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(3), i => i.MatchLdcI4(0), i => i.MatchCall(AccessTools.Method(typeof(MainManager), "GetKey", new Type[] {typeof(int), typeof(bool)})));
            cursor.GotoNext(i => i.MatchLdcI4(3), i => i.MatchLdcI4(0), i => i.MatchCall(AccessTools.Method(typeof(MainManager), "GetKey", new Type[] { typeof(int), typeof(bool) })));
            cursor.GotoNext(i => i.MatchLdcI4(3), i => i.MatchLdcI4(0), i => i.MatchCall(AccessTools.Method(typeof(MainManager), "GetKey", new Type[] { typeof(int), typeof(bool) })));

            ILLabel label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchExitMedalCategory), "IsInMedalCategory"));
            cursor.Emit(OpCodes.Brfalse, label);
            cursor.Emit(OpCodes.Ret);
            cursor.MarkLabel(label);
        }

        static bool IsInMedalCategory()
        {
            if((PauseMenu_Ext.Instance.chooseMedalCategory != -1 || MainManager.pausemenu.page > 0) && MainManager.GetKey(5, false))
            {
                PauseMenu_Ext.Instance.chooseMedalCategory = -1;
                PauseMenu_Ext.Instance.DestroyMedalCategoryIcon();
                MainManager.instance.inputcooldown = 5f;
                MainManager.listY = -1;
                MainManager.ResetList();
                if (MainManager.pausemenu.page == 0) 
                {
                    MainManager.LoadList(MainManager.pausemenu.lastmedal);
                }

                MainManager.pausemenu.page = 0;
                MainManager.pausemenu.UpdateText();
                MainManager.PlaySound("PageFlip");
                return true;
            }
            return false;
        }
    }


    public class PatchEquippedMedalListBug : PatchBasePauseMenuUpdate
    {
        public PatchEquippedMedalListBug()
        {
            priority = 1204;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdarg0(), i => i.MatchLdfld(AccessTools.Field(typeof(PauseMenu), "lastmedal")));
            cursor.RemoveRange(3);
        }
    }

}
