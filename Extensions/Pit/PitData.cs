using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using UnityEngine;

namespace BFPlus.Extensions
{
    public class PitData : MonoBehaviour
    {
        int[][][] floorEnemies = new int[][][]
        {
            //floor 1-9
            new int[][]
            {
                new int[] { (int)NewEnemies.Caveling },
                new int[] { (int)NewEnemies.Caveling, (int)NewEnemies.Caveling },
                new int[] { (int)NewEnemies.Caveling, (int)MainManager.Enemies.CordycepsAnt },
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.FlyingCaveling, (int)NewEnemies.FlyingCaveling },
                new int[] { (int)NewEnemies.FlyingCaveling, (int)NewEnemies.Caveling },
                new int[] { (int)NewEnemies.FlyingCaveling},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.ArmoredPoly, (int)NewEnemies.Caveling },
                new int[] { (int)MainManager.Enemies.ArmoredPoly, (int)NewEnemies.FlyingCaveling },
                new int[] { (int)MainManager.Enemies.ArmoredPoly, (int)MainManager.Enemies.ArmoredPoly, (int)MainManager.Enemies.CordycepsAnt },
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.Caveling, (int)NewEnemies.FlyingCaveling },
                new int[] { (int)NewEnemies.Caveling, (int)MainManager.Enemies.Mushroom},
                new int[] { (int)NewEnemies.Caveling, (int)MainManager.Enemies.CordycepsAnt, (int)MainManager.Enemies.ArmoredPoly },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.CordycepsAnt, (int)MainManager.Enemies.ArmoredPoly },
                new int[] { (int)MainManager.Enemies.CordycepsAnt, (int)MainManager.Enemies.CordycepsAnt, (int)MainManager.Enemies.CordycepsAnt },
                new int[] { (int)MainManager.Enemies.CordycepsAnt, (int)MainManager.Enemies.Mushroom },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Mushroom , (int)MainManager.Enemies.Mushroom },
                new int[] { (int)MainManager.Enemies.Mushroom },
                new int[] { (int)MainManager.Enemies.Mushroom, (int)NewEnemies.FlyingCaveling, (int)NewEnemies.FlyingCaveling },
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.FlyingCaveling, (int)MainManager.Enemies.Mushroom, (int)MainManager.Enemies.ArmoredPoly },
                new int[] { (int)NewEnemies.FlyingCaveling, (int)NewEnemies.FlyingCaveling, (int)NewEnemies.FlyingCaveling },
                new int[] { (int)NewEnemies.FlyingCaveling, (int)MainManager.Enemies.CordycepsAnt, (int)MainManager.Enemies.CordycepsAnt, (int)MainManager.Enemies.Mushroom },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.ArmoredPoly, (int)NewEnemies.Caveling, (int)NewEnemies.Caveling },
                new int[] { (int)MainManager.Enemies.ArmoredPoly, (int)MainManager.Enemies.ArmoredPoly, (int)MainManager.Enemies.ArmoredPoly, (int)MainManager.Enemies.ArmoredPoly },
                new int[] { (int)MainManager.Enemies.ArmoredPoly, (int)MainManager.Enemies.Mushroom}, },

            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Mushroom, (int)NewEnemies.FlyingCaveling, (int)NewEnemies.FlyingCaveling, (int)NewEnemies.Caveling},
                new int[] { (int)MainManager.Enemies.Mushroom, (int)MainManager.Enemies.CordycepsAnt, (int)NewEnemies.Caveling },
                new int[] { (int)MainManager.Enemies.Mushroom, (int)MainManager.Enemies.Mushroom, (int)MainManager.Enemies.Mushroom, (int)MainManager.Enemies.Mushroom },
            },
            new int[][]
            {
                //reward floor
            },
            //floor 11-20
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Sneil, (int)MainManager.Enemies.Midge },
                new int[] { (int)MainManager.Enemies.Sneil, (int)MainManager.Enemies.Sneil, (int)NewEnemies.Caveling },
                new int[] { (int)MainManager.Enemies.Sneil, (int)MainManager.Enemies.ArmoredPoly, (int)NewEnemies.FlyingCaveling  },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Midge, (int)MainManager.Enemies.Midge, (int)NewEnemies.FlyingCaveling },
                new int[] { (int)MainManager.Enemies.Midge, (int)MainManager.Enemies.Midge, (int)MainManager.Enemies.Midge },
                new int[] { (int)MainManager.Enemies.Midge, (int)MainManager.Enemies.Sneil, (int)MainManager.Enemies.Midge},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Acornling, (int)NewEnemies.Caveling, (int)MainManager.Enemies.Acornling },
                new int[] { (int)MainManager.Enemies.Acornling, (int)MainManager.Enemies.Acornling, (int)MainManager.Enemies.Acornling },
                new int[] { (int)MainManager.Enemies.Acornling, (int)MainManager.Enemies.Sneil },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Underling, (int)MainManager.Enemies.Acornling, (int)NewEnemies.Caveling },
                new int[] { (int)MainManager.Enemies.Underling, (int)MainManager.Enemies.Underling, (int)NewEnemies.FlyingCaveling},
                new int[] { (int)MainManager.Enemies.Underling, (int)MainManager.Enemies.Midge, (int)MainManager.Enemies.Midge  },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Weevil, (int)MainManager.Enemies.Weevil, (int)NewEnemies.Caveling },
                new int[] { (int)MainManager.Enemies.Weevil, (int)MainManager.Enemies.Sneil},
                new int[] { (int)MainManager.Enemies.Weevil, (int)NewEnemies.Caveling, (int)MainManager.Enemies.Acornling },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Midge, (int)MainManager.Enemies.Sneil, (int)MainManager.Enemies.Acornling },
                new int[] { (int)MainManager.Enemies.Midge, (int)MainManager.Enemies.Midge, (int)MainManager.Enemies.Midge },
                new int[] { (int)MainManager.Enemies.Midge, (int)MainManager.Enemies.Weevil, (int)NewEnemies.FlyingCaveling},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Acornling, (int)NewEnemies.Caveling, (int)MainManager.Enemies.Weevil,(int)MainManager.Enemies.Weevil  },
                new int[] { (int)MainManager.Enemies.Acornling, (int)MainManager.Enemies.Sneil, (int)MainManager.Enemies.Sneil,(int)NewEnemies.FlyingCaveling  },
                new int[] { (int)MainManager.Enemies.Acornling, (int)MainManager.Enemies.Underling, (int)MainManager.Enemies.Midge  },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Sneil, (int)MainManager.Enemies.Sneil,(int)MainManager.Enemies.Sneil,(int)MainManager.Enemies.Sneil },
                new int[] { (int)MainManager.Enemies.Sneil, (int)NewEnemies.FlyingCaveling, (int)NewEnemies.Caveling, (int)NewEnemies.FlyingCaveling },
                new int[] { (int)MainManager.Enemies.Sneil, (int)MainManager.Enemies.Midge, (int)MainManager.Enemies.Midge  },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Weevil, (int)MainManager.Enemies.Weevil, (int)NewEnemies.Caveling, (int)NewEnemies.Caveling },
                new int[] { (int)MainManager.Enemies.Weevil, (int)MainManager.Enemies.Acornling, (int)MainManager.Enemies.Sneil, (int)MainManager.Enemies.Midge},
                new int[] { (int)MainManager.Enemies.Weevil, (int)MainManager.Enemies.Midge, (int)MainManager.Enemies.Midge, (int)NewEnemies.FlyingCaveling  },
            },
            new int[][]
            {
                //reward floor
            },
            //floor 21-30
            new int[][]
            {
                new int[] { (int)NewEnemies.PirahnaChomp, (int)NewEnemies.PirahnaChomp },
                new int[] { (int)NewEnemies.PirahnaChomp, (int)NewEnemies.PirahnaChomp, (int)NewEnemies.Caveling },
                new int[] { (int)NewEnemies.PirahnaChomp, (int)MainManager.Enemies.Sneil, (int)MainManager.Enemies.AngryPlant },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Pseudoscorpion, (int)NewEnemies.PirahnaChomp, (int)MainManager.Enemies.Weevil },
                new int[] { (int)MainManager.Enemies.Pseudoscorpion, (int)MainManager.Enemies.Sandworm, (int)NewEnemies.PirahnaChomp },
                new int[] { (int)MainManager.Enemies.Pseudoscorpion, (int)MainManager.Enemies.Pseudoscorpion, (int)MainManager.Enemies.Pseudoscorpion},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Sandworm, (int)NewEnemies.PirahnaChomp, (int)MainManager.Enemies.Acornling },
                new int[] { (int)MainManager.Enemies.Sandworm, (int)MainManager.Enemies.Midge, (int)MainManager.Enemies.Midge },
                new int[] { (int)MainManager.Enemies.Sandworm, (int)MainManager.Enemies.Sandworm },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Underling, (int)MainManager.Enemies.Acornling, (int)NewEnemies.Caveling, (int)NewEnemies.FlyingCaveling },
                new int[] { (int)MainManager.Enemies.Underling, (int)MainManager.Enemies.Sandworm, (int)MainManager.Enemies.Underling},
                new int[] { (int)MainManager.Enemies.Underling, (int)NewEnemies.PirahnaChomp, (int)MainManager.Enemies.AngryPlant },
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.PirahnaChomp, (int)MainManager.Enemies.Weevil,(int)NewEnemies.PirahnaChomp },
                new int[] { (int)NewEnemies.PirahnaChomp, (int)MainManager.Enemies.Sneil,(int)MainManager.Enemies.Sandworm,(int)NewEnemies.FlyingCaveling  },
                new int[] { (int)NewEnemies.PirahnaChomp, (int)MainManager.Enemies.AngryPlant, (int)MainManager.Enemies.AngryPlant },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Weevil, (int)NewEnemies.FlyingCaveling, (int)MainManager.Enemies.AngryPlant },
                new int[] { (int)MainManager.Enemies.Weevil, (int)MainManager.Enemies.Weevil, (int)NewEnemies.Caveling, (int)NewEnemies.Caveling },
                new int[] { (int)MainManager.Enemies.Weevil, (int)NewEnemies.PirahnaChomp, (int)NewEnemies.PirahnaChomp},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Sandworm, (int)MainManager.Enemies.Sandworm, (int)MainManager.Enemies.Pseudoscorpion},
                new int[] { (int)MainManager.Enemies.Sandworm, (int)MainManager.Enemies.Sneil, (int)NewEnemies.PirahnaChomp  },
                new int[] { (int)MainManager.Enemies.Sandworm, (int)MainManager.Enemies.Underling, (int)MainManager.Enemies.Sandworm  },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Pseudoscorpion, (int)MainManager.Enemies.Pseudoscorpion, (int)MainManager.Enemies.Pseudoscorpion, (int)MainManager.Enemies.Pseudoscorpion },
                new int[] { (int)MainManager.Enemies.Pseudoscorpion, (int)NewEnemies.FlyingCaveling, (int)NewEnemies.PirahnaChomp },
                new int[] { (int)MainManager.Enemies.Pseudoscorpion, (int)MainManager.Enemies.Sneil, (int)MainManager.Enemies.Midge  },
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.PirahnaChomp, (int)NewEnemies.PirahnaChomp, (int)NewEnemies.PirahnaChomp},
                new int[] { (int)NewEnemies.PirahnaChomp, (int)MainManager.Enemies.Weevil, (int)MainManager.Enemies.Weevil},
                new int[] { (int)NewEnemies.PirahnaChomp, (int)MainManager.Enemies.Sandworm, (int)MainManager.Enemies.Sandworm, (int)NewEnemies.FlyingCaveling  },
            },
            new int[][]
            {
                //reward floor
            },
            //floor 31-40
            new int[][]
            {
                new int[] { (int)NewEnemies.BatteryShroom, (int)MainManager.Enemies.BeeBot },
                new int[] { (int)NewEnemies.BatteryShroom, (int)MainManager.Enemies.Pseudoscorpion },
                new int[] { (int)NewEnemies.BatteryShroom, (int)MainManager.Enemies.ShockWorm, (int)MainManager.Enemies.Sandworm},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.ShockWorm, (int)MainManager.Enemies.BeeBot, (int)MainManager.Enemies.BeeTurret },
                new int[] { (int)MainManager.Enemies.ShockWorm, (int)MainManager.Enemies.BeeTurret, (int)NewEnemies.PirahnaChomp },
                new int[] { (int)MainManager.Enemies.ShockWorm, (int)NewEnemies.BatteryShroom},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.BeeBot, (int)MainManager.Enemies.BeeBot, (int)MainManager.Enemies.BeeBot },
                new int[] { (int)MainManager.Enemies.BeeBot, (int)MainManager.Enemies.BeeTurret, (int)MainManager.Enemies.BeeBot },
                new int[] { (int)MainManager.Enemies.BeeBot,  (int)NewEnemies.BatteryShroom, (int)MainManager.Enemies.ShockWorm},
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.BatteryShroom, (int)NewEnemies.PirahnaChomp, (int)MainManager.Enemies.BeeBot },
                new int[] { (int)NewEnemies.BatteryShroom, (int)NewEnemies.FlyingCaveling, (int)MainManager.Enemies.ShockWorm},
                new int[] { (int)NewEnemies.BatteryShroom, (int)NewEnemies.BatteryShroom },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.BeeTurret, (int)MainManager.Enemies.BeeTurret },
                new int[] { (int)MainManager.Enemies.BeeTurret, (int)MainManager.Enemies.ShockWorm, (int)MainManager.Enemies.BeeTurret},
                new int[] { (int)MainManager.Enemies.BeeTurret, (int)NewEnemies.BatteryShroom, (int)MainManager.Enemies.BeeBot },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.BeeBot, (int)NewEnemies.FlyingCaveling, (int)NewEnemies.FlyingCaveling, (int)MainManager.Enemies.BeeTurret},
                new int[] { (int)MainManager.Enemies.BeeBot, (int)NewEnemies.BatteryShroom, (int)MainManager.Enemies.BeeBot },
                new int[] { (int)MainManager.Enemies.BeeBot, (int)NewEnemies.BatteryShroom, (int)NewEnemies.BatteryShroom},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.ShockWorm, (int)MainManager.Enemies.Abomihoney, (int)NewEnemies.BatteryShroom},
                new int[] { (int)MainManager.Enemies.ShockWorm, (int)MainManager.Enemies.Abomihoney, (int)MainManager.Enemies.Abomihoney  },
                new int[] { (int)MainManager.Enemies.ShockWorm, (int)MainManager.Enemies.ShockWorm, (int)MainManager.Enemies.Abomihoney  },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.BeeTurret, (int)MainManager.Enemies.BeeTurret, (int)MainManager.Enemies.BeeTurret },
                new int[] { (int)MainManager.Enemies.BeeTurret, (int)MainManager.Enemies.ShockWorm, (int)MainManager.Enemies.BeeBot },
                new int[] { (int)MainManager.Enemies.BeeTurret, (int)MainManager.Enemies.BeeBot, (int)MainManager.Enemies.Abomihoney  },
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.BatteryShroom, (int)NewEnemies.BatteryShroom, (int)NewEnemies.BatteryShroom},
                new int[] { (int)NewEnemies.BatteryShroom, (int)MainManager.Enemies.Abomihoney, (int)NewEnemies.BatteryShroom},
                new int[] { (int)NewEnemies.BatteryShroom, (int)MainManager.Enemies.BeeTurret, (int)MainManager.Enemies.ShockWorm, (int)MainManager.Enemies.BeeBot },
            },
            new int[][]
            {
                //reward floor
            },
            //floor 41-50
            new int[][]
            {
                new int[] { (int)NewEnemies.Worm, (int)MainManager.Enemies.Sandworm, (int)MainManager.Enemies.Thief },
                new int[] { (int)NewEnemies.Worm, (int)NewEnemies.Worm },
                new int[] { (int)NewEnemies.Worm, (int)MainManager.Enemies.Bandit, (int)MainManager.Enemies.Thief},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Thief, (int)MainManager.Enemies.Bandit, (int)MainManager.Enemies.Burglar },
                new int[] { (int)MainManager.Enemies.Thief, (int)NewEnemies.Spineling,(int)MainManager.Enemies.Thief},
                new int[] { (int)MainManager.Enemies.Thief, (int)NewEnemies.Worm, (int)MainManager.Enemies.Krawler},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Bandit, (int)MainManager.Enemies.Thief, (int)MainManager.Enemies.Sandworm },
                new int[] { (int)MainManager.Enemies.Bandit, (int)NewEnemies.Worm, (int)MainManager.Enemies.Thief },
                new int[] { (int)MainManager.Enemies.Bandit,  (int)NewEnemies.Spineling, (int)MainManager.Enemies.Thief},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Krawler, (int)MainManager.Enemies.Cape, (int)MainManager.Enemies.Cape },
                new int[] { (int)MainManager.Enemies.Krawler, (int)NewEnemies.Worm, (int)MainManager.Enemies.Sandworm},
                new int[] { (int)MainManager.Enemies.Krawler, (int)MainManager.Enemies.Krawler, (int)MainManager.Enemies.CursedSkull},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.CursedSkull, (int)MainManager.Enemies.CursedSkull, (int)MainManager.Enemies.CursedSkull },
                new int[] { (int)MainManager.Enemies.CursedSkull, (int)NewEnemies.Spineling},
                new int[] { (int)MainManager.Enemies.CursedSkull, (int)NewEnemies.Worm, (int)NewEnemies.Worm, (int)MainManager.Enemies.CursedSkull },
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.Worm, (int)NewEnemies.Worm, (int)NewEnemies.Worm},
                new int[] { (int)NewEnemies.Worm, (int)MainManager.Enemies.Burglar},
                new int[] { (int)NewEnemies.Worm, (int)MainManager.Enemies.Cape, (int)MainManager.Enemies.Bandit},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Cape, (int)MainManager.Enemies.Bandit, (int)MainManager.Enemies.Cape},
                new int[] { (int)MainManager.Enemies.Cape, (int)NewEnemies.Spineling, (int)MainManager.Enemies.CursedSkull  },
                new int[] { (int)MainManager.Enemies.Cape, (int)NewEnemies.Worm, (int)NewEnemies.Worm,  (int)MainManager.Enemies.Thief },
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.Spineling, (int)MainManager.Enemies.Krawler, (int)MainManager.Enemies.Krawler },
                new int[] { (int)NewEnemies.Spineling, (int)MainManager.Enemies.Burglar, (int)NewEnemies.Worm,(int)MainManager.Enemies.Cape  },
                new int[] { (int)NewEnemies.Spineling, (int)NewEnemies.Spineling, (int)NewEnemies.Spineling },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Burglar, (int)MainManager.Enemies.Bandit,(int)MainManager.Enemies.Thief},
                new int[] { (int)MainManager.Enemies.Burglar, (int)NewEnemies.Spineling, (int)NewEnemies.Worm},
                new int[] { (int)MainManager.Enemies.Burglar, (int)MainManager.Enemies.Sandworm, (int)MainManager.Enemies.Cape, (int)MainManager.Enemies.CursedSkull },
            },
            new int[][]
            {
                //reward floor
            },
            //floor 51-60
            new int[][]
            {
                new int[] { (int)NewEnemies.Dewling, (int)MainManager.Enemies.WildChomper, (int)MainManager.Enemies.Mantidfly },
                new int[] { (int)NewEnemies.Dewling, (int)MainManager.Enemies.LeafbugClubber, (int)NewEnemies.Dewling },
                new int[] { (int)NewEnemies.Dewling, (int)NewEnemies.Spineling, (int)MainManager.Enemies.SkullCaterpillar},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.ChomperBrute, (int)NewEnemies.PirahnaChomp,(int)MainManager.Enemies.ChomperBrute },
                new int[] { (int)MainManager.Enemies.ChomperBrute, (int)NewEnemies.Dewling, (int)MainManager.Enemies.WildChomper},
                new int[] { (int)MainManager.Enemies.ChomperBrute, (int)NewEnemies.PirahnaChomp, (int)MainManager.Enemies.WildChomper},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.WildChomper, (int)MainManager.Enemies.ChomperBrute,(int)MainManager.Enemies.ChomperBrute},
                new int[] { (int)MainManager.Enemies.WildChomper, (int)MainManager.Enemies.WildChomper, (int)NewEnemies.Dewling  },
                new int[] { (int)MainManager.Enemies.WildChomper, (int)NewEnemies.PirahnaChomp,  (int)MainManager.Enemies.WildChomper },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.LeafbugNinja, (int)MainManager.Enemies.LeafbugClubber, (int)MainManager.Enemies.LeafbugArcher },
                new int[] { (int)MainManager.Enemies.LeafbugNinja, (int)MainManager.Enemies.LeafbugNinja, (int)NewEnemies.Dewling },
                new int[] { (int)MainManager.Enemies.LeafbugNinja, (int)MainManager.Enemies.SkullCaterpillar, (int)MainManager.Enemies.LeafbugClubber},
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.Dewling, (int)MainManager.Enemies.SkullCaterpillar, (int)NewEnemies.Spineling, (int)MainManager.Enemies.LeafbugArcher },
                new int[] { (int)NewEnemies.Dewling, (int)MainManager.Enemies.Mantidfly, (int)MainManager.Enemies.WildChomper},
                new int[] { (int)NewEnemies.Dewling, (int)MainManager.Enemies.LeafbugClubber, (int)MainManager.Enemies.LeafbugClubber},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.SkullCaterpillar, (int)MainManager.Enemies.SkullCaterpillar, (int)MainManager.Enemies.SkullCaterpillar },
                new int[] { (int)MainManager.Enemies.SkullCaterpillar, (int)MainManager.Enemies.SkullCaterpillar, (int)NewEnemies.Dewling},
                new int[] { (int)MainManager.Enemies.SkullCaterpillar, (int)MainManager.Enemies.LeafbugArcher, (int)MainManager.Enemies.LeafbugArcher },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Mantidfly, (int)NewEnemies.Dewling, (int)MainManager.Enemies.Mantidfly},
                new int[] { (int)MainManager.Enemies.Mantidfly, (int)MainManager.Enemies.ChomperBrute, (int)MainManager.Enemies.SkullCaterpillar},
                new int[] { (int)MainManager.Enemies.Mantidfly, (int)MainManager.Enemies.Mantidfly, (int)MainManager.Enemies.Mantidfly},
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.Dewling, (int)MainManager.Enemies.Mantidfly, (int)MainManager.Enemies.WildChomper },
                new int[] { (int)NewEnemies.Dewling, (int)MainManager.Enemies.SkullCaterpillar, (int)MainManager.Enemies.WildChomper, (int)MainManager.Enemies.WildChomper   },
                new int[] { (int)NewEnemies.Dewling, (int)NewEnemies.SplotchSpider,(int)MainManager.Enemies.Mantidfly},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.LeafbugClubber, (int)MainManager.Enemies.LeafbugNinja, (int)MainManager.Enemies.LeafbugArcher, (int)NewEnemies.Dewling},
                new int[] { (int)MainManager.Enemies.LeafbugClubber, (int)MainManager.Enemies.SkullCaterpillar, (int)MainManager.Enemies.LeafbugArcher, (int)NewEnemies.Dewling},
                new int[] { (int)MainManager.Enemies.LeafbugClubber, (int)NewEnemies.SplotchSpider, (int)MainManager.Enemies.Mantidfly, (int)NewEnemies.Dewling },
            },
            new int[][]
            {
                //reward floor
            },
            //floor 61-70
            new int[][]
            {
                new int[] { (int)NewEnemies.SplotchSpider, (int)MainManager.Enemies.Zombeetle, (int)NewEnemies.SplotchSpider },
                new int[] { (int)NewEnemies.SplotchSpider, (int)NewEnemies.WormSwarm },
                new int[] { (int)NewEnemies.SplotchSpider, (int)MainManager.Enemies.LeafbugClubber, (int)MainManager.Enemies.Zombee},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Zombeetle, (int)MainManager.Enemies.Zombeetle, (int)NewEnemies.SplotchSpider },
                new int[] { (int)MainManager.Enemies.Zombeetle, (int)MainManager.Enemies.MothflyCluster, (int)MainManager.Enemies.Zombee},
                new int[] { (int)MainManager.Enemies.Zombeetle,(int)MainManager.Enemies.MothflyCluster, (int)MainManager.Enemies.Mothfly},
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.WormSwarm, (int)MainManager.Enemies.MothflyCluster,(int)MainManager.Enemies.ChomperBrute},
                new int[] { (int)NewEnemies.WormSwarm, (int)NewEnemies.Worm, (int)NewEnemies.Dewling  },
                new int[] { (int)NewEnemies.WormSwarm, (int)NewEnemies.WormSwarm},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Zombee, (int)NewEnemies.SplotchSpider, (int)MainManager.Enemies.Mothfly },
                new int[] { (int)MainManager.Enemies.Zombee, (int)MainManager.Enemies.Zombeetle, (int)NewEnemies.SplotchSpider },
                new int[] { (int)MainManager.Enemies.Zombee, (int)MainManager.Enemies.MothflyCluster, (int)MainManager.Enemies.MothflyCluster},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.MothflyCluster, (int)MainManager.Enemies.MothflyCluster, (int)MainManager.Enemies.MothflyCluster },
                new int[] { (int)MainManager.Enemies.MothflyCluster, (int)NewEnemies.SplotchSpider, (int)MainManager.Enemies.Zombeetle},
                new int[] { (int)MainManager.Enemies.MothflyCluster, (int)NewEnemies.SplotchSpider, (int)NewEnemies.WormSwarm},
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.WormSwarm, (int)NewEnemies.Worm,(int)NewEnemies.Worm, (int)NewEnemies.SplotchSpider },
                new int[] { (int)NewEnemies.WormSwarm, (int)MainManager.Enemies.Zombeetle, (int)MainManager.Enemies.Zombeetle},
                new int[] { (int)NewEnemies.WormSwarm, (int)MainManager.Enemies.MothflyCluster, (int)NewEnemies.WormSwarm },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Zombee, (int)MainManager.Enemies.Zombee, (int)NewEnemies.SplotchSpider,(int)MainManager.Enemies.SkullCaterpillar },
                new int[] { (int)MainManager.Enemies.Zombee, (int)NewEnemies.WormSwarm, (int)NewEnemies.Worm},
                new int[] { (int)MainManager.Enemies.Zombee, (int)MainManager.Enemies.ChomperBrute, (int)MainManager.Enemies.ChomperBrute},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Zombeetle, (int)MainManager.Enemies.DivingSpider, (int)NewEnemies.SplotchSpider },
                new int[] { (int)MainManager.Enemies.Zombeetle, (int)MainManager.Enemies.DivingSpider, (int)MainManager.Enemies.DivingSpider, (int)MainManager.Enemies.Strider   },
                new int[] { (int)MainManager.Enemies.Zombeetle, (int)MainManager.Enemies.DivingSpider, (int)NewEnemies.WormSwarm},
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.SplotchSpider, (int)NewEnemies.SplotchSpider, (int)MainManager.Enemies.DivingSpider, (int)MainManager.Enemies.Strider},
                new int[] { (int)NewEnemies.SplotchSpider, (int)NewEnemies.WormSwarm, (int)MainManager.Enemies.Zombeetle},
                new int[] { (int)NewEnemies.SplotchSpider, (int)NewEnemies.WormSwarm, (int)NewEnemies.Worm, (int)NewEnemies.Dewling },
            },
            new int[][]
            {
                //reward floor
            },
            //floor 71-80
            new int[][]
            {
                new int[] { (int)NewEnemies.Frostfly, (int)NewEnemies.Frostfly, (int)MainManager.Enemies.Ironclad },
                new int[] { (int)NewEnemies.Frostfly, (int)NewEnemies.Frostfly},
                new int[] { (int)NewEnemies.Frostfly, (int)MainManager.Enemies.MothflyCluster, (int)MainManager.Enemies.MimicSpider},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Ironclad, (int)MainManager.Enemies.Ironclad, (int)NewEnemies.Frostfly },
                new int[] { (int)MainManager.Enemies.Ironclad, (int)NewEnemies.Abomiberry, (int)NewEnemies.Abomiberry},
                new int[] { (int)MainManager.Enemies.Ironclad, (int)NewEnemies.Abomiberry, (int)MainManager.Enemies.MimicSpider},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.MimicSpider, (int)MainManager.Enemies.DivingSpider,(int)NewEnemies.LonglegsSpider,(int)NewEnemies.SplotchSpider },
                new int[] { (int)MainManager.Enemies.MimicSpider, (int)NewEnemies.LonglegsSpider, (int)NewEnemies.LonglegsSpider },
                new int[] { (int)MainManager.Enemies.MimicSpider, (int)NewEnemies.WormSwarm,(int)NewEnemies.LonglegsSpider },
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.Frostfly, (int)NewEnemies.Frostfly,(int)NewEnemies.Frostfly },
                new int[] { (int)NewEnemies.Frostfly, (int)NewEnemies.WormSwarm },
                new int[] { (int)NewEnemies.Frostfly, (int)NewEnemies.Abomiberry, (int)NewEnemies.LonglegsSpider},
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.Abomiberry, (int)NewEnemies.Abomiberry, (int)NewEnemies.Abomiberry },
                new int[] { (int)NewEnemies.Abomiberry, (int)NewEnemies.Frostfly, (int)MainManager.Enemies.Ironclad},
                new int[] { (int)NewEnemies.Abomiberry, (int)MainManager.Enemies.Ironclad, (int)NewEnemies.WormSwarm},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Plumpling, (int)NewEnemies.Spineling,(int)NewEnemies.Caveling, (int)NewEnemies.Frostfly },
                new int[] { (int)MainManager.Enemies.Plumpling, (int)NewEnemies.Frostfly, (int)NewEnemies.Frostfly},
                new int[] { (int)MainManager.Enemies.Plumpling, (int)MainManager.Enemies.Plumpling, (int)NewEnemies.LonglegsSpider },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Ironclad, (int)MainManager.Enemies.Plumpling, (int)NewEnemies.SplotchSpider,(int)NewEnemies.Abomiberry },
                new int[] { (int)MainManager.Enemies.Ironclad, (int)NewEnemies.WormSwarm, (int)MainManager.Enemies.Ironclad},
                new int[] { (int)MainManager.Enemies.Ironclad, (int)MainManager.Enemies.Ironclad, (int)MainManager.Enemies.Ironclad},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.Ruffian, (int)MainManager.Enemies.Ruffian, (int)NewEnemies.Frostfly },
                new int[] { (int)MainManager.Enemies.Ruffian, (int)NewEnemies.Abomiberry, (int)NewEnemies.Abomiberry, (int)NewEnemies.Frostfly   },
                new int[] { (int)MainManager.Enemies.Ruffian, (int)NewEnemies.LonglegsSpider, (int)NewEnemies.Abomiberry},
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.Abomiberry, (int)NewEnemies.Frostfly, (int)NewEnemies.Abomiberry, (int)NewEnemies.Frostfly},
                new int[] { (int)NewEnemies.Abomiberry, (int)NewEnemies.Abomiberry, (int)NewEnemies.Abomiberry, (int)NewEnemies.Abomiberry},
                new int[] { (int)NewEnemies.Abomiberry, (int)MainManager.Enemies.Plumpling, (int)NewEnemies.Frostfly, (int)NewEnemies.LonglegsSpider },
            },
            new int[][]
            {
                //reward floor
            },
            //floor 81-90
            new int[][]
            {
                new int[] { (int)NewEnemies.Moeruki, (int)MainManager.Enemies.FireKrawler, (int)MainManager.Enemies.DeadLanderA },
                new int[] { (int)NewEnemies.Moeruki, (int)NewEnemies.Abomiberry, (int)NewEnemies.Moeruki},
                new int[] { (int)NewEnemies.Moeruki, (int)MainManager.Enemies.FireCape, (int)MainManager.Enemies.FireWarden},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.DeadLanderA, (int)MainManager.Enemies.DeadLanderB },
                new int[] { (int)MainManager.Enemies.DeadLanderA, (int)NewEnemies.Moeruki, (int)NewEnemies.Frostfly},
                new int[] { (int)MainManager.Enemies.DeadLanderA, (int)NewEnemies.Abomiberry},
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.Abomiberry, (int)NewEnemies.Abomiberry, (int)NewEnemies.Moeruki},
                new int[] { (int)NewEnemies.Abomiberry, (int)NewEnemies.Abomiberry, (int)NewEnemies.Abomiberry },
                new int[] { (int)NewEnemies.Abomiberry, (int)NewEnemies.WormSwarm },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.FireKrawler, (int)MainManager.Enemies.FireKrawler, (int)NewEnemies.Moeruki },
                new int[] { (int)MainManager.Enemies.FireKrawler, (int)MainManager.Enemies.FireWarden, (int)MainManager.Enemies.FireCape },
                new int[] { (int)MainManager.Enemies.FireKrawler, (int)NewEnemies.Moeruki, (int)NewEnemies.Abomiberry},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.DeadLanderB, (int)MainManager.Enemies.DeadLanderA, (int)MainManager.Enemies.GoldenSeedling },
                new int[] { (int)MainManager.Enemies.DeadLanderB, (int)MainManager.Enemies.DeadLanderG, (int)NewEnemies.Moeruki},
                new int[] { (int)MainManager.Enemies.DeadLanderB, (int)NewEnemies.Moeruki, (int)NewEnemies.Abomiberry},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.FireCape, (int)MainManager.Enemies.DeadLanderG, (int)NewEnemies.FireAnt },
                new int[] { (int)MainManager.Enemies.FireCape, (int)NewEnemies.FireAnt, (int)NewEnemies.Moeruki},
                new int[] { (int)MainManager.Enemies.FireCape, (int)NewEnemies.FireAnt, (int)NewEnemies.Abomiberry },
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.Abomiberry, (int)NewEnemies.FireAnt, (int)NewEnemies.Moeruki,(int)NewEnemies.Abomiberry },
                new int[] { (int)NewEnemies.Abomiberry, (int)MainManager.Enemies.DeadLanderB, (int)MainManager.Enemies.FireCape},
                new int[] { (int)NewEnemies.Abomiberry, (int)MainManager.Enemies.DeadLanderA, (int)NewEnemies.Frostfly},
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.Moeruki, (int)NewEnemies.Moeruki, (int)NewEnemies.FireAnt },
                new int[] { (int)NewEnemies.Moeruki, (int)NewEnemies.Abomiberry, (int)NewEnemies.Abomiberry, (int)NewEnemies.FireAnt   },
                new int[] { (int)NewEnemies.Moeruki, (int)MainManager.Enemies.FireKrawler, (int)NewEnemies.FireAnt, (int)MainManager.Enemies.DeadLanderA},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.DeadLanderG, (int)NewEnemies.FireAnt, (int)MainManager.Enemies.GoldenSeedling, (int)MainManager.Enemies.GoldenSeedling},
                new int[] { (int)MainManager.Enemies.DeadLanderG, (int)MainManager.Enemies.DeadLanderA, (int)MainManager.Enemies.DeadLanderB, (int)NewEnemies.FireAnt},
                new int[] { (int)MainManager.Enemies.DeadLanderG, (int)NewEnemies.FireAnt, (int)NewEnemies.Moeruki, (int)MainManager.Enemies.GoldenSeedling },
            },
            new int[][]
            {
                //reward floor
            },
            //floor 91-100
            new int[][]
            {
                new int[] { (int)NewEnemies.FireAnt, (int)NewEnemies.FireAnt, (int)NewEnemies.MarsBud },
                new int[] { (int)NewEnemies.FireAnt, (int)NewEnemies.WormSwarm, (int)MainManager.Enemies.DeadLanderG},
                new int[] { (int)NewEnemies.FireAnt, (int)MainManager.Enemies.GoldenSeedling, (int)NewEnemies.MarsBud},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.GoldenSeedling,(int)MainManager.Enemies.GoldenSeedling, (int)MainManager.Enemies.GoldenSeedling, (int)MainManager.Enemies.GoldenSeedling },
                new int[] { (int)MainManager.Enemies.GoldenSeedling, (int)NewEnemies.FireAnt, (int)NewEnemies.MarsBud, (int)NewEnemies.FireAnt},
                new int[] { (int)MainManager.Enemies.GoldenSeedling, (int)MainManager.Enemies.GoldenSeedling, (int)NewEnemies.MarsBud},
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.MarsBud, (int)MainManager.Enemies.DeadLanderG, (int)NewEnemies.MarsBud, (int)MainManager.Enemies.DeadLanderB},
                new int[] { (int)NewEnemies.MarsBud, (int)NewEnemies.FireAnt, (int)NewEnemies.WormSwarm },
                new int[] { (int)NewEnemies.MarsBud, (int)NewEnemies.Abomiberry, (int)MainManager.Enemies.DeadLanderB, (int)MainManager.Enemies.GoldenSeedling },
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.FireAnt, (int)NewEnemies.FireAnt,(int)NewEnemies.FireAnt, (int)NewEnemies.FireAnt },
                new int[] { (int)NewEnemies.FireAnt, (int)NewEnemies.MarsBud, (int)MainManager.Enemies.FireCape, (int)NewEnemies.FireAnt },
                new int[] { (int)NewEnemies.FireAnt, (int)MainManager.Enemies.DeadLanderG, (int)NewEnemies.FireAnt, (int)NewEnemies.Abomiberry},
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.DeadLanderA, (int)MainManager.Enemies.DeadLanderA, (int)MainManager.Enemies.DeadLanderA},
                new int[] { (int)MainManager.Enemies.DeadLanderA, (int)NewEnemies.FireAnt, (int)MainManager.Enemies.GoldenSeedling },
                new int[] { (int)MainManager.Enemies.DeadLanderA, (int)NewEnemies.Abomiberry, (int)NewEnemies.Abomiberry, (int)NewEnemies.Moeruki },
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.MarsBud, (int)NewEnemies.WormSwarm, (int)NewEnemies.WormSwarm},
                new int[] { (int)NewEnemies.MarsBud, (int)NewEnemies.MarsBud, (int)NewEnemies.FireAnt, (int)NewEnemies.Abomiberry },
                new int[] { (int)NewEnemies.MarsBud, (int)NewEnemies.Abomiberry, (int)NewEnemies.Abomiberry, (int)NewEnemies.Moeruki },
            }, 
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.DeadLanderB, (int)MainManager.Enemies.DeadLanderB, (int)NewEnemies.FireAnt},
                new int[] { (int)MainManager.Enemies.DeadLanderB, (int)NewEnemies.FireAnt, (int)NewEnemies.MarsBud },
                new int[] { (int)MainManager.Enemies.DeadLanderB, (int)MainManager.Enemies.GoldenSeedling },
            },
            new int[][]
            {
                new int[] { (int)MainManager.Enemies.GoldenSeedling, (int)NewEnemies.WormSwarm, (int)MainManager.Enemies.GoldenSeedling },
                new int[] { (int)MainManager.Enemies.GoldenSeedling, (int)NewEnemies.FireAnt, (int)NewEnemies.FireAnt, (int)NewEnemies.MarsBud},
                new int[] { (int)MainManager.Enemies.GoldenSeedling, (int)NewEnemies.Abomiberry, (int)NewEnemies.MarsBud, (int)NewEnemies.FireAnt},
            },
            new int[][]
            {
                new int[] { (int)NewEnemies.Caveling, (int)NewEnemies.PirahnaChomp, (int)NewEnemies.FireAnt, (int)NewEnemies.Worm },
                new int[] { (int)NewEnemies.BatteryShroom, (int)NewEnemies.SplotchSpider, (int)NewEnemies.Dewling, (int)NewEnemies.Moeruki },
                new int[] { (int)NewEnemies.WormSwarm, (int)NewEnemies.Abomiberry, (int)NewEnemies.Mothmite, (int)NewEnemies.LonglegsSpider },
                new int[] { (int)NewEnemies.MarsBud, (int)NewEnemies.FirePopper, (int)NewEnemies.Frostfly, (int)NewEnemies.Spineling }
            }
        };

        public MainManager.Items[][] floorItemDrops = new MainManager.Items[][]
        {
            //1-9
            new MainManager.Items[]{ MainManager.Items.MoneySmall, MainManager.Items.MoneyMedium, MainManager.Items.MoneyBig, MainManager.Items.AphidEgg, MainManager.Items.HoneyDrop, MainManager.Items.Mushroom, MainManager.Items.CrunchyLeaf,MainManager.Items.DangerShroom, MainManager.Items.PoisonSpud },
            //11-19
            new MainManager.Items[]{ MainManager.Items.MoneySmall, MainManager.Items.MoneyMedium, MainManager.Items.MoneyBig, MainManager.Items.AphidEgg, MainManager.Items.HoneyDrop, MainManager.Items.AphidMilk, MainManager.Items.CrunchyLeaf,MainManager.Items.DangerShroom , MainManager.Items.HardSeed, MainManager.Items.NumbDart, MainManager.Items.ClearWater},
            //21-29
            new MainManager.Items[]{ MainManager.Items.MoneySmall, MainManager.Items.MoneyMedium, MainManager.Items.MoneyBig, MainManager.Items.AphidEgg, MainManager.Items.HoneyDrop, MainManager.Items.AphidMilk, MainManager.Items.CrunchyLeaf,MainManager.Items.DangerShroom , MainManager.Items.NumbDart, MainManager.Items.JellyBean, MainManager.Items.ShockShroom, MainManager.Items.PoisonDart, MainManager.Items.MagicDrops, MainManager.Items.VitalitySeed, MainManager.Items.GenerousSeed},
            //31-39
            new MainManager.Items[]{ MainManager.Items.MoneySmall, MainManager.Items.MoneyMedium, MainManager.Items.MoneyBig, MainManager.Items.AphidEgg, MainManager.Items.HoneyDrop, MainManager.Items.AphidMilk, MainManager.Items.CrunchyLeaf,MainManager.Items.DangerShroom, MainManager.Items.NumbDart, MainManager.Items.JellyBean, MainManager.Items.ShockShroom, MainManager.Items.MagicDrops, MainManager.Items.VitalitySeed, MainManager.Items.GenerousSeed, MainManager.Items.Battery, MainManager.Items.Coffee, MainManager.Items.FrenchFries},
            //41-49
            new MainManager.Items[]{ MainManager.Items.MoneyMedium, MainManager.Items.MoneyBig, MainManager.Items.AphidEgg, MainManager.Items.HoneyDrop, MainManager.Items.AphidMilk, MainManager.Items.CrunchyLeaf, MainManager.Items.NumbDart, MainManager.Items.JellyBean, MainManager.Items.ShockShroom, MainManager.Items.MagicDrops, MainManager.Items.VitalitySeed, MainManager.Items.GenerousSeed, MainManager.Items.Coffee, MainManager.Items.FrenchFries, MainManager.Items.Ice, MainManager.Items.Omelet, MainManager.Items.HeartyBreakfast, MainManager.Items.ShavedIce, MainManager.Items.IceCream},
            //51-59
            new MainManager.Items[]{ MainManager.Items.MoneyBig, MainManager.Items.AphidMilk, MainManager.Items.GlazedHoney, MainManager.Items.LeafCroisant, MainManager.Items.NumbDart, MainManager.Items.ShockShroom, MainManager.Items.MagicDrops, MainManager.Items.Coffee, MainManager.Items.FrenchFries, MainManager.Items.Omelet, MainManager.Items.HeartyBreakfast, MainManager.Items.ShavedIce, MainManager.Items.IceCream, MainManager.Items.PoisonCake},
            //61-69
            new MainManager.Items[]{ MainManager.Items.MoneyBig, MainManager.Items.AphidMilk, MainManager.Items.GlazedHoney, MainManager.Items.LeafCroisant, MainManager.Items.NumbDart, MainManager.Items.ShockShroom, MainManager.Items.MagicDrops, MainManager.Items.Coffee, MainManager.Items.FrenchFries, MainManager.Items.Omelet, MainManager.Items.HeartyBreakfast, MainManager.Items.ShavedIce, MainManager.Items.IceCream, MainManager.Items.PoisonCake, MainManager.Items.SpicyCandy, MainManager.Items.BurlyCandy, MainManager.Items.CoffeeCandy, MainManager.Items.FrostBomb, MainManager.Items.SpicyBomb, MainManager.Items.NumbBomb, MainManager.Items.LonglegSummoner},
            //71-79
            new MainManager.Items[]{ MainManager.Items.MoneyBig, MainManager.Items.AphidMilk, MainManager.Items.GlazedHoney, MainManager.Items.LeafCroisant, MainManager.Items.CherryPie, MainManager.Items.MagicDrops, MainManager.Items.Coffee, MainManager.Items.FrenchFries, MainManager.Items.Omelet, MainManager.Items.HeartyBreakfast, MainManager.Items.ShavedIce, MainManager.Items.IceCream, MainManager.Items.PoisonCake, MainManager.Items.SpicyCandy, MainManager.Items.BurlyCandy, MainManager.Items.CoffeeCandy, MainManager.Items.FrostBomb, MainManager.Items.SpicyBomb, MainManager.Items.NumbBomb, MainManager.Items.LonglegSummoner},
            //81-89
            new MainManager.Items[]{ MainManager.Items.MoneyBig, MainManager.Items.AphidMilk, MainManager.Items.GlazedHoney, MainManager.Items.LeafCroisant, MainManager.Items.CherryPie, MainManager.Items.MagicDrops, MainManager.Items.Coffee, MainManager.Items.FrenchFries, MainManager.Items.Omelet, MainManager.Items.HeartyBreakfast, MainManager.Items.ShavedIce, MainManager.Items.IceCream, MainManager.Items.PoisonCake, MainManager.Items.SpicyCandy, MainManager.Items.BurlyCandy, MainManager.Items.CoffeeCandy, MainManager.Items.FrostBomb, MainManager.Items.SpicyBomb, MainManager.Items.NumbBomb, MainManager.Items.LonglegSummoner, MainManager.Items.TangyPie, MainManager.Items.Guarana},
            //91-99
            new MainManager.Items[]{ MainManager.Items.AphidMilk, MainManager.Items.GlazedHoney, MainManager.Items.LeafCroisant, MainManager.Items.CherryPie, MainManager.Items.MagicDrops, MainManager.Items.Coffee, MainManager.Items.FrenchFries, MainManager.Items.Omelet, MainManager.Items.HeartyBreakfast, MainManager.Items.ShavedIce, MainManager.Items.IceCream, MainManager.Items.PoisonCake, MainManager.Items.SpicyCandy, MainManager.Items.BurlyCandy, MainManager.Items.CoffeeCandy, MainManager.Items.FrostBomb, MainManager.Items.SpicyBomb, MainManager.Items.NumbBomb, MainManager.Items.LonglegSummoner, MainManager.Items.TangyPie, MainManager.Items.Guarana, MainManager.Items.KingDinner, MainManager.Items.BerrySmoothie}
        };

        public MainManager.Items[][][] shopItems = new MainManager.Items[][][]
        {
            //shop floor 10
            new MainManager.Items[][]
            {
                //normal healing HP items
                new MainManager.Items[] { MainManager.Items.Mushroom, MainManager.Items.CrunchyLeaf, MainManager.Items.AphidEgg, MainManager.Items.JellyBean },
                //good Healing HP items
                new MainManager.Items[] {MainManager.Items.RoastBerry, MainManager.Items.MagicDrops, MainManager.Items.LeafSalad, MainManager.Items.CookedLeaf, MainManager.Items.CookedShroom, MainManager.Items.GlazedShroom, MainManager.Items.Omelet, MainManager.Items.HeartyBreakfast},
                //normal tp items
                new MainManager.Items[] { MainManager.Items.HoneyDrop, MainManager.Items.DangerShroom, MainManager.Items.AphidMilk, MainManager.Items.Mistake },
                //good tp items
                new MainManager.Items[] { MainManager.Items.GlazedHoney, MainManager.Items.HoneydLeaf, MainManager.Items.CookedDanger, MainManager.Items.HoneyShroom, MainManager.Items.MushroomStick  },
                //attacking items
                new MainManager.Items[] { MainManager.Items.VitalitySeed, MainManager.Items.GenerousSeed, MainManager.Items.SpicyBomb, MainManager.Items.BurlyBomb, MainManager.Items.NumbDart, MainManager.Items.PoisonDart, MainManager.Items.HardSeed, (MainManager.Items)NewItem.SeedlingWhistle },
            },

            //shop floor 20
            new MainManager.Items[][]
            {
                //normal healing HP items
                new MainManager.Items[] { MainManager.Items.Mushroom, MainManager.Items.CrunchyLeaf, MainManager.Items.AphidEgg, MainManager.Items.JellyBean, MainManager.Items.BerryJuice, MainManager.Items.RoastBerry, MainManager.Items.BakedYam, MainManager.Items.PlainTea },
                //good Healing HP items
                new MainManager.Items[] { MainManager.Items.MagicDrops, MainManager.Items.DryBread, MainManager.Items.LeafSalad, MainManager.Items.CookedLeaf, MainManager.Items.CookedShroom, MainManager.Items.GlazedShroom, MainManager.Items.Omelet, MainManager.Items.HeartyBreakfast, MainManager.Items.FrozenSalad},
                //normal tp items
                new MainManager.Items[] { MainManager.Items.HoneyDrop, MainManager.Items.DangerShroom, MainManager.Items.AphidMilk, MainManager.Items.Mistake, MainManager.Items.ShavedIce },
                //good tp items
                new MainManager.Items[] { MainManager.Items.GlazedHoney, MainManager.Items.NutCake, MainManager.Items.HoneydLeaf, MainManager.Items.CookedDanger, MainManager.Items.HoneyShroom, MainManager.Items.MushroomStick, MainManager.Items.HoneyMilk, MainManager.Items.DrowsyCake  },
                //attacking items
                new MainManager.Items[] {MainManager.Items.Ice, MainManager.Items.VitalitySeed, MainManager.Items.GenerousSeed, MainManager.Items.SpicyBomb, MainManager.Items.BurlyBomb,MainManager.Items.SpicyTea, MainManager.Items.NumbBomb, MainManager.Items.NumbDart, MainManager.Items.PoisonDart, MainManager.Items.HardSeed, (MainManager.Items)NewItem.SeedlingWhistle },
            },
            //shop floor 30
            new MainManager.Items[][]
            {
                //normal healing HP items
                new MainManager.Items[] { MainManager.Items.CrunchyLeaf,MainManager.Items.JellyBean, MainManager.Items.BerryJuice, MainManager.Items.RoastBerry, MainManager.Items.BakedYam, MainManager.Items.PlainTea },
                //good Healing HP items
                new MainManager.Items[] { MainManager.Items.MagicDrops, MainManager.Items.LeafSalad, MainManager.Items.CookedLeaf, MainManager.Items.CookedShroom, MainManager.Items.GlazedShroom, MainManager.Items.Omelet, MainManager.Items.HeartyBreakfast, MainManager.Items.FrozenSalad, MainManager.Items.MushroomCandy, MainManager.Items.Coffee, MainManager.Items.FrenchFries},
                //normal tp items
                new MainManager.Items[] { MainManager.Items.HoneyDrop, MainManager.Items.CookedDanger, MainManager.Items.AphidMilk, MainManager.Items.ShavedIce, MainManager.Items.HoneyPancake, MainManager.Items.ShockShroom },
                //good tp items
                new MainManager.Items[] { MainManager.Items.GlazedHoney, MainManager.Items.NutCake, MainManager.Items.HoneydLeaf, MainManager.Items.Pudding, MainManager.Items.HoneyShroom, MainManager.Items.MushroomStick, MainManager.Items.HoneyMilk, MainManager.Items.DrowsyCake, MainManager.Items.SpicyCandy, MainManager.Items.BurlyCandy, MainManager.Items.ShockCandy, MainManager.Items.HoneyMilk },
                //attacking items
                new MainManager.Items[] {MainManager.Items.Ice, MainManager.Items.VitalitySeed, MainManager.Items.GenerousSeed, MainManager.Items.SpicyBomb, MainManager.Items.BurlyBomb,MainManager.Items.SpicyTea, MainManager.Items.NumbBomb, MainManager.Items.NumbDart, (MainManager.Items)NewItem.SeedlingWhistle},
            },
            //shop floor 40
            new MainManager.Items[][]
            {
                //normal healing HP items
                new MainManager.Items[] { MainManager.Items.CrunchyLeaf, MainManager.Items.JellyBean, MainManager.Items.BerryJuice, MainManager.Items.RoastBerry, MainManager.Items.BakedYam, MainManager.Items.PlainTea, MainManager.Items.Omelet },
                //good Healing HP items
                new MainManager.Items[] { MainManager.Items.MagicDrops, MainManager.Items.LeafSalad, MainManager.Items.CookedLeaf, MainManager.Items.CookedShroom, MainManager.Items.GlazedShroom, MainManager.Items.Coffee, MainManager.Items.HeartyBreakfast, MainManager.Items.FrozenSalad, MainManager.Items.MushroomCandy, MainManager.Items.Coffee, MainManager.Items.FrenchFries, MainManager.Items.BurlyChips},
                //normal tp items
                new MainManager.Items[] { MainManager.Items.HoneyDrop, MainManager.Items.CookedDanger, MainManager.Items.AphidMilk, MainManager.Items.ShavedIce, MainManager.Items.HoneyPancake, MainManager.Items.ShockShroom },
                //good tp items
                new MainManager.Items[] { MainManager.Items.GlazedHoney, MainManager.Items.NutCake, MainManager.Items.HoneydLeaf, MainManager.Items.Pudding, MainManager.Items.HoneyShroom, MainManager.Items.MushroomStick, MainManager.Items.HoneyMilk, MainManager.Items.DrowsyCake, MainManager.Items.SpicyCandy, MainManager.Items.BurlyCandy, MainManager.Items.ShockCandy, MainManager.Items.HoneyMilk, MainManager.Items.CoffeeCandy, MainManager.Items.HoneyIceCream, MainManager.Items.IceCream},
                //attacking items
                new MainManager.Items[] { (MainManager.Items)NewItem.TauntBerry,MainManager.Items.SpicyBomb, MainManager.Items.BurlyBomb,MainManager.Items.SpicyTea, MainManager.Items.ProteinShake, MainManager.Items.NumbBomb, MainManager.Items.NumbDart, (MainManager.Items)NewItem.SeedlingWhistle, MainManager.Items.SleepBomb},
            },
            //shop floor 50
            new MainManager.Items[][]
            {
                //normal healing HP items
                new MainManager.Items[] { MainManager.Items.CrunchyLeaf, MainManager.Items.JellyBean, MainManager.Items.BerryJuice, MainManager.Items.RoastBerry, MainManager.Items.BakedYam, MainManager.Items.PlainTea, MainManager.Items.Omelet, MainManager.Items.LeafSalad },
                //good Healing HP items
                new MainManager.Items[] { MainManager.Items.MagicDrops,MainManager.Items.CookedLeaf, MainManager.Items.CookedShroom, MainManager.Items.GlazedShroom, MainManager.Items.Coffee, MainManager.Items.HeartyBreakfast, MainManager.Items.FrozenSalad, MainManager.Items.MushroomCandy, MainManager.Items.Coffee, MainManager.Items.FrenchFries, MainManager.Items.BurlyChips},
                //normal tp items
                new MainManager.Items[] { MainManager.Items.HoneyDrop, MainManager.Items.CookedDanger, MainManager.Items.AphidMilk, MainManager.Items.ShavedIce, MainManager.Items.HoneyPancake, MainManager.Items.ShockShroom },
                //good tp items
                new MainManager.Items[] { MainManager.Items.GlazedHoney, MainManager.Items.NutCake, MainManager.Items.HoneydLeaf, MainManager.Items.Pudding, MainManager.Items.HoneyShroom, MainManager.Items.MushroomStick, MainManager.Items.HoneyMilk, MainManager.Items.DrowsyCake, MainManager.Items.SpicyCandy, MainManager.Items.BurlyCandy, MainManager.Items.ShockCandy, MainManager.Items.HoneyMilk, MainManager.Items.CoffeeCandy, MainManager.Items.HoneyIceCream, MainManager.Items.IceCream},
                //attacking items
                new MainManager.Items[] { (MainManager.Items)NewItem.TauntBerry,MainManager.Items.SpicyBomb, MainManager.Items.BurlyBomb,MainManager.Items.SpicyTea, MainManager.Items.ProteinShake, MainManager.Items.NumbBomb, MainManager.Items.NumbDart, (MainManager.Items)NewItem.SeedlingWhistle, MainManager.Items.SleepBomb, (MainManager.Items)NewItem.PointSwap, (MainManager.Items)NewItem.TauntBerry},
            },
            //shop floor 60
            new MainManager.Items[][]
            {
                //normal healing HP items
                new MainManager.Items[] { MainManager.Items.CrunchyLeaf, MainManager.Items.JellyBean, MainManager.Items.BerryJuice, MainManager.Items.RoastBerry, MainManager.Items.BakedYam, MainManager.Items.PlainTea, MainManager.Items.Omelet, MainManager.Items.LeafSalad },
                //good Healing HP items
                new MainManager.Items[] { MainManager.Items.MagicDrops,MainManager.Items.CookedLeaf, MainManager.Items.CookedShroom, MainManager.Items.GlazedShroom, MainManager.Items.Coffee, MainManager.Items.HeartyBreakfast, MainManager.Items.FrozenSalad, MainManager.Items.MushroomCandy, MainManager.Items.Coffee, MainManager.Items.FrenchFries, MainManager.Items.BurlyChips, MainManager.Items.KingDinner},
                //normal tp items
                new MainManager.Items[] { MainManager.Items.HoneyDrop, MainManager.Items.CookedDanger, MainManager.Items.AphidMilk, MainManager.Items.ShavedIce, MainManager.Items.HoneyPancake, MainManager.Items.ShockShroom },
                //good tp items
                new MainManager.Items[] { MainManager.Items.GlazedHoney, MainManager.Items.NutCake, MainManager.Items.HoneydLeaf, MainManager.Items.Pudding, MainManager.Items.HoneyShroom, MainManager.Items.MushroomStick, MainManager.Items.HoneyMilk, MainManager.Items.DrowsyCake, MainManager.Items.SpicyCandy, MainManager.Items.BurlyCandy, MainManager.Items.ShockCandy, MainManager.Items.HoneyMilk, MainManager.Items.CoffeeCandy, MainManager.Items.HoneyIceCream, MainManager.Items.IceCream, MainManager.Items.BerrySmoothie},
                //attacking items
                new MainManager.Items[] { MainManager.Items.SpicyBomb, MainManager.Items.BurlyBomb,MainManager.Items.ProteinShake, MainManager.Items.NumbBomb, (MainManager.Items)NewItem.SeedlingWhistle, MainManager.Items.SleepBomb, MainManager.Items.LonglegSummoner,  (MainManager.Items)NewItem.PointSwap, (MainManager.Items)NewItem.TauntBerry},
            },
            //shop floor 70
            new MainManager.Items[][]
            {
                //normal healing HP items
                new MainManager.Items[] { MainManager.Items.CrunchyLeaf, MainManager.Items.JellyBean, MainManager.Items.BerryJuice, MainManager.Items.RoastBerry, MainManager.Items.BakedYam, MainManager.Items.PlainTea, MainManager.Items.Omelet, MainManager.Items.LeafSalad },
                //good Healing HP items
                new MainManager.Items[] { MainManager.Items.MagicDrops,MainManager.Items.CookedLeaf, MainManager.Items.CookedShroom, MainManager.Items.GlazedShroom, MainManager.Items.Coffee, MainManager.Items.HeartyBreakfast, MainManager.Items.FrozenSalad, MainManager.Items.MushroomCandy, MainManager.Items.Coffee, MainManager.Items.FrenchFries, MainManager.Items.BurlyChips, MainManager.Items.KingDinner},
                //normal tp items
                new MainManager.Items[] { MainManager.Items.HoneyDrop, MainManager.Items.CookedDanger, MainManager.Items.AphidMilk, MainManager.Items.ShavedIce, MainManager.Items.HoneyPancake, MainManager.Items.ShockShroom },
                //good tp items
                new MainManager.Items[] { MainManager.Items.GlazedHoney, MainManager.Items.NutCake, MainManager.Items.HoneydLeaf, MainManager.Items.Pudding, MainManager.Items.HoneyShroom, MainManager.Items.MushroomStick, MainManager.Items.HoneyMilk, MainManager.Items.DrowsyCake, MainManager.Items.SpicyCandy, MainManager.Items.BurlyCandy, MainManager.Items.ShockCandy, MainManager.Items.HoneyMilk, MainManager.Items.CoffeeCandy, MainManager.Items.HoneyIceCream, MainManager.Items.IceCream, MainManager.Items.BerrySmoothie},
                //attacking items
                new MainManager.Items[] { MainManager.Items.SpicyBomb, MainManager.Items.BurlyBomb,MainManager.Items.ProteinShake, MainManager.Items.NumbBomb, (MainManager.Items)NewItem.SeedlingWhistle, MainManager.Items.SleepBomb, MainManager.Items.LonglegSummoner, (MainManager.Items)NewItem.PointSwap, (MainManager.Items)NewItem.TauntBerry},
            },
            //shop floor 80
            new MainManager.Items[][]
            {
                //normal healing HP items
                new MainManager.Items[] { MainManager.Items.MagicDrops, MainManager.Items.RoastBerry, MainManager.Items.BakedYam, MainManager.Items.LeafSalad ,  MainManager.Items.FrozenSalad, MainManager.Items.MushroomCandy,},
                //good Healing HP items
                new MainManager.Items[] { MainManager.Items.GlazedShroom,MainManager.Items.HeartyBreakfast, MainManager.Items.FrozenSalad, MainManager.Items.KingDinner, MainManager.Items.TangyCarpaccio},
                //normal tp items
                new MainManager.Items[] { MainManager.Items.GlazedHoney, MainManager.Items.HoneydLeaf, MainManager.Items.BurlyCandy, MainManager.Items.HoneyMilk, MainManager.Items.ShavedIce, MainManager.Items.DrowsyCake, MainManager.Items.SpicyCandy },
                //good tp items
                new MainManager.Items[] { MainManager.Items.HoneyIceCream, MainManager.Items.Donut, MainManager.Items.BerrySmoothie, MainManager.Items.TangyPie, MainManager.Items.SquashPie, MainManager.Items.TangyJam},
                //attacking items
                new MainManager.Items[] {(MainManager.Items)NewItem.PointSwap, (MainManager.Items)NewItem.TauntBerry,MainManager.Items.SpicyBomb, MainManager.Items.BurlyBomb,MainManager.Items.ProteinShake, MainManager.Items.NumbBomb, (MainManager.Items)NewItem.SeedlingWhistle, MainManager.Items.SleepBomb, MainManager.Items.LonglegSummoner},
            },
            //shop floor 90
            new MainManager.Items[][]
            {
                //normal healing HP items
                new MainManager.Items[] { MainManager.Items.MagicDrops, MainManager.Items.RoastBerry, MainManager.Items.BakedYam, MainManager.Items.LeafSalad ,  MainManager.Items.FrozenSalad, MainManager.Items.MushroomCandy,},
                //good Healing HP items
                new MainManager.Items[] { MainManager.Items.GlazedShroom,MainManager.Items.HeartyBreakfast, MainManager.Items.FrozenSalad, MainManager.Items.KingDinner, MainManager.Items.TangyCarpaccio},
                //normal tp items
                new MainManager.Items[] { MainManager.Items.GlazedHoney, MainManager.Items.HoneydLeaf, MainManager.Items.BurlyCandy, MainManager.Items.HoneyMilk, MainManager.Items.ShavedIce, MainManager.Items.DrowsyCake, MainManager.Items.SpicyCandy },
                //good tp items
                new MainManager.Items[] { MainManager.Items.HoneyIceCream, MainManager.Items.Donut, MainManager.Items.BerrySmoothie, MainManager.Items.TangyPie, MainManager.Items.SquashPie, MainManager.Items.TangyJam},
                //attacking items
                new MainManager.Items[] { (MainManager.Items)NewItem.PointSwap, (MainManager.Items)NewItem.TauntBerry,MainManager.Items.SpicyBomb, MainManager.Items.BurlyBomb,MainManager.Items.ProteinShake, MainManager.Items.NumbBomb, (MainManager.Items)NewItem.SeedlingWhistle, MainManager.Items.SleepBomb, MainManager.Items.LonglegSummoner},
            }
        };

        Vector3[] enemyPos = new Vector3[]
        {
            new Vector3(6.6f,0f,6.2f),
            new Vector3(-0.189f,0f, 13.43f),
            new Vector3(0.226f,0f,-0.7742f),
            new Vector3(-6.131f,0f,6.5967f),
            new Vector3(-5.0793f, 0f, -0.4688f),
            new Vector3(5.8883f, 0f, -0.4689f)
        };

        public const int MAX_BUSH_PER_FLOOR = 11;

        Vector3[][] bushZones = new Vector3[][]
        {
            new Vector3[]{ new Vector3( -7.8f, 0f, 5f), new Vector3(-5f,0f,1f) },
            new Vector3[]{ new Vector3(-7.8f, 0f, 12f), new Vector3(-5f,0f,8.5f) },
            new Vector3[]{ new Vector3(-5.4f, 0f, 1f), new Vector3(4.3f, 0f, -1.65f) },
            new Vector3[]{ new Vector3(7.4f, 0f, 11.2f), new Vector3(5.5f, 0f, 0.5f) },
            new Vector3[]{ new Vector3(-3.9595f, 0f, 15.3886f), new Vector3(3.7863f, 0f, 11.6743f) },
        };

        int[][] bushTypes = new int[][]
        {
            
            new int[]{ 0,1},//1-9
            new int[]{ 2 },//11-19
            new int[]{ 3},//21-29
            new int[]{ 3,2},//31-39
            new int[]{ 3},//41-49
            new int[]{ 4,5},//51-59
            new int[]{ 6,4},//61-69
            new int[]{ 6},//71-79
            new int[]{ 5},//81-89
            new int[]{ 5,6},//91-99
        };

        MapControl.BattleLeafType[] battleLeafTypes = new MapControl.BattleLeafType[]
        {
            MapControl.BattleLeafType.Snakemouth, //1-9
            MapControl.BattleLeafType.GoldenHills,//11-19
            MapControl.BattleLeafType.Desert,//21-29
            MapControl.BattleLeafType.Bee,//31-39
            MapControl.BattleLeafType.Desert,//41-49
            MapControl.BattleLeafType.FarGrasslands,//51-59
            MapControl.BattleLeafType.Snakemouth,//61-69
            MapControl.BattleLeafType.BarrenLands,//71-79
            MapControl.BattleLeafType.Snakemouth,//81-89
            MapControl.BattleLeafType.Snakemouth,//91-99
        };

        string[] musics = new string[]
        {
            "Cave0", //1-9
            "Cave1",//11-19
            "Dungeon0",//21-29
            "Dungeon2b",//31-39
            "Dungeon1",//41-49
            "Dungeon4",//51-59
            "Lab",//61-69
            "Field4",//71-79
            "Giant1",//81-89
            "Dungeon5",//91-99
        };

        KeyValuePair<int, int>[] pitRewards = new KeyValuePair<int, int>[]
        {
            new KeyValuePair<int,int>((int)Medal.TeamEffort,  798), //floor 10
            new KeyValuePair<int,int>((int)MainManager.BadgeTypes.DefenseExchange, 505), //floor 20
            new KeyValuePair<int,int>((int)Medal.ViolentVitiation,  800),//floor 30
            new KeyValuePair<int,int>((int)Medal.TeamGleam,  801),  //floor 40
            new KeyValuePair<int,int>((int)NewItem.MysteryPouch,  802), //floor 50
            new KeyValuePair<int,int>((int)Medal.LifeLust,  803), //floor 60
            new KeyValuePair<int,int>((int)Medal.TwinedFate,  804), //floor 70
            new KeyValuePair<int,int>((int)MainManager.Items.RedRibbon,  805), //floor 80
            new KeyValuePair<int,int>((int)Medal.StrikeBlaster,  806), //floor 90
            new KeyValuePair<int,int>((int)Medal.Switcheroo,  807), //floor 100
        };

        public bool moverFloor = false;

        public void CreateBushEntities(MapControl map)
        {
            int bushNumber = UnityEngine.Random.Range(0, MAX_BUSH_PER_FLOOR + 1);
            int floorid = GetCurrentFloor();
            var currentFloorItemDrops = floorItemDrops[floorid / 10];
            for (int i = 0; i < bushNumber; i++)
            {
                var zone = bushZones[UnityEngine.Random.Range(0, bushZones.Length)];
                var position = new Vector3(UnityEngine.Random.Range(zone[0].x, zone[1].x), 0, UnityEngine.Random.Range(zone[0].z, zone[1].z));
                var bush = EntityControl.CreateNewEntity("bush" + i, -1, position);
                bush.transform.parent = map.transform;
                bush.startpos = position;

                bush.npcdata = bush.gameObject.AddComponent<NPCControl>();
                bush.npcdata.objecttype = NPCControl.ObjectTypes.BeetleGrass;
                bush.npcdata.entitytype = NPCControl.NPCType.Object;
                bush.npcdata.radius = 0;
                bush.npcdata.actionfrequency = new float[] { 200, 200 };
                bush.npcdata.boxcol = bush.gameObject.AddComponent<BoxCollider>();
                bush.npcdata.boxcol.isTrigger = false;
                bush.npcdata.boxcol.size = new Vector3(1.5f, 20, 0.75f);
                bush.npcdata.boxcol.center = new Vector3(0, 10, 0);
                bush.freezesize = Vector3.zero;
                bush.freezeoffset = Vector3.zero;
                bush.npcdata.requires = new int[0];
                bush.npcdata.limit = new int[0];
                bush.emoticonoffset = Vector3.zero;

                //this control what type of bush it is
                bush.npcdata.data = new int[] { bushTypes[floorid / 10][UnityEngine.Random.Range(0,bushTypes[floorid/10].Length)], -1 };

                //5% to have item in it
                if (UnityEngine.Random.Range(0, 100) < 5)
                {
                    bush.npcdata.vectordata = new Vector3[]
                    {
                        new Vector3((int)currentFloorItemDrops[UnityEngine.Random.Range(0, currentFloorItemDrops.Length)], -1, 0)
                    };
                }
                else
                {
                    bush.npcdata.vectordata = new Vector3[] { new Vector3(-1, -1, 0) };
                }
            }
        } 


        public int[] GetCurrentFloorEnemies()
        {
            int currentFloor = MainManager.instance.flagvar[(int)NewFlagVar.Pit_Floor] -1;
            int maxFormations = floorEnemies[currentFloor].Length;
            return floorEnemies[currentFloor][UnityEngine.Random.Range(0, maxFormations)].ToArray();
        }

        public Vector3 GetRandomEnemyPos()
        {
            return enemyPos[UnityEngine.Random.Range(0, enemyPos.Length)];
        }

        public Vector3[] GetRandomFloorEnemyItemDrop()
        {
            int floorid = GetCurrentFloor();
            Vector3[] itemDrops = new Vector3[1];
            var currentFloorItemDrops = floorItemDrops[floorid / 10];

            for(int i =0; i != itemDrops.Length;i++)
                itemDrops[i] = new Vector3((int)currentFloorItemDrops[UnityEngine.Random.Range(0, currentFloorItemDrops.Length)], -1, 0);

            return itemDrops;
        }

        public void CheckPitReward(MapControl map)
        {
            int floorid = GetCurrentFloor();
            KeyValuePair<int, int> rewardData = pitRewards[(floorid / 10) - 1];

            bool keyItem = rewardData.Key == (int)MainManager.Items.RedRibbon || rewardData.Key == (int)NewItem.MysteryPouch;
            int itemType = keyItem ? 1 : 2;
            NPCControl rewardEntity = EntityControl.CreateItem(new Vector3(0.1f, 0.4f, 13.7f), itemType, rewardData.Key, Vector3.zero, -1);
            rewardEntity.limit = new int[1] { rewardData.Value };
            rewardEntity.activationflag = rewardData.Value;

            if(rewardData.Key == (int)NewItem.MysteryPouch)
            {
                rewardEntity.data = new int[] { itemType, (int)NewEvents.CollectMagicPouch, 1 };
            }
        }

        public void SetupShop(MapControl map)
        {
            var itemEntities = map.entities.Where(e => e.name.Contains("Fixedshop")).ToList();
            int floorid = (GetCurrentFloor()/10) - 1;

            var currentFloorShopItems = shopItems[floorid];

            for (int i=0; i!= currentFloorShopItems.Length; i++)
            {
                int item = (int)currentFloorShopItems[i][UnityEngine.Random.Range(0, currentFloorShopItems[i].Length)];
                itemEntities[i].animstate = item;
            }
        }


        public void GetPitFloorEntities(List<string> data, List<string> names)
        {
            moverFloor = false;
            int floorValue = GetCurrentFloor();
            var dataAsset = MainManager_Ext.assetBundle.LoadAsset<TextAsset>("PitData").ToString().Split('\n');
            var namesAsset = MainManager_Ext.assetBundle.LoadAsset<TextAsset>("PitNames").ToString().Split('\n');

            int entityIndex = floorValue + 2;
            data.AddRange(new string[] { dataAsset[0], dataAsset[1] });
            names.AddRange(new string[] { namesAsset[0], namesAsset[1] });
            if (floorValue != 99)
            {
                if (floorValue < 95)
                {
                    //7% odds of getting mover
                    if (UnityEngine.Random.Range(0, 100) < 7 && (!MainManager.instance.flags[834] || (MainManager.instance.flags[834] && MainManager.instance.flags[835])))
                    {
                        entityIndex = 2;
                        moverFloor = true;
                    }
                }
                data.Add(dataAsset[entityIndex]);
                names.Add(namesAsset[entityIndex].Trim('\r'));
            }
            else
            {
                for(int i =0;i < 4; i++)
                {
                    data.Add(dataAsset[entityIndex+i]);
                    names.Add(namesAsset[entityIndex+i].Trim('\r'));
                }
            }
        }

        public MapControl.BattleLeafType GetCurrentBattleLeaf()
        {
            int floorid = GetCurrentFloor() / 10;
            return battleLeafTypes[floorid];
        }


        public string GetCurrentMusic()
        {
            int floorid = GetCurrentFloor() / 10;
            return musics[floorid];
        }

        public static int GetCurrentFloor() => MainManager.instance.flagvar[(int)NewFlagVar.Pit_Floor];
        
        public static PitData GetPitData()
        {
            if (MainManager.instance.GetComponent<PitData>() == null)
                MainManager.instance.gameObject.AddComponent<PitData>();
            return MainManager.instance.GetComponent<PitData>();
        }

        public void GetEnemiesPos(EntityControl[] enemies)
        {
            List<Vector3> positions = new List<Vector3>(enemyPos);

            for(int i = 0; i < enemies.Length; i++)
            {
                int index = UnityEngine.Random.Range(0, positions.Count);
                Vector3 pos = positions[index];
                enemies[i].SetPosition(pos);
                positions.RemoveAt(index);
            }
        }
    }
}
