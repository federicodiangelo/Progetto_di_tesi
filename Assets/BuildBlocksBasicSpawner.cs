using UnityEngine;

public class BuildBlocksBasicSpawner : MonoBehaviour
{
    [Header("Prefabs da spawnare (trascina qui)")]
    public GameObject[] prefabs;

    [Header("Parent e punto di spawn (opzionali)")]
    public Transform spawnParent;    // se nullo: nessun parent
    public Transform spawnPoint;     // se nullo: usa camera o lo spawner

    [Header("Spawn davanti alla camera (se spawnPoint è nullo)")]
    public bool spawnInFrontOfCamera = true;
    [Range(0.2f, 5f)] public float spawnDistance = 1.5f;
    public Vector3 spawnOffsetLocal = Vector3.zero;

    [Header("Comportamento")]
    public bool replacePrevious = true;

    [Header("Layer & Scala")]
    public bool forceLayer = true;
    public int layerToSet = 0; // Default
    public bool forceScale = true;
    public Vector3 forcedScale = new Vector3(0.2f, 0.2f, 0.2f);

    [Header("Modalità fisica")]
    [Tooltip("Se true: niente fisica. Rimane kinematic, non cade mai, ma è afferrabile.")]
    public bool noPhysicsGrabbable = true;

    private GameObject _lastSpawned;

    public void SetSelection(int index)
    {
        if (prefabs == null || prefabs.Length == 0) { Debug.LogWarning("[SPAWNER] Nessun prefab assegnato."); return; }
        if (index < 0 || index >= prefabs.Length)  { Debug.LogWarning($"[SPAWNER] Indice {index} fuori range (0..{prefabs.Length-1})."); return; }
        var prefab = prefabs[index];
        if (prefab == null) { Debug.LogWarning($"[SPAWNER] Prefab all'indice {index} è NULL."); return; }

        if (replacePrevious && _lastSpawned) { Destroy(_lastSpawned); _lastSpawned = null; }

        // posizione/rotazione/parent
        Vector3 pos; Quaternion rot; Transform parent = spawnParent ? spawnParent : null;
        if (spawnPoint)
        {
            pos = spawnPoint.position; rot = spawnPoint.rotation;
        }
        else if (spawnInFrontOfCamera && TryGetBestCamera(out var cam))
        {
            var t = cam.transform;
            pos = t.position + t.TransformDirection(spawnOffsetLocal + new Vector3(0,0,spawnDistance));
            rot = Quaternion.LookRotation(t.forward, Vector3.up);
        }
        else
        {
            pos = transform.position; rot = transform.rotation;
            if (!parent) parent = transform;
        }

        _lastSpawned = Instantiate(prefab, pos, rot, parent);
        if (!_lastSpawned.activeSelf) _lastSpawned.SetActive(true);

        if (forceLayer) SetLayerRecursively(_lastSpawned, layerToSet);
        if (forceScale) _lastSpawned.transform.localScale = forcedScale;

        ConfigureAsPhysicslessGrabbable(_lastSpawned);

        var p = _lastSpawned.transform.position;
        var ry = _lastSpawned.transform.eulerAngles.y;
        var sc = _lastSpawned.transform.localScale;
        Debug.Log($"[SPAWNER] Spawn -> [{prefab.name}] (idx={index}) pos=({p.x:F3},{p.y:F3},{p.z:F3}) rotY={ry:F1} parent={(parent?parent.name:"null")} layer={(forceLayer?layerToSet.ToString():"keep")} scale=({sc.x:F2},{sc.y:F2},{sc.z:F2})");
    }

    // ------------------------------------------------------------------------

    private void ConfigureAsPhysicslessGrabbable(GameObject go)
    {
        // Assicura collisore per l'interazione
        if (!go.GetComponent<Collider>()) go.AddComponent<BoxCollider>();

        // Assicura un Rigidbody (Oculus/Meta Grabbable lo gradisce). Lo rendiamo "senza fisica".
        var rb = go.GetComponent<Rigidbody>();
        if (!rb) rb = go.AddComponent<Rigidbody>();

        if (noPhysicsGrabbable)
        {
            rb.useGravity = false;
            rb.isKinematic = true;                 // non cade, non viene spinto
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rb.interpolation = RigidbodyInterpolation.None;
        }
        else
        {
            // Alternativa: fisica normale (se mai ti servisse)
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.mass = 0.8f;
            rb.linearDamping = 0.2f;
            rb.angularDamping = 0.05f;
        }

        // Se il prefab ha già i componenti “Network Grabbable” dei Meta Blocks, li lasciamo stare.
        // (Qui non tocchiamo TransferOwnership/Multiplayer)
    }

    private static void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform c in go.transform) SetLayerRecursively(c.gameObject, layer);
    }

    private bool TryGetBestCamera(out Camera cam)
    {
        cam = Camera.main;
        if (cam && cam.enabled) return true;
        foreach (var c in FindObjectsOfType<Camera>()) if (c && c.enabled) { cam = c; return true; }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 gizmoPos;
        if (spawnPoint) gizmoPos = spawnPoint.position;
        else if (spawnInFrontOfCamera && TryGetBestCamera(out var cam))
        {
            var t = cam.transform;
            gizmoPos = t.position + t.TransformDirection(spawnOffsetLocal + new Vector3(0,0,spawnDistance));
        }
        else gizmoPos = transform.position;

        Gizmos.DrawWireSphere(gizmoPos, 0.06f);
    }
}
