using KorzUtils.Helper;
using Modding;
using UnityEngine;

namespace TheHuntIsOn.Modules;

internal class ShadeModule : Module
{
    #region Properties

    public override string MenuDescription => "Blocks the shade from appearing (and their effect).";

    public static GameObject PlatformPrefab { get; set; }

    public static int ShadeHealth => PlayerData.instance.GetInt(nameof(PlayerData.maxHealthBase)) / 2
        * (PlayerData.instance.GetInt(nameof(PlayerData.nailSmithUpgrades)) * 4 + 5);

    #endregion

    #region Eventhandler

    private void PreventShadePenality()
    {
        if (!IsModuleUsed)
            return;
        HeroController.instance.AddGeoQuietly(PlayerData.instance.GetInt(nameof(PlayerData.geoPool)));
        PlayerData.instance.EndSoulLimiter();
        PlayerData.instance.SetInt(nameof(PlayerData.geoPool), 0);
    }

    private void SceneManager_activeSceneChanged(UnityEngine.SceneManagement.Scene oldScene, UnityEngine.SceneManagement.Scene newScene)
    {
        if (TheHuntIsOn.SaveData.ShadePlatformSpawn == ShadePlatformMode.Off)
            return;
        GameObject currentGameObject;
        switch (newScene.name)
        {
            case "Crossroads_04":
                // Salubra
                currentGameObject = GameObject.Instantiate(PlatformPrefab, new(139.7048f, 6.13f, 0), Quaternion.identity);
                currentGameObject.SetActive(true);
                // For blue lake
                if (TheHuntIsOn.SaveData.ShadePlatformSpawn == ShadePlatformMode.On || (PDHelper.HasDash && PDHelper.HasWalljump))
                {
                    currentGameObject = GameObject.Instantiate(PlatformPrefab, new(151.9073f, 15.0591f, 0), Quaternion.identity);
                    currentGameObject.SetActive(true);
                    currentGameObject = GameObject.Instantiate(PlatformPrefab, new(148.2994f, 19.5866f, 0), Quaternion.identity);
                    currentGameObject.SetActive(true);
                }
                break;
            case "Deepnest_East_06":
                currentGameObject = GameObject.Instantiate(PlatformPrefab, new(157.6747f, 17.4731f, 0), Quaternion.identity);
                currentGameObject.SetActive(true);
                currentGameObject = GameObject.Instantiate(PlatformPrefab, new(157.6747f, 22.2f, 0), Quaternion.identity);
                currentGameObject.SetActive(true);
                break;
            case "Abyss_18":
                if (TheHuntIsOn.SaveData.ShadePlatformSpawn == ShadePlatformMode.On || (TheHuntIsOn.SaveData.ShadePlatformSpawn == ShadePlatformMode.WithPrerequisites
                    && PlayerData.instance.GetBool(nameof(PlayerData.hasDash)) && PlayerData.instance.GetInt(nameof(PlayerData.fireballLevel)) > 0
                    && ShadeHealth > 20))
                {
                    currentGameObject = GameObject.Instantiate(PlatformPrefab, new(35.4f, 6.1f, 0), Quaternion.identity);
                    currentGameObject.SetActive(true);
                    currentGameObject = GameObject.Instantiate(PlatformPrefab, new(31.25f, 9.1f, 0), Quaternion.identity);
                    currentGameObject.SetActive(true);
                }
                break;
            case "Abyss_04":
                if (TheHuntIsOn.SaveData.ShadePlatformSpawn == ShadePlatformMode.On || (TheHuntIsOn.SaveData.ShadePlatformSpawn == ShadePlatformMode.WithPrerequisites
                    && PlayerData.instance.GetInt(nameof(PlayerData.fireballLevel)) > 0 && PlayerData.instance.GetBool(nameof(PlayerData.hasWalljump))))
                {
                    currentGameObject = GameObject.Instantiate(PlatformPrefab, new(84.6301f, 11.42f, 0), Quaternion.identity);
                    currentGameObject.SetActive(true);
                    currentGameObject = GameObject.Instantiate(PlatformPrefab, new(84.5f, 16.31f, 0), Quaternion.identity);
                    currentGameObject.SetActive(true);
                }
                break;
            case "Ruins2_04":
                currentGameObject = GameObject.Instantiate(PlatformPrefab, new(16.08f, 10.21f), Quaternion.identity);
                currentGameObject.SetActive(true);
                break;
            case "Waterways_04b":
                currentGameObject = GameObject.Instantiate(PlatformPrefab, new(42.52f, 15.56f), Quaternion.identity);
                currentGameObject.SetActive(true);
                currentGameObject = GameObject.Instantiate(PlatformPrefab, new(46.08f, 10.9f), Quaternion.identity);
                currentGameObject.SetActive(true);
                currentGameObject = GameObject.Instantiate(PlatformPrefab, new(50.73f, 7.6f), Quaternion.identity);
                currentGameObject.SetActive(true);
                break;
            case "Waterways_09":
                currentGameObject = GameObject.Instantiate(PlatformPrefab, new(15.79f, 7.43f), Quaternion.identity);
                currentGameObject.SetActive(true);
                currentGameObject = GameObject.Instantiate(PlatformPrefab, new(5.36f, 16.16f), Quaternion.identity);
                currentGameObject.SetActive(true);
                break;
            case "Fungus2_13":
                currentGameObject = GameObject.Instantiate(PlatformPrefab, new(5.57f, 7.37f), Quaternion.identity);
                currentGameObject.SetActive(true);
                break;
            case "Fungus2_14":
                currentGameObject = GameObject.Instantiate(PlatformPrefab, new(133.45f, 9.58f), Quaternion.identity);
                currentGameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    private bool ModHooks_GetPlayerBoolHook(string name, bool orig)
    {
        if (IsModuleUsed && name == "soulLimited")
            return false;
        return orig;
    }

    #endregion

    #region Methods

    internal override void Enable()
    {
        ModHooks.AfterPlayerDeadHook += PreventShadePenality;
        ModHooks.GetPlayerBoolHook += ModHooks_GetPlayerBoolHook;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    internal override void Disable()
    {
        ModHooks.AfterPlayerDeadHook -= PreventShadePenality;
        ModHooks.GetPlayerBoolHook -= ModHooks_GetPlayerBoolHook;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }

    #endregion
}
