using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static BattleControl;

namespace BFPlus.Extensions.EnemyAI
{

    public class FirePopperAI : AI
    {
        enum Attacks
        {
            FireToss,
            PopJump
        }

        BattleControl battle = null;
        const int FIRETOSS_DMG = 3;
        const int POPJUMP_DMG = 3;
        public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
        {
            battle = MainManager.battle;

            Dictionary<Attacks, int> attacks = new Dictionary<Attacks, int>()
            {
                { Attacks.FireToss, 60},
                { Attacks.PopJump, 40},
            };

            Attacks attack = MainManager_Ext.GetWeightedResult(attacks);

            switch (attack)
            {
                case Attacks.PopJump:
                    yield return DoPopJump(entity, actionid);
                    break;
                case Attacks.FireToss:
                    yield return DoFireToss(entity, actionid);
                    break;
            }
        }

        IEnumerator DoPopJump(EntityControl entity, int actionid)
        {
            Vector3 basePos = entity.transform.position;

            bool hardmode = battle.HardMode();
            battle.GetSingleTarget();
            entity.animstate = 101;
            MainManager.PlaySound("Charge7", 1.2f, 1);
            entity.StartCoroutine(entity.ShakeSprite(0.2f, 45f));
            yield return EventControl.sec;
            MainManager.PlaySound("Boing1", 1f, 1);
            entity.overrideanim = true;
            entity.LockRigid(true);
            entity.animstate = 104;
            yield return EventControl.tenthsec;
            float a = 0;
            float b = 40;
            Vector3 startPos = entity.transform.position;
            Vector3 targetPos = battle.playertargetentity.transform.position + Vector3.up;
            do
            {
                entity.transform.position = MainManager.BeizierCurve3(startPos, targetPos, 20, a / b);

                if (a > b / 2)
                {
                    entity.animstate = 105;
                }
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);

            yield return null;
            entity.animstate = 106;
            yield return EventControl.tenthsec;
            int turns = hardmode ? 3 : 2;
            battle.DoDamage(actionid, battle.playertargetID, POPJUMP_DMG, null, battle.commandsuccess);
            DoFirePierce(ref MainManager.instance.playerdata[battle.playertargetID], turns);

            yield return EventControl.tenthsec;
            entity.animstate = 107;
            MainManager.PlaySound("Boing1", 1f, 1);

            yield return EventControl.tenthsec;
            yield return BattleControl_Ext.LerpPosition(20, entity.transform.position, entity.transform.position + Vector3.up *4, entity.transform);

            bool stylish = UnityEngine.Random.Range(0, 10) < 7;

            if (stylish)
            {
                entity.animstate = 108;
                MainManager.PlaySound("AtkSuccess", 1.2f, 1);
                battle.StartCoroutine(BattleControl_Ext.Instance.ShowStylishMessage(entity, Vector3.down*2));

                a = 0;
                b = 25;
                do
                {
                    entity.model.eulerAngles = new Vector3(0f, 0, Mathf.Lerp(0f, 360f, a / b));
                    a += MainManager.TieFramerate(1f);
                    yield return null;
                } while (a < b);
                entity.model.localEulerAngles = Vector3.zero;
            }

            entity.animstate = 105;
            yield return BattleControl_Ext.LerpPosition(20, entity.transform.position, targetPos, entity.transform);

            entity.animstate = 106;
            yield return EventControl.tenthsec;

            battle.DoDamage(actionid, battle.playertargetID, POPJUMP_DMG, null, battle.commandsuccess);
            DoFirePierce(ref MainManager.instance.playerdata[battle.playertargetID], turns);
            yield return EventControl.tenthsec;

            entity.animstate = 107;
            MainManager.PlaySound("Boing1", 1f, 1);

            yield return EventControl.tenthsec;
            startPos = entity.transform.position;
            entity.animstate = 108;
            a = 0;
            b = 30;
            do
            {
                entity.transform.position = MainManager.BeizierCurve3(startPos, basePos, 10, a / b);
                entity.model.eulerAngles = new Vector3(0f, 0, Mathf.Lerp(0f, 360f, a / b));
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);

            entity.transform.position = basePos;
            entity.model.localEulerAngles = Vector3.zero;

            entity.animstate = 101;
            yield return EventControl.tenthsec;
            entity.animstate = 110;
            yield return EventControl.halfsec;

            entity.animstate = 0;
            entity.LockRigid(false);
        }

        IEnumerator DoFireToss(EntityControl entity, int actionid)
        {
            bool hardmode = battle.HardMode();
            battle.nonphyscal = true;
            bool burnEnemy = UnityEngine.Random.Range(0, 2) == 0;

            if(battle.enemydata.Length == 1)
            {
                burnEnemy = false;
            }

            EntityControl target = null;
            int targetId = -1;
            if (burnEnemy) 
            {
                var possibleEnemies = battle.enemydata.Select((e, i) => (e, i))
                .Where(x => x.i != actionid && x.e.position != BattlePosition.Underground)
                .Select(x => x.i)
                .ToArray();
                targetId = possibleEnemies[UnityEngine.Random.Range(0, possibleEnemies.Length)];
                target = battle.enemydata[targetId].battleentity;
            }
            else
            {
                battle.GetSingleTarget();
                target = battle.playertargetentity;
                targetId = battle.playertargetID;
            }

            entity.FaceTowards(target.transform.position);
            entity.animstate = 101;
            yield return EventControl.tenthsec;
            MainManager.PlaySound("Charge7",1.2f,1);
            entity.StartCoroutine(entity.ShakeSprite(0.2f, 45f));
            yield return EventControl.quartersec;
            Transform fireball = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/Fireball"), entity.transform.position + new Vector3(0f, 1.75f), Quaternion.identity, battle.battlemap.transform) as GameObject).transform;
            MainManager.PlaySound("WaspKingMFireball1");

            float a = 0f;
            float b = 20f;
            do
            {
                fireball.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 0.75f, a / b);
                a += MainManager.framestep;
                yield return null;
            }
            while (a < b + 1f);
            entity.animstate = 103;
            MainManager.PlaySound("Boing1");

            yield return EventControl.tenthsec;
            Vector3 targetPos = target.transform.position + Vector3.up * target.height;
            yield return MainManager.ArcMovement(fireball.gameObject, fireball.position, targetPos, new Vector3(0, 0, 20), 10, 35, true);
            MainManager.PlayParticle("Fire", targetPos);
            int turns = hardmode ? 4 : 3;
            if (burnEnemy)
            {
                MainManager.SetCondition(MainManager.BattleCondition.Fire, ref battle.enemydata[targetId], turns);
            }
            else
            { 
                
                battle.DoDamage(actionid, targetId, FIRETOSS_DMG, null, battle.commandsuccess);
                DoFirePierce(ref MainManager.instance.playerdata[targetId], turns);
            }

            yield return null;
        }

        void DoFirePierce(ref MainManager.BattleData target, int turns)
        {
            if (target.hp > 0)
            {

                if (battle.commandsuccess)
                {
                    turns--;
                    if (battle.GetSuperBlock(0))
                        turns -= 1;
                }
                battle.TryCondition(ref target, MainManager.BattleCondition.Fire, turns);
            }
        }
    }
}
