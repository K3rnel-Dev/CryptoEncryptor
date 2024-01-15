using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CryptoData
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void BtnDashboard_Click(object sender, EventArgs e)
        {
            panel1.Height = BtnDashboard.Height;
            panel1.Top = BtnDashboard.Top;
            BtnDashboard.BackColor = Color.FromArgb(46, 51, 73);
            label2.Text = "Encrypt Form";
        }

        private void BtnDashboard_Leave(object sender, EventArgs e)
        {
            BtnDashboard.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Height = button1.Height;
            panel1.Top = button1.Top;
            button1.BackColor = Color.FromArgb(46, 51, 73);
            DecryptForm dec = new DecryptForm();
            dec.Show();
        }

        private void button1_Leave(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panel1.Height = button3.Height;
            panel1.Top = button3.Top;
            button3.BackColor = Color.FromArgb(46, 51, 73);
            KeysGen keys = new KeysGen();
            keys.Show();
        }

        private void button3_Leave(object sender, EventArgs e)
        {
            button3.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog1.FileName;
                textBox1.Text = selectedFilePath;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog2.ShowDialog();
            if (result == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog2.FileName;
                textBox2.Text = selectedFilePath;
            }
        }


        private async void button4_Click(object sender, EventArgs e)
        {
            string keyFilePath = textBox2.Text;
            string targetFilePath = textBox1.Text;

            if (File.Exists(keyFilePath) && File.Exists(targetFilePath))
            {
                string base64Key = File.ReadAllText(keyFilePath);
                byte[] key = Convert.FromBase64String(base64Key);

                EncryptFile(targetFilePath, key);
                MessageBox.Show("File encrypted successfully!");
                label5.ForeColor = Color.LightGreen;
                label5.Text = "STATUS: Success!";
                await Task.Delay(2000);
                textBox1.Text = "";
                textBox2.Text = "";
                label5.ForeColor = Color.White;
                label5.Text = "STATUS: idle";
            }
            else
            {
                MessageBox.Show("Key file or target file does not exist!");
                label5.ForeColor = Color.Red;
                label5.Text = "STATUS: Failed!";
                await Task.Delay(2000);
                textBox1.Text = "";
                textBox2.Text = "";
                label5.ForeColor = Color.White;
                label5.Text = "STATUS: idle";
            }
        }

        private void EncryptFile(string filePath, byte[] key)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] fileData = new byte[fs.Length];
                    fs.Read(fileData, 0, (int)fs.Length);

                    using (Aes aesAlg = Aes.Create())
                    {
                        using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                        {
                            using (MemoryStream msEncrypt = new MemoryStream())
                            {
                                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                                {
                                    csEncrypt.Write(fileData, 0, fileData.Length);
                                }

                                File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(filePath), "encrypted_data_" + Path.GetFileName(filePath) + ".enc"), msEncrypt.ToArray());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error encrypting file: " + ex.Message);
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            this.Capture = false;
            var msg = Message.Create(this.Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
            this.WndProc(ref msg);
        }
    }
}
