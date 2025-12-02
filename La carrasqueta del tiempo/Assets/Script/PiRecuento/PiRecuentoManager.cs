using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PiRecuentoManager : MonoBehaviour
{
    [Header("Niños y Spots")]
    public List<GameObject> childrenSprites; // 3 niños únicos
    public List<HidingSpot> spots;           // 5 spots

    [Header("Rondas")]
    public int rounds = 5;

    [Header("UI")]
    public TextMeshProUGUI rondaText;
    public TextMeshProUGUI mensajeText;

    private int currentRound = 1;
    private List<GameObject> availableChildren; // niños que aún no han sido encontrados

    public GameObject panelReglas;

    void Start()
    {
        // Inicializar lista de niños disponibles
        availableChildren = new List<GameObject>(childrenSprites);

        // Ocultamos mensaje al iniciar
        mensajeText.gameObject.SetActive(false);
    }

    public void IniciarPartida() 
    {
        currentRound = 1;
        if (panelReglas != null)
            panelReglas.SetActive(false);
        availableChildren = new List<GameObject>(childrenSprites);
        StartRound();
    }
    void StartRound()
    {
        Debug.Log($"Ronda {currentRound}");

        // Reiniciamos todos los spots
        foreach (var s in spots)
        {
            s.ResetSpot();
            s.childSprite = null;
        }

        // Fin del juego
        if (availableChildren.Count == 0 || currentRound > rounds)
        {
            Debug.Log("¡Juego terminado!");
            rondaText.text = "Juego terminado";
            return;
        }

        // Mezclar spots
        List<HidingSpot> shuffledSpots = new List<HidingSpot>(spots);
        for (int i = 0; i < shuffledSpots.Count; i++)
        {
            int rand = Random.Range(i, shuffledSpots.Count);
            var temp = shuffledSpots[i];
            shuffledSpots[i] = shuffledSpots[rand];
            shuffledSpots[rand] = temp;
        }

        // Mezclar niños disponibles
        List<GameObject> shuffledChildren = new List<GameObject>(availableChildren);
        for (int i = 0; i < shuffledChildren.Count; i++)
        {
            int rand = Random.Range(i, shuffledChildren.Count);
            var temp = shuffledChildren[i];
            shuffledChildren[i] = shuffledChildren[rand];
            shuffledChildren[rand] = temp;
        }

        // Asignar cada niño disponible a un spot diferente cada ronda
        for (int i = 0; i < shuffledChildren.Count; i++)
        {
            HidingSpot spot = shuffledSpots[i];
            GameObject child = shuffledChildren[i];

            spot.hasChild = true;
            spot.childSprite = child;

            Niño n = child.GetComponent<Niño>();
            
            if (n != null)
                n.Ocultar(); // restauramos escala y sortingOrder

            // Ahora sí ponemos al niño en el spot
            child.transform.SetParent(spot.transform);
            child.transform.localPosition = Vector3.zero;
        }

        // Mensaje en consola con los spots donde están los niños
        List<string> spotsWithChildrenNames = new List<string>();
        foreach (var s in spots)
        {
            if (s.hasChild)
                spotsWithChildrenNames.Add(s.name);
        }
        Debug.Log("Niños escondidos en: " + string.Join(", ", spotsWithChildrenNames));

        rondaText.text = $"Ronda: {currentRound}/{rounds}";
    }

    public void OnSpotClicked(HidingSpot spot)
    {
        if (!spot.hasChild)
        {
            StartCoroutine(MostrarMensaje("Aquí no hay nadie..."));
        }
        else
        {
            Niño n = spot.childSprite.GetComponent<Niño>();
            if (n != null)
            {
                n.Mostrar(); // mostrar en primer plano
            }

            StartCoroutine(MostrarMensaje("¡Encontraste a un niño!"));

            // Quitar niño de la lista de disponibles
            availableChildren.Remove(spot.childSprite);

            // Esperar un momento antes de moverlo fuera
            StartCoroutine(RemoverNiñoDelay(spot.childSprite));
        }

        currentRound++;
        StartCoroutine(NextRoundDelay());
    }


    private IEnumerator RemoverNiñoDelay(GameObject child)
    {
        yield return new WaitForSeconds(2f); // tiempo para que el niño se vea
        if (child != null)
        {
            child.transform.position = new Vector3(9999, 9999, 0);
            child.transform.SetParent(null);
        }
    }


    private IEnumerator NextRoundDelay()
    {
        yield return new WaitForSeconds(1.5f);
        StartRound();
    }

    private IEnumerator MostrarMensaje(string texto)
    {
        mensajeText.gameObject.SetActive(true);
        mensajeText.text = texto;
        yield return new WaitForSeconds(2f);
        mensajeText.gameObject.SetActive(false);
    }
}


