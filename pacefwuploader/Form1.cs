using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace pacefwuploader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WebClient wc = new WebClient();
            byte[] response;
            string ipregex = @"\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";

            Regex regex = new Regex(ipregex);

            if (textBox2.Text == "" || !regex.IsMatch(textBox2.Text))
            {
                MessageBox.Show("Insira um IP válido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(textBox1.Text == "")
            {
                MessageBox.Show("Selecione o firmware.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ServicePointManager.Expect100Continue = false;

            toolStripStatusLabel1.Text = "Aguarde o modem parar de piscar o 'Ligado' e acender verde.";
            toolStripStatusLabel1.Invalidate();
            Form1.ActiveForm.Invalidate();
            Form1.ActiveForm.Update();            

            try {
                response = wc.UploadFile("http://" + textBox2.Text + "/cgi-bin/firmware.cgi", textBox1.Text);
                toolStripStatusLabel1.Text = "Sucesso";
                MessageBox.Show("Firmware enviado. Reinicie o modem pela sua página de configuração acessando: 'Gerenciamento > Reiniciar > Reiniciar'", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Certifique-se que o IP informado é do Pace V5471 e que o arquivo informado pode ser lido.\nMensagem: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel1.Text = "Erro";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.Filter = "Firmware Pace V5471|*.bin";
            openFileDialog1.FileName = "";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }
    }
}
