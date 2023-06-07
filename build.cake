
var target = Argument("target", "Test");
var solutionFolder = "./";
var configuration = Argument("configuration", "Release");


Task("Restore")
    .Does(() => {
        DotNetRestore();
    });

Task("Build")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetBuild(solutionFolder, new DotNetBuildSettings
        {
            NoRestore = true,
            Configuration = configuration
        });
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
        DotNetTest(solutionFolder, new DotNetTestSettings{
            NoRestore = true,
            Configuration = configuration,
            NoBuild = true
        });
    });

RunTarget(target);