﻿using Independentsoft.Office.Odf;
using JsonPolimi.Tipi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data.Odbc;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Telegram.Bot.Types.Enums;
using Size = System.Drawing.Size;

namespace JsonPolimi
{
    public partial class Form1 : Form
    {
        public static FileSalvare FileSalvare;

        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            var ofd = new OpenFileDialog();
            var rDialog = ofd.ShowDialog();
            if (rDialog != DialogResult.OK)
            {
                ofd.Dispose();
                return;
            }

            string content;
            try
            {
                content = File.ReadAllText(ofd.FileName);
            }
            catch (Exception e2)
            {
                MessageBox.Show("Lettura fallita! \n\n" + e2.Message);
                ofd.Dispose();
                return;
            }

            if (content.Length < 1)
            {
                MessageBox.Show("Il file letto sembra vuoto!");
                ofd.Dispose();
                return;
            }

            var stuff = JObject.Parse(content);

            var infoData = stuff["info_data"];
            var i = infoData.Children();

            foreach (var i2 in i)
            {
                var i3 = i2.First;

                Aggiungi(i3);
            }

            Variabili.L.Sort();

            ofd.Dispose();
        }

        private static void Refresh_Tabella()
        {
            if (Variabili.L == null)
            {
                MessageBox.Show("Lista vuota!");
                return;
            }

            var html = "<html><body><table>";
            var n = Variabili.L.GetCount();

            if (n <= 0)
            {
                MessageBox.Show("Lista vuota!");
                return;
            }

            for (var i = 0; i < n; i++)
            {
                var elem = Variabili.L.GetElem(i);

                html += "<tr>";

                html += "<td>";
                html += elem.Id;
                html += "</td>";

                html += "<td>";
                html += elem.Platform;
                html += "</td>";

                html += "<td>";
                html += elem.Classe;
                html += "</td>";

                html += "<td>";
                html += elem.Degree;
                html += "</td>";

                html += "<td>";
                html += elem.Language;
                html += "</td>";

                html += "<td>";
                html += elem.Office;
                html += "</td>";

                html += "<td>";
                html += elem.School;
                html += "</td>";

                html += "<td>";
                html += elem.Tipo;
                html += "</td>";

                html += "<td>";
                html += elem.Year;
                html += "</td>";

                html += "<td>";
                html += elem.IdLink;
                html += "</td>";

                html += "<td>";
                html += elem.PermanentId;
                html += "</td>";

                html += "</tr>";
            }

            html += "</table></body></html>";
            File.WriteAllText("temp.html", html);
            Process.Start("temp.html");
        }

        private static void Aggiungi(JToken i)
        {
            var g = new Gruppo
            {
                Classe = i["class"].ToString(),
                Degree = i["degree"].ToString()
            };
            try
            {
                g.Platform = i["group_type"].ToString();
            }
#pragma warning disable CS0168 // La variabile è dichiarata, ma non viene mai usata
            catch (Exception)
#pragma warning restore CS0168 // La variabile è dichiarata, ma non viene mai usata
            {
                g.Platform = i["platform"].ToString();
            }

            g.Id = i["id"].ToString();
            g.Language = i["language"].ToString();
            g.Office = i["office"].ToString();
            g.School = i["school"].ToString();
            g.IdLink = i["id_link"].ToString();

            try
            {
                g.Tipo = i["type"].ToString();
            }
            catch
            {
                g.Tipo = null;
            }

            try
            {
                g.Year = i["year"].ToString();
            }
            catch
            {
                g.Year = null;
            }

            try
            {
                g.PermanentId = i["permanentId"].ToString();
            }
            catch
            {
                g.PermanentId = null;
            }

            var data = i["LastUpdateInviteLinkTime"].ToString();
            try
            {
                g.LastUpdateInviteLinkTime = DataFromString(data);
            }
            catch (Exception e)
            {
                g.LastUpdateInviteLinkTime = null;
                throw e;
            }

            g.Aggiusta();

            Variabili.L.Add(g);
        }

        public static DateTime? DataFromString(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;
            if (data == "null")
                return null;

            if (data.Contains("T"))
            {
                //2019-07-27T17:13:23.5409603+02:00
                var a1 = data.Split('T');

                //2019-07-27
                var a2 = a1[0].Split('-');

                //17:13:23.5409603+02:00
                var a3 = a1[1].Split('+');

                //17:13:23.5409603
                var a4 = a3[0].Split('.');

                //17:13:23
                var a5 = a4[0].Split(':');

                return new DateTime(Convert.ToInt32(a2[0]), Convert.ToInt32(a2[1]), Convert.ToInt32(a2[2]), Convert.ToInt32(a5[0]), Convert.ToInt32(a5[1]), Convert.ToInt32(a5[2]));
            }

            if (data.Contains("."))
            {
                //2019-07-29 18:26:55.034083
                data = data.Split('.')[0];

                //2019-07-29 18:26:55
                var b1 = data.Split(' ');

                //2019-07-29
                var b2 = b1[0].Split('-');

                //18:26:55
                var b3 = b1[1].Split(':');

                return new DateTime(Convert.ToInt32(b2[0]), Convert.ToInt32(b2[1]), Convert.ToInt32(b2[2]), Convert.ToInt32(b3[0]), Convert.ToInt32(b3[1]), Convert.ToInt32(b3[2]));
            }

            //27/07/2019 11:42:24
            var s1 = data.Split(' ');

            //27/07/2019
            var s2 = s1[0].Split('/');

            //11:42:24
            var s3 = s1[1].Split(':');

            return new DateTime(Convert.ToInt32(s2[2]), Convert.ToInt32(s2[1]), Convert.ToInt32(s2[0]), Convert.ToInt32(s3[0]), Convert.ToInt32(s3[1]), Convert.ToInt32(s3[2]));
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Aggiusta();
            Refresh_Tabella();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
            {
                MessageBox.Show("Lista vuota!");
                return;
            }

            Aggiusta();
            Variabili.L.Sort();

            var json = "{\"info_data\":{";
            var n = Variabili.L.GetCount();
            for (var i = 0; i < n; i++)
            {
                var elem = Variabili.L.GetElem(i);

                json += '\n';
                json += '"';
                json += elem.Id;
                json += '"' + ":";

                json += elem.To_json();

                if (i != n - 1)
                    json += ",";
            }

            json += "},";
            json += '\n';
            json += "\"index_data\":[";
            for (var i = 0; i < n; i++)
            {
                var elem = Variabili.L.GetElem(i);

                json += '\n';
                json += elem.To_json();

                if (i != n - 1)
                    json += ",";
            }

            json += "]}";

            Salva(json);
        }

        private static void Salva(string json)
        {
            var o = new SaveFileDialog();
            var r = o.ShowDialog();
            if (r != DialogResult.OK)
            {
                o.Dispose();
                return;
            }

            File.WriteAllText(o.FileName, json);
            o.Dispose();
        }

        private static void Aggiusta()
        {
            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            var n = Variabili.L.GetCount();
            for (var i = 0; i < n; i++)
            {
                var elem = Variabili.L.GetElem(i);
                if (!string.IsNullOrEmpty(elem.IdLink)) continue;
                Variabili.L.Remove(i);

                i--;
                n = Variabili.L.GetCount();
            }

            n = Variabili.L.GetCount();
            for (var i = 0; i < n; i++)
            {
                var elem = Variabili.L.GetElem(i);

                var nome = AggiustaNome(elem.Classe);
                elem.Classe = nome;

                Variabili.L.SetElem(i, elem);
            }

            Variabili.L.Sort();
        }

        private static string AggiustaNome(string s)
        {
            if (s.Contains("<="))
            {
                var n = s.IndexOf("<=", StringComparison.Ordinal);
                var r = "";
                r += s.Substring(0, n);
                r += s.Substring(n + 2);
                return r;
            }

            if (s.Contains("&lt;="))
            {
                var n = s.IndexOf("&lt;=", StringComparison.Ordinal);
                var r = "";
                r += s.Substring(0, n);
                r += s.Substring(n + 5);
                return r;
            }

            return s;
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            var x = new AggiungiForm(false);
            x.ShowDialog();
            x.Dispose();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            var o = new OpenFileDialog();
            var r = o.ShowDialog();
            if (r != DialogResult.OK)
            {
                o.Dispose();
                return;
            }

            var (item1, item2) = ShowInputDialog("Anno");
            if (item1 != DialogResult.OK)
            {
                o.Dispose();
                return;
            }

            Apri_ODS(o.FileName, item2);

            o.Dispose();

            /*
            Apri_ODS("C:\\Users\\Arme\\Downloads\\pm3.ods", "2017/2018");
            Apri_ODS("C:\\Users\\Arme\\Downloads\\pm4.ods", "2018/2019");
            Apri_ODS("C:\\Users\\Arme\\Downloads\\pm5.ods", "2019/2020");
            */
        }

        private static Tuple<DialogResult, string> ShowInputDialog(string title)
        {
            var size = new Size(200, 70);
            var inputBox = new Form
            {
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ClientSize = size,
                Text = title,
                MaximizeBox = false,
                StartPosition = FormStartPosition.CenterScreen
            };

            var textBox = new TextBox
            {
                Size = new Size(size.Width - 10, 23),
                Location = new Point(5, 5),
                Text = ""
            };
            inputBox.Controls.Add(textBox);

            var okButton = new Button
            {
                DialogResult = DialogResult.OK,
                Name = "okButton",
                Size = new Size(75, 23),
                Text = "&OK",
                Location = new Point(size.Width - 80 - 80, 39)
            };
            inputBox.Controls.Add(okButton);

            var cancelButton = new Button
            {
                DialogResult = DialogResult.Cancel,
                Name = "cancelButton",
                Size = new Size(75, 23),
                Text = "&Cancel",
                Location = new Point(size.Width - 80, 39)
            };
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            var result = inputBox.ShowDialog();

            inputBox.Dispose();
            return new Tuple<DialogResult, string>(result, textBox.Text);
        }

        private static void Apri_ODS(string file, string year)
        {
            Spreadsheet x;
            try
            {
                x = new Spreadsheet();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }

            try
            {
                x.Open(file);
            }
            catch
            {
                MessageBox.Show("Non sono riuscito ad aprire il file!");
                return;
            }

            var nomeOld = new Gruppo();

            foreach (var y in x.Tables)
                foreach (var y2 in y.Rows)
                {
                    //Console.WriteLine("----- NUOVA RIGA ------");

                    var g = new InsiemeDiGruppi { GruppoDiBase = { Year = year }, NomeOld = nomeOld };

                    foreach (var y3 in y2.Cells)
                        foreach (var y4 in y3.Content)
                            if (y4 is Paragraph y5)
                                foreach (var y6 in y5.Content)
                                    Gruppo.AggiungiInformazioneAmbigua(y6.ToString(), ref g);
                            else
                                Console.WriteLine(y4.ToString());

                    g.Aggiusta();

                    foreach (var g3 in g.L) Variabili.L.Add(g3);

                    if (!string.IsNullOrEmpty(g.NomeOld.Classe)) nomeOld.Classe = g.NomeOld.Classe;

                    if (!string.IsNullOrEmpty(g.NomeOld.Language)) nomeOld.Language = g.NomeOld.Language;

                    if (!string.IsNullOrEmpty(g.NomeOld.Degree)) nomeOld.Degree = g.NomeOld.Degree;

                    if (!string.IsNullOrEmpty(g.NomeOld.School)) nomeOld.School = g.NomeOld.School;

                    if (!string.IsNullOrEmpty(g.NomeOld.Office)) nomeOld.Office = g.NomeOld.Office;

                    if (!string.IsNullOrEmpty(g.NomeOld.Year)) nomeOld.Year = g.NomeOld.Year;
                }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Vuoi davvero eliminare la lista in RAM?", "Sicuro?",
                MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes) Variabili.L = new ListaGruppo();
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            if (Variabili.L == null) Variabili.L = new ListaGruppo();

            if (Variabili.L.GetCount() <= 0)
            {
                MessageBox.Show("Lista vuota!");
                return;
            }

            Variabili.L.Sort();

            var x = new ListaGruppiModificaForm();
            x.ShowDialog();

            x.Dispose();
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            LoadGruppi();

            if (FileSalvare == null)
                FileSalvare = new FileSalvare();

            if (Variabili.L == null)
                Variabili.L = new ListaGruppo();

            foreach (var r in FileSalvare.Gruppi)
            {
                if (r.Chat.Type == ChatType.Private)
                    continue;

                var g = new Gruppo
                {
                    Classe = r.Chat.Title,
                    PermanentId = r.Chat.Id.ToString(),
                    Platform = "TG",
                    IdLink = TelegramLinkLastPart(r.Chat.InviteLink),
                    Tipo = "C",
                    LastUpdateInviteLinkTime = r.LastUpdateInviteLinkTime,
                };

                g.Aggiusta();
                Variabili.L.Add(g);
            }

            Variabili.L.Sort();
        }

        private static string TelegramLinkLastPart(string chatInviteLink)
        {
            var r = chatInviteLink.Split('/');
            return r[r.Length - 1];
        }

        public static void LoadGruppi()
        {
            var ofd = new OpenFileDialog();
            var rDialog = ofd.ShowDialog();
            if (rDialog != DialogResult.OK)
            {
                ofd.Dispose();
                return;
            }

            string content;
            try
            {
                content = File.ReadAllText(ofd.FileName);
            }
            catch (Exception e2)
            {
                MessageBox.Show("Lettura fallita! \n\n" + e2.Message);
                ofd.Dispose();
                return;
            }

            try
            {
                FileSalvare = JsonConvert.DeserializeObject<FileSalvare>(content);
            }
            catch
            {
                ;
            }

            if (FileSalvare == null)
                FileSalvare = new FileSalvare();

            ofd.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ;
        }
    }
}