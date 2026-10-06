using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FramePlugin
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void TextBox_OnlyDigitKeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем ввод только цифр и управляющих символов (например, Backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Блокируем нажатие
            }
        }

        // Проверка ширины рамки (L1)
        private void textBox_L1_Leave(object sender, EventArgs e)
        {
            if (double.TryParse(textBox_L1.Text, out double l1))
            {
                if (l1 < 120 || l1 > 600)
                {
                    SetError(textBox_L1, "Ширина рамки L1 должна быть от 120 до 600 мм.");
                }
                else
                {
                    ClearError(textBox_L1);
                    // Перепроверяем W, так как его допустимый максимум зависит от L1
                    textBox_W_Leave(sender, e);
                }
            }
            else SetError(textBox_L1, "Поле L1 не может быть пустым.");
        }

        private void SetError(TextBox textBox, string errorMessage)
        {
            textBox.BackColor = Color.LightCoral;

            // Записываем текст только в том случае, если окно ошибок пустое.
            // Это гарантирует, что первая найденная ошибка не затрется последующими.
            if (label_Errors.Text == "" || label_Errors.BackColor == Color.LightGreen)
            {
                label_Errors.Text = errorMessage;
                label_Errors.BackColor = Color.LightCoral;
            }
        }

        private void ClearError(TextBox textBox)
        {
            textBox.BackColor = SystemColors.Window; // Возвращаем исправленному полю белый фон

            // ВАЖНО: Мы больше не стираем label_Errors.Text здесь! 
            // Иначе правильное поле сотрет ошибку соседнего неправильного поля.
        }

        // Проверка высоты рамки (H1)
        private void textBox_H1_Leave(object sender, EventArgs e)
        {
            if (double.TryParse(textBox_H1.Text, out double h1))
            {
                if (h1 < 120 || h1 > 800)
                {
                    SetError(textBox_H1, "Высота рамки H1 должна быть от 120 до 800 мм.");
                }
                else
                {
                    ClearError(textBox_H1);
                    // Перепроверяем W, так как его допустимый максимум зависит от H1
                    textBox_W_Leave(sender, e);
                }
            }
            else SetError(textBox_H1, "Поле H1 не может быть пустым.");
        }

        private void textBox_W_Leave(object sender, EventArgs e)
        {
            // Прерываем проверку W, если базовые габариты введены с ошибкой
            if (textBox_L1.BackColor == Color.LightCoral || textBox_H1.BackColor == Color.LightCoral)
            {
                return;
            }

            if (double.TryParse(textBox_W.Text, out double w) &&
                double.TryParse(textBox_L1.Text, out double l1) &&
                double.TryParse(textBox_H1.Text, out double h1))
            {
                double absoluteMaxW = Math.Min((l1 - 90) / 2.0, (h1 - 90) / 2.0);

                if (absoluteMaxW < 15)
                {
                    SetError(textBox_W, "Габариты рамки слишком малы для багета.");
                }
                else if (w < 15 || w > absoluteMaxW)
                {
                    SetError(textBox_W, $"Ширина багета W при текущих габаритах должна быть от 15 до {absoluteMaxW} мм.");
                }
                else
                {
                    ClearError(textBox_W);
                }
            }
        }

        // Проверка толщины рамки (b) - независимый параметр
        private void textBox_b_Leave(object sender, EventArgs e)
        {
            if (double.TryParse(textBox_b.Text, out double b))
            {
                if (b < 10 || b > 40)
                    SetError(textBox_b, "Толщина рамки b должна быть от 10 до 40 мм.");
                else
                    ClearError(textBox_b);
            }
            else SetError(textBox_b, "Поле b не может быть пустым.");
        }

        // Проверка угла наклона (a) - независимый параметр
        private void textBox_a_Leave(object sender, EventArgs e)
        {
            if (double.TryParse(textBox_a.Text, out double a))
            {
                if (a < 10 || a > 40)
                    SetError(textBox_a, "Угол наклона a должен быть от 10 до 40 град.");
                else
                    ClearError(textBox_a);
            }
            else SetError(textBox_a, "Поле угла a не может быть пустым.");
        }

        private void button_Build_Click(object sender, EventArgs e)
        {
            // Очищаем окно предупреждений перед новым циклом проверок
            label_Errors.Text = "";
            label_Errors.BackColor = SystemColors.Control;

            // Запускаем проверки всех полей
            textBox_L1_Leave(sender, e);
            textBox_H1_Leave(sender, e);
            textBox_W_Leave(sender, e);
            textBox_b_Leave(sender, e);
            textBox_a_Leave(sender, e);

            // Если фон хотя бы одного поля красный — останавливаем процесс
            if (textBox_L1.BackColor == Color.LightCoral ||
                textBox_H1.BackColor == Color.LightCoral ||
                textBox_W.BackColor == Color.LightCoral ||
                textBox_b.BackColor == Color.LightCoral ||
                textBox_a.BackColor == Color.LightCoral)
            {
                return;
            }

            // Успешная валидация
            label_Errors.Text = "Валидация пройдена. Начинаем построение...";
            label_Errors.BackColor = Color.LightGreen;
        }
    }
}
