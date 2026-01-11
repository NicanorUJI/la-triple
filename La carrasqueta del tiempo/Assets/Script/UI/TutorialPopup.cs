using UnityEngine;

public class TutorialPopup : MonoBehaviour
{
    public Animator anim;
    public string[] requiredFlags;      // Flags que deben estar activos
    public string[] forbiddenFlags;     // Flags que NO deben estar activos

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Animator anim = GetComponentInParent<Animator>();
        if (!other.CompareTag("Player"))
            return;


        if (!CheckFlags())
            return;

        anim.SetBool("inTrigger", true);

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
                return;

            if (!CheckFlags())
                return;

            anim.SetBool("inTrigger", false);

    }

    private bool CheckFlags()
    {
        // Requisitos
        if (requiredFlags != null)
        {
            foreach (var flag in requiredFlags)
            {
                if (!string.IsNullOrEmpty(flag) && !GameManager.Check(flag))
                    return false;
            }
        }

        // Prohibidos
        if (forbiddenFlags != null)
        {
            foreach (var flag in forbiddenFlags)
            {
                if (!string.IsNullOrEmpty(flag) && GameManager.Check(flag))
                    return false;
            }
        }

        return true;
    }
}
