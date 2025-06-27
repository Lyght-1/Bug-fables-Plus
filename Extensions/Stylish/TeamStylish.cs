using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static MainManager;
using static BFPlus.Extensions.BattleControl_Ext;
namespace BFPlus.Extensions.Stylish
{
    public class TeamStylish : IStylish
    {
        bool inSecondStylish;
        IEnumerator DoFrostRelayStylish()
        {
            EntityControl[] entities = battle.GetEntities(new int[] { 0, 1, 2 });

            EntityControl vi = entities[0];

            EntityControl leif = entities[2];
            StylishUtils.ShowStylish(1.2f, leif);

            leif.LockRigid(true);
            leif.overrridejump = true;
            leif.overrideanim = true;
            leif.overrideflip = false;

            leif.FaceTowards(vi.transform.position);
            leif.animstate = 111;
            yield return EventControl.tenthsec;
            MainManager.PlayParticle("mothicenormal", "OverworldIce", vi.transform.position);
            vi.Freeze();
            yield return EventControl.quartersec;

            entities[1].animstate = (int)MainManager.Animations.Surprized;

            float a = 0;
            float b = 30;
            Vector3 startPos = leif.transform.position;
            Vector3 targetPos = vi.transform.position + Vector3.up * 2;
            bool failedStylish = false;

            leif.animstate = (int)MainManager.Animations.Jump;
            MainManager.PlaySound("Jump");
            do
            {
                leif.transform.position = MainManager.BeizierCurve3(startPos, targetPos, 5f, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);

            a = 0;
            b = 20;
            do
            {
                if (!inSecondStylish && !failedStylish)
                {
                    if (StylishUtils.CheckStylish(ref failedStylish, leif, a, 5f))
                    {
                        inSecondStylish = true;
                        MainManager.battle.StartCoroutine(DoLeifPose(leif));
                        break;
                    }
                }
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);
            leif.Emoticon(MainManager.Emoticons.None);

            yield return new WaitUntil(() => !inSecondStylish);

            vi.BreakIce();
            leif.spin = new Vector3(0, 20, 0);
            leif.animstate = (int)MainManager.Animations.Fall;
            void action(Vector3 startPosition, Vector3 endPosition, Transform obj, float starttime, float endTime)
            {
                obj.position = MainManager.BeizierCurve3(startPosition, endPosition, 5f, starttime / endTime);
            }

            Vector3[] partyPos = MainManager.battle.partypos;
            int leifIndex = Array.FindIndex(MainManager.battle.partypointer, x => x == 2);

            yield return LerpStuff(30f, leif.transform.position, partyPos[leifIndex], leif.transform, action);
            leif.spin = Vector3.zero;
            leif.LockRigid(false);
            leif.overrridejump = false;
            leif.overrideanim = false;
            leif.animstate = (int)MainManager.Animations.Idle;
            vi.animstate = (int)MainManager.Animations.Angry;
        }

        IEnumerator DoLeifPose(EntityControl leif)
        {
            StylishUtils.ShowStylish(1.2f, leif);
            leif.spin = new Vector3(0, 20, 0);
            leif.animstate = (int)MainManager.Animations.ItemGet;
            yield return EventControl.thirdsec;
            leif.spin = Vector3.zero;
            inSecondStylish = false;
        }

        IEnumerator DoFlyDropStylish()
        {
            EntityControl[] entities = battle.GetEntities(new int[] { 0, 1 });

            StylishUtils.ShowStylish(1.2f, entities[1]);
            Vector3[] basePosition = new Vector3[]
            {
                entities[0].transform.position,
                entities[1].transform.position
            };

            foreach (var entity in entities)
            {
                entity.LockRigid(true);
                entity.overrideanim = true;
                entity.overrridejump = true;
            }

            float a = 0;
            float b = 25;
            entities[1].animstate = 119;
            entities[1].digging = false;

            entities[1].spin = new Vector3(0, 30, 0);
            Vector3 startPos = entities[1].transform.position;
            Vector3 targetPos = entities[1].transform.position + Vector3.up * 3;
            bool failedStylish = false;
            do
            {
                entities[1].transform.position = Vector3.Lerp(startPos, targetPos, a / b);

                if (!inSecondStylish && !failedStylish)
                {
                    if (StylishUtils.CheckStylish(ref failedStylish, entities[0], a, 10f))
                    {
                        inSecondStylish = true;
                        MainManager.battle.StartCoroutine(DoViJump(entities[0], entities[1]));
                    }
                }
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);

            entities[0].Emoticon(MainManager.Emoticons.None);

            if (inSecondStylish)
            {
                yield return new WaitUntil(() => !inSecondStylish);

                for (int i = 0; i < entities.Length; i++)
                {
                    entities[i].animstate = (int)MainManager.Animations.Fall;
                    entities[i].spin = new Vector3(0, 30, 0);
                }
                Vector3[] start = new Vector3[]
                {
                    entities[0].transform.position,
                    entities[1].transform.position
                };
                a = 0;
                b = 20;
                do
                {
                    for (int i = 0; i < entities.Length; i++)
                    {
                        entities[i].transform.position = MainManager.BeizierCurve3(start[i], basePosition[i], 5f, a / b);
                    }
                    a += MainManager.TieFramerate(1f);
                    yield return null;
                } while (a < b);
            }
            else
            {
                a = 0f;
                b = 20f;

                startPos = entities[1].transform.position;
                entities[1].animstate = (int)MainManager.Animations.Fall;
                do
                {
                    entities[1].transform.position = Vector3.Lerp(startPos, basePosition[1], a / b);
                    a += MainManager.TieFramerate(1f);
                    yield return null;
                } while (a < b);
            }

            foreach (var entity in entities)
            {
                entity.LockRigid(false);
                entity.overrideanim = false;
                entity.overrridejump = false;
                entity.spin = Vector3.zero;
                entity.flip = true;
            }
        }

        IEnumerator DoViJump(EntityControl vi, EntityControl kabbu)
        {
            StylishUtils.ShowStylish(1.2f, vi);
            float a = 0;
            float b = 20;

            Vector3 startPos = vi.transform.position;

            float offset = (startPos.x > kabbu.transform.position.x) ? 1.15f : -1.15f;
            Vector3 targetPos = new Vector3(kabbu.transform.position.x + offset, 3.15f, kabbu.transform.position.z);

            vi.FaceTowards(kabbu.transform.position);
            vi.animstate = (int)MainManager.Animations.Jump;

            vi.PlaySound("Jump");
            do
            {
                vi.transform.position = MainManager.BeizierCurve3(startPos, targetPos, 5f, a / b);

                if (a > b / 2)
                {
                    kabbu.animstate = (int)MainManager.Animations.ItemGet;
                    kabbu.spin = Vector3.zero;
                    kabbu.FaceTowards(vi.transform.position);
                    vi.animstate = 109;
                }

                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);

            MainManager.PlaySound("AtkSuccess", 1f, 1);
            yield return EventControl.quartersec;

            vi.flip = true;
            inSecondStylish = false;
        }
        IEnumerator DoFrozenDrillStylish()
        {
            EntityControl[] entities = battle.GetEntities(new int[] { 1, 2 });

            EntityControl kabbu = entities[0];
            EntityControl leif = entities[1];
            StylishUtils.ShowStylish(1.2f, leif);

            bool originalFlip = leif.flip;

            leif.FaceTowards(kabbu.transform.position);
            leif.animstate = 105;
            yield return EventControl.tenthsec;
            Vector3 icePos = kabbu.transform.position + Vector3.up * 5 + Vector3.right * 0.5f;

            MainManager.PlayParticle("mothicenormal", icePos);
            yield return EventControl.quartersec;
            leif.animstate = 107;
            GameObject icecube = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Objects/icecube"));
            MainManager.PlaySound("Freeze");
            icecube.transform.position = icePos;
            icecube.transform.localScale = Vector3.zero;
            yield return LerpScale(30, Vector3.zero, Vector3.one, icecube.transform);

            leif.animstate = 102;
            float a = 0;
            float b = 25;
            Vector3 startPos = icecube.transform.position;
            Vector3 targetPos = kabbu.transform.position;
            bool failedStylish = false;
            bool hitBlock = false;
            do
            {
                icecube.transform.position = Vector3.Lerp(startPos, targetPos, a / b);

                if (!failedStylish && a <= 20f)
                {
                    if (StylishUtils.CheckStylish(ref failedStylish, kabbu, a, 8f))
                    {
                        hitBlock = true;
                        break;
                    }
                }

                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);
            kabbu.Emoticon(MainManager.Emoticons.None);

            if (hitBlock)
            {
                yield return DoKabbuDrillStylish(kabbu, leif, icecube);
            }
            else
            {
                kabbu.animstate = (int)MainManager.Animations.Hurt;
                UnityEngine.Object.Destroy(UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/IceShatter"), icecube.transform.position, Quaternion.Euler(-90f, 0f, 0f)) as GameObject, 1f);
                MainManager.PlaySound("IceBreak", 0.65f);
                MainManager.PlaySound("Damage0", 1f);
                UnityEngine.Object.Destroy(icecube);
                yield return EventControl.halfsec;
            }

            leif.flip = originalFlip;
        }

        IEnumerator DoKabbuDrillStylish(EntityControl kabbu, EntityControl leif, GameObject icecube)
        {
            StylishUtils.ShowStylish(1.2f, kabbu);
            kabbu.animstate = 100;
            kabbu.PlaySound("Damage0", 1f, 0.5f);
            MainManager.HitPart(kabbu.transform.position + Vector3.up / 2f);
            float a = 0f;
            float b = 35f;

            Vector3 startPos = icecube.transform.position;
            Vector3 targetPos = new Vector3(20, 0, 0);
            do
            {
                icecube.transform.position = MainManager.BeizierCurve3(startPos, targetPos, 10f, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);


            UnityEngine.Object.Destroy(icecube);
            a = 0f;
            b = 20f;
            bool gotStylish = false;
            bool failedStylish = false;
            do
            {

                if (!failedStylish)
                {
                    if (StylishUtils.CheckStylish(ref failedStylish, leif, a, 8f))
                    {
                        gotStylish = true;
                        break;
                    }
                }
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);
            leif.Emoticon(MainManager.Emoticons.None);
            kabbu.animstate = (int)MainManager.Animations.BattleIdle;
            if (gotStylish)
                yield return DoFrozenDrillStylish();
        }

        IEnumerator DoFrostBowlingStylish()
        {
            EntityControl[] entities = battle.GetEntities(new int[] { 0, 1, 2 });
            StylishUtils.ShowStylish(1.2f, entities[0]);

            entities[0].animstate = 111;
            entities[1].animstate = 114;
            entities[2].animstate = 115;

            foreach(var entity in entities)
            {
                entity.spin = new Vector3(0, 20, 0);
            }
            yield return EventControl.halfsec;

            foreach (var entity in entities)
            {
                entity.spin = Vector3.zero;
            }
            yield return EventControl.quartersec;
        }

        IEnumerator DoRoyalDecreeStylish()
        {
            StylishUtils.ShowStylish(1.2f, MainManager.instance.playerdata[0].battleentity);
            foreach(var player in MainManager.instance.playerdata)
            {
                player.battleentity.animstate = (int)MainManager.Animations.Happy;
            }
            yield return EventControl.quartersec;
        }

        IEnumerator DoRainDanceStylish()
        {
            StylishUtils.ShowStylish(1.2f, MainManager.instance.playerdata[MainManager.battle.currentturn].battleentity);
            MainManager.instance.playerdata[MainManager.battle.currentturn].battleentity.animstate = (int)MainManager.Animations.Happy;
            MainManager.instance.playerdata[battle.target].battleentity.animstate = (int)MainManager.Animations.Happy;

            yield return EventControl.quartersec;
        }

        public IEnumerator DoStylish(int actionid, int stylishID)
        {
            switch (actionid)
            {
                //Fly drop
                case 5:

                    yield return DoFlyDropStylish();
                    break;

                //frost relay
                case 26:
                    yield return DoFrostRelayStylish();
                    break;

                //Frozen Drill
                case 27:
                    yield return DoFrozenDrillStylish();
                    break;

                case 31:
                    yield return DoFrostBowlingStylish();
                    break;

                //Royal Decree
                case 46:
                    yield return DoRoyalDecreeStylish();
                    break;

                case (int)NewSkill.RainDance:
                    yield return DoRainDanceStylish();
                    break;

            }
        }
    }
}
