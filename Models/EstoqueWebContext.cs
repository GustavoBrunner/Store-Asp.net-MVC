using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EstoqueWeb.Models{
    public class EstoqueWebContext: DbContext{
        public DbSet<CategoriaModel> Categorias { get; set; }
        public DbSet<ProdutoModel> Produtos { get; set; }
        public DbSet<ClienteModel> Clientes { get; set; }
        public DbSet<PedidoModel> Pedidos { get; set; }

        public DbSet<ItemPedidoModel> ItemsPedidos { get; set; }
        public EstoqueWebContext(DbContextOptions<EstoqueWebContext> options ) :base(options){

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Código opcional, serve somente para mudar o nome da tabela do plural para o singular
            modelBuilder.Entity<CategoriaModel>().ToTable("Categoria");
            /* CADA TABELA NOVA ADICIONADA AO BANCO DE DADOS, O BANCO DE DADOS DEVE SER ATUALIZADO
            NO TERMINAL com os mesmos comandos usados na criação do banco */
            modelBuilder.Entity<ProdutoModel>().ToTable("Produto");


            /* Configuração na tabela de cliente model, indicando que cliente model tem uma relação de
            um para muitos com endereço (um cliente pode ter muitos enderecos. Tal que endereco tem uma
            chave estrangeira que é o id do usuário, ou seja, o id de quem possui aquele endereço. E também
            Endereco possuir uma chave primário composta, dada pelo Id do usuário, e pelo id do endereco
            Ou seja, o usuário 1 pode ter o endereco 1-2-3-4, o usuário 2 pode ser o endereco 5-6-7-8, etc) */
            modelBuilder.Entity<ClienteModel>()
                .OwnsMany(c => c.Enderecos, e => {
                    e.WithOwner().HasForeignKey("IdUsuario");
                    e.HasKey("IdUsuario", "IdEndereco");
                });
            /* Pegamos uma propriedade de um modelo, no caso DataCadastro, e definimos um valor
            default para ela, usando uma função do sqlite. Para outros bancos de dados, é possível usar
            a função "getTime(). PropertySaveBehaviour.Ignore define que não há chances de essa propriedade
            salvar nenhum tipo de data, ou seja, os dados serão o mesmo dos definidos inicialmente */
            modelBuilder.Entity<UsuarioModel>().Property(u => u.DataCadastro)
                .HasDefaultValueSql("datetime('now', 'localtime', 'start of day')")
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            /* Da mesma maneira, aqui pegamos a propriedade Estoque, de todos os produtos
            e definimos o valor padrão dela para 0. Esse valor é sobrescrito na hora de cadastrar um produto
            Porém se o valor não for passado, ele será 0 */
            modelBuilder.Entity<ProdutoModel>().Property(p => p.Estoque)
                .HasDefaultValue(0);
            /* Se PedidoModel possuir um endereço de entrega, esse pedido não tem a necessidade
            de possuir os dados IdEndereco e Selecionado. Então nós ignoramos, na tabela de Pedido tais
            atributos. Depois disso o endereco em questão será passado para a tabela de Pedido no banco de
            dados */
            modelBuilder.Entity<PedidoModel>()
                .OwnsOne(p => p.EnderecoEntrega, e => {
                    e.Ignore(e => e.IdEndereco);
                    e.Ignore(e => e.Selecionado);
                    e.ToTable("Pedido");
                });

            /* Definição de uma chave composta para uma entidade. Usamos o método "haskey" e passamos um
            new {chave1, chave2} */
            modelBuilder.Entity<ItemPedidoModel>()
                .HasKey(ip => new {ip.IdPedido, ip.IdProduto});
            
        }


    }
}