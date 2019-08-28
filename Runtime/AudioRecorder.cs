using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SpeechToText
{
    public class AudioRecorder
    {
        public AudioClip RecordingClip { get; private set; }
        private float[] _recodingClipData;
        private int _prevPosition;
        private readonly List<float> _recordDataList = new List<float>();
        private bool _isRecording = false;

        public void Start(int frequency)
        {
            _recordDataList.Clear();
            RecordingClip = Microphone.Start(null, true, 1, frequency);
            _recodingClipData = new float[RecordingClip.samples * RecordingClip.channels];

            Debug.Log("Record Start");

            if (_isRecording)
            {
                Recording();
            }
        }

        private async void Recording()
        {
            _isRecording = true;
            while (_isRecording)
            {
                // 1秒を超えないように
                await Task.Delay(1000 / 30);

                var position = Microphone.GetPosition(null);
                if (position < 0)
                {
                    continue;
                }

                RecordingClip.GetData(_recodingClipData, 0);

                // ローテーションしているかチェック
                if (_prevPosition > position)
                {
                    AddClipData(_prevPosition, _recodingClipData.Length);

                    _prevPosition = 0;
                }

                AddClipData(_prevPosition, position);

                _prevPosition = position;
            }
        }

        private void AddClipData(int start, int end)
        {
            for (var i = start; i < end; ++i)
            {
                _recordDataList.Add(_recodingClipData[i]);
            }
        }

        public float[] End()
        {
            _isRecording = false;
            Microphone.End(null);
            return _recordDataList.ToArray();
        }
    }
}
