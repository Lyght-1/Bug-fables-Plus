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

    public class PatchMedalList : PatchBasePauseMenuUpdateText
    {
        public PatchMedalList()
        {
            priority = 530;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            //check medal label text
            cursor.GotoNext(i => i.MatchLdcI4(61));
            cursor.GotoPrev(i => i.MatchLdsfld(out _));
            Utils.RemoveUntilInst(cursor, i => i.MatchCall(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchMedalList), "CheckMedalLabelText"));

            //change showitemlist to list 38
            cursor.GotoNext(i => i.MatchLdcI4(3),i=>i.MatchCall(out _), i => i.MatchLdcI4(0), i => i.MatchLdcI4(0), i => i.MatchCall(out _));
            cursor.Next.OpCode = OpCodes.Ldc_I4;
            cursor.Next.Operand = (int)NewListType.MedalCategories;

            var jumpLabel = cursor.Prev.Operand;
            //in medal category go to the choosen category medals
            cursor.GotoPrev(i => i.MatchLdarg0(), i => i.MatchLdfld(out _), i => i.MatchBrfalse(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchMedalList), "CheckInMedalCategory"));
            cursor.Emit(OpCodes.Brtrue, jumpLabel);

            //if we are on page 0 and list medal categories, render stuff differently
            ILLabel label = cursor.DefineLabel();
            cursor.GotoNext(i => i.MatchLdcI4(32));
            cursor.GotoNext(i => i.MatchLdsfld(out _), i=>i.MatchLdfld(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchMedalList), "CheckMedalList"));
            cursor.Emit(OpCodes.Brfalse,label);
            cursor.Emit(OpCodes.Ret);
            cursor.MarkLabel(label);
        }

        static bool CheckMedalList()
        {
            //Console.WriteLine($"in check medal list, option : {MainManager.instance.option}, listvar length: {MainManager.listvar.Length}, page: {MainManager.pausemenu.page} ");
            if(MainManager.pausemenu.page == 0 && MainManager.listtype == (int)NewListType.MedalCategories && PauseMenu_Ext.Instance.chooseMedalCategory == -1 && MainManager.instance.option < MainManager.listvar.Length)
            {
                MainManager.instance.itemlist.parent = MainManager.pausemenu.boxes[0].transform;
                MainManager.instance.itemlist.localScale = Vector3.one;
                MainManager.instance.itemlist.localPosition = new Vector2(-6f, 3.05f);

                if (MainManager.instance.cursor == null)
                {
                    MainManager.CreateCursor(MainManager.pausemenu.boxes[0].transform);
                    MainManager.instance.cursor.transform.parent = MainManager.pausemenu.boxes[0].transform;
                }

                if (PauseMenu_Ext.Instance.medalCategories == null)
                    PauseMenu_Ext.Instance.SetupMedalCategoriesData();

                PauseMenu_Ext.MedalCategory category = PauseMenu_Ext.Instance.medalCategories[MainManager.listvar[MainManager.instance.option]];
                string descText = $"Browse your {category.name} medals!";

                if (MainManager.listvar[MainManager.instance.option] == 0)
                    descText = "Browse all of your medals!";

                MainManager.pausemenu.StartCoroutine(MainManager.SetText(string.Concat(new object[]
                {
                    "|single||singlebreak,",
                    10f,
                    "|",
                    descText
                }), 0, null, false, false, new Vector3(-5.65f, 0.75f), Vector3.zero, Vector2.one, MainManager.pausemenu.boxes[1].transform, null));

                return true;
            }
            return false;
        }

        static bool CheckInMedalCategory()
        {
            return PauseMenu_Ext.Instance.chooseMedalCategory != -1;
        }

        static string CheckMedalLabelText()
        {
            int page = MainManager.pausemenu.page;
            if (page != 0)
            {
                PauseMenu_Ext.Instance.chooseMedalCategory = -1;
                PauseMenu_Ext.Instance.DestroyMedalCategoryIcon();
            }

            if(page == 0 && PauseMenu_Ext.Instance.chooseMedalCategory != -1 && MainManager.instance.option < MainManager.listvar.Length)
            {
                PauseMenu_Ext.MedalCategory category = PauseMenu_Ext.Instance.medalCategories[PauseMenu_Ext.Instance.chooseMedalCategory];
                return category.name;
            }
            return MainManager.menutext[page == 0 ? 27 : page == 1 ? 260 : 61];
        }
    }
}
