using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        

    public Form1()
        {
            InitializeComponent();
        }

        string host = "192.168.0.101";
        int port = 8888;
        TcpClient client = new TcpClient();
        string userName = "UserName";
        bool entered = true;
        StreamReader Reader = null;
        StreamWriter Writer = null;


        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "Введите свое имя";
            textBox1.ForeColor = Color.Gray;
            listBox1.BeginUpdate();
            listBox1.Items.Add("Введите свое имя");
            listBox1.EndUpdate();
        }



        private void createConnection() {
            try
            {
                client.Connect(host, port); //подключение клиента
                Reader = new StreamReader(client.GetStream());
                Writer = new StreamWriter(client.GetStream());
                if (Writer is null || Reader is null) return;
                SendMessageAsync(Writer);
                // запускаем новый поток для получения данных
                ReceiveMessageAsync(Reader);
            }
            catch (Exception ex)
            {
                listBox1.BeginUpdate();
                listBox1.Items.Add(ex.Message);
                listBox1.EndUpdate();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Enabled = false;
            button1.Enabled = false;
            userName = textBox1.Text;
            listBox1.BeginUpdate();
            listBox1.Items.Add($"Добро пожаловать, {userName}!");
            createConnection();
            textBox2.Enabled = true;
            listBox1.EndUpdate();
            textBox2.Enabled = true;            
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.Text = null;
            textBox1.ForeColor = Color.Black;
        }
        async Task SendMessageAsync(StreamWriter writer)
        {
            // сначала отправляем имя
            if (entered) {
                await writer.WriteLineAsync(userName);
                await writer.FlushAsync();
                entered = false;
            }            
            string message = textBox2.Text;
            textBox2.Text = "";
            textBox2.Focus();
            if (message != null && message != "") {
                listBox1.BeginUpdate();
                listBox1.Items.Add($"Вы: {message}");
                listBox1.EndUpdate();
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();
            }            
        }

        async Task ReceiveMessageAsync(StreamReader reader)
        {

            // считываем ответ в виде строки
            string message = await reader.ReadLineAsync();
            // если пустой ответ, ничего не выводим на консоль
            if (!string.IsNullOrEmpty(message)) {
                listBox1.BeginUpdate();
                listBox1.Items.Add(message);
                listBox1.EndUpdate();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != null || textBox2.Text != "") SendMessageAsync(Writer);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Reader != null)
            {
                ReceiveMessageAsync(Reader);
            }
        }
    }
}
