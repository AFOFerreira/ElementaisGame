using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TxtAlertaGame : MonoBehaviour
{
    public TextMeshProUGUI txtMsg;

    public void Ativar(string msg)
    {
        txtMsg = GetComponent<TextMeshProUGUI>();
        txtMsg.SetText(msg);
        gameObject.SetActive(true);
    }

    public void Desativar()
    {
        gameObject.SetActive(false);
    }
}
