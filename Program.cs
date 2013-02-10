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
using System.Security;
using System.Threading;

namespace IritSimulation
{
	class Program
	{
		static bool DebugPrint;
		
		
		static double maxTime = 1e5 ;
		
		
		//static int DoublingTimeres = 100;
		//static int LagTimeres = 100;
		
		
		static int Repetitions =1;
		
		
		static private System.Object lockTem = new System.Object();
		static private System.Object lockFile = new System.Object();

		
		//static double[][,] Fitness;
		
		static double Nmax = 1e9;
		
		//static double[] DoublingTimeFromTo = {20,60};
		//static double[] LagTimeFromTo = {30,10*60};
		
		//static double[] DoublingTimes;
		//static double[] LagTimes ;
		
		private static ManualResetEvent _doneEvent = new ManualResetEvent(false);

		static int numerOfThreadsNotYetCompleted;
		
		
		static string simResultsFilename;
		
		public static void Main(string[] args)
		{
			
			//read cmd params
			if(args.Length>0)
			{
				DebugPrint =  System.Convert.ToBoolean(args[0]);
			}
			
			
//			//delete the file
//			simResultsFilename = "Fitness";
//			FileInfo FI= new FileInfo(simResultsFilename);
//			FI.Delete();
//			
//			//init global vars
//			DoublingTimes = new double[DoublingTimeres];
//			LagTimes = new double[LagTimeres];
//			
//			
//			for(int i=0;i<DoublingTimes.Length;i++)
//			{
//				DoublingTimes[i] = DoublingTimeFromTo[0] + (DoublingTimeFromTo[1] - DoublingTimeFromTo[0])*i/(DoublingTimes.Length-1);
//			}
//			
//			for(int i=0;i<LagTimes.Length;i++)
//			{
//				LagTimes[i] = LagTimeFromTo[0] + (LagTimeFromTo[1] - LagTimeFromTo[0])*i/(LagTimes.Length-1);
//			}
//			
//			Fitness = new double[Repetitions][,];
//			for(int r=0;r<Repetitions;r++)
//			{
//			Fitness[r] = new double[DoublingTimeres,LagTimeres];
//			}
//			

			
			
			Run4Metrix();
			
		}
		
		
		private static  void Run4Metrix()
		{
			
			
			DateTime start = DateTime.Now;
			RunSimParalel();
			//RunSim();
//			for(int r=0;r<Repetitions;r++)
//			{
//				Print2DMat2File(simResultsFilename + r.ToString("000"),Fitness[r],DoublingTimes,LagTimes);
//			}
			                Console.Beep(800,1000);
			                Console.Beep(800,1000);
//
			                
			                TimeSpan TS = DateTime.Now - start;
			                Console.WriteLine("Ended in {0}",TS);
			                Console.WriteLine();
			                
			                
			                
			}
		
		private static  void RunSimParalel()
		{
			int sid=50;
			
			
			for(int r=0;r<Repetitions;r++)
			{
//				for(int doubi=0;doubi<DoublingTimes.Length;doubi++)
//				{
//					for(int lagi=0;lagi<LagTimes.Length;lagi++)
//					{
//						
						Interlocked.Increment(ref numerOfThreadsNotYetCompleted);
						ThreadPool.QueueUserWorkItem(new WaitCallback(RunOneSimulation),(object)new SimulationParameters(sid++,r));
//					}
//					
//				}
			}
			_doneEvent.WaitOne();
			
		}
		
		
		
		private static void RunOneSimulation(object o)
		{
			try
			{
				SimulationParameters PS = (SimulationParameters)o;
				
					int r = PS.r;
				
				//double doub = DoublingTimes[doubi];
				//double lag = LagTimes[lagi];
				double lag = 5.0*60;
				double doub = 20;
				
				TubeParameters TP = new TubeParameters(Nmax,new StrainParameters[]{
				                                       	//new StrainParameters("WT",Nmax,0,30,0,30,0,21,3),
				                                       	new StrainParameters("Evolved",Nmax,0,lag,1000,lag,0,doub,3)
				                                       });
				
				
				Tube tube = new Tube(TP,maxTime);
				SimulateTube SimulateTube = new SimulateTube(PS.sid);
				
				tube = SimulateTube.Kill(tube,10*60);
				//tube = SimulateTube.GrowToNmax(tube);
				
				
				//double f = tube.LastN[1]/tube.LastN[0];
				//Fitness[r][doubi,lagi] =f;
				
				if(DebugPrint)
				{
					PrintTube2File(string.Format("r={0:00}",r),tube);
				}
				
				SimulateTube = null;
				
			}
			finally
			{
				
				
				PrintPresentege(Repetitions - numerOfThreadsNotYetCompleted, Repetitions);
				
				if (Interlocked.Decrement(ref numerOfThreadsNotYetCompleted) == 0)
				{
					_doneEvent.Set();
				}
			}
		}
		
		private struct SimulationParameters
		{
			
			
			public int sid;
			public int r;
			public SimulationParameters(int sid,int r)
			{
				this.r = r;
				this.sid = sid;
			}
		}
		
		
		
		#region Print2file
		private static void Print2DMat2File(string Filename,double[,] Mat,double[] XHeader,double[] YHeader)
		{
			
			string DFilename =  Filename + ".txt";
			string HXFilename =  Filename + "_HX.txt";
			string HYFilename =  Filename + "_HY.txt";
			
			System.IO.StreamWriter DSR = new StreamWriter(DFilename, false);
			
			System.IO.StreamWriter HXSR = new StreamWriter(HXFilename, false);
			System.IO.StreamWriter HYSR = new StreamWriter(HYFilename, false);

			for (int i=0; i<Mat.GetLength(0); i++)
			{
				HXSR.WriteLine(XHeader[i]);
			}
			for (int j=0; j<Mat.GetLength(1); j++)
			{
				HYSR.WriteLine(YHeader[j]);
				
			}
			for (int i=0; i<Mat.GetLength(0); i++)
			{
				for (int j=0; j<Mat.GetLength(1); j++)
				{
					DSR.Write("{0}\t",Mat[i,j]);
				}
				DSR.WriteLine();
			}
			
			DSR.Close();
			HXSR.Close();
			HYSR.Close();
		}

		
		private static void PrintTube2File(string Filename,Tube T)
		{
			
			Filename+=  ".txt";
			System.IO.StreamWriter SR = new StreamWriter(Filename, false);

			double[,] N = T.NBacteria;
			double[] t = T.Time;
			
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
		
		
//		private static void PrintMutFix2File(string Filename,int mi,int rep)
//
//		{
//
//			string DFilename =  Filename + ".txt";
//
//
//
//			lock (lockFile)
//			{
//				System.IO.StreamWriter DSR = new StreamWriter(DFilename,true);
//				DSR.WriteLine("{0}\t{1}\t{2}\t{3}\t",mi,rep,Cycle2Mutant[mi,rep],Cycle2Fixsation[mi,rep]);
//				DSR.Close();
//			}
//
//
//		}
//
//
//
		#endregion

		#region PrintPresentege
		public static void PrintPresentege(int ind,int from)
		{
			//Console.WriteLine(Convert.ToInt32((double)ind/from*1000));
			int fraction = Convert.ToInt32((double)ind/from*1000);
			int postfraction = Convert.ToInt32((double)(ind-1)/from*1000);
			if ((fraction-postfraction)>=1)
			{
				lock (lockTem)
				{
					Console.CursorLeft = 0;
					
					
					Console.Write("{0}% -{1} ",((double)fraction/10).ToString("00.0"),numerOfThreadsNotYetCompleted);
				}
			}
		}
		
		public static void PrintPresentege(double ind,double from)
		{
			//Console.WriteLine(Convert.ToInt32((double)ind/from*1000));
			int fraction = Convert.ToInt32((double)ind/from*1000);
			int postfraction = Convert.ToInt32((double)(ind-1)/from*1000);
			
			if ((fraction-postfraction)>=1)
			{
				lock (lockTem)
				{
					Console.CursorLeft = 0;
					
					Console.Write("{0}%  ",((double)fraction/10).ToString("00.0"));
				}
			}
		}
		
		#endregion
	}
}
