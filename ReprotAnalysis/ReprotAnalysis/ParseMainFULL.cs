using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ParseMainFULL
{
    public class EcatReading
    {
        // Store SetPoint 
        public Double[] ClampSetPoint = new Double[175];
        public Double[] BiasSetPoint = new Double[175];

        public int ClampSetPoint_Index;
        public int BiasSetPoint_Index;

        // Store the result 
        public string[] EcatResultVoutPOS = new string[175];
        public string[] EcatResultVoutNEG = new string[175];
        public string[] EcatResultVdiff = new string[175];
        public string[] EcatResultCTout = new string[175];

        // Ecat Result Index 
        public int EcatResultVoutPOS_Index;
        public int EcatResultVoutNEG_Index;
        public int EcatResultVdiff_Index;
        public int EcatResultCTout_Index;


        // Store POS (size 175)
        public Double[] EcatVoutPOS_H = new Double[175];
        public Double[] EcatVoutPOS_L = new Double[175];
        public Double[] EcatVoutPOS = new Double[175];

        // Store NEG
        public Double[] EcatVoutNEG_H = new Double[175];
        public Double[] EcatVoutNEG_L = new Double[175];
        public Double[] EcatVoutNEG = new Double[175];

        // Store Vdiff 
        public Double[] EcatVdiff_H = new Double[175];
        public Double[] EcatVdiff_L = new Double[175];
        public Double[] EcatVdiff = new Double[175];

        // Store CTout (Vbias)
        public Double[] EcatCTout_H = new Double[175];
        public Double[] EcatCTout_L = new Double[175];
        public Double[] EcatCTout = new Double[175];


        // Positive Index
        public int EcatVoutPOS_Index;


        // Negative Index
        public int EcatVoutNEG_Index;


        // Vdiff + Bias Index
        public int EcatVdiff_Index;
        public int EcatCTout_Index;


        // input for class 
        string _filePath = @"";
        private string _dirPath = @"";

        // ---> Constructor 
        public EcatReading(string fileP, string dirP)
            {
                _filePath = fileP; // path to Main_FULL.csv
                _dirPath = dirP; // Path to serial Number foler 

                // ------> Initalize the index 
                string[] csvLine = System.IO.File.ReadAllLines(_filePath);
                int csvLine_Size = csvLine.Length;

                // -------> Positive Index
                string[] POS_Data = csvLine[csvLine_Size - 25].Split(',');

                bool POS_Good_Token = POS_Data[0].Contains("Vout+");

                // ------> Negative Index
                string[] NEG_Data = csvLine[csvLine_Size - 22].Split(',');

                bool NEG_Good_Token = NEG_Data[0].Contains("Vout-");

                // ------> Vdiff Index + CTout Index 
                string[] Vdiff_Data = csvLine[csvLine_Size - 21].Split(',');
                string[] CTout_Data = csvLine[csvLine_Size - 19].Split(',');

                bool Vdiff_CTout_Good_Token = Vdiff_Data[0].Contains("Vdiff") &&
                                             CTout_Data[0].Contains("CT out");


                // -----> Ecat Reading Result 
                string[] POS_Result = csvLine[csvLine_Size - 33].Split(',');
                string[] NEG_Result = csvLine[csvLine_Size - 32].Split(',');
                string[] VCTout = csvLine[csvLine_Size - 31].Split(',');

                bool EcatResult_Good_Token = POS_Result[0].Contains("Vout+") &&
                                              NEG_Result[0].Contains("Vout-") &&
                                              VCTout[0].Contains("Vbias");

                // Checking the index + assigned Index value 
                // Count from bottom to top because the lasted value is always stay in bottom 
                if (POS_Good_Token && NEG_Good_Token && Vdiff_CTout_Good_Token && EcatResult_Good_Token)
                {
                    // POS
                    EcatVoutPOS_Index = csvLine_Size - 25;
                    // NEG
                    EcatVoutNEG_Index = csvLine_Size - 22;
                    // Vdiff
                    EcatVdiff_Index = csvLine_Size - 21;
                    // CTout
                    EcatCTout_Index = csvLine_Size - 19;


                    EcatResultVoutPOS_Index = csvLine_Size - 8;
                    EcatResultVoutNEG_Index = csvLine_Size - 7;
                    EcatResultVdiff_Index = csvLine_Size - 6;
                    EcatResultCTout_Index = csvLine_Size - 5;

                    // SetPoint 
                    ClampSetPoint_Index = csvLine_Size - 3;
                    BiasSetPoint_Index = csvLine_Size - 2;

                    // Debug purpose
                     Console.WriteLine("Ecat Reading Row Index Innit CORRECTLY");

                }
                else
                {
                    Console.WriteLine("Ecat Reading Row Index Innit INCORRECTLY !!!");
                }
         }
        //=========================================================

        public void ConvertEcatReadingInto_Double()
        {
            // ------> Initalize the index 
            string[] csvLine = System.IO.File.ReadAllLines(_filePath);
            int csvLine_Size = csvLine.Length;

            // -------> Positive Index
            string[] POS_Data = csvLine[EcatVoutPOS_Index].Split(',');


            // ------> Negative Index
            string[] NEG_Data = csvLine[EcatVoutNEG_Index].Split(',');


            // ------> Vdiff Index + Bias Index 
            string[] Vdiff_Data = csvLine[EcatVdiff_Index].Split(',');
            string[] CTout_Data = csvLine[EcatCTout_Index].Split(',');

            // ----> SetPoint
            string[] Clamp = csvLine[ClampSetPoint_Index].Split(',');
            string[] Bias = csvLine[BiasSetPoint_Index].Split(',');

            // Trim the special Character '|'
            char[] charToTrim = { '|' };

            int counterPOS = 0;
            int counterNEG = 0;
            int counterBIAS = 0;
            int counterSetPoint = 0;

            for (int EcatValuePoint = 1; EcatValuePoint < Vdiff_Data.Length; EcatValuePoint++)
            {

                bool POS_NeedToTrim = POS_Data[EcatValuePoint].Contains('|');

                bool NEG_NeedToTrim = NEG_Data[EcatValuePoint].Contains('|');

                bool Vdiff_Bias_NeedToTrim = Vdiff_Data[EcatValuePoint].Contains('|') &&
                                             CTout_Data[EcatValuePoint].Contains('|');

                bool SetPointNeedToTrim = Clamp[EcatValuePoint].Contains('|') &&
                                          Bias[EcatValuePoint].Contains('|');
                // ----> Positve 
                if (POS_NeedToTrim)
                {
                    // Trim Special Char

                    POS_Data[EcatValuePoint] = POS_Data[EcatValuePoint].Trim(charToTrim);

                    // Convert to Double and save them 

                    EcatVoutPOS[counterPOS] = Convert.ToDouble(POS_Data[EcatValuePoint]);

                    // Round it 3 decimal digit 
                    EcatVoutPOS[counterPOS] = System.Math.Round(EcatVoutPOS[counterPOS], 3);

                    counterPOS++;
                }
                else
                {
                    // Convert to Double and save them 
                    EcatVoutPOS[counterPOS] = Convert.ToDouble(POS_Data[EcatValuePoint]);

                    // Round it 3 decimal digit 
                    EcatVoutPOS[counterPOS] = System.Math.Round(EcatVoutPOS[counterPOS], 3);
                    counterPOS++;
                }

                // ----> Negative
                if (NEG_NeedToTrim)
                {
                    // Trim Special Char
                    NEG_Data[EcatValuePoint] = NEG_Data[EcatValuePoint].Trim(charToTrim);

                    // Convert to Double and save them 
                    EcatVoutNEG[counterNEG] = Convert.ToDouble(NEG_Data[EcatValuePoint]);

                    // Round it 3 decimal digit 
                    EcatVoutNEG[counterNEG] = System.Math.Round(EcatVoutNEG[counterNEG], 3);

                    counterNEG++;
                }
                else
                {
                    // Convert to Double and save them 
                    EcatVoutNEG[counterNEG] = Convert.ToDouble(NEG_Data[EcatValuePoint]);

                    // Round it 3 decimal digit 
                    EcatVoutNEG[counterNEG] = System.Math.Round(EcatVoutNEG[counterNEG], 3);
                    counterNEG++;
                }

                // ----> Vdiff + CTout 
                if (Vdiff_Bias_NeedToTrim)
                {

                    // Trim Special Char
                    Vdiff_Data[EcatValuePoint] = Vdiff_Data[EcatValuePoint].Trim(charToTrim);
                    CTout_Data[EcatValuePoint] = CTout_Data[EcatValuePoint].Trim(charToTrim);

                    // Convert to Double and save them 
                    EcatVdiff[counterBIAS] = Convert.ToDouble(Vdiff_Data[EcatValuePoint]);
                    EcatCTout[counterBIAS] = Convert.ToDouble(CTout_Data[EcatValuePoint]);

                    // Round it 3 decimal digit 
                    EcatVdiff[counterBIAS] = System.Math.Round(EcatVdiff[counterBIAS], 3);
                    EcatCTout[counterBIAS] = System.Math.Round(EcatCTout[counterBIAS], 3);

                    counterBIAS++;
                }
                else
                {
                    // Convert to Double and save them 
                    EcatVdiff[counterBIAS] = Convert.ToDouble(Vdiff_Data[EcatValuePoint]);
                    EcatCTout[counterBIAS] = Convert.ToDouble(CTout_Data[EcatValuePoint]);

                    // Round it 3 decimal digit 
                    EcatVdiff[counterBIAS] = System.Math.Round(EcatVdiff[counterBIAS], 3);
                    EcatCTout[counterBIAS] = System.Math.Round(EcatCTout[counterBIAS], 3);
                    counterBIAS++;

                }

                // ----> SetPoint
                if (SetPointNeedToTrim)
                {
                    // Trim Special char 
                    Clamp[EcatValuePoint] = Clamp[EcatValuePoint].Trim(charToTrim);
                    Bias[EcatValuePoint] = Bias[EcatValuePoint].Trim(charToTrim);

                    // Convert to Double and Save
                    ClampSetPoint[counterSetPoint] = Convert.ToDouble(Clamp[EcatValuePoint]);
                    BiasSetPoint[counterSetPoint] = Convert.ToDouble(Bias[EcatValuePoint]);
                    counterSetPoint++;

                }
                else
                {
                    // Convert to Double and Save
                    ClampSetPoint[counterSetPoint] = Convert.ToDouble(Clamp[EcatValuePoint]);
                    BiasSetPoint[counterSetPoint] = Convert.ToDouble(Bias[EcatValuePoint]);
                    counterSetPoint++;
                }

                
            } // End FOR Loop
        }

        // Ecat Get result 
        public void GetEcatResult()
        {
            // ------> Initalize the index 
            string[] csvLine = System.IO.File.ReadAllLines(_filePath);
            int csvLine_Size = csvLine.Length;


            // -----> Ecat Reading Result 
            string[] POS_Result = csvLine[EcatResultVoutPOS_Index].Split(',');
            string[] NEG_Result = csvLine[EcatResultVoutNEG_Index].Split(',');
            string[] Vdiff_Result = csvLine[EcatResultVdiff_Index].Split(',');
            string[] CTout_Result = csvLine[EcatResultCTout_Index].Split(',');


            // Trim the special Character '|'
            char[] charToTrim = { '|' };


            int counterResult = 0;
            for (int EcatValuePoint = 1; EcatValuePoint < CTout_Result.Length; EcatValuePoint++)
            {
                bool ResultNeedToTrim = POS_Result[EcatValuePoint].Contains('|') &&
                                        NEG_Result[EcatValuePoint].Contains('|') &&
                                        Vdiff_Result[EcatValuePoint].Contains('|') &&
                                        CTout_Result[EcatValuePoint].Contains('|');


                if (ResultNeedToTrim)
                {
                    // Trim special char 
                    POS_Result[EcatValuePoint] = POS_Result[EcatValuePoint].Trim(charToTrim);
                    NEG_Result[EcatValuePoint] = NEG_Result[EcatValuePoint].Trim(charToTrim);
                    Vdiff_Result[EcatValuePoint] = Vdiff_Result[EcatValuePoint].Trim(charToTrim);
                    CTout_Result[EcatValuePoint] = CTout_Result[EcatValuePoint].Trim(charToTrim);

                    // Coppy array 
                    EcatResultVoutPOS[counterResult] = POS_Result[EcatValuePoint];
                    EcatResultVoutNEG[counterResult] = NEG_Result[EcatValuePoint];
                    EcatResultVdiff[counterResult] = Vdiff_Result[EcatValuePoint];
                    EcatResultCTout[counterResult] = CTout_Result[EcatValuePoint];
                }
                else
                {
                    // Coppy array 
                    EcatResultVoutPOS[counterResult] = POS_Result[EcatValuePoint];
                    EcatResultVoutNEG[counterResult] = NEG_Result[EcatValuePoint];
                    EcatResultVdiff[counterResult] = Vdiff_Result[EcatValuePoint];
                    EcatResultCTout[counterResult] = CTout_Result[EcatValuePoint];
                }

                counterResult++;
            }


        }// End Function 

        // Calculate Ecat Boundary through MeterReading 
        public void GetEcatBoundary()
        {
            MeterReading objMeter = new MeterReading(_filePath, _dirPath);
            objMeter.ConvertMeterReadingInto_Double();
            objMeter.GetMeterResult();

            for (int ecatValuePoint = 0; ecatValuePoint < EcatVdiff.Length; ecatValuePoint++)
            {
                // Positive Boundary 
                EcatVoutPOS_H[ecatValuePoint] = Math.Round(objMeter.MeterVoutPOS[ecatValuePoint] * 1.005,3);
                EcatVoutPOS_L[ecatValuePoint] = Math.Round(objMeter.MeterVoutPOS[ecatValuePoint] * 0.995, 3);

                // Negative Boundary 
                EcatVoutNEG_H[ecatValuePoint] = Math.Round(objMeter.MeterVoutNEG[ecatValuePoint] * 1.005, 3);
                EcatVoutNEG_L[ecatValuePoint] = Math.Round(objMeter.MeterVoutNEG[ecatValuePoint] * 0.995, 3);

                // Vdiff Boundary 
                EcatVdiff_H[ecatValuePoint] = Math.Round(objMeter.MeterVdiff[ecatValuePoint] * 1.005, 3);
                EcatVdiff_L[ecatValuePoint] = Math.Round(objMeter.MeterVdiff[ecatValuePoint] * 0.995, 3);

                // CT Boundary
                EcatCTout_H[ecatValuePoint] = Math.Round(objMeter.MeterVbias[ecatValuePoint] * 1.005, 3);
                EcatCTout_L[ecatValuePoint] = Math.Round(objMeter.MeterVbias[ecatValuePoint] * 0.995, 3);

                /*Console.WriteLine($"{EcatVoutPOS_H[ecatValuePoint]}\t{EcatVoutPOS_L[ecatValuePoint]}"+
                                  $"\t{ EcatVoutNEG_H[ecatValuePoint]}\t{EcatVoutNEG_L[ecatValuePoint]}" +
                                  $"\t{EcatVdiff_H[ecatValuePoint]}\t{EcatVdiff_L[ecatValuePoint]}" +
                                  $"\t{EcatCTout_H[ecatValuePoint]}\t{EcatCTout_L[ecatValuePoint]}");*/
            }

            
        }


    }



    public class MeterReading
    {
        // Store SetPoint 
        public Double[] ClampSetPoint = new Double[175];
        public Double[] BiasSetPoint = new Double[175];

        public int ClampSetPoint_Index;
        public int BiasSetPoint_Index;

        // Store the result 
        public string[] MeterResultVoutPOS = new string[175];
        public string[] MeterResultVoutNEG = new string[175];
        public string[] MeterResultVbias = new string[175];
        
        // Meter Result Index 
        public int MeterResultVoutPOS_Index;
        public int MeterResultVoutNEG_Index;
        public int MeterResultVbias_Index;


        // Store POS (size 175)
        public Double[] MeterVoutPOS_H = new Double[175];
        public Double[] MeterVoutPOS_L = new Double[175];
        public Double[] MeterVoutPOS = new Double[175];

        // Store NEG
        public Double[] MeterVoutNEG_H = new Double[175];
        public Double[] MeterVoutNEG_L = new Double[175];
        public Double[] MeterVoutNEG = new Double[175];

        // Store Vdiff and Bias 
        public Double[] MeterVdiff = new Double[175];
        public Double[] MeterVbias = new Double[175];


        // Positive Index
         public int MeterVoutPOS_Index;
         public int MeterVoutPOS_H_Index;
         public int MeterVoutPOS_L_Index;

        // Negative Index
        public int MeterVoutNEG_Index;
        public int MeterVoutNEG_H_Index;
        public int MeterVoutNEG_L_Index;

        // Vdiff + Bias Index
        public int MeterVdiff_Index;
        public int MeterCTout_Index;
         

        

        // input for the class
        string _filePath = @"";
        private string _dirPath = @"";

        // ---> Constructor 
        public MeterReading(string fileP, string dirP)
        {
            _filePath = fileP; // path to Main_FULL.csv
            _dirPath = dirP; // Path to serial Number foler 

            // ------> Initalize the index 
            string[] csvLine = System.IO.File.ReadAllLines(_filePath);
            int csvLine_Size = csvLine.Length;
            
            // -------> Positive Index
            string[] POS_H_Data = csvLine[csvLine_Size - 43 ].Split(',');
            string[] POS_L_Data = csvLine[csvLine_Size - 42].Split(',');
            string[] POS_Data = csvLine[csvLine_Size - 41].Split(',');
            
            bool POS_Good_Token = POS_H_Data[0].Contains("HV+ Meter High") &&
                                  POS_L_Data[0].Contains("HV+ Meter Low") &&
                                  POS_Data[0].Contains("Vout+");

            // ------> Negative Index
            string[] NEG_H_Data = csvLine[csvLine_Size - 40].Split(',');
            string[] NEG_L_Data = csvLine[csvLine_Size - 39].Split(',');
            string[] NEG_Data = csvLine[csvLine_Size - 38].Split(',');

            bool NEG_Good_Token = NEG_H_Data[0].Contains("HV- Meter High") &&
                                  NEG_L_Data[0].Contains("HV- Meter Low") &&
                                  NEG_Data[0].Contains("Vout-");

            // ------> Vdiff Index + Bias Index 
            string[] Vdiff_Data = csvLine[csvLine_Size - 37].Split(',');
            string[] Bias_Data = csvLine[csvLine_Size - 36].Split(',');

            bool Vdiff_Bias_Good_Token = Vdiff_Data[0].Contains("Vdiff_calc") &&
                                         Bias_Data[0].Contains("Vbias");


            // -----> Meter Reading Result 
            string[] POS_Result = csvLine[csvLine_Size - 33].Split(',');
            string[] NEG_Result = csvLine[csvLine_Size - 32].Split(',');
            string[] VBias = csvLine[csvLine_Size - 31].Split(',');

            bool MeterResult_Good_Token = POS_Result[0].Contains("Vout+") &&
                                          NEG_Result[0].Contains("Vout-") &&
                                          VBias[0].Contains("Vbias");

            // Checking the index + assigned Index value 
            // Count from bottom to top because the lasted value is always stay in bottom 
            if (POS_Good_Token && NEG_Good_Token && Vdiff_Bias_Good_Token && MeterResult_Good_Token)
            {
                MeterVoutPOS_H_Index = csvLine_Size - 43;
                MeterVoutPOS_L_Index = csvLine_Size - 42;
                MeterVoutPOS_Index = csvLine_Size - 41;

                MeterVoutNEG_H_Index = csvLine_Size - 40;
                MeterVoutNEG_L_Index = csvLine_Size - 39;
                MeterVoutNEG_Index = csvLine_Size - 38;

                MeterVdiff_Index = csvLine_Size - 37;
                MeterCTout_Index = csvLine_Size - 36;


                MeterResultVoutPOS_Index = csvLine_Size - 33;
                MeterResultVoutNEG_Index = csvLine_Size - 32;
                MeterResultVbias_Index = csvLine_Size - 31;

                // SetPoint 
                ClampSetPoint_Index = csvLine_Size - 3;
                BiasSetPoint_Index = csvLine_Size - 2;

                // Debug purpose
                // Console.WriteLine("Meter Reading Row Index Innit CORRECTLY");

            }
            else
            {
                Console.WriteLine("Meter Reading Row Index Innit INCORRECTLY !!!");
            }

            //=========================================================
            
           
        }


        // Convert Raw Data (string) ---> Double 
        public void ConvertMeterReadingInto_Double()
        {

            // ------> Initalize the index 
            string[] csvLine = System.IO.File.ReadAllLines(_filePath);
            int csvLine_Size = csvLine.Length;

            // -------> Positive Index
            string[] POS_H_Data = csvLine[MeterVoutPOS_H_Index].Split(',');
            string[] POS_L_Data = csvLine[MeterVoutPOS_L_Index].Split(',');
            string[] POS_Data = csvLine[MeterVoutPOS_Index].Split(',');


            // ------> Negative Index
            string[] NEG_H_Data = csvLine[MeterVoutNEG_H_Index].Split(',');
            string[] NEG_L_Data = csvLine[MeterVoutNEG_L_Index].Split(',');
            string[] NEG_Data = csvLine[MeterVoutNEG_Index].Split(',');


            // ------> Vdiff Index + Bias Index 
            string[] Vdiff_Data = csvLine[MeterVdiff_Index].Split(',');
            string[] Bias_Data = csvLine[MeterCTout_Index].Split(',');

            // ----> SetPoint
            string[] Clamp = csvLine[ClampSetPoint_Index].Split(',');
            string[] Bias = csvLine[BiasSetPoint_Index].Split(',');

            // Trim the special Character '|'
            char[] charToTrim = {'|'};

            int counterPOS = 0;
            int counterNEG = 0;
            int counterBIAS = 0;
            int counterSetPoint = 0;

            for (int MeterValuePoint = 1; MeterValuePoint < Vdiff_Data.Length; MeterValuePoint++)
            {
               
                bool POS_NeedToTrim = POS_H_Data[MeterValuePoint].Contains('|') &&
                                      POS_L_Data[MeterValuePoint].Contains('|') &&
                                      POS_Data[MeterValuePoint].Contains('|');

                bool NEG_NeedToTrim = NEG_H_Data[MeterValuePoint].Contains('|') &&
                                      NEG_L_Data[MeterValuePoint].Contains('|') &&
                                      NEG_Data[MeterValuePoint].Contains('|');

                bool Vdiff_Bias_NeedToTrim = Vdiff_Data[MeterValuePoint].Contains('|') &&
                                             Bias_Data[MeterValuePoint].Contains('|');
                
                bool SetPointNeedToTrim = Clamp[MeterValuePoint].Contains('|') &&
                                          Bias[MeterValuePoint].Contains('|');
                // ----> Positve 
                if (POS_NeedToTrim)
                {
                    // Trim Special Char
                    POS_H_Data[MeterValuePoint] = POS_H_Data[MeterValuePoint].Trim(charToTrim);
                    POS_L_Data[MeterValuePoint] = POS_L_Data[MeterValuePoint].Trim(charToTrim);
                    POS_Data[MeterValuePoint] = POS_Data[MeterValuePoint].Trim(charToTrim);

                    // Convert to Double and save them 
                    MeterVoutPOS_H[counterPOS] = Convert.ToDouble(POS_H_Data[MeterValuePoint]);
                    MeterVoutPOS_L[counterPOS] = Convert.ToDouble(POS_L_Data[MeterValuePoint]);
                    MeterVoutPOS[counterPOS] = Convert.ToDouble(POS_Data[MeterValuePoint]);

                    // Round it 3 decimal digit 
                    MeterVoutPOS_H[counterPOS] = System.Math.Round(MeterVoutPOS_H[counterPOS],3);
                    MeterVoutPOS_L[counterPOS] = System.Math.Round(MeterVoutPOS_L[counterPOS],3);
                    MeterVoutPOS[counterPOS] = System.Math.Round(MeterVoutPOS[counterPOS],3);

                    counterPOS++;
                }
                else
                {
                    // Convert to Double and save them 
                    MeterVoutPOS_H[counterPOS] = Convert.ToDouble(POS_H_Data[MeterValuePoint]);
                    MeterVoutPOS_L[counterPOS] = Convert.ToDouble(POS_L_Data[MeterValuePoint]);
                    MeterVoutPOS[counterPOS] = Convert.ToDouble(POS_Data[MeterValuePoint]);

                    // Round it 3 decimal digit 
                    MeterVoutPOS_H[counterPOS] = System.Math.Round(MeterVoutPOS_H[counterPOS], 3);
                    MeterVoutPOS_L[counterPOS] = System.Math.Round(MeterVoutPOS_L[counterPOS], 3);
                    MeterVoutPOS[counterPOS] = System.Math.Round(MeterVoutPOS[counterPOS], 3);
                    counterPOS++;
                }

                // ----> Negative
                if (NEG_NeedToTrim)
                {
                    // Trim Special Char
                    NEG_H_Data[MeterValuePoint] = NEG_H_Data[MeterValuePoint].Trim(charToTrim);
                    NEG_L_Data[MeterValuePoint] = NEG_L_Data[MeterValuePoint].Trim(charToTrim);
                    NEG_Data[MeterValuePoint] = NEG_Data[MeterValuePoint].Trim(charToTrim);

                    // Convert to Double and save them 
                    MeterVoutNEG_H[counterNEG] = Convert.ToDouble(NEG_H_Data[MeterValuePoint]);
                    MeterVoutNEG_L[counterNEG] = Convert.ToDouble(NEG_L_Data[MeterValuePoint]);
                    MeterVoutNEG[counterNEG] = Convert.ToDouble(NEG_Data[MeterValuePoint]);

                    // Round it 3 decimal digit 
                    MeterVoutNEG_H[counterNEG] = System.Math.Round(MeterVoutNEG_H[counterNEG], 3);
                    MeterVoutNEG_L[counterNEG] = System.Math.Round(MeterVoutNEG_L[counterNEG], 3);
                    MeterVoutNEG[counterNEG] = System.Math.Round(MeterVoutNEG[counterNEG], 3);

                    counterNEG++;
                }
                else
                {
                    // Convert to Double and save them 
                    MeterVoutNEG_H[counterNEG] = Convert.ToDouble(NEG_H_Data[MeterValuePoint]);
                    MeterVoutNEG_L[counterNEG] = Convert.ToDouble(NEG_L_Data[MeterValuePoint]);
                    MeterVoutNEG[counterNEG] = Convert.ToDouble(NEG_Data[MeterValuePoint]);

                    // Round it 3 decimal digit 
                    MeterVoutNEG_H[counterNEG] = System.Math.Round(MeterVoutNEG_H[counterNEG], 3);
                    MeterVoutNEG_L[counterNEG] = System.Math.Round(MeterVoutNEG_L[counterNEG], 3);
                    MeterVoutNEG[counterNEG] = System.Math.Round(MeterVoutNEG[counterNEG], 3);
                    counterNEG++;
                }

                // ----> Vdiff + CTout 
                if (Vdiff_Bias_NeedToTrim)
                {
                    
                    // Trim Special Char
                    Vdiff_Data[MeterValuePoint] = Vdiff_Data[MeterValuePoint].Trim(charToTrim);
                    Bias_Data[MeterValuePoint] = Bias_Data[MeterValuePoint].Trim(charToTrim);

                    // Convert to Double and save them 
                    MeterVdiff[counterBIAS] = Convert.ToDouble(Vdiff_Data[MeterValuePoint]);
                    MeterVbias[counterBIAS] = Convert.ToDouble(Bias_Data[MeterValuePoint]);

                    // Round it 3 decimal digit 
                    MeterVdiff[counterBIAS] = System.Math.Round(MeterVdiff[counterBIAS],3);
                    MeterVbias[counterBIAS] = System.Math.Round(MeterVbias[counterBIAS],3);

                    counterBIAS++; 
                }
                else
                {
                    // Convert to Double and save them 
                    MeterVdiff[counterBIAS] = Convert.ToDouble(Vdiff_Data[MeterValuePoint]);
                    MeterVbias[counterBIAS] = Convert.ToDouble(Bias_Data[MeterValuePoint]);

                    // Round it 3 decimal digit 
                    MeterVdiff[counterBIAS] = System.Math.Round(MeterVdiff[counterBIAS], 3);
                    MeterVbias[counterBIAS] = System.Math.Round(MeterVbias[counterBIAS], 3);
                    counterBIAS++;

                }

                // ----> SetPoint
                if (SetPointNeedToTrim)
                {
                    // Trim Special char 
                    Clamp[MeterValuePoint] = Clamp[MeterValuePoint].Trim(charToTrim);
                    Bias[MeterValuePoint] = Bias[MeterValuePoint].Trim(charToTrim);

                    // Convert to Double and Save
                    ClampSetPoint[counterSetPoint] = Convert.ToDouble(Clamp[MeterValuePoint]);
                    BiasSetPoint[counterSetPoint] = Convert.ToDouble(Bias[MeterValuePoint]);
                    counterSetPoint++;

                }
                else
                {
                    // Convert to Double and Save
                    ClampSetPoint[counterSetPoint] = Convert.ToDouble(Clamp[MeterValuePoint]);
                    BiasSetPoint[counterSetPoint] = Convert.ToDouble(Bias[MeterValuePoint]);
                    counterSetPoint++;
                }

            } // End FOR Loop

            
        }// End Function 






        // Get the pass or fail result through csv line 
        public void GetMeterResult()
        {
            // ------> Initalize the index 
            string[] csvLine = System.IO.File.ReadAllLines(_filePath);
            int csvLine_Size = csvLine.Length;


            // -----> Meter Reading Result 
            string[] POS_Result = csvLine[MeterResultVoutPOS_Index].Split(',');
            string[] NEG_Result = csvLine[MeterResultVoutNEG_Index].Split(',');
            string[] Vbias_Result = csvLine[MeterResultVbias_Index].Split(',');


            // Trim the special Character '|'
            char[] charToTrim = { '|' };
            

            int counterResult = 0;
            for (int MeterValuePoint = 1; MeterValuePoint < Vbias_Result.Length; MeterValuePoint++)
            {
                bool ResultNeedToTrim = POS_Result[MeterValuePoint].Contains('|') &&
                                        NEG_Result[MeterValuePoint].Contains('|') &&
                                        Vbias_Result[MeterValuePoint].Contains('|');


                if (ResultNeedToTrim)
                {
                    // Trim special char 
                    POS_Result[MeterValuePoint] = POS_Result[MeterValuePoint].Trim(charToTrim);
                    NEG_Result[MeterValuePoint] = NEG_Result[MeterValuePoint].Trim(charToTrim);
                    Vbias_Result[MeterValuePoint] = Vbias_Result[MeterValuePoint].Trim(charToTrim);

                    // Coppy array 
                    MeterResultVoutPOS[counterResult] = POS_Result[MeterValuePoint];
                    MeterResultVoutNEG[counterResult] = NEG_Result[MeterValuePoint];
                    MeterResultVbias[counterResult] = Vbias_Result[MeterValuePoint];
                }
                else
                {
                    // Coppy array 
                    MeterResultVoutPOS[counterResult] = POS_Result[MeterValuePoint];
                    MeterResultVoutNEG[counterResult] = NEG_Result[MeterValuePoint];
                    MeterResultVbias[counterResult] = Vbias_Result[MeterValuePoint];
                }

                counterResult++;
            }


        }// End Function 


    }
}
