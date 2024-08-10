using KorzUtils.Helper;
using TheHuntIsOn.Components;
using UnityEngine;

namespace TheHuntIsOn.Modules;

internal class IntangibleGatesModule : Module
{
    #region Properties

    public override string MenuDescription => "Makes gates of arenas intangible for the player";

    #endregion

    #region Methods

    internal override void Enable()
    {
        CoroutineHelper.WaitForHero(() =>
            {
                if (HeroController.instance.GetComponent<IgnoreGates>() == null)
                    HeroController.instance.gameObject.AddComponent<IgnoreGates>();
            }, true);
    }

    internal override void Disable(){}

    #endregion
}
