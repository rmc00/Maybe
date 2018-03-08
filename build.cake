#addin Cake.Coveralls
#tool coveralls.io
#tool "nuget:https://www.nuget.org/api/v2?package=OpenCover&version=4.6.519"
#tool "nuget:https://www.nuget.org/api/v2?package=ReportGenerator&version=2.4.5"

var target = Argument("target", "Run-Tests");

Task("Restore-NuGet-Packages").Does(() =>
{
    DotNetCoreRestore("Maybe.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    DotNetCoreBuild("Maybe.sln", new DotNetCoreBuildSettings
    {
        Configuration = "release",
        ArgumentCustomization = arg => arg.AppendSwitch("/p:DebugType","=","Full")
    });
});

Task("Run-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    var success = true;
    var openCoverSettings = new OpenCoverSettings
    {
        OldStyle = true,
        MergeOutput = true
    }
    .WithFilter("+[*]Maybe* -[*]Maybe.Test*");

    var project = "Maybe.Test\\Maybe.Test.csproj";
    try 
    {
        //var projectFile = MakeAbsolute(project).ToString();
        var dotNetTestSettings = new DotNetCoreTestSettings
        {
            Configuration = "release",
            NoBuild = true
        };

        OpenCover(context => context.DotNetCoreTest(project, dotNetTestSettings), "coverage.xml", openCoverSettings);
    }
    catch(Exception ex)
    {
        success = false;
        Error("There was an error while running the tests", ex);
    }

    //ReportGenerator(paths.Files.TestCoverageOutput, paths.Directories.TestResults);

    if(!success)
    {
        throw new CakeException("There was an error while running the tests");
    }

    CoverallsIo("coverage.xml", new CoverallsIoSettings()
    {
        RepoToken = "PH6pHukW5nOSJNt0hCslvwFoZd3Fv6IC0"
    });
});

RunTarget(target);