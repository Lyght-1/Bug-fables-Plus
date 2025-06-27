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
    public class SandCastleDepthsBossEvent : StartBossFightEvent
    {
        protected override IEnumerator ApproachingBoss(EventControl instance, NPCControl caller)
        {
            EntityControl[] scorp = MainManager.GetEntities(new int[] { 2 });
            Vector3[] posArray = new Vector3[]
            {
                new Vector3(0,-6.8f,-1),
                new Vector3(1.5f,-6.8f,0),
                new Vector3(-1.5f,-6.8f,0),
                new Vector3(-1, -6.8f, -8.5f)
            };

            MainManager.SetCamera(scorp[0].transform, null, 0.035f, MainManager.defaultcamoffset + Vector3.back * 5f);
            yield return MoveParty(instance, caller, posArray);

            Transform icecube = MainManager.map.transform.Find("scorpIce");

            EntityControl[] party = MainManager.GetPartyEntities(true);
            foreach (var member in party)
            {
                member.Emoticon(MainManager.Emoticons.Exclamation, 45);
            }

            float a = 0f;
            float b = 120f;
            Vector3 basePos = icecube.position;
            do
            { 
                icecube.position = new Vector3(basePos.x + UnityEngine.Random.Range(-0.1f, 0.1f), basePos.y, basePos.z + UnityEngine.Random.Range(-0.1f, 0.1f));
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);

            Destroy(Instantiate(Resources.Load("Prefabs/Particles/IceShatter"), icecube.position, Quaternion.Euler(-90f, 0f, 0f)) as GameObject, 1f);
            scorp[0].PlaySound("IceBreak", 0.65f);
            Destroy(icecube.gameObject);
            scorp[0].animstate = 0;
            scorp[0].anim.speed = 1f;
            scorp[0].onground = false;
            scorp[0].Jump();

            foreach (var member in party)
            {
                member.animstate = (int)MainManager.Animations.Surprized;
            }
            yield return EventControl.tenthsec;

            scorp[0].animstate = 100;
            yield return EventControl.halfsec;
            yield return EventControl.quartersec;
            yield return StartBattle(new int[] { (int)NewEnemies.DullScorp });
            yield return DoWinFightEvent(instance, caller, scorp, party, (int)NewItem.SilverClaw, 856, 1, new Vector3(scorp[0].startpos.Value.x, -6.8f,scorp[0].startpos.Value.z));
        }
    }
}
