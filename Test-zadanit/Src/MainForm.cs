using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace XsltTransformApp
{
    public class MainForm : Form
    {
        private Button btnRun;
        private DataGridView dgvEmployees;
        private DataGridView dgvByMonth;
        private Label lblInfo;

        // Поля для добавления item
        private TextBox txtName;
        private TextBox txtSurname;
        private TextBox txtAmount;
        private ComboBox cmbMonth;
        private Button btnAddItem;

        string dataFile = @"Data\Data1.xml";
        string xsltFile = @"XSLT\transform.xslt";
        string employeesFile = @"Output\Employees.xml";

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.btnRun = new Button();
            this.dgvEmployees = new DataGridView();
            this.dgvByMonth = new DataGridView();
            this.lblInfo = new Label();

            this.txtName = new TextBox();
            this.txtSurname = new TextBox();
            this.txtAmount = new TextBox();
            this.cmbMonth = new ComboBox();
            this.btnAddItem = new Button();

            // 
            // btnRun
            // 
            this.btnRun.Text = "Выполнить обработку";
            this.btnRun.Top = 10;
            this.btnRun.Left = 10;
            this.btnRun.Width = 160;
            this.btnRun.Click += new EventHandler(this.btnRun_Click);

            // 
            // dgvEmployees
            // 
            this.dgvEmployees.Top = 50;
            this.dgvEmployees.Left = 10;
            this.dgvEmployees.Width = 760;
            this.dgvEmployees.Height = 300;

            // 
            // dgvByMonth
            // 
            this.dgvByMonth.Top = 360;
            this.dgvByMonth.Left = 10;
            this.dgvByMonth.Width = 760;
            this.dgvByMonth.Height = 150;

            // 
            // lblInfo
            // 
            this.lblInfo.Top = 10;
            this.lblInfo.Left = 190;
            this.lblInfo.Width = 600;
            this.lblInfo.Text = "Используются файлы: Data/Data1.xml, XSLT/transform.xslt";

            // ---- Элементы добавления нового item ----

            txtName.Left = 10;
            txtName.Top = 520;
            txtName.Width = 120;
            txtName.PlaceholderText = "Name";

            txtSurname.Left = 140;
            txtSurname.Top = 520;
            txtSurname.Width = 120;
            txtSurname.PlaceholderText = "Surname";

            txtAmount.Left = 270;
            txtAmount.Top = 520;
            txtAmount.Width = 100;
            txtAmount.PlaceholderText = "Amount";

            cmbMonth.Left = 380;
            cmbMonth.Top = 520;
            cmbMonth.Width = 120;
            cmbMonth.Items.AddRange(new string[] { "january", "february", "march" });
            cmbMonth.SelectedIndex = 0;

            btnAddItem.Left = 510;
            btnAddItem.Top = 520;
            btnAddItem.Width = 160;
            btnAddItem.Text = "Добавить item";
            btnAddItem.Click += BtnAddItem_Click;

            // 
            // MainForm
            // 
            this.Text = "XSLT Salary Processor";
            this.ClientSize = new System.Drawing.Size(800, 570);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.dgvEmployees);
            this.Controls.Add(this.dgvByMonth);
            this.Controls.Add(this.lblInfo);

            // добавляем элементы для добавления item
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtSurname);
            this.Controls.Add(this.txtAmount);
            this.Controls.Add(this.cmbMonth);
            this.Controls.Add(this.btnAddItem);
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                RunXslt();
                AddEmployeeSalarySum();
                AddTotalPayToData1();
                LoadEmployeesToGrid();
                LoadMonthsToGrid();

                MessageBox.Show("Готово!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // -----------------------------
        // 1. XSLT преобразование
        // -----------------------------
        private void RunXslt()
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(xsltFile);
            xslt.Transform(dataFile, employeesFile);
        }

        // -----------------------------
        // 2. Добавить сумму зарплат в Employee
        // -----------------------------
        private void AddEmployeeSalarySum()
        {
            XDocument doc = XDocument.Load(employeesFile);

            foreach (var emp in doc.Root.Elements("Employee"))
            {
                double sum = emp.Elements("salary")
                                .Select(x => Convert.ToDouble(x.Attribute("amount").Value.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture))
                                .Sum();

                emp.SetAttributeValue("sumSalary", sum.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
            }

            doc.Save(employeesFile);
        }

        // -----------------------------
        // 3. TotalAmount в Data1.xml
        // -----------------------------
        private void AddTotalPayToData1()
        {
            XDocument doc = XDocument.Load(dataFile);

            double total = doc.Root.Elements("item")
                .Select(x => Convert.ToDouble(x.Attribute("amount").Value.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture))
                .Sum();

            doc.Root.SetAttributeValue("totalAmount", total.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
            doc.Save(dataFile);
        }

        // -----------------------------
        // 4. Загрузка сотрудников
        // -----------------------------
        private void LoadEmployeesToGrid()
        {
            XDocument doc = XDocument.Load(employeesFile);

            var table = new DataTable();
            table.Columns.Add("Name");
            table.Columns.Add("Surname");
            table.Columns.Add("Month");
            table.Columns.Add("Amount");

            foreach (var emp in doc.Root.Elements("Employee"))
            {
                foreach (var sal in emp.Elements("salary"))
                {
                    table.Rows.Add(
                        emp.Attribute("name").Value,
                        emp.Attribute("surname").Value,
                        sal.Attribute("mount").Value,
                        sal.Attribute("amount").Value
                    );
                }
            }

            dgvEmployees.DataSource = table;
        }

        // -----------------------------
        // 5. Загрузка сумм по месяцам
        // -----------------------------
        private void LoadMonthsToGrid()
        {
            XDocument doc = XDocument.Load(employeesFile);

            var query = doc.Root
                .Elements("Employee")
                .Elements("salary")
                .GroupBy(x => x.Attribute("mount").Value)
                .Select(g => new
                {
                    Month = g.Key,
                    Total = g.Sum(x =>
                        Convert.ToDouble(x.Attribute("amount").Value.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture))
                })
                .ToList();

            dgvByMonth.DataSource = query;
        }

        // ----------------------------------------------------
        // 6. ДОБАВЛЕНИЕ НОВОГО ITEM В DATA1.XML
        // ----------------------------------------------------
        private void BtnAddItem_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtName.Text.Trim();
                string surname = txtSurname.Text.Trim();
                string amount = txtAmount.Text.Trim();
                string month = cmbMonth.SelectedItem.ToString();

                if (name == "" || surname == "" || amount == "")
                {
                    MessageBox.Show("Заполните все поля!");
                    return;
                }

                AddNewItem(name, surname, amount, month);

                RunXslt();
                AddEmployeeSalarySum();
                AddTotalPayToData1();
                LoadEmployeesToGrid();
                LoadMonthsToGrid();

                MessageBox.Show("Item добавлен и данные пересчитаны!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddNewItem(string name, string surname, string amount, string month)
        {
            XDocument doc = XDocument.Load(dataFile);

            XElement newItem = new XElement("item",
                new XAttribute("name", name),
                new XAttribute("surname", surname),
                new XAttribute("amount", amount),
                new XAttribute("mount", month)
            );

            doc.Root.Add(newItem);
            doc.Save(dataFile);
        }
    }
}
