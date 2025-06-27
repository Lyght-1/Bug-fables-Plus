using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions
{
    public class OverworldTimer : MonoBehaviour
    {
        protected float clocktime;
        public int timer = 60;
        protected DynamicFont timeUI;
        protected Transform clock;
        public int endEvent = -1;
        public bool start = false;
        bool UIActive = false;
        Vector3 offset = Vector3.zero;
        protected virtual void Start()
        {
            timeUI = DynamicFont.SetUp(true, 20f, 2, 100, Vector2.one * 2f, MainManager.GUICamera.transform, new Vector3(-7f, 3.5f, 10f) + offset);
            timeUI.dropshadow = true;

            clock = MainManager.NewUIObject("clock", MainManager.GUICamera.transform, new Vector3(-8f, 4.1f, 10f) + offset, Vector3.one, MainManager.guisprites[84], 100).transform;
            clock.gameObject.AddComponent<SpriteBounce>().MessageBounce();
            SetUIText();
        }

        protected virtual void SetUIText()
        {
            if(timeUI != null)
                timeUI.text = ":" + timer.ToString().PadLeft(2, '0');
        }

        void Update()
        {
            if(MainManager.instance.inbattle || MainManager.instance.pause || MainManager.instance.minipause)
            {
                if (UIActive)
                {
                    ShowUI(false);
                }
            }
            else
            {
                if (!UIActive)
                {
                    ShowUI(true);
                }

                if (start)
                {
                    if (clocktime > 0f)
                    {
                        clocktime -= MainManager.framestep;
                    }
                    else
                    {
                        timer--;
                        if (timer <= 0)
                        {
                            start = false;
                            if (endEvent != -1)
                            {
                                MainManager.events.StartEvent(endEvent, null);
                            }
                        }
                        clocktime = 60f;
                    }

                    if (timeUI != null)
                    {
                        SetUIText();
                    }
                }
            }

        }

        void ShowUI(bool show)
        {
            timeUI?.gameObject?.SetActive(show);
            clock?.gameObject?.SetActive(show);
            UIActive = show;
        }

        protected virtual void OnDestroy()
        {
            Destroy(timeUI?.gameObject);
            Destroy(clock?.gameObject);
        }

        public static OverworldTimer SetUpTimer(int time, int endEvent, Vector3 offset, Transform parent)
        {
            OverworldTimer timer = new GameObject("timer").AddComponent<OverworldTimer>();
            timer.transform.parent = parent;
            timer.timer = time;
            timer.endEvent = endEvent;
            timer.offset = offset;
            return timer;
        }
    }

    public class SeedlingMinigame : OverworldTimer
    {
        Vector3[][] spawnZones = new Vector3[][]
        {
            new Vector3[]{ new Vector3(-18.7f, 0.1f, -2f), new Vector3(-4f, 0.1f, -13f) },
            new Vector3[]{ new Vector3(3.4f, 0.1f, 1), new Vector3(17.78f, 0.1f, -11.98f) }
        };
        List<NPCControl> seedlings = new List<NPCControl>();
        int seedlingAmount = 20;
        DynamicFont pointsUI;
        protected override void Start()
        {
            base.Start();
            pointsUI = DynamicFont.SetUp(true, 20f, 2, 100, Vector2.one * 2f, MainManager.GUICamera.transform, new Vector3(6.5f, 3.5f, 10f));
            pointsUI.dropshadow = true;

            for (int i = 0; i < seedlingAmount; i++)
                CreateSeedling("seed" + i);

            MainManager.player.canpause = false;
            MainManager.player.candig = false;
            MainManager.instance.flagvar[1] = 0;
            timer = 60;
            endEvent = (int)NewEvents.EndSeedlingGame;
            SetUIText();
        }

        protected override void SetUIText()
        {
            base.SetUIText();
            if(pointsUI != null)
                pointsUI.text = MainManager.instance.flagvar[1].ToString().PadLeft(3, '0');
        }

        void CreateSeedling(string name)
        {
            var zone = spawnZones[UnityEngine.Random.Range(0, spawnZones.Length)];
            var position = new Vector3(UnityEngine.Random.Range(zone[0].x, zone[1].x), 0.4178f, UnityEngine.Random.Range(zone[0].z, zone[1].z));

            int odds = UnityEngine.Random.Range(0, 100);
            int animId;
            float speed = 2f;

            float goldenMultiplier = MainManager.BadgeIsEquipped((int)MainManager.BadgeTypes.Seedling) ? 1.5f : 1;

            if (odds < 10 * goldenMultiplier)
            {
                animId = (int)MainManager.AnimIDs.GoldenSeedling;
                speed = 4.5f;
            }   
            else if(odds >=10 && odds <30)
            {
                animId = (int)MainManager.AnimIDs.Underling;
                speed = 2.5f;
            }
            else
            {
                animId = (int)MainManager.AnimIDs.Seedling;
            }
            NPCControl seedling = EntityControl.CreateNewEntity(name, animId-1, position).gameObject.AddComponent<NPCControl>();

            seedling.behaviors = new NPCControl.ActionBehaviors[] { NPCControl.ActionBehaviors.Wander,NPCControl.ActionBehaviors.WalkAwayFromPlayer };
            seedling.entity = seedling.GetComponent<EntityControl>();
            seedling.entity.npcdata = seedling;
            seedling.entitytype = NPCControl.NPCType.Enemy;
            seedling.transform.parent = MainManager.map.transform;
            seedling.wanderradius = 10;
            seedling.teleportradius = 999;
            seedling.radiuslimit = 50;
            seedling.entity.destroytype = NPCControl.DeathType.Smoke;
            seedling.entity.emoticonoffset = new Vector3(0,2,0);
            seedling.actionfrequency = new float[] { 500,500 };
            seedling.insideid = -1;
            seedling.emoticonflag = new Vector2[10];
            for (int i = 0; i < seedling.emoticonflag.Length; i++)
                seedling.emoticonflag[i] = new Vector2(-1, 0);

            seedling.entity.CreateFeet();
            seedling.entity.speed = speed;
            seedling.radius = 5;

            EntityControl.IgnoreColliders(MainManager.player.entity, seedling.entity, true);
            seedlings.Add(seedling);
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("Enemy") && MainManager_Ext.inSeedlingMinigame && col is CapsuleCollider && !col.isTrigger)
            {
                EntityControl entity = col.GetComponent<EntityControl>();
                int point = 2;
                if (entity.animid == (int)MainManager.AnimIDs.GoldenSeedling-1)
                    point = 10;
                else if (entity.animid == (int)MainManager.AnimIDs.Underling-1)
                    point = 4;

                MainManager.instance.flagvar[1] += point;
                entity.rigid.isKinematic = true;
                entity.rigid.velocity = Vector3.zero;
                MainManager.DeathSmoke(entity.transform.position);
                MainManager.PlaySound("LevelUp");
                Destroy(entity.gameObject);

                CreateSeedling("seed");
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Destroy(pointsUI?.gameObject);
            foreach(var seedling in seedlings)
            {
                if (seedling != null)
                    Destroy(seedling.gameObject);
            }
        }
    }
}
