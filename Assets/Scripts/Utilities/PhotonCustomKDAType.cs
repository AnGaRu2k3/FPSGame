using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ExitGames.Client.Photon;
using Photon.Pun;

public class PhotonCustomKDAType : MonoBehaviour
{
    private void Awake()
    {
        RegisterCustomTypes();
    }

    public static void RegisterCustomTypes()
    {
        PhotonPeer.RegisterType(typeof(KDA), (byte)'K', SerializeKDA, DeserializeKDA);
    }

    private static byte[] SerializeKDA(object customObject)
    {
        KDA kda = (KDA)customObject;
        using (var stream = new System.IO.MemoryStream())
        {
            using (var writer = new System.IO.BinaryWriter(stream))
            {
                writer.Write(kda.playerName);
                writer.Write(kda.kills);
                writer.Write(kda.deaths);
            }
            return stream.ToArray();
        }
    }

    private static object DeserializeKDA(byte[] data)
    {
        KDA kda;
        using (var stream = new System.IO.MemoryStream(data))
        {
            using (var reader = new System.IO.BinaryReader(stream))
            {
                string playerName = reader.ReadString();
                int kills = reader.ReadInt32();
                int deaths = reader.ReadInt32();
                kda = new KDA(playerName, kills, deaths);
            }
        }
        return kda;
    }
}
