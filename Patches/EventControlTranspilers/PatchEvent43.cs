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

namespace BFPlus.Patches.EventControlTranspilers
{
    /// <summary>
    /// Venus bud event
    /// </summary>
    /// 

    //Change dialogues id for the venus bud in front of the pit
    public class PatchVenusPitDialogues : PatchBaseEvent43
    {
        public PatchVenusPitDialogues()
        {
            priority = 48876;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdsfld(out _),i =>i.MatchLdfld(out _),i => i.MatchLdcI4(411));
            cursor.Next.OpCode = OpCodes.Nop;
            cursor.GotoNext(i => i.MatchLdfld(out _));
            Utils.RemoveUntilInst(cursor,i => i.MatchStloc2());

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchVenusPitDialogues), "GetVenusPitDialogue"));
        }

        static int GetVenusPitDialogue()
        {
            if (!MainManager.instance.librarystuff[0, (int)NewDiscoveries.PitOfTrials])
            {
                //Before entering pit first time dialogue
                return 49;
            }

            if (MainManager.instance.flags[857])
            {
                //beat mars dialogue
                return 52;
            }

            //reached floor 50 pit dialogue
            if (MainManager.instance.flags[802])
                return 51;


            //havent reached floor 50 pit dialogue
            return 50;
        }
    }

    public class PatchVenusNewMapDialogue : PatchBaseEvent43
    {
        public PatchVenusNewMapDialogue()
        {
            priority = 48946;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdloc2(),i => i.MatchCall(out _));
            cursor.GotoNext(i => i.MatchCall(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchVenusNewMapDialogue), "GetVenusDialogue"));
        }

        static int GetVenusDialogue(int dialogueIndex)
        {
            if ((int)MainManager.map.mapid == (int)NewMaps.LeafbugVillage)
            {
                return 37;
            }
            return dialogueIndex;
        }
    }
}
