using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using ViralVial.Video;

namespace ViralVial.Utilities
{
    public static class Functions
    {
        public static Vector3? ArrayToVector3(float[] array)
        {
            if (array == null) return null;
            return new Vector3(array[0], array[1], array[2]);
        }

        public static bool LayerMaskIncludes(LayerMask mask, int layer)
        {
            return (mask.value & 1 << layer) > 0;
        }

        public static T ReadFileAndDeserialize<T>(string filePath)
        {
            if (!File.Exists(filePath)) return default(T);
            return JsonConvert.DeserializeObject<T>(ReadFromFile(filePath));
        }

        public static void SerializeAndWriteToFile(object toSerialize, string filePath)
        {
            WriteToFile(
                JsonConvert.SerializeObject(toSerialize),
                filePath
            );
        }

        public static void WriteToFile(string contents, string filePath)
        {
            StreamWriter writer;
            FileInfo file = new FileInfo(filePath);
            file.Delete();
            if (!file.Exists)
            {
                writer = file.CreateText();
            }
            else
            {
                Debug.Log("Error: file already exist");
                writer = file.AppendText();
            }
            writer.WriteLine(contents);
            writer.Flush();
            writer.Dispose();
            writer.Close();
        }

        public static string ReadFromFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public static CoroutineRunner CreateNewTimer(float time, Action callbackOnFinish)
        {
            var coroutine = new CoroutineRunner();
            coroutine.Start(TimerCoroutine(time, callbackOnFinish));
            return coroutine;
        }

        private static IEnumerator TimerCoroutine(float time, Action callbackOnFinish)
        {
            yield return new WaitForSeconds(time);
            callbackOnFinish.Invoke();
        }

        public static string ToResolutionString(this Resolution resolution)
        {
            return $"{resolution.width}x{resolution.height}";
        }

        public static SimpleResolution ToSimpleResolution(this Resolution resolution)
        {
            return new SimpleResolution(resolution);
        }
    }
}