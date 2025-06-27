using HarmonyLib;
using InputIOManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MainManager;

namespace BFPlus.Extensions
{
    public class GourmetRace : MonoBehaviour
    {
        public enum GourmetItemType
        {
            Bad,
            Good,
            VeryGood,
            Rare
        }

        Vector3[][] spawnZones = new Vector3[][]
        {
            new Vector3[]{ new Vector3(-10.85f, -4f, -13.1f), new Vector3(-2.35f, -4f, -19.54f) },
            new Vector3[]{ new Vector3(-6.85f, -4f, -7.33f), new Vector3(5.58f, -4f, -11.84f) },
            new Vector3[]{ new Vector3(-10.58f, -4f, -13.4f), new Vector3(12.15f, -4f, -15.06f) },
            new Vector3[]{ new Vector3(4.78f, -4, -16.48f), new Vector3(9.76f, -4, -21.98f) },
            new Vector3[]{ new Vector3(-10.85f, 0, -5.76f), new Vector3(-7.59f, 0f, -6.3f) },
            new Vector3[]{ new Vector3(-10.85f, -1f, -7), new Vector3(-7.56f, -1f, -8.3f) },
            new Vector3[]{ new Vector3(-10.85f, -2, -9.14f), new Vector3(-7.71f, -2, -10.21f) },
            new Vector3[]{ new Vector3(-10.85f, -3f, -11f), new Vector3(-7.53f, -3, -12.34f) },
            new Vector3[]{ new Vector3(8.8f, -3f, -11), new Vector3(12.15f, -3f, -12.15f) },
            new Vector3[]{ new Vector3(8.96f, -2f, -9.1f), new Vector3(12.15f, -2, -10.17f) },
            new Vector3[]{ new Vector3(8.81f, -1f, -7.1f), new Vector3(12.15f, -1f, -8.19f) },
            new Vector3[]{ new Vector3(8.79f, 0, -3.94f), new Vector3(12.15f, 0, -6.06f) },
        };

        Vector3[] specialSpots = new Vector3[]
        {
            new Vector3(-9.68f, -3.73f, -22.17f),
            new Vector3(-3.12f, -2f, -5.35f),
            new Vector3(7.39f, -3, -8.75f),
            new Vector3(11.2f, -2.53f, -18f),
            new Vector3(7.23f, -2.73f, -5.88f)
        };


        private Dictionary<GourmetItemType, float> itemRatios = new Dictionary<GourmetItemType, float>
        {
            { GourmetItemType.Bad, 0.5f }, 
            { GourmetItemType.Good, 0.25f }, 
            { GourmetItemType.VeryGood, 0.20f },
            { GourmetItemType.Rare, 0.05f }
        };

        MainManager.Items[][] possibleItems = new MainManager.Items[][]
        {
            new MainManager.Items[]{ MainManager.Items.HoneyDrop, MainManager.Items.Abomihoney, MainManager.Items.HoneyDanger},
            new MainManager.Items[]{ MainManager.Items.GlazedHoney, MainManager.Items.HoneyShroom, MainManager.Items.HoneydLeaf},
            new MainManager.Items[]{ MainManager.Items.HoneyPancake, MainManager.Items.HoneyMilk, MainManager.Items.HoneyIceCream},
            new MainManager.Items[]{MainManager.Items.TangyJam, MainManager.Items.Donut},
        };

        int[] pointsPerItem = new int[] { 1, 3, 5, 10 };
        int[] inputRequired = new int[] { 5, 10, 20, 30 };
        int[] sequentialKeys = new int[] { 3, 5, 8, 10 };

        DynamicFont[] points;
        GameObject[] contestantFaces;
        List<GourmetItem> items = new List<GourmetItem>();
        GourmetContestant[] contestants = new GourmetContestant[2];
        int endEvent = (int)NewEvents.EndGourmetMinigame;
        public bool start = false;
        public const int ITEM_AMOUNT = 35;
        public static int itemEaten = 0;
        void Start()
        {
            points = new DynamicFont[3];
            points[0] = DynamicFont.SetUp(true, 20f, 2, 100, Vector2.one * 2f, MainManager.GUICamera.transform, new Vector3(6.5f, 3.5f, 10f));
            points[1] = DynamicFont.SetUp(true, 20f, 2, 100, Vector2.one, MainManager.GUICamera.transform, new Vector3(7.5f, 2.7f, 10f));
            points[2] = DynamicFont.SetUp(true, 20f, 2, 100, Vector2.one, MainManager.GUICamera.transform, new Vector3(7.5f, 1.8f, 10f));
            contestantFaces = new GameObject[3];
            contestantFaces[0] = MainManager.NewUIObject("zaspIcon", MainManager.GUICamera.transform, new Vector3(7f, 3.1f, 10f), Vector3.one * 0.5f, MainManager_Ext.assetBundle.LoadAsset<Sprite>("mini_zasp"));
            contestantFaces[1] = MainManager.NewUIObject("chubeeIcon", MainManager.GUICamera.transform, new Vector3(7.05f, 2.35f, 10f), Vector3.one * 0.4f, MainManager_Ext.assetBundle.LoadAsset<Sprite>("mini_chubee"));
            contestantFaces[2] = MainManager.NewUIObject("leifIcon", MainManager.GUICamera.transform, new Vector3(6f, 4.2f, 10f), Vector3.one * 0.75f, MainManager.guisprites[173]);


            foreach (var point in points) 
            {
                point.dropshadow = true;
            }

            CreateItems();
            CreateContestants();

            MainManager.player.canpause = false;
            MainManager.instance.flagvar[1] = 0;
            itemEaten = 0;
            UpdateUiText();
        }

        void UpdateUiText()
        {
            points[0].text = MainManager.instance.flagvar[1].ToString().PadLeft(3, '0');
            points[1].text = contestants[0].points.ToString().PadLeft(3, '0');
            points[2].text = contestants[1].points.ToString().PadLeft(3, '0');
        }

        void Update()
        {
            if (start)
            {
                if (itemEaten == ITEM_AMOUNT)
                {
                    start = false;
                    foreach(var contestant in contestants)
                    {
                        contestant.entity.StopForceMove();
                        contestant.isActive = false;
                    }

                    MainManager.events.StartEvent(endEvent, null);
                }

                if (points != null)
                {
                    UpdateUiText();
                }
            }
        }

        public void StartGame()
        {
            start = true;
            for (int i = 0; i < contestants.Length; i++)
            {
                contestants[i].isActive = true;
            }
        }

        void CreateContestants()
        {
            contestants[0] = GourmetContestant.CreateContestant((int)MainManager.AnimIDs.Zasp - 1, new Vector3(-5.13f, -4f, -23.72f), 1.2f, 0.75f, "zasp", (int)MainManager.Animations.ItemGet, new Vector3(-0.5f, 1.5f, -0.1f));
            contestants[1] = GourmetContestant.CreateContestant((int)MainManager.AnimIDs.Chubee - 1, new Vector3(5f, -4f, -23.72f), 0.5f, 2f, "chubee",0, new Vector3(0f, 0.9f, -0.1f));

            for (int i = 0; i < contestants.Length; i++)
            {
                contestants[i].race = this;
            }
        }

        void CreateItems()
        {
            int count = 0;
            int currentTotal = 0;
            items.Clear();

            foreach (var pair in itemRatios)
            {
                GourmetItemType itemType = pair.Key;
                float ratio = pair.Value;
                int itemCount;
                if (count == itemRatios.Count - 1)
                {
                    itemCount = ITEM_AMOUNT - currentTotal;
                }
                else
                {
                    itemCount = Mathf.FloorToInt(ITEM_AMOUNT * ratio);
                    currentTotal += itemCount;
                }

                for (int i = 0; i < itemCount; i++)
                {
                    Vector3 position;
                    int zoneID;
                    if (itemType != GourmetItemType.Rare)
                    {
                        zoneID = UnityEngine.Random.Range(0, 5);

                        if(UnityEngine.Random.Range(0, 10) > 7)
                        {
                            zoneID = UnityEngine.Random.Range(4, spawnZones.Length);
                        }

                        Vector3[] zone = spawnZones[zoneID];
                        position = new Vector3(UnityEngine.Random.Range(zone[0].x, zone[1].x), zone[0].y, UnityEngine.Random.Range(zone[0].z, zone[1].z));
                    }
                    else
                    {
                        zoneID = -UnityEngine.Random.Range(0, specialSpots.Length);
                        position = specialSpots[-zoneID];
                    }

                    bool itemTooClose = false;
                    foreach(var itemObject in items)
                    {
                        if(Vector3.Distance(position, itemObject.transform.position) < 2)
                        {
                            i--;
                            itemTooClose = true;
                            break;
                        }
                    }

                    if (itemTooClose)
                        continue;

                    MainManager.Items item = possibleItems[(int)itemType][UnityEngine.Random.Range(0, possibleItems[(int)itemType].Length)];

                    NPCControl npcItem = EntityControl.CreateItem(position, 0, (int)item, Vector3.zero, -1);
                    npcItem.objecttype = NPCControl.ObjectTypes.None;
                    npcItem.entity.fixedentity = true;
                    npcItem.entity.alwaysactive = true;
                    npcItem.entity.Invoke("SetFixed", 0.1f);
                    GourmetItem gourmetItem = npcItem.gameObject.AddComponent<GourmetItem>();
                    gourmetItem.presses = MainManager.mashcommandalt ? sequentialKeys[(int)itemType] :inputRequired[(int)itemType];
                    gourmetItem.point = pointsPerItem[(int)itemType];
                    gourmetItem.type = itemType;
                    gourmetItem.zoneID = zoneID;
                    items.Add(gourmetItem);
                }
                count++;
            }
        }

        void OnDestroy()
        {
            foreach(var point in points)
                Destroy(point.gameObject);

            foreach (var icon in contestantFaces)
                Destroy(icon);

            foreach (var contestant in contestants)
                Destroy(contestant.gameObject);
        }

        public GourmetItem FindClosestItem(Vector3 pos)
        {
            float closestDistance = float.MaxValue;
            GourmetItem closestItem= null;
            foreach (GourmetItem item in items)
            {
                if(item != null && item.available)
                {
                    float distance = Vector3.Distance(pos, item.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestItem = item;
                    }
                }
            }
            return closestItem;
        }

        public void HideUI()
        {
            foreach (var icon in contestantFaces)
                icon.gameObject.SetActive(false);
            foreach (var point in points)
                point.gameObject.SetActive(false);
        }

        public (int AnimId,int Points)[] GetContestantsPoints()
        {
            int playerPoints = MainManager.instance.flagvar[1];
            var allParticipants = contestants
            .Select(c => (AnimId: c.entity.animid, Points :c.points ))
            .Append((AnimId: 2, Points: playerPoints))
            .OrderBy(p => p.Points)
            .ToArray();
            return allParticipants;
        }
    }

    public class GourmetItem : MonoBehaviour
    {
        public GourmetRace.GourmetItemType type;
        public int presses;
        public int point;
        public bool available = true;
        public int zoneID;
        ButtonSprite[] buttons;
        EntityControl entity;
        void Start()
        {
            entity = GetComponent<EntityControl>(); 
        }


        void OnTriggerEnter(Collider other)
        {
            if (available)
            {
                if (other.CompareTag("Player")&& !MainManager.instance.minipause)
                {
                    available = false;
                    EntityControl.IgnoreColliders(entity, MainManager.player.entity, true);
                    entity.ccol.enabled = false;
                    entity.rigid.useGravity = false;
                    entity.rigid.constraints = RigidbodyConstraints.FreezeAll;
                    transform.parent = MainManager.player.entity.sprite.transform;
                    transform.localPosition = new Vector3(0f, 0.9f, -0.1f);
                    StartCoroutine(StartPlayerConsuming(other.gameObject));
                }

                if (other.CompareTag("NPC"))
                {
                    GourmetContestant contestant = other.GetComponent<GourmetContestant>();
                    available = false;
                    EntityControl.IgnoreColliders(entity, contestant.entity, true);
                    contestant.isActive = false;
                    entity.ccol.enabled = false;
                    transform.parent = contestant.entity.sprite.transform;
                    StartCoroutine(StartContestantConsuming(contestant));
                }
            }
        }

        IEnumerator StartContestantConsuming(GourmetContestant eater)
        {
            eater.entity.StopForceMove();
            transform.localPosition = eater.itemPos;
            float a = 0;
            float b = presses * 20f / eater.eatingSpeed;
            eater.entity.animstate = eater.eatAnim;
            bool isZasp = eater.entity.animid == (int)MainManager.AnimIDs.Zasp - 1;
            do 
            {
                if (!eater.entity.sound.isPlaying)
                {
                    eater.entity.PlaySound("Eat");
                }
                a += MainManager.TieFramerate(1f);

                if (GourmetRace.itemEaten == GourmetRace.ITEM_AMOUNT - 1)
                    break;

                yield return null;
            } while (a<b);
            yield return null;
            eater.points += point;
            yield return EventControl.quartersec;
            GourmetRace.itemEaten++;
            entity.dead = true;
            entity.sprite.gameObject.SetActive(false);

            if (isZasp && UnityEngine.Random.Range(0, 100) < 20)
            {
                eater.entity.animstate = 114;
                yield return EventControl.sec;
            }
            eater.entity.animstate = 0;
            eater.isActive = true;
            eater.foundTarget = false;
            Destroy(gameObject);
        }

        public IEnumerator StartPlayerConsuming(GameObject eater)
        {
            MainManager.player.CancelAction();
            MainManager.player.entity.rigid.velocity = new Vector3(0f, MainManager.player.entity.rigid.velocity.y, 0f);
            MainManager.instance.minipause = true;
            MainManager.player.entity.backsprite = false;

            int[] buttonIds = null;
            if (MainManager.mashcommandalt)
            {
                buttons = new ButtonSprite[presses];
                buttonIds = new int[presses];
                for (int i = 0; i < buttonIds.Length; i++)
                {
                    buttonIds[i] = UnityEngine.Random.Range(0, 7);
                }

                float spacing = 1f; 
                float totalWidth = (presses - 1) * spacing;
                Vector3 startPos = new Vector3(0, -2, 8);
                float startX = startPos.x - totalWidth / 2;
                for (int i = 0; i < buttons.Length; i++)
                {
                    Color color = i == 0 ? Color.white : Color.gray;
                    Vector3 buttonPos = new Vector3(startX + i * spacing, startPos.y, startPos.z);
                    buttons[i] = new GameObject("Button" + i).AddComponent<ButtonSprite>().SetUp(buttonIds[i], -1, "", buttonPos, Vector3.one * 0.65f, 2, MainManager.GUICamera.transform, color);
                    if (InputIO.LongButton(buttonIds[i]))
                    {
                        buttons[i].shrunkkey = true;
                    }
                }
            }
            else
            {
                buttons = new ButtonSprite[1];
                buttons[0] = new GameObject().AddComponent<ButtonSprite>().SetUp(4, -1, "", new Vector3(0f, -3f, 10f), Vector3.one, 1, MainManager.GUICamera.transform);
            }

            yield return null;
            yield return null;
            int currentPress = 0;
            float eatingFrames = 0f;
            do
            {
                if (MainManager.mashcommandalt)
                {
                    if (MainManager.GetKey(buttonIds[currentPress], false))
                    {
                        buttons[currentPress].basesprite.color = Color.green;
                        currentPress++;
                        MainManager.PlaySound((currentPress < buttonIds.Length) ? "ACBeep" : "ACReady", -1, (currentPress < buttonIds.Length) ? (1f + (float)currentPress * 0.1f) : 1f, 1f);
                        if (currentPress < buttonIds.Length)
                        {
                            buttons[currentPress].basesprite.color = Color.white;
                        }
                        eatingFrames = 25f;
                    }else if(currentPress < buttonIds.Length && MainManager.GetKey(-4, false))
                    {
                        buttons[currentPress].basesprite.color = Color.red;
                        MainManager.PlayBuzzer();
                        yield return EventControl.halfsec;
                        buttons[currentPress].basesprite.color = Color.white;
                    }
                }
                else
                {
                    if (MainManager.GetKey(4))
                    {
                        currentPress++;
                        eatingFrames = 25f;
                    }

                    if (buttons[0].basesprite != null)
                    {
                        if (Mathf.Sin(Time.time * 10f) * 10f > 0f)
                        {
                            buttons[0].basesprite.color = Color.white;
                        }
                        else
                        {
                            buttons[0].basesprite.color = Color.gray;
                        }
                    }
                }

                if (eatingFrames > 0f)
                {
                    eatingFrames -= MainManager.TieFramerate(1f);
                    MainManager.player.entity.animstate = 121;
                    if (!MainManager.player.entity.sound.isPlaying)
                    {
                        MainManager.player.entity.PlaySound("Eat");
                    }
                }
                else
                {
                    MainManager.player.entity.animstate = 0;
                }

                yield return null;
            } while (currentPress < presses);

            foreach (var button in buttons)
                Destroy(button.gameObject);

            yield return null;
            MainManager.instance.flagvar[1] += point;
            yield return EventControl.quartersec;
            MainManager.instance.minipause = false;
            MainManager.player.lockkeys = false;
            GourmetRace.itemEaten++;
            Destroy(gameObject);
        }
    }

    public class GourmetContestant : MonoBehaviour 
    {
        public float speed;
        public float eatingSpeed;
        public EntityControl entity;
        public bool foundTarget = false;
        public GourmetRace race;
        GourmetItem targetItem;
        public bool isActive = false;
        public int points;
        bool doingPath = false;
        bool checkedPath = false;
        public int eatAnim;
        public Vector3 itemPos = Vector3.zero;

        Vector3[][] specialPaths = new Vector3[][]
        {
            new Vector3[]{ new Vector3(-9.2f, -4f,-13.6f) }, //left stairs bottom
            new Vector3[]{ new Vector3(10f, -4f,-13.6f) }, //right stairs bottom

            new Vector3[]{ new Vector3(-9.2f, -4f, -13.6f), new Vector3(-9.2f, 0f,-3.9f) },//left stairs top
            new Vector3[]{ new Vector3(10f, -4f, -13.6f), new Vector3(10f, 0f,-3.9f) },//right stairs top

            new Vector3[]{ new Vector3(8.6f, -4f,-21.6f), new Vector3(11.75f, -3.19f,-21.4f) }, // table
            new Vector3[]{ new Vector3(0.4f, -4f,-10.9f), new Vector3(-1.3f, -3.19f,-4.9f) }, // box
        };

        void Start()
        {
            entity = GetComponent<EntityControl>();
            entity.tag = "NPC";
        }

        void LateUpdate()
        {
            if (isActive)
            {
                if (targetItem == null || (targetItem != null && !targetItem.available))
                {
                    StopAllCoroutines();
                    doingPath = false;
                    checkedPath = false;
                    targetItem = race.FindClosestItem(transform.position);



                    if(targetItem != null)
                    {
                        foundTarget = true;
                        if (targetItem.transform.position.y > -4f)
                        {
                            entity.forcejump = true;
                        }
                        entity.MoveTowards(targetItem.transform.position, speed);
                    }
                    else
                    {
                        isActive = false;
                    }
                }
                else if(!doingPath && targetItem != null && !checkedPath)
                {
                    if(entity.forcetimer < 350f)
                    {
                        CheckPath();
                    }
                } 
            }
        }

        void CheckPath()
        {
            checkedPath = true;
            int pathID = GetPathID(targetItem);
            if (pathID > -1)
            {
                doingPath = true;
                StartCoroutine(DoPath(pathID));
            }
            else
            {
                entity.MoveTowards(targetItem.transform.position, speed);
            }
        }

        IEnumerator DoPath(int pathId)
        {
            Vector3[] path = specialPaths[pathId];
            for(int i=0;i< path.Length; i++)
            {
                entity.forcejump = true;

                if(Vector3.Distance(transform.position, path[i]) > 1)
                {
                    entity.MoveTowards(path[i], speed);
                    yield return new WaitUntil(() => !entity.forcemove);
                }
            }

            if(targetItem != null)
            {
                entity.forcejump = true;
                entity.MoveTowards(targetItem.transform.position, speed);
            }
            doingPath = false;
        }

        int GetPathID(GourmetItem item)
        {
            if(item.zoneID >= 4)
            {
                if(item.zoneID < 9)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }

            if(item.zoneID < 0)
            {
                switch (item.zoneID)
                {
                    //box
                    case -1:
                        return 5;

                    //table
                    case -3:
                        return 4;
                }
            }
            return -1;
        }

        public static GourmetContestant CreateContestant(int animid, Vector3 position, float speed, float eatingSpeed, string name, int eatAnimstate, Vector3 itemPosition)
        {
            EntityControl entity = EntityControl.CreateNewEntity(name, animid, position);
            entity.alwaysactive = true;
            GourmetContestant contestant  = entity.gameObject.AddComponent<GourmetContestant>();
            contestant.speed = speed;
            contestant.eatingSpeed = eatingSpeed;
            contestant.eatAnim = eatAnimstate;
            contestant.itemPos = itemPosition;
            return contestant;
        }
    
    }
}
