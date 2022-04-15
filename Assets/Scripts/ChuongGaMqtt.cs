using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;

namespace ChuongGa
{
    //public class Status_Data
    //{
    //    public string project_id { get; set; }
    //    public string project_name { get; set; }
    //    public string station_id { get; set; }
    //    public string station_name { get; set; }
    //    public string longitude { get; set; }
    //    public string latitude { get; set; }
    //    public string volt_battery { get; set; }
    //    public string volt_solar { get; set; }
    //    public List<data_ss> data_ss { get; set; }
    //    public string device_status { get; set; }
    //}

    public class Status_Data
    {
        public float temperature { get; set; }
        public float humidity { get; set; }
        public string light { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
    }

    public class data_ss
    {
        public string ss_name { get; set; }
        public string ss_unit { get; set; }
        public string ss_value { get; set; }
    }

    public class Config_Data
    {
        public float temperature_max { get; set; }
        public float temperature_min { get; set; }
        public int mode_fan_auto { get; set; }
    }

    public class ControlFan_Data
    {
        public int fan_status { get; set; }
        public int device_status { get; set; }

    }

    public class Button_Data
    {
        public string device { get; set; }
        public string status { get; set; }
    }

    public class ChuongGaMqtt : M2MqttUnityClient
    {
        public List<string> topics = new List<string>();


        public string msg_received_from_topic_status = "";
        public string msg_received_from_topic_control = "";

        [SerializeField]
        private InputField brokerURI;
        [SerializeField]
        private InputField usrname;
        [SerializeField]
        private InputField pw;

        [SerializeField]
        private Text errorText;


        private List<string> eventMessages = new List<string>();
        [SerializeField]
        public Status_Data _status_data;
        [SerializeField]
        public Config_Data _config_data;
        [SerializeField]
        public ControlFan_Data _controlFan_data;





        public void PublishConfig()
        {
            _config_data = new Config_Data();
            GetComponent<ChuongGaManager>().Update_Config_Value(_config_data);
            string msg_config = JsonConvert.SerializeObject(_config_data);
            client.Publish(topics[1], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("publish config");
        }

        public void PublishFan()
        {
            _controlFan_data = GetComponent<ChuongGaManager>().Update_ControlFan_Value(_controlFan_data);
            string msg_config = JsonConvert.SerializeObject(_controlFan_data);
            client.Publish(topics[2], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("publish fan");
        }

        Button_Data _button_data = new Button_Data();
        public void PublishButtonData(string device_name, string value)
        {
            _button_data.device = device_name;
            _button_data.status = value;
            string msg_config = JsonConvert.SerializeObject(_button_data);
            if (device_name == "LED")
                client.Publish(topics[1], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            else
                client.Publish(topics[2], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("publish " + msg_config);
        }

        public void SetEncrypted(bool isEncrypted)
        {
            this.isEncrypted = isEncrypted;
        }


        protected override void OnConnecting()
        {
            base.OnConnecting();
            //SetUiMessage("Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            GetComponent<ChuongGaManager>().SwitchLayer();

            //SubscribeTopics();
        }

        // Connect here
        public override void Connect()
        {
            //mqttUserName = "bkiot";
            //mqttPassword = "12345678";
            //brokerAddress = "mqttserver.tk";
            mqttUserName = usrname.text;
            mqttPassword = pw.text;
            brokerAddress = brokerURI.text;
            base.Connect();

            //SubscribeTopics();
        }

        protected override void SubscribeTopics()
        {
            foreach (string topic in topics)
            {
                Debug.Log((string)topic);
                if (topic != "")
                {
                    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

                }
            }
        }

        protected override void UnsubscribeTopics()
        {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Unsubscribe(new string[] { topic });
                }
            }

        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            errorText.text = errorMessage;
            Debug.Log("CONNECTION FAILED! " + errorMessage);
        }

        protected override void OnDisconnected()
        {
            Debug.Log("Disconnected.");
        }

        protected override void OnConnectionLost()
        {
            //GetComponent<ChuongGaManager>().SwitchLayer();
            //errorText.text = "CONNECTION LOST!";
            Debug.Log("CONNECTION LOST!");
        }



        protected override void Start()
        {

            base.Start();
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            Debug.Log(String.Format("Received: topic {0}; msg: {1}", topic,msg));
            //StoreMessage(msg);
            if (topic == topics[0])
                ProcessMessageStatus(msg);
            else
                ChangeButtonValue(msg);
        }

        private void ProcessMessageStatus(string msg)
        {
            Status_Data _status_data = JsonConvert.DeserializeObject<Status_Data>(msg);
            msg_received_from_topic_status = msg;
            GetComponent<ChuongGaManager>().Update_Status(_status_data);

        }

        private void ChangeButtonValue(string msg)
        {
            Button_Data btn_data = JsonConvert.DeserializeObject<Button_Data>(msg);
            msg_received_from_topic_control = msg;
            GetComponent<ChuongGaManager>().Update_Button(btn_data);

        }

        //private void ProcessMessageControl(string msg)
        //{
        //    _controlFan_data = JsonConvert.DeserializeObject<ControlFan_Data>(msg);
        //    msg_received_from_topic_control = msg;
        //    GetComponent<ChuongGaManager>().Update_Control(_controlFan_data);

        //}

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnValidate()
        {
            //if (autoTest)
            //{
            //    autoConnect = true;
            //}
        }

        public void UpdateConfig()
        {
           
        }

        public void UpdateControl()
        {

        }
    }
}