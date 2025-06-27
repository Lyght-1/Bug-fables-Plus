using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Extensions.Events.PitEvents
{
    public class CollectMagicPouchEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            while (MainManager.instance.message)
            {
                yield return null;
            }
            MainManager.instance.maxitems += 3;
            yield return null;
        }
    }
}
