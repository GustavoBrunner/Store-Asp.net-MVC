using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstoqueWeb.Models{
    public class ProdutoModel{
        [Key]
        public int IdProduto { get; set; }
        [Required, MaxLength(128)]
        public string Nome { get; set; }
        
        public int Estoque { get; set; }
        
        public double Preco { get; set; }
        /* Id que vai linkar o produto à categoria que ele deve ser vinculado. 
        Esta chave sera representada pela marcação ForeignKey, já que a chave vinculada será
        estrangeira. Passando como parâmetro o nome da propriedade onde essa categoria está armazenada */
        
        public int IdCategoria { get; set; }
        /* Quando a relação dos dados é de um para muitos, como é o caso. Uma categoria possui
        muitos produtos, nós indicamos que o IdCategoria será pego do objeto categoria */
        [ForeignKey("IdCategoria"), ]
        public CategoriaModel? Categoria { get; set; }
    }
}