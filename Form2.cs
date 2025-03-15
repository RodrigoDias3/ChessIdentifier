using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CG_OpenCV
{
    public partial class InfoEquipa : Form
    {
        string[] brancas = new string[16];
        string[] pretas = new string[16];

        Dictionary<string, string> nomeDasPecas = new Dictionary<string, string>
        {
            {"cavaloBranco", "Cavalo B"},
            {"torreBranco", "Torre B"},
            {"rainhaBranco", "Rainha B"},
            {"bispoBranco", "Bispo B"},
            {"reiBranco", "Rei B"},
            {"peaoBranco", "Peão B"},
            {"cavaloPreto", "Cavalo P"},
            {"torrePreto", "Torre P"},
            {"rainhaPreto", "Rainha P"},
            {"bispoPreto", "Bispo P"},
            {"reiPreto", "Rei P"},
            {"peaoPreto", "Peão P"},
            {"VAZIA", "Vazia"}
        };

        public InfoEquipa(string[][] tabuleiro, string equipa)
        {
            InitializeComponent();
            ExibirInfoEquipa(tabuleiro, equipa);
        }

        private void ExibirInfoEquipa(string[][] tabuleiro, string equipa)
        {
            int sum = 0;
            // Inicializar dicionários para contar peças
            Dictionary<string, int> contagemPecas = new Dictionary<string, int>
            {
                {"Rei", 0},
                {"Rainha", 0},
                {"Torre", 0},
                {"Bispo", 0},
                {"Cavalo", 0},
                {"Peão", 0}
            };

            // Contar as peças da equipe especificada
            for (int i = 0; i < tabuleiro.Length; i++)
            {
                for (int j = 0; j < tabuleiro[i].Length; j++)
                {
                    string peçaAtual = tabuleiro[i][j];

                    if (nomeDasPecas.ContainsKey(peçaAtual))
                    {
                        string nomePeça = nomeDasPecas[peçaAtual];

                        // Verifica se a peça pertence à equipe solicitada
                        if ((equipa == "Brancas" && peçaAtual.EndsWith("Branco")) ||
                            (equipa == "Pretas" && peçaAtual.EndsWith("Preto")))
                        {
                            string tipoPeça = nomePeça.Split(' ')[0]; // Extrai o tipo da peça (Rei, Rainha, etc.)
                            if (contagemPecas.ContainsKey(tipoPeça))
                            {
                                contagemPecas[tipoPeça]++;
                            }
                        }
                    }
                }
            }

            // Exibir contagem de peças da equipe especificada
            textBox1.AppendText($"Equipa {equipa}:");
            textBox1.AppendText(Environment.NewLine);
            foreach (var item in contagemPecas)
            {
                sum += item.Value;
                textBox1.AppendText(item.Key + ": " + item.Value);
                textBox1.AppendText(Environment.NewLine);
            }
            textBox1.AppendText(Environment.NewLine);
            textBox1.AppendText("Total : " + sum);
            textBox1.AppendText(Environment.NewLine);

            textBox1.AppendText(Environment.NewLine);

            // Exibir tabuleiro com iniciais das peças da equipe especificada
            ExibirTabuleiroIniciais(tabuleiro, equipa);
        }

        private void ExibirTabuleiroIniciais(string[][] tabuleiro, string equipa)
        {
            textBox1.AppendText("Tabuleiro:");
            textBox1.AppendText(Environment.NewLine);

            // Adicionar linha superior do tabuleiro
            textBox1.AppendText("+---+---+---+---+---+---+---+---+");
            textBox1.AppendText(Environment.NewLine);

            for (int i = 0; i < tabuleiro.Length; i++)
            {
                textBox1.AppendText("|");

                for (int j = 0; j < tabuleiro[i].Length; j++)
                {
                    string peçaAtual = tabuleiro[i][j];
                    string inicialPeça = "X";

                    if (nomeDasPecas.ContainsKey(peçaAtual))
                    {
                        string nomePeça = nomeDasPecas[peçaAtual];
                        if ((equipa == "Brancas" && peçaAtual.EndsWith("Branco")) ||
                            (equipa == "Pretas" && peçaAtual.EndsWith("Preto")))
                        {
                            inicialPeça = ObterInicial(nomePeça);
                        }
                    }

                    // Adiciona a inicial da peça ou espaço vazio, mantendo o formato compacto
                    textBox1.AppendText($" {inicialPeça} ");
                    textBox1.AppendText("|");
                }

                // Adicionar linha horizontal entre as linhas do tabuleiro
                textBox1.AppendText(Environment.NewLine);
            }

            textBox1.AppendText("+---+---+---+---+---+---+---+---+");
        }




        private string ObterInicial(string nomePeça)
        {
            switch (nomePeça)
            {
                case "Rei B": return "K";
                case "Rainha B": return "Q";
                case "Torre B": return "T";
                case "Bispo B": return "B";
                case "Cavalo B": return "C";
                case "Peão B": return "P";
                case "Rei P": return "k";
                case "Rainha P": return "q";
                case "Torre P": return "t";
                case "Bispo P": return "b";
                case "Cavalo P": return "c";
                case "Peão P": return "p";
                default: return "X";
            }
        }


    }
}
