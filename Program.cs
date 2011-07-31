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
		static int  Seed;
		static bool DebugPrint;
		static double maxTime = 1e5 ;
		
		static TubeParameters TP = new TubeParameters(1e6,new StrainParameters[]{ new StrainParameters("Hip",1e4,0.1,20,1000,21,3),new StrainParameters("WT",1e4,0.001,20,1000,21,3)});
		
		static int res = 100;
		static int maxsycles = 40;
		
		static double[] KillTime;
		static double[] Dilution ;
		static double[,] Extinction;
		
		
		
		
		static int numerOfThreadsNotYetCompleted = 0;
		private static ManualResetEvent _doneEvent = new ManualResetEvent(false);
		
		public static void Main(string[] args)
		{
			//read cmd params
			Seed = System.Convert.ToInt32(args[0]);
			DebugPrint =  System.Convert.ToBoolean(args[1]);
			
			
			//init global vars
			KillTime =new double[res];
			Dilution =new double[res];
			
			double[] KillFromTo = {0,50};
			double[] DilutionFromTo = {1,10};
			
			
			for(int i=0;i<KillTime.Length;i++)
			{
				KillTime[i] = KillFromTo[0] + (KillFromTo[1] - KillFromTo[0])*i/(KillTime.Length-1);
			}
			for(int i=0;i<Dilution.Length;i++)
			{
				Dilution[i] = DilutionFromTo[0] + (DilutionFromTo[1] - DilutionFromTo[0])*i/(Dilution.Length-1);
			}
			
			SimulateTube SimulateTube = new SimulateTube(Seed);
			Extinction = new double[KillTime.Length,Dilution.Length];
			
			DateTime start = DateTime.Now;
			RunSimParalel();
			//RunSim();
			
			Print2DMat2File("Seed=" + Seed.ToString() + "Mat",Extinction);
			Print2DMatH2File("Seed=" + Seed.ToString() + "Mat_H",Extinction,KillTime,Dilution);
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
				for(int di=0;di<Dilution.Length;di++)
				{
					Extinction[ki,di] = 0.5;
					
					if (!(ki==0 & di==0))
					{
						
						Interlocked.Increment(ref numerOfThreadsNotYetCompleted);
						ThreadPool.QueueUserWorkItem(new WaitCallback(RunOneSimulation),(object)new SimulationParameters(ki,di));
					}

					
				}
				
			}
			_doneEvent.WaitOne();
			
		}
		
		
		private static void RunSim()
		{
			
			SimulateTube SimulateTube = new SimulateTube(Seed);
			for(int ki=0;ki<KillTime.Length;ki++)
			{
				for(int di=0;di<Dilution.Length;di++)
				{
					
					Tube tube = new Tube(TP,maxTime);
					tube = SimulateTube.GrowToNmax(tube);
					int s;
					Extinction[ki,di] = 0;
					if (!(ki==0 & di==0))
					{

						
						for(s=0;s<maxsycles;s++)
						{
							PrintPresentege((di+Dilution.Length*ki)*maxsycles + s ,KillTime.Length*Dilution.Length*maxsycles);
							tube = SimulateTube.Dilut(tube,1.0/Dilution[di]);
							tube = SimulateTube.Kill(tube,KillTime[ki]);
							tube = SimulateTube.GrowToNmax(tube);
						}
						
						Extinction[ki,di] = (double)tube.LastN[0]/(tube.LastN[0]+tube.LastN[1]);
						
						if(DebugPrint)
						{
							PrintTube2File("Seed=" + Seed.ToString() + "Kill=" + KillTime[ki].ToString("0.0") + "Dilution=" + Dilution[di].ToString("0.0"),tube);
						}
					}
					
				}
			}
			
		}
		
		
		
		private static void RunOneSimulation(object o)
		{
			try
			{
				SimulationParameters PS = (SimulationParameters)o;
				
				int di = PS.di;
				int ki = PS.ki;
				
				Tube tube = new Tube(TP,maxTime);
				SimulateTube SimulateTube = new SimulateTube(Seed);
				
				tube = SimulateTube.GrowToNmax(tube);
				int s;

				

				
				for(s=0;s<maxsycles;s++)
				{
					tube = SimulateTube.Dilut(tube,1.0/Dilution[di]);
					tube = SimulateTube.Kill(tube,KillTime[ki]);
					tube = SimulateTube.GrowToNmax(tube);
				}
				
				Extinction[ki,di] = (double)tube.LastN[0]/(tube.LastN[0]+tube.LastN[1]);
				
				if(DebugPrint)
				{
					PrintTube2File("Seed=" + Seed.ToString() + "Kill=" + KillTime[ki].ToString("0.0") + "Dilution=" + Dilution[di].ToString("0.0"),tube);
				}
				
			}
			finally
			{
				
				//Console.WriteLine(numerOfThreadsNotYetCompleted);
				PrintPresentege(KillTime.Length*Dilution.Length-numerOfThreadsNotYetCompleted ,KillTime.Length*Dilution.Length);
				if (Interlocked.Decrement(ref numerOfThreadsNotYetCompleted) == 0)
				{
					_doneEvent.Set();
				}
			}
		}
		
		private struct SimulationParameters
		{
			public int ki;
			public int di;
			public SimulationParameters( int ki, int di)
			{
				this.ki = ki;
				this.di = di;
			}
		}
		
		
		
		#region Print2file
		private static void Print2DMat2File(string Filename,double[,] Mat)
		{
			
			Filename+=  ".txt";
			System.IO.StreamWriter SR = new StreamWriter(Filename, false);

			
			for (int i=0; i<Mat.GetLength(0); i++)
			{
				for (int j=0; j<Mat.GetLength(1); j++)
				{
					SR.Write("{0}\t",Mat[i,j]);
				}
				SR.WriteLine();
			}
			SR.Close();
		}

		private static void Print2DMatH2File(string Filename,double[,] Mat,double[] HeadInd0,double[] HeadInd1)
		{
			
			Filename+=  ".txt";
			System.IO.StreamWriter SR = new StreamWriter(Filename, false);
			SR.Write("0\t");
			for (int j=0; j<Mat.GetLength(1); j++)
			{
				SR.Write("{0}\t",HeadInd1[j]);
			}
			SR.WriteLine();
			
			
			
			for (int i=0; i<Mat.GetLength(0); i++)
			{
				SR.Write("{0}\t",HeadInd0[i]);
				for (int j=0; j<Mat.GetLength(1); j++)
				{
					SR.Write("{0}\t",Mat[i,j]);
				}
				SR.WriteLine();
			}
			SR.Close();
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
				Console.Write("{0}%  ",((double)fraction/10).ToString("00.0"));
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
