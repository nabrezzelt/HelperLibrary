using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace HelperLibrary.Networking.ClientServer.Packets
{
    [Serializable]
    public abstract class BasePacket
    {        
        public string SenderUid;
        public string DestinationUid;

        protected BasePacket(string senderUid, string destinationUid)
        {
            SenderUid = senderUid;
            DestinationUid = destinationUid;
        }

        public static byte[] Serialize(object packet)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            bf.Serialize(ms, packet); //Serialisiert dieses Objekt in einen ByteArray (funktioniert ählich wie JSON)
            byte[] serializedObject = ms.ToArray();
            ms.Close();

            return serializedObject;
        }

        public static object Deserialize(byte[] packetBytes)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(packetBytes);

            object packet = (BasePacket)bf.Deserialize(ms); //Deserialisiert den ByteArray in ein Objekt (von JSON zu Objekt)

            ms.Close();

            return packet;
        }

        public enum BasePacketType
        {
            Authentication,
            ClientConnection,
            List,
            Playlist,
            PlaylistHandling,
            File,
            ClientState,
            ClientControlling
        }
    }
}