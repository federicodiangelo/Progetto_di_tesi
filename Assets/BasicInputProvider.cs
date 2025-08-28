using UnityEngine;

public class BasicInputProvider : MonoBehaviour
{
    [Header("Riferimenti")]
    public BuildBlocksBasicSpawner spawner;

    [Header("Configurazione")]
    public int currentIndex = 0; // indice selezionato corrente

    public void SetSelection(int index)
    {
        Debug.Log($"[PROVIDER] Selezione -> idx={index}");
        currentIndex = index;

        if (spawner == null)
        {
            Debug.LogWarning("[PROVIDER] Nessuno spawner assegnato.");
            return;
        }

        // Non chiamiamo lo spawner se l'indice è negativo;
        // lo spawner comunque ricontrolla tutto per sicurezza.
        if (index < 0)
        {
            Debug.LogWarning("[PROVIDER] Indice negativo, ignoro.");
            return;
        }

        spawner.SetSelection(index);
    }
}
