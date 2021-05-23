using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class ImageManager : MonoBehaviour
{
    public static ImageManager instance;

    string _basePath;
    //TODO:
    //0. Make a base path
    //1. check if image already exists
    //2. save images
    //3. load images(io)
    void Start()
    {
        if (instance != null) 
        {
            GameObject.Destroy(this);
            return;
        }
        instance = this;

        _basePath = Application.persistentDataPath + "/Images/";
        if (!Directory.Exists(_basePath)) 
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    bool ImageExists(string name) 
    {
        return File.Exists(_basePath + name);
    }

    public void SaveImage(string name, byte[] bytes) 
    {
        File.WriteAllBytes(_basePath + name, bytes);
    }

    public byte[] LoadImages(string name) 
    {
        byte[] bytes = new byte[0];
        if(ImageExists(name))
            bytes =  File.ReadAllBytes(_basePath + name);
        return bytes;
    }

    public Sprite BytesToSprite(byte[] bytes) 
    {
        //create texture2D
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);

        //create sprite(to be placed in UI)
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }
}
