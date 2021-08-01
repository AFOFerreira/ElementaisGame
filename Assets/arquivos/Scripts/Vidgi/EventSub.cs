using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EventSub : MonoBehaviour
{
    static public APILogin res;


    public void Start()
    {
        Invoke("StartEventSub", 0.5f);
    }

    public void StartEventSub()
    {
        StartCoroutine(Game_Code());
    }
    IEnumerator Game_Code()
    {
        CodeMagus magusCode = new CodeMagus();
        magusCode.game_code = "MP0101";

        var json = JsonUtility.ToJson(magusCode);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
       

        UnityWebRequest www = UnityWebRequest.Post("https://games.vidgi.com.br/event_by_game_code", json);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();
        var data = www.downloadHandler.text;

        res = JsonUtility.FromJson<APILogin>(data);

        if (res.code != "200")
        {
            Debug.Log("Erro -> " + res.message);
        }
        
    }
}

