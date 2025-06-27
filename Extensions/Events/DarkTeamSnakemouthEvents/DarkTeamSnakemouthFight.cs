using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events.DarkTeamSnakemouthEvents
{
    public class DarkTeamSnakemouthFight : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            MainManager.FadeMusic(0.01f);

            EntityControl vi = MainManager.GetEntity(-4);
            EntityControl[] party = MainManager.GetPartyEntities(true);
            var chompy = MainManager.map.chompy;
            EntityControl[] darkParty = MainManager.GetEntities(new int[] { 2, 3, 4 });
            EntityControl darkVi = darkParty[0];

            var startPosition = new Vector3(-0.1f, 2.43f, -3f);

            foreach (var p in party)
            {
                p.transform.position = startPosition;
            }

            if (chompy != null)
                chompy.transform.position = startPosition;


            Vector3[] posArray = new Vector3[]
            {
                new Vector3(-3f, 2.431f, 2.142f),
                new Vector3(-4.9f, 2.431f,2.142f),
                new Vector3(-7f, 2.431f, 2.142f),
                new Vector3(-8.5f, 2.431f, 2.142f)
            };


            foreach (var e in party)
            {
                e.FaceTowards(darkVi.transform.position);
            }

            if (chompy != null)
            {
                chompy.FaceTowards(darkVi.transform.position);
            }

            MainManager.events.MoveParty(posArray);
            if (chompy != null)
            {
                chompy.forcejump = true;
            }
            while (!MainManager.PartyIsNotMoving() || (chompy != null && chompy.forcemove) || caller.entity.forcemoving != null)
            {
                yield return null;
            }

            foreach (var e in party)
            {
                e.animstate = (int)MainManager.Animations.BattleIdle;
                e.FaceTowards(darkVi.transform.position);
            }

            if (chompy != null)
            {
                chompy.FaceTowards(darkVi.transform.position);
            }

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[2], true, Vector3.zero, vi.transform, vi.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }

            darkVi.animstate = 103;
            yield return new WaitForSeconds(0.17f);
            darkVi.animstate = 104;
            yield return new WaitForSeconds(0.5f);
            darkVi.animstate = 105;
            yield return new WaitForSeconds(0.05f);
            MainManager.PlaySound("Woosh", 8, 1.1f, 1f, true);
            GameObject beemerang = UnityEngine.Object.Instantiate<GameObject>(Resources.Load("Prefabs/Objects/BeerangBattle") as GameObject);
            beemerang.transform.position = darkVi.transform.position + Vector3.up;
            Vector3 start = beemerang.transform.position + new Vector3(0.5f, 0.5f);
            Vector3 target = vi.sprite.transform.position + Vector3.up * 0.75f;
            Vector3 mid = Vector3.Lerp(start, target, 0.5f) + new Vector3(0f, 0f, -5f);
            float a = 0f;
            do
            {
                a += MainManager.framestep;
                beemerang.transform.position = MainManager.BeizierCurve3(start, target, mid, Mathf.Clamp01(a / 20f));
                beemerang.transform.localEulerAngles = new Vector3(80f, 0f, beemerang.transform.localEulerAngles.z - MainManager.framestep * 20f);
                if (a >= 20f)
                {
                    MainManager.PlaySound("Damage0");
                    MainManager.HitPart(vi.transform.position + Vector3.up);
                    vi.overrideanim = true;
                    vi.animstate = (int)MainManager.Animations.Hurt;
                    break;
                }
                yield return null;
            }
            while (a < 40f);
            UnityEngine.Object.Destroy(beemerang);
            MainManager.StopSound(8, 0.1f);
            yield return EventControl.quartersec;

            MainManager.instance.StartCoroutine(BattleControl.StartBattle(new int[]
            {
                (int)NewEnemies.DarkVi,(int)NewEnemies.DarkKabbu,(int)NewEnemies.DarkLeif
            }, -1, 3, NewMusic.DarkSnek.ToString(), null, false));

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
            foreach (var e in darkParty)
            {
                e.animstate = (int)MainManager.Animations.KO;
            }
            EntityControl genow = MainManager.GetEntity(5);
            genow.alwaysactive = true;
            EntityControl mar = MainManager.GetEntity(6);
            mar.alwaysactive = true;
            genow.transform.position = new Vector3(0, 2.43f, -4.67f);
            mar.transform.position = new Vector3(1.6f, 2.43f, -4.67f);

            yield return null;
            MainManager.FadeOut();
            yield return EventControl.halfsec;
            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[3], true, Vector3.zero, genow.transform, genow.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }
            genow.MoveTowards(new Vector3(0, 2.43f, 0.25f));
            mar.MoveTowards(new Vector3(1.6f, 2.43f, 0.25f));
            yield return EventControl.halfsec;
            while (genow.forcemove || mar.forcemove)
                yield return null;

            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[4], true, Vector3.zero, genow.transform, genow.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }
            MainManager.FadeIn();
            yield return new WaitForSeconds(1.25f);

            foreach (var e in darkParty)
                e.gameObject.SetActive(false);

            mar.gameObject.SetActive(false);
            genow.gameObject.SetActive(false);
            MainManager.FadeOut();
            yield return EventControl.sec;
            MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[6], true, Vector3.zero, vi.transform, vi.npcdata));
            while (MainManager.instance.message)
            {
                yield return null;
            }
            MainManager.ChangeMusic();
            yield return null;     
        }
    }


}
