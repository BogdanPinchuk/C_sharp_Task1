// Програма «App1» в на пів ручному режимі створює базу даних MS Access *.mdb. 
// Створений файл БД необхідний для коректної роботи «App0». Основні методи по 
// створенню файла, таблиць, внесення/очищення даних реалізовано. Щоб пояснити 
// роботу цих методів в регіоні «Testing DataBase» наведено приклади, як це 
// реалізується. І в низу згідно умови вносяться дані вказані в ТЗ.
// Також, додатково від себе, було розширено таблиці і додано об’єм/масу 
// та їх значення, що в свою чергу може моделювати/симулювати роботу Кофе-автомата.

using System;
using System.Text;

using App1.Product;

/// <summary>
/// Додаток для ручного створення БД
/// </summary>
namespace App1
{
    class Program
    {
        static void Main()
        {
            // Join Unicode
            Console.OutputEncoding = Encoding.Unicode;

            // Створення файла БД
            string pathFile = CreateDB.CreateFile("Product");
            // Примітка. Якщо необхідно, можна задати повний шлях
            // до файла БД там де він буде знаходитися, при цьому вказавши
            // назву файла БД (наприклад "Product"), 
            // формат файлу підставляється автоматично і його вказувати не потрібно

            // Створення таблиць згідно умові
            CreateDB.CreateTable(pathFile);

            #region Testing DataBase
#if false
            // занесення даних
            for (int i = 0; i < 10; i++)
            {
                CreateDB.AddDrink(i, "Product " + i, i * 5.5, 75, TypeValue.Volume, pathFile);
                CreateDB.AddAdditiv(i, "Product " + i + 10, i * 5.5, 10, TypeValue.Weight, pathFile);
            }

            // зміна даних
            CreateDB.ChangeDrink(3, "New Product " + 3, 3 * 5.5 * 1.2, 80, TypeValue.Weight, pathFile);
            CreateDB.ChangeAdditiv(3, "New Product " + 3, 3 * 5.5 * 1.2, 15, TypeValue.Weight, pathFile);

            // видалення даних
            CreateDB.DeleteDrink(3, pathFile);
            CreateDB.DeleteAdditiv(3, pathFile);

            // очищення таблиць
            CreateDB.DeleteAllDrink(pathFile);
            CreateDB.DeleteAllAdditiv(pathFile);
#endif
            #endregion

            #region Create DataBase - Створення БД
            // доадаємо в таблицю БД напоїв
            {
                int i = 0;
                CreateDB.AddDrink(++i, "Кофе", 15, 75, TypeValue.Volume, pathFile);
                CreateDB.AddDrink(++i, "Чай", 10, 75, TypeValue.Volume, pathFile);
            }
            // додаємо в таблицю БД добавок
            {
                int i = 0;
                CreateDB.AddAdditiv(++i, "Молоко", 2, 5, TypeValue.Volume, pathFile);
                CreateDB.AddAdditiv(++i, "Сахар", 1, 10, TypeValue.Weight, pathFile);
                CreateDB.AddAdditiv(++i, "Корица", 3, 10, TypeValue.Weight, pathFile);
                CreateDB.AddAdditiv(++i, "Лимон", 1, 5, TypeValue.Volume, pathFile);
                // в умові не сказано, кидається туди кусок липому певної ваги чи просто додається лимонний сік
            } 
            #endregion

            // фінальне сповіщення
            Console.WriteLine("\n\tНе забудьте спопіювати дану БД у відповідну папку, або вказати на неї правильний шлях.");

            // delay
            Console.ReadKey(true);
        }

    }
}
