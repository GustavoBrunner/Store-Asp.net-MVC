using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstoqueWeb.Models{
    [Table("Cliente")]
    public class ClienteModel : UsuarioModel{
        //indica o tipo que será usado para definir o CPF, no caso char, com tamanho 14. 
        //Isso define qual será o tipo no banco de dados, em vez de deixaro  padrão gerado por ele
        [Required, Column(TypeName = "char(14)")]
        public string CPF { get; set; }
        public DateTime DataNascimento { get; set; }

        public ICollection<EnderecoModel>? Enderecos { get; set; }
        //retorna a idade do cliente, fazendo o ano atual, subtraído pelo ano de nascimento, dividido
        //por 365.2425, e isso castado para int, para dar um valor inteiro. Como essa propriedade
        // é calculada, ela não deve ser mapeada pelo entity frameword. Logo devemos anotar ela como "not mapped"
        [NotMapped]
        public int Idade { get => (int)Math.Floor(
            DateTime.Now.Subtract(DataNascimento).TotalDays / 365.2425); }

        public ICollection<PedidoModel>? Pedidos { get; set; }
    }
}