using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using ParseMainFULL; 


// Note using padleft and padright for the good print out format 

// This application focus on analyzing Bicep Report 
namespace ReportAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bicep Report Analysis Test ");

            string pathToSerialNumberFolder = @"C:\Bicep_Project\ReprotApplication\Bicep_Data\SN_22040007_FAIL2 _STATION1\Main_FULL.csv";
            string setpoint8kv = "";
            string setpoint3kv = "";


            string table_boarder= "|///////////////////////////////////////////////////////////////////////////////|\n";
           

            MeterReading objMeter = new MeterReading(pathToSerialNumberFolder, "sampple");
            
            objMeter.ConvertMeterReadingInto_Double();
            objMeter.GetMeterResult();
            
            for (int i = 0; i < objMeter.MeterVoutPOS_H.Length; i++)
            {
               
                int errorCounter = 0;

                
                Double POS_Dev =0.000;
                Double POS_Dev_Percent = 0.000;

                Double NEG_Dev =0.000;
                Double NEG_Dev_Percent =0.000;

                Double BIAS_Dev = 0.000;
                Double BIAS_Dev_Percent = 0.000;


                // ------------------ V Postive 
                if (objMeter.MeterResultVoutPOS[i].Contains("LOW"))
                {
                    POS_Dev = Math.Abs(objMeter.MeterVoutPOS_L[i]) - Math.Abs(objMeter.MeterVoutPOS[i]);
                    POS_Dev_Percent = Math.Round((POS_Dev*100)/ Math.Abs(objMeter.MeterVoutPOS_L[i]), 3);

                    errorCounter++;
                }
                else if (objMeter.MeterResultVoutPOS[i].Contains("HIGH"))
                {
                    POS_Dev = Math.Abs(objMeter.MeterVoutPOS[i]) - Math.Abs(objMeter.MeterVoutPOS_H[i]);
                    POS_Dev_Percent = Math.Round((POS_Dev * 100) / Math.Abs(objMeter.MeterVoutPOS_H[i]), 3);
                    errorCounter++;
                }

                // ------------------- V Negative
                if (objMeter.MeterResultVoutNEG[i].Contains("LOW"))
                {
                    NEG_Dev = Math.Abs(objMeter.MeterVoutNEG_L[i]) - Math.Abs(objMeter.MeterVoutNEG[i]);
                    NEG_Dev_Percent = Math.Round((NEG_Dev * 100) / Math.Abs(objMeter.MeterVoutNEG_L[i]), 3);
                    errorCounter++;
                }
                else if (objMeter.MeterResultVoutNEG[i].Contains("HIGH"))
                {
                    NEG_Dev = Math.Abs(objMeter.MeterVoutNEG[i]) - Math.Abs(objMeter.MeterVoutNEG_H[i]);
                    NEG_Dev_Percent = Math.Round((NEG_Dev * 100) / Math.Abs(objMeter.MeterVoutNEG_H[i]), 3);
                    errorCounter++;
                }

                // ------------------- V Bias
                if (!objMeter.MeterResultVbias[i].Contains('P'))
                {
                    if (objMeter.MeterResultVbias[i].Contains("LOW"))
                    {
                        BIAS_Dev = (objMeter.BiasSetPoint[i] * 0.99) - objMeter.BiasSetPoint[i];
                        errorCounter++;

                    }
                    else if (objMeter.MeterResultVbias[i].Contains("HIGH"))
                    {
                        BIAS_Dev = (objMeter.BiasSetPoint[i] * 1.01) - objMeter.BiasSetPoint[i];
                        errorCounter++;
                    }
                    
                }

               

                // Setpoint  [Clamp , Bias]
                string Clamp = Convert.ToString(objMeter.ClampSetPoint[i]).PadRight(5);
                string Bias = Convert.ToString(objMeter.BiasSetPoint[i]).PadRight(4);

          
                // POSITIVE Convert to String with alignment 
                string vPOS_L = Convert.ToString(objMeter.MeterVoutPOS_L[i]).PadRight(7);
                string vPOS = Convert.ToString(objMeter.MeterVoutPOS[i]).PadRight(6);
                string vPOS_H = Convert.ToString(objMeter.MeterVoutPOS_H[i]).PadRight(7);
                string vPOS_R = objMeter.MeterResultVoutPOS[i].PadRight(10);
                string POS_D = Convert.ToString(POS_Dev).PadRight(3);
                string POS_D_P = Convert.ToString(POS_Dev_Percent).PadRight(5);

                // NEGATIVE Convert to String with alignment 
                string vNEG_L = Convert.ToString(objMeter.MeterVoutNEG_L[i]).PadRight(7);
                string vNEG = Convert.ToString(objMeter.MeterVoutNEG[i]).PadRight(6);
                string vNEG_H = Convert.ToString(objMeter.MeterVoutNEG_H[i]).PadRight(7);
                string vNEG_R = objMeter.MeterResultVoutNEG[i].PadRight(10);
                string NEG_D = Convert.ToString(NEG_Dev).PadRight(3);
                string NEG_D_P = Convert.ToString(NEG_Dev_Percent).PadRight(5);


                if (errorCounter != 0)
                {
                    // VBIAS Convert to String with alignment
                    string vBias_L = Convert.ToString(objMeter.BiasSetPoint[i] * 0.99).PadRight(7);
                    string vBias = Convert.ToString(objMeter.MeterVbias[i]).PadRight(6);
                    string vBias_H = Convert.ToString(objMeter.BiasSetPoint[i] * 1.01).PadRight(7);
                    string vBias_R = Convert.ToString(objMeter.MeterResultVbias[i]).PadRight(10);
                    string Bias_D = Convert.ToString(BIAS_Dev).PadRight(3);
                    string Bias_D_P = Convert.ToString(BIAS_Dev_Percent).PadRight(5);

                    // VDIFF Convert to String with alignment 
                    string vDIF_N = Convert.ToString(objMeter.MeterVoutNEG[i]).PadRight(7);
                    string vDIF = Convert.ToString(objMeter.MeterVdiff[i]).PadRight(6);
                    string vDIF_P = Convert.ToString(objMeter.MeterVoutPOS[i]).PadRight(7);




                    // Setpoint
                    Console.WriteLine($"|--------------------[{Clamp},{Bias}]---------------------------------|-[Deviation]-|");

                    // Positive
                    Console.WriteLine($"| V(+)_LOW --> {vPOS_L}  {vPOS}  {vPOS_H} <-- V(+)_HIGH | {vPOS_R}| {POS_D} ({POS_D_P}%)|");

                    // Negative 
                    Console.WriteLine($"| V(-)_LOW --> {vNEG_L}  {vNEG}  {vNEG_H} <-- V(-)_HIGH | {vNEG_R}| {NEG_D} ({NEG_D_P}%)|");

                    // Vbias 
                    Console.WriteLine($"| Vbias_LOW -> {vBias_L}  {vBias}  {vBias_H} <- Vbias_HIGH | {vBias_R}| {Bias_D} ({Bias_D_P}%)|");


                    Console.WriteLine("|-----------------Vdiff Computation---------------------------------------------|");
                    //Vdiff
                    Console.WriteLine($"| V(-)      -> {vDIF_N}  {vDIF}  {vDIF_P} <-       V(+) |-------------------------|  ");

                    Console.WriteLine(table_boarder);
                }


            }

            EcatReading objEcat = new EcatReading(pathToSerialNumberFolder, "sampple");

            objEcat.GetEcatBoundary();
            objEcat.ConvertEcatReadingInto_Double();
            objEcat.GetEcatResult();

            for (int i = 0; i < objEcat.EcatVoutNEG.Length; i++)
            {
                Console.WriteLine($"{objEcat.EcatResultVoutPOS[i]}" +
                                  $"\t{objEcat.EcatResultVoutNEG[i]}" +
                                  $"\t{objEcat.EcatResultVdiff[i]}" +
                                  $"\t{objEcat.EcatResultCTout[i]}");
            }


            Console.ReadKey();


            
        }
    }
}
