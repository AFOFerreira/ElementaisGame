using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueMananer : MonoBehaviour
{

    public TextMeshProUGUI dialogueText;

    private Queue<string> sentences;

    public Dialogue dialogue;
    
    public GameObject charA, charB;
    public Animator balao, balaoB;
    // Use this for initialization
    void Start()
    {

        sentences = new Queue<string>();

        TriggerDialogue();

        StartCoroutine(aparecerCharA(2));
        balao.GetComponent<Animator>();
        balaoB.GetComponent<Animator>();
    }
    public void StartDialogue(Dialogue dialogue)//metodo para começar o dialogo
    {

        sentences.Clear();//limpar sentenças

        foreach (string sentence in dialogue.sentences)//procurar por frases (sentenças) na classe dialogo
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();//passar de sentenças (dialogos)

    }
   
    IEnumerator aparecerCharA(int tempo)
    {
        yield return new WaitForSeconds(tempo);
        charA.SetActive(true);
       
        balao.SetBool("balaoTroca", true);
    }

    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && charA.activeInHierarchy == true)
            DisplayNextSentence();
    }


    public void DisplayNextSentence()//metodo para a passsage de dialogos
    {
        

        if (sentences.Count == 0)
        {

            charA.SetActive(false);         
            charB.SetActive(true);
            balaoB.SetBool("balaoTroca", true);
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;

    }

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueMananer>().StartDialogue(dialogue);

    }
}
