// Assets/FusionSanityProbeLite.cs
using UnityEngine;
using Fusion;

public class FusionSanityProbeLite : MonoBehaviour {
  [SerializeField] bool enableDebug = true;
  NetworkRunner _runner;
  string _last;

  void Update() {
#if UNITY_2023_1_OR_NEWER
    if (_runner == null) _runner = Object.FindAnyObjectByType<NetworkRunner>();
    if (_runner == null) _runner = Object.FindFirstObjectByType<NetworkRunner>(FindObjectsInactive.Include);
#else
    if (_runner == null) _runner = Object.FindObjectOfType<NetworkRunner>();
#endif
    string msg;
    if (_runner == null) {
      msg = "[PROBE] Runner=NULL";
    } else {
      var lp = _runner.LocalPlayer;
      msg = $"[PROBE] Runner=OK  ProvideInput={_runner.ProvideInput}  IsServer={_runner.IsServer}  LocalPlayer={(lp == PlayerRef.None ? "None" : lp.ToString())}";
    }
    if (enableDebug && msg != _last) {
      Debug.Log(msg);
      _last = msg;
    }
  }
}
