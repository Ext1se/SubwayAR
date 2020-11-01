using easyar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CityBacketMapBuilder : MonoBehaviour
{
    public Text Status;
    public GameObject Ball;
    public int MaxBallCount = 20;
    public float BallLifetime = 15;
    public ARSession Session;
    public TouchController TouchControl;

    private VIOCameraDeviceUnion vioCamera;
    private DenseSpatialMapBuilderFrameFilter dense;
    private bool canMove = true;
    private List<GameObject> balls = new List<GameObject>();

    private void Awake()
    {
        vioCamera = Session.GetComponentInChildren<VIOCameraDeviceUnion>();
        dense = Session.GetComponentInChildren<DenseSpatialMapBuilderFrameFilter>();
        TouchControl.TurnOn(TouchControl.gameObject.transform, Camera.main, false, false, false, false);
    }

    private void Update()
    {
        if (Status.IsActive())
        {
            Status.text = "VIO Device Type: " + (vioCamera.Device == null ? "-" : vioCamera.Device.DeviceType.ToString()) + Environment.NewLine +
                "Tracking Status: " + (Session.WorldRootController == null ? "-" : Session.WorldRootController.TrackingStatus.ToString()) + Environment.NewLine +
                "Dense Mesh Block Count: " + dense.MeshBlocks.Count + Environment.NewLine +
                Environment.NewLine;
        }

        if (!canMove)
        {
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
                var launchPoint = Camera.main.transform;
                var ball = Instantiate(Ball, launchPoint.position, launchPoint.rotation);
                var rigid = ball.GetComponent<Rigidbody>();
                rigid.velocity = Vector3.zero;
                rigid.AddForce(ray.direction * 35f + Vector3.up * 15f);
                if (balls.Count > 0 && balls.Count == MaxBallCount)
                {
                    Destroy(balls[0]);
                    balls.RemoveAt(0);
                }
                balls.Add(ball);
                StartCoroutine(Kill(ball, BallLifetime));
            }
            return;
        }

        if (Input.touchCount == 1)
        {
            if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && Input.touches[0].phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    /*if (currentDragonArea != null)
                    {
                        Destroy(currentDragonArea);
                    }
                    currentDragonArea = Instantiate(DragonArea, hitInfo.point, Quaternion.Euler(0,0,0));
                    currentDragonArea.transform.parent = TouchControl.transform;*/
                    TouchControl.transform.position = hitInfo.point;
                };
            }
        }
    }

    public void RotateModel(float value)
    {
        TouchControl.transform.rotation = Quaternion.Euler(
            TouchControl.transform.rotation.x,
            value,
            TouchControl.transform.rotation.z
        );
        //TouchControl.transform.RotateAround(Vector3.up, value);
    }

    public void ScaleModel(float value)
    {
        TouchControl.transform.localScale = new Vector3(value, value, value);
    }

    public void ShowMap(bool show)
    {
        if (!dense)
        {
            return;
        }
        dense.RenderMesh = show;
    }

    public void ShowMap()
    {
        ShowMap(!dense.RenderMesh);
    }

    public void EnableMove(bool move)
    {
        canMove = move;
    }

    private IEnumerator Kill(GameObject ball, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        if (balls.Remove(ball)) { Destroy(ball); }
    }
}