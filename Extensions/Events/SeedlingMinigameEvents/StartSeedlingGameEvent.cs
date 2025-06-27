using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events.SeedlingMinigameEvents
{
    public class StartSeedlingGameEvent : NewEvent
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

            if (MainManager.instance.partyorder[0] != 1)
            {
                UnityEngine.Object.Destroy(MainManager.GetEntity(-5).detect.gameObject);
            }

            MainManager.ChangeParty(new int[1] { 1 }, true, true);

            yield return null;
            MainManager.instance.playerdata[0].entity.transform.position = new Vector3(-1.7f, 0, -10.9f);
            MainManager.ResetCamera(true);

            EntityControl[] partyEntities = new EntityControl[]
            {
                EntityControl.CreateNewEntity("vi", 0, new Vector3(-4.4f, 0, 4.6f)),
                EntityControl.CreateNewEntity("mot", 2, new Vector3(-6f, 0, 5.12f))
            };
            yield return null;
            if (MainManager.map.chompy != null)
            {
                MainManager.map.chompy.following = null;
                MainManager.map.chompy.transform.position = new Vector3(-2.6f, 0f, 4.5f);
                MainManager.map.chompy.flip = false;
            }

            partyEntities[0].animstate = (int)MainManager.Animations.Happy;
            for (int i = 0; i < partyEntities.Length; i++)
            {
                partyEntities[i].transform.parent = MainManager.map.transform;
                partyEntities[i].flip = true;
            }

            foreach (var entity in MainManager.map.entities)
            {
                if (entity.npcdata != null && entity.npcdata.behaviors.Contains(NPCControl.ActionBehaviors.FleeFromPlayer))
                    entity.gameObject.SetActive(false);
            }

            SeedlingMinigame game = GameObject.Find("CatchTrigger").AddComponent<SeedlingMinigame>();
            /*caller.entity.flip = true;
            caller.entity.emoticoncooldown = 0f;
            caller.entity.emoticonid = -1;*/
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

            MainManager_Ext.inSeedlingMinigame = true;
            game.start = true;
            if (!MainManager.instance.librarystuff[0, (int)NewDiscoveries.SeedlingSnatch])
            {
                MainManager.UpdateJounal(MainManager.Library.Discovery, (int)NewDiscoveries.SeedlingSnatch);
            }
            yield return null;
        }
    }
}
