using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Patches.BattleControlTranspilers.StartBattlePatches
{
    public class PatchEventMusic : PatchBaseStartBattle
    {
        public PatchEventMusic()
        {
            priority = 644;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(MoveType.After, i => i.MatchStfld(AccessTools.Field(typeof(MainManager), "inlist")));

            int index = cursor.Index;

            cursor.GotoNext(i => i.MatchCallvirt(AccessTools.Method(typeof(BattleControl), "StartData")));
            cursor.GotoPrev(
                i => i.MatchLdfld(out _),
                i => i.MatchLdarg0(), 
                i => i.MatchLdfld(out _),
                i=>i.MatchLdarg0(), 
                i=>i.MatchLdfld(out _));

            var musicRef = cursor.Next.Operand;

            cursor.GotoPrev(
               i => i.MatchLdsfld(out _),
               i => i.MatchLdarg0(),
               i => i.MatchLdfld(out _));

            cursor.GotoNext(i => i.MatchLdfld(out _));
            var enemyRef = cursor.Next.Operand;

            cursor.Goto(index);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, musicRef);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, enemyRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchEventMusic), "CheckEventMusic"));
            cursor.Emit(OpCodes.Stfld, musicRef);
        }

        static string CheckEventMusic(string music, int[] enemyids)
        {
            if (MainManager.instance.inevent)
            {
                int[] events = new int[] { 
                    14, //Jelly shroom relay Tutorial,
                    70, //Venus buds battle
                    91, //Cable car bodyguard quest
                    97, //Beep boop battle gen&eri
                    98, //Abomihoneys malbee battle
                    3, //Crisbee I want to get better quest battle
                    40, //Theater battles
                    115, //Sand Castle boss key fight
                    126, //Mender fights
                    128, //Leafbug ambush fight
                    177, //Help me get it back quest
                    222, //Loose ends quest
                    180, //Zombee/Zombeetle battle upper snek
                    72, //Golden hills Offering optional battle 
                };

                if (events.Contains(MainManager.lastevent))
                {
                    return "Battle6";
                }

                switch (MainManager.lastevent)
                {
                    //Confidential Event
                    case 207:
                        if (enemyids.Contains((int)MainManager.Enemies.LeafbugArcher))
                            return "Battle6";
                        break;

                    //Coloseum Event
                    case 163:
                        if (enemyids.Contains((int)MainManager.Enemies.MimicSpider))
                            return "Battle6";
                        break;
                }


            }

            if (MainManager.map.mapid == MainManager.Maps.GoldenSMinigame)
                return "Battle6";

            return music;
        }
    }
}
