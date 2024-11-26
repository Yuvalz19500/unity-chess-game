using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Managers;
using Pieces;
using UnityEngine;

namespace Core
{
    public class Player
    {
        private readonly Board _board;

        public List<Piece> ActivePieces { get; } = new List<Piece>();
        public TeamColor Team { get; }

        public Player(TeamColor team, Board board)
        {
            Team = team;
            _board = board;
        }

        public void AddPiece(Piece piece)
        {
            if (!ActivePieces.Contains(piece))
            {
                ActivePieces.Add(piece);
            }
        }

        public void RemovePiece(Piece piece)
        {
            if (ActivePieces.Contains(piece))
            {
                ActivePieces.Remove(piece);
            }
        }

        public void GeneratePossibleMoves()
        {
            foreach (Piece piece in ActivePieces.Where(piece => _board.IsPieceOnBoard(piece)))
            {
                piece.SetAvailableMoves();
            }
        }

        public Piece[] GetPiecesAttackingOppositePieceOfType<T>() where T : Piece
        {
            return ActivePieces.Where(p => p.IsAttackingPieceOfType<T>()).ToArray();
        }

        public void RemoveMovesEnablingAttackOnPieceOfType<T>(Player opponent, Piece selectedPiece) where T : Piece
        {
            List<MoveInfo> movesToRemove = new List<MoveInfo>();
            foreach (KeyValuePair<MoveInfo, PieceMoveType> move in selectedPiece.MovesDict.ToDictionary(
                         entry => entry.Key, entry => entry.Value))
            {
                Vector2Int originalCoord = selectedPiece.SquarePosition;
                Piece pieceOnCoord = _board.GetPieceOnBoardFromSquareCoords(move.Key.GridPosition);
                _board.UpdateBoardOnPieceMove(selectedPiece, move.Key.GridPosition, selectedPiece.SquarePosition);
                if (pieceOnCoord)
                {
                    pieceOnCoord.Disabled = true;
                }

                opponent.GeneratePossibleMoves();

                if (opponent.GetPiecesAttackingOppositePieceOfType<T>().Length != 0)
                {
                    movesToRemove.Add(move.Key);
                }

                _board.UpdateBoardOnPieceMove(selectedPiece, originalCoord, move.Key.GridPosition);

                if (!pieceOnCoord) continue;

                _board.UpdateBoardOnPieceMove(pieceOnCoord, pieceOnCoord.SquarePosition, pieceOnCoord.SquarePosition);
                pieceOnCoord.Disabled = false;
            }

            foreach (MoveInfo moveToRemove in movesToRemove)
            {
                selectedPiece.MovesDict.Remove(moveToRemove);
            }
        }

        public Piece[] GetPiecesOfType<T>() where T : Piece
        {
            return ActivePieces.Where(piece => piece is T).ToArray();
        }

        public bool CanCoverPieceFromAttack<T>(Player opponent) where T : Piece
        {
            foreach (Piece piece in ActivePieces)
            {
                foreach (KeyValuePair<MoveInfo, PieceMoveType> move in piece.MovesDict)
                {
                    Vector2Int originalCoord = piece.SquarePosition;
                    Piece pieceOnCoord = _board.GetPieceOnBoardFromSquareCoords(move.Key.GridPosition);
                    _board.UpdateBoardOnPieceMove(piece, move.Key.GridPosition, piece.SquarePosition);

                    opponent.GeneratePossibleMoves();

                    _board.UpdateBoardOnPieceMove(piece, originalCoord, move.Key.GridPosition);
                    if (pieceOnCoord)
                    {
                        _board.UpdateBoardOnPieceMove(pieceOnCoord, pieceOnCoord.SquarePosition,
                            pieceOnCoord.SquarePosition);
                    }

                    if (opponent.GetPiecesAttackingOppositePieceOfType<T>().Length == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}