using UnityEngine;

public class BuildBlocksBasicSpawner : MonoBehaviour
{
    [Header("Prefabs da spawnare (trascina qui)")]
    public GameObject[] prefabs;

    [Header("Parent e punto di spawn (opzionali)")]
    [Tooltip("Parent degli oggetti instanziati. Se nullo, nessun parent.")]
    public Transform spawnParent;
    [Tooltip("Se assegnato, usa posizione/rotazione di questo Transform per lo spawn.")]
    public Transform spawnPoint;

    [Header("Spawn davanti alla camera")]
    [Tooltip("Se true e spawnPoint è nullo, spawna davanti alla camera.")]
    public bool spawnInFrontOfCamera = true;
    [Tooltip("Distanza davanti alla camera quando spawnInFrontOfCamera è true.")]
    [Range(0.2f, 5f)] public float spawnDistance = 1.5f;
    [Tooltip("Offset locale da applicare quando si spawna davanti alla camera (x=destra, y=su, z=avanti).")]
    public Vector3 spawnOffsetLocal = Vector3.zero;

    [Header("Comportamento")]
    [Tooltip("Se true, distrugge l'ultimo oggetto spawnato prima di spawnarne uno nuovo.")]
    public bool replacePrevious = true;

    [Header("Layer & Scala (facoltativi)")]
    [Tooltip("Se true, forza il layer su tutto l'oggetto spawnato (ricorsivo).")]
    public bool forceLayer = true;
    [Tooltip("Layer da assegnare agli oggetti spawnati (Default=0).")]
    public int layerToSet = 0; // Default
    [Tooltip("Se true, forza la scala dell'oggetto radice spawnato.")]
    public bool forceScale = true;
    [Tooltip("Scala da applicare all'oggetto spawnato (solo se forceScale=true).")]
    public Vector3 forcedScale = Vector3.one;

    private GameObject _lastSpawned;

    /// <summary>
    /// Chiamata dal provider/menu con l'indice selezionato (0..N-1)
    /// </summary>
    public void SetSelection(int index)
    {
        // Validazioni
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogWarning("[SPAWNER] Nessun prefab assegnato.");
            return;
        }
        if (index < 0 || index >= prefabs.Length)
        {
            Debug.LogWarning($"[SPAWNER] Indice {index} fuori range (0..{prefabs.Length - 1}).");
            return;
        }
        var prefab = prefabs[index];
        if (prefab == null)
        {
            Debug.LogWarning($"[SPAWNER] Prefab all'indice {index} è NULL.");
            return;
        }

        // Rimpiazza l'ultimo se richiesto
        if (replacePrevious && _lastSpawned != null)
        {
            Destroy(_lastSpawned);
            _lastSpawned = null;
        }

        // Calcola posizione/rotazione/parent
        Vector3 pos;
        Quaternion rot;
        Transform parent = spawnParent != null ? spawnParent : null;

        if (spawnPoint != null)
        {
            pos = spawnPoint.position;
            rot = spawnPoint.rotation;
        }
        else if (spawnInFrontOfCamera && TryGetBestCamera(out var cam))
        {
            var t = cam.transform;
            // punto davanti alla camera + offset locale
            pos = t.position + t.TransformDirection(spawnOffsetLocal + new Vector3(0, 0, spawnDistance));
            rot = Quaternion.LookRotation(t.forward, Vector3.up);
        }
        else
        {
            // fallback: posizione di questo spawner
            pos = transform.position;
            rot = transform.rotation;
            if (parent == null) parent = transform; // per tenerlo in scena vicino allo spawner
        }

        // Instanzia
        _lastSpawned = Instantiate(prefab, pos, rot, parent);

        // Assicurati che sia attivo
        if (!_lastSpawned.activeSelf) _lastSpawned.SetActive(true);

        // Forza layer e scala se richiesto
        if (forceLayer) SetLayerRecursively(_lastSpawned, layerToSet);
        if (forceScale) _lastSpawned.transform.localScale = forcedScale;

        // Log dettagliato
        string camName = Camera.main != null ? Camera.main.name : "(no Camera.main)";
        Debug.Log($"[SPAWNER] Spawn -> [{prefab.name}] (idx={index}) " +
                  $"pos={_lastSpawned.transform.position:F3} rotY={_lastSpawned.transform.eulerAngles.y:F1} " +
                  $"parent={(parent ? parent.name : "null")} cam={camName} layer={(forceLayer ? layerToSet.ToString() : "keep")} " +
                  $"scale={(forceScale ? forcedScale.ToString() : "keep")}");
    }

    private static void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform c in go.transform) SetLayerRecursively(c.gameObject, layer);
    }

    private bool TryGetBestCamera(out Camera cam)
    {
        cam = Camera.main;
        if (cam != null && cam.enabled) return true;

        // fallback: prima camera abilitata in scena
        var cams = FindObjectsOfType<Camera>();
        foreach (var c in cams)
        {
            if (c != null && c.enabled)
            {
                cam = c;
                return true;
            }
        }
        return false;
    }

    // Gizmo per vedere dove spawnerà quando selezioni lo spawner in Editor
    private void OnDrawGizmosSelected()
    {
        Vector3 gizmoPos;
        if (spawnPoint != null)
        {
            gizmoPos = spawnPoint.position;
        }
        else if (spawnInFrontOfCamera && TryGetBestCamera(out var cam))
        {
            var t = cam.transform;
            gizmoPos = t.position + t.TransformDirection(spawnOffsetLocal + new Vector3(0, 0, spawnDistance));
        }
        else
        {
            gizmoPos = transform.position;
        }

        Gizmos.DrawWireSphere(gizmoPos, 0.06f);
    }
}
