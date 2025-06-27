using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.EnemyAI
{
    public class LongLegsSpiderAI : AI
    {
        enum Attacks
        {
            Stomp,
            WebSpit
        }
        BattleControl battle = null;
        const int STOMP_DAMAGE = 6;
        const int WEB_SPIT_AMOUNT = 3;
        const int WEB_SPIT_DAMAGE = 2;
        public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
        {
            battle = MainManager.battle;

            Dictionary<Attacks, int> attacks = new Dictionary<Attacks, int>()
            {
                { Attacks.Stomp, 60},
                { Attacks.WebSpit, 40},
            };

            Attacks attack = MainManager_Ext.GetWeightedResult(attacks);
            switch (attack)
            {
                case Attacks.Stomp:
                    yield return DoStomp(entity, actionid);
                    break;

                case Attacks.WebSpit:
                    yield return DoWebSpit(entity, actionid);
                    break;
            }
        }

        IEnumerator DoStomp(EntityControl entity, int actionid) 
        {
            battle.GetSingleTarget();
            entity.MoveTowards(battle.playertargetentity.transform.position + Vector3.right*2,1.5f);

            yield return new WaitUntil(() => !entity.forcemove);
            MainManager.PlaySound("longLegsGrow");
            entity.animstate = 100;
            MainManager.PlaySound("Grow1");

            yield return new WaitForSeconds(0.75f);
            entity.animstate = 101;
            MainManager.PlaySound("longLegsStomp");

            yield return new WaitForSeconds(0.15f);
            battle.DoDamage(actionid, battle.playertargetID, STOMP_DAMAGE, BattleControl.AttackProperty.Flip, battle.commandsuccess);
            yield return EventControl.halfsec;
            entity.animstate = 0;
        }

        IEnumerator DoWebSpit(EntityControl entity, int actionid)
        {
            battle.nonphyscal = true;
            MainManager.PlaySound("Chew", -1, 1.5f,1, true);
            entity.animstate = 102;
            yield return EventControl.sec;
            MainManager.StopSound("Chew");

            for (int i = 0; i < WEB_SPIT_AMOUNT; i++) 
            {
                if (MainManager.GetAlivePlayerAmmount() == 0)
                    break;
                entity.animstate = 103;
                battle.GetSingleTarget();
                yield return EventControl.halfsec;
                MainManager.PlaySound("AhoneynationSpit", 1.2f);
                entity.animstate = 104;
                SpriteRenderer webWad = MainManager.NewSpriteObject(entity.transform.position + new Vector3(-0.8f, 5, -0.1f), battle.battlemap.transform, MainManager.itemsprites[0, (int)NewItem.WebWad]);
                float speed = 40f;
                if (battle.HardMode())
                    speed = 30f;

                battle.StartCoroutine(battle.Projectile(
                    WEB_SPIT_DAMAGE, BattleControl.AttackProperty.Sticky, battle.enemydata[actionid], battle.playertargetID,
                    webWad.transform, speed,10, "keepcolor", null, null, null, new Vector3(0,0,20), false));
                yield return EventControl.quartersec;
                yield return new WaitUntil(() => webWad == null);

            }
            yield return null;
        }
    }
}
