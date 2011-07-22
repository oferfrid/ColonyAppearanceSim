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
		static double Sigma;
		
		public static void Main(string[] args)
		{
			Utils.Init(1);
			
			Sigma = 0.322;
			//RunLogNormSimToN(10000);
			
			//RandDemo();
			
			//Sigma=8.364;
			//for (int k=13 ; k<21;k++)
			//{
			//	Sigma=k;
				RunNormSimToN(10000);
			//}
			//Console.Beep(1000,1000);
			//Console.ReadKey();
			
		}
	
		private static void RandDemo()
			// testing the rand generator
		{
			string FName = @"LogNormalDemo";
			string filename = FName+  ".txt";
			System.IO.StreamWriter SR = new StreamWriter(filename, false);
			double demorand;
			double mu;
			for (int i=0 ; i<100000;i++)
			{
				mu=3.28;
				demorand=Utils.RandLogNormal(mu,Sigma);
				SR.Write("{0}\t",demorand.ToString());
				
			}
			SR.Close();
		}
		
		private static void RunNormSimToN(int SimulationSize)
		{
			
			Console.WriteLine("Init utils!");
			Utils.Init(1);
			
			// Simulation Parameters
			double NBact = 1e5 ;	// target number of cells
			double tu  = 40;	// growth mean doubeling time
			int N0=1;	// Initial number of bacteria cells
			for (int j=0; j<11; j++)
			{
				Sigma=0.1 + 0.1*j;
			string FName = @"NormSimToN=" + NBact.ToString()+@"N0=" + N0.ToString() + @"tu=" + tu.ToString() + @"sigma=" + Sigma.ToString();
			string filename = FName+  ".txt";
			Console.Write(FName);
			System.IO.StreamWriter SR = new StreamWriter(filename, false);

			
			//double maxTime = Math.Log(NBact)/Math.Log(2)*tu*2; //200% more than the mean
			double maxTime = Math.Log(NBact/N0)/Math.Log(2)*30*20; //2000% more than the mean
			double colTime = 0;
			
			for (int i=0; i<SimulationSize; i++)
			{
				Colony c = new Colony(maxTime,N0,tu,Sigma);
				colTime= c.GrowtoSize(NBact);
				SR.WriteLine("{0}",colTime);
				PrintPresentege(i,SimulationSize);
			}
			SR.Close();
//				SRG.Close();
			Console.WriteLine();
			}								//new
			Console.Write("Finished!");
			
		}
		
		private static void RunLogNormSimToN(int SimulationSize)
		{
			
			Console.WriteLine("Init utils!");
			Utils.Init(1);
			
			// Simulation Parameter
			double NBact = 10000 ;	// target number of cells
			double mu  = 3.28;	// growth mean doubeling time
			int N0=1;	// Initial number of bacteria cells

			// for (int m=1; m<5; m++) {
			//	NBact =  100000*m;
			for (int j=0;j<10;j++)
			{
				mu=3.28+j*0.001;
				
				Sigma=Math.Sqrt(2*(Math.Log(28)-mu));
				
			string FName = @"LogNormSimToN=" + NBact.ToString()+@"N0=" + N0.ToString() + @"mu=" + mu.ToString() + @"sigma=" + Sigma.ToString();
			string filename = FName+  ".txt";
			System.IO.StreamWriter SR = new StreamWriter(filename, false);
			
			//double maxTime = Math.Log(NBact)/Math.Log(2)*tu*2; //200% more than the mean
			//double maxTime = Math.Log(1e6/N0)/Math.Log(2)*tu*20; //2000% more than the mean
			double maxTime = 10000*Math.Log(1e6/N0); //2000% more than the mean
			double colTime = 0;
			for (int i=0; i<SimulationSize; i++)
			{

				Colony c = new Colony(maxTime,N0,mu,Sigma);
				colTime= c.GrowtoSize(NBact);
				SR.WriteLine("{0}",colTime);
				PrintPresentege(i,SimulationSize);
			}
			SR.Close();
//				SRG.Close();
			Console.WriteLine();
			}
			Console.Write("Finished!");
			
		}
		
		private static void RunExpSimToN(int SimulationSize)
		{
			
			Console.WriteLine("Init utils!");
			Utils.Init(1);
			
			// Simulation Parameters
			double NBact = 1e5 ;	// target number of cells
			double tu  = 20;	// growth mean doubeling time
			int N0=10000;	// Initial number of bacteria cells

			// for (int m=1; m<5; m++) {
			//	NBact =  100000*m;
				string FName = @"ExpSimToN=" + NBact.ToString()+@"N0=" + N0.ToString() + @"tu=" + tu.ToString() ;
			string filename = FName+  ".txt";
			System.IO.StreamWriter SR = new StreamWriter(filename, false);

			
			//double maxTime = Math.Log(NBact)/Math.Log(2)*tu*2; //200% more than the mean
			double maxTime = 1000*Math.Log(NBact/N0)/Math.Log(2)*tu*20; //2000% more than the mean
			double colTime = 0;
			
			for (int i=0; i<SimulationSize; i++)
			{

				Colony c = new Colony(maxTime,N0,tu,Sigma);
				colTime= c.GrowtoSize(NBact);
				SR.WriteLine("{0}",colTime);
				PrintPresentege(i,SimulationSize);
			}
			SR.Close();
//				SRG.Close();
			Console.WriteLine();
//			}
			Console.Write("Finished!");
			
		}
		
		private static void RunExpSimTot(int SimulationSize)
		{
			Console.WriteLine("Init utils!");
			Utils.Init(1);
			
			// Simulation Parameters
			double Time = 250;
			double tu  = 40;	// growth std
			int N0=1;
			
			//string FName = @"BiNormalSimToT=" + Time.ToString() + "_20_2_100_2_0.2";
			//string FName = @"BiNormalSimToT=" + Time.ToString() + "_8_2_28_2_0.8";
			
			// creating a filename to save to
			string FName = @"ExpSimToT=" + Time.ToString()+ @"N0=" +N0.ToString();
			string filename = FName+  ".txt";
			System.IO.StreamWriter SR = new StreamWriter(filename, false);
			
			double[] GrowDivision;
			double N;
			
			
			for (int i=0; i<SimulationSize; i++)
			{
				Colony c = new Colony(Time+1,N0,tu,Sigma);
				c.GrowtoTime(Time);
				GrowDivision = c.GrowDivision;
				N=N0;
				for (int j=0;j<Math.Floor(Time/0.1)-1;j++)
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
			// Console.Beep(1500,500);
			
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
