namespace NetworkObjects.Debug;

public interface IWriteLoadFile
{
    bool Locked { get; }
    void AppendToFile(string metaData, string path);
    void SaveToFile(string s, string path);
}