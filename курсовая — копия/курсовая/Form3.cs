using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace курсовая
{
    public partial class Form3 : Form
    {
        private string selectedSortMethod = "Сортировка пузырьком";


        public Form3()
        {
            InitializeComponent();
        }

        // Метод для установки выбранного метода сортировки из главной формы
        public void SetSortMethod(string method)
        {
            selectedSortMethod = method;
        }

        // Обработчик кнопки "Сортировать"
        private void button1_Click(object sender, EventArgs e)
        {
            int arraySize;
            // Проверяем, что введено корректное целое положительное число
            if (!int.TryParse(textBox1.Text, out arraySize) || arraySize <= 0)
            {
                MessageBox.Show("Введите корректный размер массива!");
                return;
            }

            // Проверяем, что размер массива не превышает максимально допустимый (100 000)
            if (arraySize > 100000)
            {
                MessageBox.Show("Размер массива не может превышать 100 000!",
                              "Ошибка",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
                return;
            }

            int[] array = null;

            // В зависимости от выбранного переключателя создаем массив:
            if (radioButton1.Checked)        // Случайная генерация
                array = GenerateRandomArray(arraySize);
            else if (radioButton2.Checked)   // Ручной ввод с клавиатуры
                array = GetManualArray();
            else if (radioButton3.Checked)   // Загрузка из файла
                array = LoadArrayFromFile(arraySize);

            // Если массив не создан (ошибка), выводим сообщение и выходим
            if (array == null)
            {
                MessageBox.Show("Не удалось создать массив!");
                return;
            }

            // Вывод исходного массива в listBox1 с переносом строк
            listBox1.Items.Clear();
            DisplayArrayInListBox(listBox1, array);

            // Замеряем время выполнения сортировки с помощью Stopwatch
            Stopwatch stopwatch = Stopwatch.StartNew();
            int comparisons = 0, swaps = 0;  // Счетчики сравнений и перестановок
            int[] sortedArray = PerformSorting(array, ref comparisons, ref swaps);
            stopwatch.Stop();  // Останавливаем таймер

            // Вывод отсортированного массива в listBox2 с переносом строк
            listBox2.Items.Clear();
            DisplayArrayInListBox(listBox2, sortedArray);

            // Отображаем результаты сортировки: время, количество сравнений и перестановок
            label5.Text = $"Время сортировки: {stopwatch.Elapsed.TotalMilliseconds:F2} мс";
            label6.Text = $"Количество сравнений: {comparisons}";
            label7.Text = $"Количество перестановок: {swaps}";
        }

        // Метод для вывода массива в ListBox с переносом строк
        private void DisplayArrayInListBox(ListBox listBox, int[] array)
        {
            const int numbersPerLine = 10; // Количество чисел в одной строке
            StringBuilder line = new StringBuilder();

            for (int i = 0; i < array.Length; i++)
            {
                // Добавляем число
                line.Append(array[i].ToString());

                // Добавляем пробел между числами (кроме последнего числа в строке)
                if ((i + 1) % numbersPerLine != 0 && i != array.Length - 1)
                {
                    line.Append("  "); // Два пробела для лучшей читаемости
                }

                // Если достигли конца строки или конец массива
                if ((i + 1) % numbersPerLine == 0 || i == array.Length - 1)
                {
                    // Добавляем строку в ListBox
                    listBox.Items.Add(line.ToString());
                    line.Clear(); // Очищаем для следующей строки
                }
            }
        }

        // Обработчик кнопки "На главную" - закрывает текущую форму
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Генерация случайного массива заданного размера
        // Каждый элемент - случайное число от 1 до 999
        private int[] GenerateRandomArray(int size)
        {
            Random rand = new Random();
            int[] arr = new int[size];
            for (int i = 0; i < size; i++)
                arr[i] = rand.Next(1, 1000);  // Генерируем числа от 1 до 999
            return arr;
        }

        // Ручной ввод массива пользователем через отдельную диалоговую форму
        private int[] GetManualArray()
        {
            // Создаем вспомогательную форму для ввода
            Form inputForm = new Form();
            inputForm.Text = "Ввод массива";
            inputForm.Size = new System.Drawing.Size(400, 150);

            // Поле для ввода текста
            TextBox textBox = new TextBox() { Left = 20, Top = 20, Width = 350 };
            // Кнопка подтверждения
            Button button = new Button() { Text = "OK", Left = 150, Top = 60, Width = 100, DialogResult = DialogResult.OK };

            inputForm.Controls.Add(textBox);
            inputForm.Controls.Add(button);

            // Если пользователь нажал OK, обрабатываем введенные данные
            if (inputForm.ShowDialog() == DialogResult.OK)
            {
                // Разделяем строку по пробелам на отдельные числа
                string[] parts = textBox.Text.Split(' ');
                int[] arr = new int[parts.Length];
                for (int i = 0; i < parts.Length; i++)
                    arr[i] = int.Parse(parts[i]);  // Преобразуем строки в числа
                return arr;
            }
            return null;  // Пользователь отменил ввод
        }

        // Загрузка массива из текстового файла
        // requiredSize - требуемый размер массива (из textBox1)
        private int[] LoadArrayFromFile(int requiredSize)
        {
            // Диалог выбора файла
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            dialog.Title = "Выберите файл с массивом";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Читаем всё содержимое файла
                    string content = File.ReadAllText(dialog.FileName);

                    // Разделяем содержимое на отдельные числа
                    // Разделители: пробелы, запятые, переносы строк, табуляция
                    string[] numbers = content.Split(new char[] { ' ', ',', '\n', '\r', '\t' },
                                                     StringSplitOptions.RemoveEmptyEntries);

                    // Если в файле чисел меньше, чем требуется, показываем предупреждение
                    // и берем столько, сколько есть
                    if (numbers.Length < requiredSize)
                    {
                        MessageBox.Show($"В файле найдено только {numbers.Length} чисел, а требуется {requiredSize}.\n" +
                                      $"Будет использовано {numbers.Length} чисел.",
                                      "Предупреждение",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Warning);
                        requiredSize = numbers.Length;
                    }

                    // Создаем массив нужного размера
                    int[] arr = new int[requiredSize];

                    // Заполняем массив первыми requiredSize числами из файла
                    for (int i = 0; i < requiredSize; i++)
                    {
                        // Пытаемся преобразовать строку в число
                        if (int.TryParse(numbers[i], out int value))
                        {
                            arr[i] = value;
                        }
                        else
                        {
                            // Если в файле встретилось не число, выводим ошибку
                            MessageBox.Show($"Ошибка при чтении числа '{numbers[i]}' в позиции {i + 1}.\n" +
                                          "Убедитесь, что файл содержит только целые числа.",
                                          "Ошибка",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Error);
                            return null;
                        }
                    }

                    return arr;
                }
                catch (Exception ex)
                {
                    // Обрабатываем возможные ошибки при чтении файла
                    MessageBox.Show($"Ошибка при чтении файла: {ex.Message}",
                                  "Ошибка",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                    return null;
                }
            }
            return null;  // Пользователь не выбрал файл
        }

        // Выполнение сортировки выбранным методом
        private int[] PerformSorting(int[] arr, ref int comparisons, ref int swaps)
        {
            // Создаем копию исходного массива, чтобы не изменять оригинал
            int[] result = (int[])arr.Clone();

            // Выбираем метод сортировки в зависимости от значения selectedSortMethod
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

        // Сортировка пузырьком (Bubble Sort)
        // На каждом проходе самый большой элемент "всплывает" в конец массива
        private void BubbleSort(int[] arr, ref int comparisons, ref int swaps)
        {
            for (int i = 0; i < arr.Length - 1; i++)
                for (int j = 0; j < arr.Length - i - 1; j++)
                {
                    comparisons++;  // Увеличиваем счетчик сравнений
                    if (arr[j] > arr[j + 1])  // Если элементы не в порядке возрастания
                    {
                        swaps++;  // Увеличиваем счетчик перестановок
                        // Меняем элементы местами
                        int temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                    }
                }
        }

        // Сортировка вставками (Insertion Sort)
        // Каждый новый элемент вставляется в уже отсортированную часть массива
        private void InsertionSort(int[] arr, ref int comparisons, ref int swaps)
        {
            for (int i = 1; i < arr.Length; i++)
            {
                int key = arr[i];  // Запоминаем текущий элемент
                int j = i - 1;
                // Сдвигаем элементы, которые больше key, вправо
                while (j >= 0 && arr[j] > key)
                {
                    comparisons++;  // Увеличиваем счетчик сравнений
                    arr[j + 1] = arr[j];
                    j--;
                    swaps++;  // Каждый сдвиг считаем перестановкой
                }
                comparisons++;  // Последнее сравнение, которое завершило цикл
                arr[j + 1] = key;  // Вставляем элемент на правильное место
            }
        }

        // Сортировка выбором (Selection Sort)
        // На каждой итерации находим минимальный элемент и ставим его в начало
        private void SelectionSort(int[] arr, ref int comparisons, ref int swaps)
        {
            for (int i = 0; i < arr.Length - 1; i++)
            {
                int minIndex = i;  // Индекс минимального элемента
                for (int j = i + 1; j < arr.Length; j++)
                {
                    comparisons++;  // Увеличиваем счетчик сравнений
                    if (arr[j] < arr[minIndex])
                        minIndex = j;  // Нашли новый минимальный элемент
                }
                // Если минимальный элемент не на своем месте, меняем их местами
                if (minIndex != i)
                {
                    swaps++;  // Увеличиваем счетчик перестановок
                    int temp = arr[i];
                    arr[i] = arr[minIndex];
                    arr[minIndex] = temp;
                }
            }
        }

        // Быстрая сортировка (Quick Sort) - рекурсивный алгоритм
        // Выбирает опорный элемент и разделяет массив на части: меньше опорного и больше
        private void QuickSort(int[] arr, int left, int right, ref int comparisons, ref int swaps)
        {
            if (left < right)
            {
                int pivot = arr[right];  // Опорный элемент - последний в текущей части
                int i = left - 1;

                // Разделяем массив: элементы <= pivot перемещаем в левую часть
                for (int j = left; j < right; j++)
                {
                    comparisons++;  // Увеличиваем счетчик сравнений
                    if (arr[j] <= pivot)
                    {
                        i++;
                        swaps++;  // Увеличиваем счетчик перестановок
                        int temp = arr[i];
                        arr[i] = arr[j];
                        arr[j] = temp;
                    }
                }

                // Ставим опорный элемент на правильное место
                swaps++;
                int temp2 = arr[i + 1];
                arr[i + 1] = arr[right];
                arr[right] = temp2;

                int pivotIndex = i + 1;  // Индекс опорного элемента
                // Рекурсивно сортируем левую и правую части
                QuickSort(arr, left, pivotIndex - 1, ref comparisons, ref swaps);
                QuickSort(arr, pivotIndex + 1, right, ref comparisons, ref swaps);
            }
        }

        // Сортировка слиянием (Merge Sort) - рекурсивный алгоритм
        // Делит массив на две части, сортирует их рекурсивно, затем сливает
        private void MergeSort(int[] arr, int left, int right, ref int comparisons, ref int swaps)
        {
            if (left < right)
            {
                int mid = (left + right) / 2;  // Находим середину
                // Рекурсивно сортируем левую и правую половины
                MergeSort(arr, left, mid, ref comparisons, ref swaps);
                MergeSort(arr, mid + 1, right, ref comparisons, ref swaps);

                // Слияние двух отсортированных половин
                int[] temp = new int[right - left + 1];  // Временный массив
                int i = left, j = mid + 1, k = 0;

                // Сливаем два массива, выбирая меньший элемент из каждой части
                while (i <= mid && j <= right)
                {
                    comparisons++;  // Увеличиваем счетчик сравнений
                    if (arr[i] <= arr[j])
                        temp[k++] = arr[i++];
                    else
                        temp[k++] = arr[j++];
                    swaps++;  // Каждое копирование считаем перестановкой
                }

                // Копируем оставшиеся элементы из левой части
                while (i <= mid)
                {
                    temp[k++] = arr[i++];
                    swaps++;
                }

                // Копируем оставшиеся элементы из правой части
                while (j <= right)
                {
                    temp[k++] = arr[j++];
                    swaps++;
                }

                // Копируем временный массив обратно в исходный
                for (i = 0; i < temp.Length; i++)
                {
                    arr[left + i] = temp[i];
                    swaps++;
                }
            }
        }

        // Обработчик события нажатия клавиш в поле ввода размера массива
        // Ограничивает ввод: только цифры, запрещает минус и числа больше 100 000
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем ввод цифр, backspace (код 8) и другие управляющие символы
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && !char.IsControl(e.KeyChar))
                e.KeyChar = (char)0;  // Блокируем ввод

            // Запрещаем ввод знака минус полностью
            if (e.KeyChar == '-')
                e.KeyChar = (char)0;

            // Проверяем, не превысит ли число максимально допустимое значение (100 000)
            if (char.IsDigit(e.KeyChar))
            {
                string currentText = textBox1.Text;
                int selectionStart = textBox1.SelectionStart;
                int selectionLength = textBox1.SelectionLength;

                // Моделируем текст, который получится после ввода символа
                string newText = currentText.Remove(selectionStart, selectionLength)
                                              .Insert(selectionStart, e.KeyChar.ToString());

                // Если строка не пустая, проверяем значение
                if (!string.IsNullOrEmpty(newText))
                {
                    if (long.TryParse(newText, out long newValue))
                    {
                        if (newValue > 100000)  // Если число превышает лимит
                        {
                            e.KeyChar = (char)0;  // Блокируем ввод
                            MessageBox.Show("Размер массива не может превышать 100 000!",
                                          "Предупреждение",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Warning);
                        }
                    }
                }
            }
        }
    }
}