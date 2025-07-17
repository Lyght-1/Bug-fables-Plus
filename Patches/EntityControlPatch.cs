using HarmonyLib;
using System;
using UnityEngine;
using BFPlus.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections;
using UnityEditorInternal;
using UnityEngine.Rendering;

namespace BFPlus.Patches
{

    [HarmonyPatch(typeof(EntityControl), "SetAnim", new Type[] {typeof(string), typeof(bool)})]
    public class PatchEntityControlSetAnim
    {
        static void Prefix(EntityControl __instance)
        {
            if(__instance.animid == (int)MainManager.AnimIDs.ShielderAnt -1 && __instance.animstate == (int)MainManager.Animations.KO)
            {
                __instance.animstate = 100;
            }
        }
    }

    [HarmonyPatch(typeof(EntityControl), "ChompyRibbon")]
    public class PatchEntityControlChompyRibbon
    {
        static void Postfix(SpriteRenderer sprite)
        {
            int num = MainManager.instance.flagvar[56];

            if(num == (int)NewItem.WingRibbon)
            {
                sprite.material.color = new Color(0.3384f, 0.8271f, 1, 1);
            }
        }
    }

    [HarmonyPatch(typeof(EntityControl), "Start")]
    public class PatchEntityControlStart
    {
        static void Prefix(EntityControl __instance)
        {
            if(MainManager.player != null && MainManager.BadgeIsEquipped((int)Medal.SpuderCard) && __instance.CompareTag("Player"))
            {
                MainManager_Ext.CheckSpuderCardEffect(__instance.transform);                
            }
        }

        static void Postfix(EntityControl __instance)
        {
            //BIGFABLE Code
            if (MainManager.instance.flags[(int)NewCode.BIGFABLE])
            {
                if ((__instance.npcdata != null && (__instance.npcdata.entitytype != NPCControl.NPCType.Object && __instance.npcdata.entitytype != NPCControl.NPCType.SemiNPC)) || __instance.npcdata == null)
                {
                    __instance.startscale = __instance.transform.localScale * UnityEngine.Random.Range(0.5f, 1.75f);
                }
            }

            if (MainManager_Ext.IsNewEnemy(__instance, NewEnemies.MarsBud))
                __instance.bleeppitch = 0.7f;
        }
    }

    [HarmonyPatch(typeof(EntityControl), "CheckSpecialID")]
    public class PatchEntityControlCheckSpecialID
    {
        static void Prefix(EntityControl __instance)
        {
            if(MainManager.map != null && (NewMaps)MainManager.map.mapid == NewMaps.Pit100BaseRoom && PitData.GetCurrentFloor() > 80)
            {
                if(__instance.animid == (int)MainManager.AnimIDs.Krawler -1 || __instance.animid == (int)MainManager.AnimIDs.CursedSkull - 1 || __instance.animid == (int)MainManager.AnimIDs.Cape - 1)
                {
                    __instance.forcefire = true;
                }
            }
        }

        static void Postfix(EntityControl __instance)
        {
            if (MainManager_Ext.IsNewEnemy(__instance, NewEnemies.DarkVi) || MainManager_Ext.IsNewEnemy(__instance, NewEnemies.DarkLeif) || MainManager_Ext.IsNewEnemy(__instance, NewEnemies.DarkKabbu))
            {
                __instance.cotunknown = true;
                __instance.spritebasecolor = Color.black;
                __instance.ForceCOT();
            }

            if (MainManager_Ext.IsNewEnemy(__instance, NewEnemies.Frostfly))
            {
                var extras = __instance.extras.ToList();
                var particles = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/Snowflakes")) as GameObject;
                particles.transform.parent = __instance.spritetransform;
                particles.transform.localPosition = new Vector3(0f, 1.5f);
                extras.Add(particles);
                __instance.extras = extras.ToArray();
            }

            if (MainManager_Ext.IsNewEnemy(__instance, NewEnemies.Moeruki) || MainManager_Ext.IsNewEnemy(__instance, NewEnemies.FireAnt))
            {
                var particles = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/Flame")) as GameObject;
                particles.transform.parent = __instance.spritetransform;
                particles.transform.localPosition = new Vector3(0f, 0.75f);
                __instance.extras = new GameObject[] { particles };
            }

            if (MainManager_Ext.IsNewEnemy(__instance, NewEnemies.SplotchSpider))
            {
                for (int i = 0; i < 8; i++)
                {
                    Renderer render = __instance.model.GetChild(3 + i).GetComponent<Renderer>();
                    render.material.color = new Color(0.263f, 0.14f, 0.561f);
                }
            }



            if (MainManager_Ext.IsNewEnemy(__instance, NewEnemies.Belosslow))
            {
                __instance.rotater.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                __instance.startscale = __instance.rotater.localScale;
            }

            if (MainManager_Ext.IsNewEnemy(__instance, NewEnemies.DynamoSpore))
            {
                __instance.rotater.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                __instance.freezesize = new Vector3(3.2f, 3.5f, 1);
                __instance.freezeoffset = new Vector3(0, 1.8f);
                __instance.initialfrezeoffset = __instance.freezeoffset;
                __instance.startscale = __instance.rotater.localScale;
                var light = __instance.gameObject.AddComponent<DynamoSporeLight>();

                if ((int)MainManager.map.mapid == (int)NewMaps.PowerPlantBoss && !MainManager.instance.inevent)
                {
                    light.mode = DynamoSporeLight.Mode.ChargeUp;
                }
            }

            if (__instance.animid == (int)NewAnimID.IronSuit)
            {
                __instance.gameObject.AddComponent<IronSuit>();
            }

            if (MainManager_Ext.IsNewEnemy(__instance, NewEnemies.BatteryShroom))
            {
                __instance.freezesize = new Vector3(1.8f, 2.5f, 1);
                __instance.freezeoffset = new Vector3(0, 1.4f);
                __instance.initialfrezeoffset = __instance.freezeoffset;
                __instance.gameObject.AddComponent<BatteryShroomCopter>();

                if ((int)MainManager.map.mapid == (int)NewMaps.PowerPlantBoss && !MainManager.instance.inevent)
                {
                    __instance.hasshadow = false;
                }
            }

            if (MainManager_Ext.IsNewEnemy(__instance, NewEnemies.Levi))
            {
                __instance.freezesize = new Vector3(2f, 2.5f, 1);
                __instance.freezeoffset = new Vector3(0, 1.3f);
                __instance.initialfrezeoffset = __instance.freezeoffset;
            }

            if (MainManager_Ext.IsNewEnemy(__instance, NewEnemies.Celia))
            {
                __instance.freezesize = new Vector3(1.22f, 2.3f, 1);
                __instance.freezeoffset = new Vector3(0, 1.2f);
                __instance.initialfrezeoffset = __instance.freezeoffset;
            }

            if (__instance.animid == (int)NewAnimID.WormSwarm)
            {
                Vector3[] array = new Vector3[]
                {
                    new Vector3(-0.8f, 0f, 0.15f),
                    new Vector3(-0.5f, 0f, -0.2f),
                    new Vector3(0.5f, 0f, -0.3f),
                    new Vector3(1f, 0f, 0f),
                    new Vector3(0.3f, 0f, 0.25f),
                    new Vector3(0.1f, 0f, 0.1f),
                    new Vector3(-0.1f, 0f, -0.5f),
                    new Vector3(-0.2f, 0f, 0.15f),
                    new Vector3(1.3f, 0f, -0.3f),
                    new Vector3(0.8f, 0f, 0.15f)
                };
                __instance.subentity = new EntityControl[10];
                for (int l = 0; l < __instance.subentity.Length; l++)
                {
                    __instance.subentity[l] = EntityControl.CreateNewEntity("worm" + l, (int)NewAnimID.Worm, __instance.transform.position + array[l]);
                    __instance.subentity[l].transform.parent = __instance.transform;
                    __instance.subentity[l].hologram = __instance.hologram;
                    __instance.subentity[l].battle = __instance.battle;
                    __instance.subentity[l].animstate = 0;
                    __instance.subentity[l].gameObject.layer = 9;
                    __instance.subentity[l].destroytype = __instance.destroytype;
                    __instance.subentity[l].battleid = __instance.battleid;
                }
            }

            if (__instance.animid == (int)MainManager.AnimIDs.Scorpion - 1 && MainManager_Ext.IsNewEnemy(__instance, NewEnemies.DullScorp))
            {
                if (__instance.anim.runtimeAnimatorController.name == "Scorpion")
                {
                    __instance.anim.runtimeAnimatorController = MainManager_Ext.assetBundle.LoadAsset<RuntimeAnimatorController>("DullScorp");
                    Sprite scorpPart = MainManager_Ext.assetBundle.LoadAssetWithSubAssets<Sprite>("scorp").FirstOrDefault(s => s.name == "scorp_2");
                    CheckScorpPart(__instance.model, scorpPart);

                    if ((int)MainManager.map.mapid == (int)NewMaps.SandCastleDepthsBoss && MainManager.battle == null)
                    {
                        __instance.npcdata.trapped = true;
                        Transform iceCube = MainManager.map.transform.Find("scorpIce");
                        Physics.IgnoreCollision(iceCube.GetComponentInChildren<Collider>(), __instance.ccol, true);
                        __instance.onground = true;
                        __instance.StopForceMove();
                        __instance.extraoffset = Vector3.zero;
                        __instance.animstate = 11;
                        __instance.anim.speed = 0f;
                        __instance.npcdata.STOP();
                        __instance.hasshadow = false;
                        __instance.overrideanim = true;
                        __instance.overrideanimspeed = true;
                    }
                }
            }

            if (MainManager_Ext.IsNewEnemy(__instance, NewEnemies.Patton))
            {
                __instance.freezesize = new Vector3(2.6f, 3f, 1);
                __instance.freezeoffset = new Vector3(0, 1.6f);
                __instance.initialfrezeoffset = __instance.freezeoffset;
            }

            if (MainManager_Ext.IsNewEnemy(__instance, NewEnemies.LonglegsSpider))
            {
                __instance.freezesize = new Vector3(3f, 6f, 2.5f);
                __instance.freezeoffset = new Vector3(0, 3.2f,-0.3f);
                __instance.initialfrezeoffset = __instance.freezeoffset;
            }

            if (__instance.animid == (int)NewAnimID.Mars || __instance.animid == (int)NewAnimID.MarsSummon)
            {
                Transform endBone = FindEndBone(__instance.model);
                __instance.extras = new GameObject[]
                {
                    endBone.gameObject,
                    __instance.model.GetChild(0).gameObject
                };

                __instance.spinextra = new Vector3[]
                {
                    new Vector3(0 , 0f, __instance.animid == (int)NewAnimID.MarsSummon ? -0.1f: -0.15f)
                };
            }

            if (__instance.hologram && (__instance.animid == (int)NewAnimID.Mars || __instance.animid == (int)NewAnimID.MarsSummon || __instance.animid == (int)NewAnimID.Jester))
            {
                SpriteRenderer[] spriteInChild = __instance.model.GetComponentsInChildren<SpriteRenderer>();
                for (int i = 0; i < spriteInChild.Length; i++)
                {
                    spriteInChild[i].material = MainManager.holosprite;
                    spriteInChild[i].material.color = new Color(spriteInChild[i].material.color.r, spriteInChild[i].material.color.g, spriteInChild[i].material.color.b, 0.5f);
                }
            }
        }

        static Transform FindEndBone(Transform parent)
        {
            foreach(Transform t in parent)
            {
                if(t.name == "Bone.012")
                {
                    return t;
                }
               Transform child = FindEndBone(t);

                if (child != null)
                    return child;
            }
            return null;
        }

        static void CheckScorpPart(Transform t, Sprite scorpPart)
        {
            foreach (Transform child in t)
            {
                if (child.name.Contains("scorpion_7"))
                {
                    child.GetComponent<SpriteRenderer>().sprite = scorpPart;
                }
                else if (child.name == "Plane")
                {
                    child.GetComponent<SkinnedMeshRenderer>().material = MainManager_Ext.assetBundle.LoadAsset<Material>("ScorpLeg");
                }
                CheckScorpPart(child, scorpPart);
            }
        }

        
    }

    [HarmonyPatch(typeof(EntityControl), "UpdateConditionBubbles")]
    public class PatchEntityControlUpdateContidionBubbles
    {
        static void CreateIcon(EntityControl entity, bool condition, List<Transform> icons, Vector3 pos, int medal)
        {
            CreateIcon(entity, condition, icons, pos, MainManager.itemsprites[1, medal]);
        }

        static void CreateIcon(EntityControl entity, bool condition, List<Transform> icons, Vector3 pos, Sprite iconSprite)
        {
            if (condition)
            {
                Transform icon = entity.NewConditionIcon(iconSprite, pos);
                icons.Add(icon);
                icon.gameObject.SetActive(false);
            }
        }

        static void Postfix(EntityControl __instance, bool right, MainManager.BattleData data)
        {
            if (!__instance.nocondition && data.hp > 0 && data.eatenby == null && !__instance.iskill && !__instance.dead && __instance.battleid > -1)
            {
                float y = (!__instance.digging) ? (data.cursoroffset.y + __instance.height) : 1f;
                float x = -0.5f + data.cursoroffset.x;
                if (right)
                {
                    x = 0.5f + data.cursoroffset.x;
                }
                Vector3 pos = new Vector3(x, y, data.cursoroffset.z - 0.1f);
                List<Transform> list = new List<Transform>(__instance.statusicons);
                var entityExt = __instance.GetComponent<Entity_Ext>();
                if (__instance.playerentity)
                {
                    CreateIcon(__instance, BattleControl_Ext.Instance.InVengeance && MainManager.BadgeIsEquipped((int)Medal.Vengeance, data.trueid), list, pos, (int)Medal.Vengeance);
                    CreateIcon(__instance, MainManager.BadgeIsEquipped((int)Medal.Blightfury) && __instance.animid == 2, list, pos, (int)Medal.Blightfury);
                    CreateIcon(__instance, data.hp <= 4 && MainManager.BadgeIsEquipped((int)Medal.Adrenaline, data.trueid) && !entityExt.adrenalineUsed, list, pos, (int)Medal.Adrenaline);

                    if (MainManager.HasCondition(MainManager.BattleCondition.Freeze, data) > -1)
                    {
                        CreateIcon(__instance, MainManager.BadgeIsEquipped((int)Medal.ThinIce, data.trueid), list, pos, (int)Medal.ThinIce);
                        CreateIcon(__instance, MainManager.BadgeIsEquipped((int)Medal.Cryostatis, data.trueid), list, pos, (int)Medal.Cryostatis);
                    }

                    CreateIcon(__instance, Math.Abs((-1 * data.cantmove + 1) / 3) > 0 && MainManager.BadgeIsEquipped((int)Medal.KineticEnergy, data.trueid), list, pos, (int)Medal.KineticEnergy);

                    if (MainManager.HasCondition(MainManager.BattleCondition.Numb, data) > -1)
                        CreateIcon(__instance, MainManager.BadgeIsEquipped((int)Medal.PotentialEnergy, data.trueid), list, pos, (int)Medal.PotentialEnergy);

                    if (MainManager.HasCondition(MainManager.BattleCondition.Sleep, data) > -1)
                    {
                        CreateIcon(__instance, MainManager.BadgeIsEquipped((int)Medal.DestinyDream, data.trueid), list, pos, (int)Medal.DestinyDream);
                        CreateIcon(__instance, MainManager.BadgeIsEquipped((int)Medal.SweetDreams, data.trueid), list, pos, (int)Medal.SweetDreams);
                        CreateIcon(__instance, MainManager.BadgeIsEquipped((int)Medal.Nightmare, data.trueid), list, pos, (int)Medal.Nightmare);
                    }

                    CreateIcon(__instance, entityExt && entityExt.sleepScheduled, list, pos, (int)Medal.SleepSchedule);

                    if (entityExt != null && MainManager.BadgeIsEquipped((int)Medal.LifeLust, data.trueid) && entityExt.healedThisTurn > 0)
                    {
                        CreateIcon(__instance, true, list, pos, (int)Medal.LifeLust);
                        __instance.StartCoroutine(MainManager.SetText("|triui||center||sort,100||color,1||dropshadow,0.05,-0.05|" + entityExt.healedThisTurn, 2, null, false, false, new Vector3(0f, -0.2f, -0.01f), Vector3.zero, Vector2.one, list[list.Count - 1], null));
                    }

                    if (data.charge > 0)
                    {
                        CreateIcon(__instance, MainManager.BadgeIsEquipped((int)Medal.Recharge, data.trueid), list, pos, (int)Medal.Recharge);
                        CreateIcon(__instance, MainManager.BadgeIsEquipped((int)Medal.ChargeGuard, data.trueid), list, pos, (int)Medal.ChargeGuard);
                    }

                    CreateIcon(__instance, MainManager.BadgeIsEquipped((int)Medal.TwinedFate, data.trueid) && MainManager.instance.playerdata.Any(p => p.hp > 0 && p.hp <= 4 && p.trueid != data.trueid) && !BattleControl_Ext.Instance.twinedFateUsed, list, pos, (int)Medal.TwinedFate);
                    CreateIcon(__instance, MainManager.BadgeIsEquipped((int)Medal.TeamGleam) && MainManager.instance.tp == MainManager.instance.maxtp, list, pos, (int)Medal.TeamGleam);

                    int turns = MainManager.battle.turns;
                    if (MainManager.BadgeIsEquipped((int)Medal.OddWarrior, data.trueid))
                    {

                        bool odd = (turns + 1) % 2 != 0;
                        CreateIcon(__instance, true, list, pos, (int)Medal.OddWarrior);
                        __instance.StartCoroutine(MainManager.SetText("|triui||center||sort,100|" + "|color," + (odd ? "2|" : "1|") + "|dropshadow,0.05,-0.05|" + (odd ? "+" : "-"), 2, null, false, false, new Vector3(-0.35f, -0.2f, -0.01f), Vector3.zero, Vector2.one, list[list.Count - 1], null));
                    }

                    CreateIcon(__instance, turns == BattleControl_Ext.Instance.trustFallTurn + 1 && BattleControl_Ext.Instance.trustFallTurn > -1, list, pos, (int)Medal.TrustFall);

                    if (MainManager.HasCondition(MainManager.BattleCondition.Inked, data) > -1)
                    {
                        CreateIcon(__instance, MainManager.BadgeIsEquipped((int)Medal.InkBubble, data.trueid), list, pos, (int)Medal.InkBubble);
                    }

                    if (MainManager.HasCondition(MainManager.BattleCondition.Sticky, data) > -1)
                    {
                        CreateIcon(__instance, MainManager.BadgeIsEquipped((int)Medal.ThickSilk, data.trueid), list, pos, (int)Medal.ThickSilk);
                        CreateIcon(__instance, MainManager.BadgeIsEquipped((int)Medal.SpiderBait, data.trueid), list, pos, (int)Medal.SpiderBait);
                    }
                }

                if (MainManager.HasCondition(MainManager.BattleCondition.Sturdy, data) > -1)
                {
                    Sprite shellSprite = MainManager_Ext.assetBundle.LoadAsset<Sprite>("statusImmune");
                    CreateIcon(__instance, MainManager.HasCondition(MainManager.BattleCondition.Sturdy, data) > -1, list, pos, shellSprite);
                }

                if (MainManager.HasCondition(MainManager.BattleCondition.Inked, data) > -1)
                {
                    CreateIcon(__instance, MainManager.BadgeIsEquipped((int)Medal.CatalystSpill, data.trueid), list, pos, (int)Medal.CatalystSpill);
                }

                CreateIcon(__instance, entityExt.permanentInkTriggered, list, pos, (int)Medal.PermanentInk);
                CreateIcon(__instance, data.charge >0 && data.animid == (int)NewEnemies.LeafbugShaman, list, pos, (int)Medal.ChargeGuard);

                if (list.Count != __instance.statusicons.Length)
                {
                    __instance.statusicons = list.ToArray();
                }
            }
		}
    }

    [HarmonyPatch(typeof(EntityControl), "Death", new Type[] { typeof(bool)})]
    public class PatchEntityControlDeath
    {
        static void Prefix(EntityControl __instance, bool activatekill, ref IEnumerator __result)
        {
            if(__instance.animid == (int)NewAnimID.WormSwarm)
            {
                foreach(var entity in __instance.subentity)
                    entity.deathcoroutine = entity.StartCoroutine(entity.Death(true));
            }
        }
    }

    [HarmonyPatch(typeof(EntityControl), "SetAnimator")]
    public class PatchEntityControlSetAnimator
    {
        static void Postfix(EntityControl __instance)
        {
            if (!__instance.model && Enum.IsDefined(typeof(NewAnimID), __instance.animid))
            {
                if (__instance.animid == (int)NewAnimID.Worm || __instance.animid == (int)NewAnimID.WormSwarm)
                {
                    __instance.anim.runtimeAnimatorController = MainManager_Ext.assetBundle.LoadAsset<RuntimeAnimatorController>("Worm");
                }
                else
                {
                    __instance.anim.runtimeAnimatorController = MainManager_Ext.assetBundle.LoadAsset<RuntimeAnimatorController>(((NewAnimID)__instance.animid).ToString());
                }
            }

            if (!MainManager.instance.flags[616])   
            {
                if (!__instance.isplayer && !__instance.mainparty)
                {
                    MainManager_Ext.CheckEnemyVariantAnimator(__instance);

                    MainManager.AnimIDs[] newAnims = new MainManager.AnimIDs[] { MainManager.AnimIDs.Jayde, MainManager.AnimIDs.Patton, 
                        MainManager.AnimIDs.TeaMoth, MainManager.AnimIDs.DragonflyLady, MainManager.AnimIDs.WaspGeneral, MainManager.AnimIDs.WaspQueen };

                    foreach(var newAnim in newAnims)
                    {
                        if (__instance.animid == (int)newAnim - 1)
                            __instance.anim.runtimeAnimatorController = MainManager_Ext.assetBundle.LoadAsset<RuntimeAnimatorController>(newAnim.ToString());
                    }
                }
                else
                {
                    if (MainManager.BadgeIsEquipped((int)Medal.Switcheroo) && !MainManager.instance.flags[916])
                    {
                        __instance.anim.runtimeAnimatorController = MainManager_Ext.Instance.GetSwitcherooAnim(__instance.animid);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(EntityControl), "AnimSpecificQuirks")]
    public class PatchEntityControlAnimSpecificQuirks
    {
        static void Prefix(EntityControl __instance)
        {
            if (__instance.animid == (int)MainManager.AnimIDs.Mushroom - 1 && MainManager_Ext.IsNewEnemy(__instance, NewEnemies.BatteryShroom))
            {
                if (__instance.height > 0.1f && (__instance.extras == null || __instance.extras.Length == 0 || __instance.extras[0] == null))
                {
                    Sprite copterSprite = MainManager_Ext.assetBundle.LoadAssetWithSubAssets<Sprite>("BatteryShroom").Where(s => s.name == "BatteryShroom_20").FirstOrDefault();
                    __instance.extras = new GameObject[]
                    {
                        MainManager.NewSpriteObject("copter", new Vector3(0f, 0f, 0f), new Vector3(85f, 0f, 0f), __instance.spritetransform, copterSprite, MainManager.spritemat).gameObject
                    };
                    __instance.spinextra = new Vector3[] { new Vector3(0f, 0f, 15f) };

                    __instance.extras[0].transform.localEulerAngles = new Vector3(85f, 25f, 25f);
                    __instance.extras[0].transform.localPosition = Vector3.zero;
                    __instance.extras[0].transform.localScale = Vector3.one;
                }

                if (__instance.bobspeed < 0.1f)
                {
                    __instance.bobspeed = 0.1f;
                    __instance.startbs = __instance.bobspeed;
                }
                if (__instance.bobrange < 0.1f)
                {
                    __instance.bobrange = 4f;
                    __instance.startbf = __instance.bobrange;
                }

                if(__instance.animstate >= 103)
                {
                    __instance.bobrange = 0;
                    __instance.startbf = __instance.bobrange;
                    __instance.bobspeed = 0;
                    __instance.startbs = __instance.bobspeed;
                }

                if (__instance.extras != null && __instance.extras.Length > 0 && __instance.extras[0] != null)
                {
                    if(__instance.anim.GetCurrentAnimatorClipInfo(0).Length > 0)
                    {
                        string animName = __instance.anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                        __instance.extras[0].gameObject.SetActive((animName.Contains('f') || __instance.animstate == 100 || __instance.animstate == 101) && __instance.animstate != (int)MainManager.Animations.Hurt && __instance.animstate != (int)MainManager.Animations.HurtFallen);

                    }

                    __instance.extras[0].transform.localEulerAngles += __instance.spinextra[0] * MainManager.framestep;
                }
            }
        }

        static void Postfix(EntityControl __instance)
        {
            if (__instance.animid == (int)NewAnimID.WormSwarm)
            {
                if (__instance.subentity != null && __instance.subentity.Length > 0)
                {
                    foreach (var entity in __instance.subentity)
                    {
                        entity.animstate = __instance.animstate;
                        entity.spin = __instance.spin;
                        entity.flip = __instance.flip;
                        entity.sprite.material.color = __instance.sprite.material.color;
                        if (!MainManager.instance.inbattle)
                        {
                            entity.digging = __instance.digging;
                            entity.nodigpart = true;
                            entity.rigid.isKinematic = true;
                        }
                    }
                }
            }

            if (__instance.animid == (int)MainManager.AnimIDs.Flowering - 1)
            {
                if (__instance.extras != null && __instance.extras.Length > 0 && __instance.extras[0] != null)
                {
                    __instance.extras[0].gameObject.SetActive(__instance.flyinganim || __instance.animstate == 100 || __instance.animstate == 101 || __instance.animstate == 102);
                }
            }


            if(__instance.animid == (int)NewAnimID.Mars || __instance.animid == (int)NewAnimID.MarsSummon)
            {
                if (__instance.extras != null && __instance.extras.Length > 0 && __instance.extras[0] != null && __instance.extras[1]!=null)
                {
                    __instance.extras[1].transform.position = __instance.extras[0].transform.position + __instance.spinextra[0];
                }
            }
   
        }
    }

    //Create Resistance icon under hp bar
    [HarmonyPatch(typeof(EntityControl), "Jump", new Type[] { })]
    public class PatchEntityControlJump
    {
        static void Postfix(EntityControl __instance)
        {
            if (__instance.animid == (int)NewAnimID.Jester || __instance.animid == (int)NewAnimID.FirePopper)
            {
                __instance.PlaySound("Boing1",1f,1.2f);
            }
        }

    }


    //Create Resistance icon under hp bar
    [HarmonyPatch(typeof(EntityControl), "UpdateSpriteMat")]
    public class PatchEntityControlUpdateSpriteMat
    {
        static void Postfix(EntityControl __instance)
        {
            if (__instance.cotunknown)
            {
                __instance.ForceCOT();
            }
        }

    }

    //Create Resistance icon under hp bar
    [HarmonyPatch(typeof(EntityControl), "CreateHPBar")]
    public class PatchEntityControlCreateHPBar
    {
        static void Postfix(EntityControl __instance)
        {
            Entity_Ext.GetEntity_Ext(__instance).CreateResIcons();            
        }
    }

    [HarmonyPatch(typeof(EntityControl), "AddModel")]
    public class PatchEntityControlAddModel
    {
        static void Postfix(EntityControl __instance)
        {
            if(MainManager_Ext.IsNewEnemy(__instance, NewEnemies.SplotchSpider))
            {
                __instance.anim.runtimeAnimatorController = MainManager_Ext.assetBundle.LoadAsset<RuntimeAnimatorController>(NewEnemies.SplotchSpider.ToString());
            }

            if (__instance.animid == (int)MainManager.AnimIDs.LongLegs - 1)
            {
                __instance.anim.runtimeAnimatorController = MainManager_Ext.assetBundle.LoadAsset<RuntimeAnimatorController>(MainManager.AnimIDs.LongLegs.ToString());
                __instance.model.transform.localPosition = new Vector3(0, 4.7f, 0);
            }

            if (__instance.animid == (int)NewAnimID.Jester || __instance.animid == (int)NewAnimID.FirePopper)
            {
                foreach (var renderer in __instance.model.GetComponentsInChildren<SpriteRenderer>())
                    renderer.material = MainManager.spritemat;


                if(__instance.animid == (int)NewAnimID.Jester)
                {
                    foreach (var renderer in __instance.model.GetComponentsInChildren<MeshRenderer>())
                    {
                        Material mainMat = renderer.material;
                        renderer.materials = new Material[] { mainMat, MainManager.outlinemain };
                    }

                    GameObject bodySprings = __instance.model.transform.GetChild(0).gameObject;

                    MainManager_Ext.AddJesterComponent(bodySprings.transform, 6, Vector3.zero, __instance.model.transform);

                    Transform spring5 = bodySprings.transform.GetChild(5);
                    MainManager_Ext.AddJesterComponent(spring5.GetChild(0), 11, new Vector3(1, -1.5f, 0), __instance.model.transform);
                    MainManager_Ext.AddJesterComponent(spring5.GetChild(1), 11, new Vector3(-1, -1.5f, 0), __instance.model.transform);

                    Transform box = __instance.model.GetChild(1);

                    foreach (Transform child in box)
                    {
                        if (child.name.Contains("Fire"))
                        {
                            Transform flame = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/Flame")) as GameObject).transform;
                            flame.parent = child.transform;
                            flame.localPosition = Vector3.zero;
                            flame.localScale = Vector3.one * 1.2f;
                        }
                    }
                }
                else
                {
                    GameObject bodySprings = __instance.model.transform.GetChild(0).gameObject;

                    MainManager_Ext.AddJesterComponent(bodySprings.transform, 4, Vector3.zero, __instance.model.transform,false);
                }
            }

            if(__instance.animid == (int)NewAnimID.Mars || __instance.animid == (int)NewAnimID.MarsSummon)
            {
                foreach (var renderer in __instance.model.GetComponentsInChildren<SpriteRenderer>())
                    renderer.material = MainManager.spritemat;

                /*foreach (var renderer in __instance.model.GetComponentsInChildren<SkinnedMeshRenderer>())
                    renderer.material.shader = MainManager.spritemat.shader;*/


                if (__instance.animid == (int)NewAnimID.Mars)
                {
                    Transform pot = __instance.model.transform.Find("MarsPot");

                    //vines
                    //pot.GetChild(0).GetComponent<MeshRenderer>().material.shader = MainManager.spritemat.shader;
                }

                if (__instance.animid == (int)NewAnimID.MarsSummon)
                {
                    Transform dirt = __instance.model.transform.Find("Dirt");
                    dirt.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sprites/Entities/pitcher")[16];
                }
            }
        }

    }

    [HarmonyPatch(typeof(EntityControl), "SetDialogueBleep")]
    public class PatchEntityControlSetDialogueBleep
    {
        static void Postfix(EntityControl __instance)
        {
            if (MainManager_Ext.IsNewEnemy(__instance, NewEnemies.MarsBud))
            {
                __instance.bleeppitch = 0.7f;
                __instance.dialoguebleepid = 8;
            }

            if (__instance.animid == (int)MainManager.AnimIDs.WaspBomber-1)
            {
                __instance.bleeppitch = 1f;
                __instance.dialoguebleepid = 5;
            }
        }
    }

    [HarmonyPatch(typeof(EntityControl), "BreakIce")]
    public class PatchEntityControlBreakIce
    {
        static void Prefix(EntityControl __instance)
        {
            if (MainManager.instance.inbattle)
            {
                if (__instance.playerentity && __instance.icecube != null)
                {
                    BattleControl_Ext.Instance.CheckThinIce(__instance);
                }
            }
        }

        static void Postfix(EntityControl __instance)
        {
            if (MainManager.instance.inbattle)
            {
                __instance.icecube = null; // make sure to set icecube to null after it being destroyed,
                                           // fix annoying stuff with flash freeze
            }
        }
    }
}
