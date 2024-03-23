using KorzUtils.Helper;
using Modding;
using Satchel.BetterMenus;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hkmp.Api.Client;
using Hkmp.Api.Server;
using TheHuntIsOn.HkmpAddon;
using TheHuntIsOn.Modules;
using TheHuntIsOn.Modules.HealthModules;
using UnityEngine;

namespace TheHuntIsOn;

public class TheHuntIsOn : Mod, IGlobalSettings<HuntGlobalSaveData>, ICustomMenuMod
{
    #region Constructors

    public TheHuntIsOn() 
    { 
        Instance = this;
        On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.OnEnter += PlayerDataBoolTest_OnEnter;
        On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter += IntCompare_OnEnter;
    }

    #endregion

    #region Properties

    public static TheHuntIsOn Instance { get; set; }

    public static HuntGlobalSaveData SaveData { get; set; } = new();

    public bool ToggleButtonInsideMenu { get; }

    public Menu MenuRef { get; set; }
    
    public HuntClientAddon HuntClientAddon { get; private set; }
    public HuntServerAddon HuntServerAddon { get; private set; }

    internal List<Module> Modules { get; set; } = new()
    {
        new ArenaModule(),
        new BenchModule(),
        new CharmModule(),
        new CompletionModule(),
        new DreamEntranceModule(),
        new DreamHealModule(),
        new ElevatorModule(),
        new EnemyModule(),
        new LifeseedModule(),
        new MaskModule(),
        new NotchModule(),
        new RespawnModule(),
        new ShadeModule(),
        new SpaModule(),
        new StagModule(),
        new TramModule(),
        new AutoTriggerBossModule(),
        new ItemNetworkModule()
    };

    #endregion

    #region Mod Setup

    public override string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString();

    public override List<(string, string)> GetPreloadNames() => new()
    {
        ("Crossroads_04", "_Scenery/plat_float_01"),
        ("White_Palace_03_hub", "doorWarp"),
        ("Crossroads_01", "_Transition Gates/door1")
    };

    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        HuntClientAddon = new HuntClientAddon();
        HuntServerAddon = new HuntServerAddon();
        ClientAddon.RegisterAddon(HuntClientAddon);
        ServerAddon.RegisterAddon(HuntServerAddon);
        
        On.UIManager.StartNewGame += UIManager_StartNewGame;
        On.UIManager.ContinueGame += UIManager_ContinueGame;
        On.UIManager.ReturnToMainMenu += UIManager_ReturnToMainMenu;
        ShadeModule.PlatformPrefab = preloadedObjects["Crossroads_04"]["_Scenery/plat_float_01"];
        EnemyModule.TeleporterPrefab = preloadedObjects["White_Palace_03_hub"]["doorWarp"];
        ElevatorModule.Door = preloadedObjects["Crossroads_01"]["_Transition Gates/door1"];
    }

    #endregion

    #region Eventhandler

    private System.Collections.IEnumerator UIManager_ReturnToMainMenu(On.UIManager.orig_ReturnToMainMenu orig, UIManager self)
    {
        yield return orig(self);
        foreach (Module module in Modules)
            module.Unload();
    }

    private void UIManager_ContinueGame(On.UIManager.orig_ContinueGame orig, UIManager self)
    {
        orig(self);
        CoroutineHelper.WaitForHero(() => HealthControl.Reset(PlayerData.instance.GetInt(nameof(PlayerData.maxHealth))) , true);
        foreach (Module module in Modules)
            module.Load();
    }

    private void UIManager_StartNewGame(On.UIManager.orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
    {
        orig(self, permaDeath, bossRush);
        HealthControl.Reset(5);
        foreach (Module module in Modules)
            module.Load();
    }

    private void PlayerDataBoolTest_OnEnter(On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.orig_OnEnter orig, HutongGames.PlayMaker.Actions.PlayerDataBoolTest self)
    {
        if (self.IsCorrectContext("Spell Control", "Knight", "Slug?"))
            self.Fsm.Variables.FindFsmFloat("Time Per MP Drain").Value *= SaveData.FocusSpeed;
        else if ((self.IsCorrectContext("Spell Control", "Knight", "Spore Cloud") || self.IsCorrectContext("Spell Control", "Knight", "Spore Cloud 2")))
            HeroController.instance.TakeMP(SaveData.FocusCost - 33);
        orig(self);
    }

    private void IntCompare_OnEnter(On.HutongGames.PlayMaker.Actions.IntCompare.orig_OnEnter orig, HutongGames.PlayMaker.Actions.IntCompare self)
    {
        if (self.IsCorrectContext("Spell Control", "Knight", null) && self.integer1.Name == "MP" && (self.State.Name == "Can Focus?"
            || self.State.Name == "Full HP?" || self.State.Name == "Full HP? 2"))
            self.integer2.Value = SaveData.FocusCost;
        orig(self);
    }

    #endregion

    #region Methods

    internal static bool IsModuleUsed<T>() where T : Module 
        => Instance.Modules.FirstOrDefault(x => x is T)?.IsModuleUsed ?? false;

    #endregion

    #region Interfaces

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
    {
        List<Element> elements = new()
        {
            new HorizontalOption("Role:", "Flag that indicates if player is a hunter or speedrunner.", new string[]{"Hunter", "Speedrunner" },
            x => SaveData.IsHunter = x == 0,
            () => SaveData.IsHunter ? 0 : 1),
            new CustomSlider("Focus Cost:", x => SaveData.FocusCost = (int)x, () => SaveData.FocusCost, 33, 99, true),
            new CustomSlider("Focus Speed:", x => SaveData.FocusSpeed = x, () => SaveData.FocusSpeed, 1, 3),
            new HorizontalOption("Shade Platform Mode", "Controls when/if platform spawn at Shade Skip location", new string[] {"Off", "Conditional", "On"},
            x => SaveData.ShadePlatformSpawn = (ShadePlatformMode)x,
            () => (int)SaveData.ShadePlatformSpawn)
        };
        foreach (Module module in Modules)
        {
            elements.Add(new HorizontalOption(module.GetModuleName(), module.MenuDescription, new string[] { "Off", "Both", "Hunter", "Speedrunner" },
            x => module.Affection = (ModuleAffection)x,
            () => (int)module.Affection));
        }
        elements.Add(new HorizontalOption("Retain bosses", "Causes the completion and enemy module to ignore bosses.", new string[] { "Off", "On" },
            x => Module.RetainBosses = x == 1,
            () => Module.RetainBosses ? 1 : 0));
        MenuRef ??= new("The Hunt is on", elements.ToArray());
        return MenuRef.GetMenuScreen(modListMenu);
    }

    public void OnLoadGlobal(HuntGlobalSaveData saveData)
    {
        SaveData = saveData ?? new();
        SaveData.AffectionTable ??= new();
        foreach (Module module in Modules)
            if (SaveData.AffectionTable.ContainsKey(module.GetType().Name))
                module.Affection = SaveData.AffectionTable[module.GetType().Name];
        Module.RetainBosses = SaveData.RetainBosses;
    }

    public HuntGlobalSaveData OnSaveGlobal()
    {
        HuntGlobalSaveData globalData = new()
        {
            FocusCost = SaveData.FocusCost,
            FocusSpeed = SaveData.FocusSpeed,
            IsHunter = SaveData.IsHunter,
            ShadePlatformSpawn = SaveData.ShadePlatformSpawn,
            RetainBosses = Module.RetainBosses
        };

        foreach (Module module in Modules)
            globalData.AffectionTable.Add(module.GetType().Name, module.Affection);
        return globalData;
    }

    #endregion
}