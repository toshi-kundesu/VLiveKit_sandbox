using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VLiveKit.Release
{
    public static class UnityPackageExporter
    {
        public static void Export()
        {
            var sourcePath = GetRequiredArgument("-exportPackageSourcePath").Replace('\\', '/').TrimEnd('/');
            var outputPath = GetRequiredArgument("-exportPackageOutputPath");
            var includeDependencies = GetBoolArgument("-exportPackageIncludeDependencies", false);

            if (!AssetDatabase.IsValidFolder(sourcePath) && AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(sourcePath) == null)
            {
                throw new ArgumentException($"Export source path was not found in the AssetDatabase: {sourcePath}");
            }

            var outputDirectory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var options = ExportPackageOptions.Recurse;
            if (includeDependencies)
            {
                options |= ExportPackageOptions.IncludeDependencies;
            }

            Debug.Log($"Exporting unitypackage: {sourcePath} -> {outputPath}");
            AssetDatabase.ExportPackage(sourcePath, outputPath, options);

            if (!File.Exists(outputPath))
            {
                throw new FileNotFoundException($"Unitypackage was not created: {outputPath}");
            }
        }

        private static string GetRequiredArgument(string name)
        {
            var value = GetArgument(name);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"Missing required argument: {name}");
            }

            return value;
        }

        private static bool GetBoolArgument(string name, bool defaultValue)
        {
            var value = GetArgument(name);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : bool.Parse(value);
        }

        private static string GetArgument(string name)
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == name)
                {
                    return args[i + 1];
                }
            }

            return null;
        }
    }
}
