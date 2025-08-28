using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleMenuExclusive : MonoBehaviour
{
    [Header("Toggle del menu (uno per voce)")]
    public Toggle[] toggles;

    [Header("Output (opzionale)")]
    public BasicInputProvider provider;     // per inoltrare l'indice selezionato
    public int initialIndex = 0;

    private bool _initializing = false;

    private void OnEnable()
    {
        NormalizeInitialState();
    }

    private void OnDisable()
    {
        UnsubscribeAll();
    }

    public void NormalizeInitialState()
    {
        _initializing = true;

        if (toggles == null || toggles.Length == 0)
        {
            Debug.LogWarning("[MENU] Nessun toggle assegnato.");
            _initializing = false;
            return;
        }

        // Clamp dell'indice iniziale
        int safeIndex = Mathf.Clamp(initialIndex, 0, toggles.Length - 1);

        // Sottoscrizioni e setup
        UnsubscribeAll(); // per evitare doppie sottoscrizioni
        for (int i = 0; i < toggles.Length; i++)
        {
            int idx = i; // cattura
            toggles[i].onValueChanged.AddListener((isOn) => OnChildToggle(idx, isOn));
            toggles[i].isOn = (i == safeIndex);
        }

        Debug.Log($"[MENU] Iniziale -> {safeIndex}");

        // Inoltra al provider la selezione iniziale
        if (provider != null)
            provider.SetSelection(safeIndex);

        _initializing = false;
    }

    private void UnsubscribeAll()
    {
        if (toggles == null) return;
        foreach (var t in toggles)
        {
            if (t != null) t.onValueChanged.RemoveAllListeners();
        }
    }

    public void OnChildToggle(int index, bool isOn)
    {
        if (_initializing) return;

        if (isOn)
        {
            // Spegne tutti gli altri
            for (int i = 0; i < toggles.Length; i++)
            {
                if (i != index && toggles[i] != null && toggles[i].isOn)
                    toggles[i].isOn = false;
            }

            Debug.Log($"[MENU] Selezionato -> {index}");
            if (provider != null)
                provider.SetSelection(index);
        }
        else
        {
            // Evita che restino tutti OFF: se l’utente spegne quello attivo,
            // riaccendilo immediatamente.
            bool anyOn = false;
            for (int i = 0; i < toggles.Length; i++)
            {
                if (toggles[i] != null && toggles[i].isOn) { anyOn = true; break; }
            }
            if (!anyOn && toggles != null && index >= 0 && index < toggles.Length && toggles[index] != null)
            {
                toggles[index].isOn = true;
                Debug.Log("[MENU] Impedito Tutti OFF");
            }
        }
    }
}
