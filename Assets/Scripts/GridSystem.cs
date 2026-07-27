using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public enum PieceType
    {
        EMPTY,
        NORMAL,
        BUBBLE,
        COUNT,
    }

    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    }

    public int xDim;
    public int yDim;
    public float fillTime;
    private WaitForSeconds fillTimeWait;
    private bool inverse = false;

    private GamePiece pressedPiece;
    private GamePiece enteredPiece;

    public PiecePrefab[] piecePrefabs;
    private Dictionary<PieceType,GameObject> piecePrefabDict;
    // 背景格子
    [SerializeField] private GameObject backgroundPrefab;


    private GamePiece[,] pieces;
    
    void Start()
    {
        piecePrefabDict = new Dictionary<PieceType, GameObject>();
        // Add all the prefabs to the dictionary
        for(int i = 0; i < piecePrefabs.Length; i++)
        {
            if(!piecePrefabDict.ContainsKey(piecePrefabs[i].type))
            {
                piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }
        
        // 创建背景网格
        for(int i = 0; i < xDim; i++)
        {
            for(int j = 0; j < yDim; j++)
            {
                GameObject gameObject = Instantiate(backgroundPrefab, GetWorldPosition(i,j), Quaternion.identity);
                gameObject.transform.SetParent(transform);
            }
        }

        // 创建空格子
        pieces = new GamePiece[xDim, yDim];
        for(int i = 0; i < xDim; i++)
        {
            for(int j = 0; j < yDim; j++)
            {
                SpawnNewPiece(i, j, PieceType.EMPTY);

                //GameObject newpieces = Instantiate(piecePrefabDict[PieceType.NORMAL], Vector3.zero, Quaternion.identity);
                //newpieces.transform.SetParent(transform);
                //newpieces.name = i + " " + j;

                //pieces[i, j] = newpieces.GetComponent<GamePiece>();
                //pieces[i, j].Init(i, j, this, PieceType.NORMAL);

                //if(pieces[i, j].IsMovable())
                //{
                //    pieces[i, j].MovablePiece.Move(i, j);
                //}

                //if(pieces[i, j].IsColor())
                //{
                //    pieces[i, j].ColorPiece.SetColor((ColorPiece.ColorType)Random.Range(0, pieces[i, j].ColorPiece.NumColors));
                //}
            }
        }

        Destroy(pieces[0,0].gameObject);
        SpawnNewPiece(0, 0, PieceType.BUBBLE);
        
        Destroy(pieces[4,4].gameObject);
        SpawnNewPiece(4, 4, PieceType.BUBBLE);

        //Destroy(pieces[5,3].gameObject);
        //SpawnNewPiece(5, 3, PieceType.BUBBLE);

        //Destroy(pieces[6, 3].gameObject);
        //SpawnNewPiece(6, 3, PieceType.BUBBLE);

        fillTimeWait = new WaitForSeconds(fillTime);
        StartCoroutine(Fill());

    }


    void Update()
    {
    }

    /// <summary>
    /// 获取世界坐标
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - xDim / 2.0f + x, transform.position.y + yDim / 2.0f - y);
    }

    /// <summary>
    /// 创建新的格子
    /// </summary>
    public GamePiece SpawnNewPiece(int x, int y, PieceType type)
    {
        GameObject newPiece = Instantiate(piecePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);
        newPiece.transform.SetParent(transform);

        pieces[x,y] = newPiece.GetComponent<GamePiece>();
        pieces[x,y].Init(x, y, this, type);

        return pieces[x,y];
    }

    /// <summary>
    /// 判断两个格子是否相邻
    /// </summary>
    /// <param name="piece1"></param>
    /// <param name="piece2"></param>
    /// <returns></returns>
    public bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1)
            || (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1);
    }

    /// <summary>
    /// 交换两个格子
    /// </summary>
    /// <param name="piece1"></param>
    /// <param name="piece2"></param>
    public void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        if(piece1.IsMovable() && piece2.IsMovable())
        {
            pieces[piece1.X, piece1.Y] = piece2;
            pieces[piece2.X, piece2.Y] = piece1;

            if(GetMatch(piece1,piece2.X,piece2.Y) != null || GetMatch(piece2,piece1.X,piece1.Y) != null)    // 判断是否匹配
            {
                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                piece1.MovablePiece.Move(piece2.X, piece2.Y, fillTime);
                piece2.MovablePiece.Move(piece1X, piece1Y, fillTime);
            }
            else
            {
                pieces[piece1.X, piece1.Y] = piece1;
                pieces[piece2.X, piece2.Y] = piece2;
            }
        }
    }

    /// <summary>
    /// 按压格子
    /// </summary>
    /// <param name="piece"></param>
    public void PressPiece(GamePiece piece)
    {
        pressedPiece = piece;
    }

    public void EnterPiece(GamePiece piece)
    {
        enteredPiece = piece;
    }

    /// <summary>
    /// 释放格子
    /// </summary>
    public void ReleasePiece()
    {
        if(IsAdjacent(pressedPiece,enteredPiece))   // 判断是否相邻
        {
            SwapPieces(pressedPiece, enteredPiece);
        }
    }

    /// <summary>
    /// 获取匹配的格子
    /// </summary>
    public List<GamePiece> GetMatch(GamePiece piece,int newX, int newY)
    {
        if(piece.IsColored())
        {
            ColorPiece.ColorType color = piece.ColorPiece.Color;
            List<GamePiece> horizontalPieces = new List<GamePiece>();
            List<GamePiece> verticalPieces = new List<GamePiece>();
            List<GamePiece> matchingPieces = new List<GamePiece>();

            #region 横向
            horizontalPieces.Add(piece);
            
            for(int dir =0; dir<=1;dir++)   // 0 左 1 右
            {
                for(int xOffset = 1; xOffset<xDim; xOffset++)   // 横向
                {
                    int x;

                    if(dir == 0)
                    {
                        x = newX - xOffset;
                    }
                    else
                    {
                        x = newX + xOffset;
                    }

                    if(x < 0 || x >= xDim)
                    {
                        break;
                    }

                    if(pieces[x,newY].IsColored() && pieces[x, newY].ColorPiece.Color == color)
                    {
                        horizontalPieces.Add(pieces[x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if(horizontalPieces.Count >= 3)
            {
                for(int i = 0; i < horizontalPieces.Count; i++)
                {
                    matchingPieces.Add(horizontalPieces[i]);
                }
            }

            if(horizontalPieces.Count >= 3)
            {
                for(int i =0; i< horizontalPieces.Count; i++)
                {
                    for(int dir =0; dir <= 1; dir++)
                    {
                        for(int yOffset = 1; yOffset < yDim; yOffset++) 
                        {
                            int y;
                            if (dir == 0)
                            {
                                y = newY - yOffset;
                            }
                            else
                            {
                                y = newY + yOffset;
                            }
                            
                            if(y < 0 || y >= yDim)
                            {
                                break;
                            }

                            // 检查垂直方向上的游戏块是否与指定颜色匹配，并收集匹配的块
                            // 如果遇到不匹配的块则停止检查
                            if (pieces[horizontalPieces[i].X, y].IsColored() && pieces[horizontalPieces[i].X, y].ColorPiece.Color == color)
                            {
                                // 当前位置的块有色且颜色匹配，将其添加到垂直块列表中
                                Debug.Log($"Adding piece at position ({horizontalPieces[i].X}, {y}) to verticalPieces. Color: {pieces[horizontalPieces[i].X, y].ColorPiece.Color}");
                                verticalPieces.Add(pieces[horizontalPieces[i].X, y]);
                            }
                            else
                            {
                                // 遇到不匹配的块或无色块，停止垂直方向的检查
                                break;  
                            }
                        }
                    }

                    // 检查垂直方向上的匹配片段数量，如果少于2个则清空列表，否则将所有垂直片段添加到匹配结果中
                    // 该逻辑块用于处理垂直方向的匹配检测和结果合并
                    if (verticalPieces.Count < 2)
                    {
                        // 垂直方向匹配片段不足，清空列表
                        verticalPieces.Clear();
                        Debug.Log("verticalPieces.Clear();");
                    }
                    else
                    {
                        // 将所有垂直方向的匹配片段添加到总匹配结果中
                        for (int j = 0; j < verticalPieces.Count; j++)
                        {
                            Debug.Log("verticalPieces.Add(pieces[horizontalPieces[i].X, y]);");
                            matchingPieces.Add(verticalPieces[j]);
                        }
                        // 找到有效匹配，跳出循环
                        break;
                    }
                }
            }

            if(matchingPieces.Count >= 3)
            {
                Debug.Log("return matchingPieces;");
                return matchingPieces;
            }
            #endregion

            #region 纵向
            horizontalPieces.Clear();
            verticalPieces.Clear();
            verticalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < yDim; yOffset++)
                {
                    int y;

                    if (dir == 0)
                    {
                        y = newY + yOffset;
                    }
                    else
                    {
                        y = newY + yOffset;
                    }

                    if (y < 0 || y >= yDim)
                    {
                        break;
                    }

                    //判断格子颜色
                    if (pieces[newX, y].IsColored() && pieces[newX, y].ColorPiece.Color == color)
                    {
                        verticalPieces.Add(pieces[newX, y]);
                    }
                    else //颜色不匹配
                    {
                        break;
                    }
                }
            }

            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    matchingPieces.Add(verticalPieces[i]);
                }
            }

            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < xDim; xOffset++)
                        {
                            int x;
                            if (dir == 0)
                            {
                                x = newX - xOffset;
                            }
                            else
                            {
                                x = newX + xOffset;
                            }

                            if (x < 0 || x >= xDim)
                            {
                                break;
                            }

                            // 检查水平方向上的游戏块是否与指定颜色匹配，并收集匹配的块
                            // 如果遇到不匹配的块则停止检查
                            if (pieces[x, verticalPieces[i].Y].IsColored() && pieces[x, verticalPieces[i].Y].ColorPiece.Color == color)
                            {
                                // 当前位置的块有色且颜色匹配，将其添加到水平块列表中
                                horizontalPieces.Add(pieces[x, verticalPieces[i].Y]);
                            }
                            else
                            {
                                // 遇到不匹配的块或无色块，停止垂直方向的检查
                                break;
                            }
                        }
                    }

                    if (horizontalPieces.Count < 2)
                    {
                        horizontalPieces.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < horizontalPieces.Count; j++)
                        {
                            matchingPieces.Add(horizontalPieces[j]);
                        }
                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }
            #endregion
        }

        return null;
    }


    #region 填充
    public IEnumerator Fill()
    {
        while(FillStep())
        {
            inverse = !inverse; //每次 FillStep 后反转方向
            yield return new WaitForSeconds(fillTime);
        }
    }

    /// <summary>
    /// 填充一行
    /// </summary>
    /// <returns></returns>
    public bool FillStep()
    {
        bool movePiece = false;

        for(int y = yDim-2; y>=0; y--)
        {
            // 填充行  
            for (int loopX = 0; loopX < xDim; loopX++)
            {
                int x = loopX;

                // 如果需要反向处理，则将当前X坐标转换为反向坐标
                // 通过用X维度的最大值减去当前循环X坐标来实现反向映射
                if (inverse)
                {
                    x = xDim - 1 - loopX;
                }

                GamePiece piece = pieces[x, y];
                // 移动格子
                if (piece.IsMovable())
                {
                    GamePiece pieceBlow = pieces[x, y + 1];
                    //垂直移动
                    if (pieceBlow.Type == PieceType.EMPTY)
                    {
                        Destroy(pieceBlow.gameObject);
                        piece.MovablePiece.Move(x, y + 1, fillTime);
                        pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        movePiece = true;
                    }
                    else
                    {
                        //检测斜向格子
                        for(int diag = -1; diag <= 1; diag ++)
                        {
                            // 检查并处理游戏棋子的对角线移动逻辑
                            // 该代码块用于判断当前棋子是否可以沿对角线方向移动到相邻位置
                            // 主要逻辑包括：
                            // 1. 计算对角线目标位置坐标
                            // 2. 检查目标位置是否在有效范围内
                            // 3. 验证目标位置是否为空位且上方无阻挡物
                            // 4. 如果满足条件则执行棋子移动操作
                            if (diag != 0)
                            {
                                int diagX = x + diag;   // 计算对角线x坐标，根据inverse标志决定是向左还是向右偏移

                                if (inverse)    // 如果是反向移动，则从当前位置减去偏移量
                                {
                                    diagX = x - diag;
                                }

                                if(diagX >= 0 && diagX < xDim)  // 检查计算后的对角线坐标是否在有效范围内
                                {
                                    GamePiece diagonalPiece = pieces[diagX, y + 1];     //获取对角线位置下方的棋子对象

                                    if(diagonalPiece.Type == PieceType.EMPTY)   // 检查该位置是否为空位（可放置棋子）
                                    {
                                        bool hasPieceAbove = true;  // 标记对角线上方是否存在不可移动的棋子阻挡

                                        for(int aboveY = y; aboveY >= 0; aboveY--)  // 从当前位置向上遍历检查是否有阻挡物
                                        {
                                            GamePiece pieceAbove = pieces[diagX, aboveY];   // 获取上方指定位置的棋子

                                            if(pieceAbove.IsMovable())  // 如果遇到可移动的棋子，则停止检查（视为无阻挡）
                                            {
                                                break;
                                            }
                                            else if(!pieceAbove.IsMovable() && pieceAbove.Type != PieceType.EMPTY)  // 如果遇到不可移动且非空的棋子，则标记为有阻挡并停止检查
                                            {
                                                hasPieceAbove = false;
                                                break;
                                            }
                                        }

                                        if(!hasPieceAbove)   // 如果对角线上方无阻挡，则执行棋子移动操作
                                        {
                                            // 销毁目标位置的空棋子对象
                                            Destroy(diagonalPiece.gameObject);
                                            // 移动当前棋子到对角线位置
                                            piece.MovablePiece.Move(diagX, y + 1, fillTime);
                                            // 更新棋盘数组中的棋子位置信息
                                            pieces[diagX, y + 1] = piece;
                                            // 在原位置生成新的空棋子
                                            SpawnNewPiece(x, y, PieceType.EMPTY);
                                            // 标记已执行移动操作并跳出循环
                                            movePiece = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // 填充列
        for (int x = 0; x < xDim; x++)
        {
            GamePiece pieceBlow = pieces[x, 0];

            if (pieceBlow.Type == PieceType.EMPTY)
            {
                Destroy(pieceBlow.gameObject);
                GameObject newPiece = Instantiate(piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity);
                newPiece.transform.SetParent(transform);

                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                pieces[x, 0].MovablePiece.Move(x, 0, fillTime);
                pieces[x, 0].ColorPiece.SetColor((ColorPiece.ColorType)Random.Range(0, pieces[x, 0].ColorPiece.NumColors));
                movePiece = true;
            }
        }

        return movePiece;
    }

    #endregion
}
