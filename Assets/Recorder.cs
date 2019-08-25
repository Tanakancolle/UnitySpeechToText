using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GoogleCloudSpeechApiController
{
    public class Recorder
    {
        public AudioClip RecordingClip { get; private set; }
        private float[] _recodingClipData;
        private int _prevPosition;
        private readonly List<float> _recordDataList = new List<float>();

        public void Start(int frequency)
        {
            _recordDataList.Clear();
            RecordingClip = Microphone.Start(null, true, 1, frequency);
            _recodingClipData = new float[RecordingClip.samples * RecordingClip.channels];

            Debug.Log("Record Start");
            Update();
        }

        private async void Update()
        {
            while (Microphone.IsRecording(null))
            {
                var position = Microphone.GetPosition(null);
                if (position < 0)
                {
                    return;
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

                // 1秒を超えないように
                await Task.Delay(100);
            }
        }

        private void AddClipData(int start, int end)
        {
            for (var i = start; i < end; ++i)
            {
                _recordDataList.Add(_recodingClipData[i]);
            }
        }

        public List<float> End()
        {
            Debug.Log("Record End");
            Microphone.End(null);
            return _recordDataList;
        }
    }
}
