using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;

namespace BFPlus.Extensions.Events.NewBossesEvents
{
    public abstract class StartBossFightEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            MainManager.FadeMusic(0.01f);
            yield return ApproachingBoss(instance, caller);
            MainManager.ResetCamera(true);
        }

        protected IEnumerator StartBattle(int[] enemies)
        {
            MainManager.instance.StartCoroutine(BattleControl.StartBattle(enemies, -1, -1, NewMusic.PlusBosses.ToString(), null, false));
            yield return EventControl.sec;
            while (MainManager.battle != null)
            {
                yield return null;
            }
        }

        protected abstract IEnumerator ApproachingBoss(EventControl instance, NPCControl caller);

        protected IEnumerator MoveParty(EventControl instance, NPCControl caller, Vector3[] posArray)
        {
            MainManager.FadeMusic(0.01f);

            var chompy = MainManager.map.chompy;

            instance.MoveParty(posArray);
            if (chompy != null)
            {
                chompy.forcejump = true;
            }           
            while (!MainManager.PartyIsNotMoving() || (chompy != null && chompy.forcemove) || caller.entity.forcemoving != null)
            {
                yield return null;
            }
        }

        protected IEnumerator DoWinFightEvent(EventControl instance, NPCControl caller, EntityControl[] enemies, EntityControl[] party, int itemID, int flag, int itemType, Vector3 itemPos, bool resetCam = true)
        {
            if(resetCam)
                MainManager.ResetCamera(true);

            foreach (var enemy in enemies)
            {
                enemy.animstate = (int)MainManager.Animations.Hurt;
            }

            foreach (var p in party)
            {
                p.animstate = (int)MainManager.Animations.BattleIdle;
            }
            yield return null;
            MainManager.FadeOut();
            yield return EventControl.sec;

            foreach (var enemy in enemies)
            {
                instance.StartCoroutine(enemy.Death());
            }

            while (!enemies[0].iskill)
            {
                yield return null;
            }
            GameObject item = EntityControl.CreateItem(itemPos, itemType, itemID, Vector3.zero, -1).gameObject;
            yield return EventControl.halfsec;

            foreach (var p in party)
            {
                p.animstate = 0;
            }

            party[0].MoveTowards(item.transform.position,1,(int)MainManager.Animations.Walk, 0);
            while (party[0].forcemove)
            {
                yield return null;
            }
            UnityEngine.Object.Destroy(item.gameObject);
            instance.GiveItem(itemType, itemID, -4);
            while (MainManager.instance.message)
            {
                yield return null;
            }
            MainManager.instance.flags[flag] = true;
            MainManager.ChangeMusic();
        }
    }
}
