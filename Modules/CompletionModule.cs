using KorzUtils.Helper;
using Modding;
using System.Collections.Generic;
using System.Linq;

namespace TheHuntIsOn.Modules;

internal class CompletionModule : Module
{
    #region Members

    private List<string> _affectedPlayerData = new()
    {
       "gladeDoorOpened",
       "openedTown",
       "openedCrossroads",
       "openedGreenpath",
       "openedRuins1",
       "openedRuins2",
       "openedFungalWastes",
       "openedRoyalGardens",
       "openedRestingGrounds",
       "openedDeepnest",
       "openedStagNest",
       "openedHiddenStation",
       "oneWayArchive",
       "cityBridge1",
       "cityBridge2",
       "cityLift1",
       "cityLift2",
       "openedMageDoor",
       "openedMageDoor_v2",
       "brokenMageWindow",
       "brokenMageWindowGlass",
       "ruins1_5_tripleDoor",
       "openedCityGate",
       "city2_sewerDoor",
       "openedLoveDoor",
       "completedQuakeArea",
       "waterwaysGate",
       "openedWaterwaysManhole",
       "waterwaysAcidDrained",
       "dungDefenderWallBroken",
       "brokeMinersWall",
       "steppedBeyondBridge",
       "deepnestBridgeCollapsed",
       "spiderCapture",
       "deepnest26b_switch",
       "openedRestingGrounds02",
       "restingGroundsCryptWall",
       "openedGardensStagStation",
       "blizzardEnded",
       "abyssGateOpened",
       "abyssLighthouse",
       "blueVineDoor",
       "shamanPillar",
       "brettaRescued",
       "zoteDefeated",
       "bathHouseWall",
       "brokeMinersWall",
       "crossroadsMawlekWall",
       "deepnestWall",
       "dungDefenderWallBroken",
       "outskirtsWall",
       "falseKnightWallBroken",
       "zoteStatueWallBroken",
       "giantFlyDefeated",
       "blocker1Defeated",
       "blocker2Defeated",
       "encounteredHornet",
       "savedByHornet",
       "mageLordEncountered",
       "mageLordEncountered_2",
       "duskKnightDefeated",
       "flukeMotherEncountered",
       "defeatedDoubleBlockers",
       "defeatedNightmareGrimm",
       "visitedDirtmouth",
       "visitedCrossroads",
       "visitedGreenpath",
       "visitedFungus",
       "visitedHive",
       "visitedCrossroadsInfected",
       "visitedRuins",
       "visitedMines",
       "visitedRoyalGardens",
       "visitedWaterways",
       "visitedAbyss",
       "visitedOutskirts",
       "visitedWhitePalace",
       "visitedCliffs",
       "visitedAbyss",
       "visitedAbyssLower",
       "visitedGodhome",
       "visitedMines10",
       "openedBlackEggDoor"
    };
    private List<string> _affectedBossPlayerData = new()
    {
        "killedInfectedKnight", // Boss Flag
        "falseKnightDefeated", // Boss Flag
        "mawlekDefeated", // Boss Flag
        "giantBuzzerDefeated", // Boss Flag
        "defeatedDungDefender", // Boss Flag
        "defeatedMantisLords", // Boss Flag
        "defeatedMegaBeamMiner", // Boss Flag
        "defeatedMegaBeamMiner2", // Boss Flag
        "defeatedMegaJelly", // Boss Flag
        "hornet1Defeated", // Boss Flag
        "collectorDefeated", // Boss Flag
        "hornetOutskirtsDefeated", // Boss Flag
        "mageLordDefeated", // Boss Flag
        "flukeMotherDefeated", // Boss Flag
        "megaMossChargerDefeated" // Boss Flag
    };
    private List<SceneBoolData> _affectedBoolData = new();

    #endregion

    #region Constructors

    public CompletionModule()
    {
        _affectedBoolData = ResourceHelper.LoadJsonResource<TheHuntIsOn, List<SceneBoolData>>("SceneData.json");
    }

    #endregion

    #region Properties

    public override string MenuDescription => "Unlocks Hallownest as a whole.";

    #endregion

    #region Eventhandler

    private bool ModHooks_GetPlayerBoolHook(string name, bool orig)
    {
        if (!IsModuleUsed)
            return orig;
        if (_affectedPlayerData.Contains(name))
            return true;
        else if (_affectedBossPlayerData.Contains(name))
        {
            if (RetainBosses)
                return orig;
            else
                return true;
        }
        else
            return orig;
    }

    private PersistentBoolData SceneData_FindMyState_PersistentBoolData(On.SceneData.orig_FindMyState_PersistentBoolData orig, SceneData self, PersistentBoolData persistentBoolData)
    {
        if (!IsModuleUsed || _affectedBoolData.FirstOrDefault(x => x.id == persistentBoolData.id && x.sceneName == persistentBoolData.sceneName) is not SceneBoolData sceneBool)
            return orig(self, persistentBoolData);
        if (RetainBosses && sceneBool.BossFlag)
            return orig(self, persistentBoolData);
        else
            return sceneBool;
    }

    private void IntCompare_OnEnter(On.HutongGames.PlayMaker.Actions.IntCompare.orig_OnEnter orig, HutongGames.PlayMaker.Actions.IntCompare self)
    {
        if (IsModuleUsed && self.IsCorrectContext("Destroy if Quake", "Battle Gate (1)", "Check"))
            self.lessThan = self.greaterThan;
        orig(self);
    }

    private void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        // Makes Hornet 2 accessible
        if (IsModuleUsed && self.gameObject.name == "blizzard_wall" && self.FsmName == "FSM")
            self.GetState("Pause").AdjustTransitions("Deactivate");
        orig(self);
    }

    #endregion

    #region Methods

    internal override void Enable()
    {
        On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
        ModHooks.GetPlayerBoolHook += ModHooks_GetPlayerBoolHook;
        On.SceneData.FindMyState_PersistentBoolData += SceneData_FindMyState_PersistentBoolData;
        On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter += IntCompare_OnEnter;
    }

    internal override void Disable()
    {
        On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;
        ModHooks.GetPlayerBoolHook -= ModHooks_GetPlayerBoolHook;
        On.SceneData.FindMyState_PersistentBoolData -= SceneData_FindMyState_PersistentBoolData;
        On.HutongGames.PlayMaker.Actions.IntCompare.OnEnter -= IntCompare_OnEnter;
    }

    #endregion
}
