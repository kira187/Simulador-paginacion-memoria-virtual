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
        String[] Colors = { "#E3A21A", "#7e3878", "#1e7145", "#000000", "#603cba", "#00aba9", "#2d89ef", "#2b5797", "#ffc40d", "#da532c", "#ee1111", "#b91d47", "#00a300" };

        public MainForm()
        {
            InitializeComponent();
            procesos = new LinkedList<Proceso>();
            random = new Random();
            process = Process.GetProcesses();
            cargarProcesos();
        }

        private void cargarProcesos()
        {
            int tiempo, Memory;
            for (int i = 0; i < 13; i++)
            {
                tiempo = random.Next(3, 5);
                Memory = random.Next(20, 200);

                Proceso proceso = new Proceso(process[i].Id, process[i].ProcessName, tiempo, Memory, Colors[i]);
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

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
