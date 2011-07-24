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
using System.Threading;

namespace IritSimulation
{
	class Program
	{
		
		
		
		private static int SimulationSize ;
		public static int _numerOfThreadsNotYetCompleted ;
		private static ManualResetEvent _doneEvent = new ManualResetEvent(false);
		
		
		
		
		public static void Main(string[] args)
		{
			Utils.Init(1);
			TimeSpan tdiff;
			DateTime Stime;
			
			
			SimulationSize = 20;
			
			Stime = DateTime.Now;
			RunSimSerial(SimulationSize);
		    tdiff =(DateTime.Now-Stime);
		    Console.WriteLine();
			Console.WriteLine("finised Sirial in {0} seconds ",tdiff.TotalSeconds );
			
			Stime = DateTime.Now;
			RunSimParalel(SimulationSize);
			 tdiff =(DateTime.Now-Stime);
			 Console.WriteLine();
			Console.WriteLine("finised paralel in {0} seconds",tdiff.TotalSeconds );
			Console.WriteLine("All done.");
			Console.ReadKey(false);
			
		}
		
		
		private static void RunSimParalel(int SimulationSize)
		{
			double maxTime = 1e5 ;
			 _numerOfThreadsNotYetCompleted = SimulationSize;
			ManualResetEvent doneEvent = new ManualResetEvent(false);
			TubeWrapper[] TubeArray = new TubeWrapper[SimulationSize];
			
			for (int i = 0; i < SimulationSize; i++)
			{
				//doneEvents[i] = new ManualResetEvent(false);

				TubeParameters TP = new TubeParameters(1e6,new StrainParameters[]{ new StrainParameters("Hip",100,0.1,20,1000,21,3),new StrainParameters("WT",100,0.001,20,1000,21,3)});
				
				TubeWrapper T = new TubeWrapper(TP,maxTime,doneEvent);
				TubeArray[i] = T;
				ThreadPool.QueueUserWorkItem(T.ThreadPoolCallback, i);
			}
		
			
			
			
		}
		
		private static void RunOneSim(object o)
		{
			double maxTime = 1e5 ;
			try
			{
				int sim = (int)o;
				TubeParameters TP = new TubeParameters(1e6,new StrainParameters[]{ new StrainParameters("Hip",100,0.1,20,1000,21,3),new StrainParameters("WT",100,0.001,20,1000,21,3)});
				
				string FName = @"ParalelSim" + sim.ToString();
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
				SR.Close();
				
				
			}
			finally
			{
				int i = Interlocked.Decrement(ref _numerOfThreadsNotYetCompleted);
				PrintPresentege(SimulationSize - _numerOfThreadsNotYetCompleted,SimulationSize);
				if (i == 0)
				{
					_doneEvent.Set();
				}
			}
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
		

		
		private static void RunSimSerial(int SimulationSize)
		{
			
			
			//Console.WriteLine("Start");
			//	Utils.Init(1);
			
			// Simulation Parameter
			double maxTime = 1e5 ;
			
			for(int sim=0;sim<SimulationSize;sim++)
			{
				
				TubeParameters TP = new TubeParameters(1e6,new StrainParameters[]{ new StrainParameters("Hip",100,0.1,20,1000,21,3),new StrainParameters("WT",100,0.001,20,1000,21,3)});
				
				string FName = @"SirialSim" + sim.ToString();
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
				SR.Close();
				
				PrintPresentege(sim,SimulationSize);
			}
			

			
			//Console.Write("Finished!");
			
		}
		
		

		
		#region Display Presentege

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
		#endregion
		
	}
}
