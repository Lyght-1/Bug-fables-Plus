using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace BFPlus.Extensions.EnemyAI
{
	public class FireAntAI : AI
	{
		BattleControl battle = null;
		public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
		{

			battle = MainManager.battle;

			if (battle.enemydata[actionid].hitaction)
			{
				entity.Emoticon(2, 35);
				MainManager.PlaySound("Wam");
				while (entity.emoticoncooldown > 0f)
				{
					yield return null;
				}
				MainManager.PlaySound("Heal3");
				battle.StartCoroutine(battle.StatEffect(battle.enemydata[actionid].battleentity, 5));
				battle.enemydata[actionid].cantmove--;
				yield break;
			}

			if(UnityEngine.Random.Range(0,10) > 0 || battle.enemydata[actionid].charge == 3)
            {
                battle.nonphyscal=true;
                int FIREBALL_AMOUNT = 2;
				int FIREBALL_DAMAGE = 4;
				Transform[] fireballs = new Transform[FIREBALL_AMOUNT];
				for (int i = 0; i < FIREBALL_AMOUNT; i++)
				{
					if (MainManager.GetAlivePlayerAmmount() > 0)
					{
						battle.GetSingleTarget();
						var playerTargetIDRef = battle.playertargetID;

						MainManager.PlaySound("WaspKingMFireball1");
						MainManager.PlaySound("Blosh");
						entity.animstate = 100;
						yield return new WaitForSeconds(0.15f);
						entity.animstate = 103;
						fireballs[i] = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/Fireball"), entity.transform.position + new Vector3(-0.75f, 1.5f), Quaternion.identity, battle.battlemap.transform) as GameObject).transform;
						float a = 0f;
						float b = 20f;
						do
						{
							fireballs[i].localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 0.75f, a / b);
							a += MainManager.framestep;
							yield return null;
						}
						while (a < b + 1f);
						entity.animstate = 101;
						MainManager.PlaySound("Chew");
						yield return EventControl.tenthsec;
                        battle.StartCoroutine(battle.Projectile(FIREBALL_DAMAGE, BattleControl.AttackProperty.Fire, battle.enemydata[actionid], playerTargetIDRef, fireballs[i], 20, 0, "SepPart@2@4", "Fire", "WaspKingMFireball2", null, Vector3.zero, false));

						yield return EventControl.quartersec;
						entity.animstate = 0;
						while (fireballs[i] != null)
						{
							yield return null;
						}
						yield return EventControl.quartersec;
					}
				}
			}
            else
            {
				battle.dontusecharge = true;
				entity.StartCoroutine(entity.ShakeSprite(0.1f, 45f));
				MainManager.PlaySound("Charge7");
				yield return EventControl.halfsec;
				battle.StartCoroutine(battle.StatEffect(battle.enemydata[actionid].battleentity, 4));
				battle.enemydata[actionid].charge = 3;
				MainManager.PlaySound("StatUp");
				yield return EventControl.halfsec;
			}
			yield return null;
		}
	}
}
