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
		
		
		
		static TubeParameters TP;
		
		
		static int res = 20;
		static int Repetitions =100;
		static int maxsycles = 200;
		
		static double[] KillTime;
		static double[,] Cycle2Fixsation;
		static double[,] Cycle2Mutant;
		
		static double LagTS ;
		
		
		static int numerOfThreadsNotYetCompleted = 0;
		private static ManualResetEvent _doneEvent = new ManualResetEvent(false);
		
		public static void Main(string[] args)
		{	
			
			//read cmd params
			if(args.Length>1)
			{
				DebugPrint =  System.Convert.ToBoolean(args[1]);
			}
			
			  LagTS = System.Convert.ToDouble(args[0]);
			
			
			TP = new TubeParameters(1e6,new StrainParameters[]{
		                                              	new StrainParameters("WT",1e4,0,LagTS,1000,LagTS,1000,21,3,new StrainMutationParameters[]{new StrainMutationParameters(1,1e-7,0)}),
		                                              	new StrainParameters("ResistanceMutant",0,0,200,1000,0,0,21,3)
		                                              });
			
			Run4Metrix();
			
		}
			private static  void RunForOne()
		{
			Cycle2Fixsation = new double[1,1];
			KillTime = new double[] {200};
			RunOneSimulation((object)new SimulationParameters(0,0));
		}
		
		
		private static  void Run4Metrix()
		{
			//init global vars
			KillTime =new double[res];
		
			
			double[] KillFromTo = {20,300};

			
			
			for(int i=0;i<KillTime.Length;i++)
			{
				KillTime[i] = KillFromTo[0] + (KillFromTo[1] - KillFromTo[0])*i/(KillTime.Length-1);
			}
			
			
			Cycle2Fixsation = new double[KillTime.Length,Repetitions];
			Cycle2Mutant  = new double[KillTime.Length,Repetitions];
			
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
			
			for(int ki=0;ki<KillTime.Length;ki++)
			{
				for(int r=0;r<Repetitions;r++)
				{
					Cycle2Fixsation[ki,r] = 0;
					Cycle2Mutant[ki,r] = 0;
					
					
						Interlocked.Increment(ref numerOfThreadsNotYetCompleted);
						ThreadPool.QueueUserWorkItem(new WaitCallback(RunOneSimulation),(object)new SimulationParameters(ki,r));
				
					
				}
				
			}
			_doneEvent.WaitOne();
			
		}
		
		
		
		private static void RunOneSimulation(object o)
		{
			try
			{
				SimulationParameters PS = (SimulationParameters)o;
				
				int r = PS.sid;
				int ki = PS.ki;
				
				Tube tube = new Tube(TP,maxTime);
				SimulateTube SimulateTube = new SimulateTube(PS.sid);
				
				tube = SimulateTube.GrowToNmax(tube);
				int s;
				for(s=0;s<maxsycles;s++)
				{
					tube = SimulateTube.Kill(tube,KillTime[ki]);
					tube = SimulateTube.GrowToNmax(tube);
					
					//arize of mutant
					if(tube.LastN[1]>0)
					{
						Cycle2Mutant[ki,r]=s;
					}
					
					//test 4 fixsasion or extiction.
					if(((double)tube.LastN[1]/(tube.LastN[0]+tube.LastN[1])>0.7) || double.IsNaN((double)tube.LastN[1]/(tube.LastN[0]+tube.LastN[1])))
					{
						break;
					}
		
				}
				
				if(double.IsNaN((double)tube.LastN[1]/(tube.LastN[0]+tube.LastN[1])))
				   {
				   	Cycle2Fixsation[ki,r] = 0;
				   }
				   else
				   {
					Cycle2Fixsation[ki,r] = s;
				   }
				
				if(DebugPrint)
				{
					PrintTube2File("Lag=" + LagTS.ToString() + "Seed=" + r.ToString() + "Kill=" + KillTime[ki].ToString("0.00") ,tube);
				}
				
			}
			finally
			{
				
				//Console.WriteLine(numerOfThreadsNotYetCompleted);
				PrintPresentege(KillTime.Length*Repetitions-numerOfThreadsNotYetCompleted ,KillTime.Length*Repetitions);
				if (Interlocked.Decrement(ref numerOfThreadsNotYetCompleted) == 0)
				{
					_doneEvent.Set();
				}
			}
		}
		
		private struct SimulationParameters
		{
			public int ki;
			public int sid;
			public SimulationParameters( int ki, int sid)
			{
				this.ki = ki;
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
				HSR.WriteLine(KillTime[i]);
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

		#endregion

		#region PrintPresentege
		public static void PrintPresentege(int ind,int from)
		{
			//Console.WriteLine(Convert.ToInt32((double)ind/from*1000));
			int fraction = Convert.ToInt32((double)ind/from*1000);
			int postfraction = Convert.ToInt32((double)(ind-1)/from*1000);
			if ((fraction-postfraction)>=1)
			{
				Console.CursorLeft = 0;
				Console.Write("{0}% -{1} ",((double)fraction/10).ToString("00.0"),numerOfThreadsNotYetCompleted);
			}
		}
		
		public static void PrintPresentege(double ind,double from)
		{
			//Console.WriteLine(Convert.ToInt32((double)ind/from*1000));
			int fraction = Convert.ToInt32((double)ind/from*1000);
			int postfraction = Convert.ToInt32((double)(ind-1)/from*1000);
			
			Console.CursorLeft = 0;
			Console.Write("{0}%  ",((double)fraction/10).ToString("00.0"));
			
		}
		
		#endregion
	}
}
