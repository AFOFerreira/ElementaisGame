using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class FacebookSetings : MonoBehaviour
{
    private void Start()
    {

    }
    // Awake function from Unity's MonoBehavior
    void Awake()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Falha ao iniciar o sdk do facebook");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    #region Login / Logout
    public void FacebookLogin()
    {
        var permissions = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(permissions, (ILoginResult r) =>
        {
            if (r.Error != null)
            {
                Debug.Log("error: " + r.Error);
                Main.Instance.ShowError(r.Error);
            }
            else
            {

                if (FB.IsLoggedIn)
                {

                    // AccessToken class will have session details
                    var aToken = AccessToken.CurrentAccessToken;

                    // Print current access token's User ID
                    Debug.Log(aToken.UserId);

                    FB.API("/me?fields=email,name,first_name", HttpMethod.POST, async result =>
                    {
                        if (result.Error == null)
                        {
                            var nome = result.ResultDictionary["name"].ToString();
                            var email = result.ResultDictionary["email"].ToString();
                            var primeiroNome = result.ResultDictionary["first_name"].ToString();
                            var nickname = primeiroNome + Random.Range(1, 10000).ToString();
                            Debug.Log("Nome: " + nome + "\nEmail: " + email + "\nPrimeiro Nome: " + primeiroNome + "\nNickName: " + nickname);
                            var x = await Main.Instance.web.cadastrarFG(nome, email, nickname);
                            if (x == "0")
                            {
                                Debug.Log("usuario já cadastrado no sistema");
                                var y = await Main.Instance.web.LoginFG(email, "1");
                                if (y == "0")
                                {
                                    Debug.Log("Erro: nao foi possivel logar");
                                    Main.Instance.ShowError("Parece que algo deu errado! não se preocupe, estamos trabalhando para corrigir isso!");
                                }
                                else
                                {
                                    JObject json = JObject.Parse(y);
                                    var v = JsonConvert.DeserializeObject<config_usuario>(json.ToString());
                                    Main.Instance.usuario.Nome = v.Nome;
                                    Main.Instance.usuario.Nickname = v.Nickname;
                                    Main.Instance.usuario.Xp = v.Xp;
                                    Main.Instance.usuario.Nivel = v.Nivel;
                                    Main.Instance.MainMenu();
                                }

                            }
                            else if (x == "1")
                            {
                                Debug.Log("usuario cadastrado com sucesso!");
                                var y = await Main.Instance.web.LoginFG(email, "1");
                                if (y == "0")
                                {
                                    Debug.Log("Erro: nao foi possivel logar");
                                    Main.Instance.ShowError("Parece que algo deu errado! não se preocupe, estamos trabalhando para corrigir isso!");
                                }
                                else
                                {
                                    JObject json = JObject.Parse(y);
                                    var v = JsonConvert.DeserializeObject<config_usuario>(json.ToString());
                                    Main.Instance.usuario.Nome = v.Nome;
                                    Main.Instance.usuario.Nickname = v.Nickname;
                                    Main.Instance.usuario.Xp = v.Xp;
                                    Main.Instance.usuario.Nivel = v.Nivel;
                                    Main.Instance.MainMenu();
                                }
                            }
                        }
                        else
                        {
                            Debug.Log(result.Error);
                            Main.Instance.ShowError(r.Error);
                        }
                    });

                }
                else
                {
                    Debug.Log("User cancelled login");
                }
            }
        });
    }

    public void FacebookLogout()
    {
        FB.LogOut();
    }
    #endregion

    public void GetPicture(Image img)
    {
        Rect rect = new Rect();
        rect.width = 128;
        rect.height = 128;
        if (FB.IsInitialized && FB.IsLoggedIn)
        {
            FB.API("me/picture?redirect=false", HttpMethod.GET, async result =>
            {
                if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
                {
                    IDictionary data = (IDictionary)result.ResultDictionary["data"];
                    string photoURL = data["url"].ToString();

                    UnityWebRequest www = UnityWebRequestTexture.GetTexture(photoURL);
                    Debug.Log(photoURL);
                    await www.SendWebRequest();

                    if (!www.isNetworkError)
                    {
                        Texture2D texture = DownloadHandlerTexture.GetContent(www);
                        img.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 100);
                    }
                }
            });
        }
        else
        {
            Debug.Log("Facebook nao inicializado");
        }

    }


    public void FacebookShareLink()
    {
        FB.ShareLink(new System.Uri("http://elementaisgame.com"), "de uma olhada", "nosso primeiro link de compartilhamento");
    }

    #region Convidar
    public void FacebookGameRequest()
    {
        FB.AppRequest("Acabei de conquistar uma incrivel carta!!", title: "hey, venha se tornar um guardião comigo!!");
    }

    public void FacebookInvite()
    {
        //colocar o link do jogo na google play ou appstore
        //FB.Mobile.AppInvite(new System.Uri());
    }
    #endregion

    public void GetFriendsPlayingThisGame()
    {
        string query = "/me/friends";
        FB.API(query, HttpMethod.GET, result =>
        {
            var dictionary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
            var friendsList = (List<object>)dictionary["data"];
            //friendsText.text = string.Empty;
            //foreach (var dict in friendsList) 
            //{
            //    friendsText.text += ((Dictionary<string, object>)dict)["name"]; 
            //}
        });
    }
}
