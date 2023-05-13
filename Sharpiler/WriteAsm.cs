using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace Sharpiler;

public static class WriteAsm
{
    private static string? _outputPath;
    private static readonly string ProjectPath = Directory.GetCurrentDirectory();
    private static readonly string HeaderPath = Path.Combine(ProjectPath, "AsmTemplate/header_template.asm");
    private static readonly string FooterPath = Path.Combine(ProjectPath, "AsmTemplate/footer_template.asm");

    public static void Initialize(string filename)
    {
        _outputPath = Path.Combine(ProjectPath, filename);
        WriteHeader();
    }

    private static void WriteHeader()
    {
        if (_outputPath == null) throw new Exception("Output path not set");
        File.WriteAllText(_outputPath, File.ReadAllText(HeaderPath));
    }

    public static void Write(string asmLine)
    {
        if (_outputPath == null) throw new Exception("Output path not set");
        File.AppendAllText(_outputPath, asmLine + Environment.NewLine);
    }

    public static void WriteFooter()
    {
        if (_outputPath == null) throw new Exception("Output path not set");
        File.AppendAllText(_outputPath, File.ReadAllText(FooterPath));
    }
}