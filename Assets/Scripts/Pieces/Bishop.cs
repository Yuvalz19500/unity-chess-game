using Enums;
using UnityEngine;

namespace Pieces
{
    public class Bishop : Piece
    {
        private readonly Vector2Int[] _directions = new Vector2Int[]
        {
            new Vector2Int(1, 1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1)
        };
        
        public override void SetAvailableMoves()
        {
            MovesDict.Clear();
            foreach (Vector2Int direction in _directions)
            {
                for (int i = 1; i <= Board.GetBoardSize(); i++)
                {
                    Vector2Int possibleMoveCoord = SquarePosition + (direction * i);
                    if (!Board.CheckIfCoordsAreOnBoard(possibleMoveCoord)) break;
                    
                    Piece piece = Board.GetPieceOnBoardFromSquareCoords(possibleMoveCoord);
                    if (piece)
                    {
                        if(piece.Team == Team) break;
                        MovesDict.Add(new MoveInfo(possibleMoveCoord, Board.CalculateBoardPositionFromSquarePosition(possibleMoveCoord)), PieceMoveType.Take);
                        break;
                    }
                    else
                    {
                        MovesDict.Add(new MoveInfo(possibleMoveCoord, Board.CalculateBoardPositionFromSquarePosition(possibleMoveCoord)), PieceMoveType.Move);
                    }
                }
            }
        }
    }
}