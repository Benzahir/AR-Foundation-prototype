
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(ARRaycastManager))]
public class MeasurementController : MonoBehaviour
{
    [SerializeField]
    private GameObject measurementPointPrefab;

    string guiText = "";

    [SerializeField]
    private ARCameraManager arCameraManager;

    private LineRenderer measureLine;

    private ARRaycastManager arRaycastManager;

    private GameObject startPoint;

    private GameObject endPoint;

    private Vector2 touchPosition = default;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
 
    void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        startPoint = Instantiate(measurementPointPrefab, Vector3.zero, Quaternion.identity);
        endPoint = Instantiate(measurementPointPrefab, Vector3.zero, Quaternion.identity);
        measureLine = GetComponent<LineRenderer>();
        startPoint.SetActive(false);
        endPoint.SetActive(false);
    }

    private void OnEnable()
    {
        if (measurementPointPrefab == null)
        {
            enabled = false;
        }
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchPosition = touch.position;
                //Set the start point
                if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    startPoint.SetActive(true);
                    Pose hitPose = hits[0].pose;
                    startPoint.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
                }
            }

            if (touch.phase == TouchPhase.Moved)
            {
                touchPosition = touch.position;
                //Set the endPoint
                if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    measureLine.enabled = true;
                    endPoint.SetActive(true);
                    Pose hitPose = hits[0].pose;
                    endPoint.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
                }
            }
        }

        //Set measure line and calculate the distance
        if (startPoint.activeSelf && endPoint.activeSelf)
        {
            measureLine.SetPosition(0, startPoint.transform.position);
            measureLine.SetPosition(1, endPoint.transform.position);
            guiText = $"Distance: {((Vector3.Distance(startPoint.transform.position, endPoint.transform.position)) * 100).ToString("F2")}";
        }
    }

    //Display distance text on screen
    void OnGUI()
    {
        GUIStyle localStyle = new GUIStyle();
        localStyle.normal.textColor = Color.white;
        localStyle.fontSize = 70;
        GUI.Label(new Rect(90, 200, Screen.width - 20, 30), guiText + " cm ", localStyle);
    }
  
    public void ResetScene()
    {
        Debug.Log("ResetScene() called.");
        //// reset augmentations
        this.startPoint.transform.position = Vector3.zero;
        this.startPoint.transform.localEulerAngles = Vector3.zero;
        this.startPoint.SetActive(false);

        this.endPoint.transform.position = Vector3.zero;
        this.endPoint.transform.localEulerAngles = Vector3.zero;
        this.endPoint.SetActive(false);

        this.measureLine.transform.position = Vector3.zero;
        this.measureLine.transform.localEulerAngles = Vector3.zero;
        if (this.measureLine.enabled)
        {
            this.measureLine.enabled = false;
          
        }
        guiText = $"Distance: {0:F2}";

    }
}

