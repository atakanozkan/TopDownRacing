using UnityEngine;

namespace TDR.Controllers
{
    public class WheelController : MonoBehaviour
    {
        [SerializeField]
        private Transform frontLeftWheel;
        [SerializeField]
        private Transform frontRightWheel;
        [SerializeField]
        private Transform rearLeftWheel;
        [SerializeField]
        private Transform rearRightWheel;

        public float forwardSpeed;
        public float rotationSpeed = 360f;

        private void Update()
        {
            RotateWheels();
        }

        private void RotateWheels()
        {
            float rotationAmount = forwardSpeed * rotationSpeed * Time.deltaTime;
            frontLeftWheel.Rotate(Vector3.right, rotationAmount);
            frontRightWheel.Rotate(Vector3.right, rotationAmount);
            rearLeftWheel.Rotate(Vector3.right, rotationAmount);
            rearRightWheel.Rotate(Vector3.right, rotationAmount);
        }

        public void SteerWheels(float steerAngle)
        {
            steerAngle = Mathf.Clamp(steerAngle, -15f, 15f);

            frontLeftWheel.localEulerAngles = new Vector3(frontLeftWheel.localEulerAngles.x, steerAngle, frontLeftWheel.localEulerAngles.z);
            frontRightWheel.localEulerAngles = new Vector3(frontRightWheel.localEulerAngles.x, steerAngle, frontRightWheel.localEulerAngles.z);
        }
    }

}
