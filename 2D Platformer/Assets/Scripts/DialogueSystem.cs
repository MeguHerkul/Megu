using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI Ayarlarý")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI textDisplay;

    [Header("Diyalog Ayarlarý")]
    [TextArea(3, 10)]
    public string[] sentences;
    public float typingSpeed = 0.05f;

    private int index;
    private bool isTalking = false;
    private bool hasTalked = false;
    private GameObject playerObj;

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (isTalking && (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)))
        {
            if (textDisplay.text == sentences[index])
            {
                NextSentence();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTalked)
        {
            playerObj = other.gameObject;
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        if (sentences.Length == 0) return;

        isTalking = true;
        hasTalked = true;
        index = 0;

        
        Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        Animator playerAnim = playerObj.GetComponent<Animator>();
        if (playerAnim != null) playerAnim.Play("Player_Idle");

        
        Animator merchantAnim = GetComponent<Animator>();
        if (merchantAnim != null)
        {
            merchantAnim.Play("NPC_Talk");
        }

        if (playerObj.GetComponent<Player>() != null)
        {
            playerObj.GetComponent<Player>().enabled = false;
        }

        dialoguePanel.SetActive(true);
        StartCoroutine(Type());
    }

    public void NextSentence()
    {
        if (index < sentences.Length - 1)
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        }
        else
        {
            dialoguePanel.SetActive(false);
            isTalking = false;

            
            Animator merchantAnim = GetComponent<Animator>();
            if (merchantAnim != null)
            {
                merchantAnim.Play("Static_Idle");
            }

            if (playerObj.GetComponent<Player>() != null)
            {
                playerObj.GetComponent<Player>().enabled = true;
            }
        }
    }

    IEnumerator Type()
    {
        textDisplay.text = "";
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}