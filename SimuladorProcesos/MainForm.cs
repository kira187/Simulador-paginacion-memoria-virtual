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
        int quantum = 1;

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
            int tiempo, typeProcess; ;
            for (int i = 0; i < 13; i++)
            {
                tiempo = random.Next(3, 15);
                typeProcess = random.Next(1, 3);

                Proceso proceso = new Proceso(process[i].Id, process[i].ProcessName, tiempo, typeProcess);
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
            string LectOrWritter;

            if (proceso.TypeProcess == 1)
            {
                LectOrWritter = "READER";
            }
            else
            {
                LectOrWritter = "WRITTER";
            }

            string[] row = {id, nombre, estado, tiempo, LectOrWritter};
            dataGridViewProcesos.Rows.Add(row);
        }

        private void IniciarRR()
        {
            Proceso[] arrProcesos = procesos.ToArray();
            roundRobin = new RoundRobin(ref dataGridViewProcesos);
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
