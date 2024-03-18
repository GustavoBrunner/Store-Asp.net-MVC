using EstoqueWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EstoqueWeb.Controllers
{
    public class PedidoController : Controller
    {
        private readonly EstoqueWebContext _context;

        public PedidoController(EstoqueWebContext context)
        {
            this._context = context;
        }
        public async Task<IActionResult> Index(int? cid)
        {
            if (cid == null)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Nenhum cliente informado!",
                    MensagemTipo.Erro);
                RedirectToAction(nameof(Index), "Cliente");
            }

            var cliente = await _context.Clientes.FindAsync(cid);
            if (cliente == null)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Não foi encontrar o cliente!",
                    MensagemTipo.Erro);
                RedirectToAction(nameof(Index), "Cliente");
            }

            var pedidos = await _context.Pedidos.Where(c => c.IdCliente == cid)
                .OrderByDescending(p => p.IdPedido).AsNoTracking()
                .ToListAsync();

            ViewBag.Cliente = cliente;
            return View(pedidos);

        }
        [HttpGet]
        public async Task<IActionResult> Cadastrar(int? cid)
        {
            if (!cid.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Nenhum cliente informado!",
                    MensagemTipo.Erro);
                return RedirectToAction(nameof(Index), "Cliente");
            }
            var cliente = await _context.Clientes.FindAsync(cid);
            if (cliente == null)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Nenhum cliente encontrado!",
                    MensagemTipo.Erro);
                return RedirectToAction(nameof(Index), "Cliente");
            }
            //aqui damos uma entrada no banco de dados passando um cliente.
            //em seguida, carregamos todos os pedidos vinculados àquele cliente.
            //Lembrando, coleções não são sempre carregadas em bancos de dados, por questão de performances
            _context?.Entry(cliente).Collection(c => c.Pedidos).Load();
            PedidoModel pedido = null;
            //verificação se algum dos pedidos do banco de dados tem id igual ao do cliente em questão
            //e se esse pedido ainda não possui data de pedido, ou seja, se ele ainda não foi fechado
            if (_context.Pedidos.Any(p => p.IdCliente == cid && !p.DataPedido.HasValue))
            {
                //capturamos esse pedido
                pedido = await _context?.Pedidos?.FirstOrDefaultAsync(p => p.IdCliente == cid
                    && !p.DataPedido.HasValue);
            }
            else
            {
                //caso não existe nenhum pedido em aberto, abrimos um novo, passando algums parâmetros
                //já. Como IdCliente e um valor inicial para o valor total
                pedido = new PedidoModel { IdCliente = cid.Value, ValorTotal = 0 };
                //adicionamos o pedido para a lista de pedidos do cliente
                cliente?.Pedidos?.Add(pedido);
                //salvamos no banco de dados
                await _context.SaveChangesAsync();
            }
            /* redirecionamos o cliente para a ação index do controlador item pedido, passando
            um parâmetro QUE DEVE SER COMPATÍVEL, EM NOME INCLUSIVE ao parâmetro da ação que lá está */
            return RedirectToAction(nameof(Index), "ItemPedido", new { ped = pedido?.IdPedido });

        }
        [HttpPost]
        public async Task<IActionResult> Cadastrar(int? id, [FromForm] PedidoModel category)
        {
            if (ModelState.IsValid)
            {
                if (id.HasValue)
                {
                    if (PedidoExists(id.Value))
                    {
                        _context.Pedidos.Update(category);
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Pedido atualizada com sucesso");
                        }
                        else
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Não foi possível atualizar a categoria!",
                                MensagemTipo.Erro);
                        }
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Esta categoria não existe!",
                            MensagemTipo.Erro);
                    }
                }
                else
                {
                    _context.Pedidos.Add(category);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Pedido adicionada com sucesso");
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Não foi possível adicionar a categoria!",
                            MensagemTipo.Erro);
                    }
                }
            }
            else
            {
                return View(category);
            }
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Excluir(int? ped)
        {
            if (!ped.HasValue)
            {

                return RedirectToAction(nameof(Index));
            }      
            if(!PedidoExists(ped.Value)) {

                return RedirectToAction(nameof(Index));
            }

            var pedido = await _context?.Pedidos?
                .Include(p => p.Cliente)
                .Include(p => p.ItensPedido)
                .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.IdPedido == ped);

            
            return View(pedido);
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(int ped)
        {
            var pedido = await _context.Pedidos.FindAsync(ped);
            if(pedido == null){
                TempData["mensagem"] = MensagemModel.Serializar("Não foi possível localizar pedido",
                    MensagemTipo.Erro);
                return RedirectToAction(nameof(Index), new {cid = pedido?.IdCliente});
            }
            _context.Pedidos.Remove(pedido);
            if (await _context.SaveChangesAsync() > 0)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido excluído com sucesso!");
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir pedido",
                    MensagemTipo.Erro);
            }
            return RedirectToAction("Index");
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(c => c.IdPedido == id);
        }
    }
}