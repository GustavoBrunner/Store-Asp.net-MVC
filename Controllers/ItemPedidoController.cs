using EstoqueWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EstoqueWeb.Controllers
{
    public class ItemPedidoController : Controller
    {
        private readonly EstoqueWebContext _context;

        public ItemPedidoController(EstoqueWebContext context)
        {
            this._context = context;
        }
        public async Task<IActionResult> Index(int? ped)
        {
            if (!ped.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não informado!",
                    MensagemTipo.Erro);
                return RedirectToAction(nameof(Index), "Cliente");
            }
            if (!_context.Pedidos.Any(p => p.IdPedido == ped))
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não existe!",
                    MensagemTipo.Erro);
                return RedirectToAction(nameof(Index), "Cliente");
            }
            /* como por padrão os bancos não incluem dados relacionados, por questão de desempenho,
            deve ser usado ou o método Collection.Load(), ou Include. Assim conseguir incluir os dados rela
            cionados a uma consulta */
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.ItensPedido.OrderBy(i => i.Produto.Nome))
                /* ThenInclude atua sobre o include anterior, como ele está carregando ItensPedido,
                o then include atuará em cima desses itens de pedido */
                .ThenInclude(i => i.Produto)
                //agora, verificamos o id do pedido para que esses dados sejam carregados somente
                //do pedido desejado, caso contrário, os dados de TODOS os pedidos serão carregados
                .FirstOrDefaultAsync(p => p.IdPedido == ped);
            ViewBag.Pedido = pedido;


            return View(pedido?.ItensPedido);

        }
        [HttpGet]
        public async Task<IActionResult> Cadastrar(int? ped, int? prod)
        {
            if (!ped.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não informado!",
                    MensagemTipo.Erro);
                return RedirectToAction(nameof(Index), "Cliente");
            }
            if (!_context.Pedidos.Any(p => p.IdPedido == ped))
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não existe!",
                    MensagemTipo.Erro);
                return RedirectToAction(nameof(Index), "Cliente");
            }

            var produtos = _context.Produtos
                .OrderBy(p => p.Nome)
            /* é possível pegar atributos específicos de um objeto, quantos quisermos. 
                Para isso usamos a palavra chave new, passando dentro delas todos os atributos que desejamos
                selecionar dentro do objeto. No caso, o id do produto, e uma concatenação de string com nome
                e preço do produto */
                .Select(p => new { p.IdProduto, NomePreco = $"{p.Nome} ({p.Preco:C})" })
                .AsNoTracking().ToList();
            var produtosSelectList = new SelectList(produtos, "IdProduto", "NomePreco");
            ViewBag.Produtos = produtosSelectList;

            if (prod.HasValue && ItemPedidoExists(ped.Value, prod.Value))
            {
                var itemPedido = await _context.ItemsPedidos
                .Include(i => i.IdProduto)
                .FirstOrDefaultAsync(i => i.IdPedido == ped && i.IdProduto == prod);
                return View(itemPedido);
            }
            return View(new ItemPedidoModel()
            { IdPedido = ped.Value, ValorUnitario = 0, Quantidade = 1 });

        }
        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromForm] ItemPedidoModel itemPedido)
        {
            if (!ModelState.IsValid)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Dados inválidos!",
                    MensagemTipo.Erro);
                var produtos = _context.Produtos
                    .OrderBy(p => p.Nome)
                    .Select(p => new { p.IdProduto, NomePreco = $"{p.Nome} ({p.Preco:C})" })
                    .AsNoTracking().ToList();
                var produtosSelectList = new SelectList(produtos, "IdProduto", "NomePreco");
                ViewBag.Produtos = produtosSelectList;
                return View(itemPedido);
            }
            if (itemPedido.IdPedido > 0)
            {
                var produto = await _context.Produtos.FindAsync(itemPedido.IdProduto);

                itemPedido.ValorUnitario = produto.Preco;
                if (ItemPedidoExists(itemPedido.IdPedido, itemPedido.IdProduto))
                {
                    _context.ItemsPedidos.Update(itemPedido);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel
                            .Serializar("Item atualizado ao pedido com sucesso!");
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Não foi possível adicionar nenhum item!",
                            MensagemTipo.Erro);
                    }
                    //return RedirectToAction(nameof(Index), new { ped = itemPedido.IdPedido} );
                }
                else
                {
                    _context.ItemsPedidos.Add(itemPedido);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel
                            .Serializar("Item adicionado ao pedido com sucesso!");
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Não foi possível adicionar item!",
                            MensagemTipo.Erro);
                    }
                    //return RedirectToAction(nameof(Index), new { ped = itemPedido.IdPedido} );
                }
                RecalcularValorTotal(itemPedido);
                return RedirectToAction(nameof(Index), new { ped = itemPedido.IdPedido });
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não informado!",
                    MensagemTipo.Erro);
                return RedirectToAction(nameof(Index), "Cliente");
            }

        }

        

        [HttpGet]
        public async Task<IActionResult> Excluir(int? ped, int? prod)
        {
            if (!ped.HasValue || !prod.HasValue)
            {
                
                return RedirectToAction(nameof(Index), "Cliente");
            }
            if (!ItemPedidoExists(ped.Value, prod.Value))
            {

                return RedirectToAction(nameof(Index), "Cliente");
            }

            var itemPedido = await _context.ItemsPedidos.FindAsync(ped, prod);
            _context?.Entry(itemPedido).Reference(i => i.Produto).Load();
            return View(itemPedido);
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(int ped, int prod)
        {
            var itemPedido = await _context.ItemsPedidos.FindAsync(ped, prod);
            if (itemPedido == null)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Item de pedido não existe",
                    MensagemTipo.Erro);
                    
                return RedirectToAction(nameof(Index), new { ped = ped });
            }
            _context.ItemsPedidos.Remove(itemPedido);
            if (await _context.SaveChangesAsync() > 0)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Item excluído com sucesso!");
                RecalcularValorTotal(itemPedido);
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir Item",
                    MensagemTipo.Erro);
                return RedirectToAction(nameof(Index), new { ped = ped });
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ItemPedidoExists(int ped, int prod)
        {
            return _context.ItemsPedidos.Any(c => c.IdPedido == ped && c.IdProduto == prod);
        }
        private async void RecalcularValorTotal(ItemPedidoModel itemPedido)
        {
            var pedido = await _context.Pedidos.FindAsync(itemPedido.IdPedido);
                pedido.ValorTotal = _context.ItemsPedidos
                    .Where(i => i.IdPedido == itemPedido.IdPedido)
                    .Sum(i => i.ValorUnitario * i.Quantidade);
                await _context.SaveChangesAsync();
        }
    }
}