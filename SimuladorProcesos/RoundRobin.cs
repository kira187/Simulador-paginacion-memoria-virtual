using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimuladorProcesos
{
    public class RoundRobin
    {
        DataGridView dataGridView;
        PictureBox pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8;
        private List<Proceso> ListProcessesWaiting = new List<Proceso>();
        private List<PictureBox> ListPictureBoxs = new List<PictureBox>();

        //----------------RoundRobin Class Constructor-------------------
        public RoundRobin(ref DataGridView temp_dataGridView, ref PictureBox temp_pictureBox1, ref PictureBox temp_pictureBox2, ref PictureBox temp_pictureBox3, ref PictureBox temp_pictureBox4, ref PictureBox temp_pictureBox5, ref PictureBox temp_pictureBox6, ref PictureBox temp_pictureBox7, ref PictureBox temp_pictureBox8)
        {
            dataGridView = temp_dataGridView;
            pictureBox1 = temp_pictureBox1;
            pictureBox2 = temp_pictureBox2;
            pictureBox3 = temp_pictureBox3;
            pictureBox4 = temp_pictureBox4;
            pictureBox5 = temp_pictureBox5;
            pictureBox6 = temp_pictureBox6;
            pictureBox7 = temp_pictureBox7;
            pictureBox8 = temp_pictureBox8;

            PictureBox[] PB = { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox6, pictureBox7, pictureBox8 };
            ListPictureBoxs.AddRange(PB);
        }

        //----------------Main Round Robin Algorithm Method-------------------
        public void runRoundRobin(ref Proceso[] procesos, int quantum)
        {
            int freeSectors = 8;
            double sectors = 0;

            foreach (var proceso1 in procesos)
            {
                proceso1.TiempoRestante = proceso1.Tiempo;
            }
            while (true)
            {
                bool executionFinished = true;
                foreach (var task in procesos)
                {
                    if(task.Estado == "NEW" || task.Estado == "READY" || task.Tiempo!= 0) // 
                    {
                        if (task.Memoria > 64)
                        {
                            sectors = Math.Ceiling(task.Memoria / 64.0);
                        }
                        else
                        {
                            sectors = 1;
                        }

                        if (freeSectors >= 1 && ListProcessesWaiting.Count() > 0)
                        {
                            List<int> arrayAux = new List<int>();
                            int j = 0;
                            foreach (var task2 in ListProcessesWaiting)
                            {
                                // Calculo de sectores
                                if (task2.Memoria > 64)
                                {
                                    sectors = Math.Ceiling(task2.Memoria / 64.0);
                                }
                                else
                                {
                                    sectors = 1;
                                }
                                
                                if (sectors <= freeSectors)
                                {
                                    // Pintado de sectores que utilizara el proceso
                                    foreach (var PB in ListPictureBoxs)
                                    {
                                        if (PB.BackColor == Color.DarkGray)
                                        {
                                            PB.BackColor = System.Drawing.ColorTranslator.FromHtml(task2.Color);
                                            sectors--;
                                            freeSectors--;
                                            if (sectors == 0) { break; }
                                        }
                                    }

                                    //algoritmo round robin
                                    if (task2.TiempoRestante > 0)
                                    {
                                        executionFinished = false;
                                        if (task2.TiempoRestante > quantum)
                                        {

                                            task2.Estado = "RUNNING";
                                            updateDataGridView(dataGridView, procesos);
                                            executionTimer(quantum);

                                            task2.TiempoRestante = task2.TiempoRestante - quantum;

                                            task2.Estado = "READY";
                                            updateDataGridView(dataGridView, procesos);

                                            arrayAux.Add(j);
                                        }
                                        else
                                        {
                                            while (task2.IO > 0)
                                            {
                                                ioExecution(procesos, task2.Id, task2.IO);
                                                task2.IO = task2.IO - 1;
                                            }

                                            task2.Estado = "RUNNING";
                                            updateDataGridView(dataGridView, procesos);
                                            executionTimer(task2.TiempoRestante);

                                            task2.TiempoRestante = 0;

                                            task2.Estado = "COMPLETED";
                                            updateDataGridView(dataGridView, procesos);

                                            double aux = Math.Ceiling(task2.Memoria / 64.0);

                                            freeSectors += (int)aux;
                                            foreach (var PB2 in ListPictureBoxs)
                                            {
                                                if (PB2.BackColor == System.Drawing.ColorTranslator.FromHtml(task2.Color))
                                                {
                                                    PB2.BackColor = Color.DarkGray;
                                                    aux--;
                                                    if (aux == 0) { break; }
                                                }
                                            }
                                            arrayAux.Add(j);
                                        }
                                    }
                                    if (task.IO > 0)
                                    {
                                        ioExecution(procesos, task.Id, task.IO);
                                        task.IO = task.IO - 1;
                                    }
                                }
                                j++;

                            }
                            foreach(var i in arrayAux)
                            {
                                ListProcessesWaiting.RemoveAt(i);
                            }
                        }
                        if (sectors <= freeSectors || task.Estado == "READY")
                        {
                            // Pintado de sectores que utilizara el proceso
                            if (task.Estado == "NEW")  // || task.Estado == "WAITING"
                            {
                                foreach (var PB in ListPictureBoxs)
                                {
                                    if (PB.BackColor == Color.DarkGray)
                                    {
                                        PB.BackColor = System.Drawing.ColorTranslator.FromHtml(task.Color);
                                        sectors--;
                                        freeSectors--;
                                        if (sectors == 0) { break; }
                                    }
                                }
                            }
                            

                            if (task.TiempoRestante > 0)
                            {
                                executionFinished = false;
                                if (task.TiempoRestante > quantum)
                                {

                                    task.Estado = "RUNNING";
                                    updateDataGridView(dataGridView, procesos);
                                    executionTimer(quantum);

                                    task.TiempoRestante = task.TiempoRestante - quantum;

                                    task.Estado = "READY";
                                    updateDataGridView(dataGridView, procesos);
                                }
                                else
                                {
                                    while (task.IO > 0)
                                    {
                                        ioExecution(procesos, task.Id, task.IO);
                                        task.IO = task.IO - 1;
                                    }

                                    task.Estado = "RUNNING";
                                    updateDataGridView(dataGridView, procesos);
                                    executionTimer(task.TiempoRestante);

                                    task.TiempoRestante = 0;
                                    task.Estado = "COMPLETED";
                                    updateDataGridView(dataGridView, procesos);

                                    double aux = Math.Ceiling(task.Memoria / 64.0);

                                    freeSectors += (int)aux;
                                    foreach (var PB in ListPictureBoxs)
                                    {
                                        if (PB.BackColor == System.Drawing.ColorTranslator.FromHtml(task.Color))
                                        {
                                            PB.BackColor = Color.DarkGray;
                                            aux--;
                                            if (aux == 0) { break; }
                                        }
                                    }

                                }
                            }
                            if (task.IO > 0)
                            {
                                ioExecution(procesos, task.Id, task.IO);
                                task.IO = task.IO - 1;
                            }
                        }
                        else
                        {
                            Console.WriteLine("SIN ESPACIO ELSE");
                            task.Estado = "WAITING";
                            updateDataGridView(dataGridView, procesos);

                            ListProcessesWaiting.Add(task);
                        }
                    }
                }
                if (executionFinished == true) { break; }
            }

            //Limpia los recuadros faltantes
            foreach (var PB in ListPictureBoxs)
            {
                if (PB.BackColor != Color.Gray)
                {
                    PB.BackColor = Color.DarkGray;
                }
            }
        }

        public void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            if (dgv.Columns[e.ColumnIndex].Name == "Estado")  //Si es la columna a evaluar
            {
                if (e.Value.ToString().Contains("RUNNING"))   //Si el valor de la celda contiene la palabra hora
                {
                    e.CellStyle.ForeColor = Color.Red;
                }
            }
        }
        
        //----------------Update Data Grid View Method-------------------------------
        public void updateDataGridView(DataGridView dataGridView, Proceso[] procesos)
        {

            dataGridView.Rows.Clear();
            int numRows = 0;

            foreach (var proceso in procesos)
            {
                string[] row = { proceso.Id.ToString(), proceso.Nombre, proceso.Estado, proceso.TiempoRestante.ToString(), proceso.Memoria.ToString() };
                dataGridView.Rows.Add(row);

                if (proceso.Estado == "RUNNING")
                {
                    numRows = dataGridView.Rows.Count - 1; }
            }
            dataGridView.ClearSelection();
            dataGridView.Rows[numRows].Selected = true;

        }
        //----------------Process IO Execution Method
        public void ioExecution(Proceso[] procesos, int id, int interupt)
        {
            //Change Process State to Waiting when it goes to IO
            foreach (var proceso in procesos)
            {
                if (proceso.Id == id && proceso.Estado != "COMPLETED")
                {
                    proceso.Estado = "WAITING";
                }
            }
            updateDataGridView(dataGridView, procesos);

            //Execute the IO for 1 second
            executionTimer(1);

            //Change Process back to Ready State after IO has completed
            foreach (var proceso in procesos)
            {
                if (proceso.Id == id && proceso.Estado != "COMPLETED")
                {
                    proceso.Estado = "READY";
                }
            }
            updateDataGridView(dataGridView, procesos);
        }

        //----------------Process Execution Timer Method
        public void executionTimer(int tempTime)
        {
            int executionTime = tempTime * 2000;
            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            if (executionTime == 0 || executionTime < 0)
            {
                return;
            }
            timer1.Interval = executionTime;
            timer1.Enabled = true;
            timer1.Start();
            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
            };
            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }
    }
}