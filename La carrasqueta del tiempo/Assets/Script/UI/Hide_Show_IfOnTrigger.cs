using UnityEngine;

public class Hide_Show_IfOnTrigger : MonoBehaviour
{
    public GameObject toHide;
    public bool hide = false;

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

    private void OnTriggerStay2D(Collider2D other)
    {
        //Animator anim = GetComponentInParent<Animator>();
        if (!other.CompareTag("Player"))
            return;


        if (!CheckFlags())
            return;

        if(hide)
            toHide.SetActive(false);
        else 
            toHide.SetActive(true);
        

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
