using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine.SocialPlatforms;

namespace BFPlus.Patches.ShowItemListPatches
{
    public class PatchDestinyDream : PatchBaseShowItemList
    {
        public PatchDestinyDream()
        {
            priority = 72223;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {

            cursor.GotoNext(MoveType.Before, i => i.MatchLdloc(out _), i => i.MatchLdcI4(0), i => i.MatchClt(), i => i.MatchBr(out _));
            var label = cursor.Prev.Operand;
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "DestinyDreamCheck"));
            cursor.Emit(OpCodes.Brtrue, label);
        }
    }

    public class PatchDestinyDreamSprite : PatchBaseShowItemList
    {
        public PatchDestinyDreamSprite()
        {
            priority = 72347;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            int originalIndex = cursor.Index;

            cursor.GotoPrev(i => i.MatchLdcI4(72));
            Instruction localText3 = cursor.Prev;
            cursor.Goto(originalIndex);

            cursor.GotoNext(MoveType.After, i => i.MatchLdstr("tp"), i => i.MatchLdloc(out _));
            Instruction localValueSpriteRendererDestiny = cursor.Prev;

            cursor.GotoNext(MoveType.After, i => i.MatchCall(out _), i => i.MatchCallvirt(out _), i => i.MatchPop());
            cursor.Emit(localValueSpriteRendererDestiny.OpCode, localValueSpriteRendererDestiny.Operand);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(BattleControl_Ext), "CreateDestinySkillSprite"));

            var stLoc = cursor.Body.Instructions[cursor.Index + 1];
            cursor.Emit(stLoc.OpCode, stLoc.Operand);

            cursor.Emit(localValueSpriteRendererDestiny.OpCode, localValueSpriteRendererDestiny.Operand);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "TestSkillIndex"));
            cursor.Emit(OpCodes.Stloc_S, localText3.Operand);
            patched = true;
        }
    }

}
