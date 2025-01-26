namespace NetworkObjects.Debug;

public class WriteLoadFile : NetworkObjects.Debug.IWriteLoadFile
{
    public bool Locked { get; private set; }

    public void SaveToFile(string data, string filePath)
    {
        if (!Locked)
        {
            Locked = true;
        }
        else
        {
            Task.Run(async delegate
            {
                await Task.Delay(500);
                SaveToFile(data, filePath);
            });
            return;
        }

        //CreatePathIfNotExists(filePath);
        // File.WriteAllText(filePath,data);
        CreateDirectoryPathIfNotExists(filePath);
        using (var stream = File.CreateText(filePath))
        {
            stream.WriteLine(data);
        }

        Locked = false;
    }

    public void AppendToFile(string data, string filePath)
    {
        if (!Locked)
        {
            Locked = true;
        }
        else
        {
            Task.Run(async delegate
            {
                await Task.Delay(500);
                SaveToFile(data, filePath);
            });
            return;
        }

        // CreatePathIfNotExists(filePath);
        File.AppendAllText(filePath, data);
        Locked = false;
    }

    public void CreateDirectoryPathIfNotExists(string fileLocation)
    {
        var arr = fileLocation.Split("/");
        var path = "";
        if (arr.Length > 0)
            for (var i = 0; i < arr.Length - 1; i++)
            {
                path = i > 0 ? $"{path}/{arr[i]}" : arr[i];
                if (path.Length <= 0) continue;
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            }
        // if (!File.Exists(fileLocation))
        // {
        //     File.AppendText(fileLocation);
        // }
    }


    public string LoadFromSave(string filePath)
    {
        if (File.Exists(filePath)) return File.ReadAllText(filePath);
        return "";
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }
}