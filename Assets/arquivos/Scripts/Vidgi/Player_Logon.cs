using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Player_Logon : MonoBehaviour
{

    public static bool noBanner;
    public Text player;
    public Text tokenText;
    public Text[] titulo_Evento = new Text[5];
    public Text[] eventValue = new Text[5];
    public Text[] EventTicket = new Text[5];
    public Text[] confirmToken = new Text[5];



    string playerUser;
    string tokens;
    string tituloName;
    string event_Value;
    string event_Ticket;

    int idController = 0;
    static public int tokenController;

    void Start()
    {
        Invoke("DataUser", 1f);
    }


    public void DataUser()
    {
        playerUser = TelaLogin.res.data[0].user_nick;
        tokens = TelaLogin.res.data[0].tokens;
        player.text = playerUser;
        tokenText.text = tokens;
        for (int i = 58; i <= int.Parse(EventSub.res.data[idController].Id); i++)
        {
            tituloName = EventSub.res.data[idController].event_title;
            event_Value = EventSub.res.data[idController].event_value;
            event_Ticket = EventSub.res.data[idController].event_ticket;
            titulo_Evento[idController].text = tituloName;
            eventValue[idController].text = event_Value;
            EventTicket[idController].text = event_Ticket;
            confirmToken[idController].text = event_Ticket;
            idController++;
        }
    }
    public void PlayGame()
    {
        tokenController = 0;
        StartCoroutine(UserEvent());
    }
    public void PlayGame2()
    {
        tokenController = 1;
        StartCoroutine(UserEvent());
    }
    public void PlayGame3()
    {
        tokenController = 2;
        StartCoroutine(UserEvent());
    }
    public void PlayGame4()
    {
        tokenController = 3;
        StartCoroutine(UserEvent());
    }
    public void PlayGame5()
    {
        tokenController = 4;
        StartCoroutine(UserEvent());
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Fase_1-1");
        //Player.livesTwo = 2;
        //Player.scoreTwo = 0;
        //Player.ringsTwo = 0;
        //Player.lifeTwo = 3;
        //Player.lifeMax = 3;
        //GameManager.isPaused = false;
        //GameManager.gameOver = false;
    }

    public void LogOff()
    {
        SceneManager.LoadScene("Login");
    }

    IEnumerator UserEvent()
    {
        UserEvents user = new UserEvents();
        user.Id = EventSub.res.data[tokenController].Id;
        user.player_nick = TelaLogin.res.data[0].user_nick;

        var json = JsonUtility.ToJson(user);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);

        Debug.Log(tokenController);

        UnityWebRequest www = UnityWebRequest.Post("https://games.vidgi.com.br/event_sub", json);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();
        var data = www.downloadHandler.text;

        APILogin res = JsonUtility.FromJson<APILogin>(data);

        Debug.Log(res.code);
        Debug.Log(res.message);

        if (res.code == "200")
        {
            Invoke("StartGame", 1f);
        }
        else
        {
            Debug.Log("ERROR: " + res.message);
        }
    }
     public void openHouse()
    {
        Application.OpenURL("https://games.vidgi.com.br/");
    }

    public void openRank()
    {
        Application.OpenURL("https://games.vidgi.com.br/lista/MP0101");
    }
}
