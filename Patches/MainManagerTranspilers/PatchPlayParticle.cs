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

namespace BFPlus.Patches.MainManagerTranspilers
{
    public class PatchPlayParticle : PatchBaseMainManagerPlayParticle
    {
        public PatchPlayParticle()
        {
            priority = 0;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After,i => i.MatchCall(out _), i => i.MatchCall(out _));
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchPlayParticle), "CheckParticle"));
        }

        static UnityEngine.Object CheckParticle(UnityEngine.Object particle, string name)
        {
            if(Enum.IsDefined(typeof(NewParticle), name))
            {
                return MainManager_Ext.assetBundle.LoadAsset<GameObject>(name);
            }
            return particle;
        }
    }
}
