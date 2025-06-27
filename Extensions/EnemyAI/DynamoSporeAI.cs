using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static BattleControl;
using static BFPlus.Extensions.BattleControl_Ext;

namespace BFPlus.Extensions.EnemyAI
{
	public class DynamoSporeAI : AI
	{
		enum Attacks
        {
			DrainJump,
			EnergyTransfer,
			SparkSpores
        }
		//Dynamo Spore
		//Despite its drowsy appearance, this gigantic fungus has boundless energy.
		//While not that strong, its slight defense and unending well of electricity allow it to continuously stall and heal.
		//Moves:
		//Drain Jump - Jumps on all bugs in a line, draining HP from each with a chance of putting them to Sleep.
		//Energy Transfer - Damages and Numbs all bugs, and grants a Charge to other enemies.
		//Spark Spores - Releases a bunch of numbing spores. Two of them hit Team Snakemouth, and two land next to the Dynamoshroom, spawning Battery-shrooms.If there is no more space for new Battery-shrooms, the sparks instead grant enemies extra turns.
		//Abilities:
		//Gains a full Charge after every turn, -1 Charge for every other enemy in the battle. (Gains no Charge with a full team of enemies)
		//Gains 3 HP per turn for every Battery-shroom in the battle.

		//Battery-shroom
		//The juvenile form of the Dynamo Spore. Pretty similar to the Snakemouth enemy.
		//Moves:
		//Spark Shot - Launches a spark at a single bug. (Damage + Numb)
		//Drain Spin - Spins at a bug, draining some HP with a chance of putting them to sleep.
		//Power Plug - Hops on top of the nearest Dynamo Spore and plugs itself in, firing a surge of electricity at the frontmost bug.This surge deals high damage and has a high chance of numbing, but drops the Battery-shroom's HP to 0 in the process, killing it. (Only used if a Dynamo Spore is in the battle and below 40% HP) 

		BattleControl battle = null;
		int ENERGY_TRANSFER_DAMAGE = 4;
		int DRAIN_JUMP_DAMAGE = 3;
		int SPARK_SPORES_DAMAGE = 3;

        public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
		{
			battle = MainManager.battle;

			float dynamoSporeHpPercent = battle.HPPercent(battle.enemydata[actionid]);
			int maxCharge = dynamoSporeHpPercent <= 0.4 ? 5 : 3;

            Dictionary<Attacks, int> attacks = new Dictionary<Attacks, int>()
            {
                { Attacks.DrainJump, 30},
                { Attacks.SparkSpores, 40},
                { Attacks.EnergyTransfer, 30},
            };

			if(dynamoSporeHpPercent <= 0.5f)
			{
                attacks = new Dictionary<Attacks, int>()
				{
					{ Attacks.DrainJump, 25},
					{ Attacks.SparkSpores, 50},
					{ Attacks.EnergyTransfer, 25},
				};
            }



            Attacks attack = MainManager_Ext.GetWeightedResult(attacks);

            switch (attack)
            {
				case Attacks.DrainJump:
					yield return DoDrainJump(entity, actionid);
					break;
				case Attacks.SparkSpores:
					yield return DoSparkSpores(entity, actionid);
					break;
				case Attacks.EnergyTransfer:
					yield return DoEnergyTransfer(entity, actionid);
					break;
			}
			battle.enemydata[actionid].charge = 0;


            //charge up + heal
            entity.animstate = (int)MainManager.Animations.Angry;
			MainManager.PlaySound("Charge7");
			var light = entity.GetComponent<DynamoSporeLight>();
			light.overrideLight = true;
			entity.StartCoroutine(entity.ShakeSprite(0.1f, 45f));
			yield return light.DoChargeUp(15f);

			entity.animstate = (int)MainManager.Animations.Jump;
			yield return EventControl.quartersec;

			entity.animstate = (int)MainManager.Animations.Idle;
			light.ResetData();

			int voltShroomCount = battle.enemydata.Count(e => e.animid == (int)NewEnemies.BatteryShroom);

            battle.enemydata[actionid].charge = maxCharge - voltShroomCount;
			battle.StartCoroutine(battle.StatEffect(battle.enemydata[actionid].battleentity, 4));
			MainManager.PlaySound("StatUp");

			if(voltShroomCount > 0)
			{
                battle.Heal(ref battle.enemydata[actionid], 3 * voltShroomCount, false);
            }
            battle.dontusecharge=true;
			yield return null;

		}

		IEnumerator DoDrainJump(EntityControl entity, int actionid)
        {
			Vector3 startPosition = entity.transform.position;

			var light = entity.GetComponent<DynamoSporeLight>();
			light.overrideLight = true;
			light.chargeFrame = 10f;

			//move towards first alive bug
			for (int i = 0; i < battle.partypointer.Length; i++)
			{
				int index = battle.partypointer[i];
				if (MainManager.instance.playerdata[index].hp > 0)
				{
					EntityControl targetEntity = MainManager.instance.playerdata[index].battleentity;

					battle.MiddleCamera(entity.transform, targetEntity.transform);
					MainManager.PlayMoveSound(entity);
					entity.MoveTowards(targetEntity.transform.position + new Vector3(3f, 0f, -0.1f));
					while (entity.forcemove)
					{
						yield return null;
					}
					MainManager.StopSound(9);
					break;
				}
			}
			float a;
			float b;

			for (int i=0; i < battle.partypointer.Length; i++)
            {
                int index = battle.partypointer[i];
                if (MainManager.instance.playerdata[index].hp > 0)
				{
					EntityControl targetEntity = MainManager.instance.playerdata[index].battleentity;
					entity.LockRigid(true);
					entity.overrridejump = true;
					a = 0f;
					b = 60;
					Vector3 pp = entity.transform.position;
					MainManager.PlaySound("Jump", -1, 0.8f, 1f);
					MainManager.PlaySound("Turn", -1, 0.7f, 1f);
					do
					{
						entity.animstate = ((a / b >= 0.5f) ? 3 : 2);
						entity.transform.position = MainManager.BeizierCurve3(pp, targetEntity.transform.position + new Vector3(0f, 0f, -0.1f), 5.5f, a / b);
						a += MainManager.TieFramerate(1f);
						yield return null;
					}
					while (a / b < 0.85f);
					if (!battle.commandsuccess)
					{
						MainManager.ShakeScreen(new Vector3(0.075f, 0.3f), 0.75f, true);
					}

					BattleControl.AttackProperty? property = null;

					if(MainManager.HasCondition(MainManager.BattleCondition.Sleep, MainManager.instance.playerdata[index]) == -1)
					{
						property = BattleControl.AttackProperty.Sleep;
					}

					int damageDone =battle.DoDamage(battle.enemydata[actionid], ref MainManager.instance.playerdata[index], DRAIN_JUMP_DAMAGE, property, new BattleControl.DamageOverride[]
					{
						BattleControl.DamageOverride.NoDamageAnim
					}, battle.commandsuccess);

					MainManager.PlayParticle("impactsmoke", targetEntity.transform.position);
					MainManager.PlaySound("Thud4");
                    if (!battle.commandsuccess)
                    {
						MainManager.PlaySound("HugeHit3");
                    }
                    else
                    {
						MainManager.ShakeScreen(new Vector3(0.05f, 0.2f), 0.5f, true);
					}

                    battle.Heal(ref battle.enemydata[actionid], damageDone, false);
				}
			}
			a = 0f;
			b = 60f;
			Vector3 lastTargetPos = entity.transform.position;
			do
			{
				entity.animstate = ((a / b >= 0.5f) ? 3 : 2);
				entity.transform.position = MainManager.BeizierCurve3(lastTargetPos, startPosition, 5.5f, a / b);
				a += MainManager.TieFramerate(1f);
				yield return null;
			}
			while (a < b + 1f);

			entity.transform.position = startPosition;
			entity.animstate = 0;
			light.overrideLight = false;
			light.chargeFrame = 30f;
			yield return null;
        }

		IEnumerator DoEnergyTransfer(EntityControl entity, int actionid)
        {
			bool hardmode =battle.HardMode();
			battle.nonphyscal = true;

            if (battle.enemydata.Length == 1)
			{
				entity.MoveTowards(new Vector3(3.5f, 0f), 2f);
				while (entity.forcemove)
				{
					yield return null;
				}
			}

			Vector3 startScale = entity.rotater.localScale;
			MainManager.PlaySound("Jump", -1, 0.8f, 1f);
			entity.Jump(entity.jumpheight * 1.25f);
			yield return EventControl.halfsec;
			while (!entity.onground)
			{
				yield return null;
			}
			battle.CameraFocus(entity);
			entity.animstate = 5;
			SpriteBounce spriteBounce = entity.rotater.gameObject.AddComponent<SpriteBounce>();
			spriteBounce.startscale = true;
			spriteBounce.MessageBounce(2f);
			MainManager.PlaySound("Charge3");

			GameObject elecParticles = MainManager.PlayParticle("Elec", entity.transform.position + Vector3.up, -1f);
			var light = entity.GetComponent<DynamoSporeLight>();
			light.overrideLight = true;
			yield return light.DoChargeUp(45);

			yield return EventControl.halfsec;
			UnityEngine.Object.Destroy(elecParticles);

			GameObject sporeParticles = MainManager.PlayParticle("Spores", entity.transform.position + Vector3.up, -1f);
			var main = sporeParticles.GetComponent<ParticleSystem>().main;
			main.startColor = Color.yellow;
			MainManager.PlaySound("Mist");
			BattleControl.SetDefaultCamera();

            battle.CreateHelpBox(4);

            float commandTime = 165f;
            float[] data = new float[] { 4f, 8f, 0f, 1f };

            //barfill decreased time
            data[2] = battle.HardMode() ? 1.35f : 1f;

            battle.StartCoroutine(battle.DoCommand(commandTime, ActionCommands.TappingKey, data));

            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.instance.playerdata[i].hp > 0)
                {
                    MainManager.instance.playerdata[i].battleentity.spin = new Vector3(0f, 15f);
                }
            }

            while (battle.doingaction)
            {
                for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                {
                    if (MainManager.instance.playerdata[i].hp > 0)
                    {
                        MainManager.instance.playerdata[i].battleentity.animstate = battle.blockcooldown > 0f ? 11 : 24;
                    }
                }
                yield return null;
            }

			bool commandSuccess = battle.barfill >= 1f;

            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
			{
				if (MainManager.instance.playerdata[i].hp > 0)
				{
                    BattleControl.AttackProperty? property = null;

                    if (MainManager.HasCondition(MainManager.BattleCondition.Numb, MainManager.instance.playerdata[i]) == -1)
                    {
                        property = BattleControl.AttackProperty.Numb;
                    }
                    battle.DoDamage(actionid, i, ENERGY_TRANSFER_DAMAGE, property, commandSuccess);
                    MainManager.instance.playerdata[i].battleentity.spin = Vector3.zero;
                }
			}
            battle.DestroyHelpBox();
            UnityEngine.Object.Destroy(spriteBounce);

			entity.rotater.transform.localScale = startScale;

			light.chargeFrame = 15f;
			light.mode = DynamoSporeLight.Mode.Decharging;
			if (battle.enemydata.Length != 1)
			{			
				for (int i = 0; i < battle.enemydata.Length; i++)
				{
					if (i != actionid)
					{
						if (battle.EnemyIsType(i, CardGame.Tribe.Fungi) || battle.enemydata[i].animid == (int)NewEnemies.BatteryShroom)
						{
                            battle.Heal(ref battle.enemydata[i], new int?(Mathf.Clamp(Mathf.CeilToInt((float)battle.enemydata[i].maxhp * 0.2f), 0, 9)), false);
							
							battle.enemydata[i].charge = Mathf.Clamp(battle.enemydata[i].charge + 1, 1, 3);
							battle.StartCoroutine(battle.StatEffect(battle.enemydata[i].battleentity, 4));
							MainManager.PlaySound("StatUp");
						}
						else
						{
                            battle.DoDamage(null, ref battle.enemydata[i], 1, BattleControl.AttackProperty.Numb, null, false);
							if (battle.enemydata[i].hp <= 1)
							{
								battle.enemydata[i].hp = 1;
							}
						}
					}
				}
			}
			yield return EventControl.sec;
			light.ResetData();
			MainManager.DestroyTemp(sporeParticles, 10f);
		}

		IEnumerator DoSparkSpores(EntityControl entity, int actionid)
		{
			battle.nonphyscal = true;

            entity.animstate = (int)MainManager.Animations.Angry;
			MainManager.PlaySound("Charge7");
			ParticleSystem chargeParticles = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/Charging")) as GameObject).GetComponent<ParticleSystem>();
			chargeParticles.transform.position = entity.transform.position + new Vector3(0,1.2f);

			var light = entity.GetComponent<DynamoSporeLight>();
			light.overrideLight = true;

			entity.StartCoroutine(entity.ShakeSprite(0.1f, 45f));

			yield return light.DoChargeUp(15);
			yield return EventControl.quartersec;
			GameObject[] numbProjectiles = new GameObject[4];
			for (int i = 0; i < numbProjectiles.Length; i++)
			{			
				if (MainManager.GetAlivePlayerAmmount() > 0)
				{
					entity.animstate = (int)MainManager.Animations.Jump;
					yield return EventControl.quartersec;
					numbProjectiles[i] = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Objects/EnergySphere"), chargeParticles.transform.position, Quaternion.identity) as GameObject);
					numbProjectiles[i].tag = "NoMapColor";
					MainManager.PlaySound("Lazer2");
					chargeParticles.Stop();
					light.state--;
					if (i < 3)
					{
						battle.GetSingleTarget();
						int playerTargetID = battle.playertargetID;

                        BattleControl.AttackProperty? property = null;

                        if (MainManager.HasCondition(MainManager.BattleCondition.Numb, MainManager.instance.playerdata[playerTargetID]) == -1)
                        {
                            property = BattleControl.AttackProperty.Numb;
                        }

                        battle.StartCoroutine(battle.Projectile
						(
                            SPARK_SPORES_DAMAGE,
                            property,
                            battle.enemydata[actionid],
                            playerTargetID,
                            numbProjectiles[i].transform,
                            40f,//speed
                            (float)UnityEngine.Random.Range(3, 6), //height
                            "SepPart@3@1,keepcolor",
                            "Stars",
                            "PingShot",
                            null,
                            Vector3.zero, //spin
                            false
                        ));
					}
					else
					{
                        battle.playertargetID = -1;
						
						Vector3? freeSpace = null;

						yield return new WaitUntil(() => !battle.summonnewenemy);

						if (battle.enemydata.Length < 3)
                        {
							Vector3[] points = new Vector3[]
							{
								new Vector3(0.5f, 0f, 0f),
								new Vector3(6.55f, 0f, 0f)
							};
							freeSpace = battle.GetFreeSpace(points, 1f, true);
						}

						int targetBuff = -1;
						Vector3 targetPos = freeSpace != null ? freeSpace.Value : Vector3.zero;
                        if (freeSpace == null && battle.enemydata.Length >1)
                        {
							List<int> possibleTargets = new List<int>();
							foreach (var enemy in battle.enemydata)
                            {
								if (enemy.battleentity.battleid != entity.battleid)
									possibleTargets.Add(enemy.battleentity.battleid);
                            }	
							targetBuff = possibleTargets[UnityEngine.Random.Range(0, possibleTargets.Count)];
							targetPos = battle.enemydata[targetBuff].battleentity.transform.position + new Vector3(0,battle.enemydata[targetBuff].battleentity.height);
						}

						float a = 0f;
						float b = 40f;
						Vector3 startPos = numbProjectiles[i].transform.position;
						do
						{
							numbProjectiles[i].transform.position = MainManager.BeizierCurve3(startPos, targetPos, 4f, a/b);
							a += MainManager.TieFramerate(1f);
							yield return null;
						}
						while (a < b);
						UnityEngine.Object.Destroy(numbProjectiles[i]);

                        if (freeSpace == null && battle.enemydata.Length > 1)
                        {
							MainManager.PlaySound("Heal3");
                            battle.StartCoroutine(battle.StatEffect(battle.enemydata[targetBuff].battleentity, 5));
							battle.enemydata[targetBuff].moreturnnextturn++;
                        }
                        else if(freeSpace != null)
                        {
							MainManager.PlaySound("Charge");
							battle.StartCoroutine(battle.SummonEnemy(BattleControl.SummonType.FromGroundKeepScale, (int)NewEnemies.BatteryShroom, targetPos, true));
						}
					}		
					yield return EventControl.quartersec;
					if (i < numbProjectiles.Length - 1)
					{
						entity.animstate = (int)MainManager.Animations.Angry;
						chargeParticles.Play();
						yield return EventControl.halfsec;
					}
				}
			}
			UnityEngine.Object.Destroy(chargeParticles.gameObject);
			while (!MainManager.ArrayIsEmpty(numbProjectiles))
			{
				yield return null;
			}
			yield return new WaitUntil(() => !battle.summonnewenemy);

			light.ResetData();
			yield return null;
        }
	}

}
