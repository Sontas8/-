using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Microsoft.VisualBasic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace курсовая
{
    public partial class Form3 : Form
    {
        private string selectedSortMethod = "Сортировка пузырьком";
        private int elementsPerLine = 25; // Количество элементов в одной строке


        public Form3()
        {
            InitializeComponent();
        }
        public void SetSortMethod(string method)
        {
            selectedSortMethod = method;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int arraySize;
            if (!int.TryParse(textBox1.Text, out arraySize) || arraySize <= 0)
            {
                MessageBox.Show("Введите корректный размер массива!");
                return;
            }

            int[] array = null;

            if (radioButton1.Checked)
                array = GenerateRandomArray(arraySize);
            else if (radioButton2.Checked)
                array = GetManualArray();
            else if (radioButton3.Checked)
                array = LoadArrayFromFile();

            if (array == null)
            {
                MessageBox.Show("Не удалось создать массив!");
                return;
            }

            // Вывод исходного массива строками по 17 элементов
            listBox1.Items.Clear();
            for (int i = 0; i < array.Length; i += 17)
            {
                string line = "";
                for (int j = i; j < i + 17 && j < array.Length; j++)
                {
                    if (j > i) line += " ";
                    line += array[j].ToString();
                }
                listBox1.Items.Add(line);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            int comparisons = 0, swaps = 0;
            int[] sortedArray = PerformSorting(array, ref comparisons, ref swaps);
            stopwatch.Stop();

            // Вывод отсортированного массива строками по 17 элементов
            listBox2.Items.Clear();
            for (int i = 0; i < sortedArray.Length; i += 17)
            {
                string line = "";
                for (int j = i; j < i + 17 && j < sortedArray.Length; j++)
                {
                    if (j > i) line += " ";
                    line += sortedArray[j].ToString();
                }
                listBox2.Items.Add(line);
            }

            label5.Text = $"Время сортировки: {stopwatch.Elapsed.TotalMilliseconds:F2} мс";
            label6.Text = $"Количество сравнений: {comparisons}";
            label7.Text = $"Количество перестановок: {swaps}";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Кнопка "На главную" - закрываем Form3 и возвращаемся на Form1
            this.Close();
        }

        private int[] GenerateRandomArray(int size)
        {
            Random rand = new Random();
            int[] arr = new int[size];
            for (int i = 0; i < size; i++)
                arr[i] = rand.Next(1, 100);
            return arr;
        }

        private int[] GetManualArray()
        {
            Form inputForm = new Form();
            inputForm.Text = "Ввод массива";
            inputForm.Size = new System.Drawing.Size(400, 150);

            TextBox textBox = new TextBox() { Left = 20, Top = 20, Width = 350 };
            Button button = new Button() { Text = "OK", Left = 150, Top = 60, Width = 100, DialogResult = DialogResult.OK };

            inputForm.Controls.Add(textBox);
            inputForm.Controls.Add(button);

            if (inputForm.ShowDialog() == DialogResult.OK)
            {
                string[] parts = textBox.Text.Split(' ');
                int[] arr = new int[parts.Length];
                for (int i = 0; i < parts.Length; i++)
                    arr[i] = int.Parse(parts[i]);
                return arr;
            }
            return null;
        }

        private int[] LoadArrayFromFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string[] lines = File.ReadAllLines(dialog.FileName);
                int[] arr = new int[lines.Length];
                for (int i = 0; i < lines.Length; i++)
                    arr[i] = int.Parse(lines[i]);
                return arr;
            }
            return null;
        }

        private int[] PerformSorting(int[] arr, ref int comparisons, ref int swaps)
        {
            int[] result = (int[])arr.Clone();

            switch (selectedSortMethod)
            {
                case "Сортировка пузырьком":
                    BubbleSort(result, ref comparisons, ref swaps);
                    break;
                case "Сортировка вставками":
                    InsertionSort(result, ref comparisons, ref swaps);
                    break;
                case "Сортировка выбором":
                    SelectionSort(result, ref comparisons, ref swaps);
                    break;
                case "Быстрая сортировка":
                    QuickSort(result, 0, result.Length - 1, ref comparisons, ref swaps);
                    break;
                case "Сортировка слиянием":
                    MergeSort(result, 0, result.Length - 1, ref comparisons, ref swaps);
                    break;
            }

            return result;
        }

        private void BubbleSort(int[] arr, ref int comparisons, ref int swaps)
        {
            for (int i = 0; i < arr.Length - 1; i++)
                for (int j = 0; j < arr.Length - i - 1; j++)
                {
                    comparisons++;
                    if (arr[j] > arr[j + 1])
                    {
                        swaps++;
                        int temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                    }
                }
        }

        private void InsertionSort(int[] arr, ref int comparisons, ref int swaps)
        {
            for (int i = 1; i < arr.Length; i++)
            {
                int key = arr[i];
                int j = i - 1;
                while (j >= 0 && arr[j] > key)
                {
                    comparisons++;
                    arr[j + 1] = arr[j];
                    j--;
                    swaps++;
                }
                comparisons++;
                arr[j + 1] = key;
            }
        }

        private void SelectionSort(int[] arr, ref int comparisons, ref int swaps)
        {
            for (int i = 0; i < arr.Length - 1; i++)
            {
                int minIndex = i;
                for (int j = i + 1; j < arr.Length; j++)
                {
                    comparisons++;
                    if (arr[j] < arr[minIndex])
                        minIndex = j;
                }
                if (minIndex != i)
                {
                    swaps++;
                    int temp = arr[i];
                    arr[i] = arr[minIndex];
                    arr[minIndex] = temp;
                }
            }
        }

        private void QuickSort(int[] arr, int left, int right, ref int comparisons, ref int swaps)
        {
            if (left < right)
            {
                int pivot = arr[right];
                int i = left - 1;

                for (int j = left; j < right; j++)
                {
                    comparisons++;
                    if (arr[j] <= pivot)
                    {
                        i++;
                        swaps++;
                        int temp = arr[i];
                        arr[i] = arr[j];
                        arr[j] = temp;
                    }
                }

                swaps++;
                int temp2 = arr[i + 1];
                arr[i + 1] = arr[right];
                arr[right] = temp2;

                int pivotIndex = i + 1;
                QuickSort(arr, left, pivotIndex - 1, ref comparisons, ref swaps);
                QuickSort(arr, pivotIndex + 1, right, ref comparisons, ref swaps);
            }
        }

        private void MergeSort(int[] arr, int left, int right, ref int comparisons, ref int swaps)
        {
            if (left < right)
            {
                int mid = (left + right) / 2;
                MergeSort(arr, left, mid, ref comparisons, ref swaps);
                MergeSort(arr, mid + 1, right, ref comparisons, ref swaps);

                int[] temp = new int[right - left + 1];
                int i = left, j = mid + 1, k = 0;

                while (i <= mid && j <= right)
                {
                    comparisons++;
                    if (arr[i] <= arr[j])
                        temp[k++] = arr[i++];
                    else
                        temp[k++] = arr[j++];
                    swaps++;
                }

                while (i <= mid)
                {
                    temp[k++] = arr[i++];
                    swaps++;
                }

                while (j <= right)
                {
                    temp[k++] = arr[j++];
                    swaps++;
                }

                for (i = 0; i < temp.Length; i++)
                {
                    arr[left + i] = temp[i];
                    swaps++;
                }
            }
        }
    }
}