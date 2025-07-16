using BepInEx;
using HarmonyLib;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Patches.DoActionPatches
{
    public abstract class PatchBase
    {
        public bool multiPatch;
        public bool patched;

        public MethodInfo TargetMethod { get; set; }
        public int indexInserted = -1;
        public int priority;
        protected abstract void ApplyPatch(ILCursor cursor);


        public void TryApplyPatch(ILCursor cursor)
        {
            ApplyPatch(cursor);
            if (!multiPatch)
            {
                patched = true;
            }
        }
    }

    public class PatchLoader
    {
        public static List<ILHook> hooks = new List<ILHook>();
        static List<PatchBase> LoadAllPatches(Type patchType)
        {
            var patchTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsSubclassOf(patchType) && !t.IsAbstract);

            var patches = new List<PatchBase>();
            foreach (var type in patchTypes)
            {
                patches.Add((PatchBase)Activator.CreateInstance(type));
            }

            return patches;
        }

        static void ApplyAllPatches(ILContext il, Type patchType)
        {
            List<PatchBase> patches = LoadAllPatches(patchType);
            patches = patches.OrderBy(p => p.priority).ToList();

            var cursor = new ILCursor(il);

            for (int i = 0; i < patches.Count; i++)
            {
                //Console.WriteLine($"patching {patches[i].GetType().Name}");
                patches[i].TryApplyPatch(cursor);
                if (!patches[i].patched)
                    i--;
            }
            foreach (var patch in patches)
            {
                if (patch.patched)
                {
                    if (patch.indexInserted > -1)
                    {
                        for (int i = -30; i < 30; i++)
                        {
                            if (i + patch.indexInserted >= 0 && i + patch.indexInserted < cursor.Body.Instructions.Count)
                            {
                                var instruction = cursor.Body.Instructions[i + patch.indexInserted];
                                Console.WriteLine($"{instruction.OpCode} - {instruction.Operand}");
                            }
                        }
                    }
                }
            }


        }

        public static void SetupILHook(MethodInfo targetMethod, Type patchType)
        {
            //Console.WriteLine($"setting up patch {patchType}");
            var hook = new ILHook(targetMethod, il =>
            {
                ApplyAllPatches(il, patchType);
            });
            //hooks.Add(hook);
        }
    }

    public abstract class PatchBaseGetChoiceInput : PatchBase { }

    public abstract class PatchBaseDoAction : PatchBase { }

    public abstract class PatchBaseSetText : PatchBase { }

    public abstract class PatchBaseChompy : PatchBase { }

    public abstract class PatchBaseShowItemList : PatchBase { }

    public abstract class PatchBaseStartBattle : PatchBase { }

    public abstract class PatchBaseSetItem : PatchBase { }

    public abstract class PatchBaseAddExperience : PatchBase { }

    public abstract class PatchBaseAdvanceMainTurn : PatchBase { }

    public abstract class PatchBaseAIAttack : PatchBase { }
    public abstract class PatchBaseAdvanceTurnEntity : PatchBase { }
    public abstract class PatchBaseCheckDead : PatchBase { }
    public abstract class PatchBaseTryFlee : PatchBase { }
    public abstract class PatchBaseEventDialogue : PatchBase { }
    public abstract class PatchBaseCalculateBaseDamage : PatchBase { }

    public abstract class PatchBaseUseItem : PatchBase { }
    public abstract class PatchBaseUpdateAnim : PatchBase { }

    public abstract class PatchBaseMainManagerUpdate : PatchBase { }
    public abstract class PatchBaseMainManagerGetEnemyData : PatchBase { }
    public abstract class PatchBaseStartMenuIntro : PatchBase { }
    public abstract class PatchBaseStartMenuShowSaves : PatchBase { }
    public abstract class PatchBasePauseMenuBuildWindow : PatchBase { }
    public abstract class PatchBasePauseMenuUpdate : PatchBase { }
    public abstract class PatchBaseNPCControlRefreshPlayer : PatchBase { }
    public abstract class PatchBaseNPCControlDoBehavior : PatchBase { }

    public abstract class PatchBaseMapControlCreateEntities : PatchBase { }
    public abstract class PatchBaseMainManagerLoadEssentials : PatchBase { }
    public abstract class PatchBaseMainManagerSetVariables : PatchBase { }
    public abstract class PatchBaseMainManagerLateUpdate : PatchBase { }
    public abstract class PatchBaseMainManagerCheckAchievement : PatchBase { }
    public abstract class PatchBaseMainManagerGetEXP : PatchBase { }
    public abstract class PatchBaseMainManagerLoadEntityData : PatchBase { }
    public abstract class PatchBaseMainManagerLevelUpMessage : PatchBase { }
    public abstract class PatchBaseMainManagerGetTPCost : PatchBase { }
    public abstract class PatchBaseMainManagerLoadMap : PatchBase { }
    public abstract class PatchBaseMainManagerLoad : PatchBase { }
    public abstract class PatchBaseMainManagerDoItemEffect : PatchBase { }
    public abstract class PatchBaseMainManagerGetItemUse : PatchBase { }
    public abstract class PatchBaseEvent8 : PatchBase { }
    public abstract class PatchBaseEvent71 : PatchBase { }
    public abstract class PatchBaseEvent28 : PatchBase { }
    public abstract class PatchBaseEvent197 : PatchBase { }
    public abstract class PatchBaseEvent208 : PatchBase { }
    public abstract class PatchBaseEvent85 : PatchBase { }
    public abstract class PatchBaseEvent153 : PatchBase { }
    public abstract class PatchBaseEvent127 : PatchBase { }
    public abstract class PatchBaseEvent218 : PatchBase { }
    public abstract class PatchBaseEntityControlAddModel : PatchBase { }
    public abstract class PatchBaseEntityControlAnimSpecificQuirks : PatchBase { }
    public abstract class PatchBaseEntityControlDeath : PatchBase { }
    public abstract class PatchBaseBattleControlGetEXP : PatchBase { }
    public abstract class PatchBaseBattleControlMultiSkillMove : PatchBase { }
    public abstract class PatchBaseGetMultiDamage : PatchBase { }
    public abstract class PatchBaseBattleControlUpdateText : PatchBase { }
    public abstract class PatchBaseShowDamageCounter : PatchBase { }
    public abstract class PatchBaseCounterAnimation : PatchBase { }
    public abstract class PatchBaseChargeAndAttack : PatchBase { }
    public abstract class PatchBaseClearBombEffect : PatchBase { }
    public abstract class PatchBaseStartMenuUpdate : PatchBase { }
    public abstract class PatchBaseStartMenuEntityBehavior : PatchBase { }
    public abstract class PatchBaseNPCControlShootProjectile : PatchBase { }
    public abstract class PatchBaseMapSetup : PatchBase { }
    public abstract class PatchBaseEvent163 : PatchBase { }
    public abstract class PatchBaseColiseumEnd : PatchBase { }
    public abstract class PatchBaseClearStatus : PatchBase { }
    public abstract class PatchBaseBattleControlUpdate : PatchBase { }
    public abstract class PatchBaseMainManagerLoopPoint : PatchBase { }
    public abstract class PatchBaseMainManagerSwitchMusic : PatchBase { }
    public abstract class PatchBaseMainManagerChangeMusic : PatchBase { }
    public abstract class PatchBaseMainManagerCheckSamira : PatchBase { }
    public abstract class PatchBaseEvent189 : PatchBase { }

    public abstract class PatchBaseMainManagerSetCondition : PatchBase { }
    public abstract class PatchBaseUpdateConditionBubbles : PatchBase { }
    public abstract class PatchBasePauseMenuUpdateText : PatchBase { }
    public abstract class PatchBaseBattleControlSetMaxOptions : PatchBase { }
    public abstract class PatchBaseBattleControlDoDamage : PatchBase { }
    public abstract class PatchBaseBattleControlRelay : PatchBase { }

    public abstract class PatchBaseEvent167 : PatchBase { }
    public abstract class PatchBaseBattleControlTryCondition : PatchBase { }
    public abstract class PatchBaseEvent83 : PatchBase { }
    public abstract class PatchBaseEvent213 : PatchBase { }
    public abstract class PatchBaseInnSleep : PatchBase { }

    public abstract class PatchBasePlayerControlDoActionTap : PatchBase { }

    public abstract class PatchBaseMainManagerMixIngredients : PatchBase { }
    public abstract class PatchBaseMainManagerPlayParticle : PatchBase { }

    public abstract class PatchBaseEvent26 : PatchBase { }
    public abstract class PatchBaseEvent59 : PatchBase { }

    public abstract class PatchBaseEntityControlUpdateAnimSpecific : PatchBase { }
    public abstract class PatchBaseNPCControlOnTriggerEnter : PatchBase { }

    public abstract class PatchBaseEvent73 : PatchBase { }
    public abstract class PatchBaseMapControlLateUpdate : PatchBase { }

    public abstract class PatchBaseEvent99 : PatchBase { }

    public abstract class PatchBasePlayerControlMovement : PatchBase { }

    public abstract class PatchBaseEvent16 : PatchBase { }

    public abstract class PatchBaseEvent204 : PatchBase { }

    public abstract class PatchBaseEvent118 : PatchBase { }

    public abstract class PatchBaseEvent162 : PatchBase { }
    public abstract class PatchBaseEvent43 : PatchBase { }

    public abstract class PatchBaseEvent148 : PatchBase { }

    public abstract class PatchBaseBattleControlInvulnerable : PatchBase { }

    public abstract class PatchBaseBattleControlReturnToOverworld : PatchBase { }

    public abstract class PatchBaseEvent194 : PatchBase { }

    public abstract class PatchBaseMainManagerRefreshSkills : PatchBase { }

    public abstract class PatchBaseEvent34 : PatchBase { }

    public abstract class PatchBaseNPCControlCheckItem : PatchBase { }

    public abstract class PatchBaseEntityControlUpdateItem : PatchBase { }

    public abstract class PatchBaseEvent65 : PatchBase { }
    public abstract class PatchBaseMainManagerTransferMap : PatchBase { }

    public abstract class PatchBaseEvent173 : PatchBase { }

    public abstract class PatchBaseEvent12 : PatchBase { }

    public abstract class PatchBaseBattleControlCheckEvent : PatchBase { }

    public abstract class PatchBaseCardGameStartCard : PatchBase { }
    public abstract class PatchBaseCardGameLoadCardData : PatchBase { }

    public abstract class PatchBaseCardGameBuildWindow : PatchBase { }

    public abstract class PatchBaseNPCControlCheckBump : PatchBase { }

    public abstract class PatchBaseBattleControlRevivePlayer : PatchBase { }

    public abstract class PatchBaseBattleControlGameover : PatchBase { }

    public abstract class PatchBaseCardGamePullCard : PatchBase { }

    public abstract class PatchBaseCardGameGetInput : PatchBase { }
    public abstract class PatchBaseCardGamePlayEnemyCards : PatchBase { }

    public abstract class PatchBaseCardGameCreateCard : PatchBase { }
}
