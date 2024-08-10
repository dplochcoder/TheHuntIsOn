using UnityEngine;

namespace TheHuntIsOn.Modules;

internal class HelperPlatformModule : Module
{
    #region Properties

    public override string MenuDescription => "Places some platforms, that helps reaching certain ledges."; 

    #endregion

    #region Eventhandler

    private void SceneManager_activeSceneChanged(UnityEngine.SceneManagement.Scene oldScene, UnityEngine.SceneManagement.Scene newScene)
    {
        if (!IsModuleUsed)
            return;
        GameObject currentGameObject;
        switch (newScene.name)
        {
            // Fireball Skip Room Greenpath
            case "Fungus1_02":
                currentGameObject = GameObject.Instantiate(ShadeModule.PlatformPrefab, new(8.03f, 57.71f, 0), Quaternion.identity);
                currentGameObject.SetActive(true);
                break;
            // Second Fireball Skip Room Greenpath
            case "Fungus1_03":
                currentGameObject = GameObject.Instantiate(ShadeModule.PlatformPrefab, new(49.34f, 19.48f, 0), Quaternion.identity);
                currentGameObject.SetActive(true);
                break;
            // Explosion Pogo
            case "Fungus2_11":
                currentGameObject = GameObject.Instantiate(ShadeModule.PlatformPrefab, new(4.26f, 8.07f, 0), Quaternion.identity);
                currentGameObject.SetActive(true);
                break;
            // Mantis Village
            case "Fungus2_14":
                currentGameObject = GameObject.Instantiate(ShadeModule.PlatformPrefab, new(15.88f, 24.04f, 0), Quaternion.identity);
                currentGameObject.SetActive(true);

                currentGameObject = GameObject.Instantiate(ShadeModule.PlatformPrefab, new(10.96f, 29.37f, 0), Quaternion.identity);
                currentGameObject.SetActive(true);
                break;
            // False Knight
            case "Crossroads_10":
                currentGameObject = GameObject.Instantiate(ShadeModule.PlatformPrefab, new(16.14f, 16.12f, 0), Quaternion.identity);
                currentGameObject.SetActive(true);

                currentGameObject = GameObject.Instantiate(ShadeModule.PlatformPrefab, new(5.0873f, 23.1602f, 0), Quaternion.identity);
                currentGameObject.SetActive(true);
                break;
            case "Fungus2_01":
                currentGameObject = GameObject.Instantiate(ShadeModule.PlatformPrefab, new(36.44f, 35.21f, 0), Quaternion.identity);
                currentGameObject.SetActive(true);
                break;
            case "Ruins2_01_b":
                currentGameObject = GameObject.Instantiate(ShadeModule.PlatformPrefab, new(30.56f, 31.59f, 0), Quaternion.identity);
                currentGameObject.SetActive(true);
                break;
            case "Deepnest_East_11":
                currentGameObject = GameObject.Instantiate(ShadeModule.PlatformPrefab, new(104.34f, 97.36f, 0), Quaternion.identity);
                currentGameObject.SetActive(true);

                currentGameObject = GameObject.Instantiate(ShadeModule.PlatformPrefab, new(102.04f, 106.51f, 0), Quaternion.identity);
                currentGameObject.SetActive(true);

                currentGameObject = GameObject.Instantiate(ShadeModule.PlatformPrefab, new(97.36f, 113.58f, 0), Quaternion.identity);
                currentGameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    #endregion

    #region Methods

    internal override void Enable()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    internal override void Disable()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    #endregion
}
