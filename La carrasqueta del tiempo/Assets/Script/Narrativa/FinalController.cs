
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FinalController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text finalText;

    [Header("Scene names")]
    public string menuSceneName = "Escena Menú";

    [Header("Final text")]
    [TextArea(8, 20)]
    public string textToShow =
        @"La festivitat de l’Onso es una festa local del poble de La Mata. Aquesta tradició es va recuperar després de quasi 50 anys la qual aveïna el final de l’hivern i l’arribada de la primavera i del bon temps. En ella, la figura de l’Onso va per tot el poble acompanyat de les figures més importants com el joglar, les diablesses i el caçador. Tot el poble ix al carrer amb menjar per compartir amb els veïns i veïnes i dsrutar tots junts d'aquesta noble tradició…";

    private void Start()
    {
        if (finalText != null)
            finalText.text = textToShow;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            GoToMenu();
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
