using Enums;
using UnityEngine;

namespace Pieces
{
    public class Pawn : Piece
    {
        public override void SetAvailableMoves()
        {
            MovesDict.Clear();

            Vector2Int moveDirection = Team == TeamColor.White ? Vector2Int.up : Vector2Int.down;
            int takeDirection = Team == TeamColor.White ? 1 : -1;
            float range = HasMoved ? 1f : 2f;
            Vector2Int[] takeDirections = { new Vector2Int(1, 1), new Vector2Int(-1, 1) };

            CalculateAvailableMoves(moveDirection, range);
            CalculateAvailableTakes(takeDirection, takeDirections);
        }

        private void CalculateAvailableTakes(int direction, Vector2Int[] takeDirections)
        {
            foreach (Vector2Int takeDirection in takeDirections)
            {
                Vector2Int possibleTakeCoords = SquarePosition + direction * takeDirection;
                if (!Board.CheckIfCoordsAreOnBoard(possibleTakeCoords)) continue;
                Piece piece = Board.GetPieceOnBoardFromSquareCoords(possibleTakeCoords);
                if(piece == null || piece.IsPieceFromSameTeam(this)) continue;
                
                MovesDict.Add(new MoveInfo(possibleTakeCoords,Board.CalculateBoardPositionFromSquarePosition(possibleTakeCoords)), PieceMoveType.Take);
            }
        }

        private void CalculateAvailableMoves(Vector2Int direction, float range)
        {
            for (int i = 1; i <= range; i++)
            {
                Vector2Int possibleMoveCoords = SquarePosition + direction * i;
                if (!Board.CheckIfCoordsAreOnBoard(possibleMoveCoords)) break;
                Piece piece = Board.GetPieceOnBoardFromSquareCoords(possibleMoveCoords);
                if(piece != null) break;
                
                MovesDict.Add(new MoveInfo(possibleMoveCoords, Board.CalculateBoardPositionFromSquarePosition(possibleMoveCoords)), PieceMoveType.Move);
            }
        }
    }
}