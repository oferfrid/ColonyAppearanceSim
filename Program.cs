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
		static int  Seed;
		static bool DebugPrint;
		
		public static void Main(string[] args)
		{
			Seed = System.Convert.ToInt32(args[0]);
			DebugPrint =  System.Convert.ToBoolean(args[1]);
			Utils.Init(Seed);
			
			Console.WriteLine("start");
			DateTime start = DateTime.Now;
			RunSim();
			Console.WriteLine();
			TimeSpan TS = DateTime.Now - start;
			Console.WriteLine("Ended in {0} seconds",TS.TotalSeconds);
			
			
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
		

		
		private static void RunSim()
		{
			
			
			Console.WriteLine("Start");
			//	Utils.Init(1);
			
			// Simulation Parameter
			double maxTime = 1e5 ;
			
			TubeParameters TP = new TubeParameters(1e6,new StrainParameters[]{ new StrainParameters("Hip",1e4,0.1,20,1000,21,3),new StrainParameters("WT",1e4,0.001,20,1000,21,3)});
			
			int res = 20;
			int maxsycles = 50;
			
			double[] KillTime =new double[res];
			double[] Dilution =new double[res];
			
			double[] KillFromTo = {0,60};
			double[] DilutionFromTo = {1,300};
			
			
			for(int i=0;i<KillTime.Length;i++)
			{
				KillTime[i] = KillFromTo[0] + (KillFromTo[1] - KillFromTo[0])*i/(res-1);
			}
			for(int i=0;i<Dilution.Length;i++)
			{
				Dilution[i] = DilutionFromTo[0] + (DilutionFromTo[1] - DilutionFromTo[0])*i/(res-1);
			}
			
			
			double[,] Extinction = new double[KillTime.Length,Dilution.Length];
			
			
			for(int ki=0;ki<KillTime.Length;ki++)
			{
				for(int di=0;di<Dilution.Length;di++)
				{
					PrintPresentege(di+Dilution.Length*ki ,KillTime.Length*Dilution.Length);
					
					Tube tube = new Tube(TP,maxTime);
					tube = SimulateTube.GrowToNmax(tube);
					int s;
					Extinction[ki,di] = 0;
					if (!(ki==0 & di==0))
					{
						for(s=0;s<maxsycles;s++)
						{
							tube = SimulateTube.Dilut(tube,1.0/Dilution[di]);
							tube = SimulateTube.Kill(tube,KillTime[ki]);
							tube = SimulateTube.GrowToNmax(tube);
							if(tube.LastN[0]/(tube.LastN[0] + tube.LastN[1])>0.8)
							{
								Extinction[ki,di] = (100.0-s)/100;
								break;
							}
							
							if(tube.LastN[0]/(tube.LastN[0] + tube.LastN[1])<0.2)
							{
								Extinction[ki,di] = -(100.0-s)/100;
								break;
							}
						}
						if(DebugPrint)
						{
							PrintTube2File("Seed=" + Seed.ToString() + "Kill=" + KillTime[ki].ToString("0.0") + "Dilution=" + Dilution[di].ToString("0.0"),tube);
						}
					}
				}
			}
			
			Print2DMat2File("Seed=" + Seed.ToString() + "Mat",Extinction);
			Print2DMatH2File("Seed=" + Seed.ToString() + "Mat_H",Extinction,KillTime,Dilution);
			Console.Beep(800,1000);
			Console.Beep(800,1000);
		}
		

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
