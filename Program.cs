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
			Utils.Init(1);
			
			
			RunLogNormSimToT(1);
			
		}
	
		private static void RandDemo(double mean,double variance)
			// testing the rand generator
		{
			string FName = @"LogNormalDemo";
			string filename = FName+  ".txt";
			System.IO.StreamWriter SR = new StreamWriter(filename, false);
			double demorand;
			
			Utils.LognormalParameters LP = Utils.CommuteLognormalParameters(mean, variance);
			
			for (int i=0 ; i<10000;i++)
			{
				demorand=Utils.RandLogNormal( LP);
				SR.WriteLine("{0}",demorand);
				
			}
			SR.Close();
		}
		
//		private static void RunNormSimToN(int SimulationSize)
//		{
//			
//			Console.WriteLine("Init utils!");
//			Utils.Init(1);
//			
//			// Simulation Parameters
//			double NBact = 1e5 ;	// target number of cells
//			double tu  = 40;	// growth mean doubeling time
//			double Sigma;
//			int N0=1;	// Initial number of bacteria cells
//			for (int j=0; j<11; j++)
//			{
//				Sigma=0.1 + 0.1*j;
//			string FName = @"NormSimToN=" + NBact.ToString()+@"N0=" + N0.ToString() + @"tu=" + tu.ToString() + @"sigma=" + Sigma.ToString();
//			string filename = FName+  ".txt";
//			Console.Write(FName);
//			System.IO.StreamWriter SR = new StreamWriter(filename, false);
//
//			
//			//double maxTime = Math.Log(NBact)/Math.Log(2)*tu*2; //200% more than the mean
//			double maxTime = Math.Log(NBact/N0)/Math.Log(2)*30*20; //2000% more than the mean
//			double colTime = 0;
//			
//			for (int i=0; i<SimulationSize; i++)
//			{
//				Tube c = new Tube(maxTime,N0,tu,Sigma);
//				colTime= c.GrowtoSize(NBact);
//				SR.WriteLine("{0}",colTime);
//				PrintPresentege(i,SimulationSize);
//			}
//			SR.Close();
////				SRG.Close();
//			Console.WriteLine();
//			}								//new
//			Console.Write("Finished!");
//			
//		}
		
		private static void RunLogNormSimToT(int SimulationSize)
		{
			
			
		//	Console.WriteLine("Init utils!");
		//	Utils.Init(1);
			
			// Simulation Parameter
			double maxTime = 1e5 ;	
			
			TubeParameters TP = new TubeParameters(1e5,new StrainParameters[]{ new StrainParameters("Hip",1000,0.1,20,1000,21,3),new StrainParameters("WT",1000,0.001,20,1000,21,3)});
							
			string FName = @"T1";
			string filename = FName+  ".txt";
			System.IO.StreamWriter SR = new StreamWriter(filename, false);

			//for (int i=0; i<SimulationSize; i++)
			
				//TODO: seperate tube from simulation (tube as structure)
				Tube tube = new Tube(TP,maxTime);
				//tube.GrowtoSize(NBact);
				
				for(int s = 0;s<5;s++)
				{
				tube = SimulateTube.GrowToNmax(tube);
				tube =SimulateTube.Kill(tube,7);
				//tube = SimulateTube.GrowToNmax(tube);
				}
				
				
				
				double[,] N = tube.NBacteria;
				double[] t = tube.Time;
				
				for (int i=0; i<N.GetLength(0); i++)
				{
					SR.Write("{0}\t",t[i]);
					for (int j=0; j<N.GetLength(1); j++)	
					{
					SR.Write("{0}\t",N[i,j]);
					}
					SR.WriteLine();
					
				}
		
//				SR.WriteLine("{0}",colTime);
//				PrintPresentege(i,SimulationSize);

			SR.Close();

			
		//	Console.Write("Finished!");
			
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
