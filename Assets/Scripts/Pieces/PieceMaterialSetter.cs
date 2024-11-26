using System;
using UnityEngine;

namespace Pieces
{
    [RequireComponent(typeof(MeshRenderer))]
    public class PieceMaterialSetter : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        public void SetParentMaterial(Material material)
        {
            _meshRenderer.material = material;
        }
    }
}