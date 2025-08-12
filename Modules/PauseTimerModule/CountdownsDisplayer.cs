using GlobalEnums;
using Hkmp.Api.Client;
using Modding.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheHuntIsOn.Modules.PauseTimerModule;
using TMPro;
using UnityEngine;

namespace TheHuntIsOn.Modules.PauseModule;

internal class CountdownsDisplayer
{
    private GameObject parent;
    private List<TextMeshPro> textMeshProCache = [];

    private IClientApi clientApi;
    private float respawnTimer;

    internal void SetClientApi(IClientApi clientApi) => this.clientApi = clientApi;

    internal void Enable()
    {
        parent = new("CountdownsDisplayer");
        Object.DontDestroyOnLoad(parent);
        parent.transform.SetParent(GameObject.Find("_GameCameras/HudCamera").transform);

        On.GameManager.Update += OnGMUpdate;
        On.GameManager.BeginSceneTransitionRoutine += OnBeginSceneTransitionRoutine;
        HeroController.instance.OnDeath += OnHeroDeath;
    }

    internal void Disable()
    {
        Object.Destroy(parent);
        parent = null;
        textMeshProCache.Clear();

        On.GameManager.Update -= OnGMUpdate;
        HeroController.instance.OnDeath -= OnHeroDeath;
    }

    private static string FormatTime(float timeSeconds)
    {
        if (timeSeconds <= 0) return "0.00";
        if (timeSeconds >= 3600)
        {
            int hours = Mathf.FloorToInt(timeSeconds / 3600);
            int minutes = Mathf.FloorToInt((timeSeconds % 3600) / 60);
            if (minutes >= 60) minutes = 59;

            string status = $"{hours} {(hours > 1 ? "hours" : "hour")}";
            if (minutes > 0) status = $"{status} and {minutes} {(minutes > 1 ? "minutes" : "minute")}";
            return status;
        }
        if (timeSeconds >= 60)
        {
            int minutes = Mathf.FloorToInt(timeSeconds / 60);
            int seconds = Mathf.FloorToInt(timeSeconds % 60);
            if (seconds >= 60) seconds = 59;
            return $"{minutes}:{seconds:00}";
        }
        if (timeSeconds >= 10) return $"{timeSeconds:00.0}";
        return $"{timeSeconds:0.00}";
    }

    private List<string> ComputeStatuses()
    {
        List<string> statuses = [];

        var saveData = TheHuntIsOn.LocalSaveData;
        if (saveData.IsServerPaused(out var unpauseSeconds))
        {
            if (!unpauseSeconds.HasValue) statuses.Append("Server Paused");
            else statuses.Append($"Unpausing in: {FormatTime(unpauseSeconds.Value)}");
        }
        if (respawnTimer > 0) statuses.Append($"Respawn in: {FormatTime(respawnTimer)}");

        foreach (var countdown in saveData.GlobalCountdowns)
        {
            if (countdown.GetDisplayTime(out float seconds)) statuses.Append($"{countdown.Message}: {FormatTime(seconds)}");
        }

        return statuses;
    }

    private static TMP_FontAsset font;
    private static TMP_FontAsset LoadFontAsset() => Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstOrDefault(font => font.name == "trajan_bold_tmpro");

    private void CreateTextMesh()
    {
        GameObject obj = new("Display");
        obj.transform.parent.SetParent(parent.transform);
        var mesh = obj.AddComponent<TextMeshPro>();
        mesh.font = (font ??= LoadFontAsset());
        mesh.color = Color.white;

        obj.layer = (int)PhysLayers.UI;
        var renderer = obj.GetOrAddComponent<MeshRenderer>();
        renderer.sortingLayerName = "HUD";
        renderer.sortingOrder = 11;

        textMeshProCache.Add(mesh);
    }

    private void UpdateStatuses(List<string> statuses)
    {
        var saveData = TheHuntIsOn.GlobalSaveData;
        var spacingParameters = saveData.PauseTimerPosition.SpacingParameters();

        var anchorPos = saveData.PauseTimerPosition;

        for (int i = 0; i < textMeshProCache.Count; i++)
        {
            var mesh = textMeshProCache[i];
            var status = i < statuses.Count ? statuses[i] : "";
            if (status == "")
            {
                mesh.gameObject.SetActive(false);
                mesh.text = "";
                mesh.transform.localPosition = new(-100, -100);
                continue;
            }

            mesh.text = status;
            mesh.fontSize = saveData.PauseTimerSize.FontSize();
            mesh.anchor = spacingParameters.anchor;
            mesh.transform.localPosition = spacingParameters.GetPosition(i, statuses.Count, saveData.PauseTimerSize.Spacing());
        }
    }

    private void OnGMUpdate(On.GameManager.orig_Update orig, GameManager self)
    {
        orig(self);

        if (respawnTimer > 0 && !TheHuntIsOn.LocalSaveData.IsServerPaused(out _))
        {
            respawnTimer -= Time.unscaledDeltaTime;
            if (respawnTimer < 0) respawnTimer = 0;
        }

        var statuses = ComputeStatuses();
        while (textMeshProCache.Count < statuses.Count) CreateTextMesh();
        UpdateStatuses(statuses);
    }

    private IEnumerator OnBeginSceneTransitionRoutine(On.GameManager.orig_BeginSceneTransitionRoutine orig, GameManager self, GameManager.SceneLoadInfo sceneLoadInfo)
    {
        var src = orig(self, sceneLoadInfo);
        if (respawnTimer > 0)
        {
            IEnumerator Modified()
            {
                yield return new WaitUntil(() => respawnTimer <= 0);
                while (src.MoveNext()) yield return src.Current;
            }
            return Modified();
        }
        else return src;
    }

    private void OnHeroDeath() => respawnTimer = Mathf.Max(respawnTimer, TheHuntIsOn.LocalSaveData.DeathTimerSeconds);
}
