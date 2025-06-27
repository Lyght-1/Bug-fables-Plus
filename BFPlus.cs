using BepInEx;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using System.Reflection;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using BFPlus.Patches.DoActionPatches;
using System.Diagnostics.Eventing.Reader;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using System;
using OpCodes = Mono.Cecil.Cil.OpCodes;
using System.Diagnostics;
using BFPlus.Extensions;
using System.Linq;
namespace BFPlus
{
    [BepInPlugin("com.Lyght.BugFables.plugins.BFPlus", "BFPlus", "1.0")]
    [BepInProcess("Bug Fables.exe")]
    public class BFPlusPlugin : BaseUnityPlugin
    {
        void Awake()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //MainManager
            MethodInfo setText = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(MainManager), "SetText", new Type[] { typeof(string), typeof(int), typeof(float?), typeof(bool), typeof(bool), typeof(Vector3), typeof(Vector3), typeof(Vector2), typeof(Transform), typeof(NPCControl) }));
            PatchLoader.SetupILHook(setText, typeof(PatchBaseSetText));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MainManager), "SetCondition",new Type[] { typeof(MainManager.BattleCondition), typeof(MainManager.BattleData).MakeByRefType(), typeof(int), typeof(int) }), typeof(PatchBaseMainManagerSetCondition));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MainManager), "ShowItemList"), typeof(PatchBaseShowItemList));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MainManager), "Update"), typeof(PatchBaseMainManagerUpdate));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MainManager), "CheckSamira", new Type[] {typeof(AudioClip)}), typeof(PatchBaseMainManagerCheckSamira));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MainManager), "GetEnemyData", new Type[] { typeof(int), typeof(bool), typeof(bool) }), typeof(PatchBaseMainManagerGetEnemyData));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(MainManager), "LoadEssentials")), typeof(PatchBaseMainManagerLoadEssentials));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(MainManager), "SwitchMusic", new Type[] {typeof(AudioClip), typeof(float), typeof(int), typeof(bool)})), typeof(PatchBaseMainManagerSwitchMusic));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MainManager), "ChangeMusic", new Type[] { typeof(AudioClip), typeof(float), typeof(int), typeof(bool) }), typeof(PatchBaseMainManagerChangeMusic));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MainManager), "SetVariables"), typeof(PatchBaseMainManagerSetVariables));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MainManager), "LateUpdate"), typeof(PatchBaseMainManagerLateUpdate));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MainManager), "CheckAchievement"), typeof(PatchBaseMainManagerCheckAchievement));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MainManager), "GetEXP", new Type[] { typeof(int), typeof(int), typeof(MainManager.Enemies?) }), typeof(PatchBaseMainManagerGetEXP));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MainManager), "LoadEntityData"), typeof(PatchBaseMainManagerLoadEntityData));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(MainManager), "LevelUpMessage")), typeof(PatchBaseMainManagerLevelUpMessage));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MainManager), "GetTPCost", new Type[] { typeof(int), typeof(int), typeof(bool) }), typeof(PatchBaseMainManagerGetTPCost));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MainManager), "LoadMap",new Type[] { typeof(int) }), typeof(PatchBaseMainManagerLoadMap));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MainManager), "Load"), typeof(PatchBaseMainManagerLoad));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MainManager), "DoItemEffect"), typeof(PatchBaseMainManagerDoItemEffect));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MainManager), "GetItemUse", new Type[] { typeof(int), typeof(int) }), typeof(PatchBaseMainManagerGetItemUse));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MainManager), "LoopPoint"), typeof(PatchBaseMainManagerLoopPoint));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(MainManager), "InnSleep",new Type[] { typeof(NPCControl), typeof(Vector3?), typeof(bool), typeof(bool), typeof(Vector3?), typeof(Vector3?) })), typeof(PatchBaseInnSleep));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MainManager), "MixIngredients"), typeof(PatchBaseMainManagerMixIngredients));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MainManager), "PlayParticle", new Type[] { typeof(string), typeof(string), typeof(Vector3), typeof(Vector3), typeof(float), typeof(int) }), typeof(PatchBaseMainManagerPlayParticle));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MainManager), "RefreshSkills"), typeof(PatchBaseMainManagerRefreshSkills));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(MainManager), "TransferMap", new Type[] { typeof(int), typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(NPCControl) })), typeof(PatchBaseMainManagerTransferMap));

            //EventControl
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event65")), typeof(PatchBaseEvent65));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event34")), typeof(PatchBaseEvent34));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event213")), typeof(PatchBaseEvent213));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event167")), typeof(PatchBaseEvent167));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event189")), typeof(PatchBaseEvent189));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event8")), typeof(PatchBaseEvent8));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event71")), typeof(PatchBaseEvent71));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event28")), typeof(PatchBaseEvent28));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event197")), typeof(PatchBaseEvent197));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event208")), typeof(PatchBaseEvent208));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event85")), typeof(PatchBaseEvent85));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event153")), typeof(PatchBaseEvent153));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event163")), typeof(PatchBaseEvent163));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event127")), typeof(PatchBaseEvent127));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event218")), typeof(PatchBaseEvent218));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "ColiseumEnd")), typeof(PatchBaseColiseumEnd));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event83")), typeof(PatchBaseEvent83));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event26")), typeof(PatchBaseEvent26));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event59")), typeof(PatchBaseEvent59));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event73")), typeof(PatchBaseEvent73));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event99")), typeof(PatchBaseEvent99));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event16")), typeof(PatchBaseEvent16));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event204")), typeof(PatchBaseEvent204));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event118")), typeof(PatchBaseEvent118));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event162")), typeof(PatchBaseEvent162));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event43")), typeof(PatchBaseEvent43));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event148")), typeof(PatchBaseEvent148));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event194")), typeof(PatchBaseEvent194));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event173")), typeof(PatchBaseEvent173));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EventControl), "Event12")), typeof(PatchBaseEvent12));

            //StartMenu
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(StartMenu), "Intro")), typeof(PatchBaseStartMenuIntro));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(StartMenu), "ShowSaves"), typeof(PatchBaseStartMenuShowSaves));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(StartMenu), "Update"), typeof(PatchBaseStartMenuUpdate));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(StartMenu), "EntityBehavior"), typeof(PatchBaseStartMenuEntityBehavior));

            //PauseMenu
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(PauseMenu), "BuildWindow")), typeof(PatchBasePauseMenuBuildWindow));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(PauseMenu), "Update"), typeof(PatchBasePauseMenuUpdate));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(PauseMenu), "MapSetup"), typeof(PatchBaseMapSetup));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(PauseMenu), "UpdateText"), typeof(PatchBasePauseMenuUpdateText));

            //EntityControl
            PatchLoader.SetupILHook(AccessTools.Method(typeof(EntityControl), "UpdateItem"), typeof(PatchBaseEntityControlUpdateItem));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(EntityControl), "AddModel"), typeof(PatchBaseEntityControlAddModel));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(EntityControl), "UpdateAnimSpecific"), typeof(PatchBaseEntityControlUpdateAnimSpecific));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(EntityControl), "UpdateConditionBubbles"), typeof(PatchBaseUpdateConditionBubbles));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(EntityControl), "AnimSpecificQuirks"), typeof(PatchBaseEntityControlAnimSpecificQuirks));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(EntityControl), "Death", new Type[] { typeof(bool) })), typeof(PatchBaseEntityControlDeath));

            //NPCControl 
            PatchLoader.SetupILHook(AccessTools.Method(typeof(NPCControl), "RefreshPlayer"), typeof(PatchBaseNPCControlRefreshPlayer));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(NPCControl), "DoBehavior", new Type[] { typeof(NPCControl.ActionBehaviors).MakeByRefType(), typeof(float) }), typeof(PatchBaseNPCControlDoBehavior));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(NPCControl), "ChargeAndAttack")), typeof(PatchBaseChargeAndAttack));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(NPCControl), "ShootProjectile")), typeof(PatchBaseNPCControlShootProjectile));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(NPCControl), "OnTriggerEnter"), typeof(PatchBaseNPCControlOnTriggerEnter));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(NPCControl), "CheckItem")), typeof(PatchBaseNPCControlCheckItem));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(NPCControl), "CheckBump"), typeof(PatchBaseNPCControlCheckBump));


            //MapControl
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MapControl), "CreateEntities"), typeof(PatchBaseMapControlCreateEntities));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(MapControl), "LateUpdate"), typeof(PatchBaseMapControlLateUpdate));


            //PlayerControl
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(PlayerControl), "DoActionTap")), typeof(PatchBasePlayerControlDoActionTap));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(PlayerControl), "Movement"), typeof(PatchBasePlayerControlMovement));


            //CardGame
            /*PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(CardGame), "StartCard")), typeof(PatchBaseCardGameStartCard));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(CardGame), "LoadCardData"), typeof(PatchBaseCardGameLoadCardData));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(CardGame), "BuildWindow")), typeof(PatchBaseCardGameBuildWindow));*/

            //BattleControl
            PatchLoader.SetupILHook(AccessTools.Method(typeof(BattleControl), "CheckEvent"), typeof(PatchBaseBattleControlCheckEvent));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BattleControl), "ReturnToOverworld")), typeof(PatchBaseBattleControlReturnToOverworld));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(BattleControl), "Invulnerable"), typeof(PatchBaseBattleControlInvulnerable));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(BattleControl), "TryCondition"), typeof(PatchBaseBattleControlTryCondition));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BattleControl), "ClearBombEffect")), typeof(PatchBaseClearBombEffect));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(BattleControl), "DoDamage", new Type[] { typeof(MainManager.BattleData?), typeof(MainManager.BattleData).MakeByRefType(), typeof(int), typeof(BattleControl.AttackProperty?), typeof(int[]), typeof(bool) }), typeof(PatchBaseBattleControlDoDamage));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(BattleControl), "GetEXP"), typeof(PatchBaseBattleControlGetEXP));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(BattleControl), "SetMaxOptions"), typeof(PatchBaseBattleControlSetMaxOptions));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(BattleControl), "ClearStatus"), typeof(PatchBaseClearStatus));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(BattleControl), "ShowDamageCounter"), typeof(PatchBaseShowDamageCounter));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(BattleControl), "MultiSkillMove"), typeof(PatchBaseBattleControlMultiSkillMove));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(BattleControl), "GetMultiDamage"), typeof(PatchBaseGetMultiDamage));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(BattleControl), "UpdateText"), typeof(PatchBaseBattleControlUpdateText));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BattleControl), "Relay")), typeof(PatchBaseBattleControlRelay));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BattleControl), "CounterAnimation")), typeof(PatchBaseCounterAnimation));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BattleControl), "Chompy")), typeof(PatchBaseChompy));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BattleControl), "StartBattle")), typeof(PatchBaseStartBattle));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(BattleControl), "SetItem"), typeof(PatchBaseSetItem));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BattleControl), "AddExperience")), typeof(PatchBaseAddExperience));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(BattleControl), "GetChoiceInput"), typeof(PatchBaseGetChoiceInput));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BattleControl), "AdvanceMainTurn")), typeof(PatchBaseAdvanceMainTurn));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BattleControl), "AIAttack")), typeof(PatchBaseAIAttack));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(BattleControl), "AdvanceTurnEntity"), typeof(PatchBaseAdvanceTurnEntity));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BattleControl), "CheckDead")), typeof(PatchBaseCheckDead));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BattleControl), "TryFlee")), typeof(PatchBaseTryFlee));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BattleControl), "EventDialogue")), typeof(PatchBaseEventDialogue));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(BattleControl), "CalculateBaseDamage"), typeof(PatchBaseCalculateBaseDamage));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BattleControl), "UseItem")), typeof(PatchBaseUseItem));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(BattleControl), "UpdateAnim", new Type[] {typeof(bool)}), typeof(PatchBaseUpdateAnim));
            PatchLoader.SetupILHook(AccessTools.Method(typeof(BattleControl), "Update"), typeof(PatchBaseBattleControlUpdate));
            PatchLoader.SetupILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BattleControl), "DoAction")), typeof(PatchBaseDoAction));
            var harmony = new Harmony("com.Lyght.BugFables.plugins.BFPlus");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds} seconds");
        }
    }
}
