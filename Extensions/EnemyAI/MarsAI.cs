using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using UnityEngine;
using static BFPlus.Extensions.BattleControl_Ext;
using static UnityEngine.Object;
using static UnityEngine.ParticleSystem;
namespace BFPlus.Extensions.EnemyAI
{
    public class MarsAI : AI
    {
        enum Attacks
        {
            SummonBud,
            VineBarrage,
            FlowerettiAttack,
            HugeSeed,
            VineAttack
        }
        BattleControl battle = null;
        int VINE_BARRAGE_DAMAGE = 8;
        int SIMPLE_VINE_DAMAGE = 10;
        int HUGE_SEED_DAMAGE = 9;
        int FLOWERETTI_HEAL = 5;
        public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
        {
            battle = MainManager.battle;

            float hpPercentMars = battle.HPPercent(battle.enemydata[actionid]);
            if (battle.enemydata[actionid].data == null)
            {
                battle.enemydata[actionid].data = new int[3];
            }

            List<Attacks> chances = new List<Attacks>() {Attacks.HugeSeed, Attacks.VineAttack, Attacks.VineAttack };

            if (hpPercentMars <= 0.6f)
                chances.Add(Attacks.VineBarrage);
            
            if (battle.enemydata.Length < 3 && battle.enemydata[actionid].data[0] <= 0)
            {
                chances.AddRange(new Attacks[] { Attacks.SummonBud, Attacks.SummonBud });
                if(battle.enemydata.Length == 1)
                {
                    chances = new List<Attacks>() { Attacks.SummonBud };
                }
            }

            if(battle.enemydata[actionid].data[0] > 0)
            {
                battle.enemydata[actionid].data[0]--;
            }
            
            switch (chances[UnityEngine.Random.Range(0, chances.Count)])
            {
                case Attacks.VineBarrage:
                    yield return DoVineBarrage(entity, actionid);
                    break;
                case Attacks.SummonBud:

                    int summonAmount = 1;
                    if(hpPercentMars <= 0.5f)
                    {
                        summonAmount = 3 - battle.enemydata.Length;
                    }
                    yield return SummonBud(entity, actionid, summonAmount);
                    battle.enemydata[actionid].data[0] = 2;
                    break;

                case Attacks.HugeSeed:
                    yield return DoHugeSeed(entity, actionid);
                    break;

                case Attacks.VineAttack:
                    entity.animstate = 100;
                    yield return EventControl.halfsec;
                    yield return battle.VineAttack(SIMPLE_VINE_DAMAGE, actionid, true);
                    break;
            }

            if (MainManager.GetAlivePlayerAmmount() > 0)
            {
                if (battle.enemydata[actionid].data[1] <= 0)
                {
                    if (UnityEngine.Random.Range(0,100) < 40 + battle.enemydata[actionid].data[2])
                    {
                        BattleControl.SetDefaultCamera();
                        yield return DoFlowerettiAttack(entity, actionid);
                        battle.enemydata[actionid].data[2] = 0;
                        battle.enemydata[actionid].data[1] = 3;
                        yield break;
                    }
                    battle.enemydata[actionid].data[2] += 10;
                }
                else
                {
                    battle.enemydata[actionid].data[1]--;
                }

            }
        }

        //101 = getting in bud, simple
        //102 = still closed bud
        //103 = getting out of bud, simple
        //104 = shoot bud
        //105 = bud charge up
        IEnumerator SummonBud(EntityControl entity, int actionid, int amount)
        {
            BattleControl.SetDefaultCamera();

            entity.animstate = 101;
            yield return EventControl.halfsec;

            for(int i=0;i<amount; i++)
            {
                MainManager.PlaySound("Charge", -1, 0.8f, 1f);
                entity.animstate = 105;
                yield return EventControl.sec;

                entity.animstate = 104;

                Vector3? freeSpace = battle.GetFreeSpace(new Vector3[] { new Vector3(0.6f, 0f, 0.35f), new Vector3(5.4f, 0f, -0.3f) }, true);

                GameObject head = entity.extras[1];
                MainManager.PlaySound("PingShot");

                SpriteRenderer seed = MainManager.NewSpriteObject(head.transform.position + Vector3.right*0.1f, null, MainManager.itemsprites[0, 23]);
                seed.material.color = new Color(0.63f, 0.129f, 0.129f);
                float a = 0f;
                float b = 40f;

                do
                {
                    seed.transform.position = MainManager.BeizierCurve3(head.transform.position, freeSpace.Value, 10f, a / b);
                    seed.transform.eulerAngles += new Vector3(0,0,20) * MainManager.TieFramerate(1f);
                    a += MainManager.framestep;
                    yield return null;
                }
                while (a < b + 1f);
                var flowerPart = Instantiate(Resources.Load("Prefabs/Particles/FlowerImpact")) as GameObject;
                var main = flowerPart.GetComponent<ParticleSystem>().main;
                main.startColor = Color.red;
                flowerPart.transform.position = seed.transform.position;
                Destroy(seed.gameObject);
                Destroy(flowerPart.gameObject,5);

                yield return EventControl.tenthsec;
                MainManager.PlaySound("VenusBudAppear", 0.8f, 1);

                battle.summonnewenemy = true;
                battle.StartCoroutine(battle.SummonEnemy(BattleControl.SummonType.FromGround, (int)NewEnemies.MarsSprout, freeSpace.Value, true));
            }
            yield return EventControl.quartersec;
            entity.animstate = 103;
            yield return new WaitUntil(() => !battle.summonnewenemy);
            yield return EventControl.halfsec;
        }

        IEnumerator DoHugeSeed(EntityControl entity, int actionid)
        {
            battle.nonphyscal = true;
            BattleControl.SetDefaultCamera();
            battle.GetSingleTarget();

            entity.animstate = 101;
            yield return EventControl.halfsec;

            MainManager.SetCamera(entity.transform, null, 0.075f, MainManager.defaultcamoffset + new Vector3(0f, 1f, 0f));
            MainManager.PlaySound("Charge", -1, 0.8f, 1f);
            entity.animstate = 105;

            yield return new WaitForSeconds(2f);
            BattleControl.SetDefaultCamera();
            entity.animstate = 104;
            GameObject head = entity.extras[1];
            MainManager.PlaySound("PingShot");

            SpriteRenderer seed = MainManager.NewSpriteObject(head.transform.position + Vector3.right * 0.1f, null, MainManager.itemsprites[0, 23]);
            seed.material.color = new Color(0.63f, 0.129f, 0.129f);
            seed.transform.localScale = Vector3.one * 2f;
            float a = 0f;
            float b = 45f;
            do
            {
                seed.transform.position = MainManager.BeizierCurve3(head.transform.position, battle.playertargetentity.transform.position, 10f, a / b);
                seed.transform.eulerAngles += new Vector3(0, 0, 20) * MainManager.TieFramerate(1f);
                a += MainManager.framestep;
                yield return null;
            }
            while (a < b + 1f);
            var flowerPart = Instantiate(Resources.Load("Prefabs/Particles/FlowerImpact")) as GameObject;
            var main = flowerPart.GetComponent<ParticleSystem>().main;
            main.startColor = Color.red;
            flowerPart.transform.position = seed.transform.position;
            MainManager.PlaySound("Explosion2", 0.8f, 1);
            Destroy(seed.gameObject);
            Destroy(flowerPart.gameObject, 5);

            battle.DoDamage(actionid, battle.playertargetID, HUGE_SEED_DAMAGE, BattleControl.AttackProperty.DefDownOnBlock, battle.commandsuccess);
            yield return EventControl.quartersec;
            entity.animstate = 103;
            yield return EventControl.halfsec;
        }
    

        IEnumerator DoVineBarrage(EntityControl entity, int actionid)
        {
            battle.nonphyscal = true;
            int vineAmount = 10;
            GameObject[] vines = new GameObject[vineAmount];
            Vector3 startPoint = new Vector3(entity.transform.position.x -2, -6, 0.5f);
            Vector3 endPoint = new Vector3(-10, startPoint.y, startPoint.z);

            Vector3 direction = (endPoint - startPoint).normalized;
            float distance = Vector3.Distance(startPoint, endPoint);

            float step = distance / (vineAmount - 1);

            for (int i = 0; i < vineAmount; i++)
            {
                Vector3 position = startPoint + direction * step * i;
                vines[i] = Instantiate(Resources.Load("Prefabs/Objects/VenusRoot"), position, Quaternion.Euler(-90, 0, 90)) as GameObject;

                foreach (var sr in vines[i].GetComponentsInChildren<SpriteRenderer>())
                {
                    sr.material.color = Color.red;
                }
            }
            yield return null;

            entity.animstate = 100;

            yield return EventControl.tenthsec;
            MainManager.PlaySound("Slash3",0.8f,1);
            yield return EventControl.tenthsec;

            MainManager.PlaySound("Rumble3");
            MainManager.ShakeScreen(new Vector3(0.1f, 0.05f, 0f), 0.75f, true);

            yield return EventControl.quartersec;

            bool[] damaged = new bool[MainManager.instance.playerdata.Length];
            for (int i = 0; i < vines.Length; i++)
            {
                yield return EventControl.tenthsec;
                vines[i].transform.localScale *= 1.65f;
                MainManager.PlaySound("Charge8",0.5f);
                MainManager.ShakeScreen(0.2f, 0.65f, true);
                battle.StartCoroutine(LerpVine(vines[i], vines[i].transform.position, damaged, actionid));
                yield return EventControl.tenthsec;
            }

            MainManager.ShakeScreen(new Vector3(0.1f, 0.05f, 0f), 0.5f, true);
            yield return EventControl.tenthsec;
        }

        IEnumerator LerpVine(GameObject vine, Vector3 startPos, bool[] damaged, int actionid)
        {
            float a = 0;
            float b = 20;

            bool checkedDamage = false;
            do
            {
                vine.transform.position = Vector3.Lerp(startPos, startPos + Vector3.up * 5f, a / b);

                if(a > b / 2 && !checkedDamage)
                {
                    checkedDamage = true;
                    for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                    {
                        if (!damaged[i])
                        {
                            EntityControl player = MainManager.instance.playerdata[i].battleentity;
                            if (vine.transform.position.x - player.transform.position.x < 1f && MainManager.instance.playerdata[i].hp > 0)
                            {
                                battle.DoDamage(actionid, i, VINE_BARRAGE_DAMAGE, null, battle.commandsuccess);
                                damaged[i] = true;
                            }
                        }
                    }
                }


                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < b);

            yield return EventControl.tenthsec;

            a = 0f;
            Vector3 localScale = vine.transform.localScale;
            do
            {
                vine.transform.localScale = Vector3.Lerp(localScale, Vector3.zero, a / 20f);
                a += MainManager.TieFramerate(1f);
                yield return null;
            }
            while (a < 20f);

            Destroy(vine);
        }

       
        IEnumerator DoFlowerettiAttack(EntityControl entity, int actionid)
        {
            entity.animstate = 101; //covers in bud

            yield return EventControl.halfsec;
            MainManager.PlaySound("Charge", -1, 0.8f, 1f);
            entity.animstate = 105; //bud charges up
            entity.ShakeSprite(0.2f, 60f);
            yield return EventControl.sec;

            entity.animstate = 106;
            MainManager.PlaySound("PingShot");
            GameObject head = entity.extras[1];

            int seedAmount = 4;

            SpriteRenderer[] seeds = new SpriteRenderer[seedAmount];

            void action(Vector3 startPos, Vector3 endPos, Transform obj, float a, float b)
            {
                obj.position = MainManager.BeizierCurve3(head.transform.position, endPos, 20f, a / b);
                obj.eulerAngles += new Vector3(0, 0, 20) * a;
            }

            for (int i = 0; i < seedAmount; i++)
            {
                seeds[i] = MainManager.NewSpriteObject(head.transform.position + (i+1) *Vector3.right*1f, null, MainManager.itemsprites[0, 23]);
                seeds[i].material.color = new Color(0.63f, 0.129f, 0.129f);

                Vector3 endPos = new Vector3(-3 + (i+1) * 3, head.transform.position.y+10, head.transform.position.z);
                battle.StartCoroutine(LerpStuff(60f, seeds[i].transform.position, endPos, seeds[i].transform, action));
            }

            yield return EventControl.halfsec;

            MainManager.PlaySound("Explosion2", 0.8f, 1);

            GameObject floweretti = Instantiate(Resources.Load("Prefabs/Particles/Floweretti")) as GameObject;
            floweretti.transform.position = new Vector3(4, 8, -0.1f);

            ParticleSystem ps = floweretti.GetComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = new MinMaxGradient(Color.red, new Color(1, 0.92f, 0.59f));
            main.loop = false;
            main.duration = 2.5f;
            main.simulationSpeed= 1.25f;

            var em = ps.emission;
            em.rateOverTimeMultiplier = 50;

            yield return EventControl.sec;
            MainManager.PlaySound("StatUp");
            MainManager.PlaySound("Heal");
            for (int i = 0; i < battle.enemydata.Length; i++)
            {
                MainManager.SetCondition(MainManager.BattleCondition.AttackUp, ref battle.enemydata[i], 3);
                MainManager.SetCondition(MainManager.BattleCondition.DefenseUp, ref battle.enemydata[i], 3);
                battle.StartCoroutine(battle.StatEffect(battle.enemydata[i].battleentity, 1));
                battle.StartCoroutine(battle.StatEffect(battle.enemydata[i].battleentity, 0));
                battle.Heal(ref battle.enemydata[i], FLOWERETTI_HEAL, true);
            }

            MainManager.PlaySound("StatDown");
            for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                MainManager.SetCondition(MainManager.BattleCondition.AttackDown, ref MainManager.instance.playerdata[i],3);
                MainManager.SetCondition(MainManager.BattleCondition.DefenseDown, ref MainManager.instance.playerdata[i], 3);
                battle.StartCoroutine(battle.StatEffect(MainManager.instance.playerdata[i].battleentity, 2));
                battle.StartCoroutine(battle.StatEffect(MainManager.instance.playerdata[i].battleentity, 3));
            }

            Destroy(floweretti, 5);
            for (int i = 0; i < seeds.Length; i++)
            {
                Destroy(seeds[i].gameObject);
            }
        }


    }
}
