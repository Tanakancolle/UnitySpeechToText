using System;
using UnityEngine;

namespace GoogleCloudSpeechApiController
{
    public class SpeechToTextController : MonoBehaviour
    {
        private const string RecognizeUrlFormat = "https://speech.googleapis.com/v1/speech:recognize?key={0}";

        [SerializeField]
        private AudioSource _audioSource;

        private SpeechRecognizeParameter _parameter;

        private Recorder _recorder;

        public void RecordStart()
        {
            _recorder = new Recorder();
            _recorder.Start(44100);
        }

        public void RecordEnd()
        {
            var dataList = _recorder.End();

            var clip = AudioClip.Create("Record Clip", dataList.Count, _recorder.RecordingClip.channels, _recorder.RecordingClip.frequency, false);
            clip.SetData(dataList.ToArray(), 0);
            _audioSource.clip = clip;
            _recorder = null;

            Debug.Log("Clip Length : " + clip.length);
        }

        public void PlayAudio()
        {
            _audioSource.Play();

            Debug.Log("Play Clip");
        }

        public void SendApi()
        {
        }
    }
}