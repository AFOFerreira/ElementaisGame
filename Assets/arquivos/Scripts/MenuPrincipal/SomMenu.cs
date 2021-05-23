using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SomMenu : MonoBehaviour
{
    private AudioBase audioBaseScript;
    // Start is called before the first frame update
    void Start()
    {
        audioBaseScript = Main.Instance.audioBase;

        audioBaseScript.playMusicaMenu();
    }

   public void somBotaoLateral()
    {
        audioBaseScript.playCliqueBotaoElemento(1);
    }
    public void somBotaoRedes()
    {
        audioBaseScript.playCliqueBotaoPlay();
    }

    public void pararMusicaMenu()
    {
        audioBaseScript.stopMusicaMenu();
    }

    public void musicaCarregando()
    {
        audioBaseScript.playFanfarraLoading();
    }
    public void somPlayGame()
    {
        audioBaseScript.playCliqueBotaoPlay();
    }
}
