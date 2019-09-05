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
        public double Free { get; private set; }

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
    }
}
    