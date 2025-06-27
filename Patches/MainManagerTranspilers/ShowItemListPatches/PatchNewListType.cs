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
    public class PatchNewListType : PatchBaseShowItemList
    {
        public PatchNewListType()
        {
            priority = 70776;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchStloc2());
            int cursorIndex = cursor.Index;

            ILLabel label = null;
            cursor.GotoNext(i=>i.MatchInitobj(out _),i => i.MatchBr(out label));
            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "CheckListType"));
            cursor.Emit(OpCodes.Brtrue, label);
        }
    }

    public class PatchNewListText : PatchBaseShowItemList
    {
        public PatchNewListText()
        {
            priority = 71654;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(
                i=>i.MatchLdarg0(),
                i=>i.MatchLdcI4(22),
                i=>i.MatchBneUn(out _), 
                i => i.MatchLdstr("Data/Dialogues"));

            int cursorIndex = cursor.Index;

            cursor.GotoPrev(MoveType.After,i => i.MatchLdstr(""), i => i.MatchStloc(out _));
            var textRef = cursor.Prev.Operand;
            cursor.GotoNext(MoveType.After,i => i.MatchLdstr("Bar"), i=>i.MatchLdloc(out _));
            var idRef = cursor.Prev.Operand;
            cursor.GotoNext(i => i.MatchStloc(out _));
            var barRef = cursor.Next.Operand;

            cursor.GotoNext(i => i.MatchLdcR4(0.7f));
            var barYOffsetRef = cursor.Prev.Operand;
            cursor.GotoNext(MoveType.After,i => i.MatchLdcR4(-2));
            var textSizeX = cursor.Next.Operand;
            cursor.GotoNext(MoveType.After, i => i.MatchLdcR4(-0.15f));
            var textSizeY = cursor.Next.Operand;

            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldloc, idRef);
            cursor.Emit(OpCodes.Ldloc, barRef);
            cursor.Emit(OpCodes.Ldloca, barYOffsetRef);
            cursor.Emit(OpCodes.Ldloca, textSizeX);
            cursor.Emit(OpCodes.Ldloca, textSizeY);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "GetNewListText"));
            cursor.Emit(OpCodes.Stloc, textRef);
        }
    }

    public class PatchNewListDesc : PatchBaseShowItemList
    {
        public PatchNewListDesc()
        {
            priority = 75145;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdsfld(AccessTools.Field(typeof(MainManager), "listdescbox")));
            cursor.GotoNext(MoveType.After,i => i.MatchStloc(out _));

            int cursorIndex = cursor.Index;
            cursor.GotoPrev(i => i.MatchStloc(out _), i => i.MatchLdstr(""), i => i.MatchStloc(out _));
            var textRef = cursor.Next.Operand;
            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "GetNewListDesc"));
            cursor.Emit(OpCodes.Stloc, textRef);
        }
    }
}
