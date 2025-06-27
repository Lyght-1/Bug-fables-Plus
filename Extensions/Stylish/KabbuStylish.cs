using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Object;
using static BattleControl;
using static BFPlus.Extensions.BattleControl_Ext;
using System.Data;
namespace BFPlus.Extensions.Stylish
{
    public class KabbuStylish : IStylish
    {
        IEnumerator DoFlip(EntityControl kabbu, int flipCount)
        {
            StylishUtils.ShowStylish(1.2f + (0.1f * flipCount), kabbu, flipCount == 0 ? 0.1f :0.02f);
            Vector3 startPos = kabbu.transform.position;
            Vector3 endPos = kabbu.transform.position + Vector3.left * 1.5f;
            float a = 0;
            float b = 25f;

            bool anotherFlip = false;
            kabbu.animstate = (int)MainManager.Animations.Jump;
            do
            {
                kabbu.spritetransform.eulerAngles = new Vector3(0f, 180f, Mathf.Lerp(0f, -360f, a / b));
                kabbu.transform.position = MainManager.BeizierCurve3(startPos, endPos, 7f + flipCount*1.5f, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);

            kabbu.spritetransform.eulerAngles = new Vector3(0f, 180f, Mathf.Lerp(0f, 180, 0));
            if (flipCount < 3)
            {
                kabbu.animstate = (int)MainManager.Animations.BattleIdle;
                a = 0f;
                b = 10f;
                do
                {
                    if (a < 3 && !BattleControl_Ext.Instance.inStylishTutorial)
                    {
                        if (MainManager.GetKey(4, false))
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (BattleControl_Ext.Instance.inStylishTutorial)
                        {
                            ButtonSprite button = new GameObject().AddComponent<ButtonSprite>().SetUp(4, -1, "", new Vector3(0f, -3f, 10f), Vector3.one, 1, MainManager.GUICamera.transform);
                            MainManager.battle.DestroyHelpBox();
                            yield return EventControl.tenthsec;
                            while (!MainManager.GetKey(4))
                            {
                                if (button.basesprite != null)
                                {
                                    button.basesprite.color = Mathf.Sin(Time.time * 10f) * 10f > 0f ? Color.white : Color.gray;
                                }
                                yield return null;
                            }
                            Destroy(button.gameObject);
                            anotherFlip = true;
                            break;
                        }


                        if (MainManager.BadgeIsEquipped((int)Medal.TimingTutor))
                            kabbu.Emoticon(MainManager.Emoticons.Exclamation);
                        if (MainManager.GetKey(4, false))
                        {
                            anotherFlip = true;
                            break;
                        }
                    }
                    a += MainManager.TieFramerate(1f);
                    yield return null;
                } while (a < b);

                kabbu.Emoticon(MainManager.Emoticons.None);
                if (anotherFlip && flipCount < 3)
                {
                    yield return DoFlip(kabbu, ++flipCount);
                }
            }
            else
            {
                yield return null;
                kabbu.animstate = (int)MainManager.Animations.Happy;
                yield return EventControl.halfsec;
            }
        }

        IEnumerator DoBasicAttackStylish()
        {
            BattleControl.SetDefaultCamera();
            EntityControl kabbu = Instance.entityAttacking;
            kabbu.overrridejump = true;
            kabbu.overrideonlyflip = true;

            Vector3 startAngle = kabbu.spritetransform.eulerAngles;
            yield return DoFlip(kabbu, 0);

            kabbu.overrridejump = false;
            kabbu.overrideonlyflip = false;
            kabbu.spritetransform.eulerAngles = startAngle;
            kabbu.animstate = (int)MainManager.Animations.BattleIdle;
        }


        IEnumerator DoTauntStylish()
        {
            EntityControl kabbu = Instance.entityAttacking;
            StylishUtils.ShowStylish(1.2f, kabbu);
            kabbu.animstate = (int)MainManager.Animations.Happy;
            yield return EventControl.halfsec;
        }

        IEnumerator DoUnderStrikeStylish()
        {
            EntityControl kabbu = Instance.entityAttacking;
            StylishUtils.ShowStylish(1.2f, kabbu);
            kabbu.overrridejump = true;
            kabbu.overrideonlyflip = true;

            Vector3 startAngle = kabbu.spritetransform.eulerAngles;
            Vector3 startPos = kabbu.transform.position;
            Vector3 endPos = kabbu.transform.position + Vector3.up * 5f;
            kabbu.animstate = (int)MainManager.Animations.Jump;

            float a = 0;
            float b = 25f;
            do
            {
                kabbu.spritetransform.eulerAngles = new Vector3(0f, 180f, Mathf.Lerp(0f, -360f, a / b));
                kabbu.transform.position = MainManager.BeizierCurve3(startPos, endPos, 7, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);

            yield return new WaitUntil(() => kabbu.onground);
            kabbu.spritetransform.eulerAngles = startAngle;
            kabbu.animstate = 101;
            kabbu.overrridejump = false;
            kabbu.overrideonlyflip = false;
            yield return EventControl.quartersec;
        }

        IEnumerator DoHeavyStrikeStylish()
        {
            SetDefaultCamera();
            EntityControl kabbu = Instance.entityAttacking;
            StylishUtils.ShowStylish(1.2f, kabbu);
            kabbu.overrridejump = true;
            kabbu.overrideonlyflip = true;
            kabbu.animstate = (int)MainManager.Animations.Jump;

            yield return null;
            yield return null;

            //this is needed cause base sprite pivot isnt the center, if we dont do that the rotation will look weird.
            kabbu.anim.enabled = false;

            Sprite originalSprite = kabbu.sprite.sprite;
            Rect spriteRect = originalSprite.rect;
            Sprite newSprite = Sprite.Create(
                originalSprite.texture,
                spriteRect,
                new Vector2(0.5f, 0.5f),
                originalSprite.pixelsPerUnit,
                0,
                SpriteMeshType.Tight,
                originalSprite.border
            );
            kabbu.sprite.sprite = newSprite;

            Vector3[] partyPos = MainManager.battle.partypos;
            Vector3 startAngle = kabbu.spritetransform.eulerAngles;
            Vector3 startPos = kabbu.transform.position;

            int kabbuIndex = Array.FindIndex(MainManager.battle.partypointer, x => x == MainManager.battle.currentturn);
            Vector3 endPos = partyPos[kabbuIndex];
            float a = 0;
            float b = 35f;
            do
            {
                kabbu.spritetransform.Rotate(new Vector3(0, 0, MainManager.TieFramerate(-20f)));
                kabbu.transform.position = MainManager.BeizierCurve3(startPos, endPos, 10, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);

            kabbu.anim.enabled = true;
            yield return new WaitUntil(() => kabbu.onground);                                    
            kabbu.spin = new Vector3(0,10,0);
            kabbu.spritetransform.eulerAngles = startAngle;
            kabbu.animstate = 101;
            kabbu.overrridejump = false;
            kabbu.overrideonlyflip = false;     

            yield return EventControl.halfsec;
            kabbu.spin = Vector3.zero;
        }

        public static IEnumerator DoPebbleTossStylish(EntityControl kabbu, int hits)
        {
            StylishUtils.ShowStylish(1.2f + 0.1f * hits, kabbu, hits == 0 ? 0.1f : 0.02f);
            kabbu.animstate = 118;
            kabbu.spin = new Vector3(0, 20, 0);
            yield return EventControl.thirdsec;
            kabbu.spin = Vector3.zero;
        }

        IEnumerator DoDashThroughStylish()
        {
            EntityControl kabbu = Instance.entityAttacking;
            StylishUtils.ShowStylish(1.2f, kabbu);

            kabbu.animstate = 117;
            MainManager.PlaySound("Spin4");

            ParticleSystem smoke = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/WalkDust")) as GameObject).GetComponent<ParticleSystem>();
            smoke.transform.eulerAngles = new Vector3(-90f, 0f);
            smoke.transform.parent = kabbu.transform;
            smoke.transform.localPosition = new Vector3(0f, 0.25f, 0.1f);
            smoke.GetComponent<Renderer>().material.renderQueue = 3001;
            
            ParticleSystem.EmissionModule se = smoke.emission;
            ParticleSystem.MainModule sd = smoke.main;
            sd.startLifetime = new ParticleSystem.MinMaxCurve(1f);
            se.rateOverTime = new ParticleSystem.MinMaxCurve(5f);
            sd.startSize = new ParticleSystem.MinMaxCurve(0.75f);

            Vector3 centerPoint = MainManager.battle.partymiddle;
            float radius = 2.5f;
            float speed = 15f;      

            float angle = 0f;
            float a = 0f;
            float b = 30f;
            Vector3 basePos = kabbu.transform.position;
            Vector3 baseRotation = kabbu.spritetransform.eulerAngles;

            kabbu.overrideflip = true;

            do
            {
                angle += speed * Time.smoothDeltaTime;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                kabbu.transform.position = new Vector3(x, kabbu.transform.position.y, z) + centerPoint;
                Vector3 direction = (kabbu.transform.position - centerPoint).normalized;
                direction.y = 0;
                kabbu.spritetransform.rotation = Quaternion.LookRotation(direction);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);

            kabbu.spritetransform.eulerAngles = baseRotation;
            yield return LerpPosition(10, kabbu.transform.position, basePos, kabbu.transform);

            kabbu.overrideflip = false;
            Destroy(smoke.gameObject);
            kabbu.spin = new Vector3(0, 10, 0);
            kabbu.animstate = 101;
            yield return EventControl.quartersec;

        }

        public IEnumerator DoStylish(int actionid, int stylishID)
        {
            Instance.entityAttacking?.Emoticon(MainManager.Emoticons.None);
            switch (actionid)
            {
                //Basic Attack
                case -1:
                    yield return DoBasicAttackStylish();
                    break;

                //taunt
                case 3:
                //pep talk
                case 43:
                    yield return DoTauntStylish();
                    break;

                //UnderStrike
                case 6:
                    yield return DoUnderStrikeStylish();
                    break;

                //DashThrough
                case 10:
                    yield return DoDashThroughStylish();
                    break;

                //Boulder Toss
                case 19:
                    yield return DoHeavyStrikeStylish();
                    break;

                //heavyStrike
                case 32:
                    yield return DoHeavyStrikeStylish();
                    break;

                case (int)NewSkill.Lecture:
                    yield return DoPebbleTossStylish(Instance.entityAttacking,stylishID);
                    break;
            }
        }
    }
}
