using System.Text.Json.Serialization;

public class EmpresaCreateDTO
{
    [JsonPropertyName("nome")]
    public string NomeEmpresa { get; set; }

    [JsonPropertyName("fantasia")]
    public string NomeFantasia { get; set; }

    [JsonPropertyName("cnpj")]
    public string CNPJ { get; set; }

    [JsonPropertyName("situacao")]
    public string Situacao { get; set; }

    [JsonPropertyName("abertura")]
    public string Abertura { get; set; }

    [JsonPropertyName("tipo")]
    public string Tipo { get; set; }

    [JsonPropertyName("natureza_juridica")]
    public string NaturezaPolitica { get; set; }

    [JsonPropertyName("atividade_principal")]
    public List<AtividadeDTO> AtividadePrincipal { get; set; }

    [JsonPropertyName("logradouro")]
    public string Logradouro { get; set; }

    [JsonPropertyName("numero")]
    public string Numero { get; set; }

    [JsonPropertyName("complemento")]
    public string Complemento { get; set; }

    [JsonPropertyName("bairro")]
    public string Bairro { get; set; }

    [JsonPropertyName("municipio")]
    public string Municipio { get; set; }

    [JsonPropertyName("uf")]
    public string UF { get; set; }

    [JsonPropertyName("cep")]
    public string CEP { get; set; }
}

public class AtividadeDTO
{
    [JsonPropertyName("text")]
    public string Text { get; set; }
}
