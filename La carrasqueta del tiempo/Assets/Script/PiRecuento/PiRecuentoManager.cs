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

    void Start()
    {
        // Inicializar lista de niños disponibles
        availableChildren = new List<GameObject>(childrenSprites);

        // Ocultamos mensaje al iniciar
        mensajeText.gameObject.SetActive(false);

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

            // Posicionar el niño en el spot
            child.transform.position = spot.transform.position;
            child.transform.SetParent(spot.transform);

            // Inicialmente detrás del spot
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sortingOrder = -1;
        }

        // Mensaje en consola con los spots donde están los niños
        List<string> spotsWithChildrenNames = new List<string>();
        foreach (var s in spots)
        {
            if (s.hasChild)
                spotsWithChildrenNames.Add(s.name);
        }
        Debug.Log("Niños escondidos en: " + string.Join(", ", spotsWithChildrenNames));

        rondaText.text = $"Ronda {currentRound} / {rounds}";
    }

    public void OnSpotClicked(HidingSpot spot)
    {
        if (!spot.hasChild)
        {
            StartCoroutine(MostrarMensaje("Aquí no hay nadie..."));
        }
        else
        {
            spot.RevealChild();
            StartCoroutine(MostrarMensaje("¡Encontraste a un niño!"));

            // Quitar al niño encontrado de la lista disponible
            if (availableChildren.Contains(spot.childSprite))
                availableChildren.Remove(spot.childSprite);
        }

        currentRound++;
        StartCoroutine(NextRoundDelay());
    }

    private IEnumerator NextRoundDelay()
    {
        yield return new WaitForSeconds(0.5f);
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


