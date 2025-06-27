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
	public class BatteryShroomAI : AI
	{
		BattleControl battle = null;
		int DAMAGE_POWER_SURGE = 8;
		int DAMAGE_SPARK_SHOT = 4;
		int DAMAGE_SPIN = 5;
		public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
		{
			battle = MainManager.battle;

			int dynamoSporeIndex = battle.EnemyInField((int)NewEnemies.DynamoSpore);
            if (dynamoSporeIndex > -1)
			{
				bool isStopped = battle.IsStopped(battle.enemydata[dynamoSporeIndex], true);
                float dynamoSporeHpPercent = battle.HPPercent(battle.enemydata[dynamoSporeIndex]);

                if (dynamoSporeHpPercent <= 0.3f && !isStopped)
				{
					yield return DoPowerPlug(entity, actionid);
					yield break;
				}
			}

			if(UnityEngine.Random.Range(0,2) == 0)
            {
				yield return DoDrainSpin(entity, actionid);
            }
            else
            {
				yield return DoSparkShot(entity, actionid);
			}
			yield return null;
			entity.animstate = 0;
			yield return battle.ChangePosition(actionID, UnityEngine.Random.Range(0, 2) == 0 ? BattleControl.BattlePosition.Ground : BattleControl.BattlePosition.Flying);
			yield return null;
		}

		IEnumerator DoSparkShot(EntityControl entity, int actionid)
		{
			battle.nonphyscal = true;
			entity.initialheight = 2;
			yield return battle.ChangePosition(actionID, BattleControl.BattlePosition.Ground);

			MainManager.instance.camspeed = 0.03f;
			MainManager.instance.camtargetpos = new Vector3?(entity.transform.position);
			MainManager.instance.camoffset = new Vector3(0f, 1.5f, -5f);
			entity.height = 0f;
			battle.GetSingleTarget();

			entity.animstate = 102;
			MainManager.PlaySound("Charge", -1, 0.8f, 1f);

			yield return new WaitForSeconds(1.25f);
			entity.animstate = 104;
			float a = 0f;
			GameObject spark = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Objects/EnergySphere"), entity.transform.position + Vector3.up * 2f, Quaternion.identity) as GameObject;
			Vector3 startPos = spark.transform.position;
			BattleControl.SetDefaultCamera();
			MainManager.PlaySound("Blosh");
			MainManager.PlaySound("Wub", 9, 0.7f, 1f, true);

			while (a < 60f)
			{
				spark.transform.position = MainManager.BeizierCurve3(startPos, battle.playertargetentity.transform.position + Vector3.up, 5f, a / 60f);
				a += MainManager.TieFramerate(1f);
				yield return null;
			}
			MainManager.StopSound(9);
			MainManager.PlaySound("PingShot");
			MainManager.PlayParticle("Stars", battle.playertargetentity.transform.position);

            BattleControl.AttackProperty? property = null;

            if (MainManager.HasCondition(MainManager.BattleCondition.Numb, MainManager.instance.playerdata[battle.playertargetID]) == -1)
            {
                property = BattleControl.AttackProperty.Numb;
            }

            battle.DoDamage(actionid, battle.playertargetID, DAMAGE_SPARK_SHOT, property, battle.commandsuccess);
			UnityEngine.Object.Destroy(spark);
			yield return new WaitForSeconds(0.75f);
		}
		IEnumerator DoDrainSpin(EntityControl entity, int actionid)
		{
			battle.GetSingleTarget();
			int playerTargetID = battle.playertargetID;
			
			entity.initialheight = 2;
			yield return battle.ChangePosition(actionID, BattleControl.BattlePosition.Flying);

			Vector3 basePosition = entity.transform.position;

			//first mov
			MainManager.PlaySound("Spin", 9, 0.8f, 0.6f, true);
			entity.animstate = 101;
			Vector3 targetPos = entity.spritetransform.position + Vector3.up + Vector3.right;

			float a = 0f;
			float b = 40f;
			Vector3 startPos = entity.transform.position;
			do
			{
				entity.transform.position = MainManager.BeizierCurve3(startPos, targetPos, -1f, a / b);
				a += MainManager.TieFramerate(1f);
				yield return null;
			}
			while (a < b);

			//second mov
			entity.animstate = 100;
			MainManager.PlaySound("Woosh2");

			a = 0f;
			b = 30f;
			startPos = entity.transform.position;
			do
			{
				entity.transform.position = MainManager.BeizierCurve3(startPos, battle.playertargetentity.transform.position, -1f, a / b);
				a += MainManager.TieFramerate(1f);
				yield return null;
			}
			while (a < b);
			MainManager.StopSound(9, 0.1f);

            BattleControl.AttackProperty? property = null;

            if (MainManager.HasCondition(MainManager.BattleCondition.Sleep, MainManager.instance.playerdata[battle.playertargetID]) == -1)
            {
                property = BattleControl.AttackProperty.Sleep;
            }

            battle.DoDamage(actionid, playerTargetID, DAMAGE_SPIN, property, battle.commandsuccess);

			//third mov
			entity.animstate = 101;
			a = 0f;
			b = 30f;
			startPos = entity.transform.position;
			do
			{
				entity.transform.position = MainManager.BeizierCurve3(startPos, basePosition,2f, a / b);
				a += MainManager.TieFramerate(1f);
				yield return null;
			}
			while (a < b);
		}

		delegate void CleanKillDelegate(ref MainManager.BattleData target, Vector3 startp);
		IEnumerator DoPowerPlug(EntityControl entity, int actionid)
		{
			int dynamoSporeIndex = battle.EnemyInField((int)NewEnemies.DynamoSpore);
            if (dynamoSporeIndex > -1)
			{
				battle.nonphyscal = true;
				EntityControl dynamo = battle.enemydata[dynamoSporeIndex].battleentity;
				Vector3 startTp = entity.transform.position;

				var light = dynamo.GetComponent<DynamoSporeLight>();
				light.overrideLight = true;
				dynamo.StartCoroutine(light.DoChargeUp(10f));
				
				if (entity.height <= 0)
					battle.enemydata[actionID].position = BattleControl.BattlePosition.Ground;

				MainManager.PlaySound("Fly", 8, 0.5f, 1f, true);
				entity.initialheight = 2;

				yield return battle.ChangePosition(actionID, BattleControl.BattlePosition.Flying);

				entity.LockRigid(true);
				entity.ForceMove(dynamo.transform.position + new Vector3(0.05f, 1.7f, 0.01f), 50f, (int)MainManager.Animations.Idle, (int)MainManager.Animations.Idle);
				while (entity.forcemoving != null)
					yield return null;

				MainManager.StopSound(8);

				yield return new WaitUntil(() => light.state == DynamoSporeLight.LightState.Full);
				light.mode = DynamoSporeLight.Mode.Stay;

				//move arms around
				entity.overrideanim = true;
				entity.animstate = 103;
				yield return new WaitForSeconds(0.416f);

				//windup
				entity.animstate = 105;
				MainManager.PlaySound("Charge", -1, 0.8f, 1f);
				yield return new WaitForSeconds(0.25f);

				//plug arms in
				MainManager.PlaySound("Boing1", -1, 1.4f, 0.8f);
				entity.animstate = 106;
				yield return null;
				dynamo.animstate = (int)MainManager.Animations.Angry;
				yield return new WaitForSeconds(0.33f);

				entity.animstate = 110;
				yield return EventControl.quartersec;

				battle.GetSingleTarget();

				//charge anim
				dynamo.StartCoroutine(dynamo.ShakeSprite(0.2f, 120f));
				var elecParticles = MainManager.PlayParticle("Elec", dynamo.transform.position + Vector3.up, -1f);
				entity.animstate = 107;
				MainManager.PlaySound("Charge7", -1, 1.4f, 1f);
				light.chargeFrame = 25f;
				light.mode = DynamoSporeLight.Mode.ChargeDown;
				yield return EventControl.tenthsec;
				MainManager.PlaySound("BOSSHolo1", -1, 1.4f, 1f);
				yield return new WaitUntil(() => light.state == DynamoSporeLight.LightState.Off);
				light.mode = DynamoSporeLight.Mode.Stay;

				entity.animstate = 108;
				entity.StartCoroutine(MainManager.LightingBolt(entity.transform.position + new Vector3(0, entity.height), battle.playertargetentity.transform.position + new Vector3(0f, 1.25f, -0.1f), Mathf.CeilToInt(Mathf.Abs(entity.transform.position.x - battle.playertargetentity.transform.position.x) / 2f), 0.75f, Color.yellow, 5f, 0.35f));
				yield return EventControl.tenthsec;
				//MainManager.PlaySound("Lazer2", -1, 1.4f,1f);

				MainManager.PlaySound("Shock", -1, 1f, 1f);
				MainManager.PlaySound("Shot", -1, 0.8f, 1f);

                BattleControl.AttackProperty? property = null;
                if (MainManager.HasCondition(MainManager.BattleCondition.Numb, MainManager.instance.playerdata[battle.playertargetID]) == -1)
                {
                    property = BattleControl.AttackProperty.Numb;
                }

                battle.DoDamage(actionID, battle.playertargetID, DAMAGE_POWER_SURGE, property, battle.commandsuccess);

				UnityEngine.Object.Destroy(elecParticles);
				yield return EventControl.halfsec;
				entity.animstate = 109;
				battle.selfsacrifice = true;
				yield return EventControl.halfsec;

                battle.Heal(ref battle.enemydata[dynamoSporeIndex], battle.enemydata[actionID].hp/2, false);
				battle.enemydata[dynamoSporeIndex].charge = Mathf.Clamp(battle.enemydata[actionID].charge + 2, 2, 3);

                dynamo.animstate = (int)MainManager.Animations.Idle;
				light.ResetData();

				MainManager.DeathSmoke(entity.transform.position + Vector3.back / 2f + Vector3.up);

				battle.CleanKill(ref battle.enemydata[actionID], startTp);
				battle.StartCoroutine(WaitUntilReorganize(battle.enemydata.Length, entity));
				yield return null;
			}
			yield return null;
		}

		IEnumerator WaitUntilReorganize(int enemyDataLength,EntityControl entity)
		{
			yield return new WaitUntil(()=>enemyDataLength != MainManager.battle.enemydata.Length);
			UnityEngine.Object.Destroy(entity.gameObject);
		}
	}

}
