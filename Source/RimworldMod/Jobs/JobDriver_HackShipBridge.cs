﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RimWorld
{
    class JobDriver_HackShipBridge : JobDriver
    {
        float workDone;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(TargetA, job);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            if (TargetA != LocalTargetInfo.Invalid)
                this.FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Goto.Goto(TargetIndex.A, PathEndMode.InteractionCell);
            Toil hackIt = Toils_General.Wait(3000, TargetA != LocalTargetInfo.Invalid ? TargetIndex.A : TargetIndex.None);
            hackIt.defaultCompleteMode = ToilCompleteMode.Delay;
            hackIt.initAction = delegate
            {
                workDone = 0;
            };
            hackIt.tickAction = delegate
            {
                workDone++;
            };
            hackIt.endConditions = new List<Func<JobCondition>>();
            hackIt.WithProgressBar(TargetIndex.A, () => workDone / 3000f);
            hackIt.WithEffect(EffecterDefOf.Research, TargetIndex.A);
            hackIt.AddFinishAction(delegate {
                if (workDone >= 2990 && pawn.health.State == PawnHealthState.Mobile && TargetA.HasThing && !TargetA.Thing.DestroyedOrNull() && TargetA.Thing is Building)
                {
                    ((Building_ShipBridge)TargetA.Thing).HackMe(pawn);
                }
            });
            yield return hackIt;
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref workDone, "WorkDone");
        }
    }
}