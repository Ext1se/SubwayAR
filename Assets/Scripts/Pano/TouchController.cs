//
// Fingers Gestures
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DigitalRubyShared
{
    public class TouchController : MonoBehaviour
    {
        public GameObject panoCamera;
        public GameObject panoViewer;
        public GameObject uiCanvas;

        public bool useGyroscope = false;
        public bool useMouse = true;

        private bool gyroscopeEnabled = false;
        private Gyroscope gyroscope;
        private Quaternion gyroRotation;

        private TapGestureRecognizer tapGesture;
        private TapGestureRecognizer doubleTapGesture;
        private PanGestureRecognizer panGesture;
        private ScaleGestureRecognizer scaleGesture;
        private LongPressGestureRecognizer longPressGesture;

        private float mouseSensitivity = 4;
        private float touchSensitivity = 600;
        private float touchSensitivityDefault = 54000;
        private float xAxisClamp = 0;
        private float minCameraScale = 25;
        private float maxCameraScale = 120;
        private float lastVelocityX = 0;
        private float lastVelocityY = 0;
        private float stepVelocity = 1.2f;
        private bool isGesturePossible = false;

        private void DebugText(string text, params object[] format)
        {
            Debug.Log(string.Format(text, format));
        }

        private void TapGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                DebugText("Tapped at {0}, {1}", gesture.FocusX, gesture.FocusY);
                ShowUI();
            }
        }

        private bool EnableGyroscope()
        {
            if (SystemInfo.supportsGyroscope)
            {
                gyroscope = Input.gyro;
                gyroscope.enabled = true;

                gyroRotation = new Quaternion(0, 0, 1, 0);
                return true;
            }
            return false;
        }

        private void CreateTapGesture()
        {
            tapGesture = new TapGestureRecognizer();
            tapGesture.StateUpdated += TapGestureCallback;
            tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;
            FingersScript.Instance.AddGesture(tapGesture);
        }

        private void DoubleTapGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                DebugText("Double tapped at {0}, {1}", gesture.FocusX, gesture.FocusY);
                ShowUI();
            }
        }

        private void CreateDoubleTapGesture()
        {
            doubleTapGesture = new TapGestureRecognizer();
            doubleTapGesture.NumberOfTapsRequired = 2;
            doubleTapGesture.StateUpdated += DoubleTapGestureCallback;
            FingersScript.Instance.AddGesture(doubleTapGesture);
        }

        private void PanGestureCallback(GestureRecognizer gesture)
        {
            DebugText("Panned, Location: {0}, {1}, Delta: {2}, {3}", gesture.VelocityX, gesture.VelocityY, gesture.DeltaX, gesture.DeltaY);

            if (panGesture.VelocityX == 0 && panGesture.VelocityY == 0)
            {
                isGesturePossible = true;
                return;
            }

            isGesturePossible = false;

            float rotationAmountX = panGesture.VelocityX / touchSensitivity;
            float rotationAmountY = panGesture.VelocityY / touchSensitivity;


            lastVelocityX = panGesture.VelocityX;
            lastVelocityY = panGesture.VelocityY;
            Rotate(rotationAmountX, rotationAmountY, true);

            if (gesture.State == GestureRecognizerState.Possible)
            {
                isGesturePossible = true;
            }
        }

        private void CreatePanGesture()
        {
            panGesture = new PanGestureRecognizer();
            panGesture.MinimumNumberOfTouchesToTrack = 1;
            panGesture.StateUpdated += PanGestureCallback;
            FingersScript.Instance.AddGesture(panGesture);
        }

        private void ScaleGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Executing)
            {
                DebugText("Scaled: {0}, Focus: {1}, {2}", scaleGesture.ScaleMultiplier, scaleGesture.FocusX, scaleGesture.FocusY);
                Camera cam = panoCamera.GetComponent<Camera>();
                float newScale = cam.fieldOfView / scaleGesture.ScaleMultiplier;
                if (newScale > minCameraScale && newScale < maxCameraScale)
                {
                    touchSensitivity = touchSensitivityDefault / newScale;
                    cam.fieldOfView = newScale;
                }
            }
        }

        private void CreateScaleGesture()
        {
            scaleGesture = new ScaleGestureRecognizer();
            scaleGesture.StateUpdated += ScaleGestureCallback;
            FingersScript.Instance.AddGesture(scaleGesture);
        }

        private void LongPressGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Began)
            {

            }
            else if (gesture.State == GestureRecognizerState.Executing)
            {

            }
            else if (gesture.State == GestureRecognizerState.Ended)
            {

            }
        }

        private void CreateLongPressGesture()
        {
            longPressGesture = new LongPressGestureRecognizer();
            longPressGesture.MaximumNumberOfTouchesToTrack = 1;
            longPressGesture.StateUpdated += LongPressGestureCallback;
            FingersScript.Instance.AddGesture(longPressGesture);
        }

        private void Start()
        {
            gyroscopeEnabled = EnableGyroscope();

            CreateDoubleTapGesture();
            CreateTapGesture();
            CreatePanGesture();
            CreateScaleGesture();
            CreateLongPressGesture();

            Camera cam = panoCamera.GetComponent<Camera>();
            touchSensitivity = touchSensitivityDefault / cam.fieldOfView;
        }

        private void Update()
        {
            if (useMouse)
            {
                //Cursor.lockState = CursorLockMode.Locked;

                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                float rotationAmountX = mouseX * mouseSensitivity;
                float rotationAmountY = mouseY * mouseSensitivity;

                Rotate(rotationAmountX, rotationAmountY);
            }

            if (isGesturePossible && (Mathf.Abs(lastVelocityX) > 1 || Mathf.Abs(lastVelocityY) > 1))
            {
                lastVelocityX /= stepVelocity;
                lastVelocityY /= stepVelocity;

                float rotationAmountX = lastVelocityX / touchSensitivity;
                float rotationAmountY = lastVelocityY / touchSensitivity;

                Rotate(rotationAmountX, rotationAmountY, true);
            }

            if (gyroscopeEnabled && useGyroscope)
            {
                panoCamera.transform.localRotation = gyroscope.attitude * gyroRotation;
                //panoCamera.transform.rotation = gyroscope.attitude * gyroRotation;
                //panoCamera.transform.localRotation = new Quaternion(gyroscope.attitude.x, gyroscope.attitude.y, -gyroscope.attitude.z, -gyroscope.attitude.w);
            }
        }

        private void ShowUI()
        {
            uiCanvas.SetActive(!uiCanvas.activeSelf);
        }

        private void Rotate(float rotationAmountX, float rotationAmountY, bool isInverseAxis = false)
        {
            int inverseAxis = isInverseAxis ? -1 : 1;
            xAxisClamp -= (rotationAmountY * inverseAxis);

            Vector3 rotationPanoViewer = panoViewer.transform.rotation.eulerAngles;
            Vector3 rotationCamera = panoCamera.transform.rotation.eulerAngles;

            rotationCamera.x -= (rotationAmountY * inverseAxis);
            rotationCamera.z = 0;
            rotationPanoViewer.y += (rotationAmountX * inverseAxis);

            if (xAxisClamp > 90)
            {
                xAxisClamp = 90;
                rotationCamera.x = 90;
            }
            else if (xAxisClamp < -90)
            {
                xAxisClamp = -90;
                rotationCamera.x = 270;
            }

            panoCamera.transform.rotation = Quaternion.Euler(rotationCamera);
            panoViewer.transform.rotation = Quaternion.Euler(rotationPanoViewer);
        }

        public void UseGyroscope(bool enabled)
        {
            if (!gyroscopeEnabled)
            {
                return;
            }
            else
            {
                useGyroscope = enabled;
                if (enabled)
                {
                    panoViewer.transform.rotation = Quaternion.Euler(90f, 0f, 0);
                }
                else
                {
                    panoViewer.transform.rotation = Quaternion.Euler(0, 0, 0);
                    panoCamera.transform.rotation = Quaternion.Euler(panoCamera.transform.rotation.x, 0, 0);
                }
            }
        }

        public void UseGyroscope()
        {
            useGyroscope = !useGyroscope;
            UseGyroscope(useGyroscope);
        }
    }
}
