using SelicBCB___Pablo_Lipa.DbConetion;
using SelicBCB___Pablo_Lipa.Objects;
using SelicBCB___Pablo_Lipa.Services;
using System.Globalization;
using System.Text;


class Program
{
    private static List<SelicBC> _selicData;
    private static readonly SelicServices _selicService = new SelicServices();
    //private static readonly DBConect DbManager = new DBConect();
    static async Task Main(string[] args)
    {
        Console.WriteLine("Iniciando aplicação SelicBC...");
        Console.WriteLine("Carregando dados da API do Banco Central...");


        _selicData = await _selicService.GetDATAfromSelic();

        if (_selicData == null || !_selicData.Any())
        {
            Console.WriteLine("Não foi possível carregar os dados da Selic. A aplicação será encerrada.");
            return;
        }

        Console.WriteLine("Dados da Selic carregados com sucesso!");

        await MainMenu();
    }


    public static async Task MainMenu()
    {
        while (true)
        {
            Console.WriteLine("\n------------------------------" +
                              "\n--- Menu Principal SelicBC ---" +
                              "\n------------------------------" +
                              "\n 1: Consultar dados da Selic  " +
                              "\n 2: Calcular juros compostos"   +
                              "\n 3: Exportar dados para CSV"    +
                              "\n 4: Encerrar"                   +
                              "\n------------------------------" +
                              "\n---------By Pablo Lipa--------" +
                              "\n------------------------------");

            Console.Write("Escolha uma opção: ");

            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Consultar_Dados();
                    break;
                case "2":
                    CalculoJurosCompostos();
                    break;
                case "3":
                    ExportarCSV();
                    break;
                case "4":
                    Console.WriteLine("Aplicação encerrada!");
                    return;
                default:
                    Console.WriteLine("Opção inválida. Por favor, tente novamente.");
                    break;
            }
        }
    }


    private static void Consultar_Dados()
    {
        Console.WriteLine("\n--- Consulta de Dados da Selic ---");
        Console.WriteLine("Os dados disponíveis são do período de:");
        Console.WriteLine("De: {0} a {1}", _selicData.First().data, _selicData.Last().data);
        Console.Write("Deseja consultar um período específico? (S/N): ");
        string? choice = Console.ReadLine()?.ToUpper();

        if (choice == "S")
        {
            Console.Write("Digite a data de início (dd/MM/yyyy): ");
            string? startDateStr = Console.ReadLine();
            Console.Write("Digite a data de fim (dd/MM/yyyy): ");
            string? endDateStr = Console.ReadLine();

            if (DateTime.TryParseExact(startDateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate) &&
                DateTime.TryParseExact(endDateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
            {
                var filteredData = _selicData
                    .Where(s => DateTime.ParseExact(s.data, "dd/MM/yyyy", CultureInfo.InvariantCulture) >= startDate &&
                                DateTime.ParseExact(s.data, "dd/MM/yyyy", CultureInfo.InvariantCulture) <= endDate)
                    .OrderBy(s => DateTime.ParseExact(s.data, "dd/MM/yyyy", CultureInfo.InvariantCulture))
                    .ToList();

                if (filteredData.Any())
                {
                    Console.WriteLine("\nResultados da Consulta:");
                    foreach (var item in filteredData)
                    {
                        Console.WriteLine("Data: " + item.data + " , Valor: " + item.valor + "%");
                    }
                }
                else
                {
                    Console.WriteLine("Nenhum dado encontrado para o período especificado.");
                }
            }
            else
            {
                Console.WriteLine("Formato de data inválido. Use dd/MM/yyyy.");
            }
        }
        else
        {
            Console.WriteLine("\nÚltimos 10 registros da Selic:");
            foreach (var item in _selicData.TakeLast(10))
            {
                Console.WriteLine("Data: " + item.data + " , Valor: " + item.valor + "%");
            }
        }
    }


    private static void CalculoJurosCompostos()
    {
        Console.WriteLine("\n--- Cálculo de Juros Compostos ---");

        if (!LerValor("Informe o valor inicial (capital): ", out float capital) || capital <= 0)
        {
            Console.WriteLine("Valor inválido. Use um número positivo.");
            return;
        }

        if (!LerData("Informe a data de início do investimento (dd/MM/yyyy): ", out DateTime inicio) ||
            !LerData("Informe a data de fim do investimento (dd/MM/yyyy): ", out DateTime fim) ||
            inicio > fim)
        {
            Console.WriteLine("Datas inválidas.");
            return;
        }

        var selics = _selicData
            .Where(s =>
            {
                var data = DateTime.ParseExact(s.data, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                return data >= inicio && data <= fim;
            })
            .OrderBy(s => DateTime.ParseExact(s.data, "dd/MM/yyyy", CultureInfo.InvariantCulture))
            .ToList();

        if (!selics.Any())
        {
            Console.WriteLine("Sem dados da Selic para o período.");
            return;
        }

        float valorFinal = capital;
        foreach (var s in selics)
            valorFinal *= 1 + (s.valor / 100f / 365f);

        Console.WriteLine($"\nValor Original: {capital:C2}");
        Console.WriteLine($"Juros Acumulados: {valorFinal - capital:C2}");
        Console.WriteLine($"Valor Total Corrigido: {valorFinal:C2}");
    }


    private static bool LerValor(string mensagem, out float valor)
    {
        Console.Write(mensagem);
        return float.TryParse(Console.ReadLine(), NumberStyles.Currency, CultureInfo.InvariantCulture, out valor);
    }

    private static bool LerData(string mensagem, out DateTime data)
    {
        Console.Write(mensagem);
        return DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out data);
    }



    private static void ExportarCSV()
    {
        Console.WriteLine("\n--- Exportar Dados ---");
        Console.WriteLine("Exportar para arquivo CSV");

        string fileName = "selic_data";
        string filePath = $"{fileName}.csv";
        try
        {
            using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                sw.WriteLine("Data;Valor");
                foreach (var item in _selicData)
                {
                    sw.WriteLine($"{item.data};{item.valor.ToString(CultureInfo.InvariantCulture)}");
                }
            }
            Console.WriteLine($"Dados exportados com sucesso para {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao exportar para CSV: {ex.Message}");
        }

    }



    /*
    private static async Task SalvarDaAPIparaBD()
    {
        Console.WriteLine("Carregando dados da API do Banco Central...");

        _selicData = await _selicService.GetDATAfromSelic();

        if (!(_selicData?.Any() ?? false))
        {
            Console.WriteLine("Não foi possível carregar dados da Selic da API.");
            return;
        }

        Console.WriteLine("Dados carregados com sucesso!");

        try
        {
            await DbManager.GetSavedSelicDataAsync(_selicData);
            Console.WriteLine("Dados salvos no banco de dados local.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao salvar dados: {ex.Message}");
        }
    }*/

}
