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
		
		static int LagTimeres = 10;
		static int PersistersFractionsres = 5;
		
		static int Repetitions =1;
		
		
		static private System.Object lockTem = new System.Object();
		static private System.Object lockFile = new System.Object();

		
		static double[,] Fitness;
		
		static double Nmax = 1e7;
		
		static double[] LagTimeFromTo = {30,10*60};
		static double[] LagTimes ;
		
		static double[] PersistersFractionFromTo = {0,0.5};
		static double[] PersistersFractions ;
		
		
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
			
			
			//delete the file
			simResultsFilename = "Fitness";
			FileInfo FI= new FileInfo(simResultsFilename);
			FI.Delete();
			
			//init global vars
			LagTimes = new double[LagTimeres];
			
			for(int i=0;i<LagTimes.Length;i++)
			{
				LagTimes[i] = LagTimeFromTo[0] + (LagTimeFromTo[1] - LagTimeFromTo[0])*i/(LagTimes.Length-1);
			}
			
			
			
			PersistersFractions = new double[PersistersFractionsres];
			
			for(int i=0;i<PersistersFractions.Length;i++)
			{
				PersistersFractions[i] = PersistersFractionFromTo[0] + (PersistersFractionFromTo[1] - PersistersFractionFromTo[0])*i/(PersistersFractions.Length-1);
			}
			
			
			Fitness = new double[LagTimes.Length,PersistersFractionsres];
			
			
			

			//RunOneSimulation((object) new SimulationParameters(50,50,0));
			
				Run4Metrix();
			
		}
		
		
		private static  void Run4Metrix()
		{
			
			DateTime start = DateTime.Now;
			RunSimParalel();
			//RunSim();
			for(int r=0;r<Repetitions;r++)
			{
				Print2DMat2File(simResultsFilename + "Nmax=" + Nmax.ToString("E1"),Fitness,LagTimes);
			}
			Console.Beep(800,1000);
			Console.Beep(800,1000);
//
			
			TimeSpan TS = DateTime.Now - start;
			Console.WriteLine("Ended in {0} minuts",TS.TotalMinutes);
			Console.WriteLine();
			
			
			
		}
		
		private static  void RunSimParalel()
		{
			int sid=50;
			
			for(int r=0;r<Repetitions;r++)
			{
				
				for(int lagi=0;lagi<LagTimes.Length;lagi++)
				{
					for(int PersistersFractioni=0;PersistersFractioni<PersistersFractions.Length;PersistersFractioni++)
					{
						
						Interlocked.Increment(ref numerOfThreadsNotYetCompleted);
						ThreadPool.QueueUserWorkItem(new WaitCallback(RunOneSimulation),(object)new SimulationParameters(lagi,PersistersFractioni,sid++,r));
					}
				}
				
				
			}
			_doneEvent.WaitOne();
			
		}
		
		
		
		private static void RunOneSimulation(object o)
		{
			try
			{
				SimulationParameters PS = (SimulationParameters)o;
				
				int lagi = PS.lagi;
				int PersistersFractioni = PS.PersistersFractioni;
				int sid = PS.sid;
				int r = PS.r;
				
				double lag = LagTimes[lagi];
				
				double PersistersFraction = PersistersFractions[PersistersFractioni];
				
				TubeParameters TP = new TubeParameters(Nmax,new StrainParameters[]{
				                                       	new StrainParameters("WT",Nmax/2,0,30,0,30,0,20,3),
				                                       	new StrainParameters("Evolved",Nmax/2,PersistersFraction,0.5,lag,0.5,lag,20,3)
				                                       });
				
				
				Tube tube = new Tube(TP,maxTime);
				SimulateTube SimulateTube = new SimulateTube(PS.sid);
				
				tube = SimulateTube.Kill(tube,5*60);
				tube = SimulateTube.GrowToNmax(tube);
				
				
				double f = tube.LastN[1]/tube.LastN[0];
				//TODO:add r
				Fitness[lagi,PersistersFractioni] =f;
				
				if(DebugPrint)
				{
					PrintTube2File(string.Format("Lag={0:0.0}_PersistersFraction={1:0.0}_R={2}",lag,PersistersFraction,r),tube);
				}
				
				SimulateTube = null;
				
			}
			finally
			{
				
				
				PrintPresentege(Fitness.Length - numerOfThreadsNotYetCompleted, Fitness.Length);
				
				if (Interlocked.Decrement(ref numerOfThreadsNotYetCompleted) == 0)
				{
					_doneEvent.Set();
				}
			}
		}
		
		private struct SimulationParameters
		{
			public int lagi;
			public int PersistersFractioni;
			public int sid;
			public int r;
			public SimulationParameters( int lagi, int PersistersFractioni,int sid,int r)
			{
				this.lagi = lagi;
				this.PersistersFractioni = PersistersFractioni;
				this.sid = sid;
				this.r = r;
			}
		}
		
		
		
		#region Print2file
		private static void Print2DMat2File(string Filename,double[,] Mat,double[] XHeader)
		{
			double[] YHeader = new double[Mat.GetLength(1)];
			for(int i=0;i<	YHeader.Length;i++)
			{
				YHeader[i] = i+1;
			}
			
			Print2DMat2File(Filename,Mat,XHeader,YHeader);
			
		}
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
