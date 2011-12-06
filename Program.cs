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
		
		
		static double maxTime = 1e6 ;
		
		
		static int res = 20;
		static int Repetitions =40;
		static int maxsycles = 1000;
		
		
		static private System.Object lockTem = new System.Object();
		static private System.Object lockFile = new System.Object();

		
		static double[] MutationRates;
		static double[,] Cycle2Fixsation;
		static double[,] Cycle2Mutant;
		
		static double LagTS ;
		
		
		static double Nmax = 1e7;
		
		static double[] MutationRateFromTo = {7,9}; //10^-(XX)
		
		private static ManualResetEvent _doneEvent = new ManualResetEvent(false);

		static int numerOfThreadsNotYetCompleted;
		
		
		static string simResultsFilename;
		
		public static void Main(string[] args)
		{
			
			//read cmd params
			if(args.Length>1)
			{
				DebugPrint =  System.Convert.ToBoolean(args[1]);
			}
			
			LagTS = System.Convert.ToDouble(args[0]);
			
			
			//delete the file
			simResultsFilename = "EvoLag" + LagTS + "__Cycle2MutantCycle2Fixsation";
			FileInfo FI= new FileInfo(simResultsFilename);
			FI.Delete();
			
			Run4Metrix();
			
		}
		
		
		private static  void Run4Metrix()
		{
			//init global vars
			MutationRates =new double[res];
			
			
			for(int i=0;i<MutationRates.Length;i++)
			{
				MutationRates[i] = Math.Pow(10.0,-(MutationRateFromTo[0] + (MutationRateFromTo[1] - MutationRateFromTo[0])*i/(MutationRates.Length-1)));
			}
			
			
			
			Cycle2Fixsation = new double[MutationRates.Length,Repetitions];
			Cycle2Mutant  = new double[MutationRates.Length,Repetitions];
			
			DateTime start = DateTime.Now;
			RunSimParalel();
			//RunSim();
			
			Print2DMat2File("EvoLag" + LagTS + "_Cycle2Fixsation",Cycle2Fixsation);
			Print2DMat2File("EvoLag" + LagTS + "_Cycle2Mutant",Cycle2Mutant);
			
			//Print2DMatH2File("EvoLag20Seed=" + Seed.ToString() + "Mat_H",Cycle2Fixsation,KillTime,Dilution);
			Console.Beep(800,1000);
			Console.Beep(800,1000);
			
			
			TimeSpan TS = DateTime.Now - start;
			Console.WriteLine("Ended in {0} minuts",TS.TotalMinutes);
			Console.WriteLine();
			
			
			
		}
		
		private static  void RunSimParalel()
		{
			int sid=50;
			
			
			for(int r=0;r<Repetitions;r++)
			{
				for(int mi=0;mi<MutationRates.Length;mi++)
				{
					Cycle2Fixsation[mi,r] = 0;
					Cycle2Mutant[mi,r] = 0;
					
					
					Interlocked.Increment(ref numerOfThreadsNotYetCompleted);
					ThreadPool.QueueUserWorkItem(new WaitCallback(RunOneSimulation),(object)new SimulationParameters(mi,r,sid++));
					
					
				}
				
			}
			_doneEvent.WaitOne();
			
		}
		
		
		
		private static void RunOneSimulation(object o)
		{
			try
			{
				SimulationParameters PS = (SimulationParameters)o;
				
				int rep = PS.rep;
				int mi = PS.mi;
				int sid = PS.sid;
				
				double MutationRate = MutationRates[mi];
				
				TubeParameters TP = new TubeParameters(Nmax,new StrainParameters[]{
				                                       	new StrainParameters("WT",1e4,0,LagTS,1000,LagTS,1000,21,3,new StrainMutationParameters[]{new StrainMutationParameters(1,MutationRate,0)}),
				                                       	new StrainParameters("ResistanceMutant",0,0,LagTS,1000,0,0,21,3)
				                                       });
				
				
				Tube tube = new Tube(TP,maxTime);
				SimulateTube SimulateTube = new SimulateTube(PS.sid);
				
				tube = SimulateTube.GrowToNmax(tube);
				int s;
				for(s=0;s<maxsycles;s++)
				{
					tube = SimulateTube.Kill(tube,240);
					tube = SimulateTube.GrowToNmax(tube);
					
					//arize of mutant
					if(tube.LastN[1]>0)
					{
						Cycle2Mutant[mi,rep]=s;
					}
					
					//test 4 fixsasion or extiction.
					if(((double)tube.LastN[1]/(tube.LastN[0]+tube.LastN[1])>0.7) || double.IsNaN((double)tube.LastN[1]/(tube.LastN[0]+tube.LastN[1])))
					{
						break;
					}
					
				}
				
				if(double.IsNaN((double)tube.LastN[1]/(tube.LastN[0]+tube.LastN[1])))
				{
					Cycle2Fixsation[mi,rep] = 0;
				}
				else
				{
					if(((double)tube.LastN[1]/(tube.LastN[0]+tube.LastN[1])>0.7))
					{
						Cycle2Fixsation[mi,rep] = s;
					}
				}
				
				if(DebugPrint)
				{
					PrintTube2File("Lag=" + LagTS.ToString() + "Repetition=" + rep.ToString() + "MutationRate=" + MutationRate.ToString("e") ,tube);
				}
				
				SimulateTube = null;
				
				PrintMutFix2File( simResultsFilename, mi, rep);
			}
			finally
			{
				
				//Console.WriteLine(numerOfThreadsNotYetCompleted);
				PrintPresentege(MutationRates.Length*Repetitions-numerOfThreadsNotYetCompleted ,MutationRates.Length*Repetitions);
				if (Interlocked.Decrement(ref numerOfThreadsNotYetCompleted) == 0)
				{
					_doneEvent.Set();
				}
			}
		}
		
		private struct SimulationParameters
		{
			public int mi;
			public int rep;
			public int sid;
			public SimulationParameters( int mi, int rep,int sid)
			{
				this.mi = mi;
				this.rep = rep;
				this.sid = sid;
			}
		}
		
		
		
		#region Print2file
		private static void Print2DMat2File(string Filename,double[,] Mat)
		{
			
			string DFilename =  Filename + ".txt";
			string HFilename =  Filename + "_H.txt";
			
			System.IO.StreamWriter DSR = new StreamWriter(DFilename, false);
			System.IO.StreamWriter HSR = new StreamWriter(HFilename, false);

			
			for (int i=0; i<Mat.GetLength(0); i++)
			{
				for (int j=0; j<Mat.GetLength(1); j++)
				{
					DSR.Write("{0}\t",Mat[i,j]);
				}
				DSR.WriteLine();
				HSR.WriteLine(MutationRates[i]);
			}
			
			DSR.Close();
			HSR.Close();
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
		
		
		private static void PrintMutFix2File(string Filename,int mi,int rep)

		{
			
			string DFilename =  Filename + ".txt";
			
			
			
			lock (lockFile)
			{
				System.IO.StreamWriter DSR = new StreamWriter(DFilename,true);
				DSR.WriteLine("{0}\t{1}\t{2}\t{3}\t",mi,rep,Cycle2Mutant[mi,rep],Cycle2Fixsation[mi,rep]);
				DSR.Close();
			}
			
			
		}

		

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
