using UnityEngine;

namespace Pieces.Movement
{
    public interface IPieceMover
    {
        public void MoveTo(Transform originTransform, Vector3 targetPos);
    }
}