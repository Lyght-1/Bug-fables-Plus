using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events.DarkTeamSnakemouthEvents
{
    public class DarkTeamSnakemouthZaryant : NewEvent
    {
        protected override IEnumerator DoEvent(NPCControl caller, EventControl instance)
        {
            while (MainManager.instance.message)
            {
                yield return null;
            }
            var entities = new EntityControl[]
            {
                MainManager.GetEntity(-4),
                MainManager.GetEntity(-5),
                MainManager.GetEntity(-6),
                MainManager.GetEntity(1),
            };

            if (!MainManager.instance.flags[810])
            {

                MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[145], true, Vector3.zero, MainManager.GetEntity(1).transform, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }

                EntityControl antGuard = MainManager.GetEntity(6);
                Vector3 startPos = antGuard.transform.position;
                antGuard.transform.position = new Vector3(-5, 0f, 0f);


                MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[146], true, Vector3.zero, antGuard.transform, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }

                antGuard.MoveTowards(new Vector3(12f, 0f, -1f), 1.5f);
                while (antGuard.forcemove)
                {
                    for (int num = 0; num < entities.Length; num++)
                    {
                        entities[num].FaceTowards(antGuard.transform.position);
                    }
                    yield return null;
                }
                MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[147], true, Vector3.zero, antGuard.transform, null));
                while (MainManager.instance.message)
                {
                    yield return null;
                }

                antGuard.MoveTowards(new Vector3(-5f, 0f, -1f), 1.5f);

                while (antGuard.forcemove)
                {
                    yield return null;
                }

                antGuard.transform.position = startPos;
                MainManager.instance.flags[810] = true;
            }
            else
            {
                if (!MainManager.instance.flags[811] || !MainManager.instance.flags[812] || !MainManager.instance.flags[814])
                {
                    MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[148], true, Vector3.zero, MainManager.GetEntity(1).transform, null));
                    while (MainManager.instance.message)
                    {
                        yield return null;
                    }
                }
                else
                {
                    MainManager.instance.StartCoroutine(MainManager.SetText(MainManager.map.dialogues[149], true, Vector3.zero, MainManager.GetEntity(1).transform, null));
                    while (MainManager.instance.message)
                    {
                        yield return null;
                    }
                    MainManager.instance.flags[815] = true;
                }
            }
        }
    }
}
