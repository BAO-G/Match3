using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePiece : MonoBehaviour
{
    private GamePiece piece;
    private IEnumerator moveCoroutinue;

    private void Awake()
    {
        piece = GetComponent<GamePiece>();
    }

    public void Move(int newX, int newY, float time)
    {
        //piece.X = newX;
        //piece.Y = newY;
        //piece.transform.localPosition = piece.GridSystem.GetWorldPosition(newX, newY);

        if(moveCoroutinue != null)
        {
            StopCoroutine(moveCoroutinue);
        }

        moveCoroutinue = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutinue);
    }

    private IEnumerator MoveCoroutine(int newX, int newY,float time)
    { 
        piece.X = newX;
        piece.Y = newY;

        Vector3 startPos = transform.position;
        Vector3 endPos = piece.GridSystem.GetWorldPosition(newX, newY);

        for(float t = 0; t < 1; t += Time.deltaTime / time)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        piece.transform.position = endPos;
    }
}
