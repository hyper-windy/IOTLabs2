using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Gauge : MonoBehaviour
{
    [SerializeField]
    private RectTransform a;

    public void ClickButton()
    {
        if(DOTween.IsTweening(a)) return;
        a.DORotate(a.eulerAngles + new Vector3(0, 0, 90), 0.5f);
    }

}