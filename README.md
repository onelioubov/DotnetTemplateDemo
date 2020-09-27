# DotnetTemplateDemo
This is a demo of how custom .NET templates can be used to quickly create a functioning and deployable API. This includes:    
* The creation of a local and remote Git repository on GitHub during template-invocation.  
    * Requires [GitHub CLI](https://cli.github.com/) configured for your GitHub account.
    * Currently only works on Linux and MacOS, but I have plans to make it friendly to Windows as well (unless you have WSL, in which case, you should be good).  
    
* A [Terraform](https://learn.hashicorp.com/tutorials/terraform/install-cli) module to create a Resource Group, Web App, and Application Insights resources within Microsoft Azure.
    * Please note that this assumes you have the following cloud resources for this to be deployed:      
        - Azure Subscription.
        - Azure Storage Account with a container for the Terraform state files.
        - Azure App Service Plan to deploy the Web App to.    
        
        The reason these resources need to be pre-created is because I'm assuming you would be running multiple web apps out of a single App Service Plan (that is managed outside of the actual repo containing your API code). If that's not the case, an App Service Plan resource can always be added to the provided Terraform template.
     * When you're ready to create your resources, make sure you're logged into your account via the [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest) and set the Subscription you want to use; then, just run a `terraform init` and `terraform apply` to get the resources created.
 * A build pipeline to compile and test your code (and generate a client when ready) using [Nuke](http://www.nuke.build/docs/getting-started/setup.html).
 * A GitHub Action to build and release your API to the resources created by Terraform. This runs automatically on push to the remote [main] branch.
    * Please note that there is still a manual step to include the publish profile from the newly-created Azure Web App (you should be able to get it as an output when you create the web app via Terraform) as a [GitHub repo secret](https://docs.github.com/en/free-pro-team@latest/actions/reference/encrypted-secrets) named azureWebAppPublishProfile. This needs to be done before you push to [main].
 * A GitHub Action to package the template itself and push it to a NuGet feed for further distribution.

#### Some Caveats
- Ultimately, this template has a few hard-coded values, such as ones for the NuGet feed and the pre-created resources in Azure (the App Service Plan and Storage Account names). This is for the purposes of showing how it can be tailored to your org (including naming conventions and some standard variables used throughout the company), where the "org" in this specific case is my GitHub account, NuGet feed, and Azure Subscription.

- This is not for general use as it will not work for you out of the box (unless you actually change the aforementioned hard-coded values to your own).

- This is for the purposes of demonstrating how custom .NET templates can be used and is not a statement on how the code within the template should actually be structured (even though _some_ of my opinions do apply here). There are probably a few things in here that you don't want to do in a live application. However, there are some resources below pertaining to the items I've implemented in this repo; they're still good to have in your toolbox, even if the demo implementation of them does not necessarily show their best usage.  

# Resources
## General .NET Tempate
[Microsoft documentation for .NET Tempates](https://docs.microsoft.com/en-us/dotnet/core/tools/custom-templates)  
[Microsoft custom template tutorial](https://docs.microsoft.com/en-us/dotnet/core/tutorials/cli-templates-create-item-template)  
[.NET Template Parameter Generators](https://github.com/dotnet/templating/wiki/Available-Parameter-Generators)  
[Post Action Registry](https://github.com/dotnet/templating/wiki/Post-Action-Registry)    
[Testing Your Templates](https://github.com/dotnet/templating/wiki/Testing-your-templates)  
[Useful Microsoft blog post](https://devblogs.microsoft.com/dotnet/how-to-create-your-own-templates-for-dotnet-new/)  

## Miscellaneous Resources
[Recommended .NET project structure](https://gist.github.com/davidfowl/ed7564297c61fe9ab814)  
[Microsoft doc for NSwag Swagger specification](https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-nswag?view=aspnetcore-3.1&tabs=netcore-cli)  
[Microsoft doc for healthchecks](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/monitor-app-health)  
[Build automation with Nuke](http://www.nuke.build/)  
[GitVersion configuration](https://gitversion.readthedocs.io/en/latest/input/docs/configuration/)  
[Serilog for logging](https://serilog.net/)  
[Using appsettings.json for Serilog configuration](https://github.com/serilog/serilog-settings-configuration)  
[Deploying Azure App Service with GitHub Actions](https://docs.microsoft.com/en-us/azure/app-service/deploy-github-actions)  
[Documenting Architecture Decisions](https://cognitect.com/blog/2011/11/15/documenting-architecture-decisions)  
[Another ADR Resource](https://github.com/joelparkerhenderson/architecture_decision_record)  
[GitHub CLI](https://cli.github.com/)  
        
If you have any questions about this repo, feel free to reach out!