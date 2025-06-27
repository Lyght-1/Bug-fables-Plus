using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events.DarkTeamSnakemouthEvents
{
    public class DarkTeamSnakemouthSearch : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            while (MainManager.instance.message)
            {
                yield return null;
            }

            if (MainManager.CurrentMap() == MainManager.Maps.BugariaResidential)
            {
                MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[102], true, Vector3.zero, caller.transform, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }
            }

            if (MainManager.CurrentMap() == MainManager.Maps.SwamplandsBoss)
            {
                var entities = new EntityControl[]
                {
                    MainManager.GetEntity(-4),
                    MainManager.GetEntity(-5),
                    MainManager.GetEntity(-6)
                };

                for (int i = 0; i < entities.Length; i++)
                {
                    entities[i].Emoticon(MainManager.Emoticons.Exclamation, 60);
                }

                EntityControl darkKabbu = EntityControl.CreateNewEntity("DarkKabbu", 1, new Vector3(1.3665f, 0f, 38.9947f));
                MainManager.SetCamera(darkKabbu.transform, darkKabbu.transform.position, 0.035f);
                yield return EventControl.sec;
                MainManager.PlaySound("Dig");
                darkKabbu.digging = true;
                while (darkKabbu.digtime < 29f)
                {
                    yield return null;
                }
                yield return EventControl.sec;
                darkKabbu.gameObject.SetActive(false);
                MainManager.ResetCamera();
                //cutscene dark kabbu
                MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[14], true, Vector3.zero, MainManager.GetEntity(-4).transform, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }
            }

            if (MainManager.CurrentMap() == MainManager.Maps.JaunesGallery)
            {
                MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[27], true, Vector3.zero, caller.transform, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }
            }

            if (!MainManager.instance.flags[811] || !MainManager.instance.flags[812] || !MainManager.instance.flags[814])
            {

                string locationText = "";

                int count = 0;
                if (!MainManager.instance.flags[811])
                {
                    locationText += "the |color,1|Bee Kingdom|color,0|";
                    count++;
                }

                if (!MainManager.instance.flags[812])
                {
                    if (count == 1)
                    {
                        if (MainManager.instance.flags[814])
                        {
                            locationText += " and ";
                        }
                        else
                        {
                            locationText += ",";
                        }
                    }

                    locationText += "the |color,1|Ant Kingdom|color,0|";
                    count++;
                }

                if (!MainManager.instance.flags[814])
                {
                    if (count > 0)
                        locationText += " and ";
                    locationText += "the |color,1|Wasp Kingdom|color,0|";
                }

                string text = $"Well, we still have {locationText} to go to.|next|Hopefully there are more clues about this fake Team Snakemouth there.";

                MainManager.instance.StartCoroutine(MainManager.SetText(text, true, Vector3.zero, MainManager.GetEntity(-5).transform, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }
            }
            else
            {
                string text = "|tail,-5|We checked every location and still have no idea where they are. Looks like it's a dead end. We should go report back to Zaryant.";
                MainManager.instance.StartCoroutine(MainManager.SetText(text, true, Vector3.zero, MainManager.GetEntity(-5).transform, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }
            }
            yield return null;
        }
    }
}
