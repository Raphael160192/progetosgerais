using System;
using System.IO;

class NomeArquivo
{
    static void Main(string[] args)
    {
        string pasta = @"C:\Apontamento Horas";

        

        if (Directory.Exists(pasta))
        {
            try
            {
                // Caminho do arquivo TXT que será criado
                string caminhoTxt = @"C:\Users\RaphaelGundim\Desktop\ListaArquivos.txt";

                // Obtém todos os nomes de arquivos na pasta especificada
                string[] arquivos = Directory.GetFiles(pasta);

                // Escreve os nomes dos arquivos no arquivo TXT
                File.WriteAllLines(caminhoTxt, arquivos);

                Console.WriteLine($"Lista de arquivos criada com sucesso em: {caminhoTxt}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("A pasta especificada não existe.");
        }

        Console.WriteLine("Pressione qualquer tecla para sair...");
        Console.ReadKey();
    }
}
