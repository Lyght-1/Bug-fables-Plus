using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MainManager;
using static BattleControl;
using static BFPlus.Extensions.BattleControl_Ext;
using static UnityEngine.Object;
using BFPlus.Extensions.BattleStuff.NewCommands;
using InputIOManager;

namespace BFPlus.Extensions.BattleStuff.Skills
{
    internal class NeedleSurge
    {
        public class SurgeParticle
        {
            public float lifetime;
            public float life = 1f;
            public float scaleFac = 1f;

            public Vector3 vel;

            public SpriteRenderer sprite;

            public static SurgeParticle NewParticle(float lifetime, float scaleFac, Vector3 startPos, Vector3 vel, Color col)
            {
                SurgeParticle particle = new SurgeParticle()
                {
                    lifetime = lifetime,
                    scaleFac = scaleFac,
                    vel = vel,
                    sprite = new GameObject().AddComponent<SpriteRenderer>()
                };
                particle.SetScale();
                particle.sprite.sprite = MainManager.instance.projectilepsrites[4];
                particle.sprite.material.color = col;
                particle.sprite.transform.position = startPos + vel.normalized * particle.sprite.transform.localScale.y * particle.sprite.sprite.texture.height / 500f;

                Vector3 dir = vel.normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
                particle.sprite.transform.localEulerAngles = new Vector3(0, 0, angle);

                return particle;
            }

            public virtual void Update()
            {
                sprite.transform.position += vel * 60 * Time.deltaTime;

                SetScale();

                if (life > 0)
                    life -= 60f * Time.deltaTime / lifetime;
                else
                    Destroy(sprite.gameObject);
            }

            public virtual float SetScale()
            {
                float scaleMult = Mathf.Lerp(1f, 0.3f, life / lifetime);
                scaleMult = 6f * scaleFac * Mathf.Sin(Mathf.PI * scaleMult);
                sprite.transform.localScale = new Vector3(1f, 0.5f, 1f) * scaleMult;
                return scaleMult;
            }
        }

        public static EntityControl user;
        public static EntityControl target;
        public static int targetTrueID;
        public static bool targetAlive;
        public static Color surgeColor;
        public static Color avgSurgeColor;
        public static List<SurgeParticle> surgeParticles = new List<SurgeParticle>();
        public static float surgePower;
        public static int generateParticles = -1;
        public static float particleCounter;

        public static IEnumerator DoNeedleSurge(EntityControl vi)
        {
            battle.dontusecharge = true;

            user = vi;
            target = MainManager.instance.playerdata[battle.target].battleentity;
            targetAlive = MainManager.instance.playerdata[target.battleid].hp > 0;
            targetTrueID = MainManager.instance.playerdata[target.battleid].trueid;

            Vector3 startPos = user.transform.position;
            Vector3 targetPos = target.transform.position;
            Vector3 dir = startPos - targetPos;
            SetCamera(null, targetPos.x < startPos.x ? targetPos : startPos, 0.01f, new Vector3(0.5f * Mathf.Sign(dir.x), 2f, -6f));

            Vector3 viStandPos = targetPos + Vector3.right * Mathf.Sign(dir.x) * 1.3f;
            yield return PrepareToSurge(viStandPos);
            yield return WaitStylish(0.5f);

            PlaySound("Charge3", 8, 0.8f, 0.5f);
            battle.StartCoroutine(GenerateSurgeParticles());
            battle.StartCoroutine(battle.DoCommand(160f, ActionCommands.SequentialKeys, new float[2] { 4f, 1f }));

            yield return WhileActionCommand();
            if (!battle.commandsuccess) 
                yield break;

            StopSound(8);
            user.animstate = 123;
            yield return new WaitForSeconds(0.1f);
            yield return DoNeedleSurgeEffect();

            generateParticles = -1;
            switch (targetTrueID)
            {
                case 0:
                    target.animstate = 115;
                    break;
                default:
                    target.animstate = (int)Animations.WeakBattleIdle;
                    break;
            }
            CleanseAndBuff(ref MainManager.instance.playerdata[user.battleid], ref MainManager.instance.playerdata[target.battleid]);
            StartStylishTimer(0, 10, stylishID: 1, stylishGain: 0.06f);
            yield return new WaitUntil(() => surgePower <= 0);
            yield return WaitStylish(0);

            SetDefaultCamera();
            target.flip = true;
            yield return new WaitUntil(() => generateParticles == 0);

            surgeParticles.Clear();
        }
        static IEnumerator PrepareToSurge(Vector3 viStandPos)
        {
            if (target.battleid == user.battleid)
            {
                user.animstate = 115;
                StartStylishTimer(2, 12, stylishID: 0, stylishGain: 0.05f, commandSuccess: false);
                yield break;
            }

            int turn = targetAlive ? 1 : 0;
            user.MoveTowards(viStandPos);
            float turnThreshold = (viStandPos - user.transform.position).magnitude * 0.1f;
            do
            {
                if (turn == 1 && (viStandPos - user.transform.position).magnitude < turnThreshold)
                {
                    turn = 2;
                    if (targetTrueID == 0)
                        target.FaceTowards(viStandPos);
                    else
                        target.flip = false;
                }
                yield return null;
            }
            while (user.forcemove);

            user.FaceTowards(target.transform.position);
            user.animstate = 115;
            if (targetTrueID == 0)
                target.animstate = (int)Animations.Surprized;
            StartStylishTimer(2, 12, stylishID: 0, stylishGain: 0.05f, commandSuccess: false);
            yield return new WaitForSeconds(0.5f);

            if (targetAlive)
            {
                if (turn == 2)
                    target.FaceTowards(viStandPos);
                switch (targetTrueID)
                {
                    case 0:
                        target.animstate = 115;
                        break;
                    case 1:
                        target.animstate = (int)Animations.Surprized;
                        break;
                    case 2:
                        target.animstate = 102;
                        break;
                }
            }
        }
        static IEnumerator WhileActionCommand()
        {
            bool emote = targetAlive;
            float delay = 40f;
            while (delay > 0)
            {
                surgePower = battle.combo / 4f;
                if (battle.combo > 0)
                {
                    battle.StartCoroutine(user.ShakeSprite(0.1f * Mathf.Pow(surgePower, 2.4f), 1f));
                    yield return null;
                }
                if (battle.doingaction)
                {
                    yield return null;
                    continue;
                }
                if (!battle.commandsuccess)
                {
                    yield return DoFailedSurge();
                    yield break;
                }
                if (emote)
                {
                    emote = false;
                    switch (targetTrueID)
                    {
                        case 1:
                            target.animstate = 105;
                            break;
                        default:
                            target.animstate = (int)Animations.Block;
                            break;
                    }
                }
                delay -= framestep;
                yield return null;
            }
        }
        static IEnumerator DoNeedleSurgeEffect()
        {
            generateParticles = 2;
            PlaySound("Numb", 0.7f, 1);
            ParticleSystem explosion = PlayParticle("ElecFast", battle.CenterPos(MainManager.instance.playerdata[target.battleid], true)).GetComponent<ParticleSystem>();
            if (explosion != null)
            {
                var main = explosion.main;
                main.startColor = avgSurgeColor;
            }

            HealAlly(ref MainManager.instance.playerdata[user.battleid], ref MainManager.instance.playerdata[target.battleid]);
            target.animstate = (int)Animations.Hurt;
            bool charge = true;
            for (float a = 0; a < 70f; a += framestep)
            {
                target.sprite.material.color = new Color(surgeColor.r, surgeColor.g, surgeColor.b, target.sprite.material.color.a);
                if (charge && a >= 35f)
                {
                    charge = false;
                    ChargeUpAlly(ref MainManager.instance.playerdata[user.battleid], ref MainManager.instance.playerdata[target.battleid]);
                    explosion = PlayParticle("ElecFast", battle.CenterPos(MainManager.instance.playerdata[target.battleid], true)).GetComponent<ParticleSystem>();
                    if (explosion != null)
                    {
                        var main = explosion.main;
                        main.startColor = avgSurgeColor;
                    }

                    if (!targetAlive)
                        target.Jump();
                }
                yield return null;
            }
        }
        static IEnumerator DoFailedSurge()
        {
            yield return new WaitForSeconds(0.2f);
            surgePower = 0;
            generateParticles = -1;
            StopSound(9);
            SetDefaultCamera();

            user.animstate = (int)Animations.Surprized;
            if (targetAlive)
            {
                switch (targetTrueID)
                {
                    case 0:
                        target.animstate = (int)Animations.Block;
                        break;
                    default:
                        target.animstate = (int)Animations.Hurt;
                        break;
                }
            }

            ShakeScreen(Vector3.one * 0.1f, 0.25f);
            PlaySound("Explosion", 1.15f, 1f);
            ParticleSystem explosion = PlayParticle("ElecFast", battle.CenterPos(MainManager.instance.playerdata[target.battleid], true)).GetComponent<ParticleSystem>();

            var main = explosion.main;
            main.startColor = avgSurgeColor;

            explosion = PlayParticle("poisoneffect", user.transform.position).GetComponent<ParticleSystem>();
            main = explosion.main;
            main.startColor = Color.Lerp(Color.gray, avgSurgeColor, 0.25f);

            yield return new WaitForSeconds(0.5f);

            if (targetAlive)
            {
                switch (targetTrueID)
                {
                    case 0:
                        target.animstate = (int)Animations.Angry;
                        break;
                    case 1:
                        target.animstate = (int)Animations.Surprized;
                        break;
                    case 2:
                        target.animstate = (int)Animations.Idle;
                        break;
                }
            }
            yield return new WaitForSeconds(1.5f);

            target.flip = true;

            yield return new WaitUntil(() => generateParticles == 0);
        }

        static IEnumerator GenerateSurgeParticles()
        {
            generateParticles = 1;
            battle.StartCoroutine(UpdateSurgeParticles());
            while (generateParticles > 0)
            {
                if (particleCounter > 0)
                {
                    float countdown = framestep * Mathf.Lerp(0.5f, 1f, surgePower);
                    countdown *= (generateParticles == 2) ? 0.5f : UnityEngine.Random.Range(0.5f, 1f);
                    particleCounter -= countdown;
                }
                else
                {
                    particleCounter = 3f;
                    Vector3 startPos;
                    Vector3 vel = UnityEngine.Random.onUnitSphere;
                    if (generateParticles == 2) // sparks around target
                    {
                        startPos = battle.CenterPos(MainManager.instance.playerdata[target.battleid], true);
                    }
                    else // sparks around vi's needles
                    {
                        startPos = battle.CenterPos(MainManager.instance.playerdata[user.battleid], true) + new Vector3(Mathf.Sign(vel.x) * 0.5f, -0.1f);
                        vel = new Vector3(vel.x + Mathf.Sign(vel.x), vel.y, vel.z * 0.2f).normalized;
                        vel *= Mathf.Lerp(0.3f, 1f, Mathf.InverseLerp(-1, 1, Vector3.Dot(vel, new Vector3(0.5f * Mathf.Sign(vel.x), 0.5f, vel.z).normalized)));
                    }
                    vel *= 0.1f;
                    surgeParticles.Add(SurgeParticle.NewParticle(
                        lifetime: 26f * UnityEngine.Random.Range(0.7f, 0.9f) * Mathf.Lerp(0.4f, 1f, surgePower),
                        scaleFac: Mathf.Lerp(0.5f, 1f, surgePower) + UnityEngine.Random.Range(-0.1f, 0.1f),
                        startPos: battle.CenterPos(MainManager.instance.playerdata[user.battleid], true) + new Vector3(Mathf.Sign(vel.x) * 0.5f, -0.1f),
                        vel: vel,
                        col: surgeColor)
                        );
                }
                yield return null;
            }
            while (surgePower > 0)
            {
                surgePower = Mathf.Max(0, surgePower - framestep / 30f);
                yield return null;
            }
            yield return new WaitUntil(() => surgeParticles.Count == 0);
            generateParticles = 0;
        }
        static IEnumerator UpdateSurgeParticles()
        {
            Color col1;
            Color col2;
            if (BadgeIsEquipped((int)BadgeTypes.NumbNeedle))
            {
                col1 = new Color(127 / 255f, 180 / 255f, 1);
                col2 = new Color(127 / 255f, 138 / 255f, 1);
            }
            else
            {
                col1 = new Color(0.6f, 214 / 255f, 0.2f);
                col2 = new Color(0.2f, 214 / 255f, 0.2f);
            }
            col1.a = 1f;
            col2.a = 1f;
            avgSurgeColor = Color.Lerp(col1, col2, 0.5f);

            while (true)
            {
                if (generateParticles == -1 && surgeParticles.Count == 0)
                    yield break;
                surgeColor = Color.Lerp(col1, col2, Mathf.InverseLerp(-1, 1, Mathf.Sin(Mathf.PI * Time.time)));
                for (int p = surgeParticles.Count - 1; p >= 0; p--)
                {
                    if (surgeParticles[p].sprite == null)
                    {
                        surgeParticles[p] = null;
                        surgeParticles.RemoveAt(p);
                    }
                    else
                    {
                        surgeParticles[p].Update();
                    }
                }
                yield return null;
            }
        }

        public static bool HealAlly(ref BattleData attacker, ref BattleData target)
        {
            int HP = BadgeHowManyEquipped((int)Medal.FireNeedles);
            if (HP == 0) 
                return false;

            // heals at least 2 HP
            HP++;
            if (target.hp <= 0)
                battle.RevivePlayer(battle.target, Mathf.Max(2, HP - target.hp), true);
            else 
                battle.Heal(ref target, HP);
            battle.Heal(ref attacker, HP);
            return true;
        }
        public static void ChargeUpAlly(ref BattleData attacker, ref BattleData target, int turnBonus = 0)
        {
            int charge = BadgeHowManyEquipped((int)BadgeTypes.NumbNeedle);
            if (charge > 0) // reflection with sleepy needles
            {
                SetCondition(BattleCondition.Reflection, ref target, charge);
                //MultiReflection(target, charge);
            }
            else // charge by default
            {
                charge++;
                Instance.ChargeUp(ref target, charge, 0.35f);
            }
        }
        public static void CleanseAndBuff(ref BattleData attacker, ref BattleData target, int turnBonus = 0)
        {
            Instance.CureNegativeStatus(ref target);

            int charge;

            // frost shields from debuffs until turn end
            if ((charge = BadgeHowManyEquipped((int)Medal.FrostNeedles)) > 0)
                Instance.ApplyStatus(BattleCondition.Sturdy, ref target, turns: charge + turnBonus, "MagicUp", 1, 1, null, target.battleentity.transform.position, Vector3.one);

            // poison inverts into regen
            if ((charge = BadgeHowManyEquipped((int)BadgeTypes.PoisonNeedle)) > 0)
                Instance.ApplyStatus(BattleCondition.GradualHP, ref target, turns: 1 + charge + turnBonus, "Heal3", 1, 1, null, target.battleentity.transform.position, Vector3.one);
        }
    }
}