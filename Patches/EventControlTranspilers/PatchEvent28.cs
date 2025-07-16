using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MainManager;

namespace BFPlus.Patches.EventControlTranspilers
{

    //Changes the reward for the samira got all music thing to the music player
    public class PatchGetMusicPlayer: PatchBaseEvent28
    {
        public PatchGetMusicPlayer()
        {
            priority = 34811;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            for(int i = 0; i < 2; i++)
            {
                cursor.GotoNext(j => j.MatchLdcI4(83));
                cursor.Emit(OpCodes.Ldc_I4, (int)NewItem.MusicPlayer);
                cursor.Remove();
            }
        }
    }

    public class PatchNewMusicParse : PatchBaseEvent28
    {
        public PatchNewMusicParse()
        {
            priority = 34310;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdtoken(out _));
            cursor.RemoveRange(2);
            cursor.GotoNext(MoveType.After, i => i.MatchCallvirt(out _), i => i.MatchCallvirt(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "CheckMusicId"));
            cursor.RemoveRange(2);
        }
    }

    /// <summary>
    /// New patch to load the music async, so it doesnt just load from resources file but also from our assetbundle if its a new music
    /// </summary>
    public class PatchNewLoadMusic : PatchBaseEvent28
    {
        public PatchNewLoadMusic()
        {
            priority = 35068;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchLdcI4(102), i=>i.MatchStfld(out _));

            int cursorIndex = cursor.Index;

            cursor.GotoNext(i => i.MatchLdfld(out _));
            var musicIdRef = cursor.Next.Operand;

            cursor.Goto(cursorIndex);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, musicIdRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "LoadNewMusicAsync"));
            Utils.InsertYieldReturn(cursor);
            ILLabel label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Br, label);

            cursor.GotoNext(i => i.MatchLdsfld(out _));
            cursor.GotoPrev(i => i.MatchLdarg0());
            cursor.MarkLabel(label);
        }
    }

    /// <summary>
    /// If we are already playing the music (should only happen with music player), then we dont load it again, prevent a bug where there wont be any music playing
    /// </summary>
    public class PatchCheckAlreadyPlayingMusic : PatchBaseEvent28
    {
        public PatchCheckAlreadyPlayingMusic()
        {
            priority = 35058;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            ILLabel label = null;
            cursor.GotoNext(MoveType.After,i=>i.MatchLdcI4(0),i => i.MatchBlt(out label));

            var musicIdRef = cursor.Instrs[cursor.Index - 3].Operand;
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, musicIdRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchCheckAlreadyPlayingMusic), "CheckAlreadyPlayingMusic"));
            cursor.Emit(OpCodes.Brtrue, label);
        }

        static bool CheckAlreadyPlayingMusic(int musicId)
        {
            return musicId == MainManager_Ext.CheckMusicId(MainManager.music[0].clip.name);
        }
    }
}
