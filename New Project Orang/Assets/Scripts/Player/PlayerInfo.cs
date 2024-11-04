using Unity.Collections;
using Unity.Netcode;

public struct PlayerInfo : INetworkSerializable
{
    public FixedString128Bytes PlayerName;
    public int Score;

    public PlayerInfo(FixedString128Bytes playerName, int score)
    {
        PlayerName = playerName;
        Score = score;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Score);
    }
}