using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using static BattleControl;
using static BFPlus.Extensions.IronSuit;
using static MainManager;

namespace BFPlus.Extensions.EnemyAI
{
    public class IronSuitAI : AI
    {
        enum Attacks
        {
            TwoPairs,
            ChaosCast,
            HeartRain,
            ClubHits,
            SpadeCloud,
            DiamondSpikes
        }
        BattleControl battle;
        const int SLOW_SWING_DMG = 4;
        const int FAST_SWING_DMG = 4;
        const int HEART_RAIN_DMG = 4;
        const int DIAMOND_SPIKE_DMG = 6;
        const int SPADE_CLOUD_DMG = 5;
        const int CLUB_HIT_DMG = 3;
        const int CHAOS_CAST_DMG = 3;
        public override IEnumerator DoBattleAI(EntityControl entity, int actionid)
        {
            battle = MainManager.battle;
            IronSuit component = entity.GetComponent<IronSuit>();
            Vector3 basePos = entity.transform.position;
            bool baseFlip = entity.flip;
            battle.SetData(actionid, 3);
            float hpPercent = battle.HPPercent(battle.enemydata[actionid]);

            if (hpPercent <= 0.6f && battle.enemydata[actionid].data[2] == 0)
            {
                entity.animstate = 101;
                MainManager.PlaySound("Charge7", -1, 1.2f, 1f);
                battle.StartCoroutine(entity.ShakeSprite(0.2f, 30f));
                yield return EventControl.halfsec;
                battle.enemydata[actionid].moves = 2;
                battle.enemydata[actionid].cantmove--;
                battle.StartCoroutine(battle.StatEffect(entity, 5));
                MainManager.PlaySound("StatUp");
                battle.enemydata[actionid].data[2] = 1;
                yield return EventControl.halfsec;
                entity.animstate = 0;
            }

            Dictionary<Attacks, int> attacks = new Dictionary<Attacks, int>()
            {
                { Attacks.TwoPairs, 50}
            };

            int baseOdds = 50;
            switch (component.currentSuit)
            {
                case Suit.Heart:
                    attacks.Add(Attacks.HeartRain, baseOdds);
                    break;
                case Suit.Spade:
                    attacks.Add(Attacks.SpadeCloud, baseOdds);
                    break;
                case Suit.Diamond:
                    attacks.Add(Attacks.DiamondSpikes, baseOdds);
                    break;
                case Suit.Club:
                    attacks.Add(Attacks.ClubHits, baseOdds);
                    break;
            }

            Attacks attack = MainManager_Ext.GetWeightedResult(attacks);

            switch (attack)
            {
                case Attacks.TwoPairs:
                    yield return DoTwoPair(entity, actionid, component);
                    break;
                case Attacks.ClubHits:
                    yield return DoClubHits(entity, actionid);
                    break;
                case Attacks.HeartRain:
                    yield return DoHeartRain(entity, actionid);
                    break;
                case Attacks.SpadeCloud:
                    yield return DoSpadeCloud(entity, actionid);
                    break;
                case Attacks.DiamondSpikes:
                    yield return DoDiamondSpikes(entity, actionid);
                    break;
            }

            entity.MoveTowards(basePos, 2);
            yield return null;
            yield return new WaitUntil(() => !entity.forcemove);
            entity.flip = baseFlip;

            if (battle.enemydata[actionid].data[0] == 0 && battle.enemydata[actionid].data[1] != battle.turns && MainManager.GetAlivePlayerAmmount() > 0)
            {
                battle.enemydata[actionid].data[0] = 1;
                battle.enemydata[actionid].data[1] = battle.turns;
                yield return DoChaosCast(entity, actionid, component);
            }
            else
            {
                battle.enemydata[actionid].data[0] = 0;
            }

            //yield return ChangeForm(entity, component, actionid);
        }

        IEnumerator DoChaosCast(EntityControl entity, int actionid, IronSuit component)
        {
            entity.animstate = 107;
            MainManager.PlaySound("CardSound2", 1.2f, 1);
            entity.spin = new Vector3(0, 10);
            Vector3 targetPoint = entity.transform.position + new Vector3(0f, 4.8f, -0.1f);

            GameObject sphere = MainManager.PlayParticle("SuitSphere", targetPoint, -1);
            sphere.transform.localScale = Vector3.one * 3;
            foreach (Transform c in sphere.transform)
                c.localScale = Vector3.one * 3;

            yield return EventControl.sec;
            MainManager.PlaySound("ClothAttack", 1.1f, 1);
            entity.spin = Vector3.zero;

            battle.GetSingleTarget();
            yield return MainManager.ArcMovement(sphere, sphere.transform.position, battle.playertargetentity.transform.position + Vector3.up * 15, new Vector3(0, 0, 20), 10, 35, false);
            battle.AddDelayedProjectile(sphere, battle.playertargetID, CHAOS_CAST_DMG, 2, 0, null, 45, battle.enemydata[actionid], null, "SuitHit", "Fall2");
            DelayedProjExtra.AddDelayedProjExtra(sphere, new int[] { (int)component.currentSuit }, DoChaosCastEffect);

            entity.animstate = 0;
            yield return EventControl.quartersec;
        }

        void DoChaosCastEffect(int projId, int[] data, int damageDone)
        {
            Console.WriteLine("in do chaos cast effect");
            BattleControl.DelayedProjectileData projData = battle.delprojs[projId];
            int playerId = battle.partypointer[projData.position];
            IronSuit.Suit suit = (IronSuit.Suit)data[0];
            if (MainManager.instance.playerdata[playerId].hp > 0)
            {
                if (!battle.commandsuccess || suit == Suit.Spade)
                {
                    int turns = 4;

                    if (battle.commandsuccess)
                    {
                        turns--;
                        if (battle.GetSuperBlock(0))
                            turns--;
                    }

                    if (turns > 1)
                    {
                        Vector3 targetPoint = MainManager.instance.playerdata[playerId].battleentity.transform.position + Vector3.up;
                        BattleControl_Ext.Instance.ApplyStatus(BattleCondition.Inked, ref MainManager.instance.playerdata[playerId], turns, "WaterSplash2", 0.8f, 1, "InkGet", targetPoint, Vector3.one);
                        BattleControl_Ext.Instance.ApplyStatus(BattleCondition.Sticky, ref MainManager.instance.playerdata[playerId], turns, "AhoneynationSpit", 1, 1, "StickyGet", targetPoint, Vector3.one);
                    }
                }

                if (suit == Suit.Heart)
                {
                    DoHeartLifesteal(damageDone, projData.calledby.battleentity, projData.calledby.battleentity.battleid, suit);
                }
            }
        }

        IEnumerator DoClubHits(EntityControl entity, int actionid)
        {
            battle.nonphyscal = true;
            Sprite[] suitParticle = MainManager_Ext.assetBundle.LoadAssetWithSubAssets<Sprite>("IronSuitParticles");
            entity.animstate = 103;
            yield return EventControl.halfsec;
            MainManager.PlaySound("Slash2", 1.2f, 1);

            SpriteRenderer[] clubs = new SpriteRenderer[6];
            bool[] clubTypes = new bool[clubs.Length];

            float arcAngle = 145;
            float startAngle = -arcAngle / 2f;
            float angleStep = (arcAngle / (clubs.Length - 1));
            Vector3 targetPoint = entity.transform.position + new Vector3(-1, 2.7f, -0.1f);
            Vector3 startPoint = entity.transform.position + new Vector3(1, 2f, -0.1f);
            float radius = 2.2f;

            for (int i = 0; i < clubs.Length; i++)
            {
                float angle = startAngle + (angleStep * i);
                float radian = angle * Mathf.Deg2Rad;
                Vector3 spawnPosition = targetPoint + new Vector3(-Mathf.Cos(radian) * radius, Mathf.Sin(radian) * radius, 0);

                clubTypes[i] = UnityEngine.Random.Range(0, 2) == 0;
                clubs[i] = new GameObject().AddComponent<SpriteRenderer>();

                clubs[i].sprite = suitParticle[3];
                clubs[i].transform.localScale = Vector3.one * 1f;
                clubs[i].transform.localEulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));
                SpinAround spin = clubs[i].gameObject.AddComponent<SpinAround>();
                if (clubTypes[i])
                {
                    clubs[i].transform.localEulerAngles = new Vector3(0, 0, -90);
                    spin.itself = new Vector3(0, 0, 10);
                    clubs[i].color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0);
                }
                else
                {
                    clubs[i].transform.localEulerAngles = new Vector3(0, 0, 90);
                    spin.itself = new Vector3(0, 0, -5);
                    clubs[i].color = new Color(Color.white.r, Color.white.g, Color.white.b, 0);
                }
                battle.StartCoroutine(BattleControl_Ext.LerpPosition(15f, startPoint, spawnPosition, clubs[i].transform));
                battle.StartCoroutine(MainManager_Ext.LerpSpriteColor(clubs[i], 30f, new Color(clubs[i].color.r, clubs[i].color.g, clubs[i].color.b, 1)));
            }
            yield return EventControl.sec;

            entity.animstate = 0;
            List<int> clubThrown = Enumerable.Range(0, clubs.Length).ToList();
            int clubsAmount = clubs.Length;
            for (int i = 0; i < clubsAmount; i++)
            {

                if (MainManager.GetAlivePlayerAmmount() == 0)
                    break;

                int index = clubThrown[UnityEngine.Random.Range(0, clubThrown.Count)];
                Transform club = clubs[index].transform;
                battle.GetSingleTarget();

                yield return EventControl.tenthsec;
                UnityEngine.Object.Destroy(clubs[index].gameObject.GetComponent<SpinAround>());
                AttackProperty property = AttackProperty.Sleep;
                float speed = 35;
                float flip = -90;
                club.rotation = Quaternion.identity;

                if (clubTypes[index])
                {
                    flip = 90;
                    property = AttackProperty.Numb;
                    speed = 25;
                    club.localEulerAngles = new Vector3(0, 0, 305);
                }
                Vector3 direction = battle.playertargetentity.transform.position - club.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + flip;
                club.rotation = Quaternion.Euler(0, 0, angle);

                battle.StartCoroutine(battle.Projectile(CLUB_HIT_DMG, property, battle.enemydata[actionid], battle.playertargetID, club, speed, "keepcolor", "ClubHit", null, null, Vector3.zero, false));
                yield return EventControl.quartersec;
                clubThrown.Remove(index);
                yield return new WaitUntil(() => club == null);
            }

            foreach (var club in clubs)
            {
                if (club != null)
                    UnityEngine.Object.Destroy(club.gameObject);
            }
        }

        IEnumerator DoSpadeCloud(EntityControl entity, int actionid)
        {
            battle.nonphyscal = true;
            entity.animstate = 101;
            MainManager.PlaySound("CardSound2", 1.2f, 1);
            Sprite[] suitParticle = MainManager_Ext.assetBundle.LoadAssetWithSubAssets<Sprite>("IronSuitParticles");

            SpriteRenderer[] spades = new SpriteRenderer[5];
            Vector3 targetPoint = entity.transform.position + new Vector3(1.2f, 4.6f, -0.1f);
            float radius = 1f;

            for (int i = 0; i < spades.Length; i++)
            {
                spades[i] = new GameObject().AddComponent<SpriteRenderer>();
                spades[i].sprite = suitParticle[1];
                spades[i].transform.localScale = Vector3.one * 1.2f;
                spades[i].color = new Color(0.3647f, 0.01568f, 0.4823f, 0);
                float angle = i * 360f / spades.Length;
                Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * radius;
                spades[i].transform.position = targetPoint + offset;

                Vector3 directionToCenter = targetPoint - spades[i].transform.position;
                float rotationAngle = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg + 90f;
                spades[i].transform.rotation = Quaternion.Euler(0, 0, rotationAngle);

                battle.StartCoroutine(MainManager_Ext.LerpSpriteColor(spades[i], 30f, new Color(spades[i].color.r, spades[i].color.g, spades[i].color.b, 1)));
            }

            battle.StartCoroutine(DoSpadeRotation(spades, 5, 0.5f, 1, 5, targetPoint, Vector3.forward, 120, 0.8f, 1.2f));
            yield return EventControl.sec;
            yield return EventControl.halfsec;
            for (int i = 0; i < spades.Length; i++)
            {
                battle.StartCoroutine(MainManager_Ext.LerpSpriteColor(spades[i], 30f, new Color(spades[i].color.r, spades[i].color.g, spades[i].color.b, 0)));
            }
            yield return EventControl.halfsec;
            yield return EventControl.tenthsec;

            radius = 5;
            targetPoint = battle.partymiddle + Vector3.up;
            for (int i = 0; i < spades.Length; i++)
            {
                float angle = i * 360f / spades.Length;

                float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
                float z = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

                spades[i].transform.position = new Vector3(targetPoint.x + x, targetPoint.y, targetPoint.z + z);
                battle.StartCoroutine(MainManager_Ext.LerpSpriteColor(spades[i], 30f, new Color(spades[i].color.r, spades[i].color.g, spades[i].color.b, 1)));
            }

            yield return DoSpadeRotation(spades, 5, radius, 1, 0.5f, targetPoint, Vector3.up, 120, 1.2f, 0.5f);

            entity.animstate = 102;
            MainManager.PlaySound("ClothAttack", 1.1f, 1);

            //slash3
            battle.PartyDamage(actionid, SPADE_CLOUD_DMG, null, battle.commandsuccess);
            GameObject spadeExplo = MainManager.PlayParticle("SpadeExplosion", battle.partymiddle, 2);
            Color spadeColor = spadeExplo.GetComponent<ParticleSystem>().main.startColor.color;

            GameObject smoke = MainManager.PlayParticle("impactsmoke", battle.partymiddle, 2);
            var main = smoke.GetComponent<ParticleSystem>().main;
            main.startColor = spadeColor;

            MainManager.PlaySound("Explosion4", 0.9f, 1);
            MainManager.PlaySound("Scanner3", 1f, 1);
            for (int i = 0; i < spades.Length; i++)
                UnityEngine.Object.Destroy(spades[i].gameObject);


            if (!battle.commandsuccess)
            {
                int turns = 4;
                if (battle.commandsuccess)
                {
                    turns--;
                    if (battle.GetSuperBlock(0))
                        turns--;
                }
                for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                {
                    battle.TryCondition(ref MainManager.instance.playerdata[i], MainManager.BattleCondition.Poison, turns);
                }

            }
            yield return EventControl.halfsec;
            entity.animstate = 0;
        }

        IEnumerator DoSpadeRotation(SpriteRenderer[] spades, float startSpeed, float startRadius, float targetSpeed, float targetRadius, Vector3 targetPoint, Vector3 axis, float time, float startPitch, float endPitch)
        {
            AudioSource sound = MainManager.PlaySound("Scanner2", -1, startPitch, 1, false);

            float a = 0;
            float b = time;
            do
            {
                float radius = Mathf.Lerp(startRadius, targetRadius, a / b);
                float speed = Mathf.Lerp(startSpeed, targetSpeed, a / b);
                if (sound != null)
                {
                    sound.pitch = Mathf.Lerp(startPitch, endPitch, a / b);
                }
                for (int i = 0; i < spades.Length; i++)
                {
                    Vector3 currentPosition = spades[i].transform.position - targetPoint;
                    Vector3 rotatedPosition = Quaternion.AngleAxis(speed * MainManager.TieFramerate(1f), axis) * currentPosition;
                    spades[i].transform.position = targetPoint + rotatedPosition.normalized * radius;

                    spades[i].transform.LookAt(targetPoint);

                    if (axis == Vector3.up)
                    {
                        spades[i].transform.LookAt(targetPoint);
                        spades[i].transform.rotation = Quaternion.Euler(90, spades[i].transform.eulerAngles.y, 0);
                    }
                    else
                    {
                        float newAngle = Mathf.Atan2(rotatedPosition.y, rotatedPosition.x) * Mathf.Rad2Deg;
                        spades[i].transform.rotation = Quaternion.Euler(0, 0, newAngle - 90);
                    }
                }

                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);
        }

        IEnumerator DoDiamondSpikes(EntityControl entity, int actionid)
        {
            battle.nonphyscal = true;
            MainManager.PlaySound("CardSound2", 0.9f, 1);
            entity.animstate = 104;
            Sprite[] suitParticle = MainManager_Ext.assetBundle.LoadAssetWithSubAssets<Sprite>("IronSuitParticles");
            SpriteRenderer[] diamond = new SpriteRenderer[4];
            Vector3 startPoint = entity.transform.position + new Vector3(1.2f, 2f, -0.1f);
            Vector3 targetPoint = entity.transform.position + new Vector3(-2, 1f, -0.1f);
            yield return EventControl.tenthsec;

            for (int i = 0; i < diamond.Length; i++)
            {
                diamond[i] = new GameObject().AddComponent<SpriteRenderer>();
                diamond[i].sprite = suitParticle[2];
                diamond[i].color = new Color(0.5333f, 0.6235f, 0.8666f, 0);
                battle.StartCoroutine(MainManager_Ext.LerpSpriteColor(diamond[i], 10f, new Color(diamond[i].color.r, diamond[i].color.g, diamond[i].color.b, 1)));
                battle.StartCoroutine(BattleControl_Ext.LerpPosition(30f, startPoint, targetPoint + (new Vector3(0, 1.2f) * i), diamond[i].transform));
                yield return EventControl.tenthsec;
            }

            yield return EventControl.sec;
            battle.GetSingleTarget();
            entity.animstate = 103;
            yield return EventControl.halfsec;
            MainManager.PlaySound("Slash2", 1.2f, 1);

            for (int i = 0; i < diamond.Length; i++)
            {
                MainManager.PlaySound("ChargeDown2", 1.2f, 1);
                targetPoint = new Vector3(diamond[i].transform.position.x, -2, diamond[i].transform.position.z);
                battle.StartCoroutine(BattleControl_Ext.LerpPosition(30f, diamond[i].transform.position, targetPoint, diamond[i].transform));
                yield return EventControl.tenthsec;
            }
            entity.animstate = 0;

            yield return EventControl.halfsec;
            startPoint = diamond[0].transform.position;
            targetPoint = battle.playertargetentity.transform.position;
            Vector3 direction = (targetPoint - startPoint).normalized;
            float distance = Vector3.Distance(startPoint, targetPoint);
            float step = distance / (diamond.Length - 1);

            for (int i = 0; i < diamond.Length; i++)
            {
                diamond[i].transform.position = startPoint + direction * step * i;
                diamond[i].transform.localScale = Vector3.zero;
            }

            for (int i = 0; i < diamond.Length; i++)
            {
                MainManager.PlaySound("Charge8", 1.2f);
                targetPoint = new Vector3(diamond[i].transform.position.x, 0f, diamond[i].transform.position.z - 0.1f);
                battle.StartCoroutine(BattleControl_Ext.LerpPosition(10f, diamond[i].transform.position, targetPoint, diamond[i].transform));
                yield return MainManager.GradualScale(diamond[i].transform, Vector3.one * (1 + i), 10f, false);
            }

            battle.DoDamage(actionid, battle.playertargetID, DIAMOND_SPIKE_DMG, null, battle.commandsuccess);
            MainManager.PlayParticle("DiamondHit", battle.playertargetentity.transform.position + Vector3.up, 3);
            bool block = battle.commandsuccess;
            if (!block)
            {
                battle.TryCondition(ref MainManager.instance.playerdata[battle.playertargetID], MainManager.BattleCondition.Freeze, 4);
            }

            yield return EventControl.halfsec;
            for (int i = diamond.Length - 1; i >= 0; i--)
            {
                battle.StartCoroutine(MainManager.GradualScale(diamond[i].transform, Vector3.zero, 20f, true));
                yield return EventControl.tenthsec;
            }

            if (battle.enemydata.Length == 1 && block)
            {
                yield return EventControl.quartersec;
                MainManager.PlaySound("Charge15", 0.9f, 1);
                MainManager.PlayParticle("impactsmoke", entity.transform.position + new Vector3(-3f, 0f));
                yield return battle.SummonEnemy(BattleControl.SummonType.FromGround, (int)MainManager.Enemies.IceWall, entity.transform.position + new Vector3(-3f, 0f));
                for (int i = 0; i < battle.enemydata.Length; i++)
                {
                    if (battle.enemydata[i].animid == (int)MainManager.Enemies.IceWall)
                    {
                        battle.enemydata[i].diebyitself = true;
                    }
                }
            }
            yield return EventControl.quartersec;
        }

        IEnumerator DoHeartRain(EntityControl entity, int actionid)
        {
            battle.nonphyscal = true;
            entity.animstate = 101;
            MainManager.PlaySound("CardSound2", 1.2f, 1);
            Sprite[] suitParticle = MainManager_Ext.assetBundle.LoadAssetWithSubAssets<Sprite>("IronSuitParticles");

            SpriteRenderer[] hearts = new SpriteRenderer[3];
            Vector3 targetPoint = entity.transform.position + new Vector3(1.2f, 4.6f, -0.1f);
            float radius = 1f;
            float speed = 360f * 2;

            for (int i = 0; i < hearts.Length; i++)
            {
                hearts[i] = new GameObject().AddComponent<SpriteRenderer>();
                hearts[i].sprite = suitParticle[0];
                hearts[i].color = new Color(0.9137f, 0.5333f, 0.2901f, 0);
                float angle = i * 360f / hearts.Length;
                Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * radius;
                hearts[i].transform.position = targetPoint + offset;

                Vector3 directionToCenter = targetPoint - hearts[i].transform.position;
                float rotationAngle = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg + 90f;
                hearts[i].transform.rotation = Quaternion.Euler(0, 0, rotationAngle);

                battle.StartCoroutine(MainManager_Ext.LerpSpriteColor(hearts[i], 30f, new Color(hearts[i].color.r, hearts[i].color.g, hearts[i].color.b, 1)));
            }

            float a = 0;
            float b = 120;
            do
            {
                for (int i = 0; i < hearts.Length; i++)
                {
                    hearts[i].transform.RotateAround(targetPoint, Vector3.forward, speed * Time.deltaTime);
                }
                a += MainManager.TieFramerate(1f);
                yield return null;
            } while (a < b);


            entity.animstate = 102;
            MainManager.PlaySound("ClothAttack", 1.1f, 1);
            //slash3
            targetPoint = battle.partymiddle + Vector3.up * 15;
            for (int i = 0; i < hearts.Length; i++)
            {
                yield return null;
                battle.StartCoroutine(BattleControl_Ext.LerpPosition(30f, hearts[i].transform.position, targetPoint, hearts[i].transform));
                Vector3 directionToCenter = targetPoint - hearts[i].transform.position;
                float rotationAngle = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg + 90f;
                hearts[i].transform.rotation = Quaternion.Euler(0, 0, rotationAngle);
                yield return null;
            }
            yield return EventControl.halfsec;
            entity.animstate = 0;

            yield return EventControl.halfsec;

            for (int i = 0; i < hearts.Length; i++)
            {
                if (MainManager.GetAlivePlayerAmmount() == 0)
                    break;
                battle.GetSingleTarget();
                targetPoint = battle.playertargetentity.transform.position + Vector3.up;

                Vector3 directionToCenter = targetPoint - hearts[i].transform.position;
                float rotationAngle = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg + 90f;
                hearts[i].transform.rotation = Quaternion.Euler(0, 0, rotationAngle);
                MainManager.PlaySound("Fall2");
                yield return EventControl.tenthsec;
                yield return battle.StartCoroutine(BattleControl_Ext.LerpPosition(40f, hearts[i].transform.position, targetPoint, hearts[i].transform));

                UnityEngine.Object.Destroy(hearts[i].gameObject);

                int damageDealt = battle.DoDamage(actionid, battle.playertargetID, HEART_RAIN_DMG, AttackProperty.Fire, battle.commandsuccess);
                DoHeartLifesteal(damageDealt, entity, actionid, IronSuit.Suit.Heart);
                MainManager.PlayParticle("HeartHit", battle.playertargetentity.transform.position + Vector3.up, 3);
                yield return EventControl.quartersec;
            }

            foreach (var heart in hearts)
            {
                if (heart != null)
                    UnityEngine.Object.Destroy(heart.gameObject);
            }
            yield return null;
        }

        IEnumerator DoTwoPair(EntityControl entity, int actionid, IronSuit component)
        {
            battle.GetSingleTarget();
            EntityControl targetEntity = battle.playertargetentity;

            battle.CameraFocusTarget();
            entity.MoveTowards(targetEntity.transform.position + new Vector3(2.5f, 0f, -0.1f), 1f);
            while (entity.forcemove)
            {
                yield return null;
            }

            List<int> attacks = new List<int>() { 0, 1 };
            int attackAmount = attacks.Count;
            bool slowSwingLast = false;
            for (int i = 0; i < attackAmount; i++)
            {
                int attackType = attacks[UnityEngine.Random.Range(0, attacks.Count)];
                attacks.Remove(attackType);
                int turns = 4;
                if (attackType == 0)
                {
                    entity.animstate = 101;
                    entity.StartCoroutine(entity.ShakeSprite(0.1f, 30));
                    yield return EventControl.sec;
                    MainManager.PlaySound("Clash", 1.2f, 1);
                    entity.animstate = 102;

                    DealTwoPairDamage(entity, actionid, component, SLOW_SWING_DMG);
                    switch (component.currentSuit)
                    {
                        case IronSuit.Suit.Heart:
                            if (!battle.commandsuccess)
                            {
                                battle.TryCondition(ref MainManager.instance.playerdata[battle.playertargetID], MainManager.BattleCondition.Fire, turns);
                            }
                            break;

                        case IronSuit.Suit.Diamond:
                            if (!battle.commandsuccess)
                            {
                                battle.TryCondition(ref MainManager.instance.playerdata[battle.playertargetID], MainManager.BattleCondition.Freeze, turns);
                            }
                            break;

                        case IronSuit.Suit.Spade:
                            if (battle.commandsuccess)
                            {
                                turns--;
                                if (battle.GetSuperBlock(0))
                                    turns--;
                            }
                            battle.TryCondition(ref MainManager.instance.playerdata[battle.playertargetID], MainManager.BattleCondition.Poison, turns);
                            break;

                        case IronSuit.Suit.Club:
                            if (i == 1)
                                slowSwingLast = true;
                            battle.StartCoroutine(battle.StatEffect(battle.enemydata[actionid].battleentity, 4));
                            battle.enemydata[actionid].charge = 2;
                            MainManager.PlaySound("StatUp");
                            break;
                    }

                    yield return EventControl.halfsec;
                }
                else
                {
                    yield return EventControl.tenthsec;
                    entity.animstate = 103;
                    yield return EventControl.halfsec;
                    MainManager.PlaySound("Slash2", 1.2f, 1);

                    DealTwoPairDamage(entity, actionid, component, FAST_SWING_DMG);
                    switch (component.currentSuit)
                    {
                        case IronSuit.Suit.Heart:
                            BattleControl_Ext.Instance.ApplyStatus(MainManager.BattleCondition.GradualHP, ref battle.enemydata[actionid], turns, "Heal3", 1, 1,
                                "MagicUp", battle.enemydata[actionid].battleentity.transform.position, Vector3.one);
                            break;

                        case IronSuit.Suit.Diamond:
                            if (!battle.commandsuccess)
                            {
                                battle.StatusEffect(MainManager.instance.playerdata[battle.playertargetID], MainManager.BattleCondition.AttackDown, turns, true);
                            }
                            break;

                        case IronSuit.Suit.Spade:
                            if (battle.commandsuccess)
                            {
                                turns--;
                                if (battle.GetSuperBlock(0))
                                    turns--;
                            }
                            battle.StatusEffect(MainManager.instance.playerdata[battle.playertargetID], MainManager.BattleCondition.DefenseDown, turns, true);
                            break;

                        case IronSuit.Suit.Club:
                            battle.StartCoroutine(battle.StatEffect(battle.enemydata[actionid].battleentity, 5));
                            battle.enemydata[actionid].moreturnnextturn += 1;
                            MainManager.PlaySound("Heal3");
                            break;
                    }

                    yield return EventControl.halfsec;
                }
            }
            BattleControl.SetDefaultCamera();
            if (slowSwingLast)
                battle.dontusecharge = true;
            yield return null;
        }

        void DealTwoPairDamage(EntityControl entity, int actionid, IronSuit component, int baseDamage)
        {
            int damage = GetSuitDamage(baseDamage, component.currentSuit);
            int damageDealt = battle.DoDamage(actionid, battle.playertargetID, damage, null, battle.commandsuccess);
            MainManager.PlayParticle(component.currentSuit.ToString() + "Hit", battle.playertargetentity.transform.position + Vector3.up, 3);
            DoHeartLifesteal(damageDealt, entity, actionid, component.currentSuit);
        }


        int GetSuitDamage(int damage, IronSuit.Suit suit)
        {
            if (suit == IronSuit.Suit.Club)
                damage -= 1;
            if (suit == IronSuit.Suit.Spade)
                damage += 1;
            return damage;
        }

        void DoHeartLifesteal(int damage, EntityControl entity, int actionid, IronSuit.Suit suit)
        {
            if (suit == IronSuit.Suit.Heart && damage > 0)
            {
                damage = Mathf.Clamp(battle.commandsuccess ? damage / 2 : damage, 1, 99);
                battle.Heal(ref battle.enemydata[actionid], damage);
            }
        }

        public static IEnumerator ChangeForm(EntityControl entity, IronSuit component, int actionid)
        {
            IronSuit.Suit newSuit = component.GetNewSuit();

            GameObject particle = MainManager.PlayParticle(newSuit.ToString() + "Particle", entity.transform.position + Vector3.up * 2);
            particle.transform.localScale = Vector3.one * 2;

            entity.animstate = 101;
            entity.spin = new Vector3(0, 20);
            MainManager.PlaySound("Charge14", 0.9f, 1);

            yield return EventControl.halfsec;

            component.currentSuit = newSuit;
            GetFormPassive(actionid, entity, newSuit);
            switch (newSuit)
            {
                case IronSuit.Suit.Diamond:
                    MainManager.PlaySound("Freeze", 0.8f);
                    break;

                case IronSuit.Suit.Spade:
                    MainManager.PlaySound("Poison", 0.8f);
                    break;

                case IronSuit.Suit.Club:
                    MainManager.PlaySound("Shock", 0.8f);
                    break;

                case IronSuit.Suit.Heart:
                    MainManager.PlaySound("Flame", 0.8f);
                    break;
            }
            entity.spin = Vector3.zero;
            yield return EventControl.halfsec;
            UnityEngine.Object.Destroy(particle, 3);
            yield return null;
            entity.animstate = 0;
        }


        static void GetFormPassive(int actionid, EntityControl entity, IronSuit.Suit suit)
        {
            var entity_Ext = Entity_Ext.GetEntity_Ext(entity);
            entity_Ext.GetOldRes(actionid);

            MainManager.battle.enemydata[actionid].def = MainManager.battle.enemydata[actionid].basedef;
            MainManager.RemoveCondition(MainManager.BattleCondition.Sturdy, MainManager.battle.enemydata[actionid]);
            switch (suit)
            {
                case IronSuit.Suit.Diamond:
                    MainManager.battle.enemydata[actionid].def = MainManager.battle.enemydata[actionid].basedef + 2;
                    MainManager.SetCondition(MainManager.BattleCondition.Sturdy, ref MainManager.battle.enemydata[actionid], 2);
                    MainManager.PlayParticle("MagicUp", entity.transform.position);
                    entity.BreakIce();
                    MainManager.RemoveCondition(MainManager.BattleCondition.Freeze, MainManager.battle.enemydata[actionid]);
                    break;

                case IronSuit.Suit.Spade:
                    MainManager.battle.enemydata[actionid].poisonres = 999;
                    MainManager.RemoveCondition(BattleCondition.Poison, MainManager.battle.enemydata[actionid]);
                    break;

                case IronSuit.Suit.Club:
                    MainManager.battle.enemydata[actionid].sleepres = 999;
                    MainManager.battle.enemydata[actionid].numbres = 999;
                    MainManager.RemoveCondition(BattleCondition.Numb, MainManager.battle.enemydata[actionid]);
                    MainManager.RemoveCondition(BattleCondition.Sleep, MainManager.battle.enemydata[actionid]);
                    MainManager.battle.enemydata[actionid].isasleep = false;
                    MainManager.battle.enemydata[actionid].isnumb = false;
                    break;

                case Suit.Heart:
                    MainManager.RemoveCondition(BattleCondition.Fire, MainManager.battle.enemydata[actionid]);
                    break;
            }
        }
    }
}
