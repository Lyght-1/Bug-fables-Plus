using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BFPlus.Extensions;
using System;

public class DynamoSporeLight : MonoBehaviour
{
    public enum LightState
    {
        Off,
        Small,
        Mid,
        High,
        Full
    }

    public enum Mode
    {
        Stay,
        ChargeUp,
        ChargeDown,
        Charging,
        Blinking,
        Decharging
    }

    public LightState state = LightState.Off;
    public Mode mode = Mode.Charging;
    public bool overrideLight = false;
    //public int charge = 0;
    Sprite[] lights;
    float chargeCooldown = 0;
    public float chargeFrame = 30f;
    SpriteRenderer lightSprite;
    EntityControl entity;
    //GameObject[] elecParticles = new GameObject[2];
    int oldAnimState = 0;

    void Start()
    {
        entity = GetComponent<EntityControl>();
        lightSprite = new GameObject("LightLevel").AddComponent<SpriteRenderer>();
        lightSprite.transform.parent = entity.spritetransform;
        //lightSprite.sortingOrder = entity.sprite.sortingOrder + 1;
        lights = MainManager_Ext.assetBundle.LoadAssetWithSubAssets<Sprite>("DynamoSporeLight");
        lightSprite.sprite = lights[0];
        lightSprite.sortingOrder = -1;
    }

    void LateUpdate()
    {
        lightSprite.transform.localPosition = new Vector3(0,0,-0.0001f);
        lightSprite.transform.localScale = Vector3.one;
        lightSprite.material.color = Color.white;

        if (entity.icecube != null)
        {
            mode = Mode.Stay;
        }
        else
        {
            if (oldAnimState != entity.animstate)
            {
                oldAnimState = entity.animstate;
                GetChargeFrame();
            }

            if (HasMaxCharge())
            {
                int charge = MainManager.battle.enemydata[entity.battleid].charge;
                if (charge == 3 && entity.sprite != null && entity.sprite.sprite != null)
                {
                    lightSprite.material.color = MainManager.RainbowColor(1, 5.9f * 1, 0.5f, 1f, 1f);

                    /*if (elecParticles.All(e => e == null))
                    {
                        for(int i=0; i< elecParticles.Length; i++)
                        {
                            elecParticles[i] = MainManager.PlayParticle("Elec", entity.spritetransform.position, -1f);
                            elecParticles[i].transform.parent = entity.spritetransform;
                            elecParticles[i].transform.localPosition = new Vector3(-0.8f + i * 1.5f, 0.5f, -0.02f);
                            elecParticles[i].transform.localScale = Vector3.one * 0.1f;
                            elecParticles[i].transform.GetChild(0).localScale = Vector3.one * 0.15f;
                        }
                    }*/

                    if (!overrideLight && (entity.animstate == (int)MainManager.Animations.Idle || entity.animstate == (int)MainManager.Animations.Sleep))
                    {
                        mode = Mode.Stay;
                        state = LightState.Full;
                    }
                    else
                    {
                        /*foreach (var elec in elecParticles)
                        {
                            if (elec != null)
                                Destroy(elec);
                        }*/
                    }
                }
            }
            else
            {
                /*foreach (var elec in elecParticles)
                {
                    if (elec != null)
                        Destroy(elec);
                }*/
                lightSprite.material.color = Color.white;
            }

            if (mode == Mode.Charging)
            {
                if (chargeCooldown <= 0)
                {
                    chargeCooldown = chargeFrame;
                    state++;

                    if ((int)state >= 4)
                    {
                        state = LightState.Full;
                        mode = Mode.Decharging;
                    }
                }
            }
            else if (mode == Mode.ChargeUp || mode == Mode.ChargeDown)
            {
                int dif = mode == Mode.ChargeUp ? 1 : -1;
                int max = mode == Mode.ChargeUp ? (int)LightState.Full : (int)LightState.Off;
                CheckCharge(mode == Mode.ChargeUp ? (int)state + dif <= max : (int)state + dif >= max, dif);
            }
            else if (mode == Mode.Blinking)
            {
                if (chargeCooldown <= 0)
                {
                    chargeCooldown = chargeFrame;
                    lightSprite.enabled = !lightSprite.enabled;
                }
            }
            else if (mode == Mode.Decharging)
            {
                if (chargeCooldown <= 0)
                {
                    chargeCooldown = chargeFrame;
                    state--;

                    if ((int)state <= 0)
                    {
                        state = LightState.Off;
                        mode = Mode.Charging;
                    }
                }
            }
            chargeCooldown -= 1f;
        }

        if (entity.sprite != null && entity.sprite.sprite != null)
        {
            var spriteId = entity.sprite.sprite.name.Split('_')[1];
            var lightId = lightSprite.sprite.name.Split('_')[1];
            if (spriteId != lightId || !lightSprite.sprite.name.Contains(state.ToString()))
            {
                lightSprite.sprite = lights.Where(a => a.name.Split('_')[1] == spriteId && a.name.Contains(state.ToString())).FirstOrDefault();
            }
        }
    }

    void GetChargeFrame()
    {
        if (!overrideLight)
        {
            switch (entity.animstate)
            {
                case (int)MainManager.Animations.Idle:
                    mode = mode == Mode.Decharging ? Mode.Decharging : Mode.Charging;
                    chargeFrame = 30f;
                    break;
                case (int)MainManager.Animations.Walk:
                    chargeFrame = 22.5f;
                    break;
                case (int)MainManager.Animations.Hurt:
                    chargeFrame = 5f;
                    break;
                case (int)MainManager.Animations.Sleep:
                    chargeFrame = 60f;
                    break;
            }
            chargeCooldown = chargeFrame;
        }
    }

    void CheckCharge(bool condition, int n)
    {
        if (chargeCooldown <= 0)
        {
            chargeCooldown = chargeFrame;
            if (condition)
                state += n;
            else
                mode = Mode.Stay;
        }
    }

    bool HasMaxCharge()
    {
        if (MainManager.instance.inbattle && MainManager.battle != null && MainManager.battle.enemydata != null && entity.battleid != -1 && entity.battleid < MainManager.battle.enemydata.Length)
        {
            return MainManager.battle.enemydata[entity.battleid].charge == 3;
        }
        return false;
    }

    public IEnumerator DoChargeUp(float frame)
    {
        chargeFrame = frame;
        mode = Mode.ChargeUp;
        state = LightState.Off;
        yield return new WaitUntil(() =>state == LightState.Full);
        mode = Mode.Stay;
    }

    public void ResetData()
    {
        chargeFrame = 30f;
        overrideLight = false;
    }
}
