using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static BFPlus.Extensions.BattleControl_Ext;

namespace BFPlus.Extensions.EnemyAI
{
	public class DullScorpAI : AI
	{
		enum Attacks
		{
			Claw,
			Iceball,
			IcyRocks
		}

		//Dull Scorp Attacks

		//Ice balls
		//Throws iceballs with his 2 claws, similar to wasp king fireballs, freeze if not blocked

		//Ice Cubes
		//Same thing as the rock throw attack, but use the stinger to carve out an iceblock into smaller ones?

		//Claw attacks
		//claw attack that can freeze


		//Scorpion Attacks
		//Claw attack
		//rock throw
		//Stinger attack
		int ICEBALL_DAMAGE = 4;
		int ICYROCK_DAMAGE = 7;
		int CLAW_DAMAGE = 3;

		BattleControl battle = null;
		public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
		{
			battle = MainManager.battle;

            float hpPercent = battle.HPPercent(battle.enemydata[actionid]);

            if (battle.enemydata[actionid].data == null)
			{
				battle.enemydata[actionid].data = new int[1];
			}

			if (hpPercent <= 0.5f && battle.enemydata[actionid].data[0] == 0)
            {
                MainManager.PlaySound("Charge7", -1, 0.8f, 1f);
                battle.StartCoroutine(entity.ShakeSprite(0.1f, 30f));
                yield return EventControl.halfsec;
                battle.enemydata[actionid].moves = 2;
				battle.enemydata[actionid].cantmove--;
                battle.StartCoroutine(battle.StatEffect(entity, 5));
                MainManager.PlaySound("StatUp");
				battle.enemydata[actionid].data[0] = 1;
                yield return EventControl.halfsec;
            }

            List<Attacks> chances = new List<Attacks>() { Attacks.Claw, Attacks.Claw, Attacks.Iceball, Attacks.Iceball, Attacks.IcyRocks };

			if (battle.enemydata.Length == 1)
			{
                chances.AddRange(new Attacks[] { Attacks.IcyRocks, Attacks.IcyRocks });
            };

            switch (chances[UnityEngine.Random.Range(0, chances.Count)])
            {
                case Attacks.Claw:
                    yield return DoClawsAttack(entity, actionid);
                    break;
                case Attacks.Iceball:
                    yield return DoIceballs(entity, actionid);
                    break;
                case Attacks.IcyRocks:
                    yield return DoIcyRocks(entity, actionid);
                    break;
            }
		}


		IEnumerator DoIceballs(EntityControl entity, int actionid)
        {
            battle.nonphyscal = true;

            float hpPercent = battle.HPPercent(battle.enemydata[actionid]);
            int iceballAmounts = hpPercent > 0.5f? 2 : 3;
			
			for(int i = 0; i < iceballAmounts; i++)
            {
				if (MainManager.GetAlivePlayerAmmount() == 0)
					break;

				battle.GetSingleTarget();
				int playerTargetID = battle.playertargetID;

				bool front = i % 2 == 0;
				entity.animstate = front ? 100 : 104;
				MainManager.PlaySound("ScorpionClaw1", front? 1.2f : 1.15f, 1f);

				yield return EventControl.thirdsec;


				Vector3 iceballPos = front ? new Vector3(-1.2f, 2.2f, -0.7f) : new Vector3(-1.2f, 3.2f, 0.1f);
				Transform iceBall = CreateIceBall(entity.transform.position + iceballPos);
				float a = 0f;
				float b = 30f;
				do
				{
					iceBall.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, a / b);
					a += MainManager.framestep;
					yield return null;
				}
				while (a < b);

				yield return EventControl.quartersec;
				entity.animstate = front ? 102 : 106;
				MainManager.PlaySound("ScorpionClaw2", front ? 1.2f : 1.3f, 1f);

                battle.StartCoroutine(battle.Projectile
                (
					ICEBALL_DAMAGE,
					BattleControl.AttackProperty.Freeze,
					battle.enemydata[actionid],
					playerTargetID,
					iceBall,
					25f,//speed
					0, //height
					null,
					"mothicenormal",
					"IceMothHit",
					null,
					Vector3.zero, //spin
					false
                ));
				yield return new WaitUntil(() => iceBall == null);
				yield return EventControl.sec;
			}
        }


		Transform CreateIceBall(Vector3 position)
        {
			Transform iceBall = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/Fireball"), position, Quaternion.identity, battle.battlemap.transform) as GameObject).transform;
			UnityEngine.Object.Destroy(iceBall.GetComponentInChildren<ParticleSystem>().gameObject);


			var icePart = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/Snowflakes")) as GameObject;
			icePart.transform.parent = iceBall;
			icePart.transform.localPosition = Vector3.zero;
			foreach (var sr in iceBall.GetComponentsInChildren<SpriteRenderer>())
			{
				sr.sprite = MainManager_Ext.assetBundle.LoadAsset<Sprite>("ScorpIceballProj");
			}
			return iceBall;
		}


		IEnumerator DoIcyRocks(EntityControl entity, int actionid)
        {
            battle.nonphyscal = true;

            entity.animstate = 110;
			MainManager.PlaySound("Dig2");
			GameObject digParticle = MainManager.PlayParticle("Digging", entity.transform.position + new Vector3(-3f, 0f));
			GameObject iceball = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Objects/IceSphere")) as GameObject;
			iceball.transform.rotation = Quaternion.Euler(0, 0, 0);
			UnityEngine.Object.Destroy(iceball.transform.GetChild(0).gameObject);

			GameObject icePart = MainManager.PlayParticle("IceRadius", iceball.transform.position, -1f);
			icePart.transform.localScale = Vector3.one * 7f;
			icePart.transform.parent = iceball.transform;

			bool isSummon = battle.enemydata.Length == 1 && UnityEngine.Random.Range(0, 2) == 0;
			EntityControl enemy = null;
			int enemyID = -1;
			if (isSummon)
            {

                MainManager.AnimIDs[] possibleEnemies = new MainManager.AnimIDs[] { MainManager.AnimIDs.Krawler, MainManager.AnimIDs.Midge, MainManager.AnimIDs.CursedSkull };
                //randomize enemy type
                int choosenEnemy = (int)possibleEnemies[UnityEngine.Random.Range(0, possibleEnemies.Length)] - 1;

				switch (choosenEnemy + 1)
				{
					case (int)MainManager.AnimIDs.Krawler:
						enemyID = (int)MainManager.Enemies.IceKrawler;
						break;

					case (int)MainManager.AnimIDs.Midge:
						enemyID = (int)NewEnemies.Frostfly;
						break;

					case (int)MainManager.AnimIDs.CursedSkull:
						enemyID = (int)MainManager.Enemies.IceWarden;
						break;

				}
                enemy = EntityControl.CreateNewEntity("enemy" + enemyID, choosenEnemy, new Vector3(0f, 0f));
				enemy.gameObject.layer = 9;
				enemy.battle = true;
				yield return null;
				enemy.LockRigid(true);
				enemy.anim.speed = 0f;
				enemy.animstate = (int)MainManager.Animations.Hurt;
				enemy.gameObject.transform.parent = iceball.transform;
				enemy.transform.localPosition = new Vector3(0.001f, -0.002f, 0);
			}
			iceball.transform.position = new Vector3(0f, -10f, entity.transform.position.z + 0.3f);
			digParticle.transform.localScale = Vector3.one * 3f;

			yield return new WaitForSeconds(0.65f);

			digParticle.transform.position = new Vector3(0f, -999f);
			entity.animstate = 111;
			yield return new WaitForSeconds(0.35f);
			MainManager.PlaySound("RockPluck");
			entity.animstate = 112;


			MainManager.ShakeScreen(0.25f, 0.2f);
			MainManager.PlayParticle("DirtExplode2", entity.transform.position + new Vector3(-3f, 0f));
			float a = 0f;
			float b = 10f;
			do
			{
				iceball.transform.position = MainManager.BeizierCurve3(entity.transform.position + new Vector3(-3.5f, -1.25f, -0.1f), entity.transform.position + new Vector3(-0.85f, 5.45f, 0f), 2f, a / b);
				a += MainManager.TieFramerate(1f);
				yield return null;
			}
			while (a < b + 1f);
			yield return new WaitForSeconds(0.3f);


            if (isSummon)
            {
				entity.animstate = 116;
				MainManager.PlaySound("ScorpionTail3");
				yield return EventControl.sec;
				entity.animstate = 117;
				MainManager.PlaySound("ScorpionTail4");
				yield return EventControl.tenthsec;
				MainManager.PlaySound("IceBreak");
				var shatterPart = MainManager.PlayParticle("IceShatter", iceball.transform.position);
				enemy.gameObject.transform.parent = battle.battlemap.transform;
				UnityEngine.Object.Destroy(iceball);

				Vector3[] points = new Vector3[]
				{
					new Vector3(0.5f, 0f, 0f),
					new Vector3(6.55f, 0f, 0f)
				};

                Vector3? vector = battle.GetFreeSpace(points, 1f, true);

				bool destroy = vector == null;
				if (vector == null)
					vector = new Vector3?(new Vector3(-15f, 5f, 0f));

				yield return MainManager.ArcMovement(enemy.transform.gameObject, enemy.transform.position, vector.Value + new Vector3(0f, 0f, -0.2f), Vector3.zero, Mathf.Clamp(Vector3.Distance(enemy.transform.position, vector.Value), 2f, 4.5f), 35f, destroy);

                if (!destroy)
                {
					battle.AddNewEnemy(enemyID, new Vector3(0f, 999f));
					yield return null;
					battle.enemydata[battle.lastaddedid].battleentity.transform.position = enemy.transform.position;
					battle.enemydata[battle.lastaddedid].cantmove = 1;
				}
				UnityEngine.Object.Destroy(enemy.gameObject);
				entity.animstate = (int)MainManager.Animations.Idle;
			}
            else
            {
				//throw iceball
				entity.animstate = 114;
				yield return EventControl.tenthsec;
				MainManager.PlaySound("Toss7");
				Vector3 startPos = iceball.transform.position;
				a = 0f;
				b = 60;
				do
				{
					iceball.transform.position = MainManager.BeizierCurve3(startPos, new Vector3(-4.5f, 1f), 6f, a / b);
					iceball.transform.Rotate(0f, 0f,30* MainManager.TieFramerate(1f));
					a += MainManager.TieFramerate(1f);
					yield return null;
				}
				while (a < b + 1f);
				MainManager.PlaySound("Thud5");
				MainManager.ShakeScreen(0.25f, 0.65f);

				MainManager.PlaySound("Explosion5");
				var mothIcePart = MainManager.PlayParticle("mothicenormal", iceball.transform.position);
				mothIcePart.transform.localScale = mothIcePart.transform.localScale * 2f;
				MainManager.PlayParticle("IceShatter", iceball.transform.position);
				UnityEngine.Object.Destroy(iceball);
				battle.PartyDamage(actionid, ICYROCK_DAMAGE, BattleControl.AttackProperty.Freeze, battle.commandsuccess);
				yield return EventControl.halfsec;
			}
			yield return null;
        }

		IEnumerator DoClawsAttack(EntityControl entity, int actionid)
		{
            battle.GetSingleTarget();
			battle.CameraFocusTarget();
            var playertargetentityRef = battle.playertargetentity;

            entity.MoveTowards(playertargetentityRef.transform.position + new Vector3(3f, 0f, -0.15f), 2f);
            while (entity.forcemove)
            {
                yield return null;
            }
            entity.animstate = 100;
            MainManager.PlaySound("ScorpionClaw1");
            yield return new WaitForSeconds(0.75f);
            entity.animstate = 102;
            MainManager.PlaySound("ScorpionClaw2");
            yield return new WaitForSeconds(0.05f);
            int playerTargetID = battle.playertargetID;
            battle.DoDamage(actionid, playerTargetID, CLAW_DAMAGE, BattleControl.AttackProperty.DefDownOnBlock, battle.commandsuccess);

            yield return new WaitForSeconds(0.35f);
            MainManager.PlaySound("ScorpionClaw1", -1, 0.95f, 1f);
            entity.animstate = 104;
            yield return EventControl.halfsec;
            MainManager.PlaySound("ScorpionClaw2", -1, 1.1f, 1f);
            entity.animstate = 106;
            yield return new WaitForSeconds(0.05f);
            battle.DoDamage(actionid, playerTargetID, CLAW_DAMAGE, BattleControl.AttackProperty.Freeze, battle.commandsuccess);

            yield return new WaitForSeconds(0.75f);
        }
	}

}
