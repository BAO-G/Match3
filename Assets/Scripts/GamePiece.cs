using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    private int x;
    private int y;
    private GridSystem gridSystem;
    private GridSystem.PieceType type;
    private MovablePiece movableComponent;
    private ColorPiece colorPiece;

    public int X 
    { 
        get { return x; } 
        set 
        { 
            if(IsMovable())
            {
                x = value; 
            }
        }
    }
    public int Y 
    { 
        get { return y; }
        set
        {
            if (IsMovable())
            {
                y = value;
            }
        }
    }
    public GridSystem GridSystem { get { return gridSystem; } }
    public GridSystem.PieceType Type { get { return type; } }
    public MovablePiece MovablePiece { get { return movableComponent; } }
    public ColorPiece ColorPiece { get { return colorPiece; } }

    private void Awake()
    {
        movableComponent = GetComponent<MovablePiece>();
        colorPiece = GetComponent<ColorPiece>();
    }


    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <param name="_gridSystem"></param>
    /// <param name="_type"></param>
    public void Init(int _x,int _y, GridSystem _gridSystem, GridSystem.PieceType _type)
    {
        x = _x;
        y = _y;
        gridSystem = _gridSystem;
        type = _type;
    }


    private void OnMouseEnter()
    {
        gridSystem.EnterPiece(this);
    }

    void OnMouseDown()
    {
        gridSystem.PressPiece(this);
    }

    void OnMouseUp()
    {
        gridSystem.ReleasePiece();
    }

    /// <summary>
    /// 是否可移动
    /// </summary>
    /// <returns></returns>
    public bool IsMovable()
    {
        return movableComponent != null;
    }

    public bool IsColored()
    {
        return colorPiece != null;
    }
}
