using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using System.Globalization;

using App0.M;
using App0.V;
using App0.M.Product;

namespace App0.P
{
    /// <summary>
    /// Менеджер
    /// </summary>
    class Presenter
    {
        /// <summary>
        /// Делегат для кольорового виведення
        /// </summary>
        /// <param name="s">рядок</param>
        /// <param name="color">колір</param>
        private delegate void PrintInConsole(string s, ConsoleColor color);
        private readonly PrintInConsole delPrint;

        /// <summary>
        /// Дані
        /// </summary>
        private readonly Model model;
        /// <summary>
        /// Вигляд
        /// </summary>
        private readonly View view;
        private SOrder data = new SOrder();

        /// <summary>
        /// Список доступних напоїв
        /// </summary>
        public List<IDrink> Drinks { get; private set; }
            = new List<IDrink>();
        /// <summary>
        /// Список доступних добавок
        /// </summary>
        public List<IAdditiv> Aditivs { get; private set; }
            = new List<IAdditiv>();
        /// <summary>
        /// Культура
        /// </summary>
        public RegionInfo Region
            => model.Region;
        /// <summary>
        /// Набір стаканчиків
        /// </summary>
        public int[] Glasses
            => model.Glasses;

        /// <summary>
        /// Загальне замовлення
        /// </summary>
        public SOrder Data
        {
            get { return data; }
            private set { data = value; }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="path">Адреса директорії в якій знаходиться БД</param>
        /// <param name="file">Назва файла БД без розширення</param>
        public Presenter(string path, string file)
        {
            // підключення моделі
            this.model = new Model(path, file, Model.Currency.Dollar);

            // завантаження даних з БД, лише після того як наявні 
            // дані можна запускати побудову інтерфейсу
            DownloadData();

            // підключення виду
            this.view = new View(this);

            // зв'язування/підключення подій
            this.model.ChangeDB += DownloadData;
            this.view.PressedN += ClearData;
            this.view.PressedG += SizeGlass;
            this.view.PressedD += Pour;
            this.view.PressedA += Insert;
            this.view.PressedP += Pay;

            // зв'язування метода
            delPrint = this.view.Print;
        }

        /// <summary>
        /// Очищення замовлення
        /// </summary>
        public void ClearData()
            => data.Clear();

        /// <summary>
        /// Вкинути добавки
        /// </summary>
        private void Insert()
        {
            if (Aditivs.Count == 0)
            {
                return;
            }

            // Блокуємо доступ іншим потокам
            lock (this.view.Block)
            {
                do
                {
                    // оновлення заповлення
                    this.view.Order();

                    // установка курсора
                    Console.SetCursorPosition(2, 17 + Math.Max(Drinks.Count, Aditivs.Count));
                    // очистка
                    Console.Write(new string(' ', Console.WindowWidth - 4));
                    // установка курсора
                    Console.SetCursorPosition(2, 17 + Math.Max(Drinks.Count, Aditivs.Count));

                    // зміна кольору
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    // Вивід
                    Console.Write("Выберите добавку: ");
                    // скидання налаштувань
                    Console.ResetColor();

                    // введення
                    string key = Console.ReadLine();

                    // при натисканні виходу
                    if ((key.ToLower() == ConsoleKey.Q.ToString().ToLower()))
                    {
                        return;
                    }

                    // спроба перевести в числове значення, а next сигналізує вірність вводу
                    int position = 0;
                    bool next = int.TryParse(key, out position);

                    // аналіз чи можна продовжувати, якщо так то записуємо вибір користувача
                    // також аналізуємо чи введені дані у допустимому діапазоні
                    if (!next || !(0 < position && position <= Aditivs.Count))
                    {
                        continue;
                    }

                    // аналіз вибраної добавки, якщо така добавка вже є, 
                    // то закодимо в меню управління нею, а якщо нема то просто додаємо
                    if (!Data.GetAdditivs().Contains(Aditivs[position - 1]))
                    {
                        // додавання добавки
                        data.AddAdditiv(Aditivs[position - 1]);
                    }

                    // якщо така добавка є то заходимо в меню щоб керувати нею
                    if (Data.GetAdditivs().Contains(Aditivs[position - 1]))
                    {
                        do
                        {
                            // оновлення заповлення
                            this.view.Order();

                            // установка курсора
                            Console.SetCursorPosition(2, 17 + Math.Max(Drinks.Count, Aditivs.Count));
                            // очистка
                            Console.Write(new string(' ', Console.WindowWidth - 4));
                            // установка курсора
                            Console.SetCursorPosition(2, 17 + Math.Max(Drinks.Count, Aditivs.Count));

                            #region Керування рівнем добавки
                            delPrint("Выберите: ", ConsoleColor.White);
                            delPrint("N - очистить, ", ConsoleColor.Yellow);
                            delPrint("+ добавить, ", ConsoleColor.Green);
                            delPrint("- удалить, ", ConsoleColor.Cyan);
                            delPrint("Q - назад. ", ConsoleColor.Red);

                            // введення клавіши
                            string key0 = Console.ReadLine().ToLower();

                            // дія згідно вибору (в даному випадку не "switch" так як 
                            // необхідно вийти лише з цього цикла а не методу)
                            if (key0 == "n") // очистить
                            {
                                data.RemoveAdditivs();
                            }
                            else if (key0 == "+")
                            {
                                data.AddAdditiv(Aditivs[position - 1]);
                            }
                            else if (key0 == "-")
                            {
                                data.RemoveAdditiv(Aditivs[position - 1]);
                            }
                            else if (key0 == "q")
                            {
                                break;
                            }
                            #endregion
                        } while (true);
                    }

                } while (true);
            }
        }

        /// <summary>
        /// Налити напій
        /// </summary>
        private void Pour()
        {
            if (Drinks.Count == 0)
            {
                return;
            }

            // Блокуємо доступ іншим потокам
            lock (this.view.Block)
            {
                do
                {
                    // оновлення заповлення
                    this.view.Order();

                    // установка курсора
                    Console.SetCursorPosition(1, 17 + Math.Max(Drinks.Count, Aditivs.Count));
                    // очистка
                    Console.Write(new string(' ', Console.WindowWidth - 2));
                    // установка курсора
                    Console.SetCursorPosition(2, 17 + Math.Max(Drinks.Count, Aditivs.Count));

                    // якщо нопою нема - вибираємо і додаємо, а якщо є, то або видаляємо
                    // або додаємо до нього такий же
                    if (Data.GetDrinks().Count == 0)
                    {
                        #region Новий напій
                        // зміна кольору
                        Console.ForegroundColor = ConsoleColor.Cyan;

                        // Вивід
                        Console.Write("Выберите напиток: ");

                        // скидання налаштувань
                        Console.ResetColor();

                        // введення
                        string key = Console.ReadLine();

                        // при натисканні виходу
                        if ((key.ToLower() == ConsoleKey.Q.ToString().ToLower()))
                        {
                            return;
                        }

                        // спроба перевести в числове значення, а next сигналізує вірність вводу
                        int position = 0;
                        bool next = int.TryParse(key, out position);

                        // аналіз чи можна продовжувати, якщо так то записуємо вибір користувача
                        // також аналізуємо чи введені дані у допустимому діапазоні
                        if (next && 0 < position && position <= Drinks.Count)
                        {
                            data.AddDrink(Drinks[position - 1]);
                        }
                        #endregion
                    }

                    // керування розміром напою
                    if (Data.GetDrinks().Count > 0)
                    {
                        do
                        {
                            // оновлення заповлення
                            this.view.Order();

                            // установка курсора
                            Console.SetCursorPosition(1, 17 + Math.Max(Drinks.Count, Aditivs.Count));
                            // очистка
                            Console.Write(new string(' ', Console.WindowWidth - 2));
                            // установка курсора
                            Console.SetCursorPosition(2, 17 + Math.Max(Drinks.Count, Aditivs.Count));

                            #region Керування рівнем напою
                            delPrint("Выберите: ", ConsoleColor.White);
                            delPrint("N - очистить, ", ConsoleColor.Yellow);
                            delPrint("+ добавить, ", ConsoleColor.Green);
                            delPrint("- удалить, ", ConsoleColor.Cyan);
                            delPrint("Q - назад. ", ConsoleColor.Red);

                            // введення клавіши
                            string key = null;
                            key = Console.ReadLine().ToLower();

                            // дія згідно вибору
                            switch (key)
                            {
                                case "n":  // очистить
                                    data.RemoveDrinks();
                                    break;
                                case "+":  // додати
                                    data.AddDrink(Data.GetDrinks()[0]);
                                    break;
                                case "-":  // убрати
                                    data.RemoveDrink();
                                    break;
                                case "q":  // виход
                                    return;
                            }

                            // якщо ми все видалили то необхідно піти в меню вище
                            if (Data.GetDrinks().Count == 0)
                            {
                                break;
                            }
                            #endregion
                        } while (true);
                    }
                } while (true);
            }
        }

        /// <summary>
        /// Вибір розмірів стаканчика
        /// </summary>
        /// <returns></returns>
        private void SizeGlass()
        {
            // набір стаканчиків
            TypeOfGlass[] value = Enum.GetValues(typeof(TypeOfGlass)).Cast<TypeOfGlass>().ToArray();

            // Блокуємо доступ іншим потокам
            lock (this.view.Block)
            {
                // перевірка введеного значення
                do
                {
                    // установка курсора
                    Console.SetCursorPosition(1, 17 + Math.Max(Drinks.Count, Aditivs.Count));
                    // очистка
                    Console.Write(new string(' ', Console.WindowWidth - 2));
                    // установка курсора
                    Console.SetCursorPosition(2, 17 + Math.Max(Drinks.Count, Aditivs.Count));

                    // зміна кольору
                    Console.ForegroundColor = ConsoleColor.Green;

                    // Вивід
                    Console.Write("Выберите размер стаканчика: ");

                    // скидання налаштувань
                    Console.ResetColor();

                    // введення
                    string key = Console.ReadLine();

                    // Примітка. Згідно даної умови і даних, можна було б
                    // написати обробку по натисканні однієї клавіши, але
                    // програма розраховується на умову коли в БД буде 
                    // більше 10 одиниць вибору продуктів

                    // при натисканні виходу 
                    if ((key.ToLower() == ConsoleKey.Q.ToString().ToLower()))
                    {
                        return;
                    }

                    // спроба перевести в числове значення, а next сигналізує вірність вводу
                    int position = 0;
                    bool next = int.TryParse(key, out position);

                    // аналіз чи можна продовжувати, якщо так то записуємо вибір користувача
                    // також аналізуємо чи введені дані у допустимому діапазоні
                    if (next && 0 < position && position <= value.Length)
                    {
                        data.SetGlass(value[position - 1]);
                        return;
                    }

                } while (true);
            }
        }

        /// <summary>
        /// Оплата замовлення
        /// </summary>
        private void Pay()
        {
            lock (this.view.Block)
            {
                // установка курсора
                Console.SetCursorPosition(1, 17 + Math.Max(Drinks.Count, Aditivs.Count));

                // очистка
                Console.Write(new string(' ', Console.WindowWidth - 2));

                // установка курсора
                Console.SetCursorPosition(2, 17 + Math.Max(Drinks.Count, Aditivs.Count));

                // Вивід
                delPrint("Спасибо за оплату. Нажмите клавишу для продолжения...", ConsoleColor.Green);
            }

            // очікування натискання клавіші
            Console.Read();
            // Примітка. При вложених методах в яких на різних рівнях
            // використовується Console.ReadKey(); вилітає на самий вищий рівень
            // тому у рівнях нижче не варно використовувати "ReadKey"

            // Збереження в лог-файд
            string info = new StringBuilder()
                .Append($"Price: {Data.Price.ToString("C2", new CultureInfo(Region.Name))}, ")
                .Append($"Products: {this.view.InfoOrder}")
                .ToString();

            new Thread(() => SaveLog.SaveOrder(info)).Start();

            // чистка даних
            ClearData();
        }

        /// <summary>
        /// Завантаження даних (таблиць продуктів)
        /// </summary>
        private void DownloadData()
        {
            // 
            lock (LoadDataBase.Block)
            {
                // завантаження даних з таблиць
                ExstractingData(LoadDataBase.Products.Tables["Drinks"], Drinks);
                ExstractingData(LoadDataBase.Products.Tables["Additivs"], Aditivs);
            }

            // оновлюємо меню
            this.view?.Menu();

            // Збереження в лог-файд
            string info = new StringBuilder()
                .Append($"Glasses - {Enum.GetValues(typeof(TypeOfGlass)).Length}, ")
                .Append($"Drinks - {Drinks.Count}, ")
                .Append($"Additivs - {Aditivs.Count};")
                .ToString();

            new Thread(() => SaveLog.SaveUpdate(info)).Start();
        }

        /// <summary>
        /// Витягування даних
        /// </summary>
        /// <typeparam name="T">Тип вихідних даних</typeparam>
        /// <param name="table">Таблиця з даними</param>
        /// <returns></returns>
        private void ExstractingData<T>(DataTable table, List<T> products)
            where T : IProduct
        {
            // очищення списку
            products.Clear();

            try
            {
                // тимчасова змінна
                IProduct temp;

                // перебираємо всі рядки в таблиці
                foreach (DataRow row in table.Rows)
                {
                    // заносимо дані в колекції
                    if (typeof(T).Equals(typeof(IDrink)))
                    {
                        temp = new SDrink((int)row["ID"], (string)row["Name"],
                            Math.Abs((float)row["Price"]), Math.Abs((float)row["SizeOf"]),
                            ((bool)row["TypeOf"]).ConvertToTypeValue());

                    }
                    else if (typeof(T).Equals(typeof(IAdditiv)))
                    {
                        temp = new SAdditiv((int)row["ID"], (string)row["Name"],
                            Math.Abs((float)row["Price"]), Math.Abs((float)row["SizeOf"]),
                            ((bool)row["TypeOf"]).ConvertToTypeValue());
                    }
                    else
                    {
                        throw new Exception("This type is not valid");
                    }

                    products.Add((T)temp);
                }
            }
            catch (ArgumentException ex)
            {
                // сохранить в лог-файл
                Console.WriteLine(ex.Message);
            }
        }

    }
}
