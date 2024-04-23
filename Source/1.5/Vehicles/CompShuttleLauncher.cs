﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Vehicles;
using Verse;

namespace SaveOurShip2.Vehicles
{
    class CompShuttleLauncher : CompVehicleLauncher
    {
        public float retreatAtHealth;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo giz in base.CompGetGizmosExtra())
                yield return giz;
            yield return new ShuttleRetreatGizmo(this);
            if(parent.Map.GetComponent<ShipMapComp>()?.ShipMapState==ShipMapState.inCombat && ((VehiclePawn)parent).handlers[0].handlers.Count>0)
            {
                VehiclePawn vehicle = ((VehiclePawn)parent);
                if (parent.Map.GetComponent<ShipMapComp>().IsPlayerShipMap)
                    yield return CommandBoard(vehicle);
                else
                    yield return CommandGoHome(vehicle);
                bool hasLaser = vehicle.CompUpgradeTree.upgrades.Contains("TurretLaserA") || vehicle.CompUpgradeTree.upgrades.Contains("TurretLaserB") || vehicle.CompUpgradeTree.upgrades.Contains("TurretLaserC");
                bool hasPlasma = vehicle.CompUpgradeTree.upgrades.Contains("TurretPlasmaA") || vehicle.CompUpgradeTree.upgrades.Contains("TurretPlasmaB") || vehicle.CompUpgradeTree.upgrades.Contains("TurretPlasmaC");
                bool hasTorpedo = vehicle.CompUpgradeTree.upgrades.Contains("TurretTorpedoA") || vehicle.CompUpgradeTree.upgrades.Contains("TurretTorpedoB") || vehicle.CompUpgradeTree.upgrades.Contains("TurretTorpedoC")
                    && vehicle.carryTracker.GetDirectlyHeldThings().Any(t => t.HasThingCategory(ResourceBank.ThingCategoryDefOf.SpaceTorpedoes));
                if (hasLaser)
                    yield return CommandIntercept(vehicle);
                if (hasLaser || hasPlasma)
                    yield return CommandStrafe(vehicle);
                if (hasTorpedo)
                    yield return CommandBomb(vehicle);
            }
        }

        Command_Action CommandBoard(VehiclePawn vehicle)
        {
            return new Command_Action
            {
                action = delegate
                {
                    ShuttleTakeoff.LaunchShuttleToCombatManager(vehicle, ShipMapComp.ShuttleMission.BOARD);
                },
                defaultLabel = TranslatorFormattedStringExtensions.Translate("SoS.ShuttleMissionBoarding"),
                defaultDesc = TranslatorFormattedStringExtensions.Translate("SoS.ShuttleMissionBoardingDesc"),
                icon = ContentFinder<Texture2D>.Get("UI/ShuttleMissionBoarding", true)
            };
        }

        Command_Action CommandGoHome(VehiclePawn vehicle)
        {
            return new Command_Action
            {
                action = delegate
                {
                    ShuttleTakeoff.LaunchShuttleToCombatManager(vehicle, ShipMapComp.ShuttleMission.BOARD);
                },
                defaultLabel = TranslatorFormattedStringExtensions.Translate("SoS.ShuttleMissionBoardingReturn"),
                defaultDesc = TranslatorFormattedStringExtensions.Translate("SoS.ShuttleMissionBoardingReturnDesc"),
                icon = ContentFinder<Texture2D>.Get("UI/ShuttleMissionBoarding", true)
            };
        }

        Command_Action CommandIntercept(VehiclePawn vehicle)
        {
            return new Command_Action
            {
                action = delegate
                {
                    ShuttleTakeoff.LaunchShuttleToCombatManager(vehicle, ShipMapComp.ShuttleMission.INTERCEPT);
                },
                defaultLabel = TranslatorFormattedStringExtensions.Translate("SoS.ShuttleMissionIntercept"),
                defaultDesc = TranslatorFormattedStringExtensions.Translate("SoS.ShuttleMissionInterceptDesc"),
                icon = ContentFinder<Texture2D>.Get("UI/ShuttleMissionIntercept", true)
            };
        }

        Command_Action CommandStrafe(VehiclePawn vehicle)
        {
            return new Command_Action
            {
                action = delegate
                {
                    ShuttleTakeoff.LaunchShuttleToCombatManager(vehicle, ShipMapComp.ShuttleMission.STRAFE);
                },
                defaultLabel = TranslatorFormattedStringExtensions.Translate("SoS.ShuttleMissionStrafe"),
                defaultDesc = TranslatorFormattedStringExtensions.Translate("SoS.ShuttleMissionStrafeDesc"),
                icon = ContentFinder<Texture2D>.Get("UI/ShuttleMissionStrafe", true)
            };
        }

        Command_Action CommandBomb(VehiclePawn vehicle)
        {
            return new Command_Action
            {
                action = delegate
                {
                    ShuttleTakeoff.LaunchShuttleToCombatManager(vehicle, ShipMapComp.ShuttleMission.BOMB);
                },
                defaultLabel = TranslatorFormattedStringExtensions.Translate("SoS.ShuttleMissionBomb"),
                defaultDesc = TranslatorFormattedStringExtensions.Translate("SoS.ShuttleMissionBombDesc"),
                icon = ContentFinder<Texture2D>.Get("UI/ShuttleMissionBomb", true)
            };
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref retreatAtHealth, "retreatAtHealth");
        }
    }
}