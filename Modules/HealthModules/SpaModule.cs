using KorzUtils.Data;
using KorzUtils.Helper;
using System;
using UnityEngine;

namespace TheHuntIsOn.Modules.HealthModules;

internal class SpaModule : Module
{
    #region Properties

    public override string MenuDescription => "Blocks the effect of spas.";

    #endregion

    #region Eventhandler

    private void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (IsModuleUsed && self.FsmName == "Spa Region")
        {
            self.AddState("Destroy", () => GameObject.Destroy(self.gameObject), Array.Empty<FsmTransitionData>());
            self.GetState("Init").AdjustTransition("FINISHED", "Destroy");
        }
        orig(self);
    }

    #endregion

    #region Methods

    internal override void Enable() => On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;

    internal override void Disable() => On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;

    #endregion
}
