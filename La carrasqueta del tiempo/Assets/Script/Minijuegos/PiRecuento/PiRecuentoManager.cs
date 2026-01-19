using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PiRecuentoManager : MonoBehaviour
{
    [Header("Niños y Spots")]
    public List<GameObject> childrenSprites; 
    public List<HidingSpot> spots;           

    [Header("Rondas")]
    public int rounds = 5;

    [Header("UI")]
    public TextMeshProUGUI rondaText;
    public TextMeshProUGUI mensajeText;

    [Header("Paneles")]
    public GameObject panelReglas;
    public GameObject panelFinPartida; 
    public Button botonContinuar;  

    [Header("Fin de partida - Imagen")]
    public Image imagenResultado;
    public TextMeshProUGUI textoResultado;
    public Sprite spriteVictoria;
    public Sprite spriteDerrota;

    [Header("Audio SFX")]
    public AudioSource sfxSource;          
    public AudioClip sonidoNinoEncontrado; 
    public AudioClip sonidoFallo;          
    public AudioClip sonidoVictoria;
    public AudioClip sonidoDerrota;

    [Header("Audio Música")]
    public AudioSource musicSource;        
    public AudioClip backgroundMusic;      

    private int currentRound = 1;
    private List<GameObject> availableChildren; 

    public bool inputEnabled = false;
    private bool haGanado = false;

    void Start()
    {
        availableChildren = new List<GameObject>(childrenSprites);

        mensajeText.gameObject.SetActive(false);
        rondaText.gameObject.SetActive(false);

        if (panelFinPartida != null)
            panelFinPartida.SetActive(false);

        // --- CAMBIO: AQUÍ YA NO INICIAMOS LA MÚSICA ---
        // La música esperará a que el jugador pulse el botón.
    }

    public void Aceptar() 
    {
        // --- CAMBIO: INICIAMOS LA MÚSICA AQUÍ ---
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;  
            musicSource.Play();
        }
        // ----------------------------------------

        inputEnabled = true;
        if (panelReglas != null)
            panelReglas.SetActive(false);
        IniciarPartida();
    }

    public void IniciarPartida()
    {
        currentRound = 1;
        availableChildren = new List<GameObject>(childrenSprites);
        rondaText.gameObject.SetActive(true);
        StartRound();
    }

    void StartRound()
    {
        // ... (El código de StartRound es idéntico al anterior) ...
        foreach (var s in spots)
        {
            s.ResetSpot();
            s.childSprite = null;
        }

        if (availableChildren.Count == 0 || currentRound > rounds)
        {
            haGanado = availableChildren.Count == 0;
            MostrarPanelFin();
            return;
        }

        List<HidingSpot> shuffledSpots = new List<HidingSpot>(spots);
        for (int i = 0; i < shuffledSpots.Count; i++)
        {
            int rand = Random.Range(i, shuffledSpots.Count);
            var temp = shuffledSpots[i];
            shuffledSpots[i] = shuffledSpots[rand];
            shuffledSpots[rand] = temp;
        }

        List<GameObject> shuffledChildren = new List<GameObject>(availableChildren);
        for (int i = 0; i < shuffledChildren.Count; i++)
        {
            int rand = Random.Range(i, shuffledChildren.Count);
            var temp = shuffledChildren[i];
            shuffledChildren[i] = shuffledChildren[rand];
            shuffledChildren[rand] = temp;
        }

        for (int i = 0; i < shuffledChildren.Count; i++)
        {
            HidingSpot spot = shuffledSpots[i];
            GameObject child = shuffledChildren[i];

            spot.hasChild = true;
            spot.childSprite = child;

            Niño n = child.GetComponent<Niño>();
            if (n != null) n.Ocultar(); 

            child.transform.SetParent(spot.transform);
            child.transform.localPosition = Vector3.zero;
        }

        rondaText.text = $"Ronda: {currentRound}/{rounds}";
    }

    public void OnSpotClicked(HidingSpot spot)
    {
        // ... (Idéntico al anterior) ...
        if (!spot.hasChild)
        {
            if (sfxSource != null && sonidoFallo != null)
                sfxSource.PlayOneShot(sonidoFallo);

            StartCoroutine(MostrarMensaje());
        }
        else
        {
            if (sfxSource != null && sonidoNinoEncontrado != null)
                sfxSource.PlayOneShot(sonidoNinoEncontrado);

            Niño n = spot.childSprite.GetComponent<Niño>();
            if (n != null) n.Mostrar(); 

            StartCoroutine(MostrarMensaje(n.nombre));
            availableChildren.Remove(spot.childSprite);
            StartCoroutine(RemoverNiñoDelay(spot.childSprite));
        }

        currentRound++;
        StartCoroutine(NextRoundDelay());
    }

    // PANEL FIN
    private void MostrarPanelFin()
    {
        inputEnabled = false;

        // PARAR MÚSICA AL TERMINAR
        if (musicSource != null)
        {
            musicSource.Stop();
        }

        if (panelFinPartida != null)
            panelFinPartida.SetActive(true);

        if (haGanado)
        {
            imagenResultado.sprite = spriteVictoria;
            textoResultado.text = "Has trobat a tots el xiquets!";
            
            if (sfxSource != null && sonidoVictoria != null)
                sfxSource.PlayOneShot(sonidoVictoria);
        }
        else
        {
            imagenResultado.sprite = spriteDerrota;
            textoResultado.text = "Els xiquets han guanyat... :(";

            if (sfxSource != null && sonidoDerrota != null)
                sfxSource.PlayOneShot(sonidoDerrota);
        }

        if (botonContinuar != null)
            botonContinuar.interactable = haGanado;
        rondaText.gameObject.SetActive(false);
    }

    // ... (El resto de funciones: ReintentarJuego, FinishGame, corrutinas... siguen igual) ...
    public void ReintentarJuego() 
    {
        panelFinPartida.SetActive(false);
        ResetGame();
    }
    public void ResetGame()
    {
        SceneManager.LoadScene("PiRecuentoMinijuego");
    }
    public void ContinuarJuego() 
    {
        if (!haGanado) return;
        panelFinPartida.SetActive(false);
        FinishGame();
    }
    public void FinishGame()
    {
        GameManager.Change("Act2_Q_ESQUELLES_HasBracelet");
        
        var mc = MissionController.Instance ?? FindObjectOfType<MissionController>();
        if (mc != null)
        {
            mc.SetActiveMission(
                4,
                "Portar la polsera a Raúl",
                "Has aconseguit una polsera al col·legi. Torna al passat i dóna-li-la a Raúl a les calderetes."
            );
        }
        StartCoroutine(ReturnToSchoolAfterDelay(0.5f));
    }

    private IEnumerator RemoverNiñoDelay(GameObject child)
    {
        float showTime = 3f;          
        float swayAngle = 15f;        
        float swaySpeed = 2f;         

        Niño n = child.GetComponent<Niño>();
        child.transform.localScale = n.originalScale * 2;

        float elapsedTime = 0f;
        while (elapsedTime <= showTime)
        {
            if (child != null)
            {
                float angle = Mathf.Sin(Time.time * swaySpeed) * swayAngle;
                child.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (child != null)
        {
            child.transform.rotation = Quaternion.identity;
            child.transform.position = new Vector3(9999, 9999, 0);
            child.transform.SetParent(null);
        }
    }

    private IEnumerator NextRoundDelay()
    {
        inputEnabled = false;
        yield return new WaitForSeconds(3f);
        inputEnabled = true;
        StartRound();
    }

    private IEnumerator MostrarMensaje()
    {
        mensajeText.gameObject.SetActive(true);
        mensajeText.text = "Ací no hi ha ningú...";
        yield return new WaitForSeconds(3f);
        mensajeText.gameObject.SetActive(false);
    }

    private IEnumerator MostrarMensaje(string nombre)
    {
        mensajeText.gameObject.SetActive(true);
        mensajeText.text = $"Has trobat a {nombre}!"; ;
        yield return new WaitForSeconds(3f);
        mensajeText.gameObject.SetActive(false);
    }

    private IEnumerator ReturnToSchoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.lastExitName = "EscuM"; 
        }
        SceneManager.LoadScene("colegio"); 
    }
}