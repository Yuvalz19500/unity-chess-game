using System;
using System.Collections.Generic;
using Enums;
using Managers;
using Move_Squares;
using Pieces;
using UnityEngine;

namespace Core
{
    [RequireComponent(typeof(AvailableMoveSquareCreator))]
    public class Board : MonoBehaviour
    {
        [SerializeField] private Transform bottomLeftSquare;
        [SerializeField] private float squareSize = 1.5f;

        private const int BoardSize = 8;
        private readonly Piece[,] _piecesGrid = new Piece[BoardSize, BoardSize];
        private Piece _activePiece;
        private AvailableMoveSquareCreator _squareCreator;

        public static event Action OnPawnPromotion; 

        private void Awake()
        {
            SetupDependencies();
            RegisterEvents();
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        private void RegisterEvents()
        {
            UIManager.OnPlayerPickPromotion += HandlePlayerPickPromotion;
        }

        private void UnregisterEvents()
        {
            UIManager.OnPlayerPickPromotion -= HandlePlayerPickPromotion;
        }

        private void SetupDependencies()
        {
            _squareCreator = GetComponent<AvailableMoveSquareCreator>();
        }

        private void MoveActivePieceAndTake(Piece pieceToTake, MoveInfo moveInfo)
        {
            if (moveInfo == null) return;
            GameManager.Instance.OnPieceTaken(pieceToTake);
            Destroy(pieceToTake.gameObject);
            
            UpdateBoardOnPieceMove(_activePiece, moveInfo.GridPosition, _activePiece.SquarePosition);
            _activePiece.MovePiece(moveInfo);
            
            if (CheckForPawnPromotion(_activePiece, moveInfo))
            {
                PromotePawn();
            }
            else
            {
                EndTurn();
            }
        }
        
        private void Castle(Piece rookToCastleWith, MoveInfo moveInfo)
        {
            if (moveInfo == null) return;

            Vector2Int kingNewGridPos = new(_activePiece.SquarePosition.x + (rookToCastleWith.SquarePosition.x < _activePiece.SquarePosition.x ? -2 : 2), 
                                            moveInfo.GridPosition.y);
            Vector2Int rookNewGridPos =
                new(rookToCastleWith.SquarePosition.x < _activePiece.SquarePosition.x
                        ? kingNewGridPos.x + 1
                        : kingNewGridPos.x - 1, moveInfo.GridPosition.y);

            if (!CheckIfCoordsAreOnBoard(kingNewGridPos) || !CheckIfCoordsAreOnBoard(rookNewGridPos)) return;

            MoveInfo kingMoveInfo = new(kingNewGridPos, CalculateBoardPositionFromSquarePosition(kingNewGridPos));
            MoveInfo rookMoveInfo = new(rookNewGridPos, CalculateBoardPositionFromSquarePosition(rookNewGridPos));

            _activePiece.MovesDict.Add(kingMoveInfo, PieceMoveType.Castle);
            _activePiece.MovesDict.Remove(moveInfo);

            UpdateBoardOnPieceMove(_activePiece, kingMoveInfo.GridPosition, _activePiece.SquarePosition);
            UpdateBoardOnPieceMove(rookToCastleWith, rookMoveInfo.GridPosition, rookToCastleWith.SquarePosition);
            
            _activePiece.MovePiece(kingMoveInfo);
            rookToCastleWith.MovePiece(rookMoveInfo);
            
            rookToCastleWith.ToggleCollider(true);
            
            EndTurn();
        }

        private void MoveActivePiece(MoveInfo moveInfo)
        {
            if (moveInfo == null) return;
            UpdateBoardOnPieceMove(_activePiece, moveInfo.GridPosition, _activePiece.SquarePosition);
            _activePiece.MovePiece(moveInfo);
            
            if (CheckForPawnPromotion(_activePiece, moveInfo))
            {
                PromotePawn();
            }
            else
            {
                EndTurn();
            }
        }

        private void PromotePawn()
        {
            OnPawnPromotion?.Invoke();
        }
        
        private void HandlePlayerPickPromotion(PieceType promotionPieceType)
        {
            Piece promotionPiece = GameManager.Instance.CreatePieceForPromotion(_activePiece, promotionPieceType, _activePiece.SquarePosition, _activePiece.Team);
            Destroy(_activePiece.gameObject);
            _activePiece = promotionPiece;

            UpdateBoardOnPieceMove(_activePiece, _activePiece.SquarePosition, _activePiece.SquarePosition);
            EndTurn();
        }

        private void EndTurn()
        {
            DeselectPiece();
            GameManager.Instance.EndTurn();
        }

        private void TogglePiecesColliderOnAvailableMove(Dictionary<MoveInfo, PieceMoveType> moveDict, bool toggle)
        {
            foreach (KeyValuePair<MoveInfo, PieceMoveType> move in moveDict)
            {
                if (move.Value != PieceMoveType.Take && move.Value != PieceMoveType.Castle) continue;
                Piece piece = GetPieceOnBoardFromSquareCoords(move.Key.GridPosition);
                piece.ToggleCollider(toggle);
            }
        }
        
        private bool CheckForPawnPromotion(Piece piece, MoveInfo moveInfo)
        {
            return piece.GetType() == typeof(Pawn) && 
                   ((moveInfo.GridPosition.y == 0 && piece.Team == TeamColor.Black) || 
                                                       (moveInfo.GridPosition.y == BoardSize - 1 && piece.Team == TeamColor.White)) ?
                true : false;
        }

        public Vector3 CalculateBoardPositionFromSquarePosition(Vector2Int squarePos)
        {
            return bottomLeftSquare.position + new Vector3(squarePos.x * squareSize, 0f, squarePos.y * squareSize);
        }
        
        public void UpdateBoardOnPieceMove(Piece piece, Vector2Int newPos, Vector2Int oldPos)
        {
            _piecesGrid[oldPos.x, oldPos.y] = null;
            _piecesGrid[newPos.x, newPos.y] = piece;
        }

        public void SetPieceOnBoard(Piece piece, Vector2Int coords)
        {
            if (CheckIfCoordsAreOnBoard(coords))
            {
                _piecesGrid[coords.x, coords.y] = piece;
            }
        }

        public bool IsPieceOnBoard(Piece piece)
        {
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (_piecesGrid[i, j] == piece)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void OnPieceSelected(Piece piece)
        {
            if(GameManager.Instance.IsGameOver()) return;
            
            if (_activePiece == piece)
            {
                DeselectPiece();
                return;
            }

            if (piece.Team != GameManager.Instance.GetActiveTeamColorTurn()) return;

            _activePiece = piece;
            TogglePiecesColliderOnAvailableMove(_activePiece.MovesDict, false);
            _squareCreator.DisplayAvailableSquareMoves(_activePiece.MovesDict);
        }

        private void DeselectPiece()
        {
            TogglePiecesColliderOnAvailableMove(_activePiece.MovesDict, true);
            _activePiece = null;
            _squareCreator.ClearActiveMoveSquares();
        }

        public Piece GetPieceOnBoardFromSquareCoords(Vector2Int coords)
        {
            return CheckIfCoordsAreOnBoard(coords) ? _piecesGrid[coords.x, coords.y] : null;
        }

        public bool CheckIfCoordsAreOnBoard(Vector2Int coords)
        {
            return coords.x is >= 0 and < BoardSize && coords.y is >= 0 and < BoardSize;
        }

        public void OnSquareSelected(MoveInfo moveInfo)
        {
            if (moveInfo == null) return;
            Piece targetSquarePiece = GetPieceOnBoardFromSquareCoords(moveInfo.GridPosition);
            if (targetSquarePiece)
            {
                if (_activePiece.MovesDict.ContainsKey(moveInfo) && _activePiece.MovesDict[moveInfo] == PieceMoveType.Castle)
                {
                    Castle(targetSquarePiece, moveInfo);
                }
                else
                {
                    MoveActivePieceAndTake(targetSquarePiece, moveInfo);
                }
            }
            else
            {
                MoveActivePiece(moveInfo);
            }
        }

        public int GetBoardSize()
        {
            return BoardSize;
        }

        public void CreateCheckSquare(Vector2Int coord)
        {
            _squareCreator.CreateSquare(new MoveInfo(coord, CalculateBoardPositionFromSquarePosition(coord)), PieceMoveType.Check);
        }

        public void ClearActiveCheckSquares()
        {
            _squareCreator.ClearActiveCheckSquares();
        }
    }
}