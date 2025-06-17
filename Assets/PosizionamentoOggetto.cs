using UnityEngine;
using UnityEngine.UI;

public class ToggleObjectActivator : MonoBehaviour
{
    public GameObject targetObject; // L’oggetto da attivare/disattivare
    public Transform menuCanvas;    // Il menù, per posizionare vicino

    private Toggle toggle;

    private void Start()
    {
        toggle = GetComponent<Toggle>(); // Prende il Toggle dallo stesso oggetto
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        targetObject.SetActive(isOn);

        if (isOn && menuCanvas != null)
        {
            // Posiziona l’oggetto vicino al menù (es. 0.5m a destra)
            targetObject.transform.position = menuCanvas.position + menuCanvas.right * 0.5f;
            targetObject.transform.rotation = menuCanvas.rotation;
        }
    }
}
