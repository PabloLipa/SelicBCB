using SelicBCB___Pablo_Lipa.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SelicBCB___Pablo_Lipa.Objects
{
    public class SelicBC
    {
        [JsonPropertyName("data")]
        public string data {  get; set; }

        [JsonPropertyName("valor")]
        [JsonConverter(typeof(ConverterFloatJson))]
        public float valor { get; set; }
    }
}
