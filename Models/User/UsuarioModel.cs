using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstoqueWeb.Models{
    [Table("Usuario")]
    public class UsuarioModel {
        [Key]
        public int IdUsuario { get; set; }
        [Required, MaxLength(128)]
        public string? Nome { get; set; }
        [Required, MaxLength(128)]
        public string? Email { get; set; }
        [MaxLength(128)]
        public string? Senha { get; set; }
        //Como essa propriedade será definida pelo banco de dados, colocamos ela como readonly
        [ReadOnly(true)]
        public DateTime? DataCadastro { get; }
    }
}