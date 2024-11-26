using System;
using System.Collections.Generic;
using Enums;
using Pieces;
using UnityEngine;

namespace Core
{
    public class PieceCreator : MonoBehaviour
    {
        [SerializeField] private GameObject[] piecesPrefabs;
        [SerializeField] private Material blackMaterial;
        [SerializeField] private Material whiteMaterial;
        [SerializeField] private Transform piecesParentObjectTransform;

        private readonly Dictionary<string, GameObject> _piecesDict = new Dictionary<string, GameObject>();

        private void Awake()
        {
            foreach (GameObject piece in piecesPrefabs)
            {
                _piecesDict.Add(piece.GetComponent<Piece>().GetType().Name, piece);
            }
        }

        public Material GetTeamMaterial(TeamColor team)
        {
            return team == TeamColor.Black ? blackMaterial : whiteMaterial;
        }

        public GameObject CreatePiece(string pieceName)
        {
            GameObject prefab = _piecesDict[pieceName];
            return prefab ? Instantiate(prefab, piecesParentObjectTransform, true) : null;
        }
    }
}