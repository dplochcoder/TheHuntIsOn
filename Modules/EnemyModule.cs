using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using KorzUtils.Helper;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheHuntIsOn.Modules.HealthModules;
using UnityEngine;

namespace TheHuntIsOn.Modules;

internal class EnemyModule : Module
{
    #region Members

    private static GameObject _dreamGate;

    #endregion

    #region Properties

    public static GameObject TeleporterPrefab { get; set; }

    public static GameObject DreamGate
    {
        get
        {
            if (_dreamGate == null)
                _dreamGate = HeroController.instance.gameObject.LocateMyFSM("Dream Nail")
                    .GetState("Spawn Gate")
                    .GetFirstAction<SpawnObjectFromGlobalPool>().gameObject.Value;

            return _dreamGate;
        }
    }

    public override string MenuDescription => "Disables all enemies.";

    #endregion

    #region Eventhandler

    private bool ModHooks_OnEnableEnemyHook(GameObject enemy, bool isAlreadyDead)
    {
        if (!IsModuleUsed)
            return isAlreadyDead;
        if (RetainBosses)
        {
            // We use the hp as an indicator if the enemy is a boss.
            // As early game bosses do not have 200+ hp, we call them here seperately.
            // Also we exclude the "adds" from Collector, Gruz Mother and Traitor Lord.
            if (enemy.name == "Mega Moss Charger" || enemy.name == "Giant Fly"
                || enemy.name == "False Knight New" || enemy.name == "Mage Knight"
                || enemy.scene.name == "Fungus3_23_boss" || enemy.name == "Head"
                || enemy.scene.name == "Ruins2_11_boss" || (enemy.name.Contains("Fly") && enemy.scene.name == "Crossroads_04"))
                return false;
            HealthManager healthManager = enemy.GetComponent<HealthManager>();
            return healthManager.hp < 200 || isAlreadyDead;
        }
        else
            return enemy.name != "Radiance";
    }

    private void BlockDreamBosses(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (IsModuleUsed)
        {
            if (self.gameObject.name == "Battle Scene" && !RetainBosses)
            {
                if (self.FsmName == "Battle Control" && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Dream_01_False_Knight")
                    self.GetState("Detect").ClearTransitions();
                else if (self.FsmName == "Battle Scene" && (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Dream_04_White_Defender"
                    || UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Waterways_15"))
                    self.GetState("Idle").ClearTransitions();
            }
            else if (self.gameObject.name == "Dream Mage Lord" && self.FsmName == "Mage Lord" && !RetainBosses)
            {
                // Remove fight start trigger.
                self.transform.Find("Start Range").gameObject.SetActive(false);
                // Remove ground
                GameObject.Find("mage_window").SetActive(false);
                // Remove the blocker that would appear upon entering the lower floor.
                GameObject[] groundObjects = GameObject.FindObjectsOfType<GameObject>().Where(x => x.name.StartsWith("Dream Gate Phase 2")).ToArray();
                for (int i = 0; i < groundObjects.Length; i++)
                    GameObject.Destroy(groundObjects[i]);
            }
            else if (self.gameObject.name == "Lost Kin" && self.FsmName == "IK Control" && !RetainBosses)
                GameObject.Destroy(self.transform.Find("Rewake Range").gameObject);
            else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Dream_Mighty_Zote"
                || UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Room_Bretta_Basement"
                && !RetainBosses)
            {
                if (self.gameObject.name == "Grey Prince" && self.FsmName == "Control")
                    self.GetState("Dormant").ClearTransitions();
                else if (self.gameObject.name == "Audience" && self.FsmName == "Battle Control")
                {
                    self.GetState("Zote Start").RemoveAllActions();
                    self.GetState("Zote Start").AddActions(() =>
                    {
                        GameObject teleporterSprite = GameObject.Instantiate(DreamGate);
                        teleporterSprite.transform.position = new(8.8f, 5f);
                        GameObject teleporter = GameObject.Instantiate(ElevatorModule.Door);
                        teleporter.name = "Zote Escape";
                        teleporter.transform.position = new(8.8f, 6.1f);
                        teleporter.SetActive(true);
                        teleporter.transform.localScale = new(0.5f, 1f, 1f);

                        PlayMakerFSM teleportFsm = teleporter.GetComponent<PlayMakerFSM>();
                        if (TheHuntIsOn.IsModuleUsed<DreamHealModule>())
                        {
                            teleportFsm.FsmVariables.FindFsmString("Entry Gate").Value = "door1";
                            teleportFsm.FsmVariables.FindFsmString("New Scene").Value = "Room_Bretta_Basement";
                            teleportFsm.GetState("Enter").GetLastAction<SendEventByName>().sendEvent.Value = "FADE OUT";
                        }
                        else
                        {
                            teleportFsm.FsmVariables.FindFsmString("Entry Gate").Value = "top1";
                            teleportFsm.FsmVariables.FindFsmString("New Scene").Value = "Tutorial_01";
                        }
                        teleportFsm.FsmVariables.FindFsmString("Prompt Name").Value = "Exit";
                        teleportFsm.FsmVariables.FindFsmBool("Crossroads Ascent").Value = false;
                    });
                }
            }
            else if (self.gameObject.name == "Gate" && self.FsmName == "Control"
                && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.StartsWith("Room_Final_Boss"))
            {
                self.GetState("Activate").Actions = Array.Empty<FsmStateAction>();
                self.GetState("Activate").AddActions(() =>
                {
                    GameObject teleporterSprite = GameObject.Instantiate(DreamGate);
                    teleporterSprite.transform.position = new(66.06f, 4.9f);
                    GameObject teleporter = GameObject.Instantiate(TeleporterPrefab);
                    teleporter.name = "Radiance Enter";
                    teleporter.transform.position = new(66.06f, 6.1f);
                    teleporter.SetActive(true);
                    teleporter.transform.localScale = new(0.5f, 1f, 1f);

                    PlayMakerFSM teleportFsm = teleporter.GetComponent<PlayMakerFSM>();
                    teleportFsm.FsmVariables.FindFsmString("Entry Gate").Value = "door1";
                    teleportFsm.FsmVariables.FindFsmString("New Scene").Value = "Dream_Final_Boss";
                    teleportFsm.GetState("Change Scene").GetFirstAction<BeginSceneTransition>().preventCameraFadeOut = true;
                    GameManager.instance.StartCoroutine(ModifyRadianceRoom());
                });
            }
            else if (self.gameObject.name == "Fk Break Wall" && self.FsmName == "Control" && RetainBosses)
                self.GetState("Pause").AdjustTransitions("Initial");
        }
        orig(self);
    }

    private void BeginSceneTransition_OnEnter(On.HutongGames.PlayMaker.Actions.BeginSceneTransition.orig_OnEnter orig, HutongGames.PlayMaker.Actions.BeginSceneTransition self)
    {
        if (IsModuleUsed && self.IsCorrectContext("Door Control", "Radiance Enter", "Change Scene"))
            GameManager.instance.StartCoroutine(ModifyRadianceRoom());
        orig(self);
    }

    #endregion

    #region Methods

    internal override void Enable()
    {
        ModHooks.OnEnableEnemyHook += ModHooks_OnEnableEnemyHook;
        On.PlayMakerFSM.OnEnable += BlockDreamBosses;
        On.HutongGames.PlayMaker.Actions.BeginSceneTransition.OnEnter += BeginSceneTransition_OnEnter;
    }

    internal override void Disable()
    {
        ModHooks.OnEnableEnemyHook -= ModHooks_OnEnableEnemyHook;
        On.PlayMakerFSM.OnEnable -= BlockDreamBosses;
        On.HutongGames.PlayMaker.Actions.BeginSceneTransition.OnEnter -= BeginSceneTransition_OnEnter;
    }

    private IEnumerator ModifyRadianceRoom()
    {
        string finalBossRoom = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        while (currentScene == finalBossRoom)
        {
            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            yield return null;
        }
        if (currentScene != "Dream_Final_Boss")
            yield break;
        // Since entering dreams doesn't lift the fade plane, we remove them manually.
        yield return new WaitForFinishedEnteringScene();
        GameManager.instance.FadeSceneIn();

        if (RetainBosses)
            // Let the UI reappear.
            GameObject.Find("_GameCameras/HudCamera/Hud Canvas").LocateMyFSM("Slide Out").SendEvent("IN");
        else
        {
            GameObject bossControl = GameObject.Find("Boss Control");
            // Spawn all platforms
            // Detach them from Radiance
            List<GameObject> platforms = new();
            Transform platformSetParent = bossControl.transform.Find("Plat Sets");
            for (int i = 1; i < platformSetParent.childCount; i++)
            {
                Transform set = platformSetParent.GetChild(i);
                foreach (Transform platform in set)
                    if (platform.gameObject.LocateMyFSM("radiant_plat") != null)
                        platforms.Add(platform.gameObject);
            }
            // Remove initial platforms
            GameObject.Destroy(platformSetParent.GetChild(0).gameObject);

            foreach (GameObject platform in platforms)
            {
                PlayMakerFSM fsm = platform.LocateMyFSM("radiant_plat");
                fsm.GetState("Idle").ClearTransitions();
                fsm.SendEvent("APPEAR");
            }

            // To prevent the camera from behaving weirdly.
            GameObject.Destroy(bossControl.transform.Find("CamLocks/CamLock Challenge").gameObject);
            bossControl.transform.Find("CamLocks").gameObject.AddComponent<ToggleCameraLock>();

            // Deletes the spawn trigger, although this should be reachable anyway. But just in case.
            GameObject.Destroy(bossControl.transform.Find("Challenge Prompt Radiant").gameObject);
            // Let the UI reappear.
            yield return new WaitUntil(() => HeroController.instance.acceptingInput);
            GameObject.Find("_GameCameras/HudCamera/Hud Canvas").LocateMyFSM("Slide Out").SendEvent("IN");

            GameObject teleporterSprite = GameObject.Instantiate(DreamGate);
            teleporterSprite.transform.position = new(49.8f, 20.4f);
            GameObject teleporter = GameObject.Instantiate(ElevatorModule.Door);
            teleporter.name = "Radiance Escape";
            teleporter.transform.position = new(49.8f, 21.4f);
            teleporter.SetActive(true);
            teleporter.transform.localScale = new(0.5f, 1f, 1f);

            PlayMakerFSM teleportFsm = teleporter.GetComponent<PlayMakerFSM>();
            if (TheHuntIsOn.IsModuleUsed<DreamHealModule>())
            {
                teleportFsm.FsmVariables.FindFsmString("Entry Gate").Value = "right1";
                teleportFsm.FsmVariables.FindFsmString("New Scene").Value = "Room_Final_Boss_Atrium";
            }
            else
            {
                teleportFsm.FsmVariables.FindFsmString("Entry Gate").Value = "top1";
                teleportFsm.FsmVariables.FindFsmString("New Scene").Value = "Tutorial_01";
            }
            teleportFsm.FsmVariables.FindFsmBool("Crossroads Ascent").Value = false;
            teleportFsm.FsmVariables.FindFsmString("Prompt Name").Value = "Exit";
        }
    }

    #endregion
}
