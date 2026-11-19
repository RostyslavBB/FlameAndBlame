using System.IO;
using UnityEngine;

namespace FB.Editor.Audio
{
    /// <summary>Writes mono 16-bit PCM WAV files.</summary>
    internal static class WavFile
    {
        public static void Write(string path, float[] samples, int rate)
        {
            using (FileStream stream = new FileStream(path, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                int dataSize = samples.Length * 2;
                writer.Write(new[] { 'R', 'I', 'F', 'F' });
                writer.Write(36 + dataSize);
                writer.Write(new[] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)1);
                writer.Write(rate);
                writer.Write(rate * 2);
                writer.Write((short)2);
                writer.Write((short)16);
                writer.Write(new[] { 'd', 'a', 't', 'a' });
                writer.Write(dataSize);
                for (int i = 0; i < samples.Length; i++)
                {
                    writer.Write((short)(Mathf.Clamp(samples[i], -1f, 1f) * short.MaxValue));
                }
            }
        }
    }
}
