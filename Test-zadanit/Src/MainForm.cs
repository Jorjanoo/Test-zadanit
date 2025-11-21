
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
            this.dgvEmployees.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // 
            // dgvByMonth
            // 
            this.dgvByMonth.Top = 360;
            this.dgvByMonth.Left = 10;
            this.dgvByMonth.Width = 760;
            this.dgvByMonth.Height = 150;
            this.dgvByMonth.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // 
            // lblInfo
            // 
            this.lblInfo.Top = 10;
            this.lblInfo.Left = 190;
            this.lblInfo.Width = 600;
            this.lblInfo.Text = "Используются файлы: Data/Data1.xml, XSLT/transform.xslt";

            // 
            // MainForm
            // 
            this.Text = "XSLT Salary Processor";
            this.ClientSize = new System.Drawing.Size(800, 530);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.dgvEmployees);
            this.Controls.Add(this.dgvByMonth);
            this.Controls.Add(this.lblInfo);
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
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RunXslt()
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(xsltFile);
            xslt.Transform(dataFile, employeesFile);
        }

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

        private void AddTotalPayToData1()
        {
            XDocument doc = XDocument.Load(dataFile);

            double total = doc.Root.Elements("item")
                .Select(x => Convert.ToDouble(x.Attribute("amount").Value.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture))
                .Sum();

            doc.Root.SetAttributeValue("totalAmount", total.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
            doc.Save(dataFile);
        }

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
    }
}
