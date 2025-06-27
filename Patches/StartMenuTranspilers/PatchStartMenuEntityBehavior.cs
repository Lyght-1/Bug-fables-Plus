using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Patches.StartMenuTranspilers
{
    public class PatchStartMenuEntityBehavior : PatchBaseStartMenuEntityBehavior
    {
        public PatchStartMenuEntityBehavior()
        {
            priority = 121;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdloc2());
            cursor.GotoPrev(MoveType.After, i => i.MatchLdarg0());
            cursor.Prev.OpCode = OpCodes.Nop;
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldloc_2);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchStartMenuEntityBehavior), "CheckNewEntityBehavior"));
            cursor.Emit(OpCodes.Ldarg_0);
        }

        static AccessTools.FieldRef<StartMenu, EntityControl[]> entitiesRef = AccessTools.FieldRefAccess<StartMenu, EntityControl[]>("entities");
        static AccessTools.FieldRef<StartMenu, float[]> entitycdRef = AccessTools.FieldRefAccess<StartMenu, float[]>("entitycd");
        static void CheckNewEntityBehavior(StartMenu startmenu, int index)
        {
            if(MainManager_Ext.backgroundData != null)
            {
                EntityControl[] entities = entitiesRef(startmenu);
                switch ((MainManager.AnimIDs)entities[index].animid + 1)
                {
                    case MainManager.AnimIDs.JumpingSpider:
                        if (MainManager_Ext.backgroundData.areaID == (int)MainManager.Areas.BarrenLands)
                        {
                            entities[index].animstate = 100;
                        }
                        break;
 
                    case MainManager.AnimIDs.Kina:
                        entities[index].talking = true;
                        break;
                    case MainManager.AnimIDs.ScientistRoach:
                        entities[index].hologram = true;
                        entities[index].UpdateSpriteMat();
                        break;
                    case MainManager.AnimIDs.Moth:
                        if (MainManager_Ext.backgroundData.areaID == (int)MainManager.Areas.UpperSnakemouth)
                        {
                            entities[index].animstate = (int)MainManager.Animations.WeakBattleIdle;
                        }
                        break;
                    case MainManager.AnimIDs.Wizard:
                        entities[index].animstate = 100;
                        break;

                    case MainManager.AnimIDs.BeeBoss:
                    case MainManager.AnimIDs.Mantidfly:
                    case MainManager.AnimIDs.Flowering:
                    case MainManager.AnimIDs.Midge:
                        entities[index].height = 1;
                        break;
                    case MainManager.AnimIDs.AntSoldier1:
                        if (MainManager_Ext.backgroundData.mapID == (int)MainManager.Maps.NearSnakemouth)
                        {
                            entities[index].animstate = 29;
                        }
                        break;

                }
            }
        }
    }
}
