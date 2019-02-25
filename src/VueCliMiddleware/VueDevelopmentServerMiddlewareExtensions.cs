using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices;
using System;

namespace VueCliMiddleware
{
    /// <summary>
    /// Extension methods for enabling Vue development server middleware support.
    /// </summary>
    public static class VueCliMiddlewareExtensions
    {
        /// <summary>
        /// Handles requests by passing them through to an instance of the vue-cli server.
        /// This means you can always serve up-to-date CLI-built resources without having
        /// to run the vue-cli server manually.
        ///
        /// This feature should only be used in development. For production deployments, be
        /// sure not to enable the vue-cli server.
        /// </summary>
        /// <param name="spaBuilder">The <see cref="ISpaBuilder"/>.</param>
        /// <param name="npmScript">The name of the script in your package.json file that launches the vue-cli server.</param>
        /// <param name="port">Specify vue cli server port number. If &lt; 80, uses random port. </param>
        /// <param name="runner">Specify the runner, Npm and Yarn are valid options. Yarn support is HIGHLY experimental.</param>
        /// <param name="regex">Specify a custom regex string to search for in the log indicating vue-cli serve is complete.</param>
        public static void UseVueCli(
            this ISpaBuilder spaBuilder,
            string npmScript,
            int port = 0,
            ScriptRunnerType runner = ScriptRunnerType.Npm,
            string regex = VueCliMiddleware.DefaultRegex,
            string uriScheme = VueCliMiddleware.DefaultUriScheme)
        {
            if (spaBuilder == null)
            {
                throw new ArgumentNullException(nameof(spaBuilder));
            }

            var spaOptions = spaBuilder.Options;

            if (string.IsNullOrEmpty(spaOptions.SourcePath))
            {
                throw new InvalidOperationException($"To use {nameof(UseVueCli)}, you must supply a non-empty value for the {nameof(SpaOptions.SourcePath)} property of {nameof(SpaOptions)} when calling {nameof(SpaApplicationBuilderExtensions.UseSpa)}.");
            }

            VueCliMiddleware.Attach(spaBuilder, npmScript, port, runner: runner, regex: regex, uriScheme: uriScheme);
        }
    }
}
