using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using App0.Product;
using App0.Calculate;

/// <summary>
/// Базовий додаток
/// </summary>
namespace App0
{
    class Program
    {
        static void Main()
        {
            // Join Unicode
            Console.OutputEncoding = Encoding.Unicode;

            // Заголовок програми
            Console.Title = "Fresh drinks";

            #region Установка культури
            // В умові вказані $ але можливо 
            // треба використовувати регіональну валюту - гривні
            RegionInfo region = RegionInfo.CurrentRegion;
            // встановити культури USA для вткористання доларів?
            if (true)
            {
                region = new RegionInfo("en-US");
            }
            #endregion

            #region Temp hided
            // назва файлу БД з можливим повним шляхом, але без розширення
            string path = string.Empty, // шлях до папки
                file = "Product";       // назва файлу

            // завантаження БД
            LoadDataBase.UpLoadDataBase(path + file);

            // якщо БД завантажена успошно запускаємо обробку запитів користувача
            if (LoadDataBase.Successful)
            {
                // Запуск інформатора який оновлюватиме БД
                Checker checker = new Checker(file);
      
                // запуск для тестування
                new Service(region).Menu();
            }
            #endregion

            // delay
            Console.ReadKey(true);
        }
    }
}
