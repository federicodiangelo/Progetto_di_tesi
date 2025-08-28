using UnityEngine;

public class FusionSanityProbe : MonoBehaviour {
  [SerializeField] private bool enable = true;

  void Update() {
    if (!enable) return;
    var sp = BasicSpawner.Instance;
    if (sp == null) return;

    // Se vuoi anche mostrare ProvideInput: prendi il runner via reflection semplice
    // (qui ci limitiamo alla mask)
    uint mask = sp.GetLocalMask();
    // Nota: se vuoi ridurre spam, logga ogni 1s come facevi (qui assumiamo throttling esterno)
    Debug.Log($"[PROBE] Runner=OK  ProvideInput=True  Mask=0x{mask:X8}");
  }
}
