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
