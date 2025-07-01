using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Patches.EventControlTranspilers
{
    //HB'S Lab Boss Event

    /// <summary>
    /// Patch New boss portrait into the B.O.S.S
    /// </summary>
    public class PatchNewBossPortrait : PatchBaseEvent85
    {
        public PatchNewBossPortrait()
        {
            priority = 95176;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(92), i=>i.MatchBeq(out _));

            cursor.GotoNext(MoveType.After,i => i.MatchLdelemRef());
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "GetNewBossPortrait"));
            Utils.RemoveUntilInst(cursor, i => i.MatchCallvirt(out _));
        }
    }

    /// <summary>
    /// Patch to add the new boss ids and set the new boss entities
    /// </summary>
    public class PatchNewBossIds : PatchBaseEvent85
    {
        public PatchNewBossIds()
        {
            priority = 96047;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            //Patch new bosses ids
            cursor.GotoNext(i => i.MatchLdstr("Miniboss"));
            var musicRef = cursor.Instrs[cursor.Index + 1].Operand;
            var idsRef = cursor.Instrs[cursor.Index - 2].Operand;
            var mapIdRef = cursor.Instrs[cursor.Index + 4].Operand;

            cursor.Emit(OpCodes.Ldflda, idsRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "GetNewBossIds"));
            cursor.Emit(OpCodes.Ldarg_0);

            //Patch new boss entity
            //96409
            cursor.GotoNext(i => i.MatchLdcI4(329));
            cursor.GotoNext(i => i.MatchLdfld(out _));
            var eposRef = cursor.Next.Operand;

            cursor.GotoNext(MoveType.After, i => i.MatchStfld(AccessTools.Field(typeof(EntityControl), "fixedentity")));
            var enemyEntityRef = cursor.Instrs[cursor.Index - 3].Operand;
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, idsRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, enemyEntityRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldflda, musicRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldflda, eposRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldflda, mapIdRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "SetNewBossEntity"));
        }
    }
    /// <summary>
    /// Adds the new bosses to the boss list
    /// </summary>
    public class PatchBossList : PatchBaseEvent85
    {
        public PatchBossList()
        {
            priority = 95038;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(94));

            cursor.GotoNext(MoveType.After,i => i.MatchLdloc(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "CheckBossList"));
            cursor.Remove();
        }
    }
    /// <summary>
    /// Adds the new minibosses to the miniboss list
    /// </summary>
    public class PatchMiniBossList : PatchBaseEvent85
    {
        public PatchMiniBossList()
        {
            priority = 95013;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchLdsfld(AccessTools.Field(typeof(EventControl), "minibosslist")));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "CheckMinibossList"));
        }
    }

    /// <summary>
    /// Check if you beat team snakemouth holo, if so, set the text to holocloak reward
    /// </summary>
    public class PatchCheckBeatTeamSnek : PatchBaseEvent85
    {
        public PatchCheckBeatTeamSnek()
        {
            priority = 97414;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(
                MoveType.After,
                i => i.MatchStloc(out _), 
                i => i.MatchLdsfld(AccessTools.Field(typeof(MainManager), "battleresult")), 
                i => i.MatchBrfalse(out _));

            var textRef = cursor.Instrs[cursor.Index - 3].Operand;

            cursor.Emit(OpCodes.Ldloc_S, textRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchCheckBeatTeamSnek), "CheckBeatTeamSnek"));
            cursor.Emit(OpCodes.Stloc_S, textRef);
        }

        static string CheckBeatTeamSnek(string text)
        {
            if (MainManager.instance.flagvar[6] == -2 && !MainManager.instance.flags[865])
            {
                MainManager.instance.flags[902] = true;
                MainManager.map.transform.Find("holoClockSprite").gameObject.SetActive(false);

                return string.Concat(new object[]
                {
                    MainManager.map.dialogues[24],
                    " ",
                    MainManager.map.dialogues[39],
                    "|break||flag,865,true||giveitem,2,",
                    (int)MainManager.BadgeTypes.HoloCloak,
                    ",22|"
                });
            }
            return text;
        }
    }

    /// <summary>
    /// Add a check to add the superbosses to the multilist
    /// </summary>
    public class PatchSuperbossList : PatchBaseEvent85
    {
        public PatchSuperbossList()
        {
            priority = 95048;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchLdcI4(163));
            cursor.GotoPrev(MoveType.After, i => i.MatchLdsfld(out _));
            cursor.Prev.OpCode = OpCodes.Nop;
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "CheckSuperbossList"));
            cursor.Emit(OpCodes.Ldsfld, AccessTools.Field(typeof(MainManager), "instance"));
        }
    }

    public class PatchOriginalMap : PatchBaseEvent85
    {
        public PatchOriginalMap()
        {
            priority = 95484;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(166));
            cursor.GotoPrev(i => i.MatchLdcI4(1), i=>i.MatchLdcI4(1));

            int cursorIndex = cursor.Index;

            cursor.GotoPrev(i => i.MatchLdfld(out _), i => i.MatchLdcI4(0), i => i.MatchLdelemRef());
            var pcfaceRef = cursor.Next.Operand;

            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, pcfaceRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchOriginalMap), "AskForMapType"));
            Utils.InsertYieldReturn(cursor);
        }

        static IEnumerator AskForMapType(SpriteRenderer[] pcface)
        {
            var callRef = AccessTools.StaticFieldRefAccess<EventControl, NPCControl>("call");
            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[99], true, Vector3.zero, callRef.transform, callRef));
            while (MainManager.instance.message)
            {
                pcface[0].enabled = Mathf.Sin(Time.time * 10f) > 0f;
                yield return null;
            }
            yield return null;
        }
    }

    public class PatchResetRenderSettings : PatchBaseEvent85
    {
        public PatchResetRenderSettings()
        {
            priority = 96701;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i=>i.MatchNewobj(out _),i => i.MatchCall(AccessTools.Method(typeof(MainManager), "SetCamera", new Type[] { typeof(Transform), typeof(Vector3?), typeof(float), typeof(Vector3), typeof(Vector3)})));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "ResetRenderSettings"));
        }
    }

    public class PatchTrophyBossAmount : PatchBaseEvent85
    {
        public PatchTrophyBossAmount()
        {
            priority = 97569;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchLdcI4(692));
            cursor.GotoPrev(i => i.MatchLdcI4(11));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchTrophyBossAmount), "GetMiniBossAmount"));
            cursor.Remove();

            cursor.GotoNext(i => i.MatchLdcI4(16));
            var amountRef = cursor.Prev.Operand;

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchTrophyBossAmount), "GetBossAmount"));
            cursor.Remove();

            cursor.GotoNext(i => i.MatchLdcI4(693));
            cursor.GotoNext(MoveType.After,i => i.MatchLdsfld(out _));
            cursor.Prev.OpCode = OpCodes.Nop;

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, amountRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchTrophyBossAmount), "CheckSuperBossTrophy"));
            cursor.Emit(OpCodes.Ldsfld, AccessTools.Field(typeof(MainManager), "instance"));
        }

        static int GetMiniBossAmount() => MainManager_Ext.Instance.minibossAmount;
        static int GetBossAmount() => MainManager_Ext.Instance.bossAmount;

        static void CheckSuperBossTrophy(int amountDefeated)
        {
            if (MainManager.instance.flagvar[1] == 2 && amountDefeated >= MainManager_Ext.SUPERBOSS_AMOUNT)
            {
                MainManager.instance.flags[900] = true;
            }
        }
    }

    public class PatchCheckHardModeAchievement : PatchBaseEvent85
    {
        public PatchCheckHardModeAchievement()
        {
            priority = 96984;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchLdcI4(23), i => i.MatchStloc(out _), i=>i.MatchLdloc(out _));
            int cursorIndex = cursor.Index;

            cursor.GotoPrev(i => i.MatchLdcI4(91));
            var enemyIdRef = cursor.Prev.Operand;
            cursor.Goto(cursorIndex);

            var idRef = cursor.Prev.Operand;
            cursor.Emit(OpCodes.Ldloc, enemyIdRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchCheckHardModeAchievement), "CheckJournalId"));
            cursor.Emit(OpCodes.Stloc, idRef);
            cursor.Emit(OpCodes.Ldloc, idRef);
        }

        static int CheckJournalId(int id,int enemyId)
        {
            if(enemyId == (int)NewEnemies.Mars)
            {
                return (int)NewAchievement.GodofWar;
            }

            return id;
        }
    }
}
