using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SpeechToText
{
    public class SpeechToTextController : MonoBehaviour
    {
        public static readonly string ApiKeySaveWord = "SpeechToText.GoogleApiKey";

        private const string RecognizeUrlFormat = "https://speech.googleapis.com/v1/speech:recognize?key={0}";
        private static readonly string SaveRecordAudioExtension = ".wav";

        private string _apiKey;

        [SerializeField]
        private LanguageCodeType _code = LanguageCodeType.ja_JP;

        [SerializeField]
        private int _recordFrequency = 44100;

        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private string _saveRecordAudioPath;

        [SerializeField]
        private SpeechRecognizeParameter _parameter = new SpeechRecognizeParameter();

        private AudioRecorder _audioRecorder;

        private float[] _recodeData;

        public AudioClip CurrentRecordClip { get; private set; }

        public event Action<SpeechRecognitionResultParameter> OnRecognitionResult;

        private void Awake()
        {
            _apiKey = PlayerPrefs.GetString(ApiKeySaveWord);
            if (string.IsNullOrEmpty(_apiKey))
            {
                Debug.LogWarning("Set Google Api Key\n`Tools/SpeechToText/Setting Window`");
            }

            _audioRecorder = new AudioRecorder();
        }

        public void RecordStart()
        {
            _audioRecorder.Start(_recordFrequency);
        }

        public void RecordEnd()
        {
            var data = _audioRecorder.End();
            CurrentRecordClip = AudioClip.Create("Record Clip", data.Length, _audioRecorder.RecordingClip.channels, _audioRecorder.RecordingClip.frequency, false);
            CurrentRecordClip.SetData(data, 0);
            Debug.Log("Clip Length : " + CurrentRecordClip.length);

            if (string.IsNullOrEmpty(_saveRecordAudioPath) == false)
            {
                if (_saveRecordAudioPath.EndsWith(SaveRecordAudioExtension, true, CultureInfo.CurrentCulture) == false)
                {
                    _saveRecordAudioPath += SaveRecordAudioExtension;
                }

                File.WriteAllBytes(_saveRecordAudioPath, AudioConverter.CreateLinear16(CurrentRecordClip));
                Debug.Log("Save Audio : " + _saveRecordAudioPath);
            }
        }

        public void PlayAudio()
        {
            if (_audioSource == null)
            {
                return;
            }

            _audioSource.clip = CurrentRecordClip;
            _audioSource.Play();
            Debug.Log("Play Clip");
        }

        public void SendApi()
        {
            SettingParameter(CurrentRecordClip);
            StartCoroutine(DoSendApi());
        }

        private void SettingParameter(AudioClip clip)
        {
            _parameter.config.SetLanguageCode(_code);
            _parameter.config.sampleRateHertz = clip.frequency;
            _parameter.config.audioChannelCount = clip.channels;
            var bytes = AudioConverter.CreateLinear16(clip);
            _parameter.audio.content = Convert.ToBase64String(bytes);
        }

        private IEnumerator DoSendApi()
        {
            var request = new UnityWebRequest(string.Format(RecognizeUrlFormat, _apiKey), UnityWebRequest.kHttpVerbPOST);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonUtility.ToJson(_parameter)));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            Debug.Log("Result : " + request.downloadHandler.text);

            var result = JsonUtility.FromJson<SpeechRecognitionResultParameter>(request.downloadHandler.text);
            OnRecognitionResult?.Invoke(result);
        }
    }
}