using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace CryptoData
{
    public partial class DecryptForm : Form
    {
        private int decryptionCount = 0;

        public DecryptForm()
        {
            InitializeComponent();
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            string keyFilePath = textBox1.Text;
            string encryptedFilePath = textBox2.Text;

            if (File.Exists(keyFilePath) && File.Exists(encryptedFilePath))
            {
                string base64Key = File.ReadAllText(keyFilePath);
                byte[] key = Convert.FromBase64String(base64Key);

                DecryptFile(encryptedFilePath, key);
                this.Close();
            }
            else
            {
                MessageBox.Show("Key file or encrypted file does not exist!");
            }
        }

        private void DecryptFile(string filePath, byte[] key)
        {
            try
            {
                byte[] fileData = File.ReadAllBytes(filePath);

                using (Aes aesAlg = Aes.Create())
                {
                    int ivSize = aesAlg.BlockSize / 8;
                    byte[] iv = new byte[ivSize];

                    // Чтение IV из зашифрованного файла
                    Array.Copy(fileData, iv, ivSize);

                    using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(key, iv))
                    {
                        using (MemoryStream msDecrypt = new MemoryStream(fileData, ivSize, fileData.Length - ivSize))
                        {
                            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                // Генерация уникального имени файла
                                string decryptedFileName = $"decrypted_{++decryptionCount}.dec";
                                string decryptedFilePath = Path.Combine(Path.GetDirectoryName(filePath), decryptedFileName);

                                using (FileStream fs = new FileStream(decryptedFilePath, FileMode.Create, FileAccess.Write))
                                {
                                    csDecrypt.CopyTo(fs);
                                }

                                MessageBox.Show($"File decrypted and saved as: {decryptedFileName}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error decrypting file: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog1.FileName;
                textBox2.Text = selectedFilePath;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog2.ShowDialog();
            if (result == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog2.FileName;
                textBox1.Text = selectedFilePath;
            }
        }
    }
}
