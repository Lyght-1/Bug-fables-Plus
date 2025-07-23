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

namespace BFPlus.Patches.MainManagerTranspilers.ShowItemListPatches
{
    /// <summary>
    /// Pretty much setting our badge shops list so it goes through the same stuff as prizes medal list
    /// here for the list sprite
    /// </summary>
    public class PatchBadgeShopListSprite : PatchBaseShowItemList
    {
        public PatchBadgeShopListSprite()
        {
            priority = 74294;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            ILLabel label = null;
            cursor.GotoNext(MoveType.After,i => i.MatchLdarg0(), i => i.MatchLdcI4(33), i => i.MatchBeq(out label));

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldc_I4, (int)NewListType.BadgeShops);
            cursor.Emit(OpCodes.Beq, label);

            ILLabel label2 = cursor.DefineLabel();

            cursor.GotoNext(i => i.MatchLdcI4(34));

            cursor.GotoNext(i => i.MatchLdarg0(), i => i.MatchLdcI4(34), i => i.MatchBneUn(out _));
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldc_I4, (int)NewListType.BadgeShops);
            cursor.Emit(OpCodes.Beq, label2);

            cursor.GotoNext(i => i.MatchLdsfld(out _));
            cursor.MarkLabel(label2);

            cursor.GotoNext(i => i.MatchLdstr("itemsprite"));
            var textRef = cursor.Prev.Operand;

            cursor.GotoNext(MoveType.After,i => i.MatchLdcI4(1), i => i.MatchLdsfld(out _), i => i.MatchLdloc(out _));
            var indexRef = cursor.Prev.Operand;

            cursor.GotoNext(i => i.MatchLdcI4(190));

            cursor.GotoNext(i => i.MatchLdloc(out _));
            var barRef = cursor.Next.Operand;

            cursor.GotoNext(MoveType.After, i => i.MatchLdcR4(0.6f), i=>i.MatchLdcR4(1f), i=>i.MatchNewobj(out _), i=>i.MatchCallvirt(out _));

            cursor.Emit(OpCodes.Ldloc, indexRef);
            cursor.Emit(OpCodes.Ldloca, textRef);
            cursor.Emit(OpCodes.Ldloca, barRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchBadgeShopListSprite), "AddMedalPrice"));
        }

        static void AddMedalPrice(int index, ref string text, ref SpriteRenderer bar)
        {
            bool underground = MainManager.map.mapid == MainManager.Maps.UndergroundBar;
            int price = Convert.ToInt32(MainManager.badgedata[MainManager.listvar[index], underground ? 7 : 5]);

            if (MainManager.instance.flags[681])
            {
                if (!underground)
                    price = 35; //Mystery price of 35 to every medal
                else
                    price = MainManager_Ext.MYSTERY_SHADE_PRICE;
            }

            MainManager.instance.StartCoroutine(MainManager.SetText("|size,0.6,0.8||single|" + price, 0, null, false, false, new Vector3(underground ? 1.9f : 1.6f, -0.25f), Vector3.zero, Vector3.one, bar.transform, null));

            Vector3 size = underground ? new Vector3(0.45f, 0.5f, 1f) : new Vector3(0.5f, 0.55f, 1f);
            MainManager.NewUIObject("currencySprite", bar.transform, new Vector2(2.5f, 0f), size, MainManager.guisprites[underground ? 83 : 29], 5);
        }
    }

    /// <summary>
    /// and here for the badge data
    /// </summary>
    public class PatchBadgeShopListData : PatchBaseShowItemList
    {
        public PatchBadgeShopListData()
        {
            priority = 75173;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            ILLabel label = null;
            cursor.GotoNext(MoveType.After, i => i.MatchLdarg0(), i => i.MatchLdcI4(33), i => i.MatchBeq(out label));

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldc_I4, (int)NewListType.BadgeShops);
            cursor.Emit(OpCodes.Beq, label);

            ILLabel label2 = null;
            cursor.GotoNext(MoveType.After, i => i.MatchLdarg0(), i => i.MatchLdcI4(33), i => i.MatchBeq(out label2));

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldc_I4, (int)NewListType.BadgeShops);
            cursor.Emit(OpCodes.Beq, label2);
        }
    }

}
