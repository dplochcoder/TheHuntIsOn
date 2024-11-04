using System;
using Hkmp.Api.Client;
using Hkmp.Api.Server;
using KorzUtils.Helper;
using Modding;
using Satchel;
using TheHuntIsOn.HkmpAddon;

namespace TheHuntIsOn.Modules;

internal class EventNetworkModule : Module
{
    #region Properties

    public override string MenuDescription => "Networks speedrunner caused events and grants items to hunters.";

    public HuntClientAddon HuntClientAddon { get; private set; }

    public HuntServerAddon HuntServerAddon { get; private set; }
    
    private bool AreAddonsLoaded { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initialize method used to register HKMP client and server addons after the module affections has been set.
    /// If the module should not affect any players, we do not register the addons. Otherwise, we do.
    /// This way if module affection is set to none and the game is restarted, players can connect to server that
    /// do not have the TheHuntIsOn server addon.
    /// </summary>
    public void Initialize()
    {
        if (Affection == ModuleAffection.None)
        {
            AreAddonsLoaded = false;
            return;
        }

        HuntClientAddon = new HuntClientAddon();
        HuntServerAddon = new HuntServerAddon();
        ClientAddon.RegisterAddon(HuntClientAddon);
        ServerAddon.RegisterAddon(HuntServerAddon);

        AreAddonsLoaded = true;
    }

    #endregion

    #region EventHandler

    private int ModHooks_OnSetPlayerIntHook(string name, int orig)
    {
        // Make sure that the player causing changes is the speedrunner
        if (!IsModuleUsed || TheHuntIsOn.SaveData.IsHunter)
        {
            return orig;
        }

        if (name == nameof(PlayerData.fireballLevel))
        {
            if (orig == 1)
            {
                // Speedrunner obtained Vengeful Spirit
                SendEvent(NetEvent.VengefulSpirit);
            }
            else if (orig == 2)
            {
                // Speedrunner obtained Shade Soul
                SendEvent(NetEvent.ShadeSoul);
            }
        }
        else if (name == nameof(PlayerData.quakeLevel))
        {
            if (orig == 1)
            {
                // Speedrunner obtained Desolate Dive
                SendEvent(NetEvent.DesolateDive);
            }
            else if (orig == 2)
            {
                // Speedrunner obtained Descending Dark
                SendEvent(NetEvent.DescendingDark);
            }
        }
        else if (name == nameof(PlayerData.screamLevel))
        {
            if (orig == 1)
            {
                // Speedrunner obtained Howling Wraiths
                SendEvent(NetEvent.HowlingWraiths);
            }
            else if (orig == 2)
            {
                // Speedrunner obtained Abyss Shriek
                SendEvent(NetEvent.AbyssShriek);
            }
        }
        else if (name == nameof(PlayerData.maxHealth))
        {
            if (orig == PlayerData.instance.GetInt(nameof(PlayerData.maxHealth))) return orig;
            
            // Speedrunner obtained a full Mask
            SendEvent(NetEvent.Mask);
        }
        else if (name == nameof(PlayerData.MPReserveMax))
        {
            if (orig == PlayerData.instance.GetInt(nameof(PlayerData.MPReserveMax))) return orig;
            
            // Speedrunner obtained a full Soul Vessel
            SendEvent(NetEvent.SoulVessel);
        }
        else if (name == nameof(PlayerData.grubsCollected))
        {
            // Speedrunner freed a grub
            SendEvent(NetEvent.Grub);
        }

        return orig;
    }

    private bool ModHooks_OnSetPlayerBoolHook(string name, bool orig)
    {
        // Make sure that the player causing changes is the speedrunner
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
            if (numMovement == 0) SendEvent(NetEvent.Movement1);
            if (numMovement == 1) SendEvent(NetEvent.Movement2);
            if (numMovement == 2) SendEvent(NetEvent.Movement3);
            if (numMovement == 3) SendEvent(NetEvent.Movement4);
            if (numMovement == 4) SendEvent(NetEvent.Movement5);
            if (numMovement == 5) SendEvent(NetEvent.Movement6);
        }

        if (orig)
        {
            if (name == nameof(PlayerData.hasDash))
            {
                SendEvent(NetEvent.MothwingCloak);
                SendConditionalMovement();
            }
            else if (name == nameof(PlayerData.hasWalljump))
            {
                SendEvent(NetEvent.MantisClaw);
                SendConditionalMovement();
            }
            else if (name == nameof(PlayerData.hasSuperDash))
            {
                SendEvent(NetEvent.CrystalHeart);
                SendConditionalMovement();
            }
            else if (name == nameof(PlayerData.hasDoubleJump))
            {
                SendEvent(NetEvent.MonarchWings);
                SendConditionalMovement();
            }
            else if (name == nameof(PlayerData.hasAcidArmour))
            {
                SendEvent(NetEvent.IsmasTear);
                SendConditionalMovement();
            }
            else if (name == nameof(PlayerData.hasShadowDash))
            {
                SendEvent(NetEvent.ShadeCloak);
                SendConditionalMovement();
            }
            else if (name == nameof(PlayerData.hasDreamNail))
            {
                SendEvent(NetEvent.DreamNail);
            }
            else if (name == nameof(PlayerData.hasCyclone))
            {
                SendEvent(NetEvent.CycloneSlash);
            }
            else if (name == nameof(PlayerData.hasUpwardSlash))
            {
                SendEvent(NetEvent.DashSlash);
            }
            else if (name == nameof(PlayerData.hasDashSlash))
            {
                SendEvent(NetEvent.GreatSlash);
            }
            else if (name 
                     is nameof(PlayerData.lurienDefeated) 
                     or nameof(PlayerData.hegemolDefeated) 
                     or nameof(PlayerData.monomonDefeated))
            {
                SendEvent(NetEvent.Dreamer);
            }
            else if (name
                     is nameof(PlayerData.openedCrossroads)
                     or nameof(PlayerData.openedDeepnest)
                     or nameof(PlayerData.openedGreenpath)
                     or nameof(PlayerData.openedRuins1)
                     or nameof(PlayerData.openedRuins2)
                     or nameof(PlayerData.openedFungalWastes)
                     or nameof(PlayerData.openedHiddenStation)
                     or nameof(PlayerData.openedRoyalGardens)
                     or nameof(PlayerData.tollBenchCity)
                     or nameof(PlayerData.tollBenchAbyss)
                     or nameof(PlayerData.tollBenchQueensGardens)
                     or nameof(PlayerData.cityLift1))
            {
                SendEvent(NetEvent.Toll);
            }
            else if (name is nameof(PlayerData.cityBridge1) or nameof(PlayerData.cityBridge2))
            {
                SendEvent(NetEvent.LeverHit);
            }
        }

        return orig;
    }
    
    private void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);
        
        // Make sure that the player causing changes is the speedrunner
        if (!IsModuleUsed || TheHuntIsOn.SaveData.IsHunter)
        {
            return;
        }

        if (self.name.Equals("Inspect Region") && self.Fsm.Name.Equals("Control") && 
            self.gameObject.scene.name is "Room_Tram" or "Room_Tram_RG")
        {
            self.InsertCustomAction("Send Event", () =>
            {
                SendEvent(NetEvent.Tram);
            }, 1);
        }
        else if (self.name.Equals("Stag") && self.Fsm.Name.Equals("Stag Control"))
        {
            self.InsertCustomAction("Fade", () =>
            {
                SendEvent(NetEvent.Stag);
            }, 0);
        }
        else if (self.name.Equals("Toll Gate Machine") && self.Fsm.Name.Equals("Toll Machine"))
        {
            if (self.gameObject.scene.name is "Mines_33" or "Fungus1_31")
            {
                self.InsertCustomAction("Open Gates", () =>
                {
                    SendEvent(NetEvent.Toll);
                }, 2);
            }
        }
        else if (self.name.Equals("Dreamnail Hit") && self.Fsm.Name.Equals("ghost_npc_dreamnail"))
        {
            // self.InsertCustomAction("Send", () =>
            // {
            //     SendEvent(NetEvent.DreamWarrior);
            // }, 1);
        }
        else if (self.name.Equals("Ghost Warrior NPC") && self.Fsm.Name.Equals("Conversation Control"))
        {
            self.InsertCustomAction("Start Fight", () =>
            {
                SendEvent(NetEvent.DreamWarrior);
            }, 7);
        }
        else if (self.name.Equals("UI List") && self.Fsm.Name.Equals("Confirm Control") && 
                 self.gameObject.scene.name != "Ruins1_05b")
        {
            var yesState = self.Fsm.GetState("Yes");
            if (yesState == null) return;
            
            var numActions = yesState.Actions.Length;
            if (numActions > 3) return;
            
            self.InsertCustomAction("Yes", () =>
            {
                SendEvent(NetEvent.ShopPurchase);
            }, 0);
        }
        else if (self.name.StartsWith("Gate Switch") ||
                 self.name.StartsWith("Toll Gate Switch") ||
                 self.name.StartsWith("Waterways_Crank_Lever") ||
                 self.name.StartsWith("Ruins Lever") ||
                 self.name.StartsWith("Mantis Lever") ||
                 self.name.StartsWith("Mines Lever") ||
                 self.name.StartsWith("WP Lever") ||
                 self.name.StartsWith("White Palace Orb Lever"))
        {
            if (self.Fsm.Name.Equals("Switch Control"))
            {
                self.InsertCustomAction("Hit", () =>
                {
                    SendEvent(NetEvent.LeverHit);
                }, 0);
            }
        }
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
                case NetItem.SoulVessel:
                    if (PlayerData.instance.MPReserveMax < 99)
                    {
                        HeroController.instance.AddToMaxMPReserve(33);
                        PlayMakerFSM.BroadcastEvent("NEW SOUL ORB");
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

    private void SendEvent(NetEvent netEvent)
    {
        LogHelper.Write<TheHuntIsOn>($"Sending triggered event: {netEvent}");
        HuntClientAddon.NetManager.SendEvent(netEvent);
    }

    internal override void Enable()
    {
        if (!AreAddonsLoaded) return;

        ModHooks.SetPlayerIntHook += ModHooks_OnSetPlayerIntHook;
        ModHooks.SetPlayerBoolHook += ModHooks_OnSetPlayerBoolHook;
        On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;

        HuntClientAddon.NetManager.GrantItemsEvent += NetManager_OnGrantItemsEvent;
    }

    internal override void Disable()
    {
        if (!AreAddonsLoaded) return;

        ModHooks.SetPlayerIntHook -= ModHooks_OnSetPlayerIntHook;
        ModHooks.SetPlayerBoolHook -= ModHooks_OnSetPlayerBoolHook;
        On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;

        HuntClientAddon.NetManager.GrantItemsEvent -= NetManager_OnGrantItemsEvent;
    }

    #endregion
}