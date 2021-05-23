using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Anim_Background : MonoBehaviour
{
    private Image CurrentImage;
    public int tempoTrocaImages;
    float time;
    int n;
    bool b;
    public Sprite[] sprites;
    void Start()
    {
        CurrentImage = GetComponent<Image>();
        n = Random.Range(0, sprites.Length);
        CurrentImage.sprite = sprites[n];
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private void FixedUpdate()
    {
        time += Time.deltaTime;
        if (time >= tempoTrocaImages)
        {
            n = Random.Range(0, sprites.Length);
            CurrentImage.sprite = sprites[n];
            time = 0f;            
        }
       
    }
}
