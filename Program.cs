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
		
		 
		
			
		static int numerOfThreadsNotYetCompleted = 0;
		private static ManualResetEvent _doneEvent = new ManualResetEvent(false);
		
		public static void Main(string[] args)
		{
			//read cmd params
			//Seed = System.Convert.ToInt32(args[0]);
			//DebugPrint =  System.Convert.ToBoolean(args[1]);
			
			//SimulationParameters PS = (SimulationParameters)o;
				
			
			double[] lag = new double[100];
			for(int i=0;i<lag.Length;i++)
			{
				lag[i] = ((8.0*60.0 - 30.0)/(20-1))*i + 30.0;
			}
			
			
			
			string Filename=  @"time2grow_4H.txt";
			System.IO.StreamWriter SR = new StreamWriter(Filename, false);
			int NumberOfSimulations = 10;
			
			for(int i=0;i<lag.Length;i++)
			{
				TubeParameters TP = new TubeParameters(1e7,new StrainParameters[]{
				                                       	new StrainParameters("WT",1e5,0,lag[i],1000,21,3)
		                                              });
				
				SR.Write(lag[i]);
				for(int s=0;s<NumberOfSimulations;s++)
				{
				Tube tube = new Tube(TP,maxTime);
				SimulateTube SimulateTube = new SimulateTube(s);
				double KillTime =240;
				tube = SimulateTube.Kill(tube,KillTime);
				tube = SimulateTube.GrowToNmax(tube);
				SR.Write("\t{0}",tube.LastT);
				
				    //PrintTube2File(@"test" + i.ToString(),tube);
				    PrintPresentege(i*NumberOfSimulations + s,lag.Length*NumberOfSimulations);
				}
				SR.WriteLine();
			}
			
			
			SR.Close();

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
