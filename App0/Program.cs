using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using App0.Product;

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

            // назва файлу БД з можливим повним шляхом, але без розширення
            string pathFile = "Product";

            // завантаження БД
            LoadDataBase loadDB = new LoadDataBase(pathFile);
            

            // delay
            Console.ReadKey(true);
        }
    }
}
