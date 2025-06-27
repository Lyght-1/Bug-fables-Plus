using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MainManager;

namespace BFPlus.Extensions.Events.NewBossesEvents
{

    public class IronTowerBossEvent : StartBossFightEvent
    {
        protected override IEnumerator ApproachingBoss(EventControl instance, NPCControl caller)
        {
            EntityControl ironSuit = MainManager.GetEntity(2);
            EntityControl[] party = MainManager.GetPartyEntities(true);
            Vector3[] posArray = new Vector3[]
            {
                new Vector3(-9,0,0),
                new Vector3(-7, 0,-1),
                new Vector3(-5, 0, 0f),
                new Vector3(-10, 0, -0.5f)
            };
            MainManager.SetCamera(ironSuit.transform.position + new Vector3(-5,0.5f,-1), 0.01f);

            foreach (var p in party)
                p.backsprite = false;

            yield return MoveParty(instance, caller, posArray);
            yield return EventControl.halfsec;

            MainManager.DialogueText(MainManager.map.dialogues[3], party[0].transform, party[0].npcdata);
            while (MainManager.instance.message)
                yield return null;

            yield return EventControl.halfsec;
            ironSuit.StartCoroutine(ironSuit.ShakeSprite(0.2f, 60f));
            yield return EventControl.tenthsec;

            foreach (var p in party)
                p.animstate = (int)MainManager.Animations.Surprized;

            yield return EventControl.sec;
            ironSuit.animstate = 106;
            yield return EventControl.sec;

            MainManager.DialogueText(MainManager.map.dialogues[1], ironSuit.transform, ironSuit.npcdata);
            while (MainManager.instance.message)
                yield return null;

            yield return EventControl.quartersec;
            ironSuit.animstate = 101;
            MainManager.DialogueText(MainManager.map.dialogues[2], ironSuit.transform, ironSuit.npcdata);
            while (MainManager.instance.message)
                yield return null;
            yield return StartBattle(new int[] { (int)NewEnemies.IronSuit });

            MainManager.SetCamera(ironSuit.transform.position + new Vector3(-5, 0.5f, -1), 1);
            ironSuit.animstate = 100;
            foreach (var p in party)
                p.animstate = (int)MainManager.Animations.BattleIdle;

            yield return null;
            MainManager.FadeOut();
            yield return EventControl.sec;

            MainManager.DialogueText(MainManager.map.dialogues[4], party[0].transform, party[0].npcdata);
            while (MainManager.instance.message)
                yield return null;

            yield return EventControl.halfsec;
            ironSuit.StartCoroutine(ironSuit.ShakeSprite(0.2f, 60f));
            yield return EventControl.tenthsec;

            foreach (var p in party)
                p.animstate = (int)MainManager.Animations.Surprized;

            ironSuit.animstate = 106;
            yield return EventControl.sec;

            Vector3 basePos = ironSuit.transform.position;
            ironSuit.MoveTowards(party[2].transform.position + Vector3.right*3,1,(int)MainManager.Animations.Walk, 0);
            yield return null;
            yield return new WaitUntil(() => !ironSuit.forcemove);

            MainManager.DialogueText(MainManager.map.dialogues[5], ironSuit.transform, ironSuit.npcdata);
            while (MainManager.instance.message)
            {
                yield return null;
            }

            ironSuit.MoveTowards(basePos, 1);
            yield return null;
            yield return new WaitUntil(() => !ironSuit.forcemove);

            ironSuit.flip = false;
            ironSuit.animstate = (int)MainManager.Animations.KO;
            yield return EventControl.sec;
            ironSuit.animstate = 105;

            MainManager.DialogueText(MainManager.map.dialogues[7], party[1].transform, party[1].npcdata);
            while (MainManager.instance.message)
            {
                yield return null;
            }

            MainManager.instance.flags[915] = true;
            MainManager.ChangeMusic();
            MainManager.ResetCamera();
            yield return EventControl.sec;
        }
    }
}
