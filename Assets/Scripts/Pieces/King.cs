using System;
using Enums;
using UnityEngine;

namespace Pieces
{
    public class King : Piece
    {
        private readonly Vector2Int[] _moveDirections = new Vector2Int[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(1, 1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1)
        };
        
        private readonly Vector2Int[] _castlingDirections = new Vector2Int[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0)
        };
        
        public override void SetAvailableMoves()
        {
            MovesDict.Clear();
            CreateNormalMoves();
            CreateCastlingMoves();
        }

        private void CreateCastlingMoves()
        {
            if (HasMoved) return;
            
            foreach (Vector2Int direction in _castlingDirections)
            {
                Vector2Int rookCoordFromDirection = new Vector2Int( direction.x == 1 ? (Board.GetBoardSize() - 1) * direction.x : 0, SquarePosition.y);
                Piece rookAtDirection =
                    Board.GetPieceOnBoardFromSquareCoords(rookCoordFromDirection);
                if (!rookAtDirection || rookAtDirection.GetType() != typeof(Rook) || rookAtDirection.HasMoved) continue;

                bool canCastle = true;
                for (int i = 1; i < Mathf.Abs(SquarePosition.x - rookCoordFromDirection.x); i++)
                {
                    Vector2Int possibleCastleCoord = SquarePosition + (direction * i);
                    if (!Board.CheckIfCoordsAreOnBoard(possibleCastleCoord))
                    {
                        canCastle = false;
                        break;
                    }

                    Piece pieceAtCoord = Board.GetPieceOnBoardFromSquareCoords(possibleCastleCoord);
                    if(pieceAtCoord)   
                    {
                        canCastle = false;
                        break;
                    }
                }

                if (canCastle)
                {
                    MovesDict.Add(new MoveInfo(rookCoordFromDirection, Board.CalculateBoardPositionFromSquarePosition(rookCoordFromDirection)), PieceMoveType.Castle);
                }
            }
        }

        private void CreateNormalMoves()
        {
            foreach (Vector2Int direction in _moveDirections)
            {
                Vector2Int possibleMoveCoord = SquarePosition + direction;
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