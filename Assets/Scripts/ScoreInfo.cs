using System;
using System.IO;

public class ScoreInfo : IComparable<ScoreInfo>
{
    public ScoreInfo(int score, string username)
    {
        Score = score;
        Username = username;
    }
        
    public int Score;
    public string Username;
    public bool isLast;

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(Score);
        writer.Write(Username);
    }

    public static ScoreInfo Deserialize(BinaryReader reader)
    {
        var score = reader.ReadInt32();
        var username = reader.ReadString();
        return new ScoreInfo(score, username);
    }
        
    public int CompareTo(ScoreInfo other)
    {
        if (other == null)
        {
            return 1;
        }

        return Score.CompareTo(other.Score);
    }
}
