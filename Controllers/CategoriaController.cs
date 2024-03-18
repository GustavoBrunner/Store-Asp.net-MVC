using EstoqueWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EstoqueWeb.Controllers{
    public class CategoriaController : Controller{
        private readonly EstoqueWebContext _context;

        public CategoriaController(EstoqueWebContext context)
        {   
            this._context = context;
        }
        public async Task<IActionResult> Index(){
            /* Método que retornará a lista de categorias. Como ela será chamada uma vez a cada requisição,
            não há a necessidade de ficar verificando se ela foi alterada ou não. Logo, usamos o método
            .AsNoTracking() */
            return View(await _context.Categorias
                .OrderBy(x => x.Nome)
                .AsNoTracking()
                .ToListAsync());
        }
        [HttpGet]
        public async Task<IActionResult> Cadastrar(int? id){
            if(id.HasValue){
                if(CategoryExists(id.Value)){
                    var categoria = _context?.Categorias.Single(x => x.IdCategoria == id.Value);
                    return View(categoria);
                }
            }
            return View(new CategoriaModel());
        }
        [HttpPost]
        public async Task<IActionResult> Cadastrar(int? id, [FromForm] CategoriaModel category){
            if(ModelState.IsValid){
                if(id.HasValue){
                    if(CategoryExists(id.Value)){
                        _context.Categorias.Update(category);
                        if(await _context.SaveChangesAsync() > 0){
                            TempData["mensagem"] = MensagemModel.Serializar("Categoria atualizada com sucesso");
                        }
                        else{
                            TempData["mensagem"] = MensagemModel.Serializar("Não foi possível atualizar a categoria!",
                                MensagemTipo.Erro);
                        }
                    }
                    else{
                        TempData["mensagem"] = MensagemModel.Serializar("Esta categoria não existe!",
                            MensagemTipo.Erro);
                    }   
                }
                else{
                    _context.Categorias.Add(category);
                    if(await _context.SaveChangesAsync() > 0){
                        TempData["mensagem"] = MensagemModel.Serializar("Categoria adicionada com sucesso");
                    }
                    else{
                        TempData["mensagem"] = MensagemModel.Serializar("Não foi possível adicionar a categoria!",
                            MensagemTipo.Erro);
                    }
                }
            }
            else{
                return View(category);
            }
            return RedirectToAction("Index");

        }
        
        [HttpGet]
        public async Task<IActionResult> Excluir(int? id){
            if( id.HasValue ) {
                if( CategoryExists( id.Value ) ){
                    var categoria = _context.Categorias.Single(c => c.IdCategoria == id);
                    return View(categoria);
                }
            }
            else{
                TempData["mensagem"] = MensagemModel.Serializar("Categoria não encontrada!", 
                    MensagemTipo.Erro);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(CategoriaModel categoriaModel){
            _context.Categorias.Remove(categoriaModel);
            if(await _context.SaveChangesAsync() > 0){
                TempData["mensagem"] = MensagemModel.Serializar("Objeto excluído com sucesso!");
            }
            else{
                TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir objeto",
                    MensagemTipo.Erro);
            }
            return RedirectToAction("Index");
        }
        
        private bool CategoryExists(int id){
            return _context.Categorias.Any(c => c.IdCategoria == id);
        }
    }
}