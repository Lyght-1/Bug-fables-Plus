using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFPlus.Extensions.Events
{
    public class TanjyFightEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            while (MainManager.instance.message)
                yield return null;

            MainManager.DialogueText(MainManager.map.dialogues[60], caller.transform, caller);
            while (MainManager.instance.message)
                yield return null;

            MainManager.battlelossevent = true;
            MainManager.instance.StartCoroutine(BattleControl.StartBattle(new int[]
            {
                (int)MainManager.Enemies.TANGYBUG
            }, -1, -1, "Battle6", null, true));
            yield return EventControl.sec;

            while (MainManager.battle != null)
                yield return null;
            EntityControl[] p = MainManager.GetPartyEntities(true);

            MainManager.battlelossevent = false;
            MainManager.ResetCamera(true);
            MainManager.FadeOut(0.05f);
            yield return EventControl.halfsec;
            bool lost = !MainManager.battleresult || MainManager.battlefled;

            foreach (var e in p)
                e.animstate = lost ? (int)MainManager.Animations.WeakBattleIdle : (int)MainManager.Animations.BattleIdle;

            if (lost)
                MainManager.DialogueText(MainManager.map.dialogues[63], caller.transform, caller);
            else
                MainManager.DialogueText(MainManager.map.dialogues[61], caller.transform, caller);

            while (MainManager.instance.message)
                yield return null;

            MainManager.ChangeMusic();
            yield return null;
        }
    }
}
