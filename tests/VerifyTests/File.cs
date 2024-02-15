
using System.Collections;
using IOFile = System.IO.File;

namespace VerifyTests;

public class File : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        var templateProjectDirectory = Environment.GetEnvironmentVariable("TemplateProjectDirectory");
        var templateOutputDirectory = Environment.GetEnvironmentVariable("TemplateOutputDirectory");

        var path = Path.GetFullPath($"./{templateOutputDirectory}", templateProjectDirectory);
        var directories = Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories);

        // Add files in folders to include
        var filesToInclude = new List<string>();
        foreach (var directory in directories)
        {
            var relativeDirPath = Path.GetRelativePath(path, directory);
            var files = Directory.EnumerateFiles(path, $"{relativeDirPath}/**", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                filesToInclude.Add(file);
            }
        }

        // Add files in root folder
        var rootFolderFiles = Directory.EnumerateFiles(path, $"*", SearchOption.TopDirectoryOnly);
        filesToInclude.AddRange(rootFolderFiles);

        foreach (var filePath in filesToInclude)
        {
            using var fs = IOFile.OpenRead(filePath);
            using var reader = new StreamReader(fs);

            yield return new object[]
            {
                new FileModel
                {
                    Name = Path.GetRelativePath(path, filePath).Replace("\\", "_").Replace("/", "_"),
                    Data = reader.ReadToEnd()
                }
            };
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}

public record FileModel
{
    public required string Name { get; init; }

    public required string Data { get; init; }
}
