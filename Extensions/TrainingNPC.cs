using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions
{
    public class TrainingNPC : MonoBehaviour
    {
        public bool doingAnim = false;
        EntityControl entity;
        Vector3 target = new Vector3(9.4f,2f,7f);

        float waitTimes = 60f;
        float animSpeed = 20f;

        void Start()
        {
            entity = GetComponent<EntityControl>();


            switch (entity.animid + 1)
            {
                case (int)MainManager.AnimIDs.ShielderAnt:
                    target = new Vector3(15.2f, 2f, -12f);
                    animSpeed = 30f;
                    waitTimes = 30f;
                    break;
                case (int)MainManager.AnimIDs.Gen:
                    target = new Vector3(9.4f, 2f, 7f);
                    animSpeed = 20f;
                    waitTimes = 60f;
                    break;
                case (int)MainManager.AnimIDs.Stratos:
                    waitTimes = 45f;
                    break;
            }
        }

        void Update()
        {
            if(!MainManager.instance.pause && !MainManager.instance.minipause && !MainManager.instance.message && entity != null && !doingAnim)
            {
                if(waitTimes <= 0)
                {
                    waitTimes = 60f;

                    switch (entity.animid+1)
                    {
                        case (int)MainManager.AnimIDs.ShielderAnt:
                            StartCoroutine(DoCeliaAnim());
                            break;
                        case (int)MainManager.AnimIDs.Gen:
                            StartCoroutine(DoGenAnim());
                            break;
                        case (int)MainManager.AnimIDs.Stratos:
                            StartCoroutine(DoStratosAnim());
                            break;
                    }
                }

                if(waitTimes > 0)
                {
                    waitTimes -= MainManager.framestep;
                }
            }
        }


        IEnumerator DoGenAnim()
        {
            doingAnim = true;
            entity.overrideanim = true;

            Vector3 startPos = entity.transform.position + new Vector3(0.75f, 1f, -0.1f);

            int dartType = UnityEngine.Random.Range(0, 2);
            SpriteRenderer item = MainManager.NewSpriteObject(startPos, entity.transform, MainManager.itemsprites[0, (dartType == 0) ? 88 : 40]);
            item.transform.localEulerAngles = new Vector3(0f, 0f, 135f);
            entity.animstate = 100;
            entity.PlaySound("Toss12", 0.2f);

            float a = 0;
            do
            {
                item.transform.position = Vector3.Lerp(startPos, target, a / animSpeed);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < animSpeed);

            entity.animstate = (int)MainManager.Animations.Idle;
            MainManager.DeathSmoke(target);
            Destroy(item.gameObject);
            entity.overrideanim = false;
            doingAnim = false;
        }

        IEnumerator DoCeliaAnim()
        {
            doingAnim = true;
            entity.overrideanim = true;

            entity.animstate = 105;
            entity.spin = new Vector3(0f, 30f);
            yield return EventControl.halfsec;

            entity.spin = Vector3.zero;
            entity.PlaySound("Toss8", 1f, 1f);
            entity.PlaySound("Toss2", 1f, 0.8f);
            Transform shield = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Objects/Shield")).transform;
            shield.GetComponent<SpriteRenderer>().materials = new Material[] { MainManager.spritematlit };
            shield.transform.parent = entity.transform;

            entity.animstate = 102;
            ParticleSystem.MainModule p = shield.GetComponentInChildren<ParticleSystem>().main;
            Vector3 startPos = entity.transform.position + new Vector3(0f, 1.25f, 0f);
            float a = 0f;
            do
            {
                shield.position = Vector3.Lerp(startPos, target, a / animSpeed);
                shield.localEulerAngles += Vector3.forward * 10f * MainManager.framestep;
                p.startRotationMultiplier = shield.localEulerAngles.z;

                a += MainManager.TieFramerate(1f);
                yield return null;

            } while (a < animSpeed);

            MainManager.PlaySoundAt("WoodHit", 1f, target);
            yield return EventControl.tenthsec;

            a = 0f;
            do
            {
                shield.position = Vector3.Lerp(target, startPos, a/animSpeed) + Vector3.up * 1.5f;
                shield.localEulerAngles += Vector3.forward * (-10f * MainManager.framestep);
                p.startRotationMultiplier = shield.localEulerAngles.z;
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < animSpeed);

            MainManager.StopSound(3);
            entity.PlaySound("Ding2", 1f, 1f);
            Destroy(shield.gameObject);
            entity.animstate = 104;

            yield return entity.SlowSpinStop(Vector3.up * -50f, 30f);
            entity.animstate = (int)MainManager.Animations.Idle;
            entity.overrideanim = false;
            doingAnim = false;
        }

        IEnumerator DoStratosAnim()
        {
            doingAnim = true;
            entity.overrideanim = true;

            entity.PlaySound("Woosh6", 0.4f);
            entity.animstate = 102;
            yield return EventControl.sec;

            entity.animstate = 103;
            entity.PlaySound("Thud4", 0.4f, 1.1f);
            yield return EventControl.quartersec;
            entity.animstate = 104;
            yield return EventControl.sec;

            entity.PlaySound("Slash2",0.4f);
            entity.animstate = 105;
            yield return EventControl.sec;

            entity.animstate = (int)MainManager.Animations.BattleIdle;
            entity.overrideanim = false;
            doingAnim = false;
        }
    }
}
