using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPiece : MonoBehaviour
{
    public enum ColorType   // 颜色类型
    {
        Yellow,
        Purple,
        Red,
        Blue,
        Green,
        Pink,
        Any,
        Count
    }
    
    [System.Serializable]
    public struct ColorSprite   // 颜色和精灵的映射
    {
        public ColorType Color;
        public Sprite Sprite;
    }

    public ColorType color;

    public ColorType Color
    {
        get
        {
            return color;
        }
        set
        {
            SetColor(value);
        }
    }


    [SerializeField] private ColorSprite[] colorSprites;
    public int NumColors => colorSprites.Length;

    private SpriteRenderer spriteRenderer;

    private Dictionary<ColorType, Sprite> colorSpriteDict;

    private void Awake()
    {
        spriteRenderer = transform.Find("Piece").GetComponent<SpriteRenderer>();

        colorSpriteDict = new Dictionary<ColorType, Sprite>();

        //添加颜色和精灵的映射
        for(int i = 0; i < colorSprites.Length; i++)
        {
            if(!colorSpriteDict.ContainsKey(colorSprites[i].Color))
            {
                colorSpriteDict.Add(colorSprites[i].Color, colorSprites[i].Sprite);
            }
        }
    }

    /// <summary>
    /// 设置颜色
    /// </summary>
    /// <param name="newColor"></param>
    public void SetColor(ColorType newColor)
    {
        color = newColor;

        if(colorSpriteDict.ContainsKey(newColor))
        {
            spriteRenderer.sprite = colorSpriteDict[newColor];
        }
    }
}
