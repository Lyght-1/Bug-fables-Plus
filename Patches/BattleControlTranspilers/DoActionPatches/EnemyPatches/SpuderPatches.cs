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
using Mono.Cecil;

namespace BFPlus.Patches.DoActionPatches.EnemyPatches
{
    public class PatchSpuderBubbleType : PatchBaseDoAction
    {
        public PatchSpuderBubbleType()
        {
            priority = 78588;
        }

        static GameObject GetSpuderBubbleType(EntityControl entity)
        {
            BattleControl_Ext.Instance.spuderStickyBubble = UnityEngine.Random.Range(0, 10) >= 5;
            GameObject bubble = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Objects/" + (!BattleControl_Ext.Instance.spuderStickyBubble ? "PoisonBubble" : "StickyBubble")), entity.model.GetChild(0).GetChild(0).transform.position, Quaternion.identity) as GameObject;
            if (BattleControl_Ext.Instance.spuderStickyBubble)
            {
                var spriteBounce = bubble.AddComponent<SpriteBounce>();
                spriteBounce.basescale = new Vector3(0.5f, 0.5f, 1);
                spriteBounce.frequency = 0.1f;
                spriteBounce.facecamera = true;
            }
            return bubble;
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchNewobj(out _), i => i.MatchStfld(typeof(MainManager).GetField("camtargetpos")), i => i.MatchLdarg0(), i=>i.MatchLdcI4(1));
            cursor.GotoNext(i=>i.MatchLdcI4(1));
            cursor.Emit(OpCodes.Ldc_I4_2);//Changes spuder base max bubble from 1 to 2.
            cursor.Remove();
            
            cursor.GotoNext(i => i.MatchLdstr("Prefabs/Objects/PoisonBubble"));
            int cursorIndex = cursor.Index;

            cursor.GotoNext(i => i.MatchLdfld(out _));

            var entityRef = cursor.Next.Operand;
            cursor.Goto(cursorIndex);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, entityRef);
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchSpuderBubbleType), "GetSpuderBubbleType"));
            cursor.RemoveRange(13);
        }
    }
    
    public class PatchSpuderBubbleProperty : PatchBaseDoAction
    {
        public PatchSpuderBubbleProperty()
        {
            priority = 78686;
        }

        static int GetSpuderBubbleProperty()
        {
            return BattleControl_Ext.Instance.spuderStickyBubble ? (int)BattleControl.AttackProperty.Sticky : (int)BattleControl.AttackProperty.Poison;
        }

        static string GetSpuderBubbleParticle()
        {
            if (BattleControl_Ext.Instance.spuderStickyBubble)
            {
                BattleControl_Ext.Instance.spuderStickyBubble = false;
                return "StickyGet";
            }
            return "PoisonEffect";
        }

        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i => i.MatchLdstr("Clomp"));
            cursor.GotoNext(i => i.MatchLdcI4(3));
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchSpuderBubbleProperty), "GetSpuderBubbleProperty"));
            cursor.Remove();

            cursor.GotoNext(MoveType.After,i => i.MatchLdnull());
            cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(PatchSpuderBubbleProperty), "GetSpuderBubbleParticle"));
            cursor.Remove();
        }
    }
}
