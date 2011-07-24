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
			
			
			RunSim(1);
			
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
		

		
		private static void RunSim(int SimulationSize)
		{
			
			
			Console.WriteLine("Start");
		//	Utils.Init(1);
			
			// Simulation Parameter
			double maxTime = 1e5 ;	
			
			TubeParameters TP = new TubeParameters(1e6,new StrainParameters[]{ new StrainParameters("Hip",100,0.1,20,1000,21,3),new StrainParameters("WT",100,0.001,20,1000,21,3)});
							
			string FName = @"T0.1";
			string filename = FName+  ".txt";
			System.IO.StreamWriter SR = new StreamWriter(filename, false);

			
				Tube tube = new Tube(TP,maxTime);
		
				tube = SimulateTube.GrowToNmax(tube);
								
				
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

			
			Console.Write("Finished!");
			
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
