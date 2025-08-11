using Hkmp.Api.Client;
using Hkmp.Api.Server;

namespace TheHuntIsOn.Modules.PauseModule;

internal class PauseModule : Module
{
    #region Properties

    public override string MenuDescription => "Enable server-wide pauses, timed unpauses, and respawn timers.";

    private PauseClientAddon PauseClientAddon;
    private PauseServerAddon PauseServerAddon;

    private bool AreAddonsLoaded;

    private readonly PauseController pauseController = new();

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

        PauseClientAddon = new(this);
        PauseServerAddon = new();
        ClientAddon.RegisterAddon(PauseClientAddon);
        ServerAddon.RegisterAddon(PauseServerAddon);

        AreAddonsLoaded = true;
    }

    #endregion

    #region Methods

    internal override void Enable()
    {
        if (!AreAddonsLoaded) return;

        pauseController.Enable();

        PauseClientAddon.NetManager.ServerPauseEvent += pauseController.OnServerPause;
        PauseClientAddon.NetManager.ServerUnpauseEvent += pauseController.OnServerUnpause;
        // PauseClientAddon.NetManager.CountdownEvent += NetManager_OnCountdownEvent;
        PauseClientAddon.NetManager.SetDeathTimerEvent += OnSetDeathTimer;
    }

    internal override void Disable()
    {
        if (!AreAddonsLoaded) return;

        pauseController.Disable();

        PauseClientAddon.NetManager.ServerPauseEvent -= pauseController.OnServerPause;
        PauseClientAddon.NetManager.ServerUnpauseEvent -= pauseController.OnServerUnpause;
        // PauseClientAddon.NetManager.CountdownEvent -= NetManager_OnCountdownEvent;
        PauseClientAddon.NetManager.SetDeathTimerEvent -= OnSetDeathTimer;
    }

    internal void SetClientApi(IClientApi clientApi) => pauseController.SetClientApi(clientApi);

    private void OnSetDeathTimer(SetDeathTimerPacket packet) => TheHuntIsOn.LocalSaveData.DeathTimer = packet.DeathTimer;

    #endregion
}