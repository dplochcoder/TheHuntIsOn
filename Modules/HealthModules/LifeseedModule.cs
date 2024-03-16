using MonoMod.Cil;
using System;

namespace TheHuntIsOn.Modules.HealthModules;

internal class LifeseedModule : Module
{
    #region Properties

    public override string MenuDescription => "Removes the effect of lifeseeds.";

    #endregion

    #region Eventhandler

    private void ScuttlerControl_Hit(ILContext il)
    {
        // Prevent lifeblood from lifeseed.
        ILCursor cursor = new(il);
        cursor.Goto(0);

        cursor.GotoNext(MoveType.After,
            x => x.MatchLdloc(1));
        cursor.EmitDelegate<Func<bool, bool>>(x => IsModuleUsed ? false : x);
    }

    #endregion

    #region Methods

    internal override void Enable() => IL.ScuttlerControl.Hit += ScuttlerControl_Hit;
    
    internal override void Disable() => IL.ScuttlerControl.Hit -= ScuttlerControl_Hit;
    
    #endregion
}
