using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class TelaLogin : MonoBehaviour
{
    [SerializeField]
    private InputField usernameField = null;
    [SerializeField]
    private InputField passwordField = null;
    [SerializeField]
    private GameObject loading = null;
    [SerializeField]
    private Toggle rememberData = null;
    [SerializeField]
    private GameObject wrong = null;

    static public APILogin res;

    void Start()
    {
       
        if (PlayerPrefs.HasKey("remember") && PlayerPrefs.GetInt("remember") == 1)
        {
            usernameField.text = PlayerPrefs.GetString("rememberLogin");
            passwordField.text = PlayerPrefs.GetString("rememberPass");
        }
    }

    public void Login()
    {
       StartCoroutine(GetLogin());
    }

    IEnumerator GetLogin()
    {
        if (rememberData.isOn)
        {
            PlayerPrefs.SetInt("remember", 1);
            PlayerPrefs.SetString("rememberLogin", usernameField.text);
            PlayerPrefs.SetString("rememberPass", passwordField.text);
        }

        UserData user = new UserData();
        user.user_email = usernameField.text;
        user.user_pass = passwordField.text;

        var json = JsonUtility.ToJson(user);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        

        UnityWebRequest www = UnityWebRequest.Post("https://games.vidgi.com.br/auth", json);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();
        var data = www.downloadHandler.text;
        
        res = JsonUtility.FromJson<APILogin>(data);

        if (res.code == "200")
        {
            loginOk();
        }
        else
        {
            Debug.Log("Erro -> " + res.message);
            Invoke("noLogin", 0.5f);
        }

    }

    

    void loginOn()
    {
        loading.SetActive(false);
        SceneManager.LoadScene("Player_Logon");
    }
    void loginFail()
    {
        wrong.SetActive(false);
    }

    void loginOk()
    {
        loading.SetActive(true);
        Invoke("loginOn", 3f);
    }

    void noLogin()
    {
        wrong.SetActive(true);
        Invoke("loginFail", 3f);
    }

    public void Register()
    {
        Application.OpenURL("https://torneios.vidgi.com.br/cadastro");
    }
}
