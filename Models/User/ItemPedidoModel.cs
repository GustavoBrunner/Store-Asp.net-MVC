using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EstoqueWeb.Models{
    [Table("ItemPedido")]
    public class ItemPedidoModel {
        //Como essa chave será definida por um objeto, nós definimos que ela não será
        //automaticamente gerada pelo banco de dados
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdPedido { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdProduto { get; set; }
        public int Quantidade { get; set; }

        public double ValorUnitario { get; set; }

        [ForeignKey("IdPedido")]
        public PedidoModel? Pedido { get; set; }

        [ForeignKey("IdProduto")]
        public ProdutoModel? Produto { get; set; }

        [NotMapped]
        public double ValorTotal { get => this.ValorUnitario * this.Quantidade; }
    }
}