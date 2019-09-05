using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using App0.Product;

namespace App0.Calculate
{
    /// <summary>
    /// Заказ
    /// </summary>
    struct SOrder
    {
        /// <summary>
        /// Чи вибраний стаканчик
        /// </summary>
        private bool isGlass;

        /// <summary>
        /// ID - стаканчика, 0 - не вибрано
        /// </summary>
        public TypeOfGlass Glass { get; set; }
        /// <summary>
        /// Чи вибраний стаканчик
        /// </summary>
        public bool IsGlass
        {
            get { return isGlass; }
            set
            {
                if (value)
                {
                    // як тільки задали стаканчик установлюємо параметри
                    Capacity = (int)Glass;
                    Free = (int)Glass;
                    isGlass = true;
                }
                else
                {
                    // коли не вибрано стаканчик
                    Capacity = 0;
                    Free = 0;
                    isGlass = value;
                }
            }
        }
        /// <summary>
        /// Ємність вибраного стаканчика
        /// </summary>
        public double Capacity { get; private set; }
        /// <summary>
        /// Вільне місце в стаканчику
        /// </summary>
        public double Free
        {
            get
            {
                // сума всії продуктів
                double sum = default(double);
                // Примітка. Окрім рідини в нас ще є тверді продукти
                // які вимірюються масою, для точного значення як змінюється 
                // об'єм рідини при вкидані деяких твердих продуктів необхідно проводити
                // дослідження, але як для прикладу приймемо, що 10 гр 
                // сума напою
                //sum += Drinks.Select(t => t.Size);
            }
        }
        /// <summary>
        /// Ціна замовлення
        /// </summary>
        public double Price { get; private set; }

        /// <summary>
        /// Список нопоїв, null - не вибрано
        /// </summary>
        public List<IDrink> Drinks { get; set; }
        /// <summary>
        /// Список добавок, null - не вибрано
        /// </summary>
        public List<IAdditiv> Additivs { get; set; }

        /// <summary>
        /// Очищення замовлення
        /// </summary>
        public void Clear()
        {
            this.Drinks = null;
            this.Additivs = null;
            this.IsGlass = false;
        }

        /// <summary>
        /// Об'єм скаканчика (вільно)/(загальний об'єм)
        /// </summary>
        /// <returns></returns>
        public string Volume()
            => IsGlass ? $"{Free}/{Capacity}" : string.Empty;
    }
}
    