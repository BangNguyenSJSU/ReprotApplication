using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using ReportAnalysis;


// This application focus on analyzing Bicep Report 
namespace ReportAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bicep Report Analysis Test ");

            string pathToSerialNumberFolder = @"C:\SN_22040007_FAIL2 _STATION1\Main_FULL.csv";

            


            MeterReading objMeter = new MeterReading(pathToSerialNumberFolder, "sampple");
            
            objMeter.ConvertMeterReadingInto_Double();
            objMeter.GetMeterResult();
            
            for (int i = 0; i < objMeter.MeterVoutPOS_H.Length; i++)
            {
               
                string[] ErrorType = { "Value < L_Limit", "Value > H_Limit", "Unexpected ON"}; 

                // Result 
                string[] Status = {"_PASS_","FAIL_L","FAIL_H","_FAIL_"};
                string POS_Result;
                Double POS_Deviation;
                string errorPOS;

                string NEG_Result;
                Double NEG_Deviation;
                string errorNEG;

                string Vbias_Result;
                Double Vbias_Deviation;
                string errorVbias;
                //-------------------------------------------------------
                if (!objMeter.MeterResultVoutPOS[i].Contains('P'))
                {
                    if (objMeter.MeterResultVoutPOS[i].Contains("LOW"))
                    {
                        POS_Result = Status[1];
                        POS_Deviation = Math.Abs(objMeter.MeterVoutPOS_L[i]) - Math.Abs(objMeter.MeterVoutPOS[i]);
                        // {objMeter.MeterVoutPOS_L[i]}  {objMeter.MeterVoutPOS[i]}
                        POS_Deviation = (POS_Deviation / objMeter.MeterVoutPOS_L[i]) * 100;
                        errorPOS = ErrorType[0];

                    }
                    else if (objMeter.MeterResultVoutPOS[i].Contains("HIGH"))
                    {
                        POS_Result = Status[2];
                        POS_Deviation = Math.Abs(objMeter.MeterVoutPOS[i]) - Math.Abs(objMeter.MeterVoutPOS_H[i]);
                        POS_Deviation = (POS_Deviation / objMeter.MeterVoutPOS_H[i]) * 100;
                        errorPOS = ErrorType[1];
                    }
                    else
                    {
                        POS_Result = Status[3];
                        errorPOS = ErrorType[2];
                    }
                }
                else
                {
                    POS_Result = Status[0];
                    errorPOS = "_GOOD_";
                }

                //----------------------------------------------------------
                if (!objMeter.MeterResultVoutNEG[i].Contains('P'))
                {
                    if (objMeter.MeterResultVoutNEG[i].Contains("LOW"))
                    {
                        NEG_Result = Status[1];
                        NEG_Deviation = Math.Abs(objMeter.MeterVoutNEG_L[i]) - Math.Abs(objMeter.MeterVoutNEG[i]);
                        NEG_Deviation = (NEG_Deviation / objMeter.MeterVoutPOS_L[i]) * 100;
                        errorNEG = ErrorType[0];
                    }
                    else if (objMeter.MeterResultVoutNEG[i].Contains("HIGH"))
                    {
                        NEG_Result = Status[2];
                        NEG_Deviation = Math.Abs(objMeter.MeterVoutNEG[i])- Math.Abs(objMeter.MeterVoutNEG_L[i]);
                        NEG_Deviation = (NEG_Deviation / objMeter.MeterVoutPOS_H[i]) * 100;
                        errorNEG = ErrorType[1];
                    }
                    else
                    {
                        NEG_Result = Status[3];
                        errorNEG = ErrorType[2];
                    }
                }
                else
                {
                    NEG_Result = Status[0];
                    errorNEG = "_GOOD_";
                }

                //---------------------------------------------------------
                if (!objMeter.MeterResultVbias[i].Contains('P'))
                {
                    if (objMeter.MeterResultVbias[i].Contains("LOW"))
                    {
                        Vbias_Result = Status[1];
                        errorVbias = ErrorType[0];
                    }
                    else if (objMeter.MeterResultVbias[i].Contains("HIGH"))
                    {
                        Vbias_Result = Status[2];
                        errorVbias = ErrorType[1];
                    }
                    else
                    {
                        Vbias_Result = Status[3];
                        errorVbias = ErrorType[2];
                    }
                }
                else
                {
                    Vbias_Result = Status[0];
                    errorVbias = "_GOOD_";
                }

                Console.WriteLine("//////////////////////////////////////////////////////////////////////////////////////////");
                Console.WriteLine($"\t\t\t\t Meter Result:[{objMeter.ClampSetPoint[i]} , {objMeter.BiasSetPoint[i]}]");
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine($"| V(+):{POS_Result} = {errorPOS} | V(-):{NEG_Result} = {errorNEG} | Vbias:{Vbias_Result} = {errorVbias} |");
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("//////////////////////////////////////////////////////////////////////////////////////////");
            }

               

            /*for (int i = 0; i < objMeterReading.MeterVoutPOS_H.Length; i++)
            {
                Console.WriteLine($"[{i}]\t{objMeterReading.ClampSetPoint[i]}\t{objMeterReading.BiasSetPoint[i]} | {objMeterReading.MeterVoutPOS_H[i]}\t{objMeterReading.MeterVoutPOS_L[i]}\t{objMeterReading.MeterVoutPOS[i]}\t" +
                                  $"| {objMeterReading.MeterVoutNEG_H[i]}\t{objMeterReading.MeterVoutNEG_L[i]}\t{objMeterReading.MeterVoutNEG[i]}\t" +
                                  $"| {objMeterReading.MeterVdiff[i]}\t{objMeterReading.MeterCTout[i]}\n\n" +
                                  $"| {objMeterReading.MeterResultVoutPOS[i]}\t{objMeterReading.MeterResultVoutNEG[i]}\t\t{objMeterReading.MeterResultVbias[i]}");

                Console.WriteLine("------------------------------------------------------------------");
            }*/

           

           
            

            /*for (int row = 0; row < reportMainTest.Length; row++)
            {
                Console.WriteLine(reportMainTest[row]);
            }*/

            /*Console.WriteLine(reportMainTest[endRow]);
            Console.WriteLine(reportMainTest[VSet3kV_Row]);
            Console.WriteLine(reportMainTest[Vset8kV_Row]);

            Console.WriteLine(reportMainTest[CTout_Row]);
            Console.WriteLine(reportMainTest[Vdiff_Row]);
            Console.WriteLine(reportMainTest[VoutNeg_Row]);*/
            
            //Console.WriteLine(reportMainTest_Row[Vout_RowIndex]);








            Console.ReadKey();



        }
    }
}
