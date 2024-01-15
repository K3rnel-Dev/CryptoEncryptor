using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace CryptoData
{
    public partial class KeysGen : Form
    {
        public KeysGen()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string selectedFilePath = saveFileDialog1.FileName;
                textBox1.Text = selectedFilePath;
            }
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            string keyFilePath = textBox1.Text;

            if (!string.IsNullOrEmpty(keyFilePath))
            {
                GenerateFernetKey(keyFilePath);
                MessageBox.Show("Key generated successfully!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a valid path for the key file.");
            }
        }

        private void GenerateFernetKey(string keyFilePath)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.KeySize = 256; // Установите требуемый размер ключа (в данном случае 256 бит)

                    aesAlg.GenerateKey();
                    aesAlg.GenerateIV();

                    byte[] key = aesAlg.Key;
                    byte[] iv = aesAlg.IV;

                    // Преобразование ключа в строку base64
                    string base64Key = Convert.ToBase64String(key);

                    // Запись строки base64 в файл
                    File.WriteAllText(keyFilePath, base64Key);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating key: " + ex.Message);
            }
        }
    }
}
