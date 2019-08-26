using System;

namespace SpeechToText
{
    /// <summary>
    /// https://cloud.google.com/speech-to-text/docs/reference/rest/v1p1beta1/speech/recognize
    /// </summary>

    [Serializable]
    public class SpeechRecognizeParameter
    {
        public RecognitionConfig config = new RecognitionConfig();
        public RecognitionAudio audio = new RecognitionAudio();
        //public string name;
    }

    [Serializable]
    public class RecognitionConfig
    {
        public AudioEncoding encoding;
        public int sampleRateHertz;
        public int audioChannelCount;
        public bool enableSeparateRecognitionPerChannel;
        public string languageCode = "ja-JP";
        public int maxAlternatives;
        public bool profanityFilter;
        public SpeechContext[] speechContexts = new SpeechContext[0];
        public bool enableWordTimeOffsets;
        public bool enableAutomaticPunctuation;
        public RecognitionMetadata metadata = new RecognitionMetadata();
        public string model;
        public bool useEnhanced;
    }

    [Serializable]
    public class RecognitionAudio
    {
        public string content;
        //public string uri;
    }

    public enum AudioEncoding
    {
        ENCODING_UNSPECIFIED,
        LINEAR16,
        FLAC,
        MULAW,
        AMR,
        AMR_WB,
        OGG_OPUS,
        SPEEX_WITH_HEADER_BYTE
    }

    public enum InteractionType
    {
        INTERACTION_TYPE_UNSPECIFIED,
        DISCUSSION,
        PRESENTATION,
        PHONE_CALL,
        VOICEMAIL,
        PROFESSIONALLY_PRODUCED,
        VOICE_SEARCH,
        VOICE_COMMAND,
        DICTATION
    }

    public enum MicrophoneDistance
    {
        MICROPHONE_DISTANCE_UNSPECIFIED,
        NEARFIELD,
        MIDFIELD,
        FARFIELD,
    }

    public enum OriginalMediaType
    {
        ORIGINAL_MEDIA_TYPE_UNSPECIFIED,
        AUDIO,
        VIDEO,
    }

    public enum RecordingDeviceType
    {
        RECORDING_DEVICE_TYPE_UNSPECIFIED,
        SMARTPHONE,
        PC,
        PHONE_LINE,
        VEHICLE,
        OTHER_OUTDOOR_DEVICE,
        OTHER_INDOOR_DEVICE,
    }

    [Serializable]
    public class SpeechContext
    {
        public string[] phrases;
    }

    [Serializable]
    public class RecognitionMetadata
    {
        public InteractionType interactionType;
        public uint industryNaicsCodeOfAudio;
        public MicrophoneDistance microphoneDistance;
        public OriginalMediaType originalMediaType = OriginalMediaType.AUDIO;
        public RecordingDeviceType recordingDeviceType = RecordingDeviceType.PC;
        public string recordingDeviceName;
        public string originalMimeType;
        public string audioTopic;
    }
}
