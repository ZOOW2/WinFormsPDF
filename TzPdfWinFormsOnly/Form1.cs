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

        // ���������� ������� ��� ���������� ���� textBox1 (�����)
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // �������� �����, ��������� � textBox1
            string searchTerm = textBox1.Text;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // ���� � ���������� ��������� � textBox2
                SearchAndDisplayFile(searchTerm);
            }
            else
            {
                // ���� ����� ������, ������� textBox2
                textBox2.Clear();
            }
        }

        // ����� ��� ������ � ������ ������ �� �����
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

                                // ���������, ���������� �� ����
                                if (File.Exists(filePath))
                                {
                                    // ��������� ����� �� �����
                                    string fileText = ExtractTextFromPdf(filePath);

                                    // ���������� ���������� ����� � textBox2
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

        // ����� ��� ���������� ������ �� PDF �����
        private string ExtractTextFromPdf(string filePath)
        {
            // ���������� ���������� iTextSharp ��� ���������� ������
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
