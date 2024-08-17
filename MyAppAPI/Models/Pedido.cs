namespace MyAppAPI.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime DtPedido { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set;}
        public int ProdutoId {  get; set; }
        public Produto Produto { get; set; }
        public int Quantidade { get; set; }

    }
}




       