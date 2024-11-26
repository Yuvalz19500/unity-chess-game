using System;
using Enums;
using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Board/Starting Layout")]
    public class BoardStartingLayout : ScriptableObject
    {
        [Serializable]
        private struct BoardSquareInfo
        {
            public Vector2Int position;
            public PieceType pieceType;
            public TeamColor teamColor;
        }

        [SerializeField] private BoardSquareInfo[] boardSquares;

        public int GetPiecesCount()
        {
            return boardSquares.Length;
        }

        public Vector2Int GetPiecePositionAtIndex(int index)
        {
            return boardSquares[index].position;
        }

        public string GetPieceNameAtIndex(int index)
        {
            return boardSquares[index].pieceType.ToString();
        }

        public TeamColor GetPieceTeamColorAtIndex(int index)
        {
            return boardSquares[index].teamColor;
        }
    }
}