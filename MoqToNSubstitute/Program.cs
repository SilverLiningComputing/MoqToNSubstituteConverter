using MoqToNSubstitute.Conversion;
using MoqToNSubstitute.Utilities;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MoqToNSubstitute.Tests")]

Logger.Log("Starting process...");

switch (args.Length)
{
    case 0:
        Logger.Log("No arguments, running analysis on current directory");
        MoqToNSubstituteConverter.Convert();
        break;
    case 1:
        var onlyArg = args[0];
        if (bool.TryParse(onlyArg, out var analysisOnly))
        {
            Logger.Log(analysisOnly ? "Running analysis on current directory" : "Running transformation on current directory");
            MoqToNSubstituteConverter.Convert("", analysisOnly);
        }
        else
        {
            Logger.Log($"Running analysis on onlyArg");
            MoqToNSubstituteConverter.Convert(onlyArg);
        }
        break;
    default:
        var first = args[0];
        var second = args[1];
        // Both values were boolean
        if (bool.TryParse(first, out var firstBool) && bool.TryParse(second, out _))
        {
            Logger.Log(firstBool ? "Running analysis on current directory" : "Running transformation on current directory");
            // Use the first boolean value
            MoqToNSubstituteConverter.Convert("", firstBool);
        }
        // First value is not boolean but the second one is
        else if (!bool.TryParse(first, out _) && bool.TryParse(second, out var secondBool))
        {
            Logger.Log($"Running analysis on: {first}");
            MoqToNSubstituteConverter.Convert(first, secondBool);
        }
        // Neither value is a boolean
        else if (!bool.TryParse(first, out _) && !bool.TryParse(second, out _))
        {
            Logger.Log($"Running analysis on: {first}");
            MoqToNSubstituteConverter.Convert(first);
        }
        else
        {
            Logger.Log(firstBool ? $"Running analysis on: {second}" : $"Running transformation on: {second}");
            MoqToNSubstituteConverter.Convert(second, firstBool);
        }
        break;
}
