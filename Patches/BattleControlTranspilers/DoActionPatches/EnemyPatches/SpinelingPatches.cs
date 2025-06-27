using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BFPlus.Extensions;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace BFPlus.Patches.DoActionPatches.EnemyPatches
{
    public class Spineling
    {
        static bool GetSpineLingChance()
        {
            var battle = MainManager.battle;

            if (battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.Spineling)
            {
                return !BattleControl_Ext.Instance.spinelingFlipped;
            }
            return UnityEngine.Random.Range(0, 100) < 50;
        }

        static Sprite GetSpinelingProjectile()
        {
            var battle = MainManager.battle;
            if (battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.Spineling)
            {
                return MainManager_Ext.assetBundle.LoadAsset<Sprite>("SpinelingProjectile");
            }
            return MainManager.instance.projectilepsrites[8];
        }

        static int GetSpinelingProjectileDamage()
        {
            if (MainManager.battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.Spineling)
            {
                int damageProj = 5;
                var battle = MainManager.battle;
                var addDelayedProj = AccessTools.Method(typeof(BattleControl), "AddDelayedProjectile");
                var getRandomAvaliablePlayer = AccessTools.Method(typeof(BattleControl), "GetRandomAvaliablePlayer", new Type[] { typeof(bool) });

                int target = (int)getRandomAvaliablePlayer.Invoke(battle, new object[] { true });

                if (target > -1)
                {
                    GameObject delProj = new GameObject();
                    delProj.transform.position = new Vector3(battle.enemydata[BattleControl_Ext.actionID].battleentity.transform.position.x - 1f, 10f);
                    delProj.AddComponent<SpriteRenderer>().sprite = MainManager_Ext.assetBundle.LoadAsset<Sprite>("SpinelingProjectile");
                    addDelayedProj.Invoke(battle, new object[] { delProj, target, damageProj - 3, 1, 0, BattleControl.AttackProperty.Poison, 55f, battle.enemydata[BattleControl_Ext.actionID], "PingShot", null, "Fall2" });
                }
                return damageProj;
            }
            return 2;
        }

        static int GetSpinelingSpinDamage()
        {
            if (MainManager.battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.Spineling)
            {
                return 3;
            }
            return 1;
        }

        static BattleControl.AttackProperty? GetSpinelingProperty()
        {
            if (MainManager.battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.Spineling)
            {
                return BattleControl.AttackProperty.Poison;
            }
            return BattleControl.AttackProperty.Pierce;
        }

        static int GetSpinelingSpinHits()
        {
            if (MainManager.battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.Spineling)
            {
                return 3;
            }
            return 2;
        }
    }
    public class PatchSpinelingChances : PatchBaseDoAction
    {
        public PatchSpinelingChances()
        {
            priority = 65355;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(256));
            cursor.GotoNext(i=>i.MatchLdcI4(0),i => i.MatchLdcI4(100));
            cursor.RemoveRange(4);
            var label = cursor.Next.Operand;

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(Spineling), "GetSpineLingChance"));
            cursor.Emit(OpCodes.Brfalse, label);
            cursor.Remove();
        }
    }
    
    public class PatchSpinelingProjectileType: PatchBaseDoAction
    {
        public PatchSpinelingProjectileType()
        {
            priority = 65437;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(258));

            cursor.GotoNext(i => i.MatchLdsfld(out _), i => i.MatchLdfld(out _), i => i.MatchLdcI4(8));

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(Spineling), "GetSpinelingProjectile"));
            cursor.RemoveRange(4);
        }
    }
    
    public class PatchSpinelingProjectileDMG : PatchBaseDoAction
    {
        public PatchSpinelingProjectileDMG()
        {
            priority = 65676;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(259));
            cursor.GotoNext(MoveType.After, i => i.MatchLdfld(AccessTools.Field(typeof(BattleControl), "playertargetID")));

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(Spineling), "GetSpinelingProjectileDamage"));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(Spineling), "GetSpinelingProperty"));
            cursor.RemoveRange(3);
        }
    }
    
    public class PatchSpinelingSpinHits: PatchBaseDoAction
    {

        public PatchSpinelingSpinHits()
        {
            priority = 65966;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(266));
            cursor.GotoNext(i => i.MatchLdcI4(2));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(Spineling), "GetSpinelingSpinHits"));
            cursor.Remove();       
        }
    }
   
    public class PatchSpinelingSpinAttack : PatchBaseDoAction
    {
        public PatchSpinelingSpinAttack()
        {
            priority = 65931;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(265));
            cursor.GotoNext(MoveType.After, i => i.MatchLdfld(AccessTools.Field(typeof(BattleControl), "playertargetID")));

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(Spineling), "GetSpinelingSpinDamage"));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(Spineling), "GetSpinelingProperty"));
            cursor.RemoveRange(3);
        }
    }
}
