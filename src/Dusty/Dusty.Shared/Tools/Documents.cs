using System.ComponentModel;
using Microsoft.Extensions.AI;
using Serilog;

namespace Dusty.Shared.Tools;

public class Documents
{
    public static string DocumentsRoot = string.Empty;

    public static AIFunction[] Tools =>
    [
        AIFunctionFactory.Create(SaveContent, Functions.SaveContent),
        AIFunctionFactory.Create(GetContent, Functions.GetContent),
        AIFunctionFactory.Create(ListFiles, Functions.ListFiles)
    ];

    public static string FormatToolOutput(FunctionCallContent functionCall)
    {
        var toolName = functionCall.Name;
        var arguments = functionCall.Arguments;
        if (string.IsNullOrWhiteSpace(toolName)
            || arguments == null
            || arguments.Count == 0)
        {
            return string.Empty;
        }

        var filePath = arguments.TryGetValue("filePath", out var path) ? path?.ToString() : string.Empty;
        filePath ??= arguments.TryGetValue("directoryPath", out var dir) ? dir?.ToString() : string.Empty;

        return toolName switch
        {
            Functions.SaveContent => $"Saving content to file: {filePath}",
            Functions.GetContent => $"Retrieving content from file: {filePath}",
            Functions.ListFiles => $"Listing files in directory: {filePath}",
            _ => $"Tool '{toolName}' is not supported."
        };
    }

    [Description("Safely save's or updates the content of a file in the user's Documents vault.")]
    public static Task SaveContent(
        [Description("The path to the file in the Documents vault, relative to the vault root. For example, 'notes/my-note.md'.")]
        string filePath,
        [Description("The content to save in the file. If the file already exists, it will be updated with this content.")]
        string content)
    {
        Log.Information("Saving content to file: {FilePath}", filePath);
        return Task.CompletedTask;
    }

    [Description("Safely retrieves the content of a file from the user's Documents vault. Returns a JSON formatted {\"error\": \"string\"} if the file does not exist or an error occurs.")]
    public async static Task<string> GetContent(
        [Description("The path to the file in the Documents vault, relative to the vault root. For example, 'notes/my-note.md'.")]
        string filePath)
    {
        Log.Information("Retrieving content from file: {FilePath}", filePath);
        return "# This is my Title\n\nThis is the content of my document.";
    }

    [Description("Lists all Markdown files in the Documents vault or a specific directory within it.")]
    public static string[] ListFiles(
        [Description("The path to the directory in the Documents vault, relative to the vault root. For example, 'notes/'. If not provided, lists all files in the vault.")]
        string? directoryPath = null)
    {
        return
        [
            "notes/my-note.md",
            "notes/another-note.md",
            "archive/old-note.md",
            "projects/project1/README.md"
        ];

        // return;
        //
        // // Placeholder for actual implementation
        // var relativePath = directoryPath ?? string.Empty;
        // var filePath = Path.Combine(DocumentsRoot, relativePath);
        // var files = Directory.GetFiles(filePath, "*.md", SearchOption.AllDirectories)
        //     .Select(f => Path.GetRelativePath(DocumentsRoot, f))
        //     .ToArray();
        //
        // return Task.FromResult(files);
    }

    public static class Functions
    {
        public const string SaveContent = "Documents.SaveContent";
        public const string GetContent = "Documents.GetContent";
        public const string ListFiles = "Documents.ListFiles";
    }
}
