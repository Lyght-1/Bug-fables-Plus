using BFPlus.Extensions.Maps.AbandonedTower;
using BFPlus.Extensions.Maps.DeepCave;
using BFPlus.Extensions.Maps.NewPowerPlant;
using BFPlus.Extensions.Maps.PitMaps;
using BFPlus.Extensions.Maps.SandCastleDepths;
using BFPlus.Extensions.Maps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BFPlus.Extensions.EnemyAI
{
    public abstract class AI
    {
        public abstract IEnumerator DoBattleAI(EntityControl entity, int actionid);

        static private Dictionary<NewEnemies, Type> EnemyTypeToAI = new Dictionary<NewEnemies, Type>()
        {
            {NewEnemies.DarkVi, typeof(DarkViAI) },
            {NewEnemies.DarkKabbu, typeof(DarkKabbuAI) },
            {NewEnemies.DarkLeif, typeof(DarkLeifAI) },
            {NewEnemies.Worm, typeof(WormAI) },
            {NewEnemies.WormSwarm, typeof(WormAI) },
            {NewEnemies.Dewling, typeof(DewlingAI) },
            {NewEnemies.FireAnt, typeof(FireAntAI) },
            {NewEnemies.Belosslow, typeof(BelosslowAI) },
            {NewEnemies.DynamoSpore, typeof(DynamoSporeAI) },
            {NewEnemies.BatteryShroom, typeof(BatteryShroomAI) },
            {NewEnemies.DullScorp, typeof(DullScorpAI) },
            {NewEnemies.Mars, typeof(MarsAI) },
            {NewEnemies.MarsSprout, typeof(MarsSproutAI) },
            {NewEnemies.Levi, typeof(LeviAI) },
            {NewEnemies.Celia, typeof(CeliaAI) },
            {NewEnemies.Mothmite, typeof(MothmiteAI) },
            {NewEnemies.MarsBud, typeof(MarsBudAI) },
            {NewEnemies.TermiteKnight, typeof(TermiteKnightAI) },
            {NewEnemies.LeafbugShaman, typeof(LeafbugShamanAI) },
            {NewEnemies.Jester, typeof(JesterAI) },
            {NewEnemies.IronSuit, typeof(IronSuitAI) },
            {NewEnemies.FirePopper, typeof(FirePopperAI) },
            {NewEnemies.Patton, typeof(PattonAI) },
            {NewEnemies.LonglegsSpider, typeof(LongLegsSpiderAI) },
        };

        public static AI GetAI(NewEnemies enemyType)
        {
            if (EnemyTypeToAI.TryGetValue(enemyType, out var enemyClassType))
            {
                return (AI)Activator.CreateInstance(enemyClassType);
            }
            throw new ArgumentException($"No enemy ai class found for enemy type {enemyType}");
        }

        public static bool HasCustomAI(NewEnemies enemyType)
        {
            return EnemyTypeToAI.ContainsKey(enemyType);    
        }
    }
}
