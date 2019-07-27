﻿using System;
using System.Windows.Forms;

namespace JsonPolimi
{
    public partial class ListaGruppiModificaForm : Form
    {
        public ListaGruppiModificaForm()
        {
            InitializeComponent();
        }

        private void ListaGruppiModificaForm_Load(object sender, EventArgs e)
        {
            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            Filtra(null, 0);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var i = listBox1.SelectedIndex;
            if (i < 0 || i >= listBox1.Items.Count)
            {
                MessageBox.Show("Devi selezionare un gruppo!");
                return;
            }

            var r = (Riga)listBox1.Items[i];

            r.G.Aggiusta();
            var x = new AggiungiForm(true, r.G);
            x.ShowDialog();

            r.G = AggiungiForm.g;

            listBox1.Items[i] = r;
            Variabili.L.SetElem(r.I, AggiungiForm.g);
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            Filtra(textBox1.Text, comboBox1.SelectedIndex);
        }

        private void Filtra(string text, int selectedIndex)
        {
            listBox1.Items.Clear();

            if (text == null)
                text = "";

            if (selectedIndex < 0)
                selectedIndex = 0;

            text = text.ToLower();

            for (var i = 0; i < Variabili.L.GetCount(); i++)
            {
                var variable = Variabili.L.GetElem(i);

                if (!variable.Classe.ToLower().Contains(text)) continue;

                if (selectedIndex == 0 || variable.Platform.ToUpper() == comboBox1.Items[selectedIndex].ToString())
                    listBox1.Items.Add(new Riga(variable, i));
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var v1 = numericUpDown1.Value;
            var v2 = numericUpDown2.Value;
            if (v1 == -1 || v2 == -1)
            {
                MessageBox.Show("Devi selezionare dei valori validi!");
                return;
            }

            var dialogResult = MessageBox.Show("Sei sicuro di volerli unire?", "Sicuro?", MessageBoxButtons.YesNo);
            if (dialogResult != DialogResult.Yes) return;
            Variabili.L.MergeUnione(v1, v2);
            Filtra(textBox1.Text, comboBox1.SelectedIndex);
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Filtra(textBox1.Text, comboBox1.SelectedIndex);
        }
    }
}