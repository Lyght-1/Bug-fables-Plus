using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events
{
    public class CheckMedalEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            int num = 0;
            int dialogueId=0;
            if ((int)MainManager.map.mapid == (int)NewMaps.BeehiveMinigame)
            {
                num = (int)Medal.GourmetStomach;
                dialogueId = 23;
            }
            MainManager.DialogueText("|face,party,caller|" + string.Concat(new string[]
            {
                "|boxstyle,4||spd,0||color,1|",
                MainManager.badgedata[num, 0],
                "|color,0||line||sizemulti,0.9,0.9|",
                MainManager.badgedata[num, 1],
                "|next,-4|"
            }) + "|boxstyle,0||size,1,1||spd,-1|" + MainManager.map.dialogues[dialogueId], null, caller);

            while (MainManager.instance.message)
            {
                yield return null;
            }
            yield return null;
        }
    }
}
