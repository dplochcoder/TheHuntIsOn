using HutongGames.PlayMaker;
using KorzUtils.Data;
using KorzUtils.Helper;

namespace TheHuntIsOn.Modules.HealthModules;

internal class DreamHealModule : Module
{
    #region Properties

    public override string MenuDescription => "Blocks the heal from entering a dream room.";

    #endregion

    #region Eventhandler

    private void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (IsModuleUsed && self.FsmName == "Control" && self.gameObject.name == "Dream Entry" && self.gameObject.scene.name.Contains("Dream"))
        {
            FsmStateAction[] takeControlActions = self.GetState("Take Control").Actions;
            self.AddState(new(self.Fsm)
            {
                Name = "Block Heal",
                Actions = new FsmStateAction[]
                {
                    takeControlActions[0], // Relinquish Control
                    takeControlActions[1], // Stop Animation Control
                    takeControlActions[2], // Next Frame event
                    takeControlActions[3], // SetMeshRenderer
                    //takeControlActions[4], // Max Health
                    //takeControlActions[5], // Update Blue Health
                }
            }, FsmTransitionData.FromTargetState("Facing").WithEventName("FINISHED"));
            self.GetState("Door Entry").AdjustTransitions("Block Heal");
        }
        orig(self);
    }

    #endregion

    #region Methods

    internal override void Enable() => On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
    
    internal override void Disable() => On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;

    #endregion
}
