using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bluebit.MatrixLibrary;


namespace خوارزمية_بلايفير
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //string alphabets = "ABCDEFGHIKLMNOPQRSTUVWXYZ";
        string alphabets = "أبتثجحخدذرزسشصضطظعغفقكلمنهوي";


        private void button3_Click(object sender, EventArgs e)   ///مسح مربعات النصوص 
        {
            textBoxKey.Clear();
            textBoxPlanText.Clear();
            textBoxCipherText.Clear();
            textBoxDecipherText.Clear();
        }

        private void textBoxKey_TextChanged(object sender, EventArgs e)  ///بناء جدول التشفير
        {
            if (textBoxKey.Text == "")
            {
                dataGridViewKey.DataSource = null;
            }
            else
            {
                string key = textBoxKey.Text.ToUpper();
                //key = key.Replace('J', 'I');                     // استبدال كل جي بـ آي

                for (int i = 0; i < key.Count(); i++)           // إزالة الأحرف المكررة من جدول التشفير
                {
                    for (int j = i + 1; j < key.Count(); j++)
                    {
                        if (key[i] == key[j])
                        {
                            key = key.Remove(j, 1);
                            j--;
                        }
                    }
                }

                for (int i = 0; i < 28 /*25*/; i++)                    // تعبئة حروف جدول التشفير بسترينج
                {
                    if (key.Contains(alphabets[i]))             //A=>Z
                        continue;
                    else
                        key = key + alphabets[i];               //TashferBCD
                }

                List<string[]> list = new List<string[]>();
                for (int i = 0; i < 28/*25*/; i += 4)
                {
                    list.Add(new string[] { Convert.ToString(key[i]), Convert.ToString(key[i + 1]), Convert.ToString(key[i + 2]), Convert.ToString(key[i + 3]), /*Convert.ToString(key[i + 4])*/ });
                }

                DataTable table = ConvertListToDataTable(list);
                dataGridViewKey.DataSource = table;
                //dataGridViewKey.DataSource = from table12 in list
                //                             select table12;
            }
        }

        static DataTable ConvertListToDataTable(List<string[]> list)
        {
            // New table.
            DataTable table = new DataTable();

            // Get max columns.
            int columns = 0;
            foreach (var array in list)
            {
                if (array.Length > columns)
                {
                    columns = array.Length;
                }
            }

            // Add columns.
            for (int i = 0; i < columns; i++)
            {
                table.Columns.Add();
            }

            // Add rows.
            foreach (var array in list)
            {
                table.Rows.Add(array);
            }

            return table;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////

        public static int Mod(int a, int n)
        {
            return a - (int)Math.Floor((double)a / n) * n;
        }

        private void button1_Click(object sender, EventArgs e)  /// تشفير النص الأصلي
        {
            if (textBoxKey.Text == "" || textBoxPlanText.Text == "")
                MessageBox.Show("بعض الحقول المطلوبة فارغة");
            else
            {
                textBoxCipherText.Clear();
                string PlanText = textBoxPlanText.Text.ToUpper() + " ";
                //PlanText = PlanText.Replace("J", "I");
                for (int i = 0; i < PlanText.Length; i += 2)
                {
                    if (alphabets.Contains(PlanText[i]))
                    {
                        if (!alphabets.Contains(PlanText[i + 1]))
                        {
                            if (PlanText[i] == 'س')
                                PlanText = PlanText.Insert(i + 1, "ص");
                            else
                                PlanText = PlanText.Insert(i + 1, "س");
                        }

                        if (PlanText[i] == PlanText[i + 1])
                        {
                            if (PlanText[i] == 'س')
                            {
                                PlanText = PlanText.Insert(i + 1, "ص");
                            }
                            else
                                PlanText = PlanText.Insert(i + 1, "س");
                        }
                        
                        
                        int RowChar1 = 0;
                        int ColumnChar1 = 0;
                        int RowChar2 = 0;
                        int ColumnChar2 = 0;

                        for (int R = 0; R < 7 /*5*/; R++)
                        {
                            for (int C = 0; C < 4 /*5*/; C++)
                            {

                                if (dataGridViewKey.Rows[R].Cells[C].Value.ToString() == Convert.ToString(PlanText[i]))
                                {
                                    RowChar1 = R;
                                    ColumnChar1 = C;
                                }
                                if (dataGridViewKey.Rows[R].Cells[C].Value.ToString() == Convert.ToString(PlanText[i + 1]))
                                {
                                    RowChar2 = R;
                                    ColumnChar2 = C;
                                }

                            }
                        }
                        if (RowChar1 == RowChar2)
                        {
                            textBoxCipherText.Text += dataGridViewKey.Rows[RowChar1].Cells[Mod(ColumnChar1 + 1, 4/*5*/)].Value.ToString();
                            textBoxCipherText.Text += dataGridViewKey.Rows[RowChar1].Cells[Mod(ColumnChar2 + 1, 4/*5*/)].Value.ToString();
                        }
                        else
                            if (ColumnChar1 == ColumnChar2)
                            {
                                textBoxCipherText.Text += dataGridViewKey.Rows[Mod(RowChar1 + 1, 7/*5*/)].Cells[ColumnChar1].Value.ToString();
                                textBoxCipherText.Text += dataGridViewKey.Rows[Mod(RowChar2 + 1, 7/*5*/)].Cells[ColumnChar1].Value.ToString();
                            }
                            else
                            {
                                textBoxCipherText.Text += dataGridViewKey.Rows[RowChar1].Cells[ColumnChar2].Value.ToString();
                                textBoxCipherText.Text += dataGridViewKey.Rows[RowChar2].Cells[ColumnChar1].Value.ToString();
                            }
                    }
                    else
                    {
                        textBoxCipherText.Text += PlanText[i];
                        i--;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)      /// عملية فك التشفير
        {
            if (textBoxKey.Text == "" || textBoxCipherText.Text == "")
                MessageBox.Show("بعض الحقول المطلوبة فارغة");
            else
            {
                textBoxDecipherText.Clear();
                string CipherText = textBoxCipherText.Text.ToUpper() + " ";
                for (int i = 0; i < CipherText.Length; i += 2)
                {
                    if (alphabets.Contains(CipherText[i]))
                    {
                        int RowChar1 = 0;
                        int ColumnChar1 = 0;
                        int RowChar2 = 0;
                        int ColumnChar2 = 0;

                        for (int R = 0; R < 7; R++)
                        {
                            for (int C = 0; C < 4; C++)
                            {

                                if (dataGridViewKey.Rows[R].Cells[C].Value.ToString() == Convert.ToString(CipherText[i]))
                                {
                                    RowChar1 = R;
                                    ColumnChar1 = C;
                                }
                                if (dataGridViewKey.Rows[R].Cells[C].Value.ToString() == Convert.ToString(CipherText[i + 1]))
                                {
                                    RowChar2 = R;
                                    ColumnChar2 = C;
                                }

                            }
                        }
                        if (RowChar1 == RowChar2)
                        {
                            textBoxDecipherText.Text += dataGridViewKey.Rows[RowChar1].Cells[Mod(ColumnChar1 - 1, 4/*5*/)].Value.ToString();
                            textBoxDecipherText.Text += dataGridViewKey.Rows[RowChar1].Cells[Mod(ColumnChar2 - 1, 4/*5*/)].Value.ToString();
                        }
                        else
                            if (ColumnChar1 == ColumnChar2)
                            {
                                textBoxDecipherText.Text += dataGridViewKey.Rows[Mod(RowChar1 - 1, 7/*5*/)].Cells[ColumnChar1].Value.ToString();
                                textBoxDecipherText.Text += dataGridViewKey.Rows[Mod(RowChar2 - 1, 7/*5*/)].Cells[ColumnChar1].Value.ToString();
                            }
                            else
                            {
                                textBoxDecipherText.Text += dataGridViewKey.Rows[RowChar1].Cells[ColumnChar2].Value.ToString();
                                textBoxDecipherText.Text += dataGridViewKey.Rows[RowChar2].Cells[ColumnChar1].Value.ToString();
                            }
                    }
                    else
                    {
                        textBoxDecipherText.Text += CipherText[i];
                        i--;
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Bluebit.MatrixLibrary.Matrix a = new Bluebit.MatrixLibrary.Matrix(3, 3);
            Bluebit.MatrixLibrary.Matrix A = new Bluebit.MatrixLibrary.Matrix(new double[,] { { 3, 0 }, { 2, 1 } });
            //a = A.Inverse();
            MessageBox.Show(A.Determinant().ToString());
            //textBoxKey.Text = a[0, 0].ToString() + "  " + a[0, 1].ToString() + "  " + a[1, 0].ToString()+"  " + a[0, 1].ToString();
            dataGridViewKey.DataSource = a.ToArray(); 


        }
    }
}
