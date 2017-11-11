const string BUILD = "Build";

const string PACK = "Package";
const string TEST = "Test";
const string RESTORE = "Restore";
const string CLEAN = "Clean";

const string DEFAULT = "Default";
const string RELEASE = "Release";

var target = Argument("target", DEFAULT);
var configuration = Argument("configuration", RELEASE);

//  Project Directory
var projDir = "./src/netcore10/";      
// Solution file if needed
var solutionFile = "RedmineApi.Core.sln"; 
// Destination Binary File Directory name i.e. bin
var binDir = String.Concat(projDir,"bin");
// The output directory the build artefacts saved too
var outputDir = Directory(binDir) + Directory(configuration);  
//
var outputPackDir = "../nupkgs";

var version = "1.0.1";
var revision = "";

var buildSettings = new DotNetCoreBuildSettings
    {
        Framework = "netcoreapp2.0",
        Configuration = configuration,
        ArgumentCustomization = args => args.Append(String.Concat("/p:SemVer=", version))
    };


Task(CLEAN)
    .Does(()=>
    {
        CleanDirectory(outputDir);
        CleanDirectory(outputPackDir);
    });

Task(RESTORE)
    .Does(() =>
    {
        //"src/\" \"test/\" - 
        DotNetCoreRestore(projDir);
    });

Task(BUILD)
    .IsDependentOn(CLEAN)
    .IsDependentOn(RESTORE)
    .Does(() =>
    {
        DotNetCoreBuild(solutionFile, buildSettings);
    });


Task(PACK)
    .IsDependentOn(BUILD)
    .Does(() =>
    {
        var packSettings = new DotNetCorePackSettings
        {
            OutputDirectory = outputPackDir,
            Configuration = configuration,
            VersionSuffix = revision,
            ArgumentCustomization = args => args.Append(String.Concat("/p:PackageVersion=",version)),
            NoBuild = true
        };

        DotNetCorePack(solutionFile, packSettings);
    });

Task(DEFAULT)
    .IsDependentOn(PACK);

RunTarget(target);

private void CleanDirectory(string path)
{
    if(DirectoryExists(path))
    {
        DeleteDirectory(path, recursive: true);
    }
}