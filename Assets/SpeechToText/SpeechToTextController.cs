﻿using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SpeechToText
{
    public class SpeechToTextController : MonoBehaviour
    {
        private const string RecognizeUrlFormat = "https://speech.googleapis.com/v1/speech:recognize?key={0}";

        [SerializeField]
        private string _apiKey;

        [SerializeField]
        private AudioSource _audioSource;

        private SpeechRecognizeParameter _parameter = new SpeechRecognizeParameter();

        private AudioRecorder _audioRecorder;

        private float[] _recodeData;

        public void RecordStart()
        {
            _audioRecorder = new AudioRecorder();
            _audioRecorder.Start(16000);
        }

        public void RecordEnd()
        {
            var data = _audioRecorder.End();
            var clip = AudioClip.Create("Record Clip", data.Length, _audioRecorder.RecordingClip.channels, _audioRecorder.RecordingClip.frequency, false);
            clip.SetData(data, 0);
            _audioSource.clip = clip;
            Debug.Log("Clip Length : " + clip.length);

            SettingParameter(clip);
            SendApi();
        }

        public void PlayAudio()
        {
            _audioSource.Play();

            Debug.Log("Play Clip");
        }

        private void SettingParameter(AudioClip clip)
        {
            _parameter.config.SetLanguageCode(LanguageCodeType.ja_JP);
            _parameter.config.sampleRateHertz = clip.frequency;
            _parameter.config.audioChannelCount = clip.channels;
            var bytes = AudioConverter.CreateLinear16(clip);
            _parameter.audio.content = Convert.ToBase64String(bytes);
        }

        private void SendApi()
        {
            StartCoroutine(DoSendApi());
        }

        private IEnumerator DoSendApi()
        {
            var request = new UnityWebRequest(string.Format(RecognizeUrlFormat, _apiKey), UnityWebRequest.kHttpVerbPOST);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonUtility.ToJson(_parameter)));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.Send();

            Debug.Log("Result : " + request.downloadHandler.text);
        }
    }
}