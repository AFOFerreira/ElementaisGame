using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Erro_Tela : MonoBehaviour
{
    public TextMeshProUGUI erro;
    void Start()
    {
        LeanTween.scale(gameObject, new Vector2(1, 1), 0.3f);
    }
    public void Close()
    {
        LeanTween.scale(gameObject, new Vector2(0, 0), 0.3f).setOnComplete(DestroyMe);
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void MensagemErro(string msg)
    {
        erro.text = msg;
    }
}
