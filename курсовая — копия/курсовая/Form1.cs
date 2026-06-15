using System;
using System.Windows.Forms;

namespace курсовая
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string selectedMethod = string.Empty;

            if (radioButton1.Checked)
                selectedMethod = "Сортировка пузырьком";
            else if (radioButton2.Checked)
                selectedMethod = "Сортировка вставками";
            else if (radioButton3.Checked)
                selectedMethod = "Сортировка выбором";
            else if (radioButton4.Checked)
                selectedMethod = "Быстрая сортировка";
            else if (radioButton5.Checked)
                selectedMethod = "Сортировка слиянием";
            else
            {
                MessageBox.Show("Пожалуйста, выберите метод сортировки!");
                return;
            }

            // Скрываем Form1
            this.Hide();

            // Создаем и открываем Form3
            Form3 form3 = new Form3();
            form3.SetSortMethod(selectedMethod);
            form3.ShowDialog();

            // После закрытия Form3 показываем Form1 снова
            this.Show();
        }
    }
}