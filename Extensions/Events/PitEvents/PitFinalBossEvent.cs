using BFPlus.Extensions.EnemyAI;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MainManager;

namespace BFPlus.Extensions.Events.PitEvents
{
    public class PitFinalBossEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            while (MainManager.instance.message)
                yield return null;

            var chompy = MainManager.map.chompy;
            EntityControl[] party = MainManager.GetPartyEntities(true);

            Vector3[] posArray = new Vector3[]
            {
                new Vector3(-6.4f,0f, -3.4f),
                new Vector3(-7.6f, 0f,-4.4f),
                new Vector3(-9f, 0f, -3.4f),
                new Vector3(-9.5f,0f, -4f)
            };

            for(int i=0; i< party.Length; i++)
            {
                party[i].transform.position = posArray[i];
                party[i].FaceDown();
                party[i].animstate = (int)MainManager.Animations.BattleIdle;
            }

            if(chompy != null)
                chompy.transform.position = posArray[3];

            foreach (var e in party)
            {
                e.flip = true;
            }

            if (chompy != null)
                chompy.flip = true;

            MainManager.FadeMusic(0.01f);
            MainManager.SetCamera(caller.transform.position + Vector3.up * 5, new Vector3(-10, 0, 0), new Vector3(0,5, -25f), 0.01f);
            instance.StartCoroutine(caller.entity.ShakeSprite(0.1f, 120f));
            yield return EventControl.sec;
            yield return EventControl.sec;

            foreach (var e in party)
            {
                e.animstate = (int)MainManager.Animations.Surprized;
            }
            party[2].animstate = (int)MainManager.Animations.BattleIdle;

            int dialogueId = MainManager.instance.flags[857] ? 8 : 2;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[dialogueId], true, Vector3.zero, caller.transform, caller));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            instance.StartCoroutine(caller.entity.ShakeSprite(0.1f, 120f));
            yield return EventControl.sec;
            yield return EventControl.sec;

            //gets out of flower
            MainManager.ChangeMusic("Venus");
            caller.entity.animstate = 103;
            yield return EventControl.sec;

            if (!MainManager.instance.flags[857])
                MainManager.SetCamera(party[1].transform, null, 1f, MainManager.defaultcamoffset, new Vector3(-5f, -55f, 0f));
            else
                MainManager.SetCamera(caller.transform, null, 1f, new Vector3(-2,10,-15), new Vector3(-5f, 0, 0f));


            caller.entity.animstate = (int)MainManager.Animations.Idle;

            dialogueId = MainManager.instance.flags[857] ? 9 : 3;
            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[dialogueId], true, Vector3.zero, caller.transform, caller));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            caller.entity.animstate = 101;
            yield return EventControl.halfsec;
            MainManager.SetCamera(caller.transform.position + Vector3.up * 5, new Vector3(10, 0, 0), new Vector3(0, 0, -20f), 0.01f);
            instance.StartCoroutine(caller.entity.ShakeSprite(0.1f, 120f));

            EntityControl[] buds = new EntityControl[2];
            GameObject head = caller.entity.extras[1];
            float a = 0f;
            float b = 40f;
            Vector3 targetPos;
            for (int i = 0; i < buds.Length; i++)
            {
                buds[i] = EntityControl.CreateNewEntity("bud" + i, (int)NewAnimID.MarsSummon, new Vector3(-5+ (10*i), 0, -1f));
                buds[i].transform.parent = MainManager.map.transform;
                buds[i].transform.localScale = Vector3.zero;

                MainManager.PlaySound("Charge", -1, 0.8f, 1f);
                caller.entity.animstate = 105;
                yield return EventControl.sec;
                caller.entity.animstate = 104;

                MainManager.PlaySound("PingShot");

                SpriteRenderer seed = MainManager.NewSpriteObject(head.transform.position + Vector3.right * 0.1f, null, MainManager.itemsprites[0, 23]);
                seed.material.color = new Color(0.63f, 0.129f, 0.129f);
                seed.transform.localScale = Vector3.one * 2f;
                a = 0f;

                targetPos = new Vector3(buds[i].transform.position.x, 0, buds[i].transform.position.z);
                do
                {
                    seed.transform.position = MainManager.BeizierCurve3(head.transform.position, targetPos, 10f, a / b);
                    seed.transform.eulerAngles += new Vector3(0, 0, 20) * MainManager.TieFramerate(1f);
                    a += MainManager.framestep;
                    yield return null;
                }
                while (a < b + 1f);
                var flowerPart = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/FlowerImpact")) as GameObject;
                var main = flowerPart.GetComponent<ParticleSystem>().main;
                main.startColor = Color.red;
                flowerPart.transform.position = seed.transform.position;
                UnityEngine.Object.Destroy(seed.gameObject);
                UnityEngine.Object.Destroy(flowerPart.gameObject, 5);

                yield return EventControl.tenthsec;
                MainManager.PlaySound("VenusBudAppear", 0.8f, 1);
                buds[i].StartCoroutine(GrowBud(buds[i], Vector3.one*2, 60f));
            }


            yield return EventControl.quartersec;
            caller.entity.animstate = 103;
            yield return EventControl.halfsec;
            caller.entity.animstate = (int)MainManager.Animations.Idle;
            foreach(var bud in buds)
            {
                bud.animstate = 100;
            }
            yield return EventControl.quartersec;

            caller.entity.animstate = 100;
            yield return EventControl.halfsec;
            MainManager.instance.StartCoroutine(BattleControl.StartBattle(new int[]
            {
                (int)NewEnemies.MarsSprout, (int)NewEnemies.Mars, (int)NewEnemies.MarsSprout
            }, -1, -1, "MarsTheme", null, false));

            yield return EventControl.sec;
            while (MainManager.battle != null)
            {
                yield return null;
            }
            MainManager.ResetCamera(true);

            foreach (var e in party)
            {
                e.animstate = (int)MainManager.Animations.WeakBattleIdle;
            }
            caller.entity.animstate = (int)MainManager.Animations.Hurt;
            foreach (var bud in buds)
            {
                bud.animstate = (int)MainManager.Animations.Hurt;
            }
            MainManager.SetCamera(caller.transform.position, new Vector3(10, 0, 0), new Vector3(0, 5, -25f), 1f);
            yield return null;
            MainManager.FadeOut();
            yield return EventControl.halfsec;

            if (MainManager.BadgeIsEquipped(11) || MainManager.instance.flags[614])
            {
                MainManager.UpdateJounal(MainManager.Library.Logbook, (int)NewAchievement.GodofWar);
            }
            MainManager.UpdateJounal(MainManager.Library.Logbook, (int)NewAchievement.UndergroundExplorer);
            MainManager.AddPrizeMedal((int)NewPrizeFlag.Mars);
            foreach (var bud in buds)
            {
                bud.StartCoroutine(GrowBud(bud, Vector3.zero, 120f));
            }

            yield return EventControl.sec;

            dialogueId = MainManager.instance.flags[857] ? 10 : 4;


            //caller.entity.animstate = (int)MainManager.Animations.Idle;
            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[dialogueId], true, Vector3.zero, caller.transform, caller));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            caller.entity.animstate = 101;
            yield return EventControl.halfsec;
            MainManager.SetCamera(caller.transform.position + Vector3.up * 5, new Vector3(10, 0, 0), new Vector3(0, 0, -20f), 0.01f);
            instance.StartCoroutine(caller.entity.ShakeSprite(0.1f, 120f));

            MainManager.PlaySound("Charge", -1, 0.8f, 1f);
            caller.entity.animstate = 105;
            yield return EventControl.sec;
            caller.entity.animstate = 104;

            MainManager.PlaySound("PingShot");

            GameObject itemSprite = null;

            if (!MainManager.instance.flags[857])
                itemSprite = EntityControl.CreateItem(head.transform.position + Vector3.right * 0.1f, 2, (int)Medal.Switcheroo, Vector3.zero, -1).gameObject;
            else
                itemSprite = EntityControl.CreateItem(head.transform.position + Vector3.right * 0.1f, 0, (int)MainManager.Items.MoneyBig, Vector3.zero, -1).gameObject;

            a = 0f;
            b = 40f;
            targetPos = new Vector3(-5,0,0);
            do
            {
                itemSprite.transform.position = MainManager.BeizierCurve3(head.transform.position, targetPos, 10f, a / b);
                a += MainManager.framestep;
                yield return null;
            }
            while (a < b + 1f);

            Vector3 basePos = party[0].transform.position;

            party[0].MoveTowards(itemSprite.transform.position);
            while (party[0].forcemove)
            {
                yield return null;
            }

            if (!MainManager.instance.flags[857])
                instance.GiveItem(2, (int)Medal.Switcheroo, -4);
            else
                instance.GiveItem(-1, 500, -4);

            UnityEngine.Object.Destroy(itemSprite.gameObject);
            while (MainManager.instance.message)
            {
                yield return null;
            }
            party[0].MoveTowards(basePos);
            while (party[0].forcemove)
            {
                yield return null;
            }

            party[0].FaceTowards(caller.transform.position);

            MainManager.SetCamera(caller.transform.position + Vector3.up * 5, new Vector3(10, 0, 0), new Vector3(0, 0, -20f), 0.01f);
            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[5], true, Vector3.zero, caller.transform, caller));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            yield return EventControl.halfsec;

            MainManager.FadeIn(0.05f);
            yield return null;
            SpriteRenderer dimmer = MainManager.GetTransitionSprite();
            while (dimmer.color.a < 0.95f)
            {
                yield return null;
            }
            yield return EventControl.sec;
            MainManager.GetEntity(1).transform.position = new Vector3(-6, 0, 45);
            caller.interacttype = NPCControl.Interaction.None;

            MainManager.ResetCamera();

            foreach (var e in party)
            {
                e.animstate = (int)MainManager.Animations.Idle;
            }
            yield return EventControl.sec;
            MainManager.FadeOut(0.05f);

            party[0].FaceTowards(party[2].transform.position);

            if (!MainManager.instance.flags[857])
            {
                MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[6], true, Vector3.zero, party[2].transform, party[2].npcdata));
                while (MainManager.instance.message)
                {
                    yield return null;
                }
            }
            MainManager.instance.flags[857] = true;
            MainManager.ChangeMusic();
            yield return EventControl.tenthsec; 
            yield return null;
        }

        IEnumerator GrowBud(EntityControl bud, Vector3 scaleTarget, float frameTime)
        {
            float a = 0;
            do
            {
                bud.transform.localScale = Vector3.Lerp(bud.transform.localScale, scaleTarget, a / frameTime);
                a += MainManager.TieFramerate(1f);
                yield return null;

            } while (a < frameTime);
        }
    }
}
