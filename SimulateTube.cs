/*
 * Created by SharpDevelop.
 * User: oferfrid
 * Date: 02/08/2009
 * Time: 12:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace IritSimulation
{
	/// <summary>
	/// Description of SimulateTube.
	/// </summary>
	public  class SimulateTube
	{
		Utils Utils;
		
		
		public SimulateTube(		int seed)
		{
			Utils =new Utils(seed);
		}
		
		private   Tube CommuteLag(Tube T,double GrowKill)
			
		{
			//Get LagTime for each bacteria strain
			for(int s=0;s<T.TP.NumberOfStrains;s++)
			{
				//Calc Number of normal cels
				double N0Persisters = Math.Round((double)T.LastN[s]*T.TP.Strains[s].PersistersLevel);
				double N0Normal = T.LastN[s] - N0Persisters;
				
				double mulagNormal = 1.0/T.TP.Strains[s].LagMeanNormal;
				double mulagPersisters = 1.0/T.TP.Strains[s].LagMeanPersisters;
				
				//put normal first divition
				for(int i=0;i<N0Normal;i++)
				{
					double LagTime = Utils.RandDecayExponantial(mulagNormal) ;
					int DivInd = GetIndFromdouble(LagTime,T.dt);
					T.GrowDivision[DivInd+T.LastT,s]+=GrowKill;
				}
				//put Persisters first divition
				for(int i=0;i<N0Persisters;i++)
				{
					double LagTime = Utils.RandDecayExponantial(mulagPersisters);
					int DivInd = GetIndFromdouble(LagTime,T.dt);
					T.GrowDivision[DivInd+T.LastT,s]+=GrowKill;
				}
			}
			return T;
			
		}
		
		public   Tube CommuteLagForGrow(Tube T)
		{
			return CommuteLag( T, 1);
		}
		public   Tube CommuteLagForKill(Tube T)
		{
			return CommuteLag( T,-1);
		}
		
		public  Tube Kill(Tube T,double KillTime)
		{
			//usin kill whan bacteria divides
			CommuteLagForKill(T);
			int KillTimeInIndexs = (int)Math.Round(KillTime/T.dt)  ;
			
			//zero future divitions
			for(int tt = T.LastT + KillTimeInIndexs; tt<T.GrowDivision.GetLength(0);tt++)
			{
				for (int s=0;s<T.TP.NumberOfStrains;s++)
				{
					T.GrowDivision[tt,s]=0;
				}
			}
			
			T.LastT += KillTimeInIndexs;
			
			return T;
		}
		
		public static Tube Dilut(Tube T,double ratio)
		{
		
			
			for (int s=0;s<T.TP.NumberOfStrains;s++)
				{
				 double  NewN = Utils.RandBinomial(T.LastN[s],ratio);
				 T.GrowDivision[T.LastT,s]-=T.LastN[s] - NewN;
				}
			
			T.LastT++;
		
		return T;
		}
		
		
			public  Tube GrowToNmax(Tube T)
		{
			double[] N = new double[T.TP.NumberOfStrains];
			double NTot = 0;
			CommuteLagForGrow(T);
			for (int s=0;s<T.TP.NumberOfStrains;s++)
			{
				N[s]=T.LastN[s];
			}
			int t=T.LastT;
			
			do
			{
				NTot = 0;
				for (int s=0;s<T.TP.NumberOfStrains;s++)
				{
					
					for(int i=0;i<T.GrowDivision[t,s];i++)
					{
						
						//add the next 2 divisions
						int ind;
						ind = GetDivisionTimeIndex(T.TP.Strains[s].DivLognormalParameters,T.dt);
						T.GrowDivision[t+ind,s]++;
						ind = GetDivisionTimeIndex(T.TP.Strains[s].DivLognormalParameters,T.dt);
						T.GrowDivision[t+ind,s]++;
						N[s]++;
						
					}
					
					NTot +=N[s];
				}
				t++;
				
			}while (NTot<T.TP.Nmax);
			
			T.LastT = t-1;
			
			//zero future divitions
			for(int tt = t; tt<T.GrowDivision.GetLength(0);tt++)
			{
				for (int s=0;s<T.TP.NumberOfStrains;s++)
				{
					T.GrowDivision[tt,s]=0;
				}
			}
			
			return T;
		}
		
		public static Tube GrowToTime(Tube T,double Time)
		{
			int GrowTimeInIndexs = (int)Math.Round(Time/T.dt)  ;
			int EndGrowingIndex = GrowTimeInIndexs + T.LastT;
			
			
			double[] N = new double[T.TP.NumberOfStrains];
			double NTot = 0;
			CommuteLagForGrow(T);
			
			for (int s=0;s<T.TP.NumberOfStrains;s++)
			{
				N[s]=T.LastN[s];
			}
			int t=T.LastT;
			
			do
			{
				NTot = 0;
				for (int s=0;s<T.TP.NumberOfStrains;s++)
				{
					
					for(int i=0;i<T.GrowDivision[t,s];i++)
					{
						
						//add the next 2 divisions
						int ind;
						ind = GetDivisionTimeIndex(T.TP.Strains[s].DivLognormalParameters,T.dt);
						T.GrowDivision[t+ind,s]++;
						ind = GetDivisionTimeIndex(T.TP.Strains[s].DivLognormalParameters,T.dt);
						T.GrowDivision[t+ind,s]++;
						N[s]++;
						
					}
					
					NTot +=N[s];
				}
				t++;
				
			}while (t<EndGrowingIndex);
			
			T.LastT = t-1;
			
			//zero future divitions
			for(int tt = t; tt<T.GrowDivision.GetLength(0);tt++)
			{
				for (int s=0;s<T.TP.NumberOfStrains;s++)
				{
					T.GrowDivision[tt,s]=0;
				}
			}
			
			return T;
		}
		
		private  int GetDivisionTimeIndex(Utils.LognormalParameters LP,double dt)
		{
			
			double DivTime = Utils.RandLogNormal(LP);
			
			int DivInd = GetIndFromdouble(DivTime,dt);
			return DivInd;
		}
		
		private  int GetIndFromdouble(double Time,double dt)
		{
			int ind;
			if (Time == double.PositiveInfinity)
			{
				throw new Exception("PositiveInfinity!");
			}
			else
			{
				ind = Convert.ToInt32(Math.Round((Time/dt)));
			}
			if (ind<0)
			{
				throw new Exception("Negative index!");
			}
			return ind;
		}
		
		
		
	}
}

