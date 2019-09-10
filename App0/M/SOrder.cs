using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using App0.M.Product;

namespace App0.M
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
        /// Список нопоїв, null - не вибрано
        /// </summary>
        private List<IDrink> drinks;
        /// <summary>
        /// Список добавок, null - не вибрано
        /// </summary>
        private List<IAdditiv> additivs;
        /// <summary>
        /// Тип стаканчика
        /// </summary>
        private TypeOfGlass glass;

        /// <summary>
        /// Тип стаканчика
        /// </summary>
        public TypeOfGlass Glass
        {
            get { return glass; }
            set
            {
                // також врахуємо умову, що користувач міг зробити замовленння,
                // а тоді вирішив змінити розмір стаканчика і очищати зразу не дуже
                // сподобається клієнтові. Тому краще в певних моментах зберігати замовлення
                // якщо зайняте місце менше від вибраного нового стаканчика, то дозволяємо 
                // зміну, інакше чистимо замовлення, щоб не було переповнення стаканчика.

                // якщо ж відбувається зміна стаканчика і переповнення, то чистимо замовлення
                if (Capacity - Free > (double)value)
                {
                    Clear();
                }

                glass = value;
                isGlass = true;
            }
        }
        /// <summary>
        /// Чи вибраний стаканчик
        /// </summary>
        public bool IsGlass
        {
            get { return isGlass; }
            set
            {
                // зовні стуруктури стаканчик можна лише забрати, але не поставити
                // поставити стаканчик можна лише присвоївши зразу його тип,
                // тобто, змінити на true можна лише зсередини структури
                if (!value)
                {
                    // очищення всього, а там є реалізація присвоєння false
                    Clear();
                }
            }
        }
        /// <summary>
        /// Ємність вибраного стаканчика
        /// </summary>
        public double Capacity
        {
            get
            {
                if (!IsGlass)
                {
                    return 0;
                }
                else
                {
                    return (double)Glass;
                }
            }
        }
        /// <summary>
        /// Вільне місце в стаканчику
        /// </summary>
        public double Free
        {
            get
            {
                if (!IsGlass)
                {
                    return 0;
                }

                // сума всіх продуктів
                double sum = default(double);
                //Примітка.Окрім рідини в нас ще є тверді продукти
                // які вимірюються масою, для точного значення як змінюється
                // об'єм рідини при вкидані деяких твердих продуктів необхідно проводити
                // дослідження, але як для прикладу приймемо, що 10 гр при попаданні
                // в рідину перетворюється в 5 мл, тобто 1 г = 0,5 мл

                #region Сума напою
                foreach (var i in GetDrinks())
                {
                    if (i.TypeOfValue == TypeValue.Volume)
                    {
                        sum += i.Size;
                    }
                    else
                    {
                        sum += 0.5 * i.Size;
                    }
                }
                #endregion

                #region Сума добавок
                foreach (var i in GetAdditivs())
                {
                    if (i.TypeOfValue == TypeValue.Volume)
                    {
                        sum += i.Size;
                    }
                    else
                    {
                        sum += 0.5 * i.Size;
                    }
                }
                #endregion

                return Capacity - sum;
            }
        }
        /// <summary>
        /// Ціна замовлення
        /// </summary>
        public double Price
        {
            get
            {
                // сума всіх продуктів
                double sum = default(double);

                // Сума напою
                sum += GetDrinks().Select(t => t.Price).Sum();
                // Сума добавок
                sum += GetAdditivs().Select(t => t.Price).Sum();

                return sum;
            }
        }

        /// <summary>
        /// Додати напій
        /// </summary>
        /// <param name="drink">Напій</param>
        public bool AddDrink(IDrink drink)
        {
            // якщо стаканчика немає, додавати нічого не можна
            if (!IsGlass)
            {
                return false;
            }

            // необхідно не допустити переповнення стаканчика
            // розрахунок об'єму продукту який пробують додати
            double size = (drink.TypeOfValue == TypeValue.Volume) ? drink.Size : drink.Size * 0.5;

            // якщо перший раз то ініціалізуємо
            drinks = drinks ?? new List<IDrink>();

            // якщо є вільне місце то додаємо продукт
            if (drinks.Count == 0)
            {
                if (Free >= size)
                {
                    drinks.Add(drink);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // необхідно недопустити змішування напоїв
            if (drinks[0].Name == drink.Name && Free >= size)
            {
                drinks.Add(drink);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Отримати колекцію напоїв
        /// </summary>
        /// <returns></returns>
        public List<IDrink> GetDrinks()
            => drinks = drinks ?? new List<IDrink>();

        /// <summary>
        /// Додати добавку
        /// </summary>
        /// <param name="additiv">Добавка</param>
        public bool AddAdditiv(IAdditiv additiv)
        {
            // якщо стаканчика немає, додавати нічого не можна
            if (!IsGlass)
            {
                return false;
            }

            // необхідно не допустити переповнення стаканчика
            // розрахунок об'єму продукту який пробують додати
            double size = (additiv.TypeOfValue == TypeValue.Volume) ? additiv.Size : additiv.Size * 0.5;

            // якщо перший раз то ініціалізуємо
            additivs = additivs ?? new List<IAdditiv>();

            // якщо є вільне місце то додаємо продукт
            if (additivs.Count == 0)
            {
                if (Free >= size)
                {
                    additivs.Add(additiv);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // необхідно недопустити переповнення в наступних випадках
            if (Free >= size)
            {
                additivs.Add(additiv);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Отримати колекцію добавок
        /// </summary>
        /// <returns></returns>
        public List<IAdditiv> GetAdditivs()
            => additivs = additivs ?? new List<IAdditiv>();

        /// <summary>
        /// Видалити 1 порцію напою
        /// </summary>
        public void RemoveDrink()
        {
            drinks = drinks ?? new List<IDrink>();
            
            if (drinks.Count > 0)
            {
                drinks.RemoveAt(0);
            }
        }

        /// <summary>
        /// Видалити весь напій
        /// </summary>
        public void RemoveDrinks()
            => drinks?.Clear();

        /// <summary>
        /// Видалити 1 порцію добавки
        /// </summary>
        public void RemoveAdditiv(IAdditiv product)
            => additivs?.Remove(product);

        /// <summary>
        /// Видалити всі добавки
        /// </summary>
        public void RemoveAdditivs()
            => additivs?.Clear();

        /// <summary>
        /// Очищення замовлення
        /// </summary>
        public void Clear()
        {
            this.drinks?.Clear();
            this.additivs?.Clear();
            this.isGlass = false;
        }

        /// <summary>
        /// Об'єм скаканчика (вільно)/(загальний об'єм)
        /// </summary>
        /// <returns></returns>
        public string Volume()
            => IsGlass ? $"{Free:F2} / {Capacity:F2}" : string.Empty;
    }
}
