using UnityEngine;
using System.Collections;

public class AutoSaveManager : MonoBehaviour
{
    public static AutoSaveManager Instance { get; private set; }

    [SerializeField] private float autoSaveInterval = 60f;

    private void Awake()
    {
        // Asegurarnos de que solo haya uno
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // <- clave para que no se destruya al cambiar de escena
    }

    private void Start()
    {
        StartCoroutine(AutoSaveRoutine());
    }

    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveInterval);

            Debug.Log("[AutoSave] Guardado automático realizado.");
        }
    }
}
