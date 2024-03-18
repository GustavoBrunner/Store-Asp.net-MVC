using EstoqueWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EstoqueWeb.Controllers{
    public class ClienteController : Controller{
        private readonly EstoqueWebContext _context;

        public ClienteController(EstoqueWebContext context)
        {   
            this._context = context;
        }
        public async Task<IActionResult> Index(){
            /* Método que retornará a lista de clientes. Como ela será chamada uma vez a cada requisição,
            não há a necessidade de ficar verificando se ela foi alterada ou não. Logo, usamos o método
            .AsNoTracking() */
            return View(await _context.Clientes
                .OrderBy(x => x.Nome)
                .AsNoTracking()
                .ToListAsync());
        }
        [HttpGet]
        public async Task<IActionResult> Cadastrar(int? id){
            if(id.HasValue){
                if(ClientExists(id.Value)){
                    var cliente = _context?.Clientes.Single(x => x.IdUsuario == id.Value);
                    return View(cliente);
                }
                else{
                    TempData["mensagem"] = MensagemModel.Serializar("Cliente não encontrado.", 
                        MensagemTipo.Erro);
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(new ClienteModel());
        }
        [HttpPost]
        public async Task<IActionResult> Cadastrar(int? id, [FromForm] ClienteModel cliente){
            if(ModelState.IsValid){
                if(id.HasValue){
                    if(ClientExists(id.Value)){
                        _context.Clientes.Update(cliente);
                        /* Esse código indica para o banco de dados que a senha não deve ser modificada.
                        Caso esse código não seja escrito, a senha de usuário será sobrescrita com um
                        campo nulo, uma vez que agora ela aceita valores nulos */
                        _context.Entry(cliente).Property(x => x.Senha).IsModified = false;
                        if(await _context.SaveChangesAsync() > 0){
                            TempData["mensagem"] = MensagemModel.Serializar("Cliente atualizado com sucesso");
                        }
                        else{
                            TempData["mensagem"] = MensagemModel.Serializar("Não foi possível atualizar o cliente!",
                                MensagemTipo.Erro);
                        }
                    }
                    else{
                        TempData["mensagem"] = MensagemModel.Serializar("Esta cliente não existe!",
                            MensagemTipo.Erro);
                    }   
                }
                else{
                    _context.Clientes.Add(cliente);
                    if(await _context.SaveChangesAsync() > 0){
                        TempData["mensagem"] = MensagemModel.Serializar("Cliente adicionado com sucesso");
                    }
                    else{
                        TempData["mensagem"] = MensagemModel.Serializar("Não foi possível adicionar o cliente!",
                            MensagemTipo.Erro);
                    }
                }
            }
            else{
                return View(cliente);
            }
            return RedirectToAction("Index");

        }
        
        [HttpGet]
        public async Task<IActionResult> Excluir(int? id){
            if( id.HasValue ) {
                if( ClientExists( id.Value ) ){
                    var cliente = _context.Clientes.Single(c => c.IdUsuario == id);
                    return View(cliente);
                }
            }
            else{
                TempData["mensagem"] = MensagemModel.Serializar("Cliente não encontrado!", 
                    MensagemTipo.Erro);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(ClienteModel clienteModel){
            var cliente = await _context.Clientes.FindAsync(clienteModel.IdUsuario);
            if(cliente != null){
                _context.Clientes.Remove(cliente);
                if(await _context.SaveChangesAsync() > 0){
                    TempData["mensagem"] = MensagemModel.Serializar("Cliente excluído com sucesso!");
                }
                else{
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir cliente",
                        MensagemTipo.Erro);
                }
            }
            return RedirectToAction("Index");
        }
        
        private bool ClientExists(int id){
            return _context.Clientes.Any(c => c.IdUsuario == id);
        }
    }
}