using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Engine<T> : SingletonMonoBehaviour<Engine<T>>
{
    protected Dictionary<int, T> dictionary = new Dictionary<int, T>();

    protected override bool dontDestroyOnLoad => true;

    //読み込むテキストファイルのPath
    protected abstract string loadTextPath { get; }

    protected override void Awake()
    {
        base.Awake();

        if(System.IO.File.Exists("Assets/Resources/" + loadTextPath + ".txt"))
        {
            var textAsset = Resources.Load<TextAsset>(loadTextPath);
            if (textAsset)
            {
                LoadDictionary(textAsset);
            }
        }
        else
        {
            Debug.LogError("Not Found: " + loadTextPath);
        }
    }

    //テキストファイルから読み込み追加していく
    protected abstract void LoadDictionary(TextAsset textAsset);

    //keyからvalueを取得
    public T Get(int key)
    {
        if (dictionary.ContainsKey(key))
        {
            return dictionary[key];
        }

        return default(T);
    }

    public List<T> Gets(List<int> keys)
    {
        var values = new List<T>();

        foreach (int key in keys){
            var value = Get(key);
            if (value != null)
            {
                values.Add(value);
            }
        }

        return values;
    }
}
