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
    public class ViStylish : IStylish
    {
        IEnumerator DoBasicAttackStylish()
        {
            yield return null;
            EntityControl vi = Instance.entityAttacking;
            vi.overrideflip = false;
            vi.flip = true;
            vi.spin = new Vector3(0, 20, 0);
            vi.animstate = 111;

            StylishUtils.ShowStylish(1.2f, vi);
            yield return EventControl.halfsec;
            vi.spin = Vector3.zero;
            yield return EventControl.quartersec;
        }

        IEnumerator DoTornadoTossHitStylish()
        {
            EntityControl vi = Instance.entityAttacking;
            StylishUtils.ShowStylish(1.3f, vi, 0.02f);
            vi.flip = !vi.flip;
            vi.animstate = 109;
            yield return EventControl.quartersec;
        }

        IEnumerator DoStashStylish()
        {
            EntityControl vi = Instance.entityAttacking;
            StylishUtils.ShowStylish(1.2f, vi);
            vi.animstate = (int)MainManager.Animations.Happy;
            yield return EventControl.halfsec;
        }

        IEnumerator DoHeavyThrowStylish()
        {
            EntityControl vi = Instance.entityAttacking;
            StylishUtils.ShowStylish(1.2f, vi);
            vi.animstate = 111;
            yield return EventControl.halfsec;
        }

        IEnumerator DoNeedleTossStylish()
        {
            EntityControl vi = Instance.entityAttacking;
            StylishUtils.ShowStylish(1.2f, vi);
            vi.animstate = 107;
            vi.spin = new Vector3(0, 20, 0);
            yield return EventControl.quartersec;
            vi.spin = Vector3.zero;
        }

        IEnumerator DoNeedlePincerStylish()
        {
            BattleControl.SetDefaultCamera();
            EntityControl vi = Instance.entityAttacking;
            StylishUtils.ShowStylish(1.2f, vi);
            vi.animstate = 111;
            vi.spin = new Vector3(0, 30, 0);

            Vector3[] partyPos = MainManager.battle.partypos;
            int viIndex = Array.FindIndex(MainManager.battle.partypointer, x => x == MainManager.battle.currentturn);
            Vector3 endPos = partyPos[viIndex];
            Vector3 startPos = vi.transform.position;
            float a = 0;
            float b = 35f;
            do
            {
                vi.transform.position = Vector3.Lerp(startPos, endPos, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);

            yield return EventControl.quartersec;
            vi.spin = Vector3.zero;
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
                
                //Tornado Toss
                case 2:

                    switch (stylishID)
                    {
                        //End hit
                        case 0:
                            yield return DoBasicAttackStylish();
                            break;

                        //tornado hits
                        case 1:
                            yield return DoTornadoTossHitStylish();
                            break;
                    }
                    break;

                //needle toss
                case 16:
                    yield return DoNeedleTossStylish();
                    break;

                //Hurricane toss
                case 18:
                    yield return DoBasicAttackStylish();
                    break;

                case 24:
                    yield return DoNeedlePincerStylish();
                    break;

                //stash / sharing stash
                case 11:
                case 45:
                    yield return DoStashStylish();
                    break;

                //heavy throw
                case 44:
                    yield return DoHeavyThrowStylish();
                    break;

                case (int)NewSkill.Steal:
                    yield return DoBasicAttackStylish();
                    break;

            }
        }
    }
}
