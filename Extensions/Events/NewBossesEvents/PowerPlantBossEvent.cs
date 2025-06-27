using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BFPlus.Extensions.Events.NewBossesEvents
{
    public class PowerPlantBossEvent : StartBossFightEvent
    {
        protected override IEnumerator ApproachingBoss(EventControl instance, NPCControl caller)
        {
            EntityControl[] shrooms = MainManager.GetEntities(new int[] { 0, 2, 3 });
            Vector3[] posArray = new Vector3[]
            {
                new Vector3(0,0,-7),
                new Vector3(-1, 0,-7.7f),
                new Vector3(1, 0, -7.7f),
                new Vector3(-1, 0, -8.5f)
            };
            MainManager.SetCamera(shrooms[0].transform, null, 0.035f);
            yield return MoveParty(instance, caller, posArray);

            foreach (var particle in MainManager.map.mainmesh.Find("ElecParticles").GetComponentsInChildren<ParticleSystem>())
            {
                particle.Stop();
            }
            MainManager.PlaySound("Jump", -1, 0.8f, 1f);
            shrooms[0].Jump(shrooms[0].jumpheight * 1.25f);
            yield return EventControl.halfsec;
            while (!shrooms[0].onground)
            {
                yield return null;
            }

            shrooms[0].animstate = 5;
            yield return EventControl.quartersec;

            yield return StartBattle(new int[] { (int)NewEnemies.BatteryShroom, (int)NewEnemies.DynamoSpore, (int)NewEnemies.BatteryShroom });

            MainManager.map.mainmesh.Find("ElecParticles").gameObject.SetActive(false);
            yield return DoWinFightEvent(instance, caller, shrooms, MainManager.GetPartyEntities(true), (int)NewItem.SilverFuse, 847, 1, shrooms[0].startpos.Value + Vector3.up * 0.5f);
        }
    }
}
