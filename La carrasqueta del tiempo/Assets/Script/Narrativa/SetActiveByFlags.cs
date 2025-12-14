using UnityEngine;

public class SetActiveByFlags : MonoBehaviour
{
    [Header("Se activa SOLO si todas estas flags están en true")]
    public string[] requiredAll;

    [Header("Se desactiva si CUALQUIERA de estas flags está en true")]
    public string[] forbiddenAny;

    private void Start()
    {
        Apply();
    }

    public void Apply()
    {
        bool ok = true;

        if (requiredAll != null)
        {
            foreach (var f in requiredAll)
            {
                if (!string.IsNullOrEmpty(f) && !GameManager.Check(f))
                {
                    ok = false;
                    break;
                }
            }
        }

        if (ok && forbiddenAny != null)
        {
            foreach (var f in forbiddenAny)
            {
                if (!string.IsNullOrEmpty(f) && GameManager.Check(f))
                {
                    ok = false;
                    break;
                }
            }
        }

        gameObject.SetActive(ok);
    }
}
