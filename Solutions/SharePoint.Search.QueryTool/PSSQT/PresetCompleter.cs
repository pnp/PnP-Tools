using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace PSSQT
{
    public class PresetCompleter : IArgumentCompleter
    {
        public IEnumerable<CompletionResult> CompleteArgument(string commandName, string parameterName, string wordToComplete, CommandAst commandAst, IDictionary fakeBoundParameters)
        {
            // We need to ask the backend for a complete list of available groups. This is before the Backend parameter has been bound, so wee need to look at fakeBoundParameters
            try
            {
                List<string> availablePresets = new List<string>();

                var environmentVariable = Environment.GetEnvironmentVariable("PSSQT_PresetsPath");

                if (environmentVariable != null)
                {
                    var dirs = environmentVariable.Split(';');
                        
                    foreach (var dir in dirs)
                    {
                        availablePresets.AddRange(Directory.GetFiles(dir, "*.xml").Select(p => Path.GetFileNameWithoutExtension(p)));
                    }
                }

                var modifiedWordToComplete = Path.GetFileNameWithoutExtension(wordToComplete);

                return availablePresets.
                      Where(new WildcardPattern(modifiedWordToComplete + "*", WildcardOptions.IgnoreCase).IsMatch).
                      Select(s => new CompletionResult(s));

            }
            catch (Exception)
            {
                return new List<CompletionResult>();
            }
        }

    }


}
