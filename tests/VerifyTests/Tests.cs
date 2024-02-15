namespace VerifyTests;

public class Tests
{
    [Theory]
    [ClassData(typeof(File))]
    public Task Test(FileModel file)
    {
        var templateProjectDirectory = Environment.GetEnvironmentVariable("TemplateProjectDirectory");
        var testOutputDirectory = Environment.GetEnvironmentVariable("TestOutputDirectory");

        return Verify(file.Data)
            .UseMethodName(file.Name)
            .UseDirectory(Path.GetFullPath($"./{testOutputDirectory}", templateProjectDirectory));
            //.AutoVerify();
    }
}