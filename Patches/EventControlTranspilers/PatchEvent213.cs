using BFPlus.Extensions;
using BFPlus.Patches.DoActionPatches;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Patches.EventControlTranspilers
{
    //Kabbu's request event
    public class PatchKabbuRequestMusic : PatchBaseEvent213
    {
        public PatchKabbuRequestMusic()
        {
            priority = 252952;
        }
        protected override void ApplyPatch(ILCursor cursor)
        {
            cursor.GotoNext(i=>i.MatchLdstr("Moth"));
            cursor.Next.Operand = "KabbuTheme";     
        }
    }
}
