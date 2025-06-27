using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events.PitEvents
{
    public class MoverEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            while (MainManager.instance.message)
            {
                yield return null;
            }

            int[] travelCosts = new int[]{10,30,10};
            int[] floorSkips = new int[] { 2, 5 };

            MainManager.DialogueText(MainManager.map.dialogues[2], caller.transform, caller);
            while (MainManager.instance.message)
            {
                yield return null;
            }
            if (MainManager.instance.option <= 2)
            {
                int moneyCost = travelCosts[MainManager.instance.option];
                if (MainManager.instance.money < moneyCost)
                {
                    MainManager.DialogueText(MainManager.map.dialogues[4], caller.transform, caller);
                    while (MainManager.instance.message)
                    {
                        yield return null;
                    }
                }
                else
                {
                    MainManager.DialogueText(string.Concat(new object[]
                    {
                    (moneyCost > 0) ? "|showmoney|" : "",
                    "|money,-",
                    moneyCost,
                    "|",
                    MainManager.map.dialogues[5]
                    }), caller.transform, caller);
                    while (MainManager.instance.message)
                    {
                        yield return null;
                    }

                    if(MainManager.instance.option < 2)
                    {
                        MainManager.FadeIn();
                        yield return EventControl.sec;
                        MainManager.instance.flagvar[(int)NewFlagVar.Pit_Floor] += floorSkips[MainManager.instance.option]-1;
                        yield return LoadPitRoom(instance);
                        yield break;
                    }
                    else
                    {
                        MainManager.events.StartEvent((int)NewEvents.LeavePit, caller);
                        endEvent = false;
                    }
                }
            }
            yield return EventControl.quartersec;
        }
    }
}
