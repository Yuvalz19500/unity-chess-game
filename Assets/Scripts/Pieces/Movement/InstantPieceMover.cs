using UnityEngine;

namespace Pieces.Movement
{
    public class InstantPieceMover : MonoBehaviour, IPieceMover
    {
        public void MoveTo(Transform originTransform, Vector3 targetPos)
        {
            transform.position = targetPos;
        }
    }
}