using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using App0.Product;
using System.Data;
using System.Globalization;

namespace App0.Calculate
{
    /// <summary>
    /// Обслуговування запитів клієнта
    /// </summary>
    class Service
    {
        /// <summary>
        /// Делегат на відслідковування зміни параметрів консолі і БД
        /// </summary>
        private delegate void ChangeData();
        /// <summary>
        /// Подія яка відбувається при зміні розмірів вікна консолі
        /// </summary>
        private event ChangeData ChangeSize;
        /// <summary>
        /// Подія яка відбувається при зміні БД
        /// </summary>
        private event ChangeData ChangeDB;

        /// <summary>
        /// для синхронного доступу до консолі
        /// </summary>
        private readonly object block = new object();
        /// <summary>
        /// Список доступних напоїв
        /// </summary>
        private List<IDrink> drinks = new List<IDrink>();
        /// <summary>
        /// Список доступних добавок
        /// </summary>
        private List<IAdditiv> aditivs = new List<IAdditiv>();
        /// <summary>
        /// Установка культури
        /// </summary>
        private readonly RegionInfo region;
        /// <summary>
        /// Загальне замовлення
        /// </summary>
        private SOrder data;
        /// <summary>
        /// Дані по замовленню, необхідні для лог-файла
        /// </summary>
        private string infoOrder = string.Empty;

        /// <summary>
        /// Конструктор діалогу
        /// </summary>
        public Service(RegionInfo region)
        {
            // Установка культури
            this.region = region;

            // завантаження даних з БД, лише після того як наявні 
            // дані можна запускати побудову інтерфейсу
            DownloadData();

            // запуск відображення меню
            new Thread(Menu).Start();

            // підписи на події
            ChangeSize += Menu;         // оновлення меню
            ChangeDB += DownloadData;   // оновлення БД

            // запуск відслідковування змін
            new Thread(CheckerChange).Start();
        }

        /// <summary>
        /// Меню для користувача
        /// </summary>
        public void Menu()
        {
            // Очищення консолі
            Console.Clear();

            // масив потоків
            Thread[] thread = new Thread[]
            {
                // рамка
                new Thread(Frame),
                // стаканчик
                new Thread(Glass),
                // напої
                new Thread(Drink),
                // добавки
                new Thread(Aditiv),
            };

            // запуск виконання
            for (int i = 0; i < thread.Length; i++)
            {
                thread[i].Start();
            }

            // очікуємо їх виконання
            for (int i = 0; i < thread.Length; i++)
            {
                thread[i].Join();
            }

            // запуск обробки введення даних
            new Thread(Entry).Start();
        }

        /// <summary>
        /// Відображення рамок
        /// </summary>
        private void Frame()
        {
            // Примітка. Блокування потрібно, щоб не з'являтися артефакти 
            // при масштабуванні вікна консолі

            // Блокуємо доступ іншим потокам
            lock (block)
            {
                // зміна кольору
                //Console.ForegroundColor = ConsoleColor.White;

                // верх і низ
                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < Console.WindowWidth; i++)
                {
                    Console.Write('#');
                }
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
                for (int i = 0; i < Console.WindowWidth; i++)
                {
                    Console.Write('#');
                }

                // ліво і право
                for (int i = 0; i < Console.WindowHeight - 1; i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write('#');
                }
                for (int i = 0; i < Console.WindowHeight - 1; i++)
                {
                    Console.SetCursorPosition(Console.WindowWidth - 1, i);
                    Console.Write('#');
                }

                // після стаканчиків
                Console.SetCursorPosition(0, 4);
                for (int i = 0; i < Console.WindowWidth; i++)
                {
                    Console.Write('#');
                }

                // висота на яку треба далі спускатись
                int height = 6 + Math.Max(drinks.Count, aditivs.Count);

                // між напоями і добавками
                for (int i = 0; i < height; i++)
                {
                    Console.SetCursorPosition(Console.WindowWidth / 2, i + 4);
                    Console.Write('#');
                }

                height += 3;
                // після продуктів
                Console.SetCursorPosition(0, height);
                for (int i = 0; i < Console.WindowWidth; i++)
                {
                    Console.Write('#');
                }

                height += 6;
                // після відображення замовлення
                Console.SetCursorPosition(0, height);
                for (int i = 0; i < Console.WindowWidth; i++)
                {
                    Console.Write('#');
                }

                // скидання налаштувань
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Відображення набору стаканчиків
        /// </summary>
        private void Glass()
        {
            // Блокуємо доступ іншим потокам
            lock (block)
            {
                // установка курсора
                Console.SetCursorPosition(2, 2);

                // зміна кольору
                Console.ForegroundColor = ConsoleColor.Green;

                // Створення масиву даних стаканів
                TypeOfGlass[] array = Enum.GetValues(typeof(TypeOfGlass))
                    .Cast<TypeOfGlass>().ToArray();

                StringBuilder glass = new StringBuilder()
                    .Append("Емкость стаканчика: ");

                // Вивід
                Console.Write(glass.ToString());

                // зміна кольору
                Console.ForegroundColor = ConsoleColor.Yellow;

                // очищення
                glass.Clear();

                {
                    int j = 1;
                    foreach (var i in array)
                    {
                        glass.Append($" {j++}) " + (int)i + ", ");
                    }
                }

                // вивід
                Console.Write(glass.ToString().Trim().TrimEnd(',') + " мл");

                // скидання налаштувань
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Відображення набору напоїв
        /// </summary>
        private void Drink()
        {
            // Блокуємо доступ іншим потокам
            lock (block)
            {
                // установка курсора
                Console.SetCursorPosition(2, 6);

                // зміна кольору
                Console.ForegroundColor = ConsoleColor.Cyan;

                StringBuilder drink = new StringBuilder()
                    .Append("Напитки:");

                // Вивід
                Console.Write(drink.ToString());

                // зміна кольору
                Console.ForegroundColor = ConsoleColor.Yellow;

                foreach (var i in drinks)
                {
                    // установка курсора
                    Console.SetCursorPosition(2, 7 + i.ID);
                    Console.Write($"  {i.ID}. {i.Name,-4} - " +
                        $"{i.Price.ToString("C2", new CultureInfo(region.Name))} " +
                        $": {i.Size,3} {((i.TypeOfValue == TypeValue.Volume) ? "мл" : "г")}");
                }

                // скидання налаштувань
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Відображення набору добавок
        /// </summary>
        private void Aditiv()
        {
            // Блокуємо доступ іншим потокам
            lock (block)
            {
                // установка курсора
                Console.SetCursorPosition(Console.WindowWidth / 2 + 2, 6);

                // зміна кольору
                Console.ForegroundColor = ConsoleColor.Magenta;

                StringBuilder aditiv = new StringBuilder()
                    .Append("Добавки:");

                // Вивід
                Console.Write(aditiv.ToString());

                // зміна кольору
                Console.ForegroundColor = ConsoleColor.Yellow;

                foreach (var i in aditivs)
                {
                    // установка курсора
                    Console.SetCursorPosition(Console.WindowWidth / 2 + 2, 7 + i.ID);
                    Console.Write($"  {i.ID}. {i.Name,-6} - " +
                        $"{i.Price.ToString("C2", new CultureInfo(region.Name))} " +
                        $": {i.Size,3} {((i.TypeOfValue == TypeValue.Volume) ? "мл" : "г")}");
                }

                // скидання налаштувань
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Виведення замовлення клієнта
        /// </summary>
        private void Order()
        {
            // Примітка. В даному випадку, розташування "чека/замовлення" з вказаними
            // даними зручне/сприятливе. Якщо ж буде необхідно розширювати асортимент 
            // продуктів/замовлення, то необхідно або враховувати величину рядка
            // замовлення і нижню частину зміщувати, або ж перенести цей пункт після
            // блоку упралінням замовлення.

            // Блокуємо доступ іншим потокам
            lock (block)
            {
                #region Продукти замовлення
                // установка курсора
                Console.SetCursorPosition(2, 11 + Math.Max(drinks.Count, aditivs.Count));
                // очистка
                Console.Write(new string(' ', Console.WindowWidth - 4));

                // установка курсора
                Console.SetCursorPosition(2, 11 + Math.Max(drinks.Count, aditivs.Count));

                // якщо стаканчик вибрано то можна відображати замовлення
                if (data.IsGlass)
                {
                    // зміна кольору
                    Console.ForegroundColor = ConsoleColor.Red;
                    // Вивід
                    Console.Write("Ваш заказ: ");

                    // зміна кольору
                    Console.ForegroundColor = ConsoleColor.Green;
                    // Вивід
                    {
                        string s = $"G - {Enum.GetValues(typeof(TypeOfGlass)).Cast<TypeOfGlass>().ToList().IndexOf(data.Glass) + 1};";
                        // збереження даних для лог-файлу
                        infoOrder = s;
                        Console.Write(s);
                    }

                    // Перевірка чи вибнано напій
                    if (data.GetDrinks().Count > 0)
                    {
                        // зміна кольору
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        // Вивід
                        {
                            string s = $" D - {data.GetDrinks()[0].ID}x{data.GetDrinks().Count};";
                            // збереження даних для лог-файлу
                            infoOrder += s;
                            Console.Write(s);
                        }
                    }

                    // Перевірка чи вибнано добавки
                    if (data.GetAdditivs().Count > 0)
                    {
                        // зміна кольору
                        Console.ForegroundColor = ConsoleColor.Magenta;

                        // Вивід
                        var s = new StringBuilder($" A -");

                        foreach (var i in aditivs)
                        {
                            if (data.GetAdditivs().Contains(i))
                            {
                                s.Append($" {i.ID}x{data.GetAdditivs().Where(t => t.ID == i.ID).Select(t => t).ToList().Count},");
                            }
                        }

                        // Вивід
                        {
                            string s1 = s.ToString().TrimEnd(',') + "; ";
                            // збереження даних для лог-файлу
                            infoOrder += s1;
                            Console.Write(s1);
                        }
                    }

                    #region For MS Visual Studio 2019 and Windows 10
#if false
                    // Можна зробити умову, що можливість оплати відбувається лише при умові вибору напою
                    if (data.GetDrinks().Count > 0)
                    {
                        // установка курсора
                        Console.SetCursorPosition(Console.WindowWidth / 2 + 2, 11 + Math.Max(drinks.Count, aditivs.Count));

                        // зміна кольору
                        Console.ForegroundColor = ConsoleColor.White;
                        // Вивід
                        Console.Write("P - оптала;");
                    } 
#endif 
                    #endregion
                }
                #endregion

                // установка курсора
                Console.SetCursorPosition(2, 13 + Math.Max(drinks.Count, aditivs.Count));
                // очистка
                Console.Write(new string(' ', Console.WindowWidth - 4));

                // установка курсора
                Console.SetCursorPosition(2, 13 + Math.Max(drinks.Count, aditivs.Count));

                #region Ціна
                if (data.IsGlass)
                {
                    // зміна кольору
                    Console.ForegroundColor = ConsoleColor.Red;
                    // Вивід
                    Console.Write("Стоимость: ");

                    // зміна кольору
                    Console.ForegroundColor = ConsoleColor.Green;
                    // Вивід
                    Console.Write($"{data.Price.ToString("C2", new CultureInfo(region.Name))}; ");

                    #region For MS Visual Studio 2015 and Windows 7
#if true
                    // Можна зробити умову, що можливість оплати відбувається лише при умові вибору напою
                    if (data.GetDrinks().Count > 0)
                    {
                        // установка курсора (початку це було добре відображено на MS visual srudio v. 2019)
                        // але на v. 2015 - затираються дані на консолі
                        //Console.SetCursorPosition(Console.WindowWidth / 2 + 2, 11 + Math.Max(drinks.Count, aditivs.Count));

                        // зміна кольору
                        Console.ForegroundColor = ConsoleColor.White;
                        // Вивід
                        Console.Write("P - оптала;");
                    }
#endif
                    #endregion
                }
                #endregion

                #region Об'єм
                if (data.IsGlass)
                {
                    // установка курсора
                    Console.SetCursorPosition(Console.WindowWidth / 2 + 2, 13 + Math.Max(drinks.Count, aditivs.Count));

                    // зміна кольору
                    Console.ForegroundColor = ConsoleColor.Red;
                    // Вивід
                    Console.Write("Объем: ");

                    // зміна кольору
                    Console.ForegroundColor = ConsoleColor.Green;
                    // Вивід
                    Console.Write($"{data.Volume()} мл");
                }
                #endregion

                // скидання налаштувань
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Взаємодія з клієнтом
        /// </summary>
        private void Entry()
        {
            // Примітка. Блокувати не потрібно там де э очыкування курсора, 
            // так як не буде можливості оновити меню коректно без артефактів

            // Обробка натискань
            while (true)
            {
                // Результат замовлення
                Order();

                // Головне меню вибору
                MenuChange();

                // клавіша вводу
                ConsoleKey? key = null;
                key = Console.ReadKey(true).Key;

                // дія згідно вибору
                switch (key)
                {
                    case ConsoleKey.N:  // очистить
                        if (data.IsGlass)
                        {
                            data.Clear();
                        }
                        break;
                    case ConsoleKey.G:  // стаканчик
                        SizeGlass();
                        break;
                    case ConsoleKey.D:  // напиток
                        if (data.IsGlass)
                        {
                            Pour();
                        }
                        break;
                    case ConsoleKey.A:  // добавка
                        if (data.IsGlass)
                        {
                            Insert();
                        }
                        break;
                    case ConsoleKey.P:  // оплата
                        if (data.GetDrinks().Count > 0)
                        {
                            Pay();
                        }
                        break;
                    case ConsoleKey.Escape:  // виход
                        Environment.Exit(0);
                        break;
                }
            }
        }

        /// <summary>
        /// Вкинути добавки
        /// </summary>
        private void Insert()
        {
            if (aditivs.Count == 0)
            {
                return;
            }

            // Блокуємо доступ іншим потокам
            lock (block)
            {
                do
                {
                    // оновлення заповлення
                    Order();

                    // установка курсора
                    Console.SetCursorPosition(2, 17 + Math.Max(drinks.Count, aditivs.Count));
                    // очистка
                    Console.Write(new string(' ', Console.WindowWidth - 4));
                    // установка курсора
                    Console.SetCursorPosition(2, 17 + Math.Max(drinks.Count, aditivs.Count));

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
                    if (!next || !(0 < position && position <= aditivs.Count))
                    {
                        continue;
                    }

                    // аналіз вибраної добавки, якщо така добавка вже є, 
                    // то закодимо в меню управління нею, а якщо нема то просто додаємо
                    if (!data.GetAdditivs().Contains(aditivs[position - 1]))
                    {
                        // додавання добавки
                        data.AddAdditiv(aditivs[position - 1]);
                    }

                    // якщо така добавка є то заходимо в меню щоб керувати нею
                    if (data.GetAdditivs().Contains(aditivs[position - 1]))
                    {
                        do
                        {
                            // оновлення заповлення
                            Order();

                            // установка курсора
                            Console.SetCursorPosition(2, 17 + Math.Max(drinks.Count, aditivs.Count));
                            // очистка
                            Console.Write(new string(' ', Console.WindowWidth - 4));
                            // установка курсора
                            Console.SetCursorPosition(2, 17 + Math.Max(drinks.Count, aditivs.Count));

                            #region Керування рівнем добавки
                            Print("Выберите: ", ConsoleColor.White);
                            Print("N - очистить, ", ConsoleColor.Yellow);
                            Print("+ добавить, ", ConsoleColor.Green);
                            Print("- удалить, ", ConsoleColor.Cyan);
                            Print("Q - назад. ", ConsoleColor.Red);

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
                                data.AddAdditiv(aditivs[position - 1]);
                            }
                            else if (key0 == "-")
                            {
                                data.RemoveAdditiv(aditivs[position - 1]);
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
            if (drinks.Count == 0)
            {
                return;
            }

            // Блокуємо доступ іншим потокам
            lock (block)
            {
                do
                {
                    // оновлення заповлення
                    Order();

                    // установка курсора
                    Console.SetCursorPosition(1, 17 + Math.Max(drinks.Count, aditivs.Count));
                    // очистка
                    Console.Write(new string(' ', Console.WindowWidth - 2));
                    // установка курсора
                    Console.SetCursorPosition(2, 17 + Math.Max(drinks.Count, aditivs.Count));

                    // якщо нопою нема - вибираємо і додаємо, а якщо є, то або видаляємо
                    // або додаємо до нього такий же
                    if (data.GetDrinks().Count == 0)
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
                        if (next && 0 < position && position <= drinks.Count)
                        {
                            data.AddDrink(drinks[position - 1]);
                        }
                        #endregion
                    }

                    // керування розміром напою
                    if (data.GetDrinks().Count > 0)
                    {
                        do
                        {
                            // оновлення заповлення
                            Order();

                            // установка курсора
                            Console.SetCursorPosition(1, 17 + Math.Max(drinks.Count, aditivs.Count));
                            // очистка
                            Console.Write(new string(' ', Console.WindowWidth - 2));
                            // установка курсора
                            Console.SetCursorPosition(2, 17 + Math.Max(drinks.Count, aditivs.Count));

                            #region Керування рівнем напою
                            Print("Выберите: ", ConsoleColor.White);
                            Print("N - очистить, ", ConsoleColor.Yellow);
                            Print("+ добавить, ", ConsoleColor.Green);
                            Print("- удалить, ", ConsoleColor.Cyan);
                            Print("Q - назад. ", ConsoleColor.Red);

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
                                    data.AddDrink(data.GetDrinks()[0]);
                                    break;
                                case "-":  // убрати
                                    data.RemoveDrink();
                                    break;
                                case "q":  // виход
                                    return;
                            }

                            // якщо ми все видалили то необхідно піти в меню вище
                            if (data.GetDrinks().Count == 0)
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
            lock (block)
            {
                // перевірка введеного значення
                do
                {
                    // установка курсора
                    Console.SetCursorPosition(1, 17 + Math.Max(drinks.Count, aditivs.Count));
                    // очистка
                    Console.Write(new string(' ', Console.WindowWidth - 2));
                    // установка курсора
                    Console.SetCursorPosition(2, 17 + Math.Max(drinks.Count, aditivs.Count));

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
                        data.Glass = value[position - 1];
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
            lock (block)
            {
                // установка курсора
                Console.SetCursorPosition(1, 17 + Math.Max(drinks.Count, aditivs.Count));

                // очистка
                Console.Write(new string(' ', Console.WindowWidth - 2));

                // установка курсора
                Console.SetCursorPosition(2, 17 + Math.Max(drinks.Count, aditivs.Count));

                // Вивід
                Print("Спасибо за оплату. Нажмите клавишу для продолжения...", ConsoleColor.Green);
            }

            // очікування натискання клавіші
            Console.Read();
            // Примітка. При вложених методах в яких на різних рівнях
            // використовується Console.ReadKey(); вилітає на самий вищий рівень
            // тому у рівнях нижче не варно використовувати "ReadKey"

            // Збереження в лог-файд
            string info = new StringBuilder()
                .Append($"Price: {data.Price.ToString("C2", new CultureInfo(region.Name))}, ")
                .Append($"Products: {infoOrder}")
                .ToString();

            new Thread(() => SaveLog.SaveOrder(info)).Start();

            // чистка даних
            data.Clear();
        }

        /// <summary>
        /// Головне меню вибору
        /// </summary>
        private void MenuChange()
        {
            // Блокуємо доступ іншим потокам
            lock (block)
            {
                // установка курсора
                Console.SetCursorPosition(1, 17 + Math.Max(drinks.Count, aditivs.Count));

                // очистка
                Console.Write(new string(' ', Console.WindowWidth - 2));

                // установка курсора
                Console.SetCursorPosition(2, 17 + Math.Max(drinks.Count, aditivs.Count));

                #region Menu of change
                Print("Меню: ", ConsoleColor.White);
                Print("N - очистить, ", ConsoleColor.Yellow);
                Print("G - стаканчик, ", ConsoleColor.Green);
                Print("D - напиток, ", ConsoleColor.Cyan);
                Print("A - добавка, ", ConsoleColor.Magenta);
                Print("Esc - выход. ", ConsoleColor.Red);

                // скидання налаштувань
                Console.ResetColor();
                #endregion
            }
        }

        /// <summary>
        /// Швидке виведеня в консоль деяких частин меню
        /// </summary>
        /// <param name="s">рядок</param>
        /// <param name="color">колір</param>
        private void Print(string s, ConsoleColor color)
        {
            // зміна кольору
            Console.ForegroundColor = color;
            // Вивід
            Console.Write(s);
        }

        /// <summary>
        /// Перевірка зміни розмірів консолі
        /// </summary>
        private void CheckerChange()
        {
            // висота і ширина вікна консолі
            int hight = Console.WindowHeight,
                weigth = Console.WindowWidth;

            while (true)
            {
                // якщо ширина і висота консолі змінилася
                // то оновлюємо внутрішнє меню
                if (hight != Console.WindowHeight ||
                    weigth != Console.WindowWidth)
                {
                    // оновлюємо дані
                    hight = Console.WindowHeight;
                    weigth = Console.WindowWidth;

                    // Запускаємо подію оновлення меню
                    ChangeSize.Invoke();

                }

                // якщо оновилася БД
                if (LoadDataBase.IsUpdateDB)
                {
                    // оновлюємо БД
                    ChangeDB.Invoke();
                    // оновлюємо меню
                    ChangeSize.Invoke();
                    // ставимо, що БД "застаріла"
                    LoadDataBase.IsUpdateDB = false;
                }
            }
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
                ExstractingData(LoadDataBase.Products.Tables["Drinks"], drinks);
                ExstractingData(LoadDataBase.Products.Tables["Additivs"], aditivs);
            }

            // Збереження в лог-файд
            string info = new StringBuilder()
                .Append($"Glasses - {Enum.GetValues(typeof(TypeOfGlass)).Length}, ")
                .Append($"Drinks - {drinks.Count}, ")
                .Append($"Additivs - {aditivs.Count};")
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
