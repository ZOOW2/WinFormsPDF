using System;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace TzPdfWinFormsOnly
{
    public partial class SearchPDF : Form
    {
        public SearchPDF()
        {
            InitializeComponent();
        }

        // Обработчик события для текстового поля textBox1 (поиск)
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Получаем текст, введенный в textBox1
            string searchTerm = textBox1.Text;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Ищем и отображаем результат в textBox2
                SearchAndDisplayFile(searchTerm);
            }
            else
            {
                // Если текст пустой, очищаем textBox2
                textBox2.Clear();
            }
        }

        // Метод для поиска и вывода данных из файла
        private void SearchAndDisplayFile(string fileNumber)
        {
            string connectionString = "server=localhost;user=root;password=root;database=pdf";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Path FROM info WHERE ID = @ID";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", fileNumber);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string filePath = reader.GetString("Path");

                                // Проверяем, существует ли файл
                                if (File.Exists(filePath))
                                {
                                    // Извлекаем текст из файла
                                    string fileText = ExtractTextFromPdf(filePath);

                                    // Отображаем содержимое файла в textBox2
                                    textBox2.Text = fileText;
                                }
                                else
                                {
                                    textBox2.Text = "File not found.";
                                }
                            }
                        }
                        else
                        {
                            textBox2.Text = "No data found for this number.";
                        }
                    }
                }
            }
        }

        // Метод для извлечения текста из PDF файла
        private string ExtractTextFromPdf(string filePath)
        {
            // Используем библиотеку iTextSharp для извлечения текста
            StringBuilder text = new StringBuilder();
            try
            {
                using (iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(filePath))
                {
                    for (int page = 1; page <= reader.NumberOfPages; page++)
                    {
                        text.AppendLine(iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page));
                    }
                }
            }
            catch (Exception ex)
            {
                text.AppendLine("Error reading PDF: " + ex.Message);
            }
            return text.ToString();
        }
    }
}
