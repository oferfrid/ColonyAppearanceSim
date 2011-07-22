/*
 * Created by SharpDevelop.
 * User: oferfrid
 * Date: 02/08/2009
 * Time: 11:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;

namespace IritSimulation
{
	class Program
	{
		public static void Main(string[] args)
		{
//		Utils.Init(1);
//			for (int i=0 ; i<10000;i++)
//			{
//				Console.WriteLine(Utils.RandBiNormal(20,2,40,2,0.2));
//			}
			
			//RunExpSimTot(10000);
			
			RunExpSimToN(10000);
		}
		
		
		private static void RunExpSimToN(int SimulationSize)
		{
			
			Console.WriteLine("Init utils!");
			Utils.Init(1);
			
			// Simulation Parameters
			double NBact = 1e4 ;
			double gSig  = 20;	// growth std
			int N0=20;


			string FName = @"ExpSimToN=" + NBact.ToString()+@"N0=" + N0.ToString()  ;
			
			
			
			string filename = FName+  ".txt";
			System.IO.StreamWriter SR = new StreamWriter(filename, false);

			
			double maxTime = NBact/Math.Log(2)*gSig*2; //10% more then the mean
			
			
			for (int i=0; i<SimulationSize; i++)
			{

				Colony c = new Colony(maxTime,N0,gSig);
				SR.WriteLine("{0}",c.GrowtoSize(NBact));
				PrintPresentege(i,SimulationSize);
			}
			SR.Close();
//				SRG.Close();
			Console.WriteLine();
//			}
			Console.Write("Finished!");
			Console.Beep(1500,500);
			
		}
		
		private static void RunExpSimTot(int SimulationSize)
		{
			Console.WriteLine("Init utils!");
			Utils.Init(1);
			
			// Simulation Parameters
			double Time = 150;
			double gSig  = 20;	// growth std
			int N0=1;
			
			//string FName = @"BiNormalSimToT=" + Time.ToString() + "_20_2_100_2_0.2";
			//string FName = @"BiNormalSimToT=" + Time.ToString() + "_8_2_28_2_0.8";
			string FName = @"ExpSimToT=" + Time.ToString()+ @"N0=" +N0.ToString();
			
			string filename = FName+  ".txt";
			System.IO.StreamWriter SR = new StreamWriter(filename, false);
			double[] GrowDivision;
			double N;
			
			
			for (int i=0; i<SimulationSize; i++)
			{
				Colony c = new Colony(Time*4,N0,gSig);
				c.GrowtoTime(Time);
				GrowDivision = c.GrowDivision;
				N=N0;
				for (int j=0;j<Math.Floor(Time/0.1);j++)
				{
					N = N+GrowDivision[j];
					SR.Write("{0}\t",N.ToString());
				}
				SR.WriteLine();
				PrintPresentege(i,SimulationSize);
			}

			SR.Close();
//				SRG.Close();
			Console.WriteLine();
//			}
			Console.Write("Finished!");
			Console.Beep(1500,500);
			
		}
		
		

		public static void PrintPresentege(int ind,int from)
		{
			//Console.WriteLine(Convert.ToInt32((double)ind/from*1000));
			int fraction = Convert.ToInt32((double)ind/from*1000);
			int postfraction = Convert.ToInt32((double)(ind-1)/from*1000);
			if ((fraction-postfraction)>=1)
			{
				Console.CursorLeft = 0;
				Console.Write("{0}%",((double)fraction/10).ToString("00.0"));
			}
		}
		
		public static void PrintPresentege(double ind,double from)
		{
			//Console.WriteLine(Convert.ToInt32((double)ind/from*1000));
			int fraction = Convert.ToInt32((double)ind/from*1000);
			int postfraction = Convert.ToInt32((double)(ind-1)/from*1000);
			
			Console.CursorLeft = 0;
			Console.Write("{0}%",((double)fraction/10).ToString("00.0"));
			
		}
	}
}
