using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OdinNative.Unity.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace OdinNative.Unity.Samples
{
    public class SimplePushToTalk : MonoBehaviour
    {
        public string RoomName;
        [SerializeField]
        public KeyCode PushToTalkHotkey;
        public bool UsePushToTalk = true;
        public MicrophoneReader AudioSender;

        public CanvasGroup microphoneCanvasGroup;
        public Image microphoneIcon;
        public bool showHUD;
        public bool blockMicrophone;

        private void Reset()
        {
            RoomName = "default";
            PushToTalkHotkey = KeyCode.C;
            UsePushToTalk = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            if (AudioSender == null)
                AudioSender = FindObjectOfType<MicrophoneReader>();

            //OdinHandler.Instance.JoinRoom(RoomName);
        }

        // Update is called once per frame
        void Update()
        {
            if (AudioSender)
                AudioSender.SilenceCapturedAudio = !(UsePushToTalk ? Input.GetKey(PushToTalkHotkey) : true) || blockMicrophone;

            microphoneCanvasGroup.alpha = showHUD ? 1 : 0;
            microphoneIcon.color = AudioSender.SilenceCapturedAudio ? Color.grey : Color.green;
        }
    }
}