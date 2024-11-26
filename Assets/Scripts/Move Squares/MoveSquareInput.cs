using System;
using Core;
using UnityEngine;

namespace Move_Squares
{
    public class MoveSquareInput : MonoBehaviour
    {
        private Board _board;
        private AvailableMoveSquareCreator _squareCreator;

        private void Awake()
        {
            _board = GameObject.FindWithTag("Board").GetComponent<Board>();
            _squareCreator = _board.GetComponent<AvailableMoveSquareCreator>();
        }

        private void OnMouseDown()
        {
            _board.OnSquareSelected(_squareCreator.GetMoveInfoForSelectedSquare(gameObject));
        }
    }
}