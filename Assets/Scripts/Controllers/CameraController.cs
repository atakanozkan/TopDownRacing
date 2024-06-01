using UnityEngine;

namespace TDR.Controllers
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Transform playerTransform;
        public Vector3 offset;

        private void LateUpdate()
        {
            if(playerTransform == null)
            {
                return;
            }
            Vector3 tempPos = playerTransform.position + offset;

            transform.position = new Vector3(transform.position.x, tempPos.y, tempPos.z);
        }
    }

}
