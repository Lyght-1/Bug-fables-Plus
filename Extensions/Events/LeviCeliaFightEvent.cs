﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events
{

    public class LeviCeliaFightEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            while (MainManager.instance.message)
                yield return null;

            EntityControl[] party = MainManager.GetPartyEntities(true);
            EntityControl[] teamCelia = MainManager.GetEntities(new int[] { 11,12});

            yield return new WaitUntil(() => !teamCelia[0].GetComponent<TrainingNPC>().doingAnim);

            teamCelia[0].animstate = (int)MainManager.Animations.Idle;

            foreach (var entity in teamCelia)
                entity.FacePlayer();

            if (!MainManager.instance.flags[860])
            {
                MainManager.DialogueText(MainManager.map.dialogues[18], teamCelia[0].transform, caller);
                while (MainManager.instance.message)
                    yield return null;
            }

            int dialogueId = MainManager.instance.flags[859] ? 29 : 19;

            MainManager.DialogueText(MainManager.map.dialogues[dialogueId], teamCelia[0].transform, caller);
            while (MainManager.instance.message)
                yield return null;

            if (MainManager.instance.option == 0)
            {
                foreach (var e in party)
                    e.animstate = (int)MainManager.Animations.BattleIdle;

                teamCelia[0].animstate = (int)MainManager.Animations.BattleIdle;

                MainManager.DialogueText(MainManager.map.dialogues[22], party[0].transform, caller);
                while (MainManager.instance.message)
                    yield return null;

                MainManager.battlelossevent = true;
                MainManager.instance.StartCoroutine(BattleControl.StartBattle(new int[]
                {
                (int)NewEnemies.Levi, (int)NewEnemies.Celia
                }, -1, -1, NewMusic.PlusBosses.ToString(), null, false));
                yield return EventControl.sec;

                while (MainManager.battle != null)
                    yield return null;

                bool lost = !MainManager.battleresult;
                foreach (var e in party)
                    e.animstate = lost ? (int)MainManager.Animations.WeakBattleIdle : (int)MainManager.Animations.BattleIdle;
                
                foreach (var e in teamCelia)
                    e.animstate = !lost ? (int)MainManager.Animations.WeakBattleIdle : (int)MainManager.Animations.Idle;

                if(!lost && MainManager.instance.flagvar[(int)NewFlagVar.TeamCelia_Reward] == 0)
                    MainManager.AddPrizeMedal((int)NewPrizeFlag.TeamCelia);

                MainManager.ResetCamera(true);
                MainManager.FadeOut(0.05f);
                yield return EventControl.halfsec;
                if (!MainManager.instance.flags[859])
                {
                    if (lost)
                        MainManager.DialogueText(MainManager.map.dialogues[25], party[2].transform, caller);
                    else
                        MainManager.DialogueText(MainManager.map.dialogues[23], caller.transform, caller);
                }
                else
                {
                    if (lost)
                        MainManager.DialogueText(MainManager.map.dialogues[31], party[2].transform, caller);
                    else
                        MainManager.DialogueText(MainManager.map.dialogues[30], caller.transform, caller);
                }

                while (MainManager.instance.message)
                    yield return null;

                yield return EventControl.tenthsec;

                MainManager.ChangeMusic();

                foreach (var e in teamCelia)
                    e.animstate = (int)MainManager.Animations.Idle;

                if (!MainManager.instance.flags[859])
                {
                    MainManager.instance.flags[859] = true;
                    MainManager.CompleteQuest((int)NewQuest.InNeedOfTraining);
                }

            }
            else
            {
                MainManager.DialogueText(MainManager.map.dialogues[21], teamCelia[0].transform, caller);
                while (MainManager.instance.message)
                    yield return null;
            }
        }
    }
}
