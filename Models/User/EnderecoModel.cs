using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EstoqueWeb.Models{
    /* Cria uma relação de pertencimento com outra classe. Ou seja, alguém vai POSSUIR um ou mais
    endereços. Isso servirá tanto para os clientes, quanto para os pedidos. Os dados desses endereços
    ficarão armazenados na mesma tabela que o cliente, no banco de dados, caso não seja mapeado para outra
    tabela */
    [Owned, Table("Endereco")]
    public class EnderecoModel {
        
        public int IdEndereco { get; set; }

        [Required]
        public string Logradouro { get; set; }

        [Required]
        public string Numero { get; set; }

        public string? Complemento { get; set; }

        [Required]
        public string Bairro { get; set; }

        [Required]
        public string Cidade { get; set; }

        [Required, Column(TypeName = "char(2)")]
        public string Estado { get; set; }

        [Required, Column(TypeName = "char(9)")]
        public string CEP { get; set; }

        public bool Selecionado { get; set; }

        public string? Referencia { get; set; }
        [NotMapped]
        
        public string EnderecoCompleto { get => 
            $"{Logradouro}, {Numero}, {Complemento}, {Bairro} {Cidade}, {Estado}, CEP {CEP}"; }
    }
}