using KorzUtils.Enums;
using KorzUtils.Helper;
using Modding;

namespace TheHuntIsOn.Modules;

internal class CharmModule : Module
{
    #region Properties

    public override string MenuDescription => "Auto equips picked up charms and change their cost to 1.";

    #endregion

    #region Eventhandler

    private bool ModHooks_SetPlayerBoolHook(string name, bool orig)
    {
        if (IsModuleUsed && name.StartsWith("gotCharm_") && orig)
        {
            if (int.TryParse(name.Substring("gotCharm_".Length), out int charmNumber))
                CoroutineHelper.WaitFrames(x =>
                {
                    HealthControl.BlockHeal = true;
                    CharmHelper.SetCharmCost(x, 1);
                    CharmHelper.EquipCharm(x);
                    HealthControl.BlockHeal = false;
                }, (CharmRef)charmNumber, true, 3);
        }
        return orig;
    }

    #endregion

    #region Methods

    internal override void Enable() => ModHooks.SetPlayerBoolHook += ModHooks_SetPlayerBoolHook;

    internal override void Disable() => ModHooks.SetPlayerBoolHook -= ModHooks_SetPlayerBoolHook;
	#endregion
}
