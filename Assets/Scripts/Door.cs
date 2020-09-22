using TMPro;
using UnityEngine;

public class Door : MonoBehaviour
{
    public AudioClip open, close;
    public AudioSource source;
    
    public Animator animator;
    public TextMeshProUGUI[] info;

    public bool canInteract;
    public bool isOpen;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        canInteract = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        canInteract = false;
    }


    private void Update()
    {
        foreach (TextMeshProUGUI text in info)
        {
            text.gameObject.SetActive(canInteract);
            text.text = isOpen ? "\"E\" kapat" : "\"E\" aç";
        }
        
        if (!Input.GetKeyDown(KeyCode.E)) return;
        if (!canInteract) return;

        if (!isOpen)
        {
            animator.SetTrigger("Open");
            source.clip = open;
            source.Play();

        }
        else if (isOpen)
        {
            animator.SetTrigger("Close");
            source.clip = close;
            source.Play();
        }

        isOpen = !isOpen;
        
    }

}
