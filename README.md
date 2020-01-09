# VueCliMiddleware - Supporting Vue Cli and Quasar Cli

[![](https://img.shields.io/nuget/v/VueCliMiddleware.svg)](https://www.nuget.org/packages/VueCliMiddleware/)

This is a stand-alone module to add Vue Cli and Quasar Cli support to AspNet Core.

See the examples here: [https://github.com/EEParker/aspnetcore-vueclimiddleware/tree/master/samples](https://github.com/EEParker/aspnetcore-vueclimiddleware/tree/master/samples)

## ASP.NET 3.0 Preview Endpoint Routing (experimental)
First, be sure to switch Vue Cli or Quasar Cli to output distribution files to wwwroot directly (not dist).

* Quasar CLI: regex: "Compiled successfully"
* Vue CLI: regex: default or "running at" or "Starting development server"
 >the reason for **`Starting development server`** ,the npm-script running checkpoint: 
 Although the dev server may eventually tell us the URL it's listening on,
                    it doesn't do so until it's finished compiling, and even then only if there were
                    no compiler warnings. So instead of waiting for that, consider it ready as soon
                     as it starts listening for requests.[see the codes](https://github.com/EEParker/aspnetcore-vueclimiddleware/blob/master/src/VueCliMiddleware/VueDevelopmentServerMiddleware.cs#L91)

See [Migrating Asp.Net 2.2 to 3.0 Endpoint Routing](https://docs.microsoft.com/en-us/aspnet/core/migration/22-to-30?view=aspnetcore-2.2&tabs=visual-studio#update-routing-startup-code)
```csharp


    public class Startup {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // NOTE: PRODUCTION Ensure this is the same path that is specified in your webpack output
            services.AddSpaStaticFiles(opt => opt.RootPath = "ClientApp/dist");
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // NOTE: PRODUCTION uses webpack static files
            app.UseSpaStaticFiles();

            // To use with EndpointRouting
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // NOTE: VueCliProxy is meant for developement and hot module reload
                // NOTE: SSR has not been tested
                // Production systems should only need the UseSpaStaticFiles() (above)
                // You could wrap this proxy in either
                // if (System.Diagnostics.Debugger.IsAttached)
                // or a preprocessor such as #if DEBUG
                endpoints.MapToVueCliProxy(
                    "{*path}",
                    new SpaOptions { SourcePath = "ClientApp" },
                    npmScript: (System.Diagnostics.Debugger.IsAttached) ? "serve" : null,
                    regex: "Compiled successfully",
                    forceKill: true
                    );
            });
        }
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
           services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });
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
