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

            string pathToSerialNumberFolder = @"C:\Users\bang.nguyen\OneDrive - XP Power\Desktop\Bicept project\ReprotApplication\Bicep_Data\SN_22040007_FAIL2 _STATION1\Main_FULL.csv";
            string setpoint8kv = "";
            string setpoint3kv = "";


            EcatReading objEcat = new EcatReading(pathToSerialNumberFolder, "sampple");

            objEcat.GetEcatBoundary();
            objEcat.ConvertEcatReadingInto_Double();
            objEcat.GetEcatResult();


            string table_boarder= "|///////////////////////////////////////////////////////////////////////////////////////////|\n";
           

            MeterReading objMeter = new MeterReading(pathToSerialNumberFolder, "sampple");
            
            objMeter.ConvertMeterReadingInto_Double();
            objMeter.GetMeterResult();
            
            for (int i = 0; i < objMeter.MeterVoutPOS_H.Length; i++)
            {
               
                int errorCounter = 0;

                // --------- Meter Section ---------
                Double Meter_POS_Dev =0.000;
                Double Meter_POS_Dev_Percent = 0.000;

                Double Meter_NEG_Dev = 0.000;
                Double Meter_NEG_Dev_Percent = 0.000;

                Double Meter_BIAS_Dev = 0.000;
                Double Meter_BIAS_Dev_Percent = 0.000;

                // --------- Ecat Section ---------
                Double Ecat_POS_Dev = 0.000;
                Double Ecat_POS_Dev_Percent = 0.000;

                Double Ecat_NEG_Dev = 0.000;
                Double Ecat_NEG_Dev_Percent = 0.000;

                Double Ecat_BIAS_Dev = 0.000;
                Double Ecat_BIAS_Dev_Percent = 0.000;


                // ------------------ V Postive 
                if (objMeter.MeterResultVoutPOS[i].Contains("LOW"))
                {
                    Meter_POS_Dev = Math.Abs(objMeter.MeterVoutPOS_L[i]) - Math.Abs(objMeter.MeterVoutPOS[i]);
                    Meter_POS_Dev_Percent = Math.Round((Meter_POS_Dev*100)/ Math.Abs(objMeter.MeterVoutPOS_L[i]), 3);

                    errorCounter++;
                }
                else if (objMeter.MeterResultVoutPOS[i].Contains("HIGH"))
                {
                    Meter_POS_Dev = Math.Abs(objMeter.MeterVoutPOS[i]) - Math.Abs(objMeter.MeterVoutPOS_H[i]);
                    Meter_POS_Dev_Percent = Math.Round((Meter_POS_Dev * 100) / Math.Abs(objMeter.MeterVoutPOS_H[i]), 3);
                    errorCounter++;
                }

                // ------------------- V Negative
                if (objMeter.MeterResultVoutNEG[i].Contains("LOW"))
                {
                    Meter_NEG_Dev = Math.Abs(objMeter.MeterVoutNEG_L[i]) - Math.Abs(objMeter.MeterVoutNEG[i]);
                    Meter_NEG_Dev_Percent = Math.Round((Meter_NEG_Dev * 100) / Math.Abs(objMeter.MeterVoutNEG_L[i]), 3);
                    errorCounter++;
                }
                else if (objMeter.MeterResultVoutNEG[i].Contains("HIGH"))
                {
                    Meter_NEG_Dev = Math.Abs(objMeter.MeterVoutNEG[i]) - Math.Abs(objMeter.MeterVoutNEG_H[i]);
                    Meter_NEG_Dev_Percent = Math.Round((Meter_NEG_Dev * 100) / Math.Abs(objMeter.MeterVoutNEG_H[i]), 3);
                    errorCounter++;
                }

                // ------------------- V Bias
                if (!objMeter.MeterResultVbias[i].Contains('P'))
                {
                    if (objMeter.MeterResultVbias[i].Contains("LOW"))
                    {
                        Meter_BIAS_Dev = (objMeter.BiasSetPoint[i] * 0.99) - objMeter.BiasSetPoint[i];
                        errorCounter++;

                    }
                    else if (objMeter.MeterResultVbias[i].Contains("HIGH"))
                    {
                        Meter_BIAS_Dev = (objMeter.BiasSetPoint[i] * 1.01) - objMeter.BiasSetPoint[i];
                        errorCounter++;
                    }
                    
                }


                //==================================================================
                //                Meter Convert to string section
                //==================================================================
                // Setpoint  [Clamp , Bias]
                string Clamp = Convert.ToString(objMeter.ClampSetPoint[i]).PadRight(5);
                string Bias = Convert.ToString(objMeter.BiasSetPoint[i]).PadRight(4);


                // POSITIVE Convert to String with alignment 
                string Meter_vPOS_L = Convert.ToString(objMeter.MeterVoutPOS_L[i]).PadRight(9);
                string Meter_vPOS = Convert.ToString(objMeter.MeterVoutPOS[i]).PadRight(8);
                string Meter_vPOS_H = Convert.ToString(objMeter.MeterVoutPOS_H[i]).PadRight(9);
                string Meter_vPOS_R = objMeter.MeterResultVoutPOS[i].PadRight(10);
                string Meter_POS_D = Convert.ToString(Meter_POS_Dev).PadRight(3);
                string Meter_POS_D_P = Convert.ToString(Meter_POS_Dev_Percent).PadRight(5);

                // NEGATIVE Convert to String with alignment 
                string Meter_vNEG_L = Convert.ToString(objMeter.MeterVoutNEG_L[i]).PadRight(9);
                string Meter_vNEG = Convert.ToString(objMeter.MeterVoutNEG[i]).PadRight(8);
                string Meter_vNEG_H = Convert.ToString(objMeter.MeterVoutNEG_H[i]).PadRight(9);
                string Meter_vNEG_R = objMeter.MeterResultVoutNEG[i].PadRight(10);
                string Meter_NEG_D = Convert.ToString(Meter_NEG_Dev).PadRight(3);
                string Meter_NEG_D_P = Convert.ToString(Meter_NEG_Dev_Percent).PadRight(5);

                // VBIAS Convert to String with alignment
                string Meter_vBias_L = Convert.ToString(objMeter.BiasSetPoint[i] * 0.99).PadRight(9);
                string Meter_vBias = Convert.ToString(objMeter.MeterVbias[i]).PadRight(8);
                string Meter_vBias_H = Convert.ToString(objMeter.BiasSetPoint[i] * 1.01).PadRight(9);
                string Meter_vBias_R = Convert.ToString(objMeter.MeterResultVbias[i]).PadRight(10);
                string Meter_Bias_D = Convert.ToString(Meter_BIAS_Dev).PadRight(3);
                string Meter_Bias_D_P = Convert.ToString(Meter_BIAS_Dev_Percent).PadRight(5);

                // VDIFF Convert to String with alignment 
                string Meter_vDIF_N = Convert.ToString(objMeter.MeterVoutNEG[i]).PadRight(9);
                string Meter_vDIF = Convert.ToString(objMeter.MeterVdiff[i]).PadRight(8);
                string Meter_vDIF_P = Convert.ToString(objMeter.MeterVoutPOS[i]).PadRight(9);

                //==================================================================
                //                Ecat Convert to string section
                //==================================================================


                // POSITIVE Convert to String with alignment 
                string Ecat_vPOS_L = Convert.ToString(objEcat.EcatVoutPOS_L[i]).PadRight(9);
                string Ecat_vPOS = Convert.ToString(objEcat.EcatVoutPOS[i]).PadRight(8);
                string Ecat_vPOS_H = Convert.ToString(objEcat.EcatVoutPOS_H[i]).PadRight(9);
                string Ecat_vPOS_R = objEcat.EcatResultVoutPOS[i].PadRight(10);
                string Ecat_POS_D = Convert.ToString(Ecat_POS_Dev).PadRight(3);
                string Ecat_POS_D_P = Convert.ToString(Ecat_POS_Dev_Percent).PadRight(5);

                // NEGATIVE Convert to String with alignment 
                string Ecat_vNEG_L = Convert.ToString(objEcat.EcatVoutNEG_L[i]).PadRight(9);
                string Ecat_vNEG = Convert.ToString(objEcat.EcatVoutNEG[i]).PadRight(8);
                string Ecat_vNEG_H = Convert.ToString(objEcat.EcatVoutNEG_H[i]).PadRight(9);
                string Ecat_vNEG_R = objEcat.EcatResultVoutNEG[i].PadRight(10);
                string Ecat_NEG_D = Convert.ToString(Ecat_NEG_Dev).PadRight(3);
                string Ecat_NEG_D_P = Convert.ToString(Ecat_NEG_Dev_Percent).PadRight(5);

                // VBIAS Convert to String with alignment
                string Ecat_vBias_L = Convert.ToString(objEcat.BiasSetPoint[i] * 0.99).PadRight(9);
                string Ecat_vBias = Convert.ToString(objEcat.EcatCTout[i]).PadRight(8);
                string Ecat_vBias_H = Convert.ToString(objEcat.BiasSetPoint[i] * 1.01).PadRight(9);
                string Ecat_vBias_R = Convert.ToString(objEcat.EcatResultCTout[i]).PadRight(10);
                string Ecat_Bias_D = Convert.ToString(Ecat_BIAS_Dev).PadRight(3);
                string Ecat_Bias_D_P = Convert.ToString(Ecat_BIAS_Dev_Percent).PadRight(5);

                // VDIFF Convert to String with alignment ( Vdiff LOW and Vdiff HIGH)
                string Ecat_vDIF_L = Convert.ToString(objEcat.EcatVdiff_L[i]).PadRight(9);
                string Ecat_vDIF = Convert.ToString(objEcat.EcatVdiff[i]).PadRight(8);
                string Ecat_vDIF_H = Convert.ToString(objEcat.EcatVdiff_H[i]).PadRight(9);
                string Ecat_vDIF_R = objEcat.EcatResultVdiff[i].PadRight(10);



                if (errorCounter != 0)
                {


                    //==================================================================
                    //                Meter Convert to string section
                    //==================================================================


                    // Setpoint
                    Console.WriteLine($"|--------------------[{Clamp},{Bias}]---------------------------|-[Result]--|----[Deviation]----|");

                    // Positive
                    Console.WriteLine($"| V(+)_LOW --> {Meter_vPOS_L}  {Meter_vPOS}  {Meter_vPOS_H} <-- V(+)_HIGH | {Meter_vPOS_R}| Volt: {Meter_POS_D} ({Meter_POS_D_P}%)|");

                    // Negative 
                    Console.WriteLine($"| V(-)_LOW --> {Meter_vNEG_L}  {Meter_vNEG}  {Meter_vNEG_H} <-- V(-)_HIGH | {Meter_vNEG_R}| Volt: {Meter_NEG_D} ({Meter_NEG_D_P}%)|");

                    // Vbias 
                    Console.WriteLine($"| Vbias_LOW -> {Meter_vBias_L}  {Meter_vBias}  {Meter_vBias_H} <- Vbias_HIGH | {Meter_vBias_R}| Volt: {Meter_Bias_D} ({Meter_Bias_D_P}%)|");


                    Console.WriteLine("|----------------------vDiff METER----------------------------------------------------------|");
                    //Vdiff
                    Console.WriteLine($"| V(-)      -> {Meter_vDIF_N}  {Meter_vDIF}  {Meter_vDIF_P} <-       V(+) |-------------------------------|  ");


                    Console.WriteLine(("-------------------------ECAT SECTION-------------------------------------------------------|"));

                    // Positive
                    Console.WriteLine($"| V(+)_LOW --> {Ecat_vPOS_L}  {Ecat_vPOS}  {Ecat_vPOS_H} <-- V(+)_HIGH | {Ecat_vPOS_R}| Volt: {Ecat_POS_D} ({Ecat_POS_D_P}%)|");

                    // Negative 
                    Console.WriteLine($"| V(-)_LOW --> {Ecat_vNEG_L}  {Ecat_vNEG}  {Ecat_vNEG_H} <-- V(-)_HIGH | {Ecat_vNEG_R}| Volt: {Ecat_NEG_D} ({Ecat_NEG_D_P}%)|");

                    // Vbias 
                    Console.WriteLine($"| CTout_LOW -> {Ecat_vBias_L}  {Ecat_vBias}  {Ecat_vBias_H} <- CTout_HIGH | {Ecat_vBias_R}| Volt: {Ecat_Bias_D} ({Ecat_Bias_D_P}%)|");

                   

                    //Console.WriteLine("|-----------------vDiff ECAT----------------------------------------------------------------|");
                    //Vdiff
                    Console.WriteLine($"| vDiff_L   -> {Ecat_vDIF_L}  {Ecat_vDIF}  {Ecat_vDIF_H} <-    vDiff_H | {Ecat_vDIF_R}|-------------------|  ");

                    Console.WriteLine(table_boarder);
                    Console.WriteLine((" "));


                    //==================================================================
                    //                Ecat Convert to string section
                    //==================================================================



                }


            }


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
