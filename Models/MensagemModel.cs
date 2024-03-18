using Newtonsoft.Json;

namespace EstoqueWeb.Models
{
    public enum MensagemTipo{
        Info,
        Erro,
    }
    public class MensagemModel{
        public MensagemTipo Tipo { get; set; }
        public string Text { get; set; }
        public MensagemModel(string mensagem, MensagemTipo tipo)
        {
            this.Text = mensagem;
            this.Tipo = tipo;
        }


        public static string Serializar(string mensagem, MensagemTipo tipo = MensagemTipo.Info){
            var mensagemModel = new MensagemModel(mensagem, tipo);
            return JsonConvert.SerializeObject(mensagemModel);
        }
        public static MensagemModel Deserializar(string mensagem){
            return JsonConvert.DeserializeObject<MensagemModel>(mensagem);
        }
    }
}