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
using UnityEngine.Experimental.UIElements;

namespace BFPlus.Patches.BattleControlTranspilers
{
    public class PatchEndOfBattleChecks : PatchBaseBattleControlReturnToOverworld
    {
        public PatchEndOfBattleChecks()
        {
            priority = 157515;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            ILLabel label = null;
            cursor.GotoNext(i => i.MatchLdsfld(out _), i => i.MatchLdcI4(0), i => i.MatchStfld(AccessTools.Field(typeof(MainManager), "minipause")));

            int cursorIndex = cursor.Index;
            cursor.GotoNext(i => i.MatchBr(out label));
            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchEndOfBattleChecks), "CheckPitEnemyDead"));
            cursor.Emit(OpCodes.Brtrue,label);

            cursor.GotoNext(MoveType.After,i=>i.MatchLdstr("CheckAchievement"), i=>i.MatchLdcR4(0.5f), i=>i.MatchCallvirt(out _));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchEndOfBattleChecks), "CheckEndOfBattleStuff"));
        }


        static void CheckEndOfBattleStuff()
        {
            if (!MainManager.battlefled && MainManager.instance.flags[816] && !MainManager.instance.flags[817] && MainManager.map.mapid == MainManager.Maps.GoldenSMinigame)
            {
                var calledFrom = MainManager.battle.sdata.called;
                if (calledFrom != null && calledFrom.entity.animid == (int)NewAnimID.Worm || calledFrom.entity.animid == (int)NewAnimID.WormSwarm)
                {
                    if (calledFrom.activationflag > -1)
                    {
                        MainManager.instance.flags[calledFrom.activationflag] = true;

                        bool killedAll = true;
                        for (int i = 822; i < 826; i++)
                        {
                            if (!MainManager.instance.flags[i])
                            {
                                killedAll = false;
                            }
                        }
                        MainManager.instance.flags[826] = killedAll;
                    }
                }
            }

            if (!MainManager.instance.librarystuff[3, (int)NewAchievement.SuperBug])
            {
                MainManager_Ext.Instance.CheckSuperBugAchievement();
            }
            MainManager.ApplyBadges();
            MainManager.ApplyStatBonus();

            BattleControl_Ext.Instance.ResetStuff();
        }


        static bool CheckPitEnemyDead()
        {
            BattleControl battle = MainManager.battle;
            if (!MainManager.battlefled && (int)MainManager.map.mapid == (int)NewMaps.Pit100BaseRoom && battle.caller != null && battle.caller.entitytype == NPCControl.NPCType.Enemy)
            {
                MainManager.instance.inevent = true;
                MainManager.instance.StartCoroutine(EventControl_Ext.WaitForPitEnemyDeath(battle.caller));
                return true;
            }

            return false;
        }
    }
}
