using System.Text;

// CNX.CodeGen - Công cụ chuyên viết tool (scaffolding CLI) cho dự án CNX_Counting.
// Tuân theo docs/cnx-agent-spec.json (codeGenTool). Sinh DTO/skeleton theo chuẩn dự án.
//
// Cách dùng:
//   dotnet run --project src/Tools/CNX.CodeGen -- dto --name Customer --fields "Name:string,Code:string,IsActive:bool" [--out <thư mục>]
//   dotnet run --project src/Tools/CNX.CodeGen -- help

return CommandRouter.Run(args);

internal static class CommandRouter
{
    public static int Run(string[] args)
    {
        if (args.Length == 0 || args[0] is "help" or "-h" or "--help")
        {
            PrintHelp();
            return 0;
        }

        var command = args[0].ToLowerInvariant();
        var options = ParseOptions(args.Skip(1));

        switch (command)
        {
            case "dto":
                return GenerateDto(options);
            default:
                Console.Error.WriteLine($"Lệnh không hợp lệ: '{command}'. Chạy 'help' để xem hướng dẫn.");
                return 1;
        }
    }

    private static int GenerateDto(IReadOnlyDictionary<string, string> options)
    {
        if (!options.TryGetValue("name", out var name) || string.IsNullOrWhiteSpace(name))
        {
            Console.Error.WriteLine("Thiếu --name. Ví dụ: dto --name Customer --fields \"Name:string,Code:string\"");
            return 1;
        }

        options.TryGetValue("fields", out var fieldsRaw);
        var code = DtoTemplate.Build(name, fieldsRaw ?? string.Empty);

        if (options.TryGetValue("out", out var outDir) && !string.IsNullOrWhiteSpace(outDir))
        {
            Directory.CreateDirectory(outDir);
            var path = Path.Combine(outDir, $"{name}Dto.cs");
            File.WriteAllText(path, code);
            Console.WriteLine($"Đã sinh: {path}");
        }
        else
        {
            Console.WriteLine(code);
        }

        return 0;
    }

    private static Dictionary<string, string> ParseOptions(IEnumerable<string> args)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        string? key = null;
        foreach (var arg in args)
        {
            if (arg.StartsWith("--", StringComparison.Ordinal))
            {
                key = arg[2..];
                result[key] = string.Empty;
            }
            else if (key is not null)
            {
                result[key] = arg;
                key = null;
            }
        }

        return result;
    }

    private static void PrintHelp()
    {
        Console.WriteLine("CNX.CodeGen - Công cụ scaffolding cho CNX_Counting");
        Console.WriteLine();
        Console.WriteLine("Lệnh:");
        Console.WriteLine("  dto --name <Tên> --fields \"Field:type,...\" [--out <thư mục>]");
        Console.WriteLine("  help");
        Console.WriteLine();
        Console.WriteLine("Ví dụ:");
        Console.WriteLine("  dto --name Customer --fields \"Name:string,Code:string,IsActive:bool\"");
    }
}

internal static class DtoTemplate
{
    public static string Build(string name, string fieldsRaw)
    {
        var sb = new StringBuilder();
        sb.AppendLine("namespace CNX.Application.DTOs;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// DTO cho {name}. Sinh tự động bởi CNX.CodeGen.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed class {name}Dto");
        sb.AppendLine("{");

        foreach (var field in ParseFields(fieldsRaw))
        {
            var initializer = field.Type == "string" ? " = string.Empty;" : string.Empty;
            sb.AppendLine($"    public {field.Type} {field.Name} {{ get; set; }}{initializer}");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    private static IEnumerable<(string Name, string Type)> ParseFields(string fieldsRaw)
    {
        if (string.IsNullOrWhiteSpace(fieldsRaw))
        {
            yield break;
        }

        foreach (var part in fieldsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var pair = part.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var fieldName = pair[0];
            var fieldType = pair.Length > 1 ? pair[1] : "string";
            yield return (fieldName, fieldType);
        }
    }
}
