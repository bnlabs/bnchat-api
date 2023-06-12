#addin "nuget:?package=Cake.Git&version=3.0.0"
#addin "nuget:?package=Newtonsoft.Json&version=11.0.2"

using Cake.Git;
using Newtonsoft.Json;
using System;
using System.IO;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
var target = Argument("target", "Test");
var solutionFolder = "./";
var configuration = Argument("configuration", "Release");


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
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

Task("Generate Build Info")
    .IsDependentOn("Build")
    .Does((cakeContext) => {
        var lastCommit = ((ICakeContext)cakeContext).GitLogTip("./");
        var repoPath = DirectoryPath.FromString(".");
        var currentBranch = ((ICakeContext)cakeContext).GitBranchCurrent(repoPath);
        var timeZone = TimeZoneInfo.Local;

        var gitHash = lastCommit.Sha?.Substring(0,7) ?? "";
        var buildDate = DateTime.Now;
        var buildName = $"{currentBranch.FriendlyName}:{gitHash}";

        var data = new { buildInfo = new {
            buildName,
            hash = gitHash,
            branch = currentBranch.FriendlyName,
            buildDate = buildDate.ToString("yyy-MM-dd HH:mm") + timeZone.DisplayName
        }};

        var json = JsonConvert.SerializeObject(data);
        var outputPath = "./ToffApi";
        var fileName = "buildinfo.json";

        var filePath = System.IO.Path.Combine(outputPath, fileName);
        System.IO.File.WriteAllText(filePath, json);
    });

Task("Test")
    .IsDependentOn("Build")
    .IsDependentOn("Generate Build Info")
    .Does(() => {
        DotNetTest(solutionFolder, new DotNetTestSettings{
            NoRestore = true,
            Configuration = configuration,
            NoBuild = true
        });
    });


//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
RunTarget(target);