using Enums;
using UnityEngine;

namespace Pieces
{
    public class Knight : Piece
    {
        private readonly Vector2Int[] _offsets = new Vector2Int[]
        {
            new Vector2Int(-1, 2),
            new Vector2Int(1, 2),
            new Vector2Int(2, 1),
            new Vector2Int(-2, 1),
            new Vector2Int(-2, -1),
            new Vector2Int(2, -1),
            new Vector2Int(-1, -2),
            new Vector2Int(1, -2)
        };
        
        public override void SetAvailableMoves()
        {
            MovesDict.Clear();
            foreach (Vector2Int offset in _offsets)
            {
                Vector2Int possibleMoveCoord = SquarePosition + offset;
                
                if(!Board.CheckIfCoordsAreOnBoard(possibleMoveCoord)) continue;
                
                Piece piece = Board.GetPieceOnBoardFromSquareCoords(possibleMoveCoord);
                if (piece)
                {
                    if(piece.Team == Team) continue;
                    MovesDict.Add(new MoveInfo(possibleMoveCoord, Board.CalculateBoardPositionFromSquarePosition(possibleMoveCoord)), PieceMoveType.Take);
                }
                else
                {
                    MovesDict.Add(new MoveInfo(possibleMoveCoord, Board.CalculateBoardPositionFromSquarePosition(possibleMoveCoord)), PieceMoveType.Move);
                }
            }
        }
    }
}