using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MainManager;

namespace BFPlus.Extensions.Events
{

    public class LeafbugShamanFightEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            EntityControl[] party = MainManager.GetPartyEntities(true);
            EntityControl shaman = MainManager.GetEntity(1);
            MainManager.FadeMusic(0.01f);
            MainManager.SetCamera(shaman.transform.position, 0.02f);

            yield return EventControl.sec;
            yield return EventControl.halfsec;

            shaman.animstate = (int)MainManager.Animations.Angry;
            shaman.Jump(15);
            MainManager.PlaySound("Jump", 0.8f, 1);

            yield return EventControl.tenthsec;
            yield return new WaitUntil(() => shaman.onground);
            MainManager.PlaySound("Thud", 1.2f, 1);
            MainManager.ShakeScreen(Vector3.one * 0.15f, 0.75f);

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[2], true, Vector3.zero, shaman.transform, null));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            yield return EventControl.tenthsec;
            MainManager.instance.StartCoroutine(BattleControl.StartBattle(
                new int[] { (int)MainManager.Enemies.LeafbugNinja, (int)NewEnemies.LeafbugShaman, (int)MainManager.Enemies.LeafbugArcher }, 
                -1, -1, NewMusic.PlusBosses.ToString(), null, false)
            );

            yield return EventControl.sec;
            while (MainManager.battle != null)
            {
                yield return null;
            }
            MainManager.SetCamera(shaman.transform.position);
            Vector3[] posArray = new Vector3[]
            {
                new Vector3(0f, 0f, -7.2f),
                new Vector3(-2f, 0f, -8.5f),
                new Vector3(2f, 0f, -8.5f),
                new Vector3(2.4f, 0f, -9f)
            };

            for (int i = 0; i < party.Length; i++)
            {
                party[i].transform.position = posArray[i];
                party[i].animstate = (int)MainManager.Animations.Idle;
                party[i].FaceTowards(Vector3.zero,true);
            }

            var chompy = MainManager.map.chompy;
            if (chompy != null)
            {
                chompy.transform.position = posArray[3];
            }

            //laying on ground, defeat anim
            shaman.animstate = 111;
            yield return EventControl.halfsec;
            MainManager.FadeOut();
            yield return EventControl.sec;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[3], true, Vector3.zero, shaman.transform, null));
            while (MainManager.instance.message)
            {
                yield return null;
            }
            shaman.ShakeSprite(0.1f, 60f);
            yield return EventControl.sec;
            shaman.animstate = (int)MainManager.Animations.Idle;
            shaman.Jump(15);
            MainManager.PlaySound("Jump", 0.8f, 1);

            yield return EventControl.tenthsec;
            yield return new WaitUntil(() => shaman.onground);
            MainManager.PlaySound("Thud", 1.2f, 1);
            MainManager.ShakeScreen(Vector3.one * 0.15f, 0.75f);
            yield return EventControl.tenthsec;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[4], true, Vector3.zero, shaman.transform, null));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            MainManager.SetCamera(party[0].transform.position, 0.02f);
            party[2].flip = false;
            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[5], true, Vector3.zero, party[0].transform, null));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            MainManager.FadeIn();
            yield return EventControl.sec;
            
            MainManager.instance.flags[886] = true;
            MainManager.AddPrizeMedal((int)NewPrizeFlag.Shaman);
            MainManager.LoadMap();
            MainManager.ResetCamera();
            MainManager.ChangeMusic();

            yield return EventControl.sec;
            MainManager.FadeOut();
            yield return EventControl.sec;
        }
    }
}
