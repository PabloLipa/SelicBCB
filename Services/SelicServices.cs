using SelicBCB___Pablo_Lipa.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SelicBCB___Pablo_Lipa.Services
{
    public class SelicServices
    {

        public readonly HttpClient _httpClient;
        public const string LinkURLAPI = "https://api.bcb.gov.br/dados/serie/bcdata.sgs.4390/dados?formato=json";


        public SelicServices()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<SelicBC>> GetDATAfromSelic()
        {
            try
            {
                HttpResponseMessage getResponse = await _httpClient.GetAsync(LinkURLAPI);
                getResponse.EnsureSuccessStatusCode();



                string jsonResponse = await getResponse.Content.ReadAsStringAsync();
                List<SelicBC> selicData = JsonSerializer.Deserialize<List<SelicBC>>(jsonResponse);
                return selicData;

            }
            catch (HttpRequestException errorHttp)
            {
                Console.WriteLine("Erro na conexao com a API: {0}",errorHttp.Message);
                return null;
            }
            catch (JsonException errorJson)
            {
                Console.WriteLine("Erro ao serializar os dados no json:{0}",errorJson.Message);
                return null;
            }
            catch (Exception error) {
                Console.WriteLine("Erro inesperado: {0}",error.Message);
                return null;
            }


        }

    }
}
