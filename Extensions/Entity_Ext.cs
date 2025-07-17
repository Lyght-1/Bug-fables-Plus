using BFPlus.Extensions.EnemyAI;
using BFPlus.Patches.EntityControlTranspilers;
using HarmonyLib;
using InputIOManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using static BattleControl;
using static MazeGame;
using static PromptAnim;

namespace BFPlus.Extensions
{
    public class Entity_Ext : MonoBehaviour
    {
        public int fireDamage = 0;
        public int sleepScheduleTurns = 1;
        public int asleepTurns = 0;
        public int tauntedBy = -1;
        public bool vitiation = false;
        public int vitiationDmg = 0;
        public const int MAX_VITIATION_DMG = 8;
        public bool sleepScheduled = false;
        public int healedThisTurn = 0;
        public int id = -1;
        public int lastHp = -1;
        public int itemId = -1;
        public bool inkDebuffed = false;
        public bool permanentInkTriggered = false;
        public bool slugskinActive = false;
        public bool smearchargeActive = false;
        public bool overrideDamageAnim = false;
        public int lastTurnHp = -1;
        public SpriteRenderer item;
        Stat[] resStats = new Stat[4];
        Transform iconHolder;
        public EntityControl entity;
        public bool isPlayer = false;
        SpriteRenderer[] modelSprites = null;
        public int[] oldRes = new int[4] {90, 90, 90, 90};
        public GameObject slugskin = null;
        Animator slugAnim = null;
        public bool inkWellActive = false;
        public bool adrenalineUsed = false;
        void Start()
        {
            entity = GetComponent<EntityControl>();
            isPlayer = CompareTag("Player");
            if (entity.model)
            {
                modelSprites = entity.model.GetComponentsInChildren<SpriteRenderer>();
            }
        }

        void Update()
        {
            if (isPlayer)
            {
                if (id != -1 && id < MainManager.instance.playerdata.Length && MainManager.BadgeIsEquipped((int)Medal.LifeLust, id))
                {
                    if (lastHp < MainManager.instance.playerdata[id].hp) /*&& MainManager.battle.currentchoice != BattleControl.Actions.Item*/
                    {
                        healedThisTurn += MainManager.instance.playerdata[id].hp - lastHp;
                    }
                    lastHp = MainManager.instance.playerdata[id].hp;
                }
            }
            else
            {
                if (MainManager.instance.inbattle && entity != null && !entity.dead)
                {
                    if (Time.frameCount % 3 == 0)
                    {
                        BattleControl battle = MainManager.battle;
                        int battleId = entity.battleid;

                        if(battleId < battle.enemydata.Length)
                        {
                            if (MainManager_Ext.showResistance && !battle.hideenemyhp && (MainManager.instance.librarystuff[1, battle.enemydata[battleId].animid] || battle.scopeequipped || battle.HPBarOnOther(battle.enemydata[battleId].animid)))
                            {
                                if (CheckResIcons(battle))
                                {
                                    foreach (var stat in resStats)
                                        stat.CheckStat(battleId);
                                }
                            }

                            if(entity.animid == (int)NewAnimID.IronSuit)
                            {
                                UpdateIronSuitRes(battleId);
                            }
                        }
                    }
                }
            }
        }

        void LateUpdate()
        {
            if (MainManager.instance.inbattle && entity != null && !entity.dead)
            {
                if (slugskin != null)
                {
                    if (slugskinActive && slugskin.transform.localScale.x < 0.01f)
                    {
                        slugAnim.Play("Open");
                    }
                    else if (!slugskinActive && slugskin.transform.localScale.x > 0.99f)
                    {
                        slugAnim.Play("Close");
                    }
                    slugskin.transform.localPosition = slugskin.transform.localScale.magnitude > 0.15f ? new Vector3(0f, 0.9f) + new Vector3(0f, entity.height) : new Vector3(0f, -999f);
                }

                if (item != null)
                {
                    item.enabled = !MainManager.battle.action && entity.deathcoroutine == null;
                    item.transform.position = entity.spritetransform.position + new Vector3(0.6f, 0.5f, -0.1f);
                }

            }
        }

        public void CreateSlugskin()
        {
            if(slugskin == null)
            {
                slugskin = Instantiate(MainManager_Ext.assetBundle.LoadAsset<GameObject>("SlugskinShield"));
                slugskin.transform.parent = entity.rotater.transform;
                slugskin.transform.localScale = Vector3.zero;
                slugskin.transform.localPosition = new Vector3(0f, 0.9f);
                slugskin.transform.localEulerAngles = new Vector3(90, 0, 0);
                slugAnim = slugskin.GetComponent<Animator>();
                slugAnim.Play("StayClose");

                Renderer component = slugskin.GetComponent<Renderer>();
                component.material.color = new Color(1f, 1f, 1f, 0.55f);
                component.material.renderQueue = 2505;

                /*GameObject shieldTemp = Instantiate(Resources.Load("Prefabs/Objects/BubbleShield")) as GameObject;
                Material outline = shieldTemp.GetComponent<MeshRenderer>().materials[1];
                Destroy(shieldTemp);

                MeshRenderer mr = slugskin.GetComponent<MeshRenderer>();
                mr.materials = new Material[] { mr.material, outline };*/


                //StaticModeLAnim sma = slugskin.gameObject.AddComponent<StaticModelAnim>();
            }
        }

        void UpdateIronSuitRes(int battleId)
        {
            MainManager.BattleData ironSuit = MainManager.battle.enemydata[battleId];
            if (ironSuit.poisonres < 999)
                oldRes[0] = ironSuit.poisonres;
            if (ironSuit.freezeres < 999)
                oldRes[1] = ironSuit.freezeres;
            if (ironSuit.numbres < 999)
                oldRes[2] = ironSuit.numbres;
            if (ironSuit.sleepres < 999)
                oldRes[3] = ironSuit.sleepres;
        }

        public void GetOldRes(int battleId)
        {
            MainManager.battle.enemydata[battleId].poisonres= oldRes[0];
            MainManager.battle.enemydata[battleId].freezeres = oldRes[1];
            MainManager.battle.enemydata[battleId].numbres = oldRes[2];
            MainManager.battle.enemydata[battleId].sleepres = oldRes[3];
        }

        public static Entity_Ext GetEntity_Ext(EntityControl entity)
        {
            if (entity.GetComponent<Entity_Ext>() == null)
            {
                return entity.gameObject.AddComponent<Entity_Ext>();
            }
            return entity.GetComponent<Entity_Ext>();
        }

        public void CreateItem(int itemID)
        {
            item = new GameObject("item").AddComponent<SpriteRenderer>();
            item.sprite = MainManager.itemsprites[0, itemID];
            item.material = MainManager.spritemat;
            item.material.renderQueue = 50000;
            item.gameObject.layer = 14;
            item.transform.parent = transform;
            item.transform.position = entity.spritetransform.position + new Vector3(0.6f, 0.5f, -0.1f);
            itemId = itemID;
        }

        bool CheckResIcons(BattleControl battle)
        {
            bool active = false;
            if (!battle.action)
            {
                if (battle.currentaction == Pick.SelectEnemy)
                {
                    if (battle.itemarea == AttackArea.AllEnemies || battle.itemarea == AttackArea.All || (battle.avaliabletargets[battle.option].battleentity != null && battle.avaliabletargets[battle.option].battleentity.battleid == entity.battleid && battle.itemarea == AttackArea.SingleEnemy))
                    {
                        active = true;
                    }
                }
            }
            iconHolder.gameObject.SetActive(active);
            return active;
        }

        public void CreateResIcons()
        {
            if (entity == null)
                entity = GetComponent<EntityControl>();
            var component = entity.hpbar.Find("back").GetComponent<SpriteRenderer>();
            var dropOffset = new Vector3(0.03f, -0.04f);

            iconHolder = new GameObject("iconHolder").transform;
            iconHolder.parent = entity.transform;
            iconHolder.transform.localPosition = Vector3.zero;
            iconHolder.gameObject.SetActive(false);

            CreateIcon(iconHolder, new Vector3(-0.65f, -0.1f), new Vector3(0.35f, 0.35f, 1f), MainManager.guisprites[44], Vector2.one, "poisonIcon", component,dropOffset,0, new Color(0.92f, 0.82f, 0.92f));
            CreateIcon(iconHolder, new Vector3(-0.25f, -0.1f), new Vector3(0.35f, 0.35f, 1f), MainManager.guisprites[43], Vector2.one, "freezeIcon", component, dropOffset,1, Color.white);
            CreateIcon(iconHolder, new Vector3(0.2f, -0.1f), new Vector3(0.3f, 0.3f, 1f), MainManager.guisprites[45], new Vector2(1.2f, 1.2f), "numbIcon", component, dropOffset,2, new Color(0.75f, 0.39f, 0.05f));
            CreateIcon(iconHolder, new Vector3(0.6f, -0.1f), new Vector3(0.28f, 0.28f, 1f), MainManager.guisprites[46], new Vector2(1.2f, 1.2f), "sleepIcon", component, dropOffset,3, new Color(0.68f, 0.85f, 0.90f));
            iconHolder.transform.localScale = Vector3.one * 1.2f;
        }

        void CreateIcon(Transform iconParent, Vector3 pos, Vector3 size, Sprite sprite, Vector3 fontSize, string name, SpriteRenderer component, Vector3 dropOffset, int index, Color color)
        {
            var icon = MainManager.NewUIObject(name, iconParent, pos, size, sprite, 10);
            DynamicFont stat = DynamicFont.SetUp(string.Empty, false, true, 5f, 2, component.sortingOrder + 20, fontSize, icon.transform, Vector3.zero, color);
            stat.dropshadow = true;
            stat.dropoffset = dropOffset;
            string[] resNames = new string[] { "poisonres", "freezeres", "numbres", "sleepres" };
            resStats[index] = new Stat(stat, resNames[index]);
        }

        public void CheckInkDebuff(ref MainManager.BattleData data)
        {
            int addedValue = inkDebuffed ? 25 : -25;

            if (data.battleentity.playerentity && MainManager.BadgeIsEquipped((int)Medal.InkBubble, data.trueid))
            {
                addedValue = inkDebuffed ? 50 : -50;
                addedValue *= -MainManager.BadgeHowManyEquipped((int)Medal.InkBubble, data.trueid);
            }

            if(data.poisonres < 110)
            {
                data.poisonres = Mathf.Clamp(data.poisonres + addedValue, -999, 999);
            }

            if (data.freezeres < 110)
            {
                data.freezeres = Mathf.Clamp(data.freezeres + addedValue, -999, 999);
            }

            if (data.numbres < 110)
            {
                data.numbres = Mathf.Clamp(data.numbres + addedValue, -999, 999);
            }

            if (data.sleepres < 110)
            {
                data.sleepres = Mathf.Clamp(data.sleepres + addedValue, -999, 999);
            }

            for (int i = 0; i < oldRes.Length; i++) 
            {
                if (oldRes[i] < 110)
                {
                    oldRes[i] = Mathf.Clamp(oldRes[i] + addedValue, -999, 999);
                }
            }

            inkDebuffed = !inkDebuffed;
        }

        class Stat
        {
            FieldInfo statRef;
            DynamicFont dynamicFont;
            int lastRes = -1;
            public Stat(DynamicFont stat, string fieldName)
            {
                dynamicFont = stat;
                statRef = AccessTools.Field(typeof(MainManager.BattleData), fieldName);
            }


            public void CheckStat(int battleId)
            {
                if(battleId >= 0 && battleId < MainManager.battle.enemydata.Length)
                {
                    int currentRes = (int)statRef.GetValue(MainManager.battle.enemydata[battleId]);

                    if (currentRes != lastRes && dynamicFont.letters != null)
                    {
                        lastRes = currentRes;
                        dynamicFont.text = Mathf.Clamp(currentRes,0,999).ToString();
                        ChangeAlignement();
                    }
                }
            }

            void ChangeAlignement()
            {
                foreach (var letter in dynamicFont.letters)
                {
                    if (letter != null)
                    {
                        letter.anchor = TextAnchor.MiddleCenter;
                        letter.alignment = TextAlignment.Center;
                    }
                }
            }
        }

        public void UpdateModelSprite()
        {
            if(modelSprites != null && entity != null && entity.sprite != null)
            {
                Color spriteColor = entity.sprite.material.color;

                foreach (var sprite in modelSprites) 
                {
                    sprite.material.color = spriteColor;
                }
            }
        }
    }
}
