using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using NAudio.Wave;
using NAudio.CoreAudioApi;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {













   
       public  WaveIn input;
        //поток для речи собеседника
       public WaveOut output;
        //буфферный поток для передачи через сеть
        public BufferedWaveProvider bufferStream;
        //поток для прослушивания входящих сообщений
    
        public Form1()
        {
            InitializeComponent();
            input = new WaveIn();
            //определяем его формат - частота дискретизации 16000 Гц, ширина сэмпла - 16 бит, 1 канал - моно
            input.WaveFormat = new WaveFormat(16000, 16, 1);
            //добавляем код обработки нашего голоса, поступающего на микрофон
            input.DataAvailable += Voice_Input;
            //создаем поток для прослушивания входящего звука
            output = new WaveOut();
            //создаем поток для буферного потока и определяем у него такой же формат как и потока с микрофона
            bufferStream = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
            //привязываем поток входящего звука к буферному потоку
       
            
        }
        public int n_size = 10;
        public double[,] data = new double[10,100];
         public  class Neuron
        {
             public bool lockingMemory = false;
              public double sum = 0;  
            public const int heigth=100,width=10;
            public double[,] inputs = new double[width,heigth];
            public double[,] weight = new double[width, heigth];
            public double[] outputs = new double[4]; 
           public  Neuron()
            {




            }
           public void Absolute(){
                sum = 0; 
               for(int i=0;i<width;i++)
                   for (int j = 0; j < heigth; j++)
                   {
                       sum += inputs[i, j] * weight[i, j];


                   }
              
           }
               public double Sign(){

                   return Math.Sign(sum);



               }

            public double Tanh(){

                   return Math.Tanh(sum);



               }
          
            public void Result(){
                outputs[0]=sum;
                outputs[1]=Tanh();
                outputs[2]=Sign();



            }

            }
         public Neuron[] Sum = new Neuron[10];
         public double error = 0;
         public void LearningPerseptron() // обучение
         {
             label1.Text = "";
             
             
                
                  for (int i = 0; i < 10; i++)
                  {
                      double answer=Convert.ToDouble(listBox1.Items[i]);
                      Sum[i].Absolute();
                      Sum[i].Result();
                      label1.Text += " " + Sum[i].outputs[0].ToString() + " ";
                      if(Sum[i].lockingMemory==false) 
                      for (int j = 0; j < n_size; j++)
                      {
                          double difference = answer - Sum[i].outputs[0];
                          error = (0.001 * difference * (Sum[i].inputs[0, j]/ Math.Pow(10, Sum[j].inputs[0, j].ToString().Length - 1)));
        
                          Sum[i].weight[0, j] += error;
                          Sum[i].Absolute();
                          Sum[i].Result();
                          if (Math.Abs(answer - Sum[i].outputs[0]) < 0.01 && Sum[i].outputs[0]>0) { if (Sum[i].lockingMemory != true) { Sum[i].lockingMemory = true; MessageBox.Show(i.ToString()); } }



                      }

                  







             }






         }
         public double maximum = 0;
         public double minimum = 0;
         public int index = 0;
         public int lastindex = 0;
        public void DrawMainStream() {
            Bitmap mainstream = new Bitmap(600, 50);
            Graphics g = Graphics.FromImage(mainstream); 
            for (int i = 0; i < 10; i++) { 
                if (Sum[i].lockingMemory == true)
                { Sum[i].Absolute();
                Sum[i].Result();
                    if (Sum[i].outputs[0] > maximum) 
                    { maximum = Sum[i].outputs[0]; } } } minimum = maximum;
            for (int i = 0; i < 10; i++) { if (Sum[i].lockingMemory == true) { Sum[i].Absolute(); Sum[i].Result(); 
                if (Sum[i].outputs[0] < minimum && Sum[i].outputs[0] > 0  && Math.Abs(Sum[i].outputs[0]-1)<0.1) { minimum = Sum[i].outputs[0]; index = i; } } } for (int i = 0; i < 10; i++) { g.DrawEllipse(new Pen(Color.Black, 2), 10 + i * 16, 10, 10, 10); if (i == index) { g.FillEllipse(new SolidBrush(Color.Green), 10 + i * 16, 10, 10, 10); lastindex = index; } else if (i != index && Sum[i].outputs[0]>0) { g.FillEllipse(new SolidBrush(Color.Yellow), 10 + i * 16, 10, 10, 10); } else { g.FillEllipse(new SolidBrush(Color.Red), 10 + i * 16, 10, 10, 10); } } g.Dispose(); pictureBox1.Image = mainstream; }




     private void Form1_Load(object sender, EventArgs e)
     {
         TempStr = listBox1.Items[0].ToString();
         for (int i = 0; i < 10; i++)
         {

             Sum[i] = new Neuron();
             if (textBox1.Text != "" && textBox2.Text !="")
             {
                 Sum[i].inputs[0, 0] = Convert.ToDouble(textBox1.Text);
                 Sum[i].inputs[0, 1] = Convert.ToDouble(textBox2.Text); 


             }            
             Sum[i].Absolute();
             Sum[i].Result();
             label1.Text += " " + Sum[i].outputs[0].ToString() + " ";
         }
         Random r = new Random();
         for (int i = 0; i < 10; i++)
         {
             for (int j = 0; j < n_size; j++)
             {
                 Sum[i].weight[0, j] = r.NextDouble()+0.01;


             }

         }

           
        }

     private void timer1_Tick(object sender, EventArgs e)
     {
         LearningPerseptron();
         DrawMainStream();
        
     }

     private void button1_Click(object sender, EventArgs e)
     {
         for (int i = 0; i < 10; i++)
         {

             if (textBox1.Text != "" && textBox2.Text != "")
             {
                 Sum[i].inputs[0, 0] = Convert.ToDouble(textBox1.Text);
                 Sum[i].inputs[0, 1] = Convert.ToDouble(textBox2.Text); ;


             }
             Sum[i].Absolute();
             Sum[i].Result();
            
         }
         if (timer1.Enabled == true) timer1.Enabled = false;
         else timer1.Enabled = true;
     }
     public int eCount =0;
     public int temp = -1, lasttemp=-1;
     public string TempStr = "";
     private void button2_Click(object sender, EventArgs e)
     {
         eCount=0;
         temp++;
         if (temp> 9) temp = 0;
         if (temp!= lasttemp)
         {
             if (lasttemp < 0) lasttemp = 0;
             listBox1.Items[lasttemp] = TempStr;
             TempStr= listBox1.Items[temp].ToString();


             listBox1.Items[temp] += "!";

             lasttemp = temp;

         }

     }

     private void button3_Click(object sender, EventArgs e)
     {
         eCount++;
         TempStr = eCount.ToString();
         listBox1.Items[temp] = eCount.ToString();
           
     }

     private void button4_Click(object sender, EventArgs e)
     {
         eCount--;
         TempStr = eCount.ToString();
         listBox1.Items[temp] = eCount.ToString();
           
     }

     private void button5_Click(object sender, EventArgs e)
     {
         for (int i = 0; i < 100; i++) data[0, i] = 0;
         bool test = false;
         string text = "";
         string outtask="";
         string[] words = new string[1000];
         string[] dataWords = {"привет","как","у","тебя","дела","семья","здоровье","настроение","учёба","работа","что","делаешь","зовут","где","живёшь" };
         text = richTextBox1.Text;
         int a = 0;
         for (int i = 0; i < text.Length; i++)
         {

             if (text[i] == ' ' || text[i] == ',' || text[i] == '.' || text[i] == '*' || text[i] == '&' || text[i] == '^' || text[i] == '!' || text[i] == '?' || text[i] == '>' || text[i] == '<' || text[i] == '#' || text[i] == '$' || text[i] == '@') { if (test != true) a++; test = true; }
             else { test = false; words[a] += text[i]; }

         }
         

         for (int i = 0; i < dataWords.Length; i++)
         {
             for (int j = 0; j < a+1; j++)

                 if (dataWords[i] == words[j]) { data[0, j] = i; outtask += " :" + data[0, j].ToString();  }


         }
         for (int i = 0; i < 10; i++) { Sum[i].inputs = data; }

        
         label2.Text =outtask;

     }

     public EventHandler<WaveInEventArgs> Voice_Input { get; set; }
    }
    

}
