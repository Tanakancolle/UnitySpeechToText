using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpeechToText
{
    public static class AudioConverter
    {
        private const int Linear16BitSize = 16;
        private const int Linear16Range = 32767;

        private class HeaderElement
        {
            public readonly int size;
            public byte[] bytes;

            public HeaderElement(int size, int val) : this(size, BitConverter.GetBytes(val))
            {
            }

            public HeaderElement(int size, string str) : this(size, Encoding.UTF8.GetBytes(str))
            {
            }

            public HeaderElement(int size, byte[] bytes) : this(size)
            {
                this.bytes = bytes;
            }

            public HeaderElement(int size)
            {
                this.size = size;
            }
        }

        private static readonly HeaderElement[] Linear16HeaderElements = new[]
        {
            new HeaderElement(4, "RIFF"),
            new HeaderElement(4),
            new HeaderElement(4, "WAVE"),
            new HeaderElement(4, "fmt "),
            new HeaderElement(4, Linear16BitSize),
            new HeaderElement(2, 1),
            new HeaderElement(2, 1),
            new HeaderElement(4),
            new HeaderElement(4),
            new HeaderElement(2, Linear16BitSize / 8),
            new HeaderElement(2, Linear16BitSize),
            new HeaderElement(4, "data"),
            new HeaderElement(4),
        };

        private static readonly int HeaderSize = Linear16HeaderElements.Sum(element => element.size);

        private enum Linear16HeaderChangeElementType
        {
            ChunkSize = 1,
            Frequency = 7,
            BytePerSec = 8,
            DataSize = 12,
        }

        public static byte[] CreateLinear16(AudioClip clip)
        {
            using (var stream = new MemoryStream())
            {
                SetLinear16Header(stream, clip);
                SetAudioData(stream, clip);

                return stream.ToArray();
            }
        }

        private static void SetLinear16Header(MemoryStream stream, AudioClip clip)
        {
            var clipSize = clip.samples * 2;
            Linear16HeaderElements[(int) Linear16HeaderChangeElementType.ChunkSize].bytes = BitConverter.GetBytes(clipSize + HeaderSize - 8);
            Linear16HeaderElements[(int) Linear16HeaderChangeElementType.Frequency].bytes = BitConverter.GetBytes(clip.frequency);
            Linear16HeaderElements[(int) Linear16HeaderChangeElementType.BytePerSec].bytes = BitConverter.GetBytes(clip.frequency * 2);
            Linear16HeaderElements[(int) Linear16HeaderChangeElementType.DataSize].bytes = BitConverter.GetBytes(clipSize);

            foreach (var element in Linear16HeaderElements)
            {
                stream.Write(element.bytes, 0, element.size);
            }
        }

        private static void SetAudioData(MemoryStream stream, AudioClip clip)
        {
            var samples = new float[clip.samples];
            clip.GetData(samples, 0);

            foreach (var sample in samples)
            {
                // Sample = -1 ~ 1 to Linear16 = -32767(8) ~ 32767
                stream.Write(BitConverter.GetBytes(sample * Linear16Range), 0, 2);
            }
        }
    }
}
