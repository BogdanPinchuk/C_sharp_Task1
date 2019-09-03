using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Примітка. Так як типів виміру продукту є два: маса і об'єм,
// то правильно б задатися булєвим типом даних при передачі в БД

namespace App1.Product
{
    /// <summary>
    /// Клас методів розширення
    /// </summary>
    public static class Extention
    {
        /// <summary>
        /// Конвертувати з типу виміру (вага/об'єм) в булєвий тип
        /// </summary>
        /// <param name="type">тип в якому проводять вимір продукту - вага/об'єм</param>
        /// <returns></returns>
        public static bool ConvertToBool(this TypeValue type)
        {
            if (type == TypeValue.Weight)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Конвертувати з бульового типу в тип виміру (вага/об'єм)
        /// </summary>
        /// <param name="type">закодований тип виміру продукту - ваги/об'єму</param>
        /// <returns></returns>
        public static TypeValue ConvertToTypeValue(this bool type)
        {
            if (type)
            {
                return TypeValue.Weight;
            }
            else
            {
                return TypeValue.Volume;
            }
        }
    }
}
