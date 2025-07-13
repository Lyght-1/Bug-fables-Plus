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

namespace BFPlus.Patches.BattleControlTranspilers.AddExperiencePatches
{
    public class PatchWinJingleCheck : PatchBaseAddExperience
    {
        public PatchWinJingleCheck()
        {
            priority = 12597;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdstr("Battle6"));
            cursor.GotoNext(MoveType.After,i => i.MatchStfld(out _));
            var jingleRef = cursor.Prev.Operand;

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, jingleRef);  
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchWinJingleCheck), "CheckJingleValue"));
            cursor.Emit(OpCodes.Stfld, jingleRef);
        }

        static bool CheckJingleValue(bool jingle)
        {
            if (MainManager.musicvolume > 0f && MainManager.music[0].clip != null)
            {
                if (MainManager.music[0].clip.name == "Battle6")
                    return false;

                string[] possibleBattleTheme = new string[] { 
                    NewMusic.BattleRubberPrison.ToString(), NewMusic.BattleGoldenHills.ToString(),
                    NewMusic.BattleLostSands.ToString(),  NewMusic.BattleFactory.ToString(),NewMusic.BattleOutskirts.ToString(),
                    NewMusic.BattleMetalLake.ToString(),NewMusic.BattleForsakenLands.ToString(),NewMusic.BattleCaves.ToString(),
                    NewMusic.BattleSandCastle.ToString(), NewMusic.BattleFarGrasslands.ToString(),NewMusic.BattleSwamplands.ToString(),
                    NewMusic.BattleSnakemouthLab.ToString()
                };
                if (possibleBattleTheme.Contains(MainManager.music[0].clip.name))
                {
                    return true;
                }
            }
            return jingle;
        }

    }

    public class PatchChangeWinJingle : PatchBaseAddExperience
    {
        public PatchChangeWinJingle()
        {
            priority = 12669;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdstr("BattleWon"));
            ILLabel label = cursor.DefineLabel();
            ILLabel jumplabel = cursor.DefineLabel();
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchChangeWinJingle), "CheckWinJingle"));
            cursor.Emit(OpCodes.Brfalse, label);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchChangeWinJingle), "GetWinJingle"));
            cursor.Emit(OpCodes.Br, jumplabel);
            cursor.MarkLabel(label);

            cursor.GotoNext(i => i.MatchStfld(out _));
            cursor.MarkLabel(jumplabel);
        }

        static bool CheckWinJingle()
        {
            int[] newAreaJingleId =
            {
                (int)MainManager.Areas.RubberPrison,
                (int)MainManager.Areas.GoldenHills,
                (int)MainManager.Areas.GoldenWay,
                (int)MainManager.Areas.GoldenSettlement,
                (int)MainManager.Areas.Desert,
                (int)MainManager.Areas.SandCastle,
                (int)MainManager.Areas.DefiantRoot,
                (int)MainManager.Areas.HoneyFactory,
                (int)MainManager.Areas.BugariaOutskirts,
                (int)MainManager.Areas.GiantLair,
                (int)MainManager.Areas.MetalLake,
                (int)MainManager.Areas.StreamMountain,
                (int)MainManager.Areas.BarrenLands,
                (int)MainManager.Areas.Snakemouth,
                (int)MainManager.Areas.ChomperCaves,
                (int)MainManager.Areas.BanditHideout,
                (int)MainManager.Areas.FarGrasslands,
                (int)MainManager.Areas.WildGrasslands,
                (int)MainManager.Areas.UpperSnakemouth
            };
            if (newAreaJingleId.Contains(MainManager.instance.areaid))
            {
                return true;
            }

            return false;
        }

        static AudioSource GetWinJingle()
        {
            if (!MainManager_Ext.newBattleThemes)
            {
                return MainManager.PlaySound("BattleWon");
            }

            string newJingle = "";
            switch (MainManager.instance.areaid)
            {
                case (int)MainManager.Areas.RubberPrison:
                    newJingle = "BattleWonRubberPrison";
                    break;

                case (int)MainManager.Areas.GoldenHills:
                case (int)MainManager.Areas.GoldenWay:
                case (int)MainManager.Areas.GoldenSettlement:
                    newJingle = "BattleWonGoldenHills";
                    break;

                case (int)MainManager.Areas.Desert:
                case (int)MainManager.Areas.DefiantRoot:
                case (int)MainManager.Areas.BanditHideout:
                    newJingle = "BattleWonLostSands";
                    break;

                case (int)MainManager.Areas.HoneyFactory:
                    newJingle = "BattleWonFactory";
                    break;

                case (int)MainManager.Areas.BugariaOutskirts:
                    newJingle = "BattleWonOutskirts";
                    break;

                case (int)MainManager.Areas.MetalLake:
                    newJingle = "BattleWonMetalLake";
                    break;

                case (int)MainManager.Areas.BarrenLands:
                    newJingle = "BattleWonForsakenLands";
                    break;

                case (int)MainManager.Areas.StreamMountain:
                case (int)MainManager.Areas.Snakemouth:
                case (int)MainManager.Areas.ChomperCaves:
                    newJingle = "BattleWonCaves";
                    break;

                case (int)MainManager.Areas.SandCastle:
                    newJingle = "BattleWonSandCastle";
                    break;

                case (int)MainManager.Areas.FarGrasslands:
                    newJingle = "BattleWonFarGrasslands";
                    break;

                case (int)MainManager.Areas.WildGrasslands:
                    newJingle = "BattleWonSwamplands";
                    break;

                case (int)MainManager.Areas.UpperSnakemouth:
                    newJingle = "BattleWonSnakemouthLab";
                    break;
            }

            int mapid = MainManager_Ext.GetNewAreaId((int)MainManager.map.mapid);
            if (mapid > -1)
            {
                switch (mapid)
                {
                    //powerPlant
                    case 1:
                        newJingle = "BattleWonFactory";
                        break;

                    //Iron Tower
                    case 2:
                        newJingle = "BattleWonForsakenLands";
                        break;

                    //Leafbug village
                    case 6:
                        newJingle = "BattleWonSwamplands";
                        break;

                    //giant lair
                    case 7:
                        newJingle = "BattleWonRubberPrison";
                        break;
                }

            }

            if (MainManager.map.mapid == Maps.GoldenPathTunnel || MainManager.map.mapid == Maps.GoldenPathTunnel2)
            {
                newJingle = "BattleWonCaves";
            }

            if (newJingle != "")
            {
                return MainManager.PlaySound(MainManager_Ext.assetBundle.LoadAsset<AudioClip>(newJingle));
            }

            return null;
        }

    }
}
