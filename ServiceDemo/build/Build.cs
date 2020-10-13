using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Docker;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.NSwag;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.Docker.DockerTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.NSwag.NSwagTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });
    
    Target Test => _ => _
        .After(Compile)
        .Executes(() =>
        {
            var testProjects = Solution.AllProjects.Where(project =>
                project.Path.ToString().Contains(TestsDirectory.ToString()));
            DotNetTest(t => t
                .EnableNoBuild()
                .SetConfiguration(Configuration)
                .SetWorkingDirectory(TestsDirectory)
                .CombineWith(testProjects, (x, p) => x
                    .SetProjectFile(p)
                    .SetLogger("nunit;LogFilePath=TestResults/" + p.Name + "-Result.xml")));
        });
    
    Target GenerateClient => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var clientDir = SourceDirectory;
            var clientProjDir = clientDir / "ServiceDemo.Client";
            EnsureCleanDirectory(clientProjDir);

            var openApiPath = clientDir / "ServiceDemo.json";

            NSwagAspNetCoreToOpenApi(x => x
                .SetNSwagRuntime("NetCore31")
                .SetAssembly(SourceDirectory / "ServiceDemo.Api" / "bin" / Configuration.ToString() / "netcoreapp3.1" / "ServiceDemo.Api.dll")
                .SetDocumentName("v1")
                .EnableUseDocumentProvider()
                .SetOutputType(SchemaType.OpenApi3)
                .SetOutput(openApiPath)
            );
            
            NSwagSwaggerToCSharpClient(x => x
                .SetNSwagRuntime("NetCore31")
                .SetInput(openApiPath)
                .SetOutput(clientProjDir / "ServiceDemo.Client.cs")
                .SetNamespace("ServiceDemo.Clients")
                .SetGenerateClientInterfaces(true)
                .SetGenerateExceptionClasses(true)
                .SetExceptionClass("{controller}ClientException")
            );

             var version = GitRepository.Branch.Equals("main", StringComparison.OrdinalIgnoreCase) ? GitVersion.MajorMinorPatch : GitVersion.NuGetVersionV2;
             
             DotNet($"new classlib -o {clientProjDir}", workingDirectory: clientProjDir);
             DeleteFile(clientProjDir / "Class1.cs");
             DotNet($"add package Newtonsoft.Json", workingDirectory: clientProjDir);
             DotNet("add package System.ComponentModel.Annotations", workingDirectory: clientProjDir);
             
               DotNetPack(x => x
                   .SetProject(clientProjDir)
                   .SetOutputDirectory(ArtifactsDirectory)
                   .SetConfiguration(Configuration)
                   .SetVersion(version)
                   .SetIncludeSymbols(true)
              );
        });
    Target Publish => _ => _
        .After(Test)
        .Executes(() =>
        {
            var projectSolution = Solution.AllProjects.Where(p =>
                !p.Name.Contains("Tests")
                && !p.Name.Contains("build")
                && p.Is(ProjectType.CSharpProject));
        
            DotNetPublish(s => s
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore()
                .CombineWith(projectSolution, (x, p) => x
                    .SetProject(p)
                    .SetOutput(ArtifactsDirectory / p.Name)));
        });
}
