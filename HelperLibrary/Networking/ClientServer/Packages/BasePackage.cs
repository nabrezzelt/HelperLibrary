using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace HelperLibrary.Networking.ClientServer.Packages
{
    [Serializable]
    public abstract class BasePackage
    {
        public string SenderUid;
        public string DestinationUid;

        protected BasePackage(string senderUid, string destinationUid)
        {
            SenderUid = senderUid;
            DestinationUid = destinationUid;
        }

        public static byte[] Serialize(object package)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            bf.Serialize(ms, package); //Serialisiert dieses Objekt in einen ByteArray (funktioniert ählich wie JSON)
            byte[] serializedObject = ms.ToArray();
            ms.Close();

            return serializedObject;
        }

        public static object Deserialize(byte[] packageBytes)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(packageBytes);

            object packet = (BasePackage)bf.Deserialize(ms); //Deserialisiert den ByteArray in ein Objekt (von JSON zu Objekt)

            ms.Close();

            return packet;
        }

        public static T Deserialize<T>(byte[] packageBytes)
        {
            return (T) Deserialize(packageBytes);
        }
    }
}