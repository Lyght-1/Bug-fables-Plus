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
using static MainManager;

namespace BFPlus.Patches.BattleControlTranspilers.StartBattlePatches
{
    public class PatchAreaBattleMusic : PatchBaseStartBattle
    {
        public PatchAreaBattleMusic()
        {
            priority = 1463;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, j => j.MatchLdfld(AccessTools.Field(typeof(MainManager), "areaid")), j=> j.MatchStloc(out _));

            int cursorIndex = cursor.Index;
            ILLabel label = null;
            cursor.GotoPrev(i => i.MatchBr(out label));
            cursor.Goto(cursorIndex);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchAreaBattleMusic), "CheckNewAreaMusic"));
            cursor.Emit(OpCodes.Brtrue, label);
        }

        static bool CheckNewAreaMusic()
        {
            string newMusic = "";
            switch (MainManager.instance.areaid)
            {
                case (int)MainManager.Areas.RubberPrison:
                    newMusic = NewMusic.BattleRubberPrison.ToString();
                    break;

                case (int)MainManager.Areas.GoldenHills:
                case (int)MainManager.Areas.GoldenWay:
                case (int)MainManager.Areas.GoldenSettlement:
                    newMusic = NewMusic.BattleGoldenHills.ToString();
                    break;

                case (int)MainManager.Areas.Desert:
                case (int)MainManager.Areas.DefiantRoot:
                case (int)MainManager.Areas.BanditHideout:
                    newMusic = NewMusic.BattleLostSands.ToString();
                    break;

                case (int)MainManager.Areas.HoneyFactory:               
                    newMusic = NewMusic.BattleFactory.ToString();
                    break;

                case (int)MainManager.Areas.BugariaOutskirts:
                    newMusic = NewMusic.BattleOutskirts.ToString();
                    break;

                case (int)MainManager.Areas.MetalLake:
                    newMusic = NewMusic.BattleMetalLake.ToString();
                    break;

                case (int)MainManager.Areas.BarrenLands:
                    newMusic = NewMusic.BattleForsakenLands.ToString();
                    break;

                case (int)MainManager.Areas.Snakemouth:
                case (int)MainManager.Areas.ChomperCaves:
                case (int)MainManager.Areas.StreamMountain:
                    newMusic = NewMusic.BattleCaves.ToString();
                    break;

                case (int)MainManager.Areas.SandCastle:
                    newMusic = NewMusic.BattleSandCastle.ToString();
                    break;

                case (int)MainManager.Areas.FarGrasslands:
                    newMusic = NewMusic.BattleFarGrasslands.ToString();
                    break;

                case (int)MainManager.Areas.WildGrasslands:
                    newMusic = NewMusic.BattleSwamplands.ToString();
                    break;

                case (int)MainManager.Areas.UpperSnakemouth:
                    newMusic = NewMusic.BattleSnakemouthLab.ToString();
                    break;
            }


            int mapid = MainManager_Ext.GetNewAreaId((int)MainManager.map.mapid);
            if (mapid > -1)
            {
                switch (mapid)
                {
                    //powerPlant
                    case 1:
                        newMusic = NewMusic.BattleFactory.ToString();
                        break;

                    // irontower
                    case 2:
                        newMusic = NewMusic.BattleForsakenLands.ToString();
                        break;

                    //leafbug village
                    case 6:
                        newMusic = NewMusic.BattleSwamplands.ToString();
                        break;

                    // playroom
                    case 7:
                        newMusic = NewMusic.BattleRubberPrison.ToString();
                        break;
                }
            }

            if(MainManager.map.mapid == Maps.GoldenPathTunnel || MainManager.map.mapid == Maps.GoldenPathTunnel2)
            {
                newMusic = NewMusic.BattleCaves.ToString();
            }

            if (newMusic != "" && MainManager_Ext.newBattleThemes)
            {
                MainManager.ChangeMusic(newMusic, 1f);
                return true;
            }


            if (MainManager.instance.areaid == (int)MainManager.Areas.GiantLair)
            {
                MainManager.ChangeMusic("Battle6", 1f);
                return true;
            }

            return false;
        }
    }
}
