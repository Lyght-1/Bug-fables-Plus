using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Extensions
{
    public class EnemyItemData
    {
        public static MainManager.Items[][] enemyData =
        {
            // Zombiant
            new MainManager.Items[] {MainManager.Items.CrunchyLeaf, MainManager.Items.HoneyDrop, MainManager.Items.Mushroom, MainManager.Items.AphidEgg },

            // Jellyshroom
            new MainManager.Items[] {MainManager.Items.CrunchyLeaf, MainManager.Items.HoneyDrop, MainManager.Items.Mushroom, MainManager.Items.DangerShroom },

            // Spider
            null,

            // Zasp
            null,

            // Cactiling
            new MainManager.Items[] {MainManager.Items.CrunchyLeaf, MainManager.Items.HoneyDrop, MainManager.Items.BerryJuice, MainManager.Items.ShockShroom, MainManager.Items.JellyBean },

            // Psicorp
            new MainManager.Items[] {MainManager.Items.CrunchyLeaf, MainManager.Items.RoastBerry, MainManager.Items.BerryJuice, MainManager.Items.ShockShroom },

            // Thief
            new MainManager.Items[] { (MainManager.Items)NewItem.TauntBerry, MainManager.Items.HustleSeed, (MainManager.Items)NewItem.SeedlingWhistle, (MainManager.Items)NewItem.StickyBomb,(MainManager.Items)NewItem.WebWad, (MainManager.Items)NewItem.StickySoup, (MainManager.Items)NewItem.Arachnomuffins, (MainManager.Items)NewItem.Cottoncap, (MainManager.Items)NewItem.BanditDelights },

            // Bandit
            new MainManager.Items[] {MainManager.Items.GlazedShroom, MainManager.Items.HustleSeed, (MainManager.Items)NewItem.SeedlingWhistle, (MainManager.Items)NewItem.StickyBomb,(MainManager.Items)NewItem.WebWad, (MainManager.Items)NewItem.StickySoup, (MainManager.Items)NewItem.Arachnomuffins, (MainManager.Items)NewItem.Cottoncap, (MainManager.Items)NewItem.BanditDelights  },

            // Inichas
            new MainManager.Items[] {MainManager.Items.CrunchyLeaf, MainManager.Items.VitalitySeed, MainManager.Items.Mushroom, MainManager.Items.DangerShroom },

            // Seedling
            new MainManager.Items[] {MainManager.Items.CrunchyLeaf, MainManager.Items.HoneyDrop, MainManager.Items.VitalitySeed, MainManager.Items.GenerousSeed, MainManager.Items.HardSeed },

            // Flying Seedling
            new MainManager.Items[] {MainManager.Items.CrunchyLeaf, MainManager.Items.HoneyDrop, MainManager.Items.VitalitySeed, MainManager.Items.GenerousSeed, MainManager.Items.HardSeed },

            // Maki
            null,

            // Web
            null,

            // Spider
            null,

            // Numbnail
            new MainManager.Items[] {MainManager.Items.NumbDart, MainManager.Items.GenerousSeed, MainManager.Items.AphidEgg, (MainManager.Items)NewItem.GoldenLeaf },

            // Mothiva
            null,

            // Acornling
            new MainManager.Items[] {MainManager.Items.CrunchyLeaf, MainManager.Items.HoneyDrop, MainManager.Items.VitalitySeed, MainManager.Items.GenerousSeed, MainManager.Items.HardSeed },

            // Weevil
            new MainManager.Items[] {MainManager.Items.MagicDrops, MainManager.Items.VitalitySeed, MainManager.Items.GenerousSeed, (MainManager.Items)NewItem.GoldenLeaf },

            // Mr. Tester
            null,

            // Venus' Bud
            new MainManager.Items[] {MainManager.Items.MagicDrops, MainManager.Items.RoastBerry, MainManager.Items.HardSeed, MainManager.Items.ClearWater, (MainManager.Items)NewItem.GoldenLeaf},

            // Chomper
            new MainManager.Items[] {MainManager.Items.CrunchyLeaf, MainManager.Items.PoisonDart, MainManager.Items.VitalitySeed, MainManager.Items.GenerousSeed, MainManager.Items.HardSeed, (MainManager.Items)NewItem.GoldenLeaf },

            // Acolyte Aria
            null,

            // Vine
            null,

            // Kabbu
            null,

            // Venus' Guardian
            null,

            // Wasp Trooper
            new MainManager.Items[] {MainManager.Items.ProteinShake, MainManager.Items.GlazedShroom, MainManager.Items.VitalitySeed, MainManager.Items.GenerousSeed, MainManager.Items.NumbDart, MainManager.Items.SpicyCandy, (MainManager.Items)NewItem.SeedlingWhistle },

            // Wasp Bomber
            new MainManager.Items[] {MainManager.Items.BurlyBomb, MainManager.Items.PoisonBomb, MainManager.Items.FrostBomb, MainManager.Items.NumbBomb, MainManager.Items.SleepBomb, MainManager.Items.ClearBomb },

            // Wasp Driller
            new MainManager.Items[] {MainManager.Items.SquashSoda, MainManager.Items.SpicyBomb, MainManager.Items.CookedLeaf, MainManager.Items.BurlyTea, MainManager.Items.SpicyTea, MainManager.Items.MiteBurg, MainManager.Items.BurlyChips, (MainManager.Items)NewItem.SeedlingWhistle},

            // Wasp Scout
            new MainManager.Items[] {MainManager.Items.NumbBomb, MainManager.Items.NumbDart, MainManager.Items.PoisonDart, MainManager.Items.PoisonBomb, MainManager.Items.FrenchFries, (MainManager.Items)NewItem.SeedlingWhistle },

            // Midge
            new MainManager.Items[] {MainManager.Items.CrunchyLeaf, MainManager.Items.HoneyDrop, MainManager.Items.VitalitySeed, MainManager.Items.NumbDart, MainManager.Items.JellyBean },

            // Underling
            new MainManager.Items[] {MainManager.Items.CrunchyLeaf, MainManager.Items.HoneyDrop, MainManager.Items.VitalitySeed, MainManager.Items.GenerousSeed, MainManager.Items.HardSeed, MainManager.Items.PoisonSpud },

            // Monsieur Scarlet
            null,

            // Golden Seedling
            new MainManager.Items[] {MainManager.Items.TangyBerry, MainManager.Items.TangyJam, MainManager.Items.TangyPie, MainManager.Items.TangyJuice, MainManager.Items.TangyCarpaccio, (MainManager.Items)NewItem.SeedlingWhistle },

            // Arrow Worm
            new MainManager.Items[] {MainManager.Items.PoisonSpud, MainManager.Items.Squash, MainManager.Items.ShockShroom, MainManager.Items.DryBread},

            // Carmina
            null,

            // Seedling King
            null,

            // Broodmother
            null,

            // Plumpling
            new MainManager.Items[] {MainManager.Items.PlumplingPie, MainManager.Items.SquashSoda, (MainManager.Items)NewItem.SeedlingWhistle},

            // Flowerling
            new MainManager.Items[] { (MainManager.Items)NewItem.PointSwap, MainManager.Items.RoastBerry, MainManager.Items.MushroomCandy, MainManager.Items.ClearBomb},

            // Burglar
            new MainManager.Items[] {MainManager.Items.GlazedShroom, MainManager.Items.HustleSeed, (MainManager.Items)NewItem.SeedlingWhistle, (MainManager.Items)NewItem.StickyBomb,(MainManager.Items)NewItem.WebWad, (MainManager.Items)NewItem.StickySoup, (MainManager.Items)NewItem.Arachnomuffins, (MainManager.Items)NewItem.Cottoncap, (MainManager.Items)NewItem.BanditDelights  },

            // Astotheles
            null,

            // Mother Chomper
            null,

            // Ahoneynation
            null,

            // Bee-Boop
            new MainManager.Items[] { (MainManager.Items)NewItem.BeeBattery, MainManager.Items.GlazedHoney, MainManager.Items.ShockShroom, MainManager.Items.ShockCandy, MainManager.Items.NumbDart, MainManager.Items.RoastBerry },

            // Security Turret
            new MainManager.Items[] {(MainManager.Items)NewItem.BeeBattery,MainManager.Items.GlazedHoney, MainManager.Items.ShockShroom, MainManager.Items.ShockCandy, MainManager.Items.NumbBomb, MainManager.Items.RoastBerry },

            // Denmuki
            new MainManager.Items[] { (MainManager.Items)NewItem.BeeBattery, MainManager.Items.AphidEgg, MainManager.Items.CrunchyLeaf, MainManager.Items.ShockCandy, MainManager.Items.ProteinShake, MainManager.Items.RoastBerry },

            // Heavy Drone B-33
            null,

            // Mender
            null,

            // Abomihoney
            new MainManager.Items[] {MainManager.Items.GlazedHoney, MainManager.Items.HoneyDrop, MainManager.Items.Abomihoney, MainManager.Items.Abombhoney},

            // Dune Scorpion
            null,

            // Tidal Wyrm
            null,

            // Kali
            null,

            // Zombee
            new MainManager.Items[] { (MainManager.Items)NewItem.PointSwap, MainManager.Items.Mistake,MainManager.Items.MushroomStick, MainManager.Items.Mushroom, MainManager.Items.MushroomCandy, MainManager.Items.SpicyCandy },

            // Zombeetle
            new MainManager.Items[] { (MainManager.Items)NewItem.TauntBerry, (MainManager.Items)NewItem.PointSwap, MainManager.Items.Mistake,MainManager.Items.MushroomStick, MainManager.Items.Mushroom, MainManager.Items.MushroomCandy, MainManager.Items.BurlyCandy },

            // The Watcher
            null,

            // Peacock Spider
            null,

            // Bloatshroom
            new MainManager.Items[] {MainManager.Items.MushroomStick, MainManager.Items.Mushroom, MainManager.Items.MushroomCandy, MainManager.Items.CookedShroom, MainManager.Items.GlazedShroom, MainManager.Items.FrostBomb },

            // Krawler
            new MainManager.Items[] {MainManager.Items.BerryJuice, MainManager.Items.HustleSeed, MainManager.Items.Ice },

            // Haunted Cloth
            new MainManager.Items[] {MainManager.Items.BerryJuice, MainManager.Items.HustleSeed, MainManager.Items.Ice },

            // Sand Wall
            null,

            // Ice Wall
            null,

            // Warden
            new MainManager.Items[] {MainManager.Items.BerryJuice, MainManager.Items.HustleSeed, MainManager.Items.Ice},

            // Wasp King
            null,

            // Jumping Spider
            new MainManager.Items[] {MainManager.Items.LonglegSummoner, (MainManager.Items)NewItem.Arachnomuffins, (MainManager.Items)NewItem.StickyBomb, (MainManager.Items)NewItem.WebWad, (MainManager.Items)NewItem.Cottoncap},

            // Mimic Spider
            new MainManager.Items[] {MainManager.Items.LonglegSummoner, MainManager.Items.Squash, MainManager.Items.SquashSoda},

            // Leafbug Ninja
            new MainManager.Items[] {(MainManager.Items)NewItem.InkBomb, (MainManager.Items)NewItem.LeafbugSkewer, (MainManager.Items)NewItem.InkblotGravy,(MainManager.Items)NewItem.MurkyPizza,(MainManager.Items)NewItem.InkTrap,(MainManager.Items)NewItem.SplotchScramble, },

            // Leafbug Archer
            new MainManager.Items[] {(MainManager.Items)NewItem.InkBomb, (MainManager.Items)NewItem.LeafbugSkewer, (MainManager.Items)NewItem.InkblotGravy,(MainManager.Items)NewItem.MurkyPizza,(MainManager.Items)NewItem.InkTrap,(MainManager.Items)NewItem.SplotchScramble },

            // Leafbug Clubber
            new MainManager.Items[] {(MainManager.Items)NewItem.InkBomb, (MainManager.Items)NewItem.LeafbugSkewer, (MainManager.Items)NewItem.InkblotGravy,(MainManager.Items)NewItem.MurkyPizza,(MainManager.Items)NewItem.InkTrap,(MainManager.Items)NewItem.SplotchScramble },

            // Madesphy
            new MainManager.Items[] { MainManager.Items.HustleSeed, MainManager.Items.HoneyDrop, MainManager.Items.CrunchyLeaf, MainManager.Items.VitalitySeed },

            // The Beast
            null,

            // Chomper Brute
            new MainManager.Items[] {MainManager.Items.NutCake, MainManager.Items.DryBread, MainManager.Items.BurlyChips, MainManager.Items.SpicyCandy, MainManager.Items.CoffeeCandy },

            // Mantidfly
            new MainManager.Items[] {MainManager.Items.CrunchyLeaf, MainManager.Items.HustleSeed, MainManager.Items.NumbDart, MainManager.Items.VitalitySeed },

            // General Ultimax
            null,

            // Wild Chomper
            new MainManager.Items[] {MainManager.Items.CrunchyLeaf, MainManager.Items.PoisonDart, MainManager.Items.VitalitySeed, MainManager.Items.GenerousSeed, MainManager.Items.HardSeed },

            // Cross
            null,

            // Poi
            null,

            // Primal Weevil
            null,

            // False Monarch
            null,

            // Mothfly
            new MainManager.Items[] { MainManager.Items.CrunchyLeaf, MainManager.Items.HoneyDrop, (MainManager.Items)NewItem.InkBomb, (MainManager.Items)NewItem.InkblotGravy,(MainManager.Items)NewItem.MurkyPizza,(MainManager.Items)NewItem.SplotchScramble, },

            // Mothfly Cluster
            new MainManager.Items[] { MainManager.Items.CookedLeaf, MainManager.Items.GlazedHoney, (MainManager.Items)NewItem.InkBomb, (MainManager.Items)NewItem.InkblotGravy,(MainManager.Items)NewItem.MurkyPizza,(MainManager.Items)NewItem.SplotchScramble, },

            // Ironnail
            new MainManager.Items[] {MainManager.Items.ProteinShake, MainManager.Items.GenerousSeed, MainManager.Items.AphidEgg },

            // Belostoss
            new MainManager.Items[] {MainManager.Items.ProteinShake, MainManager.Items.GenerousSeed, MainManager.Items.CookedLeaf },

            // Ruffian
            new MainManager.Items[] {MainManager.Items.SquashSoda, MainManager.Items.DryBread, MainManager.Items.CrunchyLeaf, (MainManager.Items)NewItem.SeedlingWhistle },

            // Water Strider
            new MainManager.Items[] {MainManager.Items.LonglegSummoner},

            // Diving Spider
            new MainManager.Items[] {MainManager.Items.LonglegSummoner, MainManager.Items.MagicDrops, (MainManager.Items)NewItem.Arachnomuffins, (MainManager.Items)NewItem.StickyBomb, (MainManager.Items)NewItem.WebWad, (MainManager.Items)NewItem.Cottoncap},

            // Cenn
            null,

            // Pisci
            null,

            // Dead Lander α
            null,

            // Dead Lander β
            null,

            // Dead Lander γ
            null,

            // Wasp King
            null,

            // The Everlasting King
            null,

            // Maki
            null,

            // Kina
            null,

            // Yin
            null,

            // ULTIMAX Tank
            null,

            // Zommoth
            null,

            // Riz
            null,

            // Devourer
            null,

            // Tail
            null,

            // Rock Wall
            null,

            // Ancient Key
            null,

            // Ancient Key
            null,

            // Ancient Tablet
            null,

            // Flytrap
            null,

            // FireKrawler
            new MainManager.Items[] {MainManager.Items.BerryJuice, MainManager.Items.HustleSeed, MainManager.Items.FlameRock,MainManager.Items.Guarana },

            // FireWarden
            new MainManager.Items[] {MainManager.Items.BerryJuice, MainManager.Items.HustleSeed, MainManager.Items.FlameRock, MainManager.Items.Guarana },

            // FireCape
            new MainManager.Items[] {MainManager.Items.BerryJuice, MainManager.Items.HustleSeed,  MainManager.Items.FlameRock,MainManager.Items.Guarana },

            // IceKrawler
            new MainManager.Items[] {MainManager.Items.BerryJuice, MainManager.Items.HustleSeed, MainManager.Items.Ice },

            // IceWarden
            new MainManager.Items[] {MainManager.Items.BerryJuice, MainManager.Items.HustleSeed, MainManager.Items.Ice },

            // |glitchy,1|TANGYBUG|glitchy|
            null,

            // Stratos
            null,

            // Delilah
            null,
            // HoloVi
            null,

            // HoloKabbu
            null,

            // HoloLeif
            null,
            // Vi?
            null,

            // Kabbu?
            null,

            // Leif?
            null,

            // Caveling
            new MainManager.Items[] {MainManager.Items.CrunchyLeaf, MainManager.Items.HoneyDrop, MainManager.Items.VitalitySeed, MainManager.Items.GenerousSeed, MainManager.Items.HardSeed },

            // Flying Caveling
            new MainManager.Items[] {MainManager.Items.CrunchyLeaf, MainManager.Items.HoneyDrop, MainManager.Items.VitalitySeed, MainManager.Items.GenerousSeed, MainManager.Items.HardSeed },

            // Frostfly
            new MainManager.Items[] {MainManager.Items.ShavedIce, MainManager.Items.HoneyDrop, MainManager.Items.VitalitySeed, MainManager.Items.Ice, MainManager.Items.JellyBean },

            // Pirahna Chomp
            new MainManager.Items[] {MainManager.Items.PoisonCake, MainManager.Items.PoisonDart, MainManager.Items.VitalitySeed, MainManager.Items.GenerousSeed, MainManager.Items.DangerShroom, (MainManager.Items)NewItem.GoldenLeaf },

            // Moeruki
            new MainManager.Items[] {MainManager.Items.AphidEgg, MainManager.Items.CrunchyLeaf, MainManager.Items.BurlyCandy, MainManager.Items.ProteinShake, MainManager.Items.RoastBerry, MainManager.Items.FlameRock },

            // Abomiberry
            new MainManager.Items[] { (MainManager.Items)NewItem.TauntBerry, MainManager.Items.GlazedHoney, MainManager.Items.HoneyDrop, MainManager.Items.Abomihoney, MainManager.Items.Abombhoney},

            // Splotch Spider
            new MainManager.Items[] { MainManager.Items.LonglegSummoner, (MainManager.Items)NewItem.InkBomb, (MainManager.Items)NewItem.InkblotGravy,(MainManager.Items)NewItem.MurkyPizza,(MainManager.Items)NewItem.InkTrap,(MainManager.Items)NewItem.SplotchScramble, },

            // Worm
            new MainManager.Items[] {MainManager.Items.Squash, MainManager.Items.AphidEgg, MainManager.Items.AphidMilk, (MainManager.Items)NewItem.TauntBerry},

            // Worm Swarm
            new MainManager.Items[] {MainManager.Items.Squash, MainManager.Items.AphidEgg, MainManager.Items.AphidMilk, (MainManager.Items)NewItem.TauntBerry},

            // Spineling
            new MainManager.Items[] {MainManager.Items.CrunchyLeaf, MainManager.Items.HoneyDrop, MainManager.Items.BerryJuice, MainManager.Items.PoisonDart, MainManager.Items.JellyBean, (MainManager.Items)NewItem.SeedlingWhistle },

            // Dewling
            new MainManager.Items[] {MainManager.Items.RoastBerry, MainManager.Items.MushroomCandy, MainManager.Items.ClearBomb, (MainManager.Items)NewItem.SeedlingWhistle},

            // Fire Ant
            new MainManager.Items[] {MainManager.Items.Omelet, MainManager.Items.CookedShroom, MainManager.Items.CookedLeaf, MainManager.Items.CookedJellyBean },

            // Belosslow
            null,

            // Dynamo Spore
            null,

            // Voltshroom
            new MainManager.Items[] {MainManager.Items.ShockShroom, MainManager.Items.ShockCandy, MainManager.Items.Mushroom },

            // Dull Scorp
            null,

            // Iron Suit
            null,

            // Mars
            null,

            // Mars Sprout
            null,

            // Levi
            null,

            // Celia
            null,

            //Mothmite
            new MainManager.Items[] {MainManager.Items.CrunchyLeaf, MainManager.Items.HoneyDrop, MainManager.Items.Mushroom, MainManager.Items.DangerShroom },

            //Mars Bud
            new MainManager.Items[] {MainManager.Items.SpicyBomb, MainManager.Items.BurlyBomb, MainManager.Items.CherryPie, MainManager.Items.Guarana, MainManager.Items.BerryShake},
            
            //Termite Knight
            null,

            //Leafbug Shaman
            null,
            //Jester
            null,

            //Fire Popper
            new MainManager.Items[] { MainManager.Items.BurlyCandy, MainManager.Items.ProteinShake, MainManager.Items.RoastBerry, MainManager.Items.FlameRock, (MainManager.Items)NewItem.TauntBerry },
            
            //Patton
            null,

            //LongLegs
            new MainManager.Items[] { MainManager.Items.LonglegSummoner,(MainManager.Items)NewItem.WebWad }
,
        };
    }

}
