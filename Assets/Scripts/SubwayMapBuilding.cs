using easyar;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SubwayMapBuilding : MonoBehaviour
{
    public MenuController MenuController;
    public Text Status;
    public ARSession Session;
    public TouchController TouchControl;
    private VIOCameraDeviceUnion vioCamera;
    private DenseSpatialMapBuilderFrameFilter dense;
    private bool canMove = true;

    private void Awake()
    {
        vioCamera = Session.GetComponentInChildren<VIOCameraDeviceUnion>();
        dense = Session.GetComponentInChildren<DenseSpatialMapBuilderFrameFilter>();
        TouchControl.TurnOn(TouchControl.gameObject.transform, Camera.main, false, false, false, false);
    }

    private void Update()
    {
        if (!MenuController.UseAr)
        {
            return;
        }

        if (Status.IsActive())
        {
            Status.text = "VIO Device Type: " + (vioCamera.Device == null ? "-" : vioCamera.Device.DeviceType.ToString()) + Environment.NewLine +
                "Tracking Status: " + (Session.WorldRootController == null ? "-" : Session.WorldRootController.TrackingStatus.ToString()) + Environment.NewLine +
                "Dense Mesh Block Count: " + dense.MeshBlocks.Count + Environment.NewLine +
                Environment.NewLine;
        }

        if (!canMove)
        {
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
}