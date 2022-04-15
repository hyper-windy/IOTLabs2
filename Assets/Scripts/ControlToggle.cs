using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace ChuongGa {
    public class ControlToggle : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField]
        private bool isOn = false;
        public bool IsOn { get { return isOn; } }

        [SerializeField]
        private string buttonName;

        [SerializeField]
        private RectTransform toggleIndicator;

        [SerializeField]
        private Image backgroundImg;

        [SerializeField]
        private Color onColor;
        [SerializeField]
        private Color offColor;
        [SerializeField]
        private Color waitingColor;
        private float offX;
        private float onX;
        [SerializeField]
        private float tweenTime = 0.25f;

        [SerializeField]
        private ChuongGaMqtt mqtt;

        [SerializeField]
        private CanvasGroup canvas;

        public delegate void ValueChanged(bool value);
        public event ValueChanged OnValueChanged;

        // Start is called before the first frame update
        void Start()
        {
            offX = toggleIndicator.anchoredPosition.x;
            onX = -offX;
        }

        private void Toggle(bool value)
        {
            if (value != isOn)
            {
                isOn = value;

                ToggleColor(isOn);
                MoveIndicator(isOn);

                if (OnValueChanged != null)
                {
                    OnValueChanged(isOn);
                }
            }
        }

        private void ToggleColor(bool value)
        {
            if (value)
                backgroundImg.DOColor(onColor, tweenTime);
            else
                backgroundImg.DOColor(offColor, tweenTime);
        }

        private void MoveIndicator(bool value)
        {
            if (value)
                toggleIndicator.DOAnchorPosX(onX, tweenTime);
            else
                toggleIndicator.DOAnchorPosX(offX, tweenTime);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            canvas.interactable = false;
            canvas.blocksRaycasts = false;
            backgroundImg.color = waitingColor;

            if (isOn)
                mqtt.PublishButtonData(buttonName, "OFF");
            else
                mqtt.PublishButtonData(buttonName, "ON");

            //Toggle(!isOn);
        }

        public void Recived(bool value)
        {
            Toggle(value);

            canvas.interactable = true;
            canvas.blocksRaycasts = true;
        }

        // Update is called once per frame
        //void Update()
        //{

        //}
    }
}
