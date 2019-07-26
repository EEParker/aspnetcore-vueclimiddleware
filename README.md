# VueCliMiddleware

[![](https://img.shields.io/nuget/v/VueCliMiddleware.svg)](https://www.nuget.org/packages/VueCliMiddleware/)

This is a stand-alone module to add Vue Cli and Quasar Cli support to AspNet Core.


## ASP.NET 3.0 Preview Endpoint Routing (experimental)
First, be sure to switch Vue Cli or Quasar Cli to output distribution files to wwwroot directly (not dist).

* Quasar CLI: regex: "Compiled successfully"
* Vue CLI: regex: default or "running at"

See [Migrating Asp.Net 2.2 to 3.0 Endpoint Routing](https://docs.microsoft.com/en-us/aspnet/core/migration/22-to-30?view=aspnetcore-2.2&tabs=visual-studio#update-routing-startup-code)
```csharp
// To use with EndpointRouting

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // initialize vue cli middleware
#if DEBUG
                if (System.Diagnostics.Debugger.IsAttached)
                    endpoints.MapToVueCliProxy("{*path}", new SpaOptions { SourcePath = "ClientApp" }, "dev", regex: "Compiled successfully");
                else
#endif
                    // note: output of vue cli or quasar cli should be wwwroot
                    endpoints.MapFallbackToFile("index.html");
            });
```


## ASP.NET 2.2 Usage Example
```csharp
    using VueCliMiddleware;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
           services.AddMvc(); // etc
           
           // Need to register ISpaStaticFileProvider for UseSpaStaticFiles middleware to work
           services.AddSpaStaticFiles(configuration =>
           {
               configuration.RootPath = "ClientApp/dist";
           });
        }

        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
           // your config opts...
		   // optional basepath
		   // app.UsePathBase("/myapp");

           // add static files from SPA (/dist)
          app.UseSpaStaticFiles();

          app.UseMvc(routes => /* configure*/ );

          app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
#if DEBUG
                if (env.IsDevelopment())
                {
                    spa.UseVueCli(npmScript: "serve", port: 8080); // optional port
                }
#endif
            });
        }
    }
```

## CSPROJ Configuration
You may also need to add the following tasks to your csproj file. This are similar to what are found in the default ASPNETSPA templates.

```project.csproj
  <PropertyGroup>
    <!-- Typescript/Javascript Client Configuration -->
    <SpaRoot>ClientApp\</SpaRoot>
    <DefaultItemExcludes>$(DefaultItemExcludes);$(SpaRoot)node_modules\**</DefaultItemExcludes>
  </PropertyGroup>
  <Target Name="DebugEnsureNodeEnv" BeforeTargets="Build">
    <!-- Build Target:  Ensure Node.js is installed -->
    <Exec Command="node --version" ContinueOnError="true">
      <Output TaskParameter="ExitCode" PropertyName="ErrorCode" />
    </Exec>
    <Error Condition="'$(ErrorCode)' != '0'" Text="Node.js is required to build and run this project. To continue, please install Node.js from https://nodejs.org/, and then restart your command prompt or IDE." />
  </Target>

  <Target Name="DebugEnsureNpm" AfterTargets="DebugEnsureNodeEnv">
    <!-- Build Target:  Ensure Node.js is installed -->
    <Exec Command="npm --version" ContinueOnError="true">
      <Output TaskParameter="ExitCode" PropertyName="ErrorCode" />
    </Exec>
  </Target>

  <Target Name="EnsureNodeModulesInstalled" BeforeTargets="Build" Inputs="package.json" Outputs="packages-lock.json">
    <!-- Build Target: Restore NPM packages using npm -->
    <Message Importance="high" Text="Restoring dependencies using 'npm'. This may take several minutes..." />

    <Exec WorkingDirectory="$(SpaRoot)" Command="npm install" />
  </Target>

  <Target Name="PublishRunWebpack" AfterTargets="ComputeFilesToPublish">
    <!-- Build Target: Run webpack dist build -->
    <Message Importance="high" Text="Running npm build..." />
    <Exec WorkingDirectory="$(SpaRoot)" Command="npm run build" />

    <!-- Include the newly-built files in the publish output -->
    <ItemGroup>
      <DistFiles Include="$(SpaRoot)dist\**" />
      <ResolvedFileToPublish Include="@(DistFiles->'%(FullPath)')" Exclude="@(ResolvedFileToPublish)">
        <RelativePath>%(DistFiles.Identity)</RelativePath>
        <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
      </ResolvedFileToPublish>
    </ItemGroup>
  </Target>

```

## History

Due to the discussion [here](https://github.com/aspnet/JavaScriptServices/pull/1726), it was decided to not be included in the Microsoft owned package.
