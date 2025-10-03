using System.Text.RegularExpressions;

namespace GemelliApi.Domain.ValueObjects;

public sealed class Endereco
{
        public string Logradouro { get; private set; }
        public string Numero { get; private set; }
        public string Bairro { get; private set; }
        public string Cidade { get; private set; }
        public Estado Estado { get; private set; }
        public string CEP { get; private set; }
        public string Complemento { get; private set; }

        private Endereco() { }

        public Endereco(
            string logradouro,
            string numero,
            string bairro,
            string cidade,
            Estado estado,
            string cep,
            string complemento)
        {
            if (string.IsNullOrWhiteSpace(logradouro))
            {
                throw new ArgumentException("Logradouro é obrigatório");
            }

            if (string.IsNullOrWhiteSpace(cidade))
            {
                throw new ArgumentException("Cidade é obrigatória");
            }
            
            if (string.IsNullOrWhiteSpace(cep))
            {
                throw new ArgumentException("CEP é obrigatório");
            }
            
            if (string.IsNullOrWhiteSpace(bairro))
            {
                throw new ArgumentException("Bairro é obrigatório");
            }
            
            if (string.IsNullOrWhiteSpace(numero))
            {
                throw new ArgumentException("Número é obrigatório");
            }
            
            if (estado is null)
            {
                throw new ArgumentException("Estado é obrigatório");
            }
            
            if (string.IsNullOrWhiteSpace(complemento))
            {
                throw new ArgumentException("Complemento é obrigatório");
            }

            Logradouro = logradouro;
            Numero = numero;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
            CEP = cep;
            Complemento = complemento;
        }

        public string ObterCepFormatado()
        {
            if (CEP.Length != 8)
            {
                return CEP;
            }

            return $"{CEP.Substring(0, 5)}-{CEP.Substring(5, 3)}";
        }
}