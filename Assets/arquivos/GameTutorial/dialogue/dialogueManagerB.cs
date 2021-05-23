using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class dialogueManagerB : MonoBehaviour
{

    public TextMeshProUGUI dialogueText;

    private Queue<string> sentences;

    public Image nome_do_elemental, elemento_carta, cristais_necessarios, descricao_carta,
                   estagio_da_evolucao,luz, jayFogoPrefab, carta_auxiliar, forte_conta, pontos_de_ataque, pontos_de_defesa;

    public dialogueB dialogueB;

    public Image descricao_do_efeito, nome_da_carta, tipo_de_carta, tipo_de_efeito;

    public GameObject  charB, charA, painelParabens,  maoBancoCristais, hand3,objetoInstantiate, objetoInstantiate2, objetoInstantiate3, informacoes_player, markFieldDefault, markField1, markField2;

   
    // Use this for initialization
    void Start()
    {

        sentences = new Queue<string>();

        TriggerDialogue();
        //dialogueText = GetComponent<TextMeshProUGUI>();

    }

    private void Update()
    {
       
            if (Input.GetMouseButtonDown(0) && charA.activeInHierarchy == false && charB.activeInHierarchy == true)
                DisplayNextSentence();
       
       
    }


    public void StartDialogue(dialogueB dialogue)//metodo para começar o dialogo
    {


        sentences.Clear();//limpar sentenças

        foreach (string sentence in dialogue.sentences)//procurar por frases (sentenças) na classe dialogo
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();//passar de sentenças (dialogos)


    }

    IEnumerator aparecerMaoInformacaoPlayer(int tempo)
    {
        yield  return new WaitForSeconds(tempo);
        hand3.SetActive(true);
    }

    public void DisplayNextSentence()//metodo para a passsage de dialogos
    {
             
        if (sentences.Count == 19)//se a contagem do dialogo for igual a zero, vai para o metodo do fim do dialogo
        {

            markFieldDefault.SetActive(true);
           
        }

        if (sentences.Count == 18)//se a contagem do dialogo for igual a zero, vai para o metodo do fim do dialogo
        {
            Destroy(markFieldDefault);
            markField1.SetActive(true);
            
        }

        if (sentences.Count == 17)//se a contagem do dialogo for igual a zero, vai para o metodo do fim do dialogo
        {
            Destroy(markField1);
            markField2.SetActive(true);
            
        }

        if (sentences.Count == 16)//se a contagem do dialogo for igual a zero, vai para o metodo do fim do dialogo
        {
            
            Destroy(markField2);
           
            informacoes_player.SetActive(true);
            StartCoroutine(aparecerMaoInformacaoPlayer(2));
            

        }

        if (sentences.Count == 15)//se a contagem do dialogo for igual a zero, vai para o metodo do fim do dialogo
        {

            Destroy(hand3);
            Destroy(informacoes_player);
                    
            Instantiate(jayFogoPrefab, objetoInstantiate.transform);
            Instantiate(luz, objetoInstantiate3.transform);

        }



        if (sentences.Count == 14)//se a contagem do dialogo for igual a zero, vai para o metodo do fim do dialogo
        {
            Instantiate(nome_do_elemental, objetoInstantiate.transform);
            Destroy(jayFogoPrefab);
        }

        if (sentences.Count == 13)//se a contagem do dialogo for igual a zero, vai para o metodo do fim do dialogo
        {
            Instantiate(elemento_carta, objetoInstantiate.transform);
            //dialogueText.alignment = TextAnchor.MiddleCenter;
        }

        if (sentences.Count == 12)//se a contagem do dialogo for igual a zero, vai para o metodo do fim do dialogo
        {
            Instantiate(pontos_de_ataque, objetoInstantiate.transform);

        }

        if (sentences.Count == 11)//se a contagem do dialogo for igual a zero, vai para o metodo do fim do dialogo
        {
            Instantiate(pontos_de_defesa, objetoInstantiate.transform);

        }

        if (sentences.Count == 10)//se a contagem do dialogo for igual a zero, vai para o metodo do fim do dialogo
        {
            Instantiate(descricao_carta, objetoInstantiate.transform);

        }

        if (sentences.Count == 9)//se a contagem do dialogo for igual a zero, vai para o metodo do fim do dialogo
        {
            Instantiate(forte_conta, objetoInstantiate.transform);

        }

        if (sentences.Count == 6)//se a contagem do dialogo for igual a zero, vai para o metodo do fim do dialogo
        {
            Destroy(objetoInstantiate);
            maoBancoCristais.SetActive(true);

        }

        if (sentences.Count == 5)//se a contagem do dialogo for igual a zero, vai para o metodo do fim do dialogo
        {
            Destroy(maoBancoCristais);
           
            Instantiate(carta_auxiliar, objetoInstantiate2.transform);
        }

        if (sentences.Count == 3)//se a contagem do dialogo for igual a zero, vai para o metodo do fim do dialogo
        {
            Instantiate(descricao_do_efeito, objetoInstantiate2.transform);

        }

        if (sentences.Count == 1)//se a contagem do dialogo for igual a zero, vai para o metodo do fim do dialogo
        {

            Destroy(objetoInstantiate2);
            Destroy(objetoInstantiate3);
            
        }
        if (sentences.Count == 0)//se a contagem do dialogo for igual a zero, vai para o metodo do fim do dialogo
        {

            
            painelParabens.SetActive(true);
        }


        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
        

    }

    public void TriggerDialogue()
    {
        FindObjectOfType<dialogueManagerB>().StartDialogue(dialogueB);

    }



}
