using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Product
{
    /// <summary>
    /// Тип оцінки продукту
    /// </summary>
    public enum TypeValue
    {
        /// <summary>
        /// Вага, г (грам) // true - при конвертації
        /// </summary>
        Weight,
        /// <summary>
        /// Об'єм, мл (мілілітр) // false - при конвертації
        /// </summary>
        Volume
    }

    /// <summary>
    /// Продукт (напій, добавки, ...)
    /// </summary>
    interface IProduct
    {
        /// <summary>
        /// ID - продукту
        /// </summary>
        int ID { get; }
        /// <summary>
        /// Назва продукту
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Ціна продукту
        /// </summary>
        double Price { get; }
        /// <summary>
        /// Тип в якому вимірюється продукт
        /// </summary>
        TypeValue TypeOfValue { get; }
        /// <summary>
        /// Величина виміру об'єму чи маси
        /// </summary>
        double Size { get; }

        // Примітки.
        // 1. Це не доволі точна операція роботи з грошима, 
        // тому що б заекономити ресурси, часдо статньо скористатися типом 
        // double на відміну від decimal; якщо б це була більш складна економічна
        // операція то тоді треба використовувати decimal.
        // 2. Так як в ТЗ не було вказано, яка кількість товару використовується,
        // а у ваипадку напоїв то в межах 10. Немає сенсу використовувати без 
        // додаткових умов тип long. А так як ОС налаштована на роботу зазвичай 
        // із типом int і у разі використання byte для розрахунків воно буде підганятися,
        // а так як в уомві не сказано про обмеження пам'яті, то ми вправі використовувати
        // тип int, як локально оптимальний варіант при даних умовах
    }
}
