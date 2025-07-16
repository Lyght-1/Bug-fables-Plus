using BFPlus.Extensions;
using HarmonyLib;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static MainManager;

namespace BFPlus.Patches
{
    [HarmonyPatch(typeof(BattleControl), "EndEnemyTurn")]
    public class PatchBattleControlEndEnemyTurn
    {
        static void Prefix(BattleControl __instance)
        {
            BattleControl_Ext.enemyUsedItem = false;
        }
    }

    [HarmonyPatch(typeof(BattleControl), "ClearStatus")]
    public class PatchBattleControlClearStatus
    {
        static int conditionAmount = 0;
        static void Prefix(BattleControl __instance, ref MainManager.BattleData target)
        {
            conditionAmount = BattleControl_Ext.Instance.CalculateCleanseDamage(target);
        }

        static void Postfix(BattleControl __instance, ref MainManager.BattleData target)
        {
            int amountCleared = conditionAmount - target.condition.Count;

            if (BattleControl_Ext.actionID == (int)MainManager.Skills.Cleanse)
            {
                BattleControl_Ext.Instance.DealCleanseDamage(__instance, ref target);
            }

            var entityExt = Entity_Ext.GetEntity_Ext(target.battleentity);
            if (MainManager.HasCondition(MainManager.BattleCondition.Inked, target) == -1 && entityExt.inkDebuffed)
            {
                entityExt.CheckInkDebuff(ref target);
            }

            entityExt.slugskinActive = false;
            entityExt.vitiation = false;
            entityExt.vitiationDmg = 0;
            entityExt.healedThisTurn = 0;

            if (target.battleentity.playerentity && conditionAmount > target.condition.Count)
            {
                if (amountCleared > 0 && target.hp > 0)
                {
                    BattleControl_Ext.Instance.DoPurifyingPulseCheck(ref target, amountCleared);
                    BattleControl_Ext.Instance.DoRevitalizingRippleCheck(ref target, amountCleared);
                }
            }

        }
    }

    [HarmonyPatch(typeof(BattleControl), "SetItem")]
    public class PatchBattleControlSetItem
    {
        static bool Prefix(BattleControl __instance, int id)
        {
            if ((id == 50 || id == 51 || id == 52) && (MainManager.listtype < 0 && __instance.currentaction == BattleControl.Pick.SkillList && MainManager.BadgeIsEquipped((int)Medal.HoloSkill, __instance.currentturn)))
            {
                __instance.StartCoroutine(BattleControl_Ext.Instance.GetSkillList(id));
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(BattleControl), "HPBarOnOther")]
    public class PatchBattleControlHPBarOnOther
    {
        static void Postfix(int thisid, ref bool __result)
        {
            if (thisid == (int)NewEnemies.MarsSprout)
            {
                __result = MainManager.instance.librarystuff[1, (int)NewEnemies.Mars];
            }
        }
    }

    [HarmonyPatch(typeof(BattleControl), "RevivePlayer", new Type[] { typeof(int), typeof(int), typeof(bool) })]
    public class PatchBattleControlRevivePlayer
    {
        static bool Prefix(int id, int hp, bool showcounter)
        {
            BattleControl_Ext.Instance.InVengeance = false;
            return true;
        }
    }

    [HarmonyPatch(typeof(BattleControl), "EndPlayerTurn")]
    public class PatchBattleControlEndPlayerTurn
    {
        static bool Prefix(BattleControl __instance)
        {
            __instance.StartCoroutine(BattleControl_Ext.Instance.ResetHoloID(__instance));
            int currentTurn = __instance.currentturn;

            for(int i = 0; i < MainManager.instance.playerdata.Length; i++)
            {
                if (MainManager.instance.playerdata[i].hp > 0)
                {
                    BattleControl_Ext.Instance.CheckHDWGHConditionAmount(MainManager.instance.playerdata[i], MainManager.instance.playerdata[i].battleentity.GetComponent<Entity_Ext>());
                }
            }

            if (BattleControl_Ext.Instance.gourmetItemUse > 0)
            {
                bool isStopped = __instance.IsStoppedLite(MainManager.instance.playerdata[currentTurn]);
                int aliveEnemies = __instance.AliveEnemies();
                if (MainManager.instance.items[0].Count > 0 && !isStopped && aliveEnemies > 0 && !__instance.inevent)
                {
                    if (__instance.action)
                    {
                        __instance.StartCoroutine(BattleControl_Ext.Instance.WaitForActionGourmet());
                    }
                    else
                    {
                        BattleControl_Ext.Instance.gourmetItemUse--;
                        BattleControl_Ext.Instance.GoToItemList();
                    }
                    return false;
                }
            }
            BattleControl_Ext.Instance.gourmetItemUse = -1;
            return true;
        }
    }

    [HarmonyPatch(typeof(BattleControl), "UpdateEntities")]
    public class PatchBattleControlUpdateEntities
    {
        static void Postfix(BattleControl __instance)
        {
            //BattleControl_Ext.Instance.CheckEntitiesSprites();
        }
    }

    [HarmonyPatch(typeof(BattleControl), "PlayerTurn")]
    public class PatchBattleControlPlayerTurn
    {
        static bool Prefix(BattleControl __instance)
        {
            if (MainManager.instance.flags[(int)NewCode.EVEN] && (__instance.turns + 1) % 2 != 0)
            {
                for (int i = 0; i < MainManager.instance.playerdata.Length; i++)
                    MainManager.instance.playerdata[i].cantmove = 1;
   
                __instance.chompyattacked = true;
                __instance.aiattacked = true;
                __instance.enemy = true;
                __instance.currentturn = -1;
                __instance.UpdateAnim();
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(BattleControl), "GetChoiceInput")]
    public class PatchBattleControlGetChoiceInput
    {
        static void Prefix(BattleControl __instance)
        {
            if (__instance.currentaction == BattleControl.Pick.BaseAction && BattleControl_Ext.Instance.holoSkillID != -1)
            {
                BattleControl_Ext.Instance.holoSkillID = -1;
            }
        }
    }

    [HarmonyPatch(typeof(BattleControl), "Retry")]
    public class PatchBattleControlRetry
    {
        static void Prefix(BattleControl __instance)
        {
            BattleControl_Ext.stylishBarAmount = BattleControl_Ext.startStylishAmount;
            BattleControl_Ext.stylishReward = BattleControl_Ext.startStylishReward;
        }
    }

    [HarmonyPatch(typeof(BattleControl), "StartData")]
    public class PatchBattleControlStartData
    {
        static void Prefix(BattleControl __instance)
        {
            BattleControl_Ext.startStylishAmount = BattleControl_Ext.stylishBarAmount;
            BattleControl_Ext.startStylishReward = BattleControl_Ext.stylishReward;

            BattleControl_Ext.Instance.ResetStuff();
        }
    }

    [HarmonyPatch(typeof(BattleControl), "AddDelayedCondition")]
    public class PatchBattleControlAddDelayedCondition
    {
        static bool Prefix(BattleControl __instance, int enid)
        {
            if(MainManager.HasCondition(MainManager.BattleCondition.Sturdy, MainManager.battle.enemydata[enid]) > -1)
            {
                return false;
            }
            return true;
        }
    }


    //randomize commands but data field isnt right for each command
    [HarmonyPatch(typeof(BattleControl), "DoCommand")]
    public class PatchBattleControlDoCommand
    {
        static void FillRandomCommandData(float[] data, float[][] maxData, int[] dataType)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (i >= maxData[0].Length)
                    break;

                if (dataType[i] == 0)
                {
                    data[i] = UnityEngine.Random.Range((int)maxData[0][i], (int)maxData[1][i]);
                }
                else
                {
                    data[i] = UnityEngine.Random.Range(maxData[0][i], maxData[1][i]);
                }
            }
        }


        static float[] GetRandomCommandData(object commandType, ref float timer)
        {
            float[] data = new float[1];
            float[][] minMaxData;
            switch (commandType)
            {
                case 4:
                case 10:
                    data = new float[] { 0, 0, 0, 1f };
                    minMaxData = new float[][]
                    {
                        new float[] { 4, 6, 0.1f },
                        new float[] { 7, 8, 1.25f }
                    };

                    FillRandomCommandData(data, minMaxData, new int[] { 0, 1, 1 });

                    if (UnityEngine.Random.Range(0, 2) == 0 && (int)commandType != 10)
                        data[0] = -1;

                    timer = UnityEngine.Random.Range(160f, 220f);
                    break;

                case 5:
                    data = new float[] { 0, 1f };
                    minMaxData = new float[][]
                    {
                        new float[] { 1, 0 },
                        new float[] { 10, 7 }
                    };
                    FillRandomCommandData(data, minMaxData, new int[] { 0, 0 });
                    timer = UnityEngine.Random.Range(20f, 30f);
                    break;

                case 9:
                    data = new float[] { 0, 1f, 1, 2f };
                    minMaxData = new float[][]
                    {
                        new float[] { 25, 45,25 },
                        new float[]{ 35,70,35}
                    };
                    FillRandomCommandData(data, minMaxData, new int[] { 1, 1, 1 });
                    timer = UnityEngine.Random.Range(70f, 120f);
                    break;

                case 11:
                    data = new float[] { 0, 1f };
                    minMaxData = new float[][]
                    {
                        new float[] { 0, 1 },
                        new float[] { 3, 6 }
                    };
                    FillRandomCommandData(data, minMaxData, new int[] { 0, 0 });
                    timer = UnityEngine.Random.Range(25f, 50f);
                    break;

                case 13:
                    data = new float[] { 0, 1f };
                    minMaxData = new float[][]
                    {
                        new float[] { 1,0},
                        new float[] { 5, 2.0625f }
                    };
                    FillRandomCommandData(data, minMaxData, new int[] { 0, 1 });
                    timer = 0.01f;
                    break;

                case 14:
                    data = new float[UnityEngine.Random.Range(1, 3)];
                    minMaxData = new float[][]
                    {
                        new float[] { 2,0},
                        new float[] { 8, 0 }
                    };
                    FillRandomCommandData(data, minMaxData, new int[] { 0, 0 });
                    timer = UnityEngine.Random.Range(120f, 300f);
                    break;

                case 15:
                    data = new float[] { 0, 1f, 0, 0 };
                    minMaxData = new float[][]
                    {
                        new float[] { 1,0,1,0},
                        new float[] { 5, 0,3.25f,10 }
                    };
                    FillRandomCommandData(data, minMaxData, new int[] { 0, 0, 1, 0 });
                    timer = UnityEngine.Random.Range(60f, 120f);
                    break;

                case 16:
                    data = new float[] { 0 };
                    minMaxData = new float[][]
                    {
                        new float[] { 0},
                        new float[] { 7 }
                    };
                    FillRandomCommandData(data, minMaxData, new int[] { 0 });
                    timer = UnityEngine.Random.Range(60f, 120f);
                    break;
            }
            return data;
        }

        static void Prefix(BattleControl __instance, ref object commandtype, ref float[] data, ref float timer)
        {
            if (MainManager.instance.flags[(int)NewCode.COMMAND])
            {
                int commandsAmount = Enum.GetNames(typeof(BattleControl.ActionCommands)).Length;
                List<int> commands = Enumerable.Range(0, commandsAmount).ToList();
                List<int> unusedCommands = new List<int> { 0, 1, 2, 3, 6, 7, 8, 12 };
                commands.RemoveAll(c => unusedCommands.Contains(c));

                commandtype = commands[UnityEngine.Random.Range(0, commands.Count)];
                data = GetRandomCommandData(commandtype, ref timer);
            }
        }
    }

    [HarmonyPatch(typeof(BattleControl), "MultiSkillMove")]
    public class PatchBattleControlMultiSkillMove
    {
        static void Prefix(BattleControl __instance, int[] ids)
        {
            BattleControl_Ext.Instance.attackedThisTurn.AddRange(ids);
        }
    }

    [HarmonyPatch(typeof(BattleControl), "GetMultiDamage")]
    public class PatchBattleControlGetMultiDamage
    {
        static void Postfix(BattleControl __instance, int[] ids, ref int __result)
        {
            foreach (int id in ids)
            {
                var attacker = MainManager.GetPlayerData(id);
                __result += BattleControl_Ext.Instance.CheckKineticEnergy(attacker);
                __result += BattleControl_Ext.Instance.CheckTeamGleam();
                __result += BattleControl_Ext.Instance.CheckOddWarrior(__instance, attacker);
            }
        }
    }

    [HarmonyPatch(typeof(BattleControl), "DefaultDamageCalc")]
    public class PatchBattleControlDefaultDamageCalc
    {
        static int damageTemp = 0;

        static void Prefix(BattleControl __instance, MainManager.BattleData target, ref int basevalue, bool pierce, bool blocked, int def)
        {
            damageTemp = basevalue;
        }

        static void Postfix(BattleControl __instance, MainManager.BattleData target, ref int basevalue, bool pierce, bool blocked, int def)
        {

            bool isPlayer = target.battleentity.CompareTag("Player");

            if (isPlayer)
            {

                //shock trooper halves damage
                if(MainManager.HasCondition(MainManager.BattleCondition.Numb, target) > -1 && MainManager.BadgeIsEquipped(34, target.trueid))
                {
                    basevalue /= 2;
                }

                if (!pierce && MainManager.HasCondition(MainManager.BattleCondition.Sleep, target) > -1 && MainManager.BadgeIsEquipped((int)Medal.SweetDreams))
                {
                    int defSweetDreams = MainManager.BadgeHowManyEquipped((int)Medal.SweetDreams, target.trueid) * 1;
                    basevalue += defSweetDreams;
                    BattleControl_Ext.Instance.realDamage += defSweetDreams;
                }

                if (MainManager.BadgeIsEquipped((int)Medal.NoPainNoGain))
                {
                    int defNPNG = MainManager.BadgeHowManyEquipped((int)Medal.NoPainNoGain);
                    basevalue += defNPNG;
                    BattleControl_Ext.Instance.realDamage += defNPNG;
                }


                if (def < 0 && !pierce)
                {
                    BattleControl_Ext.Instance.realDamage -= def;
                }
            }
            else
            {
                if(target.animid == (int)NewEnemies.LeafbugShaman && target.charge > 0)
                {
                    MainManager.battle.StartCoroutine(MainManager.battle.ItemSpinAnim(target.battleentity.transform.position + Vector3.up, MainManager.itemsprites[1, (int)Medal.ChargeGuard], true));
                    MainManager.battle.enemydata[target.battleentity.battleid].def = MainManager.battle.enemydata[target.battleentity.battleid].basedef;
                    MainManager.battle.enemydata[target.battleentity.battleid].charge = 0;
                }
            }
        }
    }


    [HarmonyPatch(typeof(BattleControl), "AdvanceTurnEntity")]
    public class PatchBattleControlAdvanceTurnEntity
    {
        static void Postfix(BattleControl __instance, ref MainManager.BattleData t)
        {
            BattleControl_Ext.Instance.CheckSweetDreams(t);
            var entityExt = Entity_Ext.GetEntity_Ext(t.battleentity);
            if (MainManager.HasCondition(MainManager.BattleCondition.Taunted, t) == -1)
            {
                entityExt.tauntedBy = -1;
            }

            if (entityExt.vitiation)
            {
                entityExt.vitiationDmg = 0;
                entityExt.vitiation = false;
                t.battleentity.shieldenabled = false;
            }

            if(entityExt.isPlayer && t.hp > 0 &&MainManager.BadgeIsEquipped((int)BadgeTypes.LastWind, t.trueid) && entityExt.lastTurnHp > t.hp && entityExt.lastTurnHp - t.hp >= 8)
            {
                MainManager.battle.StartCoroutine(battle.ItemSpinAnim(t.battleentity.transform.position + Vector3.up, MainManager.itemsprites[1, (int)BadgeTypes.LastWind], true));
                t.cantmove--;
            }
            entityExt.lastTurnHp = t.hp;
            if (MainManager.HasCondition(MainManager.BattleCondition.Inked, t) == -1 && entityExt.inkDebuffed)
            {
                if (MainManager.BadgeIsEquipped((int)Medal.PermanentInk) && !entityExt.permanentInkTriggered)
                {
                    entityExt.permanentInkTriggered = true;
                }
                else
                {
                    entityExt.CheckInkDebuff(ref t);
                    entityExt.permanentInkTriggered = false;
                }
            }
        }
    }

    [HarmonyPatch(typeof(BattleControl), "StatEffect")]
    public class PatchBattleControlStatEffect
    {
        static void Prefix(BattleControl __instance, EntityControl target, int type)
        {
            if (type == 4 && target.CompareTag("Player") && MainManager.BadgeIsEquipped((int)Medal.BugBattery, MainManager.instance.playerdata[target.battleid].trueid) && MainManager.HasCondition(MainManager.BattleCondition.Sturdy, MainManager.instance.playerdata[target.battleid]) == -1)
            {
                MainManager.PlaySound("Numb");
                MainManager.SetCondition(MainManager.BattleCondition.Numb, ref MainManager.instance.playerdata[target.battleid], 2);
                MainManager.instance.playerdata[target.battleid].isnumb = true;
            }

        }
    }

    [HarmonyPatch(typeof(BattleControl), "PincerStatus")]
    public class PatchBattleControlPincerStatus
    {
        static bool Prefix(BattleControl __instance, ref MainManager.BattleData data)
        {
            if (MainManager.BadgeIsEquipped((int)Medal.FrostNeedles))
            {
                __instance.TryCondition(ref data, MainManager.BattleCondition.Freeze, 2);
            }

            if (MainManager.BadgeIsEquipped((int)Medal.FireNeedles))
            {
                __instance.TryCondition(ref data, MainManager.BattleCondition.Fire, 2);
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(BattleControl), "GetRandomAvaliablePlayer", new Type[] { })]
    public class PatchBattleControlGetRandomAvaliablePlayer
    {
        static bool Prefix(BattleControl __instance, ref int __result)
        {
            return BattleControl_Ext.Instance.GetTauntedBy(ref __result, __instance);
        }
    }

    [HarmonyPatch(typeof(BattleControl), "GetRandomAvaliablePlayer", new Type[] { typeof(bool) })]
    public class PatchBattleControlGetRandomAvaliablePlayerBool
    {
        static bool Prefix(BattleControl __instance, ref int __result)
        {
            return BattleControl_Ext.Instance.GetTauntedBy(ref __result, __instance);
        }
    }

    [HarmonyPatch(typeof(BattleControl), "CameraFocusTarget", new Type[] { })]
    public class PatchBattleControlCameraFocusTarget
    {
        static bool Prefix(BattleControl __instance)
        {
            return __instance.playertargetID >= 0;
        }
    }


    [HarmonyPatch(typeof(BattleControl), "SeedlingTackle")]
    public class PatchBattleControlSeedlingTackle
    {
        static void Prefix(BattleControl __instance, ref int damage, int attackerid, BattleControl.AttackProperty? property)
        {
            if (__instance.enemydata[attackerid].animid == (int)NewEnemies.Caveling)
                damage = 3;
        }
    }

    [HarmonyPatch]
    public class PatchBattleControlDoDamage
    {
        static MethodBase TargetMethod()
        {
            IEnumerable methods = typeof(BattleControl).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(method => method.Name == "DoDamage").Cast<MethodBase>();
            return typeof(BattleControl).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(method => method.Name == "DoDamage" && method.GetParameters().Length == 6).FirstOrDefault();
        }
        static bool isFireOrPoison = false;
        static bool hadPlating = false;
        static int beforeDoDamageHp = -1;
        static bool superBlocked = false;

        static bool Prefix(BattleControl __instance, MethodBase __originalMethod, MainManager.BattleData? attacker, ref MainManager.BattleData target, ref int damageammount, BattleControl.AttackProperty? property, ref BattleControl.DamageOverride[] overrides, bool block)
        {
            var damageOverrides = new object[] { 16, 17, 18, 19, 20 };
            isFireOrPoison = overrides != null && damageOverrides.Length == overrides.Length && !block && property == BattleControl.AttackProperty.NoExceptions;
            BattleControl_Ext.Instance.targetIsPlayer = target.battleentity.CompareTag("Player");

            if (BattleControl_Ext.Instance.targetIsPlayer)
            {
                superBlocked = (__instance.GetSuperBlock(target.battleentity.animid) || __instance.superblockedthisframe > 0f) && !__instance.IsStopped(target);
            }

            int twinedfateBug = BattleControl_Ext.GetEquippedMedalBug(Medal.TwinedFate, (i) => MainManager.instance.playerdata[i].hp > 0 && MainManager.instance.playerdata[i].eatenby == null);
            if (BattleControl_Ext.Instance.targetIsPlayer && !isFireOrPoison && twinedfateBug != -1 && !BattleControl_Ext.Instance.twinedFateUsed && target.trueid != twinedfateBug && target.hp <= 4)
            {
                BattleControl_Ext.Instance.twinedFateUsed = true;
                __instance.DoDamage(attacker, ref MainManager.instance.playerdata[twinedfateBug], damageammount, property, overrides, block);
                return false;
            }

            var entityExt = Entity_Ext.GetEntity_Ext(target.battleentity);

            if (BattleControl_Ext.Instance.targetIsPlayer && MainManager.BadgeIsEquipped((int)Medal.Revengarang) && attacker != null && !isFireOrPoison && target.trueid == 0 && block && __instance.nonphyscal)
            {
                BattleControl_Ext.Instance.revengarangIsActive = true;
                BattleControl_Ext.Instance.revengarangDMG = 1 + MainManager.BadgeHowManyEquipped((int)Medal.Revengarang);
                if (superBlocked)
                {
                    BattleControl_Ext.Instance.revengarangDMG += MainManager.BadgeHowManyEquipped((int)MainManager.BadgeTypes.SuperBlock, 0);
                }

                if (entityExt.slugskinActive)
                    BattleControl_Ext.Instance.revengarangDMG++;

            }

            BattleControl_Ext.Instance.realDamage = 0;

            beforeDoDamageHp = target.hp;

            hadPlating = target.plating;

            if (__instance.chompyattack == null && MainManager.BadgeIsEquipped((int)Medal.Blightfury) && !BattleControl_Ext.Instance.inAiAttack && !isFireOrPoison)
            {
                if (attacker == null)
                {
                    if (BattleControl_Ext.Instance.entityAttacking != null && BattleControl_Ext.Instance.entityAttacking.CompareTag("Player") && BattleControl_Ext.Instance.entityAttacking.animid == 2 && BattleControl_Ext.Instance.leifSkillIds.Contains(BattleControl_Ext.actionID))
                    {
                        BattleControl_Ext.Instance.DoPoison(ref target);
                    }
                }
                else
                {
                    if (attacker.Value.battleentity.CompareTag("Player") && attacker.Value.battleentity.animid == 2 && BattleControl_Ext.Instance.leifSkillIds.Contains(BattleControl_Ext.actionID))
                    {
                        BattleControl_Ext.Instance.DoPoison(ref target);
                    }
                }
            }

            if (!isFireOrPoison && !BattleControl_Ext.Instance.firstHitMulti && MainManager.BadgeIsEquipped((int)Medal.IgnitedMite) && MainManager.HasCondition(MainManager.BattleCondition.Fire, target) > -1)
            {
                if (__instance.chompyattack == null && (BattleControl_Ext.actionID == 18 || BattleControl_Ext.actionID == 2) && BattleControl_Ext.Instance.entityAttacking != null && BattleControl_Ext.Instance.entityAttacking.CompareTag("Player") && BattleControl_Ext.Instance.entityAttacking.animid == 0)
                {
                    BattleControl_Ext.Instance.firstHitMulti = true;
                }
                damageammount += 1;
            }

            if (attacker != null && !isFireOrPoison && __instance.chompyattack == null && BattleControl_Ext.Instance.entityAttacking.CompareTag("Player"))
            {
                damageammount += BattleControl_Ext.Instance.CheckTeamGleam();
                damageammount += BattleControl_Ext.Instance.CheckOddWarrior(__instance, attacker.Value);
                damageammount += BattleControl_Ext.Instance.CheckKineticEnergy(attacker.Value);
            }

            if (!__instance.enemy && !isFireOrPoison && __instance.chompyattack == null && BattleControl_Ext.Instance.entityAttacking != null)
            {
                BattleControl_Ext.Instance.attackedThisTurn.Add(BattleControl_Ext.Instance.entityAttacking.battleid);
            }

            //Smearcharge check
            if(BattleControl_Ext.Instance.targetIsPlayer && attacker != null  && !isFireOrPoison && MainManager.HasCondition(MainManager.BattleCondition.Inked, attacker.Value) > -1 && MainManager.BadgeIsEquipped((int)Medal.Smearcharge, target.trueid)) 
            {
                entityExt.smearchargeActive = true;
            }

            return true;
        }

        static void Postfix(BattleControl __instance, MethodBase __originalMethod, MainManager.BattleData? attacker, ref MainManager.BattleData target, ref int damageammount, bool block, int __result)
        {
            if (!isFireOrPoison && BattleControl_Ext.Instance.targetIsPlayer && target.hp - __result > 0 && MainManager.BadgeIsEquipped((int)Medal.FlashFreeze, target.trueid) && target.hp > 4 && !isFireOrPoison && MainManager.HasCondition(MainManager.BattleCondition.Sturdy, target) == -1)
            {
                MainManager.RemoveCondition(MainManager.BattleCondition.Freeze, target);
                MainManager.SetCondition(MainManager.BattleCondition.Freeze, ref target, 3);
                target.battleentity.inice = true;
                target.weakness = new List<BattleControl.AttackProperty>(new BattleControl.AttackProperty[] { BattleControl.AttackProperty.HornExtraDamage });
                target.battleentity.Freeze();
            }

            if (MainManager.BadgeIsEquipped((int)Medal.Perkfectionist) && !__instance.enemy && beforeDoDamageHp - __result == 0 && __result != 0 && beforeDoDamageHp != 0)
            {
                BattleControl_Ext.Instance.perfectKill = true;
                BattleControl_Ext.Instance.perfectKillAmount++;
            }
            var entityExt = Entity_Ext.GetEntity_Ext(target.battleentity);
            if (attacker != null && entityExt.vitiation && BattleControl_Ext.Instance.realDamage - __result > 0)
            {
                int enemyID = attacker.Value.battleentity.battleid;
                int vitiationDamage = BattleControl_Ext.Instance.realDamage - __result;

                if (entityExt.slugskinActive)
                    vitiationDamage++;

                entityExt.vitiationDmg += vitiationDamage;

                if(entityExt.vitiationDmg > Entity_Ext.MAX_VITIATION_DMG)
                {
                    entityExt.vitiation = false;
                    target.battleentity.shieldenabled = false;
                }
                BattleControl_Ext.Instance.DoFakeDamage(enemyID, vitiationDamage);
                BattleControl_Ext.Instance.realDamage = 0;
            }

            if (MainManager.BadgeIsEquipped((int)Medal.Mothflower) && BattleControl_Ext.Instance.targetIsPlayer && block && attacker != null && !isFireOrPoison && target.trueid == 2)
            {
                BattleControl_Ext.Instance.mothFlowerHits++;

                if (MainManager.BadgeIsEquipped((int)MainManager.BadgeTypes.SuperBlock, target.trueid) && superBlocked)
                    BattleControl_Ext.Instance.mothFlowerHits += MainManager.BadgeHowManyEquipped((int)MainManager.BadgeTypes.SuperBlock,target.trueid);

                if (BattleControl_Ext.Instance.mothFlowerHits >= 3)
                {
                    BattleControl_Ext.Instance.mothFlowerHits -= 3;
                    int enemyID = attacker.Value.battleentity.battleid;
                    int damage = 2;

                    if (entityExt.slugskinActive && superBlocked)
                        damage++;

                    MainManager.PlayParticle("mothicenormal", __instance.enemydata[enemyID].battleentity.transform.position + __instance.enemydata[enemyID].battleentity.height * Vector3.up, 1.5f);
                    MainManager.PlaySound("IceMothHit", 1.1f, 1f);
                    BattleControl_Ext.Instance.DoFakeDamage(enemyID, damage);
                    if (MainManager.BadgeIsEquipped((int)Medal.Blightfury))
                    {
                        BattleControl_Ext.Instance.DoPoison(ref __instance.enemydata[enemyID]);
                    }
                }
            }

            if (BattleControl_Ext.Instance.targetIsPlayer && target.hp > 0)
            {
                if (__result > 0)
                    BattleControl_Ext.Instance.PotentialEnergyCheck(ref target);
            }

            if (!isFireOrPoison)
            {
                //Nerfs Bubble shield by only allowing to block 1 attack.

                int zommothId = battle.EnemyInField((int)MainManager.Enemies.Zommoth);
                if (MainManager.HasCondition(MainManager.BattleCondition.Shield, target) > -1 && !(MainManager.lastevent == 182 && zommothId != -1 && battle.enemydata[zommothId].data != null & battle.enemydata[zommothId].data.Length >=0 && battle.enemydata[zommothId].data[0] == 0))
                {
                    MainManager.RemoveCondition(MainManager.BattleCondition.Shield, target);
                    target.battleentity.shieldenabled = false;
                }

                if (BattleControl_Ext.Instance.targetIsPlayer && target.hp > 0)
                {
                    if (MainManager.BadgeIsEquipped((int)Medal.Slugskin, target.trueid) & superBlocked && MainManager.HasCondition(MainManager.BattleCondition.Sticky, target) != -1)
                    {
                        entityExt.CreateSlugskin();
                        entityExt.slugskinActive = true;
                        MainManager.PlaySound("Shield", 1.4f, 1);
                    }

                    if (!superBlocked)
                        entityExt.slugskinActive = false;
                }

                if (!BattleControl_Ext.Instance.targetIsPlayer)
                {
                    BattleControl_Ext.Instance.CheckStrikeBlasters(__instance, target, beforeDoDamageHp);
                }

                if (!__instance.enemy && !BattleControl_Ext.Instance.targetIsPlayer && __result > BattleControl_Ext.Instance.trustFallDamage)
                {
                    if (__instance.turns == BattleControl_Ext.Instance.trustFallTurn + 1 && BattleControl_Ext.Instance.trustFallTurn != -1)
                        BattleControl_Ext.Instance.trustFallDamage = __result;
                }

                if(__instance.chompyattack == null)
                {
                    BattleControl_Ext.Instance.DoInkWellCheck(__result, ref target);
                    BattleControl_Ext.Instance.DoWebsheetCheck(attacker, ref target);
                }
            }


            if (BattleControl_Ext.Instance.targetIsPlayer && __result > 0 && MainManager.BadgeIsEquipped((int)Medal.NoPainNoGain))
            {
                MainManager.PlaySound("Heal2");
                MainManager.instance.tp = Mathf.Clamp(MainManager.instance.tp + 1, 0, MainManager.instance.maxtp);
                __instance.ShowDamageCounter(2, 1, target.battleentity.transform.position + target.cursoroffset + Vector3.up, target.battleentity.transform.position + target.cursoroffset + Vector3.up * 2);
            }
        }
    }

    [HarmonyPatch(typeof(BattleControl), "CalculateBaseDamage")]
    public class PatchBattleControlCalculateBaseDamage
    {
        static void Prefix(BattleControl __instance, MainManager.BattleData? attacker, ref MainManager.BattleData target, bool block, ref int basevalue)
        {
            BattleControl_Ext.Instance.realDamage = basevalue;

            if (BattleControl_Ext.Instance.targetIsPlayer)
            {
                if (attacker != null)
                {
                    BattleControl_Ext.Instance.realDamage -= attacker.Value.tired;
                }

                if(MainManager.HasCondition(MainManager.BattleCondition.Sticky, target)> -1 && (block || __instance.superblockedthisframe > 0f) && MainManager.BadgeIsEquipped((int)Medal.ThickSilk, target.trueid))
                {
                    basevalue--;
                }

                if (Entity_Ext.GetEntity_Ext(target.battleentity).slugskinActive)
                    basevalue--;
            }
        }

        static void Postfix(BattleControl __instance, MainManager.BattleData? attacker, ref MainManager.BattleData target, ref bool superguarded)
        {
            if (BattleControl_Ext.Instance.inEndOfTurnDamage)
            {
                target.cantmove = 1;
            }

            if(BattleControl_Ext.Instance.targetIsPlayer)
            {
                BattleControl_Ext.Instance.DoLoomLegsCheck(ref target, superguarded);
                if (MainManager.HasCondition(MainManager.BattleCondition.Sticky, target) > -1)
                {
                    BattleControl_Ext.Instance.DoHoneyWebCheck(ref target, superguarded);
                }
            }
        }
    }

    [HarmonyPatch(typeof(BattleControl), "TryCondition")]
    public class PatchBattleControlTryCondition
    {
        static bool Prefix(BattleControl __instance, ref MainManager.BattleData data, MainManager.BattleCondition condition)
        {
            if(BattleControl_Ext.Instance.IsStatusImmune(data, condition))
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(BattleControl), "ReturnToMainSelect")]
    public class PatchBattleControlReturnToMainSelect
    {
        static bool Prefix(BattleControl __instance)
        {
            if (BattleControl_Ext.Instance.gourmetItemUse >= 0)
            {
                if (BattleControl_Ext.Instance.gourmetItemUse == 1)
                {
                    MainManager.instance.tp = Mathf.Clamp(MainManager.instance.tp + MainManager_Ext.GetDoubleDipCost(), 0, MainManager.instance.maxtp);
                }
                else
                {
                    __instance.StartCoroutine(WaitForDestroyListGourmet());
                    return false;
                }
            }

            BattleControl_Ext.Instance.gourmetItemUse = -1;
            return true;
        }

        static IEnumerator WaitForDestroyListGourmet()
        {
            yield return new WaitUntil(() => BattleControl_Ext.Instance.destroyedList);
            MainManager.PlaySound("Cancel", 10);
            BattleControl_Ext.Instance.GoToItemList();
        }
    }

    [HarmonyPatch(typeof(BattleControl), "AddNewEnemy", new Type[] {typeof(int), typeof(EntityControl)})]
    public class PatchBattleControlAddNewEnemy
    {
        public static void CheckHoloEnemy(Transform enemy)
        {
            if (MainManager.instance.flags[162] && enemy != null)
            {
                enemy.GetComponent<EntityControl>().hologram = MainManager_Ext.IsHolo();
            }
        }

        static void Postfix(BattleControl __instance, ref Transform __result)
        {
            CheckHoloEnemy(__result);
        }
    }

    [HarmonyPatch(typeof(BattleControl), "AddNewEnemy", new Type[] { typeof(int), typeof(Vector3) })]
    public class PatchBattleControlAddNewEnemy2
    {
        static void Postfix(BattleControl __instance, ref Transform __result)
        {
            PatchBattleControlAddNewEnemy.CheckHoloEnemy(__result);
        }
    }

    [HarmonyPatch(typeof(BattleControl), "UndergroundCheck")]
    public class PatchBattleControlUndergroundCheck
    {
        static void Postfix(BattleControl __instance, BattleControl.BattlePosition pos, int attackid, ref bool __result)
        {
            if(pos == BattleControl.BattlePosition.Underground && attackid == (int)NewSkill.Steal)
            {
                __result = true;
            }
        }
    }
}



