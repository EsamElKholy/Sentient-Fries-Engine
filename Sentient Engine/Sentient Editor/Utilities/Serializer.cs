﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sentient_Editor.Utilities
{
    public static class Serializer
    {
        public static void ToFile<T>(T instance, string path) 
        {
            try
            {
                using var fileStream = new FileStream(path, FileMode.Create);
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(fileStream, instance);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);

                Logger.Log(MessageType.Info, $"Failed to serialize {instance} to file {path} with exception {e.Message}");

                throw;
            }
        }

        public static T? FromFile<T>(string path)
        {
            try
            {
                using var fileStream = new FileStream(path, FileMode.Open);
                var serializer = new DataContractSerializer(typeof(T));
                T? instance =  (T?)serializer.ReadObject(fileStream);

                return instance;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);

                Logger.Log(MessageType.Info, $"Failed to deserialize file {path} with exception {e.Message}");

                throw;
            }
        }
    }
}
