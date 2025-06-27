using BFPlus.Extensions;
using BFPlus.Patches.BattleControlTranspilers.AddExperiencePatches;
using BFPlus.Patches.DoActionPatches;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Patches.MainManagerTranspilers
{

    public class PatchSetVariableNewData : PatchBaseMainManagerSetVariables
    {
        public PatchSetVariableNewData()
        {
            priority = 0;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            var patchers = new DataPatcher[]
            {
                new DataPatcher(){loader = OpCodes.Ldloc_0, name = "Items", foundString = "/Items", setter = OpCodes.Stloc_0},
                new DataPatcher(){loader = OpCodes.Ldloc_1, name = "ItemData", foundString = "Data/ItemData", setter = OpCodes.Stloc_1},
                new DataPatcher(){loader = OpCodes.Ldloc_0, name = "SkillsName", foundString = "/Skills", setter = OpCodes.Stloc_0},
                new DataPatcher(){loader = OpCodes.Ldloc_1, name = "SkillData", foundString = "Data/SkillData", setter = OpCodes.Stloc_1},
                new DataPatcher(){loader = OpCodes.Ldloc_0, name = "BadgeName", foundString = "/BadgeName", setter = OpCodes.Stloc_0},
                new DataPatcher(){loader = OpCodes.Ldloc_1, name = "BadgeData", foundString = "Data/BadgeData", setter = OpCodes.Stloc_1 },
                new DataPatcher(){loader = OpCodes.Ldloc_0, name = "BadgeOrder", foundString = "Data/BadgeOrder", setter = OpCodes.Stloc_0, completeReplace = OpCodes.Ldc_I4_1},
                new DataPatcher(){loader = OpCodes.Ldloc_0, name = "BoardQuests", foundString = "/BoardQuests", setter = OpCodes.Stloc_0, delimiter = "["},
                new DataPatcher(){loader = OpCodes.Ldloc_1, name = "BoardData", foundString = "Data/BoardData", setter = OpCodes.Stloc_1 },
                new DataPatcher(){loader = OpCodes.Ldloc_S, name = "DiscoveryOrder", foundString = "Data/DiscoveryOrder", setter = OpCodes.Stloc_S, tattleList = true},
                new DataPatcher(){loader = OpCodes.Ldloc_0, name = "Discoveries", foundString = "/Discoveries", setter = OpCodes.Stloc_0},
                new DataPatcher(){loader = OpCodes.Ldloc_S, name = "TattleList", foundString = "Data/TattleList", setter = OpCodes.Stloc_S, completeReplace = OpCodes.Ldc_I4_1, tattleList = true },
                new DataPatcher(){loader = OpCodes.Ldloc_0, name = "EnemyTattle", foundString = "/EnemyTattle", setter = OpCodes.Stloc_0, delimiter = "[" },
                new DataPatcher(){loader = OpCodes.Ldloc_S, name = "CookOrder", foundString = "Data/CookOrder", setter = OpCodes.Stloc_S, tattleList = true, addEmpty = OpCodes.Ldc_I4_0 },
                new DataPatcher(){loader = OpCodes.Ldloc_0, name = "CookLibrary", foundString = "Data/CookLibrary", setter = OpCodes.Stloc_0 },
                new DataPatcher(){loader = OpCodes.Ldloc_S, name = "SynopsisOrder", foundString = "Data/SynopsisOrder", setter = OpCodes.Stloc_S, tattleList = true, addEmpty = OpCodes.Ldc_I4_0, completeReplace = OpCodes.Ldc_I4_1 },
                new DataPatcher(){loader = OpCodes.Ldloc_0, name = "Synopsis", foundString = "/Synopsis", setter = OpCodes.Stloc_0 },
                new DataPatcher(){loader = OpCodes.Ldloc_0, name = "EnemyTattle", foundString = "/EnemyTattle", setter = OpCodes.Stloc_0, delimiter = "[" }
            };

            foreach (var patch in patchers)
            {
                cursor.GotoNext(i => i.MatchLdstr(patch.foundString));
                cursor.GotoNext(MoveType.After, i => i.OpCode == patch.setter);

                var opRef = cursor.Prev.Operand;
                if (patch.tattleList)
                    cursor.Emit(patch.loader, opRef);
                else
                    cursor.Emit(patch.loader);
                cursor.Emit(OpCodes.Ldstr, patch.name);
                cursor.Emit(OpCodes.Ldstr, patch.delimiter);
                cursor.Emit(patch.completeReplace);
                cursor.Emit(patch.addEmpty);
                cursor.Emit(OpCodes.Call, AccessTools.Method(typeof(MainManager_Ext), "GetNewItems", new Type[] { typeof(string[]), typeof(string), typeof(string), typeof(bool), typeof(bool) }));
                if (patch.tattleList)
                    cursor.Emit(patch.setter, opRef);
                else
                    cursor.Emit(patch.setter);
            }

            cursor.Goto(0);
            cursor.GotoNext(i => i.MatchLdcI4(70));
            cursor.Next.OpCode = OpCodes.Ldc_I4;
            cursor.Next.Operand = MainManager_Ext.FlagVarNumber;
        }
    }
}
