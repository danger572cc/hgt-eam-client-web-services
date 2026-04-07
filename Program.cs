using HGT.EAM.Client.Configuration;
using HGT.EAM.Client.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace HGT.EAM.Client;

internal sealed class Program
{
    private static async Task<int> Main(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
                       ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") 
                       ?? "Development";

        var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .Build();

        var provider = Bootstrapper.Build(config);

        try
        {
            Log.Information("Aplicación iniciada");

            var logger       = provider.GetRequiredService<ILogger<Program>>();
            var provisionSvc = provider.GetRequiredService<ProvisionSyncService>();
            var auditSvc     = provider.GetRequiredService<PurchaseOrderAuditSyncService>();

            var apiSettings = config.GetSection("ApiSettings").Get<ApiSettings>() 
                              ?? throw new InvalidOperationException("Falta ApiSettings en appsettings.json");
            var credentialsDict = apiSettings.Credentials ?? new Dictionary<string, ApiCredentials>();

            bool continueRunning = true;

            while (continueRunning)
            {
                // Pantalla 1: Organización
                DrawHeader("SELECCIÓN DE ORGANIZACIÓN");
                
                if (credentialsDict.Count == 0)
                {
                    ShowError("No hay organizaciones configuradas en appsettings.json.");
                    break;
                }

                Console.WriteLine("  Por favor, seleccione la organización a consultar:\n");
                var orgKeys = credentialsDict.Keys.ToList();
                for (int i = 0; i < orgKeys.Count; i++)
                {
                    WriteOption((i + 1).ToString(), orgKeys[i]);
                }
                WriteOption("0", "Salir de la aplicación");
                Console.WriteLine();
                
                int selectedOrgIndex = ReadInteger($"  Opción (0-{orgKeys.Count}): ", 0, orgKeys.Count);
                if (selectedOrgIndex == 0) break;
                if (selectedOrgIndex < 0) continue;
                
                string selectedOrg = orgKeys[selectedOrgIndex - 1];
                var selectedCredentials = credentialsDict[selectedOrg];

                // Pantalla 2: Operación
                DrawHeader($"OPERACIÓN - {selectedOrg}");
                Console.WriteLine("  ¿Qué tipo de sincronización desea realizar?\n");
                WriteOption("1", "Grilla de provisiones");
                WriteOption("2", "Auditoría de órdenes de compra");
                WriteOption("0", "Volver atrás");
                Console.WriteLine();
                
                int operationOption = ReadInteger("  Opción: ", 0, 2);
                if (operationOption == 0) continue;
                if (operationOption < 1) continue;

                // Pantalla 3: Periodo
                string operationName = operationOption == 1 ? "GRILLA PROVISIONES" : "AUDITORÍA OC";
                DrawHeader($"PERIODO - {selectedOrg} - {operationName}");
                Console.WriteLine("  Seleccione el periodo de sincronización:\n");
                WriteOption("1", "Día anterior");
                WriteOption("2", "Mes anterior");
                WriteOption("3", "Mes actual");
                WriteOption("4", "Por mes y año");
                WriteOption("0", "Volver atrás");
                Console.WriteLine();

                int periodOption = ReadInteger("  Opción: ", 0, 4);
                if (periodOption == 0) continue;
                if (periodOption < 1) continue;

                string typeFilter = periodOption switch
                {
                    1 => "1",
                    2 => "2",
                    3 => "3",
                    4 => "5",
                    _ => "5"
                };

                int month = DateTime.UtcNow.Month;
                int year = DateTime.UtcNow.Year;

                if (periodOption == 4)
                {
                    // Pantalla 4: Parámetros Mes/Año
                    DrawHeader($"PARÁMETROS - {selectedOrg} - {operationName}");
                    month = ReadInteger("  Mes (1-12)         : ", 1, 12);
                    if (month < 0) continue;

                    year = ReadInteger("  Año (ej. 2025)     : ", 2000, 2100);
                    if (year < 0) continue;
                }

                int pageSize = ReadInteger("  Tamaño de página   : ", 1, 5000);
                if (pageSize < 0) continue;

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("  ======================================================\n");
                Console.ResetColor();

                if (operationOption == 1)
                {
                    // Grilla de provisiones
                    var query = new ProvisionQuery
                    {
                        Credentials = selectedCredentials,
                        TypeFilter = typeFilter,
                        Month = month,
                        Year  = year,
                        Page  = 1,
                        PageSize = pageSize
                    };

                    logger.LogInformation("[{Org}] Iniciando grilla de provisiones ({Filter})", selectedOrg, typeFilter);
                    var (totalRecords, totalPages, inserted) = await provisionSvc.FetchAndPersistAllAsync(query);

                    Console.WriteLine();
                    ShowSuccess($"Grilla completada [{selectedOrg}] — Registros: {totalRecords}, Páginas: {totalPages}, Insertados: {inserted}");
                    logger.LogInformation("[{Org}] Grilla completada: TotalRecords={R}, TotalPages={P}, Inserted={I}", selectedOrg, totalRecords, totalPages, inserted);
                }
                else
                {
                    // Auditoría de órdenes de compra
                    var query = new PurchaseOrderAuditQuery
                    {
                        Credentials = selectedCredentials,
                        TypeFilter = typeFilter,
                        Month = month, 
                        Year = year,
                        Page = 1,
                        PageSize = pageSize
                    };

                    logger.LogInformation("[{Org}] Iniciando auditoría de órdenes de compra ({Filter})", selectedOrg, typeFilter);
                    var (totalRecords, totalPages, inserted) = await auditSvc.FetchAndPersistAllAsync(query);

                    Console.WriteLine();
                    ShowSuccess($"Auditoría completada [{selectedOrg}] — Registros: {totalRecords}, Páginas: {totalPages}, Insertados: {inserted}");
                    logger.LogInformation("[{Org}] Auditoría completada: TotalRecords={R}, TotalPages={P}, Inserted={I}", selectedOrg, totalRecords, totalPages, inserted);
                }


                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("  Presione enter para continuar...");
                Console.ResetColor();
                Console.ReadLine();
            }

            Log.Information("Aplicación finalizada correctamente");
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Error fatal en la aplicación");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void DrawHeader(string title)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
        
        int totalSpaces = 56;
        int leftPadding = (totalSpaces - title.Length) / 2;
        int rightPadding = totalSpaces - title.Length - leftPadding;
        string centeredTitle = new string(' ', Math.Max(0, leftPadding)) + title + new string(' ', Math.Max(0, rightPadding));
        
        Console.WriteLine($"║{centeredTitle}║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  ⚠  {message}");
        Console.ResetColor();
    }

    private static void ShowSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  ✔ {message}");
        Console.ResetColor();
    }

    private static void WriteOption(string key, string description)
    {
        Console.Write("  ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"[{key}]");
        Console.ResetColor();
        Console.WriteLine($" {description}");
    }

    /// <summary>
    /// Reads an integer from the console within the specified range.
    /// Returns -1 if the user enters an invalid value, to return to the menu.
    /// </summary>
    private static int ReadInteger(string prompt, int min, int max)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(prompt);
        Console.ResetColor();
        
        var input = Console.ReadLine()?.Trim();
        if (int.TryParse(input, out var value) && value >= min && value <= max)
            return value;

        ShowError($"Valor inválido. Posibles valores: de {min} a {max}. Presione enter para reintentar.");
        Console.ReadLine();
        return -1;
    }
}
