using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Object;
namespace BFPlus.Extensions.Events.NewBossesEvents
{
    public class DeepCaveBossEvent : StartBossFightEvent
    {
        protected override IEnumerator ApproachingBoss(EventControl instance, NPCControl caller)
        {
            EntityControl belosslow = MainManager.GetEntity(1);
            EntityControl[] party = MainManager.GetPartyEntities(true);
            Vector3[] posArray = new Vector3[]
            {
                new Vector3(-58f, -21f, -2f),
                new Vector3(-54f, -21f, -1f),
                new Vector3(-52.5f, -21f, -2f),
                new Vector3(-51f, -21f, -2.5f)
            };

            MainManager.FadeMusic(0.01f);

            var chompy = MainManager.map.chompy;

            MainManager.events.MoveParty(posArray, true);
            if (chompy != null)
            {
                chompy.forcejump = true;
            }
            while (!MainManager.PartyIsNotMoving() || (chompy != null && chompy.forcemove))
            {
                yield return null;
            }

            MainManager.SetCamera(null,party[0].transform.position, 0.035f);
            yield return EventControl.halfsec;
            party[0].flip = true;
            yield return EventControl.halfsec;
            party[0].flip = false;
            yield return EventControl.halfsec;
            party[0].flip = true;

            MainManager.DialogueText(MainManager.map.dialogues[1], party[0].transform, caller);
            while (MainManager.instance.message)
                yield return null;

            Vector3 startPos = party[0].transform.position;
            GameObject rock = MainManager.CreateRock(new Vector3(startPos.x + -1, 20, startPos.z), Vector3.one*1.2f, Vector3.zero);
            rock.AddComponent<SpinAround>().itself = new Vector3(3f, 3f, 3f);

            yield return EventControl.halfsec;

            MainManager.PlaySound("Falling");
            foreach (var member in party)
            {
                member.Emoticon(MainManager.Emoticons.Exclamation, 45);
            }

            party[1].animstate = 105;
            MainManager.DialogueText(MainManager.map.dialogues[2], party[1].transform, caller);
            while (MainManager.instance.message)
                yield return null;

            MainManager.PlaySound("Spin4");
            party[1].animstate = 116;
            party[1].MoveTowards(party[0].transform.position, 2,116,101);
            while (party[1].forcemove)
            {
                yield return null;
            }

            Coroutine posCoroutine = instance.StartCoroutine(BattleControl_Ext.LerpPosition(60, rock.transform.position, startPos, rock.transform));
            MainManager.PlaySound("Damage0");
            party[0].animstate = (int)MainManager.Animations.Hurt;     
            party[1].digging = true;          
            float a = 0f;
            float b = 30f;
            party[0].flip = false;
            do
            {
                party[0].transform.position = MainManager.BeizierCurve3(startPos, new Vector3(-51.5f, -21, -1f), 10, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);

            yield return EventControl.halfsec;
            instance.StopCoroutine(posCoroutine);
            MainManager.PlaySound("RockBreak", -1, 1f, 0.75f);
            MainManager.ShakeScreen(0.1f, 0.5f, true);
            MainManager.CrackRock(rock.transform, true);
            yield return EventControl.halfsec;

            party[1].digging = false;
            party[1].animstate = (int)MainManager.Animations.Jump;
            a = 0f;
            startPos = party[1].transform.position;
            MainManager.PlaySound("Jump");
            do
            {
                party[1].transform.position = MainManager.BeizierCurve3(startPos, new Vector3(-53f, -21, -1f), 10, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);
            party[1].animstate = (int)MainManager.Animations.BattleIdle;

            MainManager.PlaySound("Roar", 0.7f, 1);
            yield return EventControl.sec;

            startPos = belosslow.transform.position;
            belosslow.flip = true;
            a = 0f;
            b = 60;
            do
            {
                belosslow.transform.position = MainManager.BeizierCurve3(startPos, new Vector3(-58f, -21, -2f), 10, a / b);
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);

            MainManager.PlaySound("Thud");
            MainManager.ShakeScreen(0.1f, 0.5f, true);

            foreach (var member in party)
            {
                member.overrideanim = true;
                member.flip = false;
                member.Jump();
                member.animstate = (int)MainManager.Animations.Hurt;
            }
            yield return EventControl.quartersec;
            belosslow.animstate = 102;
            yield return EventControl.sec;
            yield return StartBattle(new int[] { (int)NewEnemies.Belosslow });
            yield return DoWinFightEvent(instance, caller, MainManager.GetEntities(new int[] {1}), party, (int)NewItem.SilverFossil, 864, 1, belosslow.transform.position);
        }
    }
}
