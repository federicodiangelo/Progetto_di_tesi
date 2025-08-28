using System;                 // per Array.Empty<T>()
using UnityEngine;            // per MonoBehaviour, GameObject, Debug
using Fusion;                 // per NetworkBehaviour, Networked, GetInput, ecc.

public class PlayerCubeController : NetworkBehaviour {
  [Header("Refs")]
  [SerializeField] private GameObject[] cubes = Array.Empty<GameObject>();  // assegna i 4 cubi nell’inspector

  [Header("Debug")]
  [SerializeField] private bool enableDebug = true;                         // attiva/disattiva log utili

  // Stato di rete: ogni bit accende/spegne un cubo (bit 0 = cubo 0, bit 1 = cubo 1, ecc.)
  [Networked] public uint CubeMask { get; set; }

  // Cache locale dell’ultimo valore applicato ai GameObject, per evitare log/SetActive inutili.
  private uint _lastAppliedMask;

  // Applica la mask ai cubi (accende il cubo i se il bit i è 1)
  private void ApplyMaskToCubes(uint mask) {
    for (int i = 0; i < cubes.Length; i++) {
      var go = cubes[i];
      if (go == null) continue;
      bool on = (mask & (1u << i)) != 0u;
      if (go.activeSelf != on) {
        go.SetActive(on);
      }
    }
  }

  // Chiamato quando l’oggetto di rete è pronto
  public override void Spawned() {
    // Applica stato iniziale (di solito 0)
    _lastAppliedMask = CubeMask;
    ApplyMaskToCubes(_lastAppliedMask);
    if (enableDebug) Debug.Log($"[PLAYER] Spawned -> mask iniziale 0x{_lastAppliedMask:X8}");
  }

  // Logica di simulazione per-tick
  public override void FixedUpdateNetwork() {
    // 1) Lato autorità di stato (server/host o owner secondo il tuo setup): riceve input e aggiorna CubeMask
    if (HasStateAuthority) {
      if (GetInput(out NetworkInputData data)) {
        if (data.cubeHoldMask != CubeMask) {
          CubeMask = data.cubeHoldMask;   // questo verrà replicato ai client
          if (enableDebug) Debug.Log($"[PLAYER] (Authority) CubeMask <- 0x{CubeMask:X8}");
        }
      }
    }

    // 2) Lato rendering/istanza locale: se la mask replicata è cambiata rispetto all’ultima applicata, aggiorna i cubi
    if (CubeMask != _lastAppliedMask) {
      _lastAppliedMask = CubeMask;
      ApplyMaskToCubes(_lastAppliedMask);
      if (enableDebug) Debug.Log($"[PLAYER] ApplyMaskToCubes -> 0x{_lastAppliedMask:X8}");
    }
  }
}
