using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;	
using System.Diagnostics;


namespace SimuladorProcesos
{
    public partial class MainForm : MetroFramework.Forms.MetroForm
    {
        private Process[] process;
        private LinkedList<Proceso> procesos;
        private Random random;
        private RoundRobin roundRobin;
        int quantum = 2;

        public MainForm()
        {
            InitializeComponent();
            procesos = new LinkedList<Proceso>();
            random = new Random();
            process = Process.GetProcesses();
            cargarProcesos();

            //pictureBox1.BackColor = Color.FromArgb(0, 171, 169);
            //pictureBox2.BackColor = Color.FromArgb(238, 17, 17);
            //pictureBox3.BackColor = Color.FromArgb(185, 29, 71);
            //pictureBox4.BackColor = Color.FromArgb(30, 113, 69);
            //pictureBox4.BackColor = Color.FromArgb(96, 60, 186);
            //pictureBox5.BackColor = Color.FromArgb(45, 137, 239);
            //pictureBox6.BackColor = Color.FromArgb(43, 87, 151);
            //pictureBox7.BackColor = Color.FromArgb(255, 196, 13);
            //pictureBox8.BackColor = Color.FromArgb(227, 162, 26);
        }

        private void cargarProcesos()
        {
            int tiempo, Memory;
            for (int i = 0; i < 13; i++)
            {
                tiempo = random.Next(2, 6);
                Memory = random.Next(15, 500);

                Proceso proceso = new Proceso(process[i].Id, process[i].ProcessName, tiempo, Memory);
                procesos.AddLast(proceso);
                agregarProceso(proceso);
            }
        }

        private void agregarProceso(Proceso proceso)
        {
            string id = proceso.Id.ToString();
            string nombre = proceso.Nombre;
            string estado = proceso.Estado;
            string tiempo = proceso.Tiempo.ToString();
            string memoria = proceso.Memoria.ToString();

            string[] row = {id, nombre, estado, tiempo, memoria};
            dataGridViewProcesos.Rows.Add(row);
        }

        private void IniciarRR()
        {
            Proceso[] arrProcesos = procesos.ToArray();
            roundRobin = new RoundRobin(ref dataGridViewProcesos, ref pictureBox1, ref pictureBox2, ref pictureBox3, ref pictureBox4, ref pictureBox5, ref pictureBox6, ref pictureBox7, ref pictureBox8);
            roundRobin.runRoundRobin(ref arrProcesos, quantum);
        }
        
        private void buttonEjecutar_Click(object sender, EventArgs e)
        {
            IniciarRR();
        }

        private void AboutApp()
        {
            lblInfo.Text = "Es aceptable tener múltiples procesos leyendo la base de" +
                "datos al mismo tiempo, pero si un proceso está actualizando(escribiendo en) la base de datos, ningún otro" +
                "podrá tener acceso a ella, ni siquiera los lectores ";
        }
    }
}
