/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Speedometer : MonoBehaviour {

    public float MAX_SPEED_ANGLE = 0;
    public float ZERO_SPEED_ANGLE = 180;

    public float speedMax;
    public float speedMin;
    public Transform needleTranform;
    public Transform speedLabelTemplateTransform;

    private void Awake() {
        needleTranform = transform.Find("needle");
        speedLabelTemplateTransform = transform.Find("speedLabelTemplate");
        speedLabelTemplateTransform.gameObject.SetActive(false);

        // speedMax = 100f;

        CreateSpeedLabels();
    }

    // private void Update() {
    //     // HandlePlayerInput();

    //     //speed += 30f * Time.deltaTime;
    //     //if (speed > speedMax) speed = speedMax;

    //     // needleTranform.eulerAngles = new Vector3(0, 0, GetSpeedRotation());
    //     needleTranform.DORotate(new Vector3(0, 0, GetSpeedRotation()), 0.5f);

        
    // }

    public void Received(float speed)
    {
        needleTranform.DORotate(new Vector3(0, 0, GetSpeedRotation(speed)), 0.5f);
    }


    // private void HandlePlayerInput() {
    //     if (Input.GetKey(KeyCode.UpArrow)) {
    //         float acceleration = 80f;
    //         speed += acceleration * Time.deltaTime;
    //     } else {
    //         float deceleration = 20f;
    //         speed -= deceleration * Time.deltaTime;
    //     }

    //     if (Input.GetKey(KeyCode.DownArrow)) {
    //         float brakeSpeed = 100f;
    //         speed -= brakeSpeed * Time.deltaTime;
    //     }

    //     speed = Mathf.Clamp(speed, 0f, speedMax);
    // }

    private void CreateSpeedLabels() {
        int labelAmount = 10;
        float totalAngleSize = ZERO_SPEED_ANGLE - MAX_SPEED_ANGLE;

        for (int i = 0; i <= labelAmount; i++) {
            Transform speedLabelTransform = Instantiate(speedLabelTemplateTransform, transform);
            float labelSpeedNormalized = (float)i / labelAmount;
            float speedLabelAngle = ZERO_SPEED_ANGLE - labelSpeedNormalized * totalAngleSize;
            speedLabelTransform.eulerAngles = new Vector3(0, 0, speedLabelAngle);
            speedLabelTransform.Find("speedText").GetComponent<Text>().text = Mathf.RoundToInt(labelSpeedNormalized * speedMax+speedMin).ToString();
            speedLabelTransform.Find("speedText").eulerAngles = Vector3.zero;
            speedLabelTransform.gameObject.SetActive(true);
        }

        needleTranform.SetAsLastSibling();
    }

    private float GetSpeedRotation(float speed) {
        float totalAngleSize = ZERO_SPEED_ANGLE - MAX_SPEED_ANGLE;

        float speedNormalized = (speed-speedMin) / speedMax;
        
        return ZERO_SPEED_ANGLE - speedNormalized * totalAngleSize;
    }

    // [SerializeField]
    // private RectTransform a;

    // public void ClickButton()
    // {
    //     float newSpeed = speed+10;
    //     if(DOTween.IsTweening(a)) return;
    //     a.DORotate(a.eulerAngles + new Vector3(0, 0, GetSpeedRotation(newSpeed)), 0.5f);
    // }

}
