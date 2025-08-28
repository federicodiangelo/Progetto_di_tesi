using System.Collections;
using UnityEngine;


[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class XRGrabRearmOnEnable : MonoBehaviour
{
    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    Collider[] allCols;
    Rigidbody rb;

    void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb   = GetComponent<Rigidbody>();
        allCols = GetComponentsInChildren<Collider>(true);
    }

    void OnEnable()
    {
        // 1) Disattiva subito per evitare stati "sporchi"
        grab.enabled = false;

        // 2) Disattiva i collider per un frame → elimina hover/selezioni appese
        foreach (var c in allCols) c.enabled = false;

        // 3) Reset minimo della fisica (se era preso al SetActive(false))
        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();
        }

        // 4) Riabilita tutto nel frame successivo
        StartCoroutine(RearmNextFrame());
    }

    IEnumerator RearmNextFrame()
    {
        yield return null; // 1 frame

        foreach (var c in allCols) c.enabled = true;

        yield return null; // 2° frame (più stabile con XRIT)

        grab.enabled = true;

        // XRIT 3.x: usa le interfacce IXR* (metodi vecchi sono Obsolete)
        var im = grab.interactionManager;
        if (im != null)
        {
            im.CancelInteractableSelection((UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)grab);
            im.CancelInteractableHover((UnityEngine.XR.Interaction.Toolkit.Interactables.IXRHoverInteractable)grab);
        }
    }
}
