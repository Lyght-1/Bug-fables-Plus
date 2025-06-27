using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Object;
namespace BFPlus.Extensions.Events.PitEvents
{
    public class LeavePitEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            while (MainManager.instance.message)
            {
                yield return null;
            }
            MainManager.PlayTransition(0, 0, 0.075f, Color.black);
            SpriteRenderer dimmer = MainManager.instance.transitionobj[0].GetComponent<SpriteRenderer>();
            while (dimmer.color.a < 0.95f)
            {
                yield return null;
            }
            dimmer.color = Color.black;
            Destroy(MainManager.instance.GetComponent<PitData>());

            MainManager.LoadMap((int)MainManager.Maps.CaveOfTrials);
            MainManager.ChangeMusic();
            MainManager.SetCamera(MainManager.player.transform, null, 1);

            MainManager.instance.flagvar[39] = 0;

            Vector3[] posArray = new Vector3[]
            {
                new Vector3(-1.5f, 1.5f, 9f),
                new Vector3(0f, 1.5f, 9f),
                new Vector3(1.5f, 1.5f, 9f),
                new Vector3(2.5f, 1.5f, 9f)
            };

            EntityControl[] party = MainManager.GetPartyEntities(true);
            for (int i = 0; i < party.Length; i++)
            {
                party[i].transform.position = posArray[i];
            }
            if (MainManager.map.chompy != null)
            {
                MainManager.map.chompy.transform.position = posArray[4];
            }

            EntityControl lieutenant = MainManager.map.tempfollowers?.FirstOrDefault(f => f?.animid == (int)MainManager.AnimIDs.AntCapitain - 1);

            if (lieutenant != null)
            {
                lieutenant.transform.position = new Vector3(2.5f, 1.5f, 10f);
            }

            yield return EventControl.sec;
            MainManager.PlayTransition(1, 0, 0.075f, Color.black);
            dimmer = MainManager.instance.transitionobj[0].GetComponent<SpriteRenderer>();
            while (dimmer.color.a > 0.1f)
            {
                yield return null;
            }
            yield return EventControl.quartersec;
        }
    }
}
