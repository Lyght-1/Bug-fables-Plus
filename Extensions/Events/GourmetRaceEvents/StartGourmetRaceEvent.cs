using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events.GourmetRaceEvents
{
    public class StartGourmetRaceEvent : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            MainManager.FadeMusic(0.015f);
            MainManager.PlayTransition(0, 0, 0.025f, Color.black);
            yield return null;

            SpriteRenderer s = MainManager.GetTransitionSprite();
            while (s.color.a < 0.95f)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.75f);

            if (MainManager.instance.partyorder[0] != 2)
            {
                UnityEngine.Object.Destroy(MainManager.GetEntity(-6).detect.gameObject);
            }

            MainManager.ChangeParty(new int[1] { 2 }, true, true);

            yield return null;
            MainManager.instance.playerdata[0].entity.transform.position = new Vector3(0, -4f, -23.72f);
            MainManager.ResetCamera(true);

            EntityControl[] partyEntities = new EntityControl[]
            {
                EntityControl.CreateNewEntity("vi", 0, new Vector3(-1.52f, 0f, -1.14f)),
                EntityControl.CreateNewEntity("kabbu", 1, new Vector3(-3f, 0, -1.14f))
            };
            yield return null;
            if (MainManager.map.chompy != null)
            {
                MainManager.map.chompy.following = null;
                MainManager.map.chompy.transform.position = new Vector3(-5f, 0f, -1.14f);
                MainManager.map.chompy.flip = false;
            }

            for (int i = 0; i < partyEntities.Length; i++)
            {
                partyEntities[i].animstate = (int)MainManager.Animations.Happy;
                partyEntities[i].transform.parent = MainManager.map.transform;
                partyEntities[i].flip = true;
            }

            GourmetRace game = MainManager.map.gameObject.AddComponent<GourmetRace>();

            /*caller.entity.flip = true;
            caller.entity.emoticoncooldown = 0f;
            caller.entity.emoticonid = -1;*/
            ///

            CreateAudience(MainManager.map.transform, 30, Audience.Type.OnlyBee, new Vector2(1, 0.1f), new Vector3(15, 2), new Vector3(0, 0, -1), new Vector3(0, 180, 0));
            MainManager.player.npc = new List<NPCControl>();

            AudioClip battletheme = Resources.Load<AudioClip>("Audio/Music/Battle0");
            yield return null;
            while (MainManager.musiccoroutine != null)
            {
                yield return null;
            }

            yield return EventControl.tenthsec;
            MainManager.PlayTransition(1, 0, 0.025f, Color.black);

            yield return null;
            s = MainManager.GetTransitionSprite();
            while (s.color.a > 0.1f)
            {
                yield return null;
            }

            MainManager.ChangeMusic(battletheme);
            MainManager.music[0].pitch = 1.05f;

            //can tattle flag
            MainManager.instance.flags[10] = false;
            game.StartGame();

            if(!MainManager.instance.librarystuff[0, (int)NewDiscoveries.GourmetRace])
            {
                MainManager.UpdateJounal(MainManager.Library.Discovery, (int)NewDiscoveries.GourmetRace);
            }
            yield return null;
        }
    }
}
