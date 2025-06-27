using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static BattleControl;
using UnityEngine;
using HarmonyLib;
using System.Reflection;
using Mono.Cecil;
using static FlappyBee;

namespace BFPlus.Extensions.EnemyAI
{
    //Mothmite
    //Faded Mothfly in a weathered old castle.Has developed different cooperative tactics to the Mothflies of the Forsaken Lands.
    //Attacks:
    //Bash[4 DMG]: Flies up and off the screen, plummeting down towards a random target.
    //If other Mothmites are present, they will all rapid-fire rain down on the same target.
    //Drain Bubble [3 DMG, 3 TP DRAIN]: Lobs a slow bubble of gunk upward, which floats down onto a completely random target.
    //If other Mothmites are present, they will all spit bubbles at random targets in quick succession.
    //Gimmicks:
    //When attacking as a team, Mothmites after the first will deal increasing damage.
    //For any given team attack, every hit grants +1 damage to all subsequent hits.
    public class MothmiteAI : AI
    {
        const int BASE_BASH_DAMAGE = 4;
        const int BASE_DRAIN_BUBBLE_DAMAGE = 3;
        const int BASE_DRAIN_BUBBLE_TP = -3;
        BattleControl battle = null;
        int attackDone = 0;

        public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
        {
            battle = MainManager.battle;
            yield return null;

            int[] mothmites = FindAllMothmites();

            bool hardmode = battle.HardMode();
            if (UnityEngine.Random.Range(0,2) == 0)
            {
                float delay = hardmode ? 0.35f : 0.45f;
                yield return ChangeMothmitesPos(BattlePosition.Ground, mothmites);
                yield return DoAttackAll(mothmites, DoDrainBubble, delay, BASE_DRAIN_BUBBLE_DAMAGE);
            }
            else
            {
                float delay = hardmode ? 0.25f : 0.35f;
                yield return ChangeMothmitesPos(BattlePosition.Flying, mothmites);
                yield return DoAttackAll(mothmites, DoBash, delay, BASE_BASH_DAMAGE);
            }

            foreach (var moth in mothmites)
            {
                //we end turn for each mothmites, but we skip our own cause its gonna get ended after anyway
                if(moth != actionid)
                {
                    battle.EndEnemyTurn(moth);
                }
            }
        }

        int[] FindAllMothmites()
        {
            List<int> mothmites = new List<int>();
            for(int i = 0; i < battle.enemydata.Length; i++)
            {
                if (battle.enemydata[i].animid == (int)NewEnemies.Mothmite && !battle.IsStopped(battle.enemydata[i]))
                {
                    mothmites.Add(i);
                }
            }
            return mothmites.ToArray();
        }

        IEnumerator ChangeMothmitesPos(BattleControl.BattlePosition pos, int[] mothmites)
        {
            foreach (var moth in mothmites)
            {
                battle.StartCoroutine(battle.ChangePosition(moth, pos));
            }
            yield return new WaitUntil(()=>AllPos(pos, mothmites));
        }

        bool AllPos(BattleControl.BattlePosition pos, int[] mothmites)
        {
            foreach(var moth in mothmites)
            {
                if (battle.enemydata[moth].position != pos)
                    return false;
            }
            return true;
        }

        IEnumerator DoAttackAll(int[] mothmites, AttackDelegate attack, float delay, int baseDamage)
        {
            battle.GetSingleTarget();
            for (int i = 0; i < mothmites.Length; i++)
            {
                int mothId = mothmites[i];
                battle.StartCoroutine(attack(battle.enemydata[mothId].battleentity, mothId, baseDamage));
                baseDamage++;
                yield return new WaitForSeconds(delay);
            }
            yield return new WaitUntil(() => attackDone == mothmites.Length);
        }

        IEnumerator DoBash(EntityControl entity, int actionid, int damage)
        {
            int playerTargetId = battle.playertargetID;
            Vector3 targetPos = MainManager.instance.playerdata[playerTargetId].battleentity.transform.position + Vector3.up + Vector3.back * 0.1f;

            float baseHeight = entity.height;
            entity.height = 0f;
            entity.LockRigid(true);
            entity.transform.position = new Vector3(entity.transform.position.x, baseHeight, entity.transform.position.z);
            Vector3 startPos = entity.transform.position;
            MainManager.PlaySound("Toss13");

            yield return BattleControl_Ext.LerpPosition(35, entity.transform.position,new Vector3(15f, 8f) + MainManager.RandomVector(new Vector3(1, 1, 0)), entity.transform);

            entity.transform.position = new Vector3(10, 8, 0);
            entity.animstate = 100;
            yield return EventControl.quartersec;
            MainManager.PlaySound("Toss14");
            yield return BattleControl_Ext.LerpPosition(30, entity.transform.position, targetPos, entity.transform);
            if(MainManager.instance.playerdata[playerTargetId].hp > 0)
            {
                battle.DoDamage(actionid, playerTargetId, damage, null, battle.commandsuccess);
            }
            yield return new WaitForSeconds(0.15f);
            entity.overrideflip = true;
            entity.height = 0.2f;
            entity.animstate = 1;
            
            yield return BattleControl_Ext.LerpPosition(20, entity.transform.position, startPos, entity.transform);
            entity.overrideflip = false;
            entity.transform.position = new Vector3(entity.transform.position.x, 0f, entity.transform.position.z);
            entity.height = baseHeight;
            
            yield return new WaitForSeconds(0.15f);
            attackDone++;
        }

        IEnumerator DoDrainBubble(EntityControl entity, int actionid, int damage)
        {
            battle.nonphyscal = true;

            if (MainManager.GetAlivePlayerAmmount() == 0)
            {
                attackDone++;
                yield break;
            }

            int playerID = battle.GetRandomAvaliablePlayer();

            EntityControl playerEntity = MainManager.instance.playerdata[playerID].battleentity;

            entity.overrideanim = true;
            entity.animstate = (int)MainManager.Animations.Hurt;
            entity.StartCoroutine(entity.ShakeSprite(0.1f, 60f));
            MainManager.PlaySound("Charge10", -1, 1.2f, 1f);
            yield return new WaitForSeconds(0.75f);
            entity.overrideanim = false;
            GameObject bubble = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Objects/WaterBubble"), entity.transform.position + Vector3.up * 2f, Quaternion.identity) as GameObject;
            bubble.transform.localScale = Vector3.one * 0.4f;
            bubble.GetComponent<SpriteBounce>().startscale = true;
            bubble.GetComponent<SpriteRenderer>().color = Color.gray;

            Vector3 bubbleStartPos = bubble.transform.position;
            MainManager.PlaySound("Blosh");
            MainManager.PlaySound("Wub", 9, 0.8f, 1f, true);

            float a = 0f;
            while (a < 55f)
            {
                bubble.transform.position = MainManager.BeizierCurve3(bubbleStartPos, MainManager.instance.playerdata[playerID].battleentity.transform.position, 5f, a / 60f);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }

            entity.animstate = 0;
            MainManager.StopSound(9);
            MainManager.PlaySound("BubbleBurst");
            MainManager.PlayParticle("WaterSplash", MainManager.instance.playerdata[playerID].battleentity.transform.position + Vector3.up);
            battle.DoDamage(actionid, playerID, damage, null, battle.commandsuccess);
            Vector3 startPos = playerEntity.transform.position + Vector3.up * 2f;
            Vector3 endPos = playerEntity.transform.position + Vector3.up * 4f;
            BattleControl_Ext.Instance.RemoveTP(battle.commandsuccess ? BASE_DRAIN_BUBBLE_TP +2 : BASE_DRAIN_BUBBLE_TP, startPos, endPos);

            UnityEngine.Object.Destroy(bubble);
            yield return new WaitForSeconds(0.75f);
            attackDone++;
        }

        delegate IEnumerator AttackDelegate(EntityControl entity, int actionid, int damage);
    }
}
