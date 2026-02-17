using System;
using UnityEngine;

public class BaseCanvasV2<T> : BaseUI<T> where T : Enum
{
    public GameObject Parent;
    public GameObject[] SafeArea;

    [Header("Top Fill Settings")]
    public Sprite topSprite;
    public Color topColor = Color.white;

    [Header("Bottom Fill Settings")]
    public Sprite bottomSprite;
    public Color bottomColor = Color.white;

    void Awake()
    {
        if (SafeArea.Length > 0) Parent = SafeArea[0].transform.parent.gameObject;
    }

    public void SetActive(bool status)
    {
        gameObject.SetActive(status);
    }
}
