using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Extensions.Events
{
    public class LeifFungalLeechEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            while (MainManager.instance.message)
                yield return null;

            MainManager.DialogueText(MainManager.map.dialogues[93], caller.transform, caller);
            while (MainManager.instance.message)
                yield return null;

            MainManager.FadeIn();
            yield return EventControl.sec;

            MainManager.DialogueText(MainManager.map.dialogues[94], caller.transform, caller);
            while (MainManager.instance.message)
                yield return null;
            MainManager.PlaySound("Scanner1");
            yield return EventControl.halfsec;

            MainManager.PlaySound("Shield");
            yield return EventControl.halfsec;

            MainManager.DialogueText(MainManager.map.dialogues[95], caller.transform, caller);
            while (MainManager.instance.message)
                yield return null;

            yield return EventControl.quartersec;
            MainManager.PlaySound("Scanner1");
            yield return EventControl.sec;
            MainManager.PlaySound("Scanner2",0,1,1,true);
            yield return EventControl.sec;
            yield return EventControl.sec;
            MainManager.StopSound("Scanner2");
            MainManager.PlaySound("Scanner3");
            yield return EventControl.sec;

            EntityControl[] party = MainManager.GetPartyEntities(true);

            party[2].animstate = (int)MainManager.Animations.WeakBattleIdle;
            MainManager.FadeOut();
            yield return EventControl.sec;

            MainManager.DialogueText(MainManager.map.dialogues[96], caller.transform, caller);
            while (MainManager.instance.message)
                yield return null;

            party[2].animstate = 124;
            MainManager.PlaySound("Buzz1");
            MainManager.PlaySound("Fungi");

            yield return null;

            party[0].animstate = (int)MainManager.Animations.Surprized;
            party[1].animstate = (int)MainManager.Animations.Surprized;

            yield return EventControl.sec;
            yield return EventControl.sec;
            MainManager.StopSound("Fungi");

            MainManager.DialogueText(MainManager.map.dialogues[97], party[2].transform, party[2].npcdata);
            while (MainManager.instance.message)
                yield return null;

        }

    }
}
