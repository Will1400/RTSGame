using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SerializationHelper : MonoBehaviour
{
    public static byte[] Serialize(object obj)
    {
        if (obj == null)
            return null;

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();

        bf.Serialize(ms, obj);

        return ms.ToArray();
    }

    public static T Deserialize<T>(byte[] byteArray)
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        ms.Write(byteArray, 0, byteArray.Length);
        ms.Seek(0, SeekOrigin.Begin);

        return (T)bf.Deserialize(ms);
    }
}
