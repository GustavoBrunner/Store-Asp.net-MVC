using EstoqueWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EstoqueWeb.Controllers{
    public class EnderecoController : Controller{
        private readonly EstoqueWebContext _context;

        public EnderecoController(EstoqueWebContext context)
        {   
            this._context = context;
        }
        public async Task<IActionResult> Index(int? cid){
            if(cid.HasValue){
                var cliente = await _context.Clientes.FindAsync(cid);
                if(cliente != null){
                    /* Por padrão, o entity framework não carrega coleções, por questão de desempenho.
                    Então para que consigamos carregar uma coleção, devemos fazer isso de maneira explícita,
                    através do código listado abaixo */
                    ViewBag.Cliente = cliente;
                    _context.Entry(cliente).Collection(c => c.Enderecos).Load();
                    return View(cliente.Enderecos);
                }
                else{
                    TempData["mensagem"] = MensagemModel.Serializar("Cliente não encontrado!",
                        MensagemTipo.Erro);
                    /* RedirectToAction permite que passamos também o nome de um controller */
                    return RedirectToAction(nameof(Index), "Cliente");
                }
            }
            else{
                TempData["mensagem"] = MensagemModel.Serializar("Não foi possível mostrar os endereços!",
                    MensagemTipo.Erro);
                /* RedirectToAction permite que passamos também o nome de um controller */
                return RedirectToAction(nameof(Index), "Cliente");
            }
        }
        [HttpGet]
        //esse método recebe dois parâmetros, sendo um deles opcional, o eid, quando o eid for fornecido, significa que ocorrerá uma alteração
        //num endereço existente
        public async Task<IActionResult> Cadastrar(int? cid, int? eid){
            if(cid.HasValue){
                var cliente = await _context.Clientes.FindAsync(cid);
                if(cliente != null){
                    ViewBag.Cliente = cliente;
                    if(eid.HasValue){
                        _context.Entry(cliente).Collection(c => c.Enderecos).Load();
                        var endereco = cliente?.Enderecos?.FirstOrDefault(e => e.IdEndereco == eid);
                        if(endereco != null){
                            return View(endereco);
                        }
                        else{
                            TempData["mensagem"] = MensagemModel.Serializar("Nenhum endereço encontrado!",
                                MensagemTipo.Erro);
                        }
                    }
                    else{
                        return View(new EnderecoModel());
                    }   
                }
                else{
                    TempData["mensagem"] = MensagemModel.Serializar("Nenhum cliente encontrado!",
                        MensagemTipo.Erro);
                }
                return RedirectToAction(nameof(Index));
            }
            else{
                TempData["mensagem"] = MensagemModel.Serializar("Nenhum proprietário de endereço informado!",
                    MensagemTipo.Erro);
                return RedirectToAction(nameof(Index), "Cliente");
            }

        }
        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromForm] int? idUsuario, [FromForm] EnderecoModel endereco){
            //checa se veio algum id de usuário dentro da rota. Esse id virá através de um hidden input no formulario
            if(idUsuario.HasValue){
                //pega o cliente correspondente ao id no banco de dados
                var cliente = await _context.Clientes.FindAsync(idUsuario);
                //passa esse cliente para a view bag
                ViewBag.Cliente = cliente;
                //checa se o modelstate do modelo enderecomodel é válido
                if(ModelState.IsValid){
                    //checa se a quantidade de endereços é igual a 0, se sim, o endereço inserido
                    //que é o primeiro, será o endereço padrão
                    if(cliente.Enderecos.Count() == 0) endereco.Selecionado = true;
                    //Obtém o cep normalizado, ou seja, só números e um hífen, antes dos 3 últimos numeros
                    endereco.CEP = ObterCepNormalizado(endereco.CEP);

                    //chega se o id do endereço é maior que 0, ou seja, se já existem outros, ou se ele é o primeiro
                    if(endereco.IdEndereco > 0){

                        //verifica se o endereço está vindo como selecionado, ou seja, definido como padrão
                        if(endereco.Selecionado){
                            //se sim, todos os outros endereços são definidos como Selecionado = false
                            cliente.Enderecos.ToList().ForEach(e => e.Selecionado = false);
                        }
                        //verifica se o endereço existe na lista de endereços do cliente
                        //, passando as chaves primárias existentes na tabela de endereço
                        if(EnderecoExists(idUsuario.Value, endereco.IdEndereco)){
                            //se existir, o endereço é retornado da lista de endereços do cliente
                            var enderecoAtual = cliente.Enderecos
                                .FirstOrDefault(e => e.IdEndereco == endereco.IdEndereco);
                            //faz uma verificação no banco de dados pra ver se algum dado do endereço foi alterado
                            _context?.Entry(enderecoAtual).CurrentValues.SetValues(endereco);
                            //caso o endereço esteja inalterado, retornamos um erro
                            if(_context?.Entry(enderecoAtual).State == EntityState.Unchanged){
                                TempData["mensagem"] = MensagemModel.Serializar("Nenhum dado do endereço foi alterado", 
                                    MensagemTipo.Erro);
                            }
                            else{
                                //se o endereço tiver sido alterado, salvamos os dados da memória para o banco de dados
                                //e vemos se um ou mais objetos foram afetados pela mudança
                                if(await _context.SaveChangesAsync()>0){
                                    TempData["mensagem"] = MensagemModel.Serializar("Endereço alterado com sucesso!");
                                }
                                else{
                                    TempData["mensagem"] = MensagemModel.Serializar("Erro ao alterar o endereço", 
                                        MensagemTipo.Erro);
                                }
                            }
                        }
                        //se o endereço não existir
                        else{
                            TempData["mensagem"] = MensagemModel.Serializar("Endereço não existe!", 
                                        MensagemTipo.Erro);
                        }
                    }
                    //se o número de endereços for 0, ou seja, se vamos adicionar o primeiro endereço
                    else{
                        //verifica se a quantidade de endereços é maior que 0, se não, recebe um, se sim, recebe o maior id +1
                        var idEndereco = cliente.Enderecos.Count() > 0? cliente.Enderecos
                            .Max(e => e.IdEndereco) + 1 : 1;

                        //passamos esse id para o id do endereço que estamos cadastrando
                        endereco.IdEndereco = idEndereco;

                        //pegamos, no banco de dado, um cliente cujo Id seja igual ao id passado na rota,
                        //e adicionamos à sua lista de endereços o novo endereço
                        _context?.Clientes?.FirstOrDefault(c => c.IdUsuario == idUsuario)
                            .Enderecos?.Add(endereco);
                        //tentamos salvar
                        if(await _context.SaveChangesAsync() > 0){
                            TempData["mensagem"] = MensagemModel.Serializar("Endereço cadastrado com sucesso!");
                        }
                        else{
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao cadastrar o endereço", 
                                MensagemTipo.Erro);
                        }
                    }
                    //redirecionamento para a view Index do controller endereco, passando um novo id de cliente, que será idUsuario
                    //ou seja, voltarão a serem listados os endereços do usuário com esse id
                    return RedirectToAction(nameof(Index), "Endereco", new { cid = idUsuario });
                }
                else{
                    return View(endereco);
                }
            }
            else{
                TempData["mensagem"] = MensagemModel.Serializar("Nenhum proprietário de endereço foi informado", 
                    MensagemTipo.Erro);
                    return RedirectToAction(nameof(Index), "Cliente");
            }

        }

        private string ObterCepNormalizado(string cep)
        {
            string cepNormalizado = cep.Replace("-", "").Replace(".", "");
            return cepNormalizado.Insert(5, "-");
        }

        [HttpGet]
        public async Task<IActionResult> Excluir(int? cid, int? eid){
            if(!cid.HasValue){
                TempData["mensagem"] = MensagemModel.Serializar("Cliente não informado",
                    MensagemTipo.Erro);
                return RedirectToAction(nameof(Index), "Cliente");
            }
            if(!eid.HasValue){
                TempData["mensagem"] = MensagemModel.Serializar("Endereço não informado",
                    MensagemTipo.Erro);
                return RedirectToAction(nameof(Index), new { cid = cid });
            }

            var cliente = await _context.Clientes.FindAsync(cid);
            var endereco = cliente.Enderecos.FirstOrDefault(e => e.IdEndereco == eid);
            if(endereco == null){
                TempData["mensagem"] = MensagemModel.Serializar("Endereço não encontrado",
                    MensagemTipo.Erro);
                return RedirectToAction(nameof(Index), new { cid = cid });
            }
            
            ViewBag.Cliente = cliente;
            return View(endereco);
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(int idUsuario, int idEndereco){
            //procura o cliente com o id vindo da rota
            var cliente = await _context.Clientes.FindAsync(idUsuario);
            //pega o primeiro endereço cujo id é igual ao id do endereço vindo da rota
            var endereco = cliente?.Enderecos?.FirstOrDefault(e => e.IdEndereco == idEndereco);

            //se o endereço existor
            if(endereco != null){
                //entra na coleção de endereços do cliente que foi pegado do banco e retira o endereço
                cliente?.Enderecos?.Remove(endereco);
                //tenta salvar as alterações no banco de dados
                if(await _context.SaveChangesAsync() > 0){
                    TempData["mensagem"] = MensagemModel.Serializar("Objeto excluído com sucesso!");
                    /* verifica se o endereço excluído é o endereço padrão, e se o cliente possui 
                    mais algum outro endereço restante. Importante, pra questão de otimização, a verificação
                    de selecionado do endereço, pois assim já evita mais uma consulta no banco de dados */
                    if(endereco.Selecionado && cliente?.Enderecos?.Count() > 0){
                        //Pega o primeiro endereço restante dos que restaram, caso restaram, e o transforma
                        //em padrão
                        cliente.Enderecos.FirstOrDefault().Selecionado = true;
                        //Salva as alterações no banco de dados de novo
                        await _context.SaveChangesAsync();
                    }
                }
                else{
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir objeto",
                        MensagemTipo.Erro);
                }
                
            }
            else{
                TempData["mensagem"] = MensagemModel.Serializar("Não foi possível encontrar endereço",
                        MensagemTipo.Erro);
            }
            /* Retorna a ação para a listagem de endereços, passando uma nova id do atual usuário
            para voltar a listar todos os endereços dele */
            return RedirectToAction("Index", new { cid = idUsuario } );
        }

        private bool EnderecoExists(int cid, int eid){
            return _context.Clientes.FirstOrDefault(c => c.IdUsuario == cid).Enderecos
                .Any(e => e.IdEndereco == eid);
        }
    }
}