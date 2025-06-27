using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngineInternal;
using BFPlus.Extensions;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace BFPlus.Patches.DoActionPatches.EnemyPatches
{
    public class PatchMoerukiDMG : PatchBaseDoAction
    {
        public PatchMoerukiDMG()
        {
            priority = 72143;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdcI4(368));

            cursor.GotoNext(MoveType.After, i => i.MatchLdfld(AccessTools.Field(typeof(BattleControl), "playertargetID")));

            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchMoerukiDMG), "GetMoerukiDamage"));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchMoerukiDMG), "GetMoerukiProperty"));
            cursor.RemoveRange(3);

            cursor.GotoNext(i => i.MatchLdstr("ElecFast"));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchMoerukiDMG), "GetMoerukiAtkParticles"));
            cursor.Remove();

            cursor.GotoNext(i => i.MatchLdstr("Shock"));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchMoerukiDMG), "GetMoerukiAttackSound"));
            cursor.Remove();

            cursor.GotoNext(MoveType.After, i => i.MatchLdcI4(30), i=>i.MatchBr(out _));
            cursor.GotoNext(i => i.MatchLdarg0());

            var label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchMoerukiDMG), "CheckMoerukiProjectile"));
            cursor.Emit(OpCodes.Brfalse, label);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchMoerukiDMG), "DoMoerukiProjectile"));
            Utils.InsertYieldReturn(cursor);
            cursor.MarkLabel(label);
        }

        static int GetMoerukiDamage()
        {
            if (MainManager.battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.Moeruki)
            {
                return 6;
            }
            return 2;
        }

        static BattleControl.AttackProperty? GetMoerukiProperty()
        {
            if (MainManager.battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.Moeruki)
            {
                return BattleControl.AttackProperty.Fire;
            }
            return BattleControl.AttackProperty.Numb;
        }

        static string GetMoerukiAtkParticles()
        {
            if (MainManager.battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.Moeruki)
            {
                return "Fire";
            }
            return "ElecFast";
        }

        static string GetMoerukiAttackSound()
        {
            if (MainManager.battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.Moeruki)
            {
                return "WaspKingMFireBall2";
            }
            return "Shock";
        }

        static IEnumerator DoMoerukiProjectile()
        {
            if (MainManager.battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.Moeruki)
            {
                var addDelayedProj = AccessTools.Method(typeof(BattleControl), "AddDelayedProjectile");
                var getRandomAvaliablePlayer = AccessTools.Method(typeof(BattleControl), "GetRandomAvaliablePlayer", new Type[] { typeof(bool) });

                var battle = MainManager.battle;
                int target = (int)getRandomAvaliablePlayer.Invoke(battle, new object[] { true });

                if (target > -1)
                {
                    var entity = MainManager.battle.enemydata[BattleControl_Ext.actionID].battleentity;
                    entity.animstate = 100;
                    MainManager.PlaySound("Charge17");
                    battle.StartCoroutine(entity.ShakeSprite(new Vector3(0.1f, 0f), 30f));
                    yield return EventControl.halfsec;
                    entity.animstate = 101;

                    Transform fireball = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Particles/Fireball"), entity.transform.position + new Vector3(-0.2f, 0.8f, -0.2f), Quaternion.identity, battle.battlemap.transform) as GameObject).transform;
                    Vector3 startPosition = fireball.position;
                    float a = 0f;
                    float b = 30f;
                    do
                    {
                        fireball.position = MainManager.SmoothLerp(startPosition, new Vector3(entity.transform.position.x - 1f, 10f), a / b);
                        a += MainManager.TieFramerate(1f);
                        yield return null;
                    }
                    while (a < b + 1f);
                    fireball.position = new Vector3(0f, 20f);

                    addDelayedProj.Invoke(battle, new object[] { fireball.gameObject, target, 6, 2, 0, BattleControl.AttackProperty.Fire, 55f, battle.enemydata[BattleControl_Ext.actionID], "WaspKingMFireBall2", "Fire", "Fall2" });
                    battle.enemydata[BattleControl_Ext.actionID].data[0] = UnityEngine.Random.Range(2, 4);
                }
            }
        }

        static bool CheckMoerukiProjectile()
        {
            bool isMoeruki = MainManager.battle.enemydata[BattleControl_Ext.actionID].animid == (int)NewEnemies.Moeruki;
            if (MainManager.battle.enemydata[BattleControl_Ext.actionID].data == null)
            {
                MainManager.battle.enemydata[BattleControl_Ext.actionID].data = new int[1];
            }

            if (isMoeruki)
            {
                MainManager.battle.enemydata[BattleControl_Ext.actionID].data[0]--;
            }

            return isMoeruki && MainManager.battle.enemydata[BattleControl_Ext.actionID].data[0] < 0;
        }
    }
}
