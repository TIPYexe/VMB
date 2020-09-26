using Finisar.SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VMB_new
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        List<om> zilieri = new List<om>();
        List<om> zilieri_zi = new List<om>();

        private void IncarcaOameni()
        {
            zilieri = SqliteDataAccess.LoadArchive();

            // Ordonez alfabetic lista de zilieri
            zilieri = zilieri.OrderBy(q => q.nume).ToList(); 
        }

        private void RefreshSezon()
        {
            IncarcaOameni();
            Lista_sezon.Items.Clear();
            int i = 1;
            foreach (om Zilier in zilieri)
            {
                var row = new string[] { Zilier.nume, Zilier.prezente.ToString(), i.ToString() };
                var lvi = new ListViewItem(row);
                i++;
                lvi.Tag = Zilier;

                Lista_sezon.Items.Add(lvi);
            }
        }

        private void RefreshZi()
        {
            //IncarcaOameni();
            tableZi.Items.Clear();
            int i = 1;
            foreach (om Zilier in zilieri_zi)
            {
                var row = new string[] { Zilier.nume, i.ToString() };
                i++;
                var lvi = new ListViewItem(row);
                lvi.Tag = Zilier;

                tableZi.Items.Add(lvi);
            }
        }

        private void RefreshBonus()
        {
            IncarcaOameni();
            tabelBonus.Items.Clear();
            int i = 1;
            foreach (om Zilier in zilieri)
            {
                if (((int)Zilier.prezente / 5) > 0)
                {
                    var row = new string[] { Zilier.nume, ((int)Zilier.prezente / 5).ToString(), i.ToString() };
                    var lvi = new ListViewItem(row);
                    i++;
                    lvi.Tag = Zilier;

                    tabelBonus.Items.Add(lvi);
                }
            }
        }

        public string ultimaZi_data;
        private void RefreshCalendar()
        {
            DateTime lastZi = SqliteDataAccess.LastZi();
            ultimaZi_data = lastZi.ToString("dd/MM/yyyy");
            ultimaZi.Text = "Ultima zi introdusa: " + ultimaZi_data;
        }

        static bool Valid(string str1, string str2)
        {
            // str1 = cel din textbox
            // str2 = cel din tabel

            string[] aux1 = str1.Split(' ');
            string[] aux2 = str2.Split(' ');

            int n = 0;
            int nr1 = aux1.Length, nr2 = aux2.Length;
            for(int i=0; i<nr1; i++)
            {
                for (int j = 0; j < nr2; j++)
                {
                    if (String.Compare(aux1[i], aux2[j]) == 0)
                    {
                        n++;
                        break;
                    }
                }
            }

            if (n == nr1 || n == nr2)
                return true;
            if(n >= 1)
            {
                DialogResult dialogResult = MessageBox.Show("Este aceeasi persoana cu\n"+str2+"?","Verificare", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                    return true;
                else
                    return false;
            }
            return false;
        }

        static bool Valid_100(string str1, string str2)
        {
            // str1 = cel din textbox
            // str2 = cel din tabel

            string[] aux1 = str1.Split(' ');
            string[] aux2 = str2.Split(' ');

            int n = 0;
            int nr1 = aux1.Length, nr2 = aux2.Length;
            for (int i = 0; i < nr1; i++)
            {
                for (int j = 0; j < nr2; j++)
                {
                    if (String.Compare(aux1[i], aux2[j]) == 0)
                    {
                        n++;
                        break;
                    }
                }
            }

            if (n == nr1 || n == nr2)
                return true;

            return false;
        }

        static bool arePermutation(String str1, String str2)
        {
            // Get lenghts of both strings  
            int n1 = str1.Length;
            int n2 = str2.Length;

            // If length of both strings is not same,  
            // then they cannot be Permutation  
            if (n1 != n2)
                return false;
            char[] ch1 = str1.ToCharArray();
            char[] ch2 = str2.ToCharArray();

            // Sort both strings  
            Array.Sort(ch1);
            Array.Sort(ch2);

            // Compare sorted strings  
            for (int i = 0; i < n1; i++)
                if (ch1[i] != ch2[i])
                    return false;

            return true;
        }

        private int Indice(string text)
        { 
            int n = 0 ;

            foreach (om Zilier in zilieri)
            {
                if (Valid_100(text, Zilier.nume))
                {
                    if (text.Length > Zilier.nume.Length)
                        SqliteDataAccess.UpdateTabel_nume(zilieri[n], text);
                    return n;
                }
                n++;
            }
            n = 0;
            foreach (om Zilier in zilieri)
            {
                if (Valid(text, Zilier.nume))
                {
                    if (text.Length > Zilier.nume.Length)
                        SqliteDataAccess.UpdateTabel_nume(zilieri[n], text);
                    return n;
                }
                n++;
            }

            return -1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tableZi.Items.Clear();
            RefreshBonus();
            RefreshSezon();
            RefreshCalendar();
              
 
            tabControl1.SelectedTab = tabPrezenta;
            if (SqliteDataAccess.validZi(this.dateTimePicker.Text))
                WarningBox.Text = "Prezenta pe " + dateTimePicker.Text + " a fost facuta!";
            else
                WarningBox.Text = "";
        }

        //sa pot muta fereastra
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        //buttonPrezenta
        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPrezenta;
            if (SqliteDataAccess.validZi(this.dateTimePicker.Text))
                WarningBox.Text = "Prezenta pe " + dateTimePicker.Text + " a fost facuta!";
            else
                WarningBox.Text = "";
        }

        private void buttonZi_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabZi;
        }

        private void buttonSez_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabSez;
        }

        private void buttonBonus_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabBonus;
        }

        private void exit_Click(object sender, EventArgs e)
        {
            zilieri.Clear();
            zilieri_zi.Clear();
            string nouaZi = this.dateTimePicker.Text;
            string[] data = nouaZi.Split('-');
            nouaZi = data[1] + "/" + data[0] + "/" + data[2];
            SqliteDataAccess.UpdateCalendar(nouaZi);
            Close();
        }

        private void tabBonus_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabBonus;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPrezenta;
            if (SqliteDataAccess.validZi(this.dateTimePicker.Text))
                WarningBox.Text = "Prezenta pe " + dateTimePicker.Text + " a fost facuta!";
            else
                WarningBox.Text = "";
        }

        private void button12_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPrezenta;
            if (SqliteDataAccess.validZi(this.dateTimePicker.Text))
                WarningBox.Text = "Prezenta pe " + dateTimePicker.Text + " a fost facuta!";
            else
                WarningBox.Text = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPrezenta;
            if (SqliteDataAccess.validZi(this.dateTimePicker.Text))
                WarningBox.Text = "Prezenta pe " + dateTimePicker.Text + " a fost facuta!";
            else
                WarningBox.Text = "";
        }

        private void button11_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabZi;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabZi;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabZi;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabSez;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabSez;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabSez;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabBonus;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabBonus;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabBonus;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void buttonJumate_Click(object sender, EventArgs e)
        {
            foreach(om Zilier in zilieri_zi)
            {
                int i = Indice(Zilier.nume);
                zilieri[i].prezente -= ((float) 0.5);
                SqliteDataAccess.UpdateTabel(zilieri[i]);
            }
            
            IncarcaOameni();
            RefreshSezon();
        }

        private void buttonNewZi_Click(object sender, EventArgs e)
        {

        }

        private void tabZi_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Lista_sezon_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void TabelSezon_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void CasetaPrezenta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
               // if (SqliteDataAccess.validZi(this.dateTimePicker.Text))
                //{
                //    CasetaPrezenta.Text = "";
                //    MessageBox.Show("ZIUA NU ESTE VALIDA");
                //}
                //else
                //{
                    om Zilier = new om();

                    string aux0 = CasetaPrezenta.Text.ToLower();
                    string[] aux = aux0.Split(' ');
                    Zilier.nume = "";

                    CasetaPrezenta.Text = "";

                    for (int j = 0; j < aux.Length; j++)
                    {
                        //MessageBox.Show(aux[j]);

                        aux[j] = aux[j].First().ToString().ToUpper() + aux[j].Substring(1);
                        Zilier.nume = Zilier.nume + aux[j] + " ";
                    }

                    Zilier.nume = Zilier.nume.Trim();
                    Zilier.nume = Zilier.nume.First().ToString().ToUpper() + Zilier.nume.Substring(1);

                    int i = Indice(Zilier.nume);
                    int ok = 1;

                    if (i == -1)
                    {
                        Zilier.prezente = 1;

                        //S-ar putea sa trebuiasca in afara if-ului
                        SqliteDataAccess.SaveZilier(Zilier);
                    }
                    else
                    {

                        foreach (om Zilier2 in zilieri_zi)
                        {
                            if (Valid(Zilier2.nume, Zilier.nume))
                                ok = 0;
                        }

                        if (ok == 1)
                        {
                            zilieri[i].prezente++;
                            SqliteDataAccess.UpdateTabel(zilieri[i]);
                        }
                    }

                    if (ok == 1)
                    {
                        zilieri_zi.Add(Zilier);

                        IncarcaOameni();
                        RefreshSezon();
                        RefreshBonus();
                        RefreshZi();
                    }

                    ConfirmationBox.Text = "";
                    if (i != -1)
                        ConfirmationBox.Text = zilieri[i].nume + " are acum " + zilieri[i].prezente.ToString() + " prezente.";
                    else
                        ConfirmationBox.Text = Zilier.nume + " a facut PRIMA prezenta.";
                //}
            }
        }

        private void exit1_Click(object sender, EventArgs e)
        {
            zilieri.Clear();
            zilieri_zi.Clear();
            string nouaZi = this.dateTimePicker.Text;
            string[] data = nouaZi.Split('-');
            nouaZi = data[1] + "/" + data[0] + "/" + data[2];
            SqliteDataAccess.UpdateCalendar(nouaZi);
            Close();
        }

        private void exit2_Click(object sender, EventArgs e)
        {
            zilieri.Clear();
            zilieri_zi.Clear();
            string nouaZi = this.dateTimePicker.Text;
            string[] data = nouaZi.Split('-');
            nouaZi = data[1] + "/" + data[0] + "/" + data[2];
            SqliteDataAccess.UpdateCalendar(nouaZi);
            Close();
        }

        private void exit3_Click(object sender, EventArgs e)
        {
            zilieri.Clear();
            zilieri_zi.Clear();
            string nouaZi = this.dateTimePicker.Text;
            string[] data = nouaZi.Split('-');
            nouaZi = data[1] + "/" + data[0] + "/" + data[2];
            SqliteDataAccess.UpdateCalendar(nouaZi);
            Close();
        }

        //searchbar bonus
        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            
        }

        private void Form1_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void tabPrezenta_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void tabSez_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void tabZi_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void tabBonus_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void panel4_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void panel5_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                string nume_search = "";
                string aux0 = Searchbar.Text.ToLower();
                string[] aux = aux0.Split(' ');

                for (int j = 0; j < aux.Length; j++)
                {
                   // MessageBox.Show(aux[j]);
                    aux[j] = aux[j].First().ToString().ToUpper() + aux[j].Substring(1);
                    nume_search = nume_search + aux[j] + " ";
                }

                nume_search = nume_search.Trim();
                nume_search = nume_search.First().ToString().ToUpper() + nume_search.Substring(1);

                int i = Indice(nume_search);
                int nr = Int32.Parse(textBox2.Text);

                if (zilieri[i].prezente < nr * 5)
                {
                    MessageBox.Show(zilieri[i].nume + " are doar " + (((int)zilieri[i].prezente) / 5).ToString() + " bonusuri.");
                }
                else
                {
                    DialogResult dialogResult = MessageBox.Show("Esti sigur ca ii dai " + nr.ToString() + " bonusuri\nlui " + zilieri[i].nume + "?", "Verificare", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        zilieri[i].prezente -= 5 * nr;
                        SqliteDataAccess.UpdateTabel(zilieri[i]);
                        ConfirmationBoxBonus.Text = zilieri[i].nume + " a primit " + nr.ToString() + " bonusuri.";
                        Searchbar.Text = "";
                        textBox2.Text = "";
                        RefreshBonus();
                        RefreshSezon();
                    }
                }
            }
        }

        private void Searchbar_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                string nume_search = "";
                string aux0 = Searchbar.Text.ToLower();
                string[] aux = aux0.Split(' ');

                for (int j = 0; j < aux.Length; j++)
                {
                    //MessageBox.Show(aux[j]);
                    
                    aux[j] = aux[j].First().ToString().ToUpper() + aux[j].Substring(1);
                    nume_search = nume_search + aux[j] + " ";
                }

                nume_search = nume_search.Trim();
                nume_search = nume_search.First().ToString().ToUpper() + nume_search.Substring(1);

                int i = Indice(nume_search);
                int nr = Int32.Parse(textBox2.Text);

                if (zilieri[i].prezente < nr * 5)
                {
                    MessageBox.Show(zilieri[i].nume + " are doar " + (((int)zilieri[i].prezente) / 5).ToString() + " bonusuri.");
                }
                else
                {
                    DialogResult dialogResult = MessageBox.Show("Esti sigur ca ii dai " + nr.ToString() + " bonusuri\nlui " + zilieri[i].nume + "?", "Verificare", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        zilieri[i].prezente -= 5 * nr;
                        SqliteDataAccess.UpdateTabel(zilieri[i]);
                        ConfirmationBoxBonus.Text = zilieri[i].nume + " a primit " + nr.ToString() + " bonusuri.";
                        Searchbar.Text = "";
                        textBox2.Text = "";
                        RefreshBonus();
                        RefreshSezon();
                    }
                }
            }
        }

        private void ultimaZi_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void CasetaPrezenta_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                if (SqliteDataAccess.validZi(this.dateTimePicker.Text))
                    WarningBox.Text = "Prezenta pe " + dateTimePicker.Text + " a fost facuta!";
                else
                    WarningBox.Text = "";
            }
        }

        private void CasetaPrezenta_MouseHover(object sender, EventArgs e)
        {
         
        }
    }
}
