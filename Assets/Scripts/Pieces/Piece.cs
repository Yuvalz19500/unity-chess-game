using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Enums;
using Pieces.Movement;
using UnityEngine;

namespace Pieces
{
    [RequireComponent(typeof(PieceMaterialSetter))]
    [RequireComponent(typeof(IPieceMover))]
    public abstract class Piece : MonoBehaviour
    {
        private PieceMaterialSetter _pieceMaterialSetter;
        private IPieceMover _pieceMover;
        private Collider _collider;
        protected Board Board { get; private set; }
        public bool HasMoved { get; private set; } = false;
        public TeamColor Team { get; private set; }
        public Dictionary<MoveInfo, PieceMoveType> MovesDict { get; } = new Dictionary<MoveInfo, PieceMoveType>();
        public Vector2Int SquarePosition { get; private set; }
        public bool Disabled { get; set; }

        public abstract void SetAvailableMoves();
        private void Awake()
        {
            SetupDependencies();
        }

        private void SetupDependencies()
        {
            _pieceMaterialSetter = GetComponent<PieceMaterialSetter>();
            _pieceMover = GetComponent<IPieceMover>();
            _collider = GetComponent<MeshCollider>();
        }
 
        private void OnMouseDown()
        {
            Board.OnPieceSelected(this);
        }

        public void SetPieceMaterial(Material pieceMaterial)
        {
            _pieceMaterialSetter.SetParentMaterial(pieceMaterial);
        }

        public void SetPieceData(Vector2Int squarePos, TeamColor team, Board board)
        {
            SquarePosition = squarePos;
            Team = team;
            Board = board;

            gameObject.transform.position = board.CalculateBoardPositionFromSquarePosition(SquarePosition);
        }

        public bool IsPieceFromSameTeam(Piece piece)
        {
            return Team == piece.Team;
        }

        public void MovePiece(MoveInfo moveInfo)
        {
            SquarePosition = moveInfo.GridPosition;
            _pieceMover.MoveTo(transform, moveInfo.WorldPosition);
            HasMoved = true;
        }

        public void ToggleCollider(bool isOn)
        {
            _collider.enabled = isOn;
        }

        public bool IsAttackingPieceOfType<T>() where T : Piece
        {
            return !Disabled && MovesDict.Any(move => Board.GetPieceOnBoardFromSquareCoords(move.Key.GridPosition) is T);
        }
    }
    
    public class MoveInfo
    {
        public MoveInfo(Vector2Int gridPosition, Vector3 worldPosition)
        {
            this.GridPosition = gridPosition;
            this.WorldPosition = worldPosition;
        }

        public Vector2Int GridPosition { get; }
        public Vector3 WorldPosition { get; }
    }
}