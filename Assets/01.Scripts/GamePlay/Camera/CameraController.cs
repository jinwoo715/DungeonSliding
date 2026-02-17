using UnityEngine;

namespace JW.DungeonSliding
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        [SerializeField] private float _baseX;
        [SerializeField] private float _basyY;
        [SerializeField] private float _basyZ;

        [SerializeField] private float _offsetX;
        [SerializeField] private float _offsetY;

        public void SetCamera(int width, int height)
        {
            int offset = width - 5;

            _camera.transform.position = new Vector3(_baseX + _offsetX * offset, _basyY + _offsetY * offset, _basyZ);
        }
    }
}
