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

namespace BFPlus.Patches.MainManagerTranspilers
{

    public class Patch100HoaxeRewardNotif : PatchBaseMainManagerTransferMap
    {
        public Patch100HoaxeRewardNotif()
        {
            priority = 34413;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchCallvirt(AccessTools.Method(typeof(EntityControl), "DetectIgnoreSphere", new Type[] {typeof(bool)})));

            ILLabel label = cursor.DefineLabel();

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(Patch100HoaxeRewardNotif), "CheckNotifCondition"));
            cursor.Emit(OpCodes.Brfalse, label);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(Patch100HoaxeRewardNotif), "DoNotif"));
            Utils.InsertYieldReturn(cursor);
            cursor.MarkLabel(label);
        }

        static bool CheckNotifCondition()
        {
            return !MainManager.instance.inevent && MainManager.instance.flags[63] && !MainManager.instance.flags[949] && !MainManager.instance.flags[950];
        }

        static IEnumerator DoNotif()
        {
            MainManager.instance.flags[950] = true;
            yield return EventControl.halfsec;
            string text = "|boxstyle,4||sound,ItemGet1||spd,0|For being such a determined super-explorer, something special awaits|line|if you rewatch the end of Hoaxe's tale.|next|Please inspect the |color,1|crown|color,0| in Team Snakemouth's house.";
            MainManager.instance.StartCoroutine(MainManager.SetText(text, true, Vector3.zero, null, null));
            while (MainManager.instance.message)
            {
                yield return null;
            }
        }
    }
}
