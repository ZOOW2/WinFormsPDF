using System;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace TzPdfWinFormsOnly
{
    public partial class SearchPDF : Form
    {
        public SearchPDF()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = textBox1.Text;

            if (!string.IsNullOrWhiteSpace(searchTerm) && int.TryParse(searchTerm, out int fileNumber))
            {
                Result(fileNumber);
            }
            else
            {
                textBox2.Clear();
            }
        }

        private void Result(int fileNumber)
        {
            string connectionString = "server=localhost;user=root;password=root;database=files";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Path FROM info WHERE Name = @Number";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Number", fileNumber);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                byte[] fileData = (byte[])reader["Path"];

                                string fileText = ReadBinary(fileData);
                                textBox2.Text = fileText;
                            }
                        }
                        else
                        {
                            textBox2.Text = "No data found.";
                        }
                    }
                }
            }
        }

        private string ReadBinary(byte[] fileData)
        {
            StringBuilder text = new StringBuilder();
            try
            {
                using (PdfReader reader = new PdfReader(fileData))
                {
                    for (int page = 1; page <= reader.NumberOfPages; page++)
                    {
                        text.AppendLine(PdfTextExtractor.GetTextFromPage(reader, page));
                    }
                }
            }
            catch (Exception ex)
            {
                text.AppendLine("Error: " + ex.Message);
            }
            return text.ToString();
        }
    }
}
