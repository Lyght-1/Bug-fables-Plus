using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using BFPlus.Properties;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Patches.EventControlTranspilers
{
    ///Credits Event

    ///change credtis logo
    public class PatchNewCreditsLogo : PatchBaseEvent204
    {
        public PatchNewCreditsLogo()
        {
            priority = 242836;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoPrev(i=>i.MatchLdstr("Sprites/GUI/title"));
            Utils.RemoveUntilInst(cursor, i => i.MatchStloc(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "LoadNewTitle"));
            
            //change credits max time
            cursor.GotoNext(i => i.MatchLdcR4(9000f));
            cursor.Next.Operand = 9500f;
        }
    }


    public class PatchMoveEndLogo : PatchBaseEvent204
    {
        public PatchMoveEndLogo()
        {
            priority = 243290;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcR4(-870f));
            cursor.Next.Operand = -930f;
        }
    }

    ///Add new credit texts to credits
    public class PatchNewCreditsText : PatchBaseEvent204
    {
        public PatchNewCreditsText()
        {
            priority = 243133;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.Goto(0);
            cursor.GotoNext(i => i.MatchStloc3());
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchNewCreditsText), "GetNewCredits"));
        }

        static string[] GetNewCredits(string[] original)
        {
            string[] newCredits = MainManager_Ext.assetBundle.LoadAsset<TextAsset>("Credits").ToString().Split(new char[] { '\n' });
            return newCredits.AddRangeToArray(original);
        }
    }

    ///Add new credit texts to credits
    public class PatchCreditsSpeed : PatchBaseEvent204
    {
        public PatchCreditsSpeed()
        {
            priority = 243500;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcR4(1.275f));
            cursor.Next.Operand = 1.35f;
        }

        static string[] GetNewCredits(string[] original)
        {
            string[] newCredits = MainManager_Ext.assetBundle.LoadAsset<TextAsset>("Credits").ToString().Split(new char[] { '\n' });
            return newCredits.AddRangeToArray(original);
        }
    }

    public class PatchChapter7HoaxeIntermissionEvent : PatchBaseEvent204
    {
        public PatchChapter7HoaxeIntermissionEvent()
        {
            priority = 242751;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdsfld(out _));

            ILLabel label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Ldc_I4, 947);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchVeGuBattleHoaxeIntermissionEvent), "CheckHoaxeIntermission"));
            cursor.Emit(OpCodes.Brtrue, label);

            cursor.GotoNext(i => i.MatchLdcR4(0.15f));
            cursor.GotoNext(i => i.MatchLdsfld(out _), i => i.MatchLdnull());

            cursor.Emit(OpCodes.Ldc_I4, (int)NewEvents.HoaxeIntermission7);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchVeGuBattleHoaxeIntermissionEvent), "StartIntermission"));
            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.Ret).MarkLabel(label);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchChapter7HoaxeIntermissionEvent), "ResetStuff"));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchChapter7HoaxeIntermissionEvent), "CheckCreditsFrom"));
            cursor.RemoveRange(3);

            cursor.GotoNext(i => i.MatchLdarg0(), i => i.MatchLdfld(out _), i => i.MatchCall(AccessTools.Method(typeof(EventControl), "ReturnMapLimits")));
            Utils.RemoveUntilInst(cursor, i => i.MatchLdcI4(0));

            cursor.GotoNext(i => i.MatchLdsfld(out _), i => i.MatchLdnull(), i => i.MatchCall(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchChapter7HoaxeIntermissionEvent), "CheckCreditsFrom"));
            cursor.RemoveRange(3);
        }

        static void ResetStuff()
        {
            MainManager.instance.flags[947] = false;

            if (!MainManager.instance.flags[948])
            {
                EntityControl[] party = MainManager.GetPartyEntities(true);

                Vector3[] positions = new Vector3[]
                {
                new Vector3(-9f, 1, 10.4f),
                new Vector3(-8f, 1, 10.4f),
                new Vector3(-7f, 1, 10.4f)
                };
                for (int i = 0; i < party.Length; i++)
                {
                    party[i].transform.position = positions[i];
                }
            }
        }

        static bool CheckCreditsFrom()
        {
            return MainManager.instance.flags[948];
        }
    }
}
