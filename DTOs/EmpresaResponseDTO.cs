namespace CadastroEmpresaApi.DTOs
{
    public class EmpresaResponseDTO
    {
        public string NomeEmpresa { get; set; }
        public string NomeFantasia { get; set; }
        public string CNPJ { get; set; }
        public string Situacao { get; set; }
        public DateTime Abertura { get; set; }
        public string Tipo { get; set; }
        public string NaturezaPolitica { get; set; }
        public string AtividadePrincipal { get; set; }
        public string Logradouro { get; set; }
        public int Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Municipio { get; set; }
        public string UF { get; set; }
        public string CEP { get; set; }
        public int UsuarioId { get; set; }
    }
}
