using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace BFPlus.Extensions.Events.HoaxeIntermissionEvents
{
    public class HoaxeIntermission2Event : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            if (MainManager.instance.flags[555])
                MainManager.FadeIn();
            MainManager.FadeMusic(0.02f);
            yield return EventControl.sec;
            yield return EventControl.sec;

            MainManager.instance.insideid = -1;

            MainManager.instance.flags[916] = true;
            MainManager.LoadMap(232);
            yield return null;
            yield return null;

            GameObject crystal = UnityEngine.Object.Instantiate(Resources.Load("prefabs/objects/SavePoint")) as GameObject;
            crystal.transform.position = new Vector3(4f, 5f, 13.21f);
            crystal.transform.GetChild(0).gameObject.SetActive(false);
            crystal.transform.parent = MainManager.map.transform;

            if (MainManager.map.chompy != null)
            {
                MainManager.map.chompy.transform.position = new Vector3(0,-999);
                MainManager.map.chompy.LockRigid(true);
            }

            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                MainManager.instance.playerdata[i].entity.transform.position = new Vector3(0, -999);
                MainManager.instance.playerdata[i].entity.LockRigid(true);
            }

            EntityControl hoaxe = EntityControl.CreateNewEntity("hoaxe", (int)NewAnimID.Hoaxe, new Vector3(44.83f, 0f, 10.22f));
            hoaxe.animstate = 101;
            hoaxe.transform.parent = MainManager.map.transform;

            EntityControl[] deadlanders = new EntityControl[3];

            for (int i = 0; i < deadlanders.Length; i++) 
            {
                deadlanders[i] = EntityControl.CreateNewEntity("deadlander"+i, (int)MainManager.AnimIDs.DeadLanderA - 1 + i, new Vector3(60+i*3, 0, 16f+i*2));
                deadlanders[i].transform.parent = MainManager.map.transform;
            }

            deadlanders[0].digging = true;
            deadlanders[0].nodigpart = true;
            deadlanders[0].transform.position = new Vector3(29.16f, -0.15f, 8f);

            MainManager.SetCamera(hoaxe.transform, null, 1f);
            MainManager.FadeOut(0.01f);
            hoaxe.MoveTowards(new Vector3(31.38f, 0f, 8.5f), 1.5f, 101, (int)MainManager.Animations.WeakBattleIdle);
            AudioSource footstep = MainManager.PlaySound("Footstep", -1, 1, 1, true);
            yield return null;
            yield return new WaitUntil(() => !hoaxe.forcemove);

            if (MainManager.SoundVolume())
                footstep.volume = 0;
            MainManager.SetCamera(null, hoaxe.transform.position, 1f);

            yield return EventControl.sec;
            hoaxe.flip = true;
            yield return EventControl.sec;
            hoaxe.flip = false;
            yield return EventControl.sec;

            deadlanders[0].animstate = 100;
            yield return EventControl.halfsec;

            deadlanders[0].PlaySound("DigPop3");
            MainManager.PlayParticle("DirtExplode2", deadlanders[0].transform.position);
            deadlanders[0].digging = false;
            deadlanders[0].flip = true;
            MainManager.ChangeMusic("Alert");
            yield return EventControl.tenthsec;

            hoaxe.animstate = (int)MainManager.Animations.Surprized;
            deadlanders[0].PlaySound("DLAlphaChomp");
            deadlanders[0].animstate = 101;
            yield return EventControl.sec;

            hoaxe.LockRigid(true);
            hoaxe.animstate = (int)MainManager.Animations.Jump;
            hoaxe.PlaySound("Jump");
            hoaxe.StartCoroutine(MainManager.ArcMovement(hoaxe.gameObject, hoaxe.transform.position + Vector3.forward*2, 5, 20));

            deadlanders[0].animstate = 102;
            deadlanders[0].PlaySound("Bite",1f,1.2f);
            yield return EventControl.halfsec;

            hoaxe.LockRigid(false);
            Vector3 targetPos = new Vector3(6.7f, 0, 12.67f);
            hoaxe.MoveTowards(targetPos, 1.5f, 101, (int)MainManager.Animations.Surprized);

            if (MainManager.SoundVolume())
                footstep.volume = 1 * MainManager.soundvolume;

            yield return EventControl.sec;
            deadlanders[0].animstate = 100;
            deadlanders[0].PlaySound("DLAlphaWoosh");
            yield return EventControl.quartersec;

            deadlanders[0].digging = true;
            deadlanders[0].PlaySound("Dig3");
            yield return EventControl.halfsec;

            if (MainManager.SoundVolume())
                footstep.volume = 0;

            yield return null;
            Vector3[] deadlandersPositions =
            {
                new Vector3(15f, 0, 10.15f),
                new Vector3(15.3f, 0f, 13.72f),
                new Vector3(14.30f, 0, 12.57f)
            };

            MainManager.PlaySound("DLGammaStep", 9, 1, 1, true);
            for (int i = 0; i < deadlanders.Length; i++)
            {
                deadlanders[i].transform.position = new Vector3(40+i*2, 0, 8.5f + i*1);
                deadlanders[i].MoveTowards(deadlandersPositions[i], 2f);
            }

            yield return EventControl.sec;
            yield return EventControl.halfsec;
            MainManager.SetCamera(null, targetPos + new Vector3(4,0,-1), 0.05f);

            yield return new WaitUntil(() => !hoaxe.forcemove);
            hoaxe.flip = true;
            GameObject tiredPart = MainManager.PlayParticle("Tired", hoaxe.transform.position + Vector3.up, -1);
            yield return new WaitUntil(() => !deadlanders[2].forcemove);

            deadlanders[0].animstate = 100;
            deadlanders[0].flip = false;
            deadlanders[0].PlaySound("DigPop3");
            deadlanders[0].digging = false;
            yield return EventControl.halfsec;
            deadlanders[0].animstate = 0;
            MainManager.StopSound(9);
            yield return EventControl.halfsec;

            for(int i = 0; i < 2; i++)
            {
                yield return EventControl.halfsec;
                MainManager.PlaySound("DLGammaStep", 9, 1, 1, true);
                deadlanders[2].MoveTowards(deadlanders[2].transform.position + Vector3.left * 2, 0.5f);
                yield return new WaitUntil(() => !deadlanders[2].forcemove);
                MainManager.StopSound(9);
            }

            MainManager.PlaySound("DLGammaClaw1");
            deadlanders[2].animstate = 100;
            deadlanders[2].StartCoroutine(deadlanders[2].ShakeSprite(0.2f, 45f));
            yield return EventControl.sec;
            deadlanders[2].LockRigid(true);

            MainManager.PlaySound("DLGammaClaw2");
            deadlanders[2].animstate = 101;
            Vector3 basePos = deadlanders[2].transform.position;
            targetPos = hoaxe.transform.position + new Vector3(0.5f, 1, 0);
            instance.StartCoroutine(BattleControl_Ext.LerpPosition(20, basePos, targetPos, deadlanders[2].transform));
            yield return EventControl.tenthsec;

            UnityEngine.Object.Destroy(tiredPart);
            hoaxe.animstate = 100;
            hoaxe.LockRigid(true);

            yield return EventControl.tenthsec;
            MainManager.PlaySound("HugeHit");
            MainManager.HitPart(targetPos);

            MainManager.ShakeScreen(new Vector3(0.2f, 0.2f), 1);

            deadlanders[2].StartCoroutine(MainManager.ArcMovement(deadlanders[2].gameObject, basePos, 5, 10));
            deadlanders[2].animstate = 0;
            yield return MainManager.ShakeObject(crystal.transform, new Vector3(0.2f, 0f), 60f, true);

            float a = 0;
            float b = 25;
            Vector3 startPos = crystal.transform.position;
            targetPos = hoaxe.transform.position + new Vector3(-1.5f, 0.5f, -0.3f);
            MainManager.PlaySound("Fall2");
            do
            {
                crystal.transform.position = Vector3.Lerp(startPos, targetPos, a / b);
                crystal.transform.localEulerAngles = new Vector3(Mathf.Lerp(0, 90, a / b), Mathf.Lerp(0, 90, a / b), 0);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);
            crystal.transform.position = targetPos;
            crystal.transform.localEulerAngles = new Vector3(90, 90, 0);

            MainManager.PlaySound("Thud3");
            MainManager.PlayParticle("impactsmoke", crystal.transform.position + Vector3.left);

            MainManager.ShakeScreen(0.1f, 1);
            hoaxe.hasshadow = false;
            hoaxe.shadow.enabled = false;
            hoaxe.shadow.gameObject.SetActive(false);
            deadlanders[2].LockRigid(false);

            for (int i = 0; i < deadlanders.Length; i++)
            {
                deadlanders[i].Jump(5);
            }

            yield return EventControl.tenthsec;
            MainManager.PlaySound("Flee");
            deadlanders[0].digging = true;

            for (int i = 0; i < deadlanders.Length; i++)
            {
                deadlanders[i].MoveTowards(new Vector3(30, 0,6), 2f);
            }

            MainManager.FadeMusic(0.1f);
            yield return EventControl.sec;
            yield return EventControl.sec;
            yield return EventControl.sec;

            EntityControl[] wasps= MainManager.GetEntities(new int[] {8,9,10,11});

            foreach (var wasp in wasps)
                wasp.alwaysactive = true;

            wasps[0].transform.position = new Vector3(-26.28f, 6.85f, 0.82f);
            wasps[0].Unfix(true);
            MainManager.SetCamera(wasps[0].transform, null, 0.02f);
            yield return EventControl.halfsec;

            wasps[0].flip = true;
            wasps[0].MoveTowards(new Vector3(-15f, 5.4531f, 0f), 1);
            yield return EventControl.sec;
            yield return new WaitUntil(() => !wasps[0].forcemove);

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[7], true, Vector3.zero, wasps[0].transform, wasps[0].npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            wasps[0].Emoticon(MainManager.Emoticons.Exclamation, 60);
            yield return EventControl.sec;

            wasps[0].MoveTowards(new Vector3(4.4f, 0,10f), 1);
            yield return new WaitUntil(() => !wasps[0].forcemove);

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[8], true, Vector3.zero, wasps[0].transform, wasps[0].npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            Vector3[] waspsPos =
            {
                new Vector3(8f, 0f, 8.7f),
                new Vector3(11f, 0, 10.5f),
                new Vector3(10f, 0, 11.4f)
            };
            //wasps[3].height = 1;
            for (int i = 1; i < wasps.Length; i++)
            {
                wasps[i].Unfix(true);
                wasps[i].transform.position = new Vector3(-5 + i * 2, 0, -2 + i * 1);
                wasps[i].MoveTowards(waspsPos[i-1], 1f);
            }
          
            yield return new WaitUntil(() => !wasps[1].forcemove && !wasps[2].forcemove && !wasps[3].forcemove);

            for (int i = 1; i < wasps.Length; i++)
            {
                wasps[i].FaceTowards(wasps[0].transform.position);
            }

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[9], true, Vector3.zero, wasps[2].transform, wasps[2].npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            waspsPos = new Vector3[]
            {
                new Vector3(6.15f, 0, 11.2f),
                new Vector3(8.14f, 0, 11.43f),
                new Vector3(8.83f,0, 13.84f)
            };
            hoaxe.animstate = (int)MainManager.Animations.KO;
            for (int i = 1; i < wasps.Length; i++)
            {
                wasps[i].MoveTowards(waspsPos[i - 1], 1f);
            }
            yield return new WaitUntil(() => !wasps[1].forcemove && !wasps[2].forcemove && !wasps[3].forcemove);
            yield return BattleControl_Ext.LerpPosition(10, crystal.transform.position, crystal.transform.position + Vector3.up * 2, crystal.transform);

            crystal.transform.parent = wasps[1].transform;

            waspsPos = new Vector3[]
            {
                new Vector3(10f, 0, 11.43f),
                new Vector3(12.14f, 0, 11.43f),
                new Vector3(12.83f,0, 13.84f)
            };
            for (int i = 1; i < wasps.Length; i++)
            {
                wasps[i].MoveTowards(waspsPos[i - 1], 1f);
            }
            yield return new WaitUntil(() => !wasps[1].forcemove && !wasps[2].forcemove && !wasps[3].forcemove);

            for (int i = 1; i < wasps.Length; i++)
            {
                wasps[i].FaceTowards(wasps[0].transform.position);
            }

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[10], true, Vector3.zero, wasps[3].transform, wasps[3].npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }
            hoaxe.LockRigid(true);
            wasps[0].gameObject.layer = 9;

            MainManager.SetCamera(wasps[0].transform.position);
            wasps[0].MoveTowards(hoaxe.transform.position, 1);
            yield return new WaitUntil(() => !wasps[0].forcemove);

            hoaxe.animstate = 102;
            hoaxe.transform.parent = wasps[0].transform;
            hoaxe.transform.localPosition = hoaxe.transform.localPosition + new Vector3(-0.11f, 1.5f);
            //hoaxe.transform.localEulerAngles = new Vector3(0,0,40);

            for (int i = 0; i < wasps.Length; i++)
            {
                wasps[i].MoveTowards(new Vector3(-4f, 0f, -5f), 1f);
            }
            MainManager.FadeIn(0.05f);

            while (wasps[0].forcemove)
            {
                for (int i = 0; i < wasps.Length; i++)
                {
                    wasps[i].flip = false;
                }
                yield return null;
            }

            MainManager.instance.flags[928] = true;
            yield return EndIntermissionPostgame(instance,73,53);
        }
    }
}
