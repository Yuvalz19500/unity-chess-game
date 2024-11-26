using System.Collections.Generic;
using Enums;
using JetBrains.Annotations;
using Pieces;
using UnityEngine;

namespace Move_Squares
{
    public class AvailableMoveSquareCreator : MonoBehaviour
    {
        [SerializeField] private GameObject squarePrefab;
        [SerializeField] private Material moveMaterial;
        [SerializeField] private Material takeMaterial;
        [SerializeField] private Material checkMaterial;
        [SerializeField] private Material castleMaterial;
        [SerializeField] private Transform moveSquaresParentObjectTransform;

        private readonly Dictionary<GameObject, MoveInfo> _activeMoveSquares = new Dictionary<GameObject, MoveInfo>();
        private readonly Dictionary<GameObject, MoveInfo> _activeCheckSquares = new Dictionary<GameObject, MoveInfo>();
        
        private void ApplyMaterials(MeshRenderer[] renderers, PieceMoveType moveType)
        {
            foreach (MeshRenderer mRenderer in renderers)
            {
                mRenderer.material = moveType switch
                {
                    PieceMoveType.Take => takeMaterial,
                    PieceMoveType.Move => moveMaterial,
                    PieceMoveType.Check => checkMaterial,
                    PieceMoveType.Castle => castleMaterial,
                    _ => null
                };
            }
        }

        private void ToggleCheckSquares([CanBeNull] Dictionary<MoveInfo, PieceMoveType> movesDict, bool toggle)
        {
            if (!toggle)
            {
                foreach (KeyValuePair<GameObject, MoveInfo> checkSquare in _activeCheckSquares)
                {
                    foreach (KeyValuePair<MoveInfo, PieceMoveType> moves in movesDict)
                    {
                        if (moves.Key.WorldPosition == checkSquare.Key.transform.position)
                        {
                            checkSquare.Key.SetActive(false);
                        }
                    }
                }   
            }
            else
            {
                foreach (KeyValuePair<GameObject, MoveInfo> checkSquare in _activeCheckSquares)
                {
                    checkSquare.Key.SetActive(true);
                }    
            }
        }
        
        public void ClearActiveMoveSquares()
        {
            foreach (KeyValuePair<GameObject, MoveInfo> square in _activeMoveSquares)
            {
                Destroy(square.Key);
            } 
            
            _activeMoveSquares.Clear();
            
            ToggleCheckSquares(null, true);
        }
        
        public void ClearActiveCheckSquares()
        {
            foreach (KeyValuePair<GameObject, MoveInfo> square in _activeCheckSquares)
            {
                Destroy(square.Key);
            } 
            
            _activeCheckSquares.Clear();
        }

        public void DisplayAvailableSquareMoves(Dictionary<MoveInfo, PieceMoveType> movesDict)
        {
            ClearActiveMoveSquares();
            ToggleCheckSquares(movesDict, false);
            
            foreach (KeyValuePair<MoveInfo, PieceMoveType> move in movesDict)
            {
                GameObject moveSquare = Instantiate(squarePrefab, moveSquaresParentObjectTransform, true);
                moveSquare.transform.position = move.Key.WorldPosition;
                MeshRenderer[] renderers = moveSquare.GetComponentsInChildren<MeshRenderer>();

                ApplyMaterials(renderers, move.Value);
                
                _activeMoveSquares.Add(moveSquare, move.Key);
            }
        }

        public MoveInfo GetMoveInfoForSelectedSquare(GameObject square)
        {
            return _activeMoveSquares.TryGetValue(square, out MoveInfo moveInfo) ? moveInfo : null;
        }

        public void CreateSquare(MoveInfo moveInfo, PieceMoveType moveType)
        {
            GameObject moveSquare = Instantiate(squarePrefab, moveSquaresParentObjectTransform, true);
            moveSquare.transform.position = moveInfo.WorldPosition;
            
            MeshRenderer[] renderers = moveSquare.GetComponentsInChildren<MeshRenderer>();
            ApplyMaterials(renderers, moveType);

            if (moveType == PieceMoveType.Check)
            {
                _activeCheckSquares.Add(moveSquare, moveInfo);
            }
            else
            {
                _activeMoveSquares.Add(moveSquare, moveInfo);
            }
        }
    }
}