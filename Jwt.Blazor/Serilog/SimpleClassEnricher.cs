using Serilog.Core;
using Serilog.Events;
using System.Collections.Generic;

namespace Jwt.Blazor
{
    // kudos: https://stackoverflow.com/a/50655507/425357
    public class SimpleClassEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var typeName = logEvent.Properties.GetValueOrDefault("SourceContext")?.ToString();

            if (typeName != null)
            {
                //var typeNameString = typeName.ToString();
                var pos = typeName.LastIndexOf('.');
                if (pos == -1)
                {
                    // '.' not found
                    // In Program.cs I add SourceContext like this: var Program = Log.ForContext("SourceContext", "Program");
                    //   then to log: Program.Information("SyncHub is starting...");
                    // 
                    // Which outputs "Program" or "Startup" so we remove the quotes & add on a space for correct formatting (space is used alongside program.cs tmpl)
                    typeName = typeName.Replace("\"", "");
                    logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("SourceContext", " " + typeName));
                }
                else
                {
                    var shortName = " " + typeName.Substring(pos + 1, typeName.Length - pos - 2);
                    logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("SourceContext", shortName));
                }
            }
        }
    }
}
