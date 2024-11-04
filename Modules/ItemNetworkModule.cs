using System;
using Hkmp.Api.Client;
using Hkmp.Api.Server;
using KorzUtils.Helper;
using Modding;
using TheHuntIsOn.HkmpAddon;

namespace TheHuntIsOn.Modules;

internal class ItemNetworkModule : Module
{
    #region Properties

    public override string MenuDescription => "Networks speedrunner obtained items and grants items to hunters.";

    public HuntClientAddon HuntClientAddon { get; private set; }

    public HuntServerAddon HuntServerAddon { get; private set; }

    /// <summary>
    /// Whether the HKMP client and server addons are loaded. This determines whether hooks are registered.
    /// </summary>
    private bool AddonsLoaded { get; }

    #endregion

    #region Constructors

    public ItemNetworkModule(bool loadAddons)
    {
        AddonsLoaded = loadAddons;
        if (!loadAddons) return;

        HuntClientAddon = new HuntClientAddon();
        HuntServerAddon = new HuntServerAddon();
        ClientAddon.RegisterAddon(HuntClientAddon);
        ServerAddon.RegisterAddon(HuntServerAddon);
    }

    #endregion

    #region EventHandler

    private int ModHooks_OnSetPlayerIntHook(string name, int orig)
    {
        // Make sure that the player obtaining items is the speedrunner
        if (!IsModuleUsed || TheHuntIsOn.SaveData.IsHunter)
        {
            return orig;
        }

        if (name == nameof(PlayerData.fireballLevel))
        {
            if (orig == 1)
            {
                // Speedrunner obtained Vengeful Spirit
                SendItemObtained(NetItem.VengefulSpirit);
            }
            else if (orig == 2)
            {
                // Speedrunner obtained Shade Soul
                SendItemObtained(NetItem.ShadeSoul);
            }
        }
        else if (name == nameof(PlayerData.quakeLevel))
        {
            if (orig == 1)
            {
                // Speedrunner obtained Desolate Dive
                SendItemObtained(NetItem.DesolateDive);
            }
            else if (orig == 2)
            {
                // Speedrunner obtained Descending Dark
                SendItemObtained(NetItem.DescendingDark);
            }
        }
        else if (name == nameof(PlayerData.screamLevel))
        {
            if (orig == 1)
            {
                // Speedrunner obtained Howling Wraiths
                SendItemObtained(NetItem.HowlingWraiths);
            }
            else if (orig == 2)
            {
                // Speedrunner obtained Abyss Shriek
                SendItemObtained(NetItem.AbyssShriek);
            }
        }

        return orig;
    }

    private bool ModHooks_OnSetPlayerBoolHook(string name, bool orig)
    {
        // Make sure that the player obtaining items is the speedrunner
        if (!IsModuleUsed || TheHuntIsOn.SaveData.IsHunter)
        {
            return orig;
        }

        var pd = PlayerData.instance;

        // Count the number of movement items before this new boolean is set
        var numMovement = 0;
        if (pd.hasDash) numMovement++;
        if (pd.hasWalljump) numMovement++;
        if (pd.hasSuperDash) numMovement++;
        if (pd.hasDoubleJump) numMovement++;
        if (pd.hasAcidArmour) numMovement++;

        // This method will network the number of movement items we have obtained, based on the number of existing
        // movement items and the newly set boolean from the hook
        void SendConditionalMovement()
        {
            if (numMovement == 0) SendItemObtained(NetItem.Movement1);
            if (numMovement == 1) SendItemObtained(NetItem.Movement2);
            if (numMovement == 2) SendItemObtained(NetItem.Movement3);
            if (numMovement == 3) SendItemObtained(NetItem.Movement4);
            if (numMovement == 4) SendItemObtained(NetItem.Movement5);
            if (numMovement == 5) SendItemObtained(NetItem.Movement6);
        }

        if (orig)
        {
            if (name == nameof(PlayerData.hasDash))
            {
                SendItemObtained(NetItem.MothwingCloak);
                SendConditionalMovement();
            }
            else if (name == nameof(PlayerData.hasWalljump))
            {
                SendItemObtained(NetItem.MantisClaw);
                SendConditionalMovement();
            }
            else if (name == nameof(PlayerData.hasSuperDash))
            {
                SendItemObtained(NetItem.CrystalHeart);
                SendConditionalMovement();
            }
            else if (name == nameof(PlayerData.hasDoubleJump))
            {
                SendItemObtained(NetItem.MonarchWings);
                SendConditionalMovement();
            }
            else if (name == nameof(PlayerData.hasAcidArmour))
            {
                SendItemObtained(NetItem.IsmasTear);
                SendConditionalMovement();
            }
            else if (name == nameof(PlayerData.hasShadowDash))
            {
                SendItemObtained(NetItem.ShadeCloak);
                SendConditionalMovement();
            }
            else if (name == nameof(PlayerData.hasDreamNail))
            {
                SendItemObtained(NetItem.DreamNail);
            }
            else if (name == nameof(PlayerData.hasCyclone))
            {
                SendItemObtained(NetItem.CycloneSlash);
            }
            else if (name == nameof(PlayerData.hasUpwardSlash))
            {
                SendItemObtained(NetItem.DashSlash);
            }
            else if (name == nameof(PlayerData.hasDashSlash))
            {
                SendItemObtained(NetItem.GreatSlash);
            }
        }

        return orig;
    }

    private void NetManager_OnGrantItemsEvent(NetItem[] netItems)
    {
        // Check if the player is not the speedrunner
        if (!IsModuleUsed || !TheHuntIsOn.SaveData.IsHunter)
            return;
        
        LogHelper.Write<TheHuntIsOn>("OnGrantItems:");

        var pd = PlayerData.instance;

        foreach (var netItem in netItems)
        {
            LogHelper.Write<TheHuntIsOn>($"  {netItem}");
            switch (netItem)
            {
                case NetItem.VengefulSpirit:
                    PDHelper.FireballLevel = 1;
                    break;
                case NetItem.DesolateDive:
                    PDHelper.QuakeLevel = 1;
                    break;
                case NetItem.HowlingWraiths:
                    PDHelper.ScreamLevel = 1;
                    break;
                case NetItem.ShadeSoul:
                    PDHelper.FireballLevel = 2;
                    break;
                case NetItem.DescendingDark:
                    PDHelper.QuakeLevel = 2;
                    break;
                case NetItem.AbyssShriek:
                    PDHelper.ScreamLevel = 2;
                    break;
                case NetItem.MothwingCloak:
                    PDHelper.HasDash = true;
                    PDHelper.CanDash = true;
                    break;
                case NetItem.MantisClaw:
                    PDHelper.HasWalljump = true;
                    break;
                case NetItem.CrystalHeart:
                    PDHelper.HasSuperDash = true;
                    break;
                case NetItem.MonarchWings:
                    PDHelper.HasDoubleJump = true;
                    break;
                case NetItem.IsmasTear:
                    PDHelper.HasAcidArmour = true;
                    PlayMakerFSM.BroadcastEvent("GET ACID ARMOUR");
                    break;
                case NetItem.ShadeCloak:
                    PDHelper.HasDash = true;
                    PDHelper.CanDash = true;
                    PDHelper.HasShadowDash = true;
                    break;
                case NetItem.DreamNail:
                    PDHelper.HasDreamNail = true;
                    break;
                case NetItem.CycloneSlash:
                    PDHelper.HasNailArt = true;
                    PDHelper.HasCyclone = true;
                    break;
                case NetItem.DashSlash:
                    PDHelper.HasNailArt = true;
                    PDHelper.HasUpwardSlash = true;
                    break;
                case NetItem.GreatSlash:
                    PDHelper.HasNailArt = true;
                    PDHelper.HasDashSlash = true;
                    break;
                case NetItem.Mask:
                    if (PlayerData.instance.maxHealthBase < 9)
                    {
                        HeroController.instance.MaxHealth();
                        HeroController.instance.AddToMaxHealth(1);
                        PlayMakerFSM.BroadcastEvent("MAX HP UP");
                    }

                    break;
                case NetItem.NailUpgrade:
                    // Find the first nail upgrade that the player does not have yet
                    // In other words find the first damage value from the array for which the player's nail damage
                    // is lower and set it to that value
                    var damages = new[] { 9, 13, 17, 21 };
                    foreach (var damage in damages)
                    {
                        if (pd.nailDamage < damage)
                        {
                            pd.honedNail = true;
                            pd.nailDamage = damage;
                            PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
                            pd.nailSmithUpgrades += 1;
                            break;
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    #endregion

    #region Methods

    private void SendItemObtained(NetItem item)
    {
        LogHelper.Write<TheHuntIsOn>($"Sending obtained item: {item}");
        HuntClientAddon.NetManager.SendItemObtained(item);
    }

    internal override void Enable()
    {
        if (!AddonsLoaded) return;

        ModHooks.SetPlayerIntHook += ModHooks_OnSetPlayerIntHook;
        ModHooks.SetPlayerBoolHook += ModHooks_OnSetPlayerBoolHook;

        HuntClientAddon.NetManager.GrantItemsEvent += NetManager_OnGrantItemsEvent;
    }

    internal override void Disable()
    {
        if (!AddonsLoaded) return;

        ModHooks.SetPlayerIntHook -= ModHooks_OnSetPlayerIntHook;
        ModHooks.SetPlayerBoolHook -= ModHooks_OnSetPlayerBoolHook;

        HuntClientAddon.NetManager.GrantItemsEvent -= NetManager_OnGrantItemsEvent;
    }

    #endregion
}