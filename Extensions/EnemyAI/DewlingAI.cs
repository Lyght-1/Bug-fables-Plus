using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static FlappyBee;

namespace BFPlus.Extensions.EnemyAI
{
    public class DewlingAI : AI
    {
		BattleControl battle = null;
        public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
        {

			battle = MainManager.battle;
			if (battle.enemydata[actionid].data == null)
            {
				battle.enemydata[actionid].data = new int[1];
            }

			if(battle.enemydata[actionid].data[0] >= 2)
            {
				yield return battle.ChangePosition(actionid, BattleControl.BattlePosition.Flying);
                battle.enemydata[actionid].data[0] = 0;
			}

			if (battle.enemydata[actionid].position == BattleControl.BattlePosition.Ground)
            {
				yield return FreezyDew(entity,actionid);
				battle.enemydata[actionid].data[0]++;
				for (int i = 0; i < battle.enemydata.Length; i++)
				{
					if (battle.enemydata[i].animid != (int)NewEnemies.Dewling)
					{
                        battle.Heal(ref battle.enemydata[i], 1, false);
						if (UnityEngine.Random.Range(0, 10) >= 4)
						{
							battle.StartCoroutine(battle.StatEffect(battle.enemydata[i].battleentity, 0));
							MainManager.SetCondition(MainManager.BattleCondition.AttackUp, ref battle.enemydata[i], 2);
						}
						else
						{
							battle.StartCoroutine(battle.StatEffect(battle.enemydata[i].battleentity, 1));
							MainManager.SetCondition(MainManager.BattleCondition.DefenseUp, ref battle.enemydata[i], 2);
						}
					}
				}
			}

			if (battle.enemydata[actionid].position == BattleControl.BattlePosition.Flying)
            {
				bool heal = false;
				if (battle.enemydata.Length > 1)
				{
					for (int i = 0; i < battle.enemydata.Length; i++)
					{
						if (i != actionid && battle.HPPercent(battle.enemydata[i]) < 0.6f)
						{
							heal = true;
							break;
						}
					}
				}

				if (heal)
					yield return DoDewlingHeal(entity, actionid);
				else
					yield return DoAirKick(entity, actionid);
			}

		}

		IEnumerator FreezyDew(EntityControl entity, int actionid)
        {
			int damage = 5;

			battle.nonphyscal = true;
			battle.GetSingleTarget();
            var playerTargetIDRef = battle.playertargetID;

			MainManager.instance.camspeed = 0.03f;
			MainManager.instance.camtargetpos = new Vector3?(entity.transform.position);
			MainManager.instance.camoffset = new Vector3(0f, 1.5f, -5f);
			entity.overrideanim = true;
			entity.animstate = 104;
			entity.StartCoroutine(entity.ShakeSprite(0.1f, 60f));
			MainManager.PlaySound("Charge14", -1, 1f, 1f);
			yield return new WaitForSeconds(0.75f);
			entity.animstate = 103;
			entity.overrideanim = false;
			GameObject bubble = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Objects/WaterBubble"), entity.transform.position + Vector3.up * 2f, Quaternion.identity) as GameObject;
			bubble.transform.localScale = Vector3.one * 0.5f;
			bubble.GetComponent<SpriteBounce>().startscale = true;

			MainManager.PlayParticle("WaterSplash", entity.transform.position + Vector3.up);
			Vector3 bubbleStartPos = bubble.transform.position;
			BattleControl.SetDefaultCamera();
			MainManager.PlaySound("Blosh");
			MainManager.PlaySound("Wub", 9, 0.8f, 1f, true);

			float a = 0f;
			while (a < 55f)
			{
				bubble.transform.position = MainManager.BeizierCurve3(bubbleStartPos, MainManager.instance.playerdata[playerTargetIDRef].battleentity.transform.position + Vector3.up, 5f, a / 60f);
				a += MainManager.TieFramerate(1f);
				yield return null;
			}

			entity.animstate = 0;
			MainManager.StopSound(9);
			MainManager.PlayParticle("WaterSplash", MainManager.instance.playerdata[playerTargetIDRef].battleentity.transform.position + Vector3.up);
			MainManager.PlaySound("BubbleBurst");
			battle.DoDamage(actionid, playerTargetIDRef, damage, BattleControl.AttackProperty.Freeze, battle.commandsuccess);
			UnityEngine.Object.Destroy(bubble);
			yield return new WaitForSeconds(0.75f);
        }

        IEnumerator DoAirKick(EntityControl entity, int actionid)
        {	
			battle.GetSingleTarget();
            var playerTargetIDRef = battle.playertargetID;
			int damage = 5;
			Transform target = MainManager.instance.playerdata[playerTargetIDRef].battleentity.transform;

			MainManager.instance.camoffset = new Vector3(1f, 2.3f, -6f);
			MainManager.instance.camtarget = target;
			MainManager.PlayMoveSound(entity.originalid, 9, true);
			entity.MoveTowards(target.position + Vector3.right * 2f + Vector3.back * 0.15f, 1.5f, 1, 100);
			while (entity.forcemove)
			{
				yield return null;
			}
			MainManager.StopSound(9);
			float th = entity.height;
			entity.height = 0f;
			entity.rigid.useGravity = false;
			entity.rigid.velocity = Vector3.zero;
			entity.transform.position = new Vector3(entity.transform.position.x, th, entity.transform.position.z);
			entity.animstate = 100;

			float a = 0f;
			MainManager.PlaySound("FastWoosh");
			Vector3 tpos = entity.transform.position;
			while (a < 1f)
			{
				entity.transform.position = MainManager.BeizierCurve3(tpos, tpos + Vector3.right * 0.75f, -0.01f, a);
				a += MainManager.TieFramerate(0.055f);
				yield return null;
			}
			entity.animstate = 101;

			Vector3 tpos2 = entity.transform.position;
			yield return new WaitForSeconds(0.2f);
			a = 0f;

			entity.animstate = 100;

			MainManager.PlaySound("Turn2");
			while (a < 1f)
			{
				entity.transform.position = Vector3.Lerp(tpos2, target.position + Vector3.up + Vector3.back * 0.1f, a);
				a += MainManager.TieFramerate(0.07f);
				yield return null;
			}
			battle.DoDamage(actionid, playerTargetIDRef, damage, BattleControl.AttackProperty.Sticky, battle.commandsuccess);

			yield return new WaitForSeconds(0.15f);
			entity.overrideflip = true;
			a = 0f;
			entity.height = 0.2f;
			entity.animstate = 1;
			while (a < 1f)
			{
				entity.transform.position = Vector3.Lerp(target.position + Vector3.up + Vector3.back * 0.1f, tpos, a);
				a += MainManager.TieFramerate(0.045f);
				yield return null;
			}
			entity.overrideflip = false;
			entity.transform.position = new Vector3(entity.transform.position.x, 0f, entity.transform.position.z);
			entity.height = th;
			yield return new WaitForSeconds(0.2f);

			entity.bobrange = entity.startbf;
			entity.bobspeed = entity.startbs;
		}

		IEnumerator DoDewlingHeal(EntityControl entity, int actionid)
        {
			MainManager.PlayMoveSound(entity.originalid, 9, true);
			entity.LockRigid(true);

			Vector3 basePosition = entity.transform.position;
			float a = 0f;
			float b = 50f;
			Vector3 pos = entity.transform.position;
			GameObject healParticles = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/FloweringHeal"),entity.transform.position + new Vector3(0,entity.height), Quaternion.identity) as GameObject;
			healParticles.transform.parent = entity.transform;
			Vector3 targetPos = new Vector3(0.5f,2);
			MainManager.PlaySound("Charge14");
			entity.spin = new Vector3(0f, 10f);
			do
			{
				entity.transform.position = Vector3.Lerp(entity.transform.position, targetPos, a/b);
				a += MainManager.TieFramerate(1f);
				yield return null;
			}
			while (a < b);

			entity.ForceMove(new Vector3(0.5f, 2), 15f, 1, 0);
			while (entity.forcemoving != null)
			{
				yield return null;
			}
			entity.ForceMove(new Vector3(7.5f, 2), 45f, 1, 0);
			while (entity.forcemoving != null)
			{
				yield return null;
			}
			MainManager.StopSound(9);
			yield return EventControl.halfsec;
			MainManager.DestroyTemp(ref healParticles, 2f);

			for (int i = 0; i < battle.enemydata.Length; i++)
			{
				if(i != actionid)
                {
                    battle.Heal(ref battle.enemydata[i], 4, false);
                    battle.ClearStatus(ref battle.enemydata[i]);
				}
			}
			MainManager.PlaySound("StatUp");
			entity.spin = Vector3.zero;
			a = 0f;
			b = 50f;
			pos = entity.transform.position;
			do
			{
				entity.transform.position = Vector3.Lerp(pos, basePosition, a / b);
				a += MainManager.TieFramerate(1f);
				yield return null;
			} while (a < b);

			entity.transform.position = basePosition;
			yield return EventControl.quartersec;
			MainManager.DestroyTemp(ref healParticles, 2f);
			entity.spin = Vector3.zero;
			entity.LockRigid(false);	
		}
    }
}
