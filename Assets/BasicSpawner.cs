using System; // ArraySegment<T>
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;  // NetAddress, NetConnectFailedReason, ecc.
using UnityEngine;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks {

  // === Singleton richiesto dagli altri script ===
  public static BasicSpawner Instance { get; private set; }

  [SerializeField] private bool enableDebug = true;
  [SerializeField] private NetworkObject playerPrefab;
  [SerializeField, Range(1, 32)] private int maxCubes = 8;

  private NetworkRunner _runner;
  private readonly Dictionary<PlayerRef, NetworkObject> _spawned = new();
  private readonly Dictionary<PlayerRef, bool[]> _held = new();
  private uint _lastMask = 0u;

  private void Awake() {
    if (Instance != null && Instance != this) {
      Destroy(gameObject);
      return;
    }
    Instance = this;
    // opzionale se cambi scena:
    // DontDestroyOnLoad(gameObject);
  }

  private void EnsureHeldFor(PlayerRef player) {
    int len = Mathf.Clamp(maxCubes, 1, 32);
    if (!_held.TryGetValue(player, out var arr) || arr == null || arr.Length != len) {
      _held[player] = new bool[len];
      if (enableDebug) Debug.Log($"[SPAWNER] EnsureHeldFor -> creato array input per {player} (len={len})");
    }
  }

  async void Start() {
    _runner = gameObject.AddComponent<NetworkRunner>();
    _runner.ProvideInput = true;
    _runner.AddCallbacks(this);

    if (enableDebug) Debug.Log("[SPAWNER] Avvio Runner Host sulla scena corrente...");
    await _runner.StartGame(new StartGameArgs {
      GameMode     = GameMode.Host,
      SessionName  = "TestSession",
      SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
    });
  }

  // ====================== INetworkRunnerCallbacks ======================

  public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
    EnsureHeldFor(player);
    if (enableDebug) Debug.Log($"[SPAWNER] Player JOINED: {player} | IsServer={runner.IsServer}");

    if (runner.IsServer) {
      // >>> Usa la versione qualificata di Random <<<
      Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(-2f, 2f), 1f, UnityEngine.Random.Range(-2f, 2f));
      var obj = runner.Spawn(playerPrefab, spawnPos, Quaternion.identity, player);
      _spawned[player] = obj;
      if (enableDebug) Debug.Log($"[SPAWNER] Avatar spawnato per {player} @ {spawnPos}");
    }
  }

  public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
    if (enableDebug) Debug.Log($"[SPAWNER] Player LEFT: {player}");
    if (_spawned.TryGetValue(player, out var obj)) {
      runner.Despawn(obj);
      _spawned.Remove(player);
    }
    _held.Remove(player);
  }

  public void OnInput(NetworkRunner runner, NetworkInput input) {
    var local = runner.LocalPlayer;
    EnsureHeldFor(local);

    var arr = _held[local];
    uint mask = 0u;
    int len = Mathf.Min(arr.Length, 32);
    for (int i = 0; i < len; i++) if (arr[i]) mask |= (1u << i);

    if (enableDebug && mask != _lastMask) {
      Debug.Log($"[SPAWNER] Input mask cambiata -> 0x{mask:X8}");
      _lastMask = mask;
    }
    input.Set(new NetworkInputData { cubeHoldMask = mask });
  }

  public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {
    input.Set(new NetworkInputData { cubeHoldMask = 0u });
    if (enableDebug) Debug.Log($"[SPAWNER] OnInputMissing per {player} -> mask 0");
  }

  // Metodi usati dalla UI (Toggle/Menu)
  public void HoldDown(int index) {
    if (_runner == null) { if (enableDebug) Debug.LogWarning("[SPAWNER] HoldDown: runner NULL"); return; }
    var local = _runner.LocalPlayer;
    EnsureHeldFor(local);
    var arr = _held[local];
    if (index < 0 || index >= arr.Length) { if (enableDebug) Debug.LogWarning($"[SPAWNER] HoldDown index fuori range: {index}"); return; }
    arr[index] = true;
    if (enableDebug) Debug.Log($"[SPAWNER] HoldDown idx={index} (Local={local})");
  }

  public void HoldUp(int index) {
    if (_runner == null) { if (enableDebug) Debug.LogWarning("[SPAWNER] HoldUp: runner NULL"); return; }
    var local = _runner.LocalPlayer;
    EnsureHeldFor(local);
    var arr = _held[local];
    if (index < 0 || index >= arr.Length) { if (enableDebug) Debug.LogWarning($"[SPAWNER] HoldUp index fuori range: {index}"); return; }
    arr[index] = false;
    if (enableDebug) Debug.Log($"[SPAWNER] HoldUp   idx={index} (Local={local})");
  }

  public uint GetLocalMask() {
    if (_runner == null) return 0u;
    var local = _runner.LocalPlayer;
    EnsureHeldFor(local);
    var arr = _held[local];
    uint mask = 0u;
    int len = Mathf.Min(arr.Length, 32);
    for (int i = 0; i < len; i++) if (arr[i]) mask |= (1u << i);
    return mask;
  }

  // ----- callback stub richieste dall'interfaccia -----
  public void OnConnectedToServer(NetworkRunner runner) { }
  public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
  public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
  public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
  public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
  public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
  public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
  public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
  public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
  public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
  public void OnSceneLoadDone(NetworkRunner runner) { }
  public void OnSceneLoadStart(NetworkRunner runner) { }
  public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
  public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
  public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}
